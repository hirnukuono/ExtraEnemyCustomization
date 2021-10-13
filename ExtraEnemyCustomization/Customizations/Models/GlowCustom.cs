using EECustom.Customizations.Models.Inject;
using EECustom.Customizations.Models.Handlers;
using EECustom.Events;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public class GlowCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        private readonly static Color DefaultPropaWakeColor = ES_HibernateWakeUp.m_propagatedWakeupColor;
        private readonly static Vector4 DefaultPropaWakeLocation = ES_HibernateWakeUp.m_propagatedWakeupLocation;
        private readonly static System.Random _Random = new System.Random();

        public Color DefaultColor { get; set; } = Color.black;
        
        public Color DetectionColor { get; set; } = new Vector4(9f, 3.9f, 3f, 1f);
        public Color HeartbeatColor { get; set; } = new Vector4(9f, 3.9f, 3f, 1f);
        public Color SelfWakeupColor { get; set; } = new Vector4(3f, 0.2f, 0.2f, 0.5f) * 3f;
        public Color PropagateWakeupColor { get; set; } = new Vector4(3f, 0.2f, 0.2f, 0.5f) * 3f;

        public Color TentacleAttackColor { get; set; } = new Vector4(1.5f, 0.1f, 0.1f, 1f) * 1.75f;
        public Color ShooterFireColor { get; set; } = new Vector4(1f, 0.5f, 0.45f, 1f) * 2.15f;

        public PulseEffectData[] PulseEffects { get; set; } = new PulseEffectData[0];
        

        public override string GetProcessName()
        {
            return "Glow";
        }

        public override void OnConfigLoaded()
        {
            for(int i = 0; i < PulseEffects.Length; i++)
            {
                var pulse = PulseEffects[i];
                LogDev($"PatternFound!: {pulse.GlowRawPattern}");
                
                pulse.CachePattern();
                foreach (var pat in pulse.PatternData)
                {
                    LogDev($" - Data, step: {pat.StepDuration}, progression: {pat.Progression}");
                }

                PulseEffects[i] = pulse;
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            //Those are fine
            agent.MaterialHandler.m_defaultGlowColor = DefaultColor;
            agent.Locomotion.ShooterAttack.m_attackGlowColor_End = ShooterFireColor;
            agent.Locomotion.StrikerAttack.m_attackGlowColor = TentacleAttackColor;
            agent.Locomotion.TankMultiTargetAttack.m_attackGlowColor = TentacleAttackColor;
            agent.Locomotion.HibernateWakeup.m_selfWakeupColor = SelfWakeupColor;
            agent.Locomotion.Hibernate.m_heartbeatColorVec = HeartbeatColor;
            
            foreach(var pulse in PulseEffects)
            {
                var manager = agent.gameObject.AddComponent<PulseHandler>();
                manager.PulseData = pulse;
                if(pulse.RandomizeTime)
                {
                    var interval = Math.Max(0.0f, pulse.Duration);
                    var rand = (float)_Random.NextDouble() * interval;
                    manager.StartDelay = rand;
                }
            }

            GlowEvents.RegisterOnGlow(agent, OnGlow);

            //And this is static LMAO
            //ES_HibernateWakeUp.m_propagatedWakeupColor = PropagateWakeupColor
            //ES_Hibernate.s_closestDistanceDetectionColorVec = HibernateColor
        }

        public Color OnGlow(EnemyAgent agent, Color color, Vector4 location)
        {
            if (color == DefaultPropaWakeColor)
            {
                return PropagateWakeupColor;
            }
            else if (color == (Color)agent.Locomotion.Hibernate.m_detectingColorVec)
            {
                return new Color(DetectionColor.r, DetectionColor.g, DetectionColor.b, color.a);
            }

            return color;
        }
    }

    public struct PulseEffectData
    {
        public PulseEffectTarget Target { get; set; }
        public float Duration { get; set; }
        [JsonPropertyName("GlowPattern")]
        public string GlowRawPattern { get; set; }
        public Color GlowColor { get; set; }
        public bool RandomizeTime { get; set; }
        public bool KeepOnDead { get; set; }
        public bool AlwaysPulse { get; set; }

        public PatternDataCache[] PatternData;

        public PulseEffectData(PulseEffectTarget target = PulseEffectTarget.None, float interval = 1.0f, string pattern = "0")
        {
            Target = target;
            Duration = interval;
            GlowRawPattern = pattern;
            GlowColor = Color.red;
            RandomizeTime = true;
            KeepOnDead = false;
            AlwaysPulse = false;

            PatternData = null;
        }

        public PatternDataCache[] CachePattern()
        {
            List<PatternDataCache> cacheList = new List<PatternDataCache>();
            PatternDataCache currentCache = default;

            foreach(var c in GlowRawPattern)
            {
                var progressionValue = c switch
                {
                    '0' => 0.0f,
                    '1' => 0.1f,
                    '2' => 0.2f,
                    '3' => 0.3f,
                    '4' => 0.4f,
                    '5' => 0.5f,
                    '6' => 0.6f,
                    '7' => 0.7f,
                    '8' => 0.8f,
                    '9' => 0.9f,
                    'f' => 1.0f,
                    '-' => -2.0f,
                    '+' => -3.0f,
                    _ => -1.0f
                };

                switch (progressionValue)
                {
                    case -1.0f: //Other weird Char
                        continue;

                    case -2.0f: //Delay
                        if (currentCache.StepDuration > 0 && currentCache.Progression >= 0.0f)
                        {
                            cacheList.Add(currentCache);
                            currentCache = default;
                        }

                        if (currentCache.Progression < 0.0f)
                        {
                            currentCache.StepDuration++;
                        }
                        else
                        {
                            currentCache.StepDuration = 1;
                            currentCache.Progression = -1.0f;
                        }
                        break;

                    case -3.0f: //Continue
                        if (currentCache.StepDuration == 0)
                        {
                            throw new ArgumentException("Pattern '+' has came out first before other value appear!");
                        }
                        currentCache.StepDuration++;
                        break;

                    default: //Just Value
                        if (currentCache.StepDuration > 0)
                        {
                            cacheList.Add(currentCache);
                            currentCache = default;
                        }
                        currentCache.StepDuration = 1;
                        currentCache.Progression = progressionValue;
                        break;
                }
            }

            if (currentCache.StepDuration > 0)
            {
                cacheList.Add(currentCache);
            }

            PatternData = cacheList.ToArray();
            return PatternData;
        }
    }

    public enum PulseEffectTarget
    {
        None,
        Hibernate,
        Combat,
        Scout
    }

    public struct PatternDataCache
    {
        public int StepDuration;
        public float Progression;
    }
}
