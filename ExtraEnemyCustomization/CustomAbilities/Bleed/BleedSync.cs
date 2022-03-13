using EECustom.CustomAbilities.Bleed.Handlers;
using EECustom.Networking;
using EECustom.Utils;
using Player;
using SNetwork;

namespace EECustom.CustomAbilities.Bleed
{
    internal sealed class BleedSync : SyncedPlayerEvent<BleedingData>
    {
        public override string GUID => "BLD";

        public override bool SendToTargetOnly => true;

        public override bool AllowBots => false;

        protected override void Receive(BleedingData packet, SNet_Player receivedPlayer)
        {
            if (TryGetPlayerAgent(receivedPlayer, out var agent))
            {
                Logger.Verbose($"Bleed Received: [{agent.PlayerSlotIndex}] {packet.damage} {packet.interval} {packet.duration}");

                if (packet.duration >= 0.0f)
                {
                    if (!Rand.CanDoBy(packet.chanceToBleed))
                        return;

                    GetHandler(agent).DoBleed(packet.damage, packet.interval, packet.duration, packet.liquid);
                }
                else
                {
                    GetHandler(agent).StopBleed();
                }
            } 
        }

        private static BleedHandler GetHandler(PlayerAgent agent)
        {
            var handler = agent.gameObject.GetComponent<BleedHandler>();
            if (handler == null)
            {
                handler = agent.gameObject.AddComponent<BleedHandler>();
            }
            handler.Agent = agent;
            return handler;
        }
    }
}