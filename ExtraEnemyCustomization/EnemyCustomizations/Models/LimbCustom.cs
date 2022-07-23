using EEC.EnemyCustomizations.Models.Handlers;
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
                var newHealth = limbCustomData.Health.GetAbsValue(balancingBlock.Health.BodypartHealth);
                var modifier = limb.gameObject.AddOrGetComponent<LimbDataModifier>();
                modifier.Agent.Set(agent);
                modifier.Limb.Set(limb);
                modifier.NewHealth.Set(newHealth);
                modifier.NewMaxHealth.Set(newHealth);

                var isCustom = (limbCustomData.LimbType == LimbDamageType.ArmorCustom || limbCustomData.LimbType == LimbDamageType.WeakspotCustom);
                float multi;
                switch (limbCustomData.LimbType)
                {
                    case LimbDamageType.Normal:
                        modifier.SetMulti(eLimbDamageType.Normal, armor: 1.0f, weakspot: 1.0f);
                        break;

                    case LimbDamageType.Armor:
                    case LimbDamageType.ArmorCustom:
                        multi = isCustom ? limbCustomData.CustomMulti : healthData.ArmorDamageMulti;
                        modifier.SetMulti(eLimbDamageType.Armor, armor: multi, weakspot: 1.0f);
                        break;

                    case LimbDamageType.Weakspot:
                    case LimbDamageType.WeakspotCustom:
                        multi = isCustom ? limbCustomData.CustomMulti : healthData.WeakspotDamageMulti;
                        modifier.SetMulti(eLimbDamageType.Weakspot, armor: 1.0f, weakspot: multi);
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