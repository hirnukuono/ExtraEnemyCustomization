using EECustom.Customizations.Shared.Handlers;
using Player;
using System;

namespace EECustom.Networking.Events
{
    public class BleedingEvent : SyncedEvent<BleedingPacket>
    {
        private static readonly Random _random = new();
        private static PlayerAgent _localAgent = null;
        private static BleedingHandler _handler = null;

        public override void Receive(BleedingPacket packet)
        {
            if (!PlayerManager.HasLocalPlayerAgent())
                return;

            _localAgent = PlayerManager.GetLocalPlayerAgent();
            if (_localAgent.PlayerSlotIndex != packet.playerSlot)
                return;

            Logger.Verbose($"Bleed Received: [{packet.playerSlot}] {packet.damage} {packet.interval} {packet.duration}");

            if (packet.duration >= 0.0f)
            {
                if (packet.chanceToBleed <= _random.NextDouble())
                    return;

                UpdateHandler();
                _handler.DoBleed(packet.damage, packet.interval, packet.duration, packet.liquid);
            }
            else
            {
                UpdateHandler();
                _handler.StopBleed();
            }
        }

        private void UpdateHandler()
        {
            _handler = _localAgent.gameObject.GetComponent<BleedingHandler>();

            if (_handler == null)
            {
                _handler = _localAgent.gameObject.AddComponent<BleedingHandler>();
            }
            _handler.Agent = _localAgent;
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