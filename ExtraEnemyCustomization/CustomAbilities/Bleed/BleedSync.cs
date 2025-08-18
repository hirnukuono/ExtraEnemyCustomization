using EEC.CustomAbilities.Bleed.Handlers;
using EEC.Networking;
using EEC.Utils;
using Player;
using SNetwork;

namespace EEC.CustomAbilities.Bleed
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
                if (Logger.VerboseLogAllowed)
                    Logger.Verbose($"Bleed Received: [{agent.PlayerSlotIndex}] {packet.damage} {packet.interval} {packet.duration}");

                if (packet.duration >= 0.0f && agent.Alive)
                {
                    if (!Rand.CanDoBy(packet.chanceToBleed))
                        return;

                    GetHandler(agent).DoBleed(packet);
                }
                else
                {
                    GetHandler(agent).StopBleed();
                }
            }
        }

        private static BleedHandler GetHandler(PlayerAgent agent)
        {
            var handler = agent.gameObject.AddOrGetComponent<BleedHandler>();
            handler.Agent = agent;
            return handler;
        }
    }
}