using EEC.EnemyCustomizations.Shared;
using EEC.Events;
using EEC.Managers;
using Enemies;
using Gear;
using Player;

namespace EEC.EnemyCustomizations.Abilities
{
    public sealed class BleedAttackCustom : AttackCustomBase<BleedSetting>
    {
        public override bool DisableProjectileDamageEvent => false;

        public override string GetProcessName()
        {
            return "BleedAttack";
        }

        public override void OnConfigLoaded()
        {
            base.OnConfigLoaded();

            if (ConfigManager.Global.CanMediStopBleeding)
                ResourcePackEvents.ReceiveMedi += RecieveMedi;
        }

        public override void OnConfigUnloaded()
        {
            base.OnConfigLoaded();

            if (ConfigManager.Global.CanMediStopBleeding)
                ResourcePackEvents.ReceiveMedi -= RecieveMedi;
        }

        protected override void OnApplyEffect(BleedSetting setting, PlayerAgent player, EnemyAgent inflictor)
        {
            setting.DoBleed(player);
        }

        protected override void OnApplyProjectileEffect(BleedSetting setting, PlayerAgent player, EnemyAgent inflictor, ProjectileBase projectile)
        {
            OnApplyEffect(setting, player, inflictor);
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