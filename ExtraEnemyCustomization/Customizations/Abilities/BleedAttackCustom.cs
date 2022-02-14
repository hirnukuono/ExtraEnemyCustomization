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
            DoBleed(player, inflictor, MeleeData);
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            DoBleed(player, inflictor, TentacleData);
        }

        private void DoBleed(PlayerAgent player, Agent inflictor, BleedSetting setting)
        {
            if (inflictor.TryCastToEnemyAgent(out var enemy))
            {
                if (IsTarget(enemy))
                {
                    setting.DoBleed(player);
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