using EECustom.Networking;
using SNetwork;

namespace EECustom.CustomAbilities.DrainStamina
{
    internal sealed class DrainStaminaSync : SyncedPlayerEvent<DrainStaminaData>
    {
        public override string GUID => "DRS";
        public override bool SendToTargetOnly => true;
        public override bool AllowBots => false;

        protected override void Receive(DrainStaminaData packet, SNet_Player receivedPlayer)
        {
            if (TryGetPlayerAgent(receivedPlayer, out var agent))
            {
                if (Logger.VerboseLogAllowed)
                    Logger.Verbose($"DrainStamina [{agent.PlayerSlotIndex} {packet.amount} {packet.amountInCombat}]");

                agent.Stamina.UseStamina(new PlayerStamina.ActionCost()
                {
                    baseStaminaCostOutOfCombat = packet.amount,
                    baseStaminaCostInCombat = packet.amountInCombat,
                    resetRestingTimerOutOfCombat = packet.resetRecover,
                    resetRestingTimerInCombat = packet.resetRecoverInCombat
                }, 1.0f);
            }
        }
    }
}