using Agents;
using EECustom.Attributes;
using EECustom.Utils;
using Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.Models.Handlers
{
    [InjectToIl2Cpp]
    public class PulseHandler : MonoBehaviour
    {
        public PulseEffectData PulseData;
        public float StartDelay = 0.0f;

        private AgentMode _TargetAgentMode;
        private EnemyAgent _Agent;
        private float _Timer = 0.0f;
        private float _UpdateTimerDelay = 0.0f;
        private Color _DefaultColor;
        private int _CurrentPatternIndex = 0;
        private int _PatternLength = 0;

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

            _PatternLength = PulseData.PatternData.Length;
            if (_PatternLength <= 1)
            {
                enabled = false;
                return;
            }

            _Agent = GetComponentInParent<EnemyAgent>();
            _DefaultColor = _Agent.MaterialHandler.m_defaultGlowColor;
            _TargetAgentMode = PulseData.Target switch
            {
                PulseEffectTarget.Hibernate => AgentMode.Hibernate,
                PulseEffectTarget.Combat => AgentMode.Agressive,
                PulseEffectTarget.Scout => AgentMode.Scout,
                PulseEffectTarget.None => AgentMode.Off,
                _ => AgentMode.Hibernate
            };

            if (_TargetAgentMode == AgentMode.Off)
            {
                enabled = false;
                return;
            }

            
            var interval = Math.Max(0.0f, PulseData.Duration);
            _UpdateTimerDelay = interval / _PatternLength;
            _Timer = Clock.Time + StartDelay;
        }

        internal void Update()
        {
            if (_Timer > Clock.Time)
                return;

            if (_Agent.AI.Mode != _TargetAgentMode)
                return;

            if (!_Agent.Alive && !PulseData.KeepOnDead)
                return;

            if (!PulseData.AlwaysPulse)
            {
                switch (_TargetAgentMode)
                {
                    case AgentMode.Hibernate:
                        if (_Agent.IsHibernationDetecting)
                            return;
                        break;

                    case AgentMode.Agressive:
                        switch (_Agent.Locomotion.CurrentStateEnum)
                        {
                            case ES_StateEnum.ShooterAttack:
                            case ES_StateEnum.StrikerAttack:
                            case ES_StateEnum.TankMultiTargetAttack:
                            case ES_StateEnum.ScoutScream:
                                return;
                        }
                        break;

                    case AgentMode.Scout:
                        var detection = _Agent.Locomotion.ScoutDetection.m_antennaDetection;
                        if (detection == null)
                            break;

                        if (detection.m_wantsToHaveTendrils)
                            return;
                        break;
                }
            }

            if (_CurrentPatternIndex >= _PatternLength)
            {
                _CurrentPatternIndex = 0;
            }

            var patternData = PulseData.PatternData[_CurrentPatternIndex++];
            var duration = patternData.StepDuration * _UpdateTimerDelay;

            if (patternData.Progression >= 0.0f)
            {
                var newColor = Color.Lerp(_DefaultColor, PulseData.GlowColor, patternData.Progression);
                _Agent.Appearance.InterpolateGlow(newColor, duration);
            }
            _Timer = Clock.Time + duration;
        }
    }
}
