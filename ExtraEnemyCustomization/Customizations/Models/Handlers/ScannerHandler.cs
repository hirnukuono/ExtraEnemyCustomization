using Agents;
using EECustom.Attributes;
using Enemies;
using SNetwork;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.Customizations.Models.Handlers
{
    public enum EnemyState
    {
        Initial,
        Hibernate,
        Detect,
        Heartbeat,
        Wakeup,
        Scout,
        ScoutDetect
    }

    [InjectToIl2Cpp]
    public sealed class ScannerHandler : MonoBehaviour
    {
        public EnemyAgent OwnerAgent;
        public Color DefaultColor;
        public Color WakeupColor;
        public Color DetectionColor;
        public Color HeartbeatColor;
        public Color PatrolColor;
        public Color FeelerColor;

        public bool UsingDetectionColor = false;
        public bool UsingScoutColor = false;

        public float InterpDuration = 0.5f;

        private AgentMode _agentMode = AgentMode.Off;
        private EnemyState _currentState = EnemyState.Hibernate;
        private bool _interpDone = true;
        private float _interpTimer = 0.0f;
        private float _interpStartTime = 0.0f;
        private Color _previousColor = Color.white;

        internal void Start()
        {
            SetAgentMode_Master(OwnerAgent.AI.Mode);
            UpdateState(out _currentState);

            _previousColor = GetStateColor(_currentState);
            OwnerAgent.ScannerColor = _previousColor;
        }

        internal void Update()
        {
            if (SNet.IsMaster)
            {
                var currentMode = OwnerAgent.AI.Mode;
                if (_agentMode != currentMode)
                {
                    SetAgentMode_Master(currentMode);
                }
            }

            UpdateState(out var state);

            if (_currentState != state)
            {
                _currentState = state;
                _interpDone = false;
                _interpTimer = Clock.Time + InterpDuration;
                _interpStartTime = Clock.Time;
                _previousColor = OwnerAgent.m_scannerColor;
            }

            if (!_interpDone)
            {
                if (Clock.Time >= _interpTimer)
                {
                    OwnerAgent.ScannerColor = GetStateColor(_currentState);
                    _interpDone = true;
                    return;
                }

                var progress = Mathf.InverseLerp(_interpStartTime, _interpTimer, Clock.Time);
                var color1 = _previousColor;
                var color2 = GetStateColor(_currentState);
                var newColor = Color.Lerp(color1, color2, progress);
                OwnerAgent.ScannerColor = newColor;
            }
        }

        internal void OnDestroy()
        {
            OwnerAgent = null;
        }

        [HideFromIl2Cpp]
        internal void SetAgentMode_Master(AgentMode mode)
        {
            if (SNet.IsMaster)
            {
                ScannerCustom._sync.SetState(OwnerAgent.GlobalID, new ScannerStatusPacket()
                {
                    mode = mode
                });

                _agentMode = mode;
            }
        }

        [HideFromIl2Cpp]
        internal void UpdateAgentMode(AgentMode mode)
        {
            _agentMode = mode;
        }

        [HideFromIl2Cpp]
        private void UpdateState(out EnemyState state)
        {
            switch (_agentMode)
            {
                case AgentMode.Hibernate:
                    if (!UsingDetectionColor)
                    {
                        state = EnemyState.Hibernate;
                        return;
                    }

                    if (OwnerAgent.IsHibernationDetecting)
                    {
                        if (OwnerAgent.Locomotion.Hibernate.m_heartbeatActive)
                        {
                            state = EnemyState.Heartbeat;
                            return;
                        }
                        else
                        {
                            state = EnemyState.Detect;
                            return;
                        }
                    }
                    else if (OwnerAgent.Locomotion.CurrentStateEnum == ES_StateEnum.HibernateWakeUp)
                    {
                        state = EnemyState.Wakeup;
                        return;
                    }
                    else
                    {
                        state = EnemyState.Hibernate;
                        return;
                    }

                case AgentMode.Agressive:
                    state = EnemyState.Wakeup;
                    return;

                case AgentMode.Scout:
                    if (!UsingScoutColor)
                    {
                        state = EnemyState.Wakeup;
                        return;
                    }

                    if (OwnerAgent.Locomotion.CurrentStateEnum == ES_StateEnum.ScoutScream)
                    {
                        state = EnemyState.Wakeup;
                        return;
                    }

                    var detection = OwnerAgent.Locomotion.ScoutDetection.m_antennaDetection;
                    if (detection == null)
                    {
                        state = EnemyState.Scout;
                        return;
                    }

                    if (detection.m_wantsToHaveTendrils)
                    {
                        state = EnemyState.ScoutDetect;
                        return;
                    }
                    else
                    {
                        state = EnemyState.Scout;
                        return;
                    }

                default:
                    state = EnemyState.Hibernate;
                    return;
            }
        }

        [HideFromIl2Cpp]
        private Color GetStateColor(EnemyState state)
        {
            return state switch
            {
                EnemyState.Hibernate => DefaultColor,
                EnemyState.Detect => DetectionColor,
                EnemyState.Heartbeat => HeartbeatColor,
                EnemyState.Wakeup => WakeupColor,
                EnemyState.Scout => PatrolColor,
                EnemyState.ScoutDetect => FeelerColor,
                _ => DefaultColor
            };
        }
    }
}