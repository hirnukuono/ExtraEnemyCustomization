using EECustom.Customizations.Shared.Handlers;
using Player;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Networking.Events
{
    
    public class BleedingEvent : SyncedEvent<BleedingPacket>
    {
        private static readonly Random _random = new();

        public override void Receive(BleedingPacket packet)
        {
            if (!PlayerManager.HasLocalPlayerAgent())
                return;

            var localAgent = PlayerManager.GetLocalPlayerAgent();
            if (localAgent.PlayerSlotIndex != packet.playerSlot)
                return;

            Logger.Verbose($"Bleed Received: [{packet.playerSlot}] {packet.damage} {packet.interval} {packet.duration}");

            if (packet.chanceToBleed <= _random.NextDouble())
                return;

            var handler = localAgent.gameObject.GetComponent<BleedingHandler>();

            if (handler == null)
            {
                handler = localAgent.gameObject.AddComponent<BleedingHandler>();
            }
            handler.Agent = localAgent;
            handler.DoBleed(packet.damage, packet.interval, packet.duration, packet.liquid);
        }
    }

    public struct BleedingPacket
    {
        public int playerSlot;
        public float interval;
        public float duration;
        public float damage;
        public float chanceToBleed;
        public ScreenLiquidSettingName liquid;
    }
}
