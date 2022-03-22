using EEC.Utils.Json.Elements;
using Enemies;
using SNetwork;

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
                if (!SNet.IsMaster)
                    return;

                if (!ab.AllowedMode.IsMatch(agent))
                    return;

                DoTriggerDelayed(ab.Ability, agent, ab.Delay);
            }
        }
    }

    public sealed class DeathAbilitySetting : AbilitySettingBase
    {
        public AgentModeTarget AllowedMode { get; set; } = AgentModeTarget.Agressive;
        public float Delay { get; set; } = 0.0f;
    }
}