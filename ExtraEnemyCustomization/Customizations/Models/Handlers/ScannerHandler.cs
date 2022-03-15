using Agents;
using BepInEx.IL2CPP.Utils;
using EECustom.Attributes;
using EECustom.Utils;
using Enemies;
using System.Collections;
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
        public AgentMode CurrentMode => _agentMode;

        private ScannerColorData _data;
        private EnemyAgent _ownerAgent;
        private Coroutine _colorInterpolationCoroutine;
        private AgentMode _agentMode = AgentMode.Off;
        private EnemyState _currentState = EnemyState.Initial;
        private Color _previousColor = Color.white;
        private Color _doneColor = Color.white;
        private bool _disableScriptAfterDone = false;

        internal void OnEnable()
        {
            _ownerAgent = GetComponent<EnemyAgent>();
            if (ScannerCustom._colorLookup.TryGetValue(_ownerAgent.EnemyDataID, out _data))
            {
                UpdateState(out var state);
                _ownerAgent.ScannerColor = GetStateColor(state);
                TryDisable();

                MonoBehaviourExtensions.StartCoroutine(this, UpdateLoop());
            }
            else
            {
                Logger.Error("ColorData was missing! - Unable to start ScannerHandler!");
                Destroy(this);
            }
        }

        [HideFromIl2Cpp]
        private IEnumerator UpdateLoop()
        {
            var currentUpdateInterval = _data.UpdateInterval;
            var yielder = WaitFor.Seconds[_data.UpdateInterval];

            while (true)
            {
                DoUpdate();

                if (currentUpdateInterval != _data.UpdateInterval)
                {
                    yielder = WaitFor.Seconds[_data.UpdateInterval];
                }
                yield return yielder;
            }
        }

        [HideFromIl2Cpp]
        private void DoUpdate()
        {
            EnemyState state;
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
                _colorInterpolationCoroutine = MonoBehaviourExtensions.StartCoroutine(this, ColorInterpolation());
            }
        }

        [HideFromIl2Cpp]
        private IEnumerator ColorInterpolation()
        {
            var interpTimer = new Timer(_data.InterpDuration);

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

        internal void OnDestroy()
        {
            _ownerAgent = null;
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

            if (_data.OptimizeOnAwake)
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
                    if (!_data.UsingDetectionColor)
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
                    if (!_data.UsingScoutColor)
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
                EnemyState.Hibernate => _data.DefaultColor,
                EnemyState.Detect => _data.DetectionColor,
                EnemyState.Heartbeat => _data.HeartbeatColor,
                EnemyState.Wakeup => _data.WakeupColor,
                EnemyState.Scout => _data.PatrolColor,
                EnemyState.ScoutDetect => _data.FeelerOutColor,
                _ => _data.DefaultColor
            };
        }
    }
}