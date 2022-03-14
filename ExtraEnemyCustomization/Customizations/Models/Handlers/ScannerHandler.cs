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

        private Coroutine _colorInterpolationCoroutine;
        private AgentMode _agentMode = AgentMode.Off;
        private EnemyState _currentState = EnemyState.Initial;
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

        internal void OnEnable()
        {
            MonoBehaviourExtensions.StartCoroutine(this, UpdateLoop());
        }

        [HideFromIl2Cpp]
        private IEnumerator UpdateLoop()
        {
            var currentUpdateInterval = UpdateInterval;
            var yielder = new WaitForSeconds(UpdateInterval);

            while (true)
            {
                DoUpdate();
                if (currentUpdateInterval != UpdateInterval)
                {
                    yielder = new WaitForSeconds(UpdateInterval);
                }
                yield return yielder;
            }
        }

        [HideFromIl2Cpp]
        private void DoUpdate()
        {
            if (!_isSetup)
                return;

            EnemyState state;
            if (!OwnerAgent.IsInDetectedList)
            {
                //NOT Visible on Bio-Tracker

                if (OwnerAgent.UpdateMode != NodeUpdateMode.Close)
                    return;

                UpdateState(out state);
                if (_currentState != state)
                {
                    _currentState = state;
                    OwnerAgent.ScannerColor = GetStateColor(state);
                    TryDisable();
                }
                return;
            }

            //Visible on Bio-Tracker
            UpdateState(out state);
            if (_currentState != state)
            {
                _currentState = state;
                _previousColor = OwnerAgent.m_scannerColor;
                _doneColor = GetStateColor(state);

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
            var interpTimer = new Timer(InterpDuration);

            while (!interpTimer.TickAndCheckDone())
            {
                var progress = interpTimer.Progress;
                var newColor = Color.Lerp(_previousColor, _doneColor, progress);
                OwnerAgent.ScannerColor = newColor;
                yield return null;
            }

            OwnerAgent.ScannerColor = _doneColor;
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