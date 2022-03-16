using EECustom.CustomAbilities.DrainStamina;
using Player;

namespace EECustom.Customizations.Shared
{
    public sealed class DrainStaminaSetting
    {
        public bool Enabled { get; set; } = false;
        public float DrainAmount { get; set; } = 0.0f;
        public float DrainAmountInCombat { get; set; } = 0.0f;
        public bool ResetRecoverTimer { get; set; } = false;
        public bool ResetRecoverTimerInCombat { get; set; } = false;

        public void DoDrain(PlayerAgent agent)
        {
            DrainStaminaManager.DoDrain(agent, new DrainStaminaData()
            {
                amount = DrainAmount,
                amountInCombat = DrainAmountInCombat,
                resetRecover = ResetRecoverTimer,
                resetRecoverInCombat = ResetRecoverTimerInCombat
            });
        }
    }
}