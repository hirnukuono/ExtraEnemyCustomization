using EECustom.Customizations.Abilities.Handlers;
using EECustom.Utils.JsonElements;
using Enemies;

namespace EECustom.Customizations.Abilities
{
    public sealed class HealthRegenCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public HealthRegenData[] RegenDatas { get; set; } = new HealthRegenData[0];

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
    }

    public class HealthRegenData
    {
        public float RegenInterval { get; set; } = 1.0f;
        public float DelayUntilRegenStart { get; set; } = 5.0f;
        public bool CanDamageInterruptRegen { get; set; } = true;
        public ValueBase RegenAmount { get; set; } = ValueBase.Zero;
        public ValueBase RegenCap { get; set; } = ValueBase.Zero;
    }
}