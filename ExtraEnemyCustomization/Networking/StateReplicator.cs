using EECustom.Events;
using GTFO.API;
using SNetwork;
using System;
using System.Collections.Generic;

namespace EECustom.Networking
{
    public struct ReplicatorPayload<S> where S : struct
    {
        public ushort key;
        public S state;
    }

    public abstract class StateReplicator<S> where S : struct
    {
        private static readonly List<ushort> _listeningKeys = new();
        private static readonly Dictionary<ushort, Action<S>> _onStateChangeLookup = new();
        private static readonly Dictionary<ushort, S> _stateDataLookup = new();

        public abstract bool ClearOnLevelCleanup { get; }

        public bool IsSetup { get => _isInitialized; }
        public string SetStateName { get; private set; } = string.Empty;
        public string ChangeRequestName { get; private set; } = string.Empty;

        private bool _isInitialized = false;

        public void Initialize()
        {
            if (_isInitialized)
                return;

            SNetEvents.AgentSpawned += SNetEvents_AgentSpawned;
            if (ClearOnLevelCleanup)
            {
                LevelEvents.LevelCleanup += Clear;
            }

            SetStateName = $"EEC_StateReplicator_{typeof(S).Name}SS";
            ChangeRequestName = $"EEC_StateReplicator_{typeof(S).Name}CR";
            NetworkAPI.RegisterEvent<ReplicatorPayload<S>>(SetStateName, ReceiveSetState_FromMaster);
            NetworkAPI.RegisterEvent<ReplicatorPayload<S>>(ChangeRequestName, ReceiveSetState_FromClient);
            _isInitialized = true;
        }

        private void SNetEvents_AgentSpawned(SNet_Player player)
        {
            if (SNet.IsMaster)
            {
                foreach (var state in _stateDataLookup)
                {
                    var newState = new ReplicatorPayload<S>()
                    {
                        key = state.Key,
                        state = state.Value
                    };

                    NetworkAPI.InvokeEvent(SetStateName, newState, player, SNet_ChannelType.GameOrderCritical);
                }
            }
        }

        public void Register(ushort id, S startState, Action<S> onChanged = null)
        {
            if (IsRegistered(id))
                return;

            _listeningKeys.Add(id);
            _onStateChangeLookup[id] = onChanged;
            if (_stateDataLookup.TryGetValue(id, out var savedState))
            {
                _onStateChangeLookup[id]?.Invoke(savedState);
                OnStateChange(id, savedState);
            }
            else
            {
                _stateDataLookup[id] = startState;
                _onStateChangeLookup[id]?.Invoke(startState);
                OnStateChange(id, startState);
            }
        }

        public void Deregister(ushort id)
        {
            if (!IsRegistered(id))
                return;

            _listeningKeys.Remove(id);
            _onStateChangeLookup.Remove(id);
            _stateDataLookup.Remove(id);
        }

        private void Clear()
        {
            _listeningKeys.Clear();
            _onStateChangeLookup.Clear();
            _stateDataLookup.Clear();
        }

        public void SetState(ushort id, S state)
        {
            if (!IsRegistered(id))
                return;

            var newState = new ReplicatorPayload<S>()
            {
                key = id,
                state = state
            };

            if (SNet.IsMaster)
            {
                NetworkAPI.InvokeEvent(SetStateName, newState, SNet_ChannelType.GameOrderCritical);
                _stateDataLookup[id] = state;

                ReceiveSetState_FromMaster(SNet.Master.Lookup, newState);
            }
            else if (SNet.HasMaster)
            {
                NetworkAPI.InvokeEvent(ChangeRequestName, newState, SNet.Master, SNet_ChannelType.GameOrderCritical);
                _stateDataLookup[id] = state;
            }
        }

        public bool TryGetState(ushort id, out S state)
        {
            if (!IsRegistered(id))
            {
                Logger.Error($"KEY: {id} has not registered!");
                state = default;
                return false;
            }

            if (!_stateDataLookup.TryGetValue(id, out state))
            {
                Logger.Error($"KEY: {id} has not registered!");
                return false;
            }
            return true;
        }

        private void ReceiveSetState_FromMaster(ulong sender, ReplicatorPayload<S> statePacket)
        {
            var key = statePacket.key;
            var newState = statePacket.state;
            if (_stateDataLookup.TryGetValue(key, out var savedState))
            {
                if (IsRegistered(statePacket.key))
                {
                    _onStateChangeLookup[key]?.Invoke(newState);
                    OnStateChange(key, newState);
                }
                _stateDataLookup[key] = newState;
            }
            _stateDataLookup[key] = newState;
        }

        private void ReceiveSetState_FromClient(ulong sender, ReplicatorPayload<S> statePacket)
        {
            if (!SNet.IsMaster)
                return;

            SetState(statePacket.key, statePacket.state);
        }

        public bool IsRegistered(ushort id)
        {
            return _listeningKeys.Contains(id);
        }

        public virtual void OnStateChange(ushort id, S newState)
        {
        }
    }
}