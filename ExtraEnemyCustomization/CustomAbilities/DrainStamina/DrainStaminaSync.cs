using EEC.Networking;
using Player;
using SNetwork;

namespace EEC.CustomAbilities.DrainStamina
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

                var cost = new PlayerStamina.ActionCost()
                {
                    baseStaminaCostOutOfCombat = packet.amount,
                    baseStaminaCostInCombat = packet.amountInCombat,
                    resetRestingTimerOutOfCombat = packet.resetRecover,
                    resetRestingTimerInCombat = packet.resetRecoverInCombat
                };

                if (cost.baseStaminaCostInCombat < 0 && cost.baseStaminaCostOutOfCombat < 0)
                    AddStamina(agent, cost);
                else
                    agent.Stamina.UseStamina(cost, 1.0f);
            }
        }

        private static void AddStamina(PlayerAgent agent, PlayerStamina.ActionCost cost)
        {
            float stamina;
            bool resetRegen;
            bool inCombat = agent.PlayerData.StaminaUsageAffectedByDrama && DramaManager.CurrentStateEnum != DRAMA_State.Combat;
            if (inCombat)
            {
                stamina = -cost.baseStaminaCostOutOfCombat;
                resetRegen = cost.resetRestingTimerOutOfCombat;
            }
            else
            {
                stamina = -cost.baseStaminaCostInCombat;
                resetRegen = cost.resetRestingTimerInCombat;
            }

            var staminaObj = agent.Stamina;
            float newStamina = Math.Min(staminaObj.Stamina + stamina, 1f);
            if (inCombat && newStamina < agent.PlayerData.StaminaMaximumCapWhenInCombat)
                newStamina = agent.PlayerData.StaminaMaximumCapWhenInCombat;
            staminaObj.Stamina = Math.Max(staminaObj.Stamina, newStamina);
            if (resetRegen)
                staminaObj.m_lastExertion = Clock.Time;
        }
    }
}