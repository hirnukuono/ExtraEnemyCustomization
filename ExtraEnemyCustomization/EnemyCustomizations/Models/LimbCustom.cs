using EEC.Utils.Json.Elements;
using Enemies;
using GameData;
using System;
using System.Linq;

namespace EEC.EnemyCustomizations.Models
{
    public sealed class LimbCustom : EnemyCustomBase, IEnemyPrefabBuiltEvent
    {
        public LimbData[] Limbs { get; set; } = Array.Empty<LimbData>();

        public override string GetProcessName()
        {
            return "Limb";
        }

        public void OnPrefabBuilt(EnemyAgent agent, EnemyDataBlock enemyData)
        {
            var damageBase = agent.GetComponentInChildren<Dam_EnemyDamageBase>();
            if (damageBase == null)
                return;

            if (!enemyData.TryGetBalancingBlock(out var balancingBlock))
                return;

            var allLimbData = Limbs.SingleOrDefault(x => x.LimbName.InvariantEquals("All", ignoreCase: true));
            var healthData = balancingBlock.Health;
            foreach (var limb in damageBase.DamageLimbs)
            {
                if (Logger.VerboseLogAllowed)
                    LogVerbose($" - Found Limb: {limb.name}");

                var limbCustomData = Limbs.SingleOrDefault(x => x.LimbName.InvariantEquals(limb.name, ignoreCase: true));
                if (limbCustomData == null)
                {
                    if (allLimbData == null)
                    {
                        continue;
                    }
                    limbCustomData = allLimbData;
                }

                if (Logger.VerboseLogAllowed)
                    LogVerbose($" - Applying Setting to Limb, LimbType: {limbCustomData.LimbType}, CustomMult: {limbCustomData.CustomMulti}, HealthValue: {limbCustomData.Health}");
                var newHealth = limbCustomData.Health.GetAbsValue(limb.m_healthMax);
                limb.m_health = newHealth;
                limb.m_healthMax = newHealth;

                var isCustom = (limbCustomData.LimbType == LimbDamageType.ArmorCustom || limbCustomData.LimbType == LimbDamageType.WeakspotCustom);
                switch (limbCustomData.LimbType)
                {
                    case LimbDamageType.Normal:
                        limb.m_armorDamageMulti = 1.0f;
                        limb.m_weakspotDamageMulti = 1.0f;
                        limb.m_type = eLimbDamageType.Normal;
                        break;

                    case LimbDamageType.Armor:
                    case LimbDamageType.ArmorCustom:
                        limb.m_type = eLimbDamageType.Armor;
                        limb.m_weakspotDamageMulti = 1.0f;
                        limb.m_armorDamageMulti = isCustom ? limbCustomData.CustomMulti : healthData.ArmorDamageMulti;
                        break;

                    case LimbDamageType.Weakspot:
                    case LimbDamageType.WeakspotCustom:
                        limb.m_type = eLimbDamageType.Weakspot;
                        limb.m_armorDamageMulti = 1.0f;
                        limb.m_weakspotDamageMulti = isCustom ? limbCustomData.CustomMulti : healthData.WeakspotDamageMulti;
                        break;
                }
            }
        }

        public sealed class LimbData
        {
            public string LimbName { get; set; } = "Head";
            public LimbDamageType LimbType { get; set; } = LimbDamageType.Weakspot;
            public float CustomMulti { get; set; } = 1.0f;
            public ValueBase Health { get; set; } = ValueBase.Unchanged;
        }

        public enum LimbDamageType
        {
            Normal,
            Weakspot,
            WeakspotCustom,
            Armor,
            ArmorCustom
        }
    }
}