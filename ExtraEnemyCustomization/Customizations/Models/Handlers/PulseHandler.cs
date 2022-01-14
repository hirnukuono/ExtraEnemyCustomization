using Agents;
using EECustom.Attributes;
using Enemies;
using System;
using UnityEngine;

namespace EECustom.Customizations.Models.Handlers
{
    [InjectToIl2Cpp]
    public class PulseHandler : MonoBehaviour
    {
        public PulseEffectData PulseData;
        public float StartDelay = 0.0f;

        private EnemyAgent _ownerAgent;
        private float _timer = 0.0f;
        private float _updateTimerDelay = 0.0f;
        private Color _defaultColor;
        private int _currentPatternIndex = 0;
        private int _patternLength = 0;

        public PulseHandler(IntPtr ptr) : base(ptr)
        {
        }

        internal void Start()
        {
            if (PulseData.PatternData == null)
            {
                enabled = false;
                return;
            }

            _patternLength = PulseData.PatternData.Length;
            if (_patternLength <= 1)
            {
                enabled = false;
                return;
            }

            _ownerAgent = GetComponentInParent<EnemyAgent>();
            _defaultColor = _ownerAgent.MaterialHandler.m_defaultGlowColor;

            var interval = Math.Max(0.0f, PulseData.Duration);
            _updateTimerDelay = interval / _patternLength;
            _timer = Clock.Time + StartDelay;
        }

        internal void Update()
        {
            if (_timer > Clock.Time)
                return;

            if (!PulseData.Target.IsMatch(_ownerAgent))
                return;

            if (!_ownerAgent.Alive && !PulseData.KeepOnDead)
                return;

            if (!PulseData.AlwaysPulse)
            {
                switch (_ownerAgent.AI.Mode)
                {
                    case AgentMode.Hibernate:
                        if (_ownerAgent.IsHibernationDetecting)
                            return;
                        break;

                    case AgentMode.Agressive:
                        switch (_ownerAgent.Locomotion.CurrentStateEnum)
                        {
                            case ES_StateEnum.ShooterAttack:
                            case ES_StateEnum.StrikerAttack:
                            case ES_StateEnum.TankMultiTargetAttack:
                            case ES_StateEnum.ScoutScream:
                                return;
                        }
                        break;

                    case AgentMode.Scout:
                        var detection = _ownerAgent.Locomotion.ScoutDetection.m_antennaDetection;
                        if (detection == null)
                            break;

                        if (detection.m_wantsToHaveTendrils)
                            return;
                        break;
                }
            }

            if (_currentPatternIndex >= _patternLength)
            {
                _currentPatternIndex = 0;
            }

            var patternData = PulseData.PatternData[_currentPatternIndex++];
            var duration = patternData.StepDuration * _updateTimerDelay;

            if (patternData.Progression >= 0.0f)
            {
                var newColor = Color.Lerp(_defaultColor, PulseData.GlowColor, patternData.Progression);
                _ownerAgent.Appearance.InterpolateGlow(newColor, duration);
            }
            _timer = Clock.Time + duration;
        }
    }
}