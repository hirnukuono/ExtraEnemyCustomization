using Agents;
using EECustom.Customizations.Shared;
using EECustom.Events;
using EECustom.Utils;
using EECustom.Utils.JsonElements;
using Player;

namespace EECustom.Customizations.Abilities
{
    public sealed class ExplosiveAttackCustom : EnemyCustomBase
    {
        public ExplosionSetting MeleeData { get; set; } = new();
        public ExplosionSetting TentacleData { get; set; } = new();

        public override string GetProcessName()
        {
            return "ExplosiveAttack";
        }

        public override void OnConfigLoaded()
        {
            LocalPlayerDamageEvents.MeleeDamage += OnMelee;
            LocalPlayerDamageEvents.TentacleDamage += OnTentacle;
        }

        public override void OnConfigUnloaded()
        {
            LocalPlayerDamageEvents.MeleeDamage -= OnMelee;
            LocalPlayerDamageEvents.TentacleDamage -= OnTentacle;
        }

        public void OnMelee(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                MeleeData.DoExplode(player);
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                TentacleData.DoExplode(player);
            }
        }
    }
}