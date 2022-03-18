using EEC.Events;
using GTFO.API;
using SNetwork;
using System;
using System.Collections.Generic;

namespace EEC.Networking
{
    public struct ReplicatorPayload<S> where S : struct
    {
        public ushort key;
        public S state;
    }

    public sealed class StateContext<S> where S : struct
    {
        public bool Registered = false;
        public Action<S> OnStateChanged = null;
        public S State;
    }

    public abstract class StateReplicator<S> where S : struct
    {
        private static readonly Dictionary<ushort, StateContext<S>> _lookup = new(500);

        public abstract bool ClearOnLevelCleanup { get; }

        public abstract string GUID { get; }
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

            SetStateName = $"EECRp{GUID}S";
            ChangeRequestName = $"EECRp{GUID}R";
            NetworkAPI.RegisterEvent<ReplicatorPayload<S>>(SetStateName, ReceiveSetState_FromMaster);
            NetworkAPI.RegisterEvent<ReplicatorPayload<S>>(ChangeRequestName, ReceiveSetState_FromClient);
            _isInitialized = true;
        }

        private void SNetEvents_AgentSpawned(SNet_Player player)
        {
            if (SNet.IsMaster)
            {
                foreach (var state in _lookup)
                {
                    var newState = new ReplicatorPayload<S>()
                    {
                        key = state.Key,
                        state = state.Value.State
                    };

                    NetworkAPI.InvokeEvent(SetStateName, newState, player, SNet_ChannelType.GameOrderCritical);
                }
            }
        }

        public void Register(ushort id, S startState, Action<S> onChanged = null)
        {
            if (TryGetContext(id, out var context))
            {
                if (context.Registered)
                    return;

                context.Registered = true;
                context.OnStateChanged = onChanged;
            }
            else
            {
                context = new StateContext<S>
                {
                    Registered = true,
                    OnStateChanged = onChanged,
                    State = startState
                };
                _lookup[id] = context;
            }

            context.OnStateChanged?.Invoke(context.State);
            OnStateChange(id, context.State);
        }

        public void Deregister(ushort id)
        {
            if (TryGetContext(id, out var context))
            {
                if (!context.Registered)
                    return;

                _lookup.Remove(id);
            }
        }

        private void Clear()
        {
            _lookup.Clear();
        }

        public void SetState(ushort id, S state)
        {
            if (!TryGetContext(id, out var context) || !context.Registered)
                return;

            var newState = new ReplicatorPayload<S>()
            {
                key = id,
                state = state
            };

            if (SNet.IsMaster)
            {
                NetworkAPI.InvokeEvent(SetStateName, newState, SNet_ChannelType.GameOrderCritical);
                context.State = state;

                ReceiveSetState_FromMaster(SNet.Master.Lookup, newState);
            }
            else if (SNet.HasMaster)
            {
                NetworkAPI.InvokeEvent(ChangeRequestName, newState, SNet.Master, SNet_ChannelType.GameOrderCritical);
                context.State = state;
            }
        }

        public bool TryGetState(ushort id, out S state)
        {
            if (!TryGetContext(id, out var context) || !context.Registered)
            {
                Logger.Error($"KEY: {id} has not registered!");
                state = default;
                return false;
            }

            state = context.State;
            return true;
        }

        private void ReceiveSetState_FromMaster(ulong sender, ReplicatorPayload<S> statePacket)
        {
            var key = statePacket.key;
            var newState = statePacket.state;
            if (TryGetContext(key, out var context))
            {
                context.State = newState;

                if (context.Registered)
                {
                    context.OnStateChanged?.Invoke(newState);
                    OnStateChange(key, newState);
                }
            }
            else
            {
                _lookup[key] = new StateContext<S>()
                {
                    Registered = false,
                    State = newState
                };
            }
        }

        private void ReceiveSetState_FromClient(ulong sender, ReplicatorPayload<S> statePacket)
        {
            if (!SNet.IsMaster)
                return;

            SetState(statePacket.key, statePacket.state);
        }

        public bool TryGetContext(ushort id, out StateContext<S> context)
        {
            return _lookup.TryGetValue(id, out context);
        }

        public virtual void OnStateChange(ushort id, S newState)
        {
        }
    }
}