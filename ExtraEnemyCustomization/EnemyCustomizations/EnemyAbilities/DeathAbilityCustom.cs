using EEC.EnemyCustomizations.EnemyAbilities.Abilities;
using EEC.Utils.Json.Elements;
using Enemies;
using GTFO.API.Utilities;
using System;
using System.Threading.Tasks;

namespace EEC.EnemyCustomizations.EnemyAbilities
{
    public sealed class DeathAbilityCustom : EnemyAbilityCustomBase<DeathAbilitySetting>, IEnemyDeadEvent
    {
        public override string GetProcessName()
        {
            return "DeathAbility";
        }

        public void OnDead(EnemyAgent agent)
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
            ThreadDispatcher.Dispatch(() =>
            {
                ability?.TriggerSync(agent);
            });
        }
    }

    public sealed class DeathAbilitySetting : AbilitySettingBase
    {
        public AgentModeTarget AllowedMode { get; set; } = AgentModeTarget.Agressive;
        public float Delay { get; set; } = 0.0f;
    }
}