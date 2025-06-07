using GTFO.API;
using Player;
using SNetwork;
using System;
using System.Runtime.InteropServices;

namespace EEC.Networking
{
    internal struct SyncedPlayerEventPayload
    {
        public ulong lookup;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public byte[] packetBytes;

        public void Serialize<T>(T packet) where T : struct
        {
            int size = Marshal.SizeOf(packet);

            if (size >= 30)
            {
                throw new ArgumentException("PacketData Exceed size of 30 : Unable to Serialize", nameof(T));
            }

            byte[] bytes = new byte[30];

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(packet, ptr, false);
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);

            packetBytes = bytes;
        }

        public T Deserialize<T>()
        {
            int size = Marshal.SizeOf(typeof(T));

            if (size > packetBytes.Length)
            {
                throw new ArgumentException("Packet Exceed size of 30 : Unable to Deserialize", nameof(T));
            }

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(packetBytes, 0, ptr, size);
            T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return obj;
        }

        public bool TryGetPlayer(out SNet_Player player)
        {
            if (lookup == 0UL)
            {
                player = null;
                return false;
            }
            if (lookup < NetworkManager.LOWEST_STEAMID64)
            {
                return SNet.Core.TryGetPlayerBot((int)lookup - 1, out player);
            }
            return SNet.Core.TryGetPlayer(lookup, out player, false);
        }
    }

    public abstract class SyncedPlayerEvent<T> where T : struct
    {
        public delegate void ReceiveHandler(T packet, SNet_Player receivedPlayer);

        public abstract string GUID { get; }
        public abstract bool SendToTargetOnly { get; }
        public abstract bool AllowBots { get; }
        public bool IsSetup { get => _isSetup; }
        public string EventName { get; private set; } = string.Empty;

        private bool _isSetup = false;

        public void Setup()
        {
            if (_isSetup)
                return;

            EventName = $"EECp{GUID}";
            NetworkAPI.RegisterEvent<SyncedPlayerEventPayload>(EventName, Received_Callback);
            _isSetup = true;
        }

        public bool TryGetPlayerAgent(SNet_Player player, out PlayerAgent agent)
        {
            if (!player.HasPlayerAgent)
            {
                agent = null;
                return false;
            }

            agent = player.m_playerAgent.TryCast<PlayerAgent>();
            return agent != null;
        }

        public void SendToPlayers(T packetData, params PlayerAgent[] agents)
        {
            foreach (var agent in agents)
            {
                if (agent == null || agent.Owner == null)
                    continue;

                SendToPlayer(packetData, agent.Owner);
            }
        }

        public void SendToPlayers(T packetData, params SNet_Player[] players)
        {
            foreach (var player in players)
            {
                SendToPlayer(packetData, player);
            }
        }

        public void SendToPlayer(T packetData, PlayerAgent agent)
        {
            if (agent == null || agent.Owner == null)
                return;

            SendToPlayer(packetData, agent.Owner);
        }

        public void SendToPlayer(T packetData, SNet_Player player)
        {
            if (player == null)
            {
                Logger.Error($"{GetType().Name} - SyncedPlayerEvent player was null!");
                return;
            }

            if (player.Lookup == 0)
            {
                Logger.Error($"{GetType().Name} - SyncedPlayerEvent lookup for player was 0!");
                return;
            }

            var payload = new SyncedPlayerEventPayload()
            {
                lookup = player.Lookup
            };
            payload.Serialize(packetData);

            if (player.IsBot)
            {
                if (!AllowBots)
                    return;

                if (SNet.IsMaster)
                {
                    Received_Callback(SNet.Master.Lookup, payload);
                }
                else if (SNet.HasMaster)
                {
                    NetworkAPI.InvokeEvent(EventName, payload, SNet.Master);
                }
            }
            else
            {
                if (SendToTargetOnly)
                {
                    if (player.IsLocal)
                    {
                        Received_Callback(player.Lookup, payload);
                    }
                    else
                    {
                        NetworkAPI.InvokeEvent(EventName, payload, player);
                    }
                }
                else
                {
                    NetworkAPI.InvokeEvent(EventName, payload);
                    Received_Callback(player.Lookup, payload);
                }
            }
        }

        private void Received_Callback(ulong sender, SyncedPlayerEventPayload payload)
        {
            if (!payload.TryGetPlayer(out var player))
                return;

            bool shouldProcess;
            if (player.IsBot)
            {
                shouldProcess = AllowBots;
            }
            else
            {
                if (player.IsLocal)
                {
                    shouldProcess = true;
                }
                else
                {
                    shouldProcess = !SendToTargetOnly;
                }
            }

            if (shouldProcess)
            {
                Received(payload.Deserialize<T>(), player);
            }
        }

        private void Received(T packet, SNet_Player receivedPlayer)
        {
            Receive(packet, receivedPlayer);
            OnReceive?.Invoke(packet, receivedPlayer);
        }

        protected virtual void Receive(T packet, SNet_Player receivedPlayer)
        {
        }

        public event ReceiveHandler OnReceive;
    }
}