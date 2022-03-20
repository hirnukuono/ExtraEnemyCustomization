using EEC.EnemyCustomizations.EnemyAbilities.Abilities;
using EEC.Utils.Json.Elements;
using Enemies;
using GTFO.API.Utilities;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEC.EnemyCustomizations.EnemyAbilities
{
    public sealed class LimbDestroyedAbilityCustom : EnemyAbilityCustomBase<LimbDestroyedAbilitySetting>
    {
        public override string GetProcessName()
        {
            return "LimbDestroyedAbility";
        }

        public override void OnBehaviourAssigned(EnemyAgent agent, AbilityBehaviour behaviour, LimbDestroyedAbilitySetting setting)
        {
            var damageBase = agent.GetComponentInChildren<Dam_EnemyDamageBase>();
            if (damageBase == null)
                return;

            var allLimbData = setting.Limbs.SingleOrDefault(x => x.InvariantEquals("All", ignoreCase: true));
            foreach (var limb in damageBase.DamageLimbs)
            {
                if (Logger.VerboseLogAllowed)
                    LogVerbose($" - Found Limb: {limb.name}");

                var limbCustomData = setting.Limbs.SingleOrDefault(x => x.InvariantEquals(limb.name, ignoreCase: true));
                if (limbCustomData == null)
                {
                    if (allLimbData == null)
                    {
                        continue;
                    }
                    limbCustomData = allLimbData;
                }

                limb.add_OnLimbDestroyed(new Action(() =>
                {
                    if (!SNet.IsMaster)
                        return;

                    if (!setting.AllowedMode.IsMatch(agent))
                        return;

                    DoTriggerDelayed(setting.Ability, agent, setting.Delay);
                }));
            }
        }
    }

    public sealed class LimbDestroyedAbilitySetting : AbilitySettingBase
    {
        public string[] Limbs { get; set; } = Array.Empty<string>();
        public AgentModeTarget AllowedMode { get; set; } = AgentModeTarget.Agressive;
        public float Delay { get; set; } = 0.0f;
    }
}
