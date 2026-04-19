using EEC.Networking;
using SNetwork;

namespace EEC.CustomAbilities.Infection
{
    internal sealed class InfectionSync : SyncedPlayerEvent<InfectionData>
    {
        public override string GUID => "INF";
        public override bool SendToTargetOnly => true;
        public override bool AllowBots => false;

        protected override void Receive(InfectionData packet, SNet_Player receivedPlayer)
        {
            if (TryGetPlayerAgent(receivedPlayer, out var agent))
            {
                if (Logger.VerboseLogAllowed)
                    Logger.Verbose($"Infection [{agent.PlayerSlotIndex} {packet.infection}]");

                if (packet.soundEventID != 0u)
                {
                    agent.Sound.Post(packet.soundEventID);
                }

                if (packet.useEffect)
                {
                    var liquidSetting = ScreenLiquidSettingName.spitterJizz;
                    if (packet.infection < 0.0f)
                    {
                        liquidSetting = ScreenLiquidSettingName.disinfectionStation_Apply;
                    }
                    ScreenLiquidManager.TryApply(liquidSetting, agent.Position, packet.screenLiquidRange, true);
                }

                agent.Damage.ModifyInfection(new pInfection()
                {
                    amount = packet.infection,
                    effect = pInfectionEffect.None,
                    mode = pInfectionMode.Add
                }, true, true);
            }
        }
    }
}