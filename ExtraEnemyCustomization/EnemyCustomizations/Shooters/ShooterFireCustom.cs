using Agents;
using EEC.EnemyCustomizations.Shooters.Handlers;
using EEC.Utils.Json.Elements;
using Enemies;
using System;
using System.Linq;
using UnityEngine;

namespace EEC.EnemyCustomizations.Shooters
{
    public sealed class ShooterFireCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyAgentModeEvent
    {
        public FireSetting[] FireSettings { get; set; } = Array.Empty<FireSetting>();

        public override string GetProcessName()
        {
            return "ShooterFire";
        }

        public override void OnConfigLoaded()
        {
            FireSettings = FireSettings.OrderByDescending(f => f.FromDistance).ToArray();
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var projectileSetting = agent.GetComponentInChildren<EAB_ProjectileShooter>(true);
            if (projectileSetting != null)
            {
                var defaultValue = new ShooterFireOption();
                defaultValue.CopyFrom(projectileSetting);

                if (FireSettings.Length == 1)
                {
                    FireSettings[0].ApplyToEAB(projectileSetting, defaultValue);
                }
            }
        }

        public void OnAgentModeChanged(EnemyAgent agent, AgentMode newMode)
        {
            if (FireSettings.Length <= 1 || newMode != AgentMode.Agressive)
                return;

            var projectileSetting = agent.GetComponentInChildren<EAB_ProjectileShooter>(true);
            if (projectileSetting != null)
            {
                var defaultValue = new ShooterFireOption();
                defaultValue.CopyFrom(projectileSetting);

                var distance = GetCurrentDistance(agent);
                var newSetting = FireSettings.First(x => x.FromDistance <= distance);
                newSetting.ApplyToEAB(projectileSetting, defaultValue);

                if (FireSettings.Length > 1)
                {
                    var routine = new ShooterDistSettingRoutine(newSetting)
                    {
                        EAB_Shooter = projectileSetting,
                        DefaultValue = defaultValue,
                        FireSettings = FireSettings
                    };
                    agent.AI.StartCoroutine(routine.Routine());
                }
            }
        }

        private float GetCurrentDistance(EnemyAgent agent)
        {
            if (agent.AI.IsTargetValid) return agent.AI.Target.m_distance;

            var min = FireSettings.First(setting => setting.FromDistance > 0).FromDistance;
            float distance = float.MaxValue;
            foreach (var target in agent.AI.m_behaviourData.Targets)
            {
                if (!target.m_agent.Alive) continue;

                if (distance > target.m_distance && !Physics.Linecast(agent.EyePosition, target.m_agent.EyePosition, LayerManager.MASK_WORLD))
                {
                    distance = target.m_distance;
                    if (distance < min)
                        return distance;
                }
            }
            return distance;
        }

        public sealed class FireSetting
        {
            public float FromDistance { get; set; } = -1.0f;

            public bool OverrideProjectileType { get; set; } = true;
            public ProjectileType ProjectileType { get; set; } = ProjectileType.TargetingLarge;
            public ValueBase BurstCount { get; set; } = ValueBase.Unchanged;
            public ValueBase BurstDelay { get; set; } = ValueBase.Unchanged;
            public ValueBase ShotDelayMin { get; set; } = ValueBase.Unchanged;
            public ValueBase ShotDelayMax { get; set; } = ValueBase.Unchanged;
            public ValueBase InitialFireDelay { get; set; } = ValueBase.Unchanged;
            public ValueBase ShotSpreadXMin { get; set; } = ValueBase.Unchanged;
            public ValueBase ShotSpreadXMax { get; set; } = ValueBase.Unchanged;
            public ValueBase ShotSpreadYMin { get; set; } = ValueBase.Unchanged;
            public ValueBase ShotSpreadYMax { get; set; } = ValueBase.Unchanged;

            public void ApplyToEAB(EAB_ProjectileShooter eab, ShooterFireOption defValue)
            {
                if (OverrideProjectileType)
                    eab.m_type = ProjectileType;

                eab.m_burstCount = BurstCount.GetAbsValue(defValue.burstCount);
                eab.m_burstDelay = BurstDelay.GetAbsValue(defValue.burstDelay);
                eab.m_shotDelayMin = ShotDelayMin.GetAbsValue(defValue.shotDelayMin);
                eab.m_shotDelayMax = ShotDelayMax.GetAbsValue(defValue.shotDelayMax);
                eab.m_initialFireDelay = InitialFireDelay.GetAbsValue(defValue.initialFireDelay);
                eab.m_shotSpreadX = new Vector2(ShotSpreadXMin.GetAbsValue(defValue.shotSpreadX.x), ShotSpreadXMax.GetAbsValue(defValue.shotSpreadX.y));
                eab.m_shotSpreadY = new Vector2(ShotSpreadYMin.GetAbsValue(defValue.shotSpreadY.x), ShotSpreadYMax.GetAbsValue(defValue.shotSpreadY.y));
            }
        }
    }

    public struct ShooterFireOption
    {
        public int burstCount;
        public float burstDelay;
        public float shotDelayMin;
        public float shotDelayMax;
        public float initialFireDelay;
        public Vector2 shotSpreadX;
        public Vector2 shotSpreadY;

        public void CopyFrom(EAB_ProjectileShooter eabShooter)
        {
            burstCount = eabShooter.m_burstCount;
            burstDelay = eabShooter.m_burstDelay;
            shotDelayMin = eabShooter.m_shotDelayMin;
            shotDelayMax = eabShooter.m_shotDelayMax;
            initialFireDelay = eabShooter.m_initialFireDelay;
            shotSpreadX = eabShooter.m_shotSpreadX;
            shotSpreadY = eabShooter.m_shotSpreadY;
        }

        public void CopyTo(EAB_ProjectileShooter eabShooter)
        {
            eabShooter.m_burstCount = burstCount;
            eabShooter.m_burstDelay = burstDelay;
            eabShooter.m_shotDelayMin = shotDelayMin;
            eabShooter.m_shotDelayMax = shotDelayMax;
            eabShooter.m_initialFireDelay = initialFireDelay;
            eabShooter.m_shotSpreadX = shotSpreadX;
            eabShooter.m_shotSpreadY = shotSpreadY;
        }
    }
}