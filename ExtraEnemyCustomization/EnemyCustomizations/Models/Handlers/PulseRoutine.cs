using Agents;
using EEC.Utils.Unity;
using Enemies;
using System;
using System.Collections;
using UnityEngine;

namespace EEC.EnemyCustomizations.Models.Handlers
{
    public class PulseRoutine
    {
        public EnemyAgent Agent;
        public PulseEffectData PulseData;
        public float StartDelay = 0.0f;

        private float _updateDelay = 0.0f;
        private Color _defaultColor;
        private int _currentPatternIndex = 0;
        private int _patternLength = 0;

        public IEnumerator Routine()
        {
            if (PulseData.PatternData == null)
            {
                yield break;
            }

            _patternLength = PulseData.PatternData.Length;
            if (_patternLength <= 1)
            {
                yield break;
            }

            _defaultColor = Agent.MaterialHandler.m_defaultGlowColor;

            var interval = Math.Max(0.0f, PulseData.Duration);
            _updateDelay = interval / _patternLength;

            yield return WaitFor.Seconds[StartDelay];

            var yielder = WaitFor.Seconds[_updateDelay];
            while (true)
            {
                DoUpdate();
                yield return yielder;
            }
        }

        private void DoUpdate()
        {
            if (!PulseData.Target.IsMatch(Agent))
                return;

            if (!Agent.Alive && !PulseData.KeepOnDead)
                return;

            if (!PulseData.AlwaysPulse)
            {
                switch (Agent.AI.Mode)
                {
                    case AgentMode.Hibernate:
                        if (Agent.IsHibernationDetecting)
                            return;
                        break;

                    case AgentMode.Agressive:
                        if (Agent.Locomotion.IsAttacking())
                            return;

                        switch (Agent.Locomotion.CurrentStateEnum)
                        {
                            case ES_StateEnum.ShooterAttack:
                            case ES_StateEnum.StrikerAttack:
                            case ES_StateEnum.TankMultiTargetAttack:
                            case ES_StateEnum.ScoutScream:
                                return;
                        }

                        break;

                    case AgentMode.Scout:
                        var detection = Agent.Locomotion.ScoutDetection.m_antennaDetection;
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
            var duration = patternData.StepDuration * _updateDelay;

            if (patternData.Progression >= 0.0f)
            {
                var newColor = Color.Lerp(_defaultColor, PulseData.GlowColor, patternData.Progression);
                Agent.Appearance.InterpolateGlow(newColor, duration);
            }
        }
    }
}
