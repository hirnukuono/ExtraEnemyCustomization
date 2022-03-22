using EEC.CustomAbilities.Bleed;
using EEC.Utils;
using EEC.Utils.Json.Elements;
using Player;

namespace EEC.EnemyCustomizations.Shared
{
    public interface IBleedSetting
    {
        public ValueBase Damage { get; set; }
        public float ChanceToBleed { get; set; }
        public float Interval { get; set; }
        public float Duration { get; set; }
        public bool HasLiquid { get; set; }
        public bool CanBeStacked { get; set; }
        public ScreenLiquidSettingName LiquidSetting { get; set; }
        public uint OverrideBleedingTextID { get; set; }
    }

    public sealed class BleedSetting
    {
        public bool Enabled { get; set; } = false;
        public ValueBase Damage { get; set; } = ValueBase.Zero;
        public float ChanceToBleed { get; set; } = 0.0f;
        public float Interval { get; set; } = 0.0f;
        public float Duration { get; set; } = 0.0f;
        public bool HasLiquid { get; set; } = true;
        public bool CanBeStacked { get; set; } = false;
        public ScreenLiquidSettingName LiquidSetting { get; set; } = ScreenLiquidSettingName.enemyBlood_Squirt;
        public uint OverrideBleedingTextID { get; set; } = 0u;

        public void DoBleed(PlayerAgent agent)
        {
            BleedManager.DoBleed(agent, new BleedingData()
            {
                interval = Interval,
                duration = Duration,
                damage = Damage.GetAbsValue(PlayerData.MaxHealth),
                chanceToBleed = ChanceToBleed,
                doStack = CanBeStacked,
                liquid = HasLiquid ? LiquidSetting : (ScreenLiquidSettingName)(-1),
                textSpecialOverride = OverrideBleedingTextID
            });
        }

        public static void StopBleed(PlayerAgent agent)
        {
            BleedManager.StopBleed(agent);
        }
    }
}