using EECustom.Customizations.EnemyAbilities.Abilities;
using EECustom.Utils;
using EECustom.Utils.JsonElements;
using Enemies;
using System;
using System.Threading.Tasks;

namespace EECustom.Customizations.EnemyAbilities
{
    public sealed class DeathAbilityCustom : EnemyAbilityCustomBase<DeathAbilitySetting>, IEnemySpawnedEvent
    {
        public override string GetProcessName()
        {
            return "DeathAbility";
        }

        public override void OnDead(EnemyAgent agent)
        {
            foreach (var ab in Abilities)
            {
                if (!ab.AllowedMode.IsMatch(agent))
                    return;

                _ = DoTriggerDelayed(ab.Ability, agent, ab.Delay);
            }
        }

        private static async Task DoTriggerDelayed(IAbility ability, EnemyAgent agent, float delay)
        {
            await Task.Delay((int)Math.Round(delay * 1000.0f));
            ThreadDispatcher.Enqueue(() =>
            {
                ability?.TriggerSync(agent);
            });
        }
    }

    public class DeathAbilitySetting : AbilitySettingBase
    {
        public AgentModeTarget AllowedMode { get; set; } = AgentModeTarget.Agressive;
        public float Delay { get; set; } = 0.0f;
    }
}