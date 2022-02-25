using EECustom.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.CustomAbilities.DrainStamina
{
    [CallConstructorOnLoad]
    public static class DrainStaminaManager
    {
        internal static DrainStaminaSync Sync { get; private set; } = new();

        static DrainStaminaManager()
        {
            Sync.Setup();
        }

        public static void DoDrain(DrainStaminaData data)
        {
            Sync.Send(data);
        }
    }

    public struct DrainStaminaData
    {
        public int playerSlot;
        public float amount;
        public float amountInCombat;
        public bool resetRecover;
        public bool resetRecoverInCombat;
    }
}
