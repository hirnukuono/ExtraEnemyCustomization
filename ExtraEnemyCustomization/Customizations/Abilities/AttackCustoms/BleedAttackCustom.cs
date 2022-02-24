using Agents;
using EECustom.Customizations.Shared;
using EECustom.Events;
using EECustom.Managers;
using Enemies;
using Gear;
using Player;

namespace EECustom.Customizations.Abilities
{
    public sealed class BleedAttackCustom : AttackCustomBase<BleedSetting>
    {
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