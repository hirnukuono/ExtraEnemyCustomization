using Agents;
using EEC.Attributes;
using EEC.Utils.Unity;
using Enemies;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using System;
using System.Collections;
using UnityEngine;

namespace EEC.EnemyCustomizations.Models.Handlers
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
        public AgentMode CurrentMode => _agentMode;

        public Il2CppReferenceField<EnemyAgent> OwnerAgent;
        public Il2CppValueField<Color> DefaultColor;
        public Il2CppValueField<Color> WakeupColor;
        public Il2CppValueField<Color> DetectionColor;
        public Il2CppValueField<Color> HeartbeatColor;
        public Il2CppValueField<Color> PatrolColor;
        public Il2CppValueField<Color> FeelerOutColor;

        public Il2CppValueField<bool> UsingDetectionColor;
        public Il2CppValueField<bool> UsingScoutColor;

        public Il2CppValueField<float> InterpDuration;
        public Il2CppValueField<float> UpdateInterval;

        public Il2CppValueField<bool> OptimizeOnAwake;

        private EnemyAgent _ownerAgent;
        private Coroutine _colorInterpolationCoroutine;
        private AgentMode _agentMode = AgentMode.Off;
        private EnemyState _currentState = EnemyState.Initial;
        private Color _previousColor = Color.white;
        private Color _doneColor = Color.white;
        private bool _disableScriptAfterDone = false;
        private Coroutine _updateRoutine = null;

        private void OnEnable()
        {
            _ownerAgent = OwnerAgent;
            if (_ownerAgent == null)
            {
                Logger.Error("EnemyAgent was missing! - Unable to start ScannerHandler!");
                Destroy(this);
                return;
            }

            if (_updateRoutine != null)
            {
                StopCoroutine(_updateRoutine);
            }
            _updateRoutine = this.StartCoroutine(UpdateLoop());
        }

        [HideFromIl2Cpp]
        private IEnumerator UpdateLoop()
        {
            var currentUpdateInterval = UpdateInterval;
            var yielder = WaitFor.Seconds[UpdateInterval];

            while (true)
            {
                DoUpdate();

                if (currentUpdateInterval != UpdateInterval)
                {
                    yielder = WaitFor.Seconds[UpdateInterval];
                }
                yield return yielder;
            }
        }

        [HideFromIl2Cpp]
        private void DoUpdate()
        {
            if (!_ownerAgent.IsInDetectedList)
            {
                //NOT Visible on Bio-Tracker

                if (_ownerAgent.UpdateMode != NodeUpdateMode.Close)
                    return;

                if (HasNewState())
                {
                    _ownerAgent.ScannerColor = GetStateColor(_currentState);
                    TryDisable();
                }
                return;
            }

            //Visible on Bio-Tracker
            if (HasNewState())
            {
                _previousColor = _ownerAgent.m_scannerColor;
                _doneColor = GetStateColor(_currentState);

                if (_colorInterpolationCoroutine != null)
                {
                    StopCoroutine(_colorInterpolationCoroutine);
                }
                _colorInterpolationCoroutine = this.StartCoroutine(ColorInterpolation());
            }
        }

        [HideFromIl2Cpp]
        private IEnumerator ColorInterpolation()
        {
            var interpTimer = new Timer(InterpDuration);

            while (!interpTimer.TickAndCheckDone())
            {
                var progress = interpTimer.Progress;
                var newColor = Color.Lerp(_previousColor, _doneColor, progress);
                _ownerAgent.ScannerColor = newColor;
                yield return null;
            }

            _ownerAgent.ScannerColor = _doneColor;
            _colorInterpolationCoroutine = null;
            TryDisable();
            yield return null;
        }

        [HideFromIl2Cpp]
        private void TryDisable()
        {
            if (_disableScriptAfterDone)
            {
                enabled = false;
                _disableScriptAfterDone = false;
            }
        }

        [HideFromIl2Cpp]
        internal void UpdateAgentMode(AgentMode mode, bool forceUpdateWithoutTransition = false)
        {
            _agentMode = mode;

            if (forceUpdateWithoutTransition)
            {
                if (HasNewState())
                {
                    _ownerAgent.ScannerColor = GetStateColor(_currentState);
                }
            }

            if (OptimizeOnAwake)
            {
                if (_agentMode == AgentMode.Agressive)
                {
                    _disableScriptAfterDone = true;
                }
                else if (!enabled)
                {
                    enabled = true;
                }
            }
        }

        [HideFromIl2Cpp]
        private bool HasNewState()
        {
            UpdateState(out var newState);
            if (_currentState != newState)
            {
                _currentState = newState;
                return true;
            }
            return false;
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

                    if (_ownerAgent.IsHibernationDetecting)
                    {
                        if (_ownerAgent.Locomotion.Hibernate.m_heartbeatActive)
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
                    else if (_ownerAgent.Locomotion.CurrentStateEnum == ES_StateEnum.HibernateWakeUp)
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

                    if (_ownerAgent.Locomotion.CurrentStateEnum == ES_StateEnum.ScoutScream)
                    {
                        state = EnemyState.Wakeup;
                        return;
                    }

                    var detection = _ownerAgent.Locomotion.ScoutDetection.m_antennaDetection;
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
                EnemyState.ScoutDetect => FeelerOutColor,
                _ => DefaultColor
            };
        }

        private void OnDestroy()
        {
            _ownerAgent = null;
            _colorInterpolationCoroutine = null;
        }
    }
}