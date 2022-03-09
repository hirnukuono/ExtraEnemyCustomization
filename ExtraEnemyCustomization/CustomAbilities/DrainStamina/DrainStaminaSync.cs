using EECustom.Networking;
using Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.CustomAbilities.DrainStamina
{
    internal sealed class DrainStaminaSync : SyncedEvent<DrainStaminaData>
    {
        public override string GUID => "DRS";

        public override void Receive(DrainStaminaData packet)
        {
            if (!PlayerManager.HasLocalPlayerAgent())
                return;

            var localAgent = PlayerManager.GetLocalPlayerAgent();
            if (localAgent.PlayerSlotIndex != packet.playerSlot)
                return;

            localAgent.Stamina.UseStamina(new PlayerStamina.ActionCost()
            {
                baseStaminaCostOutOfCombat = packet.amount,
                baseStaminaCostInCombat = packet.amountInCombat,
                resetRestingTimerOutOfCombat = packet.resetRecover,
                resetRestingTimerInCombat = packet.resetRecoverInCombat
            }, 1.0f);
        }
    }
}
