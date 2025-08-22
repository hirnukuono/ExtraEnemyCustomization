using EEC.Managers;
using Player;

namespace EEC.CustomAbilities.Bleed
{
    [CallConstructorOnLoad]
    public static class BleedManager
    {
        internal static BleedSync Sync { get; private set; } = new();

        static BleedManager()
        {
            Sync.Setup();
        }

        public static void DoBleed(PlayerAgent agent, BleedingData data)
        {
            if (!ConfigManager.Global.CanDownStopBleeding || data.duration < 0 || agent.Alive)
                Sync.SendToPlayer(data, agent);
        }

        public static void StopBleed(PlayerAgent agent)
        {
            DoBleed(agent, new BleedingData()
            {
                interval = 0.0f,
                duration = -1.0f,
                damage = 0
            });
        }
    }

    public struct BleedingData
    {
        public float interval;
        public float duration;
        public float damage;
        public float chanceToBleed;
        public bool doStack;
        public ScreenLiquidSettingName liquid;
        public uint textSpecialOverride;
    }
}