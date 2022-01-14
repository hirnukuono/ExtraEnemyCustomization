using Agents;
using EECustom.Customizations.Abilities.Handlers;
using EECustom.Customizations.Shared;
using EECustom.Events;
using EECustom.Managers;
using EECustom.Utils;
using EECustom.Utils.JsonElements;
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

            if (ConfigManager.Current.Global.CanMediStopBleeding)
                ResourcePackEvents.ReceiveMedi += RecieveMedi;
        }

        public override void OnConfigUnloaded()
        {
            LocalPlayerDamageEvents.MeleeDamage -= OnMelee;
            LocalPlayerDamageEvents.TentacleDamage -= OnTentacle;

            if (ConfigManager.Current.Global.CanMediStopBleeding)
                ResourcePackEvents.ReceiveMedi -= RecieveMedi;
        }

        public void OnMelee(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                var enemyAgent = inflictor.TryCast<EnemyAgent>();
                if (enemyAgent != null)
                    MeleeData.TryBleed(player);
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                var enemyAgent = inflictor.TryCast<EnemyAgent>();
                if (enemyAgent != null)
                    TentacleData.TryBleed(player);
            }
        }

        public void RecieveMedi(iResourcePackReceiver receiver, float _)
        {
            var player = receiver.TryCast<PlayerAgent>();
            if (player != null && player.IsLocallyOwned)
            {
                BleedSetting.StopBleed(player);
            }
        }
    }
}