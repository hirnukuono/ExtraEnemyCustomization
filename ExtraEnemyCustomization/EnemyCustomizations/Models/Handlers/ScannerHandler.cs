using Agents;
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
            EnemyState oldState = EnemyState.Initial;

            if (!_ownerAgent.IsInDetectedList)
            {
                //NOT Visible on Bio-Tracker

                if (_ownerAgent.UpdateMode != NodeUpdateMode.Close)
                    return;

                if (HasNewState(ref oldState))
                {
                    _ownerAgent.ScannerColor = GetStateColor(_currentState);
                    TryDisable();
                }
                return;
            }

            //Visible on Bio-Tracker
            if (HasNewState(ref oldState))
            {
                if (oldState == EnemyState.Initial)
                {
                    _previousColor = GetStateColor(_currentState);
                    _doneColor = _previousColor;
                }
                else
                {
                    _previousColor = _ownerAgent.m_scannerColor;
                    _doneColor = GetStateColor(_currentState);
                }
                StartColorInterpolation(_previousColor, _doneColor);
            }
        }

        [HideFromIl2Cpp]
        private void StartColorInterpolation(Color fromColor, Color toColor)
        {
            if (_colorInterpolationCoroutine != null)
            {
                StopCoroutine(_colorInterpolationCoroutine);
            }
            _colorInterpolationCoroutine = this.StartCoroutine(ColorInterpolation(fromColor, toColor));
        }

        [HideFromIl2Cpp]
        private IEnumerator ColorInterpolation(Color fromColor, Color toColor)
        {
            var interpTimer = new Timer(InterpDuration);

            while (!interpTimer.TickAndCheckDone())
            {
                var progress = interpTimer.Progress;
                var newColor = Color.Lerp(fromColor, toColor, progress);
                _ownerAgent.ScannerColor = newColor;
                yield return null;
            }

            _ownerAgent.ScannerColor = toColor;
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
        private bool HasNewState(ref EnemyState oldState)
        {
            UpdateState(out var newState);
            if (_currentState != newState)
            {
                oldState = _currentState;
                _currentState = newState;
                return true;
            }
            return false;
        }

        [HideFromIl2Cpp]
        private void UpdateState(out EnemyState state)
        {
            var loco = _ownerAgent.Locomotion;
            switch (loco.CurrentStateEnum)
            {
                case ES_StateEnum.ClimbLadder:
                    state = _currentState;
                    break;

                case ES_StateEnum.StuckInGlue:
                    state = EnemyState.Hibernate;
                    break;

                case ES_StateEnum.Hibernate:
                    if (loco.Hibernate.m_heartbeatActive)
                    {
                        state = EnemyState.Heartbeat;
                        break;
                    }
                    state = _ownerAgent.IsHibernationDetecting ? EnemyState.Detect : EnemyState.Hibernate;
                    break;

                case ES_StateEnum.ScoutDetection:
                    state = EnemyState.ScoutDetect;
                    break;

                case ES_StateEnum.PathMove:
                    if (_ownerAgent.AI.m_scoutPath == null)
                    {
                        state = EnemyState.Wakeup;
                        break;
                    }

                    if (loco.ScoutScream.m_state == ES_ScoutScream.ScoutScreamState.Done)
                    {
                        state = EnemyState.Wakeup;
                        break;
                    }
                    state = EnemyState.Scout;
                    break;

                default:
                    state = EnemyState.Wakeup;
                    break;
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