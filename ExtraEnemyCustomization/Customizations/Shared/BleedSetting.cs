using EECustom.Networking;
using EECustom.Utils;
using EECustom.Utils.JsonElements;
using Player;

namespace EECustom.Customizations.Shared
{
    public sealed class BleedSetting
    {
        public bool Enabled { get; set; } = false;
        public ValueBase Damage { get; set; } = ValueBase.Zero;
        public float ChanceToBleed { get; set; } = 0.0f;
        public float Interval { get; set; } = 0.0f;
        public float Duration { get; set; } = 0.0f;
        public bool HasLiquid { get; set; } = true;
        public ScreenLiquidSettingName LiquidSetting { get; set; } = ScreenLiquidSettingName.enemyBlood_Squirt;

        public void TryBleed(PlayerAgent agent)
        {
            NetworkManager.Bleeding.Send(new Networking.Events.BleedingPacket()
            {
                playerSlot = agent.PlayerSlotIndex,
                interval = Interval,
                duration = Duration,
                damage = Damage.GetAbsValue(PlayerData.MaxHealth),
                chanceToBleed = ChanceToBleed,
                liquid = HasLiquid ? LiquidSetting : (ScreenLiquidSettingName)(-1)
            });
        }

        public void DoBleed(PlayerAgent agent)
        {
            NetworkManager.Bleeding.Send(new Networking.Events.BleedingPacket()
            {
                playerSlot = agent.PlayerSlotIndex,
                interval = Interval,
                duration = Duration,
                damage = Damage.GetAbsValue(PlayerData.MaxHealth),
                chanceToBleed = 1.0f,
                liquid = HasLiquid ? LiquidSetting : (ScreenLiquidSettingName)(-1)
            });
        }

        public static void StopBleed(PlayerAgent agent)
        {
            NetworkManager.Bleeding.Send(new Networking.Events.BleedingPacket()
            {
                playerSlot = agent.PlayerSlotIndex,
                interval = 10.0f,
                duration = -1.0f,
                damage = 0
            });
        }
    }
}