using Agents;
using EECustom.Attributes;
using Enemies;
using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.Customizations.Models.Handlers
{
    public enum EnemyState
    {
        Hibernate,
        Detect,
        Heartbeat,
        Wakeup,
        Scout,
        ScoutDetect
    }

    [InjectToIl2Cpp]
    public class ScannerHandler : MonoBehaviour
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

        private EnemyState _lastState = EnemyState.Hibernate;
        private EnemyState _currentState = EnemyState.Hibernate;
        private bool _interpDone = true;
        private float _interpTimer = 0.0f;
        private float _interpStartTime = 0.0f;

        public ScannerHandler(IntPtr ptr) : base(ptr)
        {
        }

        internal void Start()
        {
            _currentState = OwnerAgent.AI.Mode switch
            {
                AgentMode.Off => EnemyState.Hibernate,
                AgentMode.Agressive => EnemyState.Wakeup,
                AgentMode.Patrolling => EnemyState.Scout,
                AgentMode.Scout => EnemyState.Wakeup,
                AgentMode.Hibernate => EnemyState.Hibernate,
                _ => EnemyState.Hibernate,
            };
            _lastState = _currentState;
        }

        internal void Update()
        {
            UpdateState(out var state);

            if (_currentState != state)
            {
                _lastState = _currentState;
                _currentState = state;
                _interpDone = false;
                _interpTimer = Clock.Time + InterpDuration;
                _interpStartTime = Clock.Time;
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
                var color1 = GetStateColor(_lastState);
                var color2 = GetStateColor(_currentState);
                var newColor = Color.Lerp(color1, color2, progress);
                OwnerAgent.ScannerColor = newColor;
            }
        }

        [HideFromIl2Cpp]
        private void UpdateState(out EnemyState state)
        {
            switch (OwnerAgent.AI.Mode)
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