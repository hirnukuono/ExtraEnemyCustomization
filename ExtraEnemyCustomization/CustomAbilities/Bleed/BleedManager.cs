using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.CustomAbilities.Bleed
{
    public static class BleedManager
    {
        internal static BleedSync Sync { get; private set; } = new();

        static BleedManager()
        {
            Sync.Setup();
        }

        public static void DoBleed(BleedingData data)
        {
            Sync.Send(data);
        }

        public static void StopBleed(int playerIndex)
        {
            DoBleed(new BleedingData()
            {
                playerSlot = playerIndex,
                interval = 0.0f,
                duration = -1.0f,
                damage = 0
            });
        }
    }

    public struct BleedingData
    {
        public int playerSlot;
        public float interval;
        public float duration;
        public float damage;
        public float chanceToBleed;
        public ScreenLiquidSettingName liquid;
    }
}
