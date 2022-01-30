using HarmonyLib;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Networking.Inject
{
    [HarmonyPatch(typeof(SNet_GlobalManager), nameof(SNet_GlobalManager.OnPlayerEvent))]
    internal static class Inject_SNet_GlobalManager
    {
        internal static void Postfix(SNet_Player player, SNet_PlayerEvent playerEvent, SNet_PlayerEventReason reason)
        {
            switch (playerEvent)
            {
                case SNet_PlayerEvent.PlayerAgentSpawned:
                    SNetEvents.OnAgentSpawned(player);
                    break;
            }
        }
    }
}
