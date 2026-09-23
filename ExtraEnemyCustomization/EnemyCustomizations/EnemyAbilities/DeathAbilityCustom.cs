using EEC.Managers.Properties;
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
            if (!SNet.IsMaster)
                return;

            bool wasKilled = EnemyDeathManager.WasKilled(agent);
            foreach (var ab in Abilities)
            {
                if ((!wasKilled && ab.IgnoreDespawn) || !ab.AllowedMode.IsMatch(agent))
                    continue;

                DoTriggerDelayed(ab.Ability, agent, ab.Delay, useClientPos: true);
            }
        }
    }

    public sealed class DeathAbilitySetting : AbilitySettingBase
    {
        public AgentModeTarget AllowedMode { get; set; } = AgentModeTarget.Agressive;
        public float Delay { get; set; } = 0f;
        public bool IgnoreDespawn { get; set; } = true;
    }
}