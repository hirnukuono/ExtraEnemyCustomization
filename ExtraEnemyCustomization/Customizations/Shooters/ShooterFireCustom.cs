using EECustom.Customizations.Shooters.Handlers;
using EECustom.Utils.JsonElements;
using Enemies;
using System.Linq;
using UnityEngine;

namespace EECustom.Customizations.Shooters
{
    public sealed class ShooterFireCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public FireSetting[] FireSettings { get; set; } = new FireSetting[0];

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
                if (FireSettings.Length > 1)
                {
                    var clone = new EAB_ProjectileShooter()
                    {
                        m_burstCount = projectileSetting.m_burstCount,
                        m_burstDelay = projectileSetting.m_burstDelay,
                        m_shotDelayMin = projectileSetting.m_shotDelayMin,
                        m_shotDelayMax = projectileSetting.m_shotDelayMax,
                        m_initialFireDelay = projectileSetting.m_initialFireDelay,
                        m_shotSpreadX = projectileSetting.m_shotSpreadX,
                        m_shotSpreadY = projectileSetting.m_shotSpreadY
                    };

                    var ability = agent.gameObject.AddComponent<ShooterDistSettingHandler>();
                    ability.DefaultValue = clone;
                    ability.EAB_Shooter = projectileSetting;
                    ability.FireSettings = FireSettings;
                }
                else if (FireSettings.Length == 1)
                {
                    FireSettings[0].ApplyToEAB(projectileSetting);
                }
            }
        }
    }

    public class FireSetting
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

        public void ApplyToEAB(EAB_ProjectileShooter eab, EAB_ProjectileShooter defValue = null)
        {
            if (OverrideProjectileType)
                eab.m_type = ProjectileType;

            if (defValue == null)
            {
                defValue = eab;
            }

            eab.m_burstCount = BurstCount.GetAbsValue(defValue.m_burstCount);
            eab.m_burstDelay = BurstDelay.GetAbsValue(defValue.m_burstDelay);
            eab.m_shotDelayMin = ShotDelayMin.GetAbsValue(defValue.m_shotDelayMin);
            eab.m_shotDelayMax = ShotDelayMax.GetAbsValue(defValue.m_shotDelayMax);
            eab.m_initialFireDelay = InitialFireDelay.GetAbsValue(defValue.m_initialFireDelay);
            eab.m_shotSpreadX = new Vector2(ShotSpreadXMin.GetAbsValue(defValue.m_shotSpreadX.x), ShotSpreadXMax.GetAbsValue(defValue.m_shotSpreadX.y));
            eab.m_shotSpreadY = new Vector2(ShotSpreadYMin.GetAbsValue(defValue.m_shotSpreadY.x), ShotSpreadYMax.GetAbsValue(defValue.m_shotSpreadY.y));
        }
    }
}