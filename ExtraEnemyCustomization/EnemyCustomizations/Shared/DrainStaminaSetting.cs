using EEC.CustomAbilities.DrainStamina;
using Player;

namespace EEC.EnemyCustomizations.Shared
{
    public interface IDrainStaminaSetting
    {
        public float DrainAmount { get; set; }
        public float DrainAmountInCombat { get; set; }
        public bool ResetRecoverTimer { get; set; }
        public bool ResetRecoverTimerInCombat { get; set; }
    }

    public sealed class DrainStaminaSetting : IDrainStaminaSetting
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