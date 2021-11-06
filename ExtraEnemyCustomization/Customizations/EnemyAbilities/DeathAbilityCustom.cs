using EECustom.Customizations.EnemyAbilities.Abilities;
using Enemies;
using System;
using System.Threading.Tasks;

namespace EECustom.Customizations.EnemyAbilities
{
    public class DeathAbilityCustom : EnemyAbilityCustomBase<DeathAbilitySetting>, IEnemySpawnedEvent
    {

        public override string GetProcessName()
        {
            return "DeathAbility";
        }

        public override void OnDead(EnemyAgent agent)
        {
            foreach (var ab in Abilities)
            {
                _ = DoTriggerDelayed(ab.Ability, agent, ab.Delay);
            }
        }

        public async Task DoTriggerDelayed(IAbility ability, EnemyAgent agent, float delay)
        {
            await Task.Delay((int)Math.Round(delay * 1000.0f));
            ability?.TriggerSync(agent);
        }
    }

    public class DeathAbilitySetting : AbilitySettingBase
    {
        public float Delay { get; set; } = 0.0f;
    }
}
