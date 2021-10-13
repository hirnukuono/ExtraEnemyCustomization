using Agents;
using EECustom.Utils;
using Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public class ScannerHandler : MonoBehaviour
    {
        public EnemyAgent _Agent;
        public Color _DefaultColor;
        public Color _WakeupColor;
        public Color _DetectionColor;
        public Color _HeartbeatColor;
        public Color _PatrolColor;
        public Color _FeelerColor;

        public bool _UsingDetectionColor = false;
        public bool _UsingScoutColor = false;

        public float _InterpDuration = 0.5f;

        private EnemyState LastState = EnemyState.Hibernate;
        private EnemyState CurrentState = EnemyState.Hibernate;
        private bool InterpDone = true;
        private float InterpTimer = 0.0f;
        private float InterpStartTime = 0.0f;
        

        public ScannerHandler(IntPtr ptr) : base(ptr)
        {
            
        }

        internal void Start()
        {

        }

        internal void Update()
        {
            UpdateState(out var state);

            if (CurrentState != state)
            {
                LastState = CurrentState;
                CurrentState = state;
                InterpDone = false;
                InterpTimer = Clock.Time + _InterpDuration;
                InterpStartTime = Clock.Time;
            }

            if (!InterpDone)
            {
                if (Clock.Time >= InterpTimer)
                {
                    _Agent.ScannerColor = GetStateColor(CurrentState);
                    InterpDone = true;
                    return;
                }

                var progress = Mathf.InverseLerp(InterpStartTime, InterpTimer, Clock.Time);
                var color1 = GetStateColor(LastState);
                var color2 = GetStateColor(CurrentState);
                var newColor = Color.Lerp(color1, color2, progress);
                _Agent.ScannerColor = newColor;
            }
        }

        [HideFromIl2Cpp]
        private void UpdateState(out EnemyState state)
        {
            switch (_Agent.AI.Mode)
            {
                case AgentMode.Hibernate:
                    if (!_UsingDetectionColor)
                    {
                        state = EnemyState.Hibernate;
                        return;
                    }

                    if (_Agent.IsHibernationDetecting)
                    {
                        if (_Agent.Locomotion.Hibernate.m_heartbeatActive)
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
                    if (!_UsingScoutColor)
                    {
                        state = EnemyState.Wakeup;
                        return;
                    }

                    var detection = _Agent.Locomotion.ScoutDetection.m_antennaDetection;
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
                EnemyState.Hibernate => _DefaultColor,
                EnemyState.Detect => _DetectionColor,
                EnemyState.Heartbeat => _HeartbeatColor,
                EnemyState.Wakeup => _WakeupColor,
                EnemyState.Scout => _PatrolColor,
                EnemyState.ScoutDetect => _FeelerColor,
                _ => _DefaultColor
            };
        }
    }
}
