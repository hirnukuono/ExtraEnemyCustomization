using EECustom.CustomAbilities.DrainStamina;
using Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Shared
{
    public sealed class DrainStaminaSetting
    {
        public bool Enabled { get; set; } = false;
        public float DrainAmount { get; set; } = 0.05f;
        public float DrainAmountInCombat { get; set; } = 0.1f;
        public bool ResetRecoverTimer { get; set; } = false;
        public bool ResetRecoverTimerInCombat { get; set; } = false;

        public void DoDrain(PlayerAgent agent)
        {
            DrainStaminaManager.DoDrain(new DrainStaminaData()
            {
                playerSlot = agent.PlayerSlotIndex,
                amount = DrainAmount,
                amountInCombat = DrainAmountInCombat,
                resetRecover = ResetRecoverTimer,
                resetRecoverInCombat = ResetRecoverTimerInCombat
            });
        }
    }
}
