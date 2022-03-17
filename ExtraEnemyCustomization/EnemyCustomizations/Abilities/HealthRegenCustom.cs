using EEC.EnemyCustomizations.Abilities.Handlers;
using EEC.Utils.Json.Elements;
using Enemies;
using System;

namespace EEC.EnemyCustomizations.Abilities
{
    public sealed class HealthRegenCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public HealthRegenData[] RegenDatas { get; set; } = Array.Empty<HealthRegenData>();

        public override string GetProcessName()
        {
            return "HealthRegen";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            if (agent.Damage != null)
            {
                foreach (var regenData in RegenDatas)
                {
                    var ability = agent.gameObject.AddComponent<HealthRegenHandler>();
                    ability.DamageBase = agent.Damage;
                    ability.RegenData = regenData;
                }
            }
        }

        public sealed class HealthRegenData
        {
            public float RegenInterval { get; set; } = 1.0f;
            public float DelayUntilRegenStart { get; set; } = 5.0f;
            public bool CanDamageInterruptRegen { get; set; } = true;
            public ValueBase RegenAmount { get; set; } = ValueBase.Zero;
            public ValueBase RegenCap { get; set; } = ValueBase.Zero;
        }
    }
}