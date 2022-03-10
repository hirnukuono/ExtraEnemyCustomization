using EECustom.CustomAbilities.Bleed.Handlers;
using EECustom.Networking;
using EECustom.Utils;
using Player;

namespace EECustom.CustomAbilities.Bleed
{
    internal sealed class BleedSync : SyncedEvent<BleedingData>
    {
        private static PlayerAgent _localAgent = null;
        private static BleedHandler _handler = null;

        public override string GUID => "BLD";

        public override void Receive(BleedingData packet)
        {
            if (!PlayerManager.HasLocalPlayerAgent())
                return;

            _localAgent = PlayerManager.GetLocalPlayerAgent();
            if (_localAgent.PlayerSlotIndex != packet.playerSlot)
                return;

            Logger.Verbose($"Bleed Received: [{packet.playerSlot}] {packet.damage} {packet.interval} {packet.duration}");

            if (packet.duration >= 0.0f)
            {
                if (!Rand.CanDoBy(packet.chanceToBleed))
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

        private static void UpdateHandler()
        {
            _handler = _localAgent.gameObject.GetComponent<BleedHandler>();

            if (_handler == null)
            {
                _handler = _localAgent.gameObject.AddComponent<BleedHandler>();
            }
            _handler.Agent = _localAgent;
        }
    }
}