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
            if (SNet.IsMaster)
            {
                Logger.Verbose($"Bleed Received: [{packet.playerSlot}] {packet.damage} {packet.interval} {packet.duration}");

                if (packet.chanceToBleed <= _random.NextDouble())
                    return;

                var player = SNet.Slots.GetPlayerInSlot(packet.playerSlot);
                if (player == null)
                    return;

                if (!player.HasPlayerAgent)
                    return;

                var agent = player.PlayerAgent.Cast<PlayerAgent>();
                var handler = agent.gameObject.GetComponent<BleedingHandler>();

                if (handler == null)
                {
                    handler = agent.gameObject.AddComponent<BleedingHandler>();
                }
                handler.Agent = agent;
                handler.DoBleed(packet.damage, packet.interval, packet.duration);
            }
        }
    }

    public struct BleedingPacket
    {
        public int playerSlot;
        public float interval;
        public float duration;
        public float damage;
        public float chanceToBleed;
    }
}
