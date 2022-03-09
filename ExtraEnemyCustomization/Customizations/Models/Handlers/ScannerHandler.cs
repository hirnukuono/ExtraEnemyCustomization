using Agents;
using EECustom.Attributes;
using EECustom.Utils;
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
    internal sealed class ScannerHandler : MonoBehaviour
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
        public float UpdateInterval = 0.05f;

        public bool OptimizeOnAwake = true;

        private AgentMode _agentMode = AgentMode.Off;
        private EnemyState _currentState = EnemyState.Initial;
        private bool _interpDone = true;
        private Timer _interpTimer;
        private Timer _updateTimer;
        private Color _previousColor = Color.white;
        private Color _doneColor = Color.white;
        private bool _disableScriptAfterDone = false;
        private bool _isSetup = false;

        [HideFromIl2Cpp]
        internal void Setup()
        {
            UpdateState(out var state);
            OwnerAgent.ScannerColor = GetStateColor(state);

            if (_disableScriptAfterDone)
            {
                enabled = false;
                _disableScriptAfterDone = false;
            }

            _isSetup = true;
        }

        internal void Update()
        {
            if (!_isSetup)
                return;

            EnemyState state;
            if (!OwnerAgent.IsInDetectedList)
            {
                //NOT Visible on Bio-Tracker

                if (OwnerAgent.UpdateMode != NodeUpdateMode.Close)
                    return;

                if (!_updateTimer.TickAndCheckDone())
                    return;

                UpdateState(out state);
                if (_currentState != state)
                {
                    _currentState = state;
                    _interpDone = true;
                    _interpTimer.Reset(InterpDuration);
                    OwnerAgent.ScannerColor = GetStateColor(state);
                }
                return;
            }

            //Visible on Bio-Tracker

            if (!_updateTimer.TickAndCheckDone())
                return;

            UpdateState(out state);
            if (_currentState != state)
            {
                _currentState = state;
                _previousColor = OwnerAgent.m_scannerColor;
                _doneColor = GetStateColor(state);
                _interpDone = false;
                _interpTimer.Reset(InterpDuration);
            }

            if (!_interpDone)
            {
                if (_interpTimer.TickAndCheckDone(_updateTimer.PassedTime))
                {
                    OwnerAgent.ScannerColor = _doneColor;
                    _interpDone = true;

                    if (_disableScriptAfterDone)
                    {
                        _disableScriptAfterDone = false;
                        enabled = false;
                    }
                    return;
                }

                var progress = _interpTimer.Progress;
                var newColor = Color.Lerp(_previousColor, _doneColor, progress);
                OwnerAgent.ScannerColor = newColor;
            }

            _updateTimer.Reset(UpdateInterval);
        }

        internal void OnDestroy()
        {
            OwnerAgent = null;
        }

        [HideFromIl2Cpp]
        internal void UpdateAgentMode(AgentMode mode)
        {
            _agentMode = mode;

            if (OptimizeOnAwake)
            {
                if (_agentMode == AgentMode.Agressive)
                {
                    _disableScriptAfterDone = true;
                }
                else if (!enabled && _isSetup)
                {
                    enabled = true;
                }
            }
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