using EECustom.Events;
using EECustom.Networking;
using EECustom.Utils;
using EECustom.Utils.JsonElements;
using Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Shared
{
    public sealed class BleedSetting
    {
        public bool Enabled { get; set; } = false;
        public ValueBase Damage { get; set; } = ValueBase.Zero;
        public float ChanceToBleed { get; set; } = 0.0f;
        public float Interval { get; set; } = 0.0f;
        public float Duration { get; set; } = 0.0f;

        public void TryBleed(PlayerAgent agent)
        {
            NetworkManager.Bleeding.Send(new Networking.Events.BleedingPacket()
            {
                playerSlot = agent.PlayerSlotIndex,
                interval = Interval,
                duration = Duration,
                damage = Damage.GetAbsValue(PlayerData.MaxHealth),
                chanceToBleed = ChanceToBleed
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
                chanceToBleed = 1.0f
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
