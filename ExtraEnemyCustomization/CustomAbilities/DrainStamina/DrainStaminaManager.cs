using EEC.Attributes;
using Player;

namespace EEC.CustomAbilities.DrainStamina
{
    [CallConstructorOnLoad]
    public static class DrainStaminaManager
    {
        internal static DrainStaminaSync Sync { get; private set; } = new();

        static DrainStaminaManager()
        {
            Sync.Setup();
        }

        public static void DoDrain(PlayerAgent agent, DrainStaminaData data)
        {
            Sync.SendToPlayer(data, agent);
        }
    }

    public struct DrainStaminaData
    {
        public float amount;
        public float amountInCombat;
        public bool resetRecover;
        public bool resetRecoverInCombat;
    }
}