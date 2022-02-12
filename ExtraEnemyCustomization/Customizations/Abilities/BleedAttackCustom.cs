using Agents;
using EECustom.Customizations.Shared;
using EECustom.Events;
using EECustom.Managers;
using Enemies;
using Gear;
using Player;

namespace EECustom.Customizations.Abilities
{
    public sealed class BleedAttackCustom : EnemyCustomBase
    {
        public BleedSetting MeleeData { get; set; } = new();
        public BleedSetting TentacleData { get; set; } = new();

        public override string GetProcessName()
        {
            return "BleedAttack";
        }

        public override void OnConfigLoaded()
        {
            LocalPlayerDamageEvents.MeleeDamage += OnMelee;
            LocalPlayerDamageEvents.TentacleDamage += OnTentacle;

            if (ConfigManager.Global.CanMediStopBleeding)
                ResourcePackEvents.ReceiveMedi += RecieveMedi;
        }

        public override void OnConfigUnloaded()
        {
            LocalPlayerDamageEvents.MeleeDamage -= OnMelee;
            LocalPlayerDamageEvents.TentacleDamage -= OnTentacle;

            if (ConfigManager.Global.CanMediStopBleeding)
                ResourcePackEvents.ReceiveMedi -= RecieveMedi;
        }

        public void OnMelee(PlayerAgent player, Agent inflictor, float damage)
        {
            if (inflictor is not null && inflictor.Type == AgentType.Enemy)
            {
                var enemy = inflictor.Cast<EnemyAgent>();

                if (IsTarget(enemy))
                {
                    MeleeData.DoBleed(player);
                }
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (inflictor is not null && inflictor.Type == AgentType.Enemy)
            {
                var enemy = inflictor.Cast<EnemyAgent>();

                if (IsTarget(enemy))
                {
                    TentacleData.DoBleed(player);
                }
            }
        }

        private static void RecieveMedi(iResourcePackReceiver receiver, float _)
        {
            var player = receiver.TryCast<PlayerAgent>();
            if (player != null && player.IsLocallyOwned)
            {
                BleedSetting.StopBleed(player);
            }
        }
    }
}