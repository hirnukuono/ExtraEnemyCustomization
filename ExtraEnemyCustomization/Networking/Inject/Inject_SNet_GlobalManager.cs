using HarmonyLib;
using SNetwork;

namespace EECustom.Networking.Inject
{
    [HarmonyPatch(typeof(SNet_GlobalManager))]
    internal static class Inject_SNet_GlobalManager
    {
        [HarmonyPatch(nameof(SNet_GlobalManager.OnPlayerEvent))]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        internal static void Post_PlayerEvent(SNet_Player player, SNet_PlayerEvent playerEvent/*, SNet_PlayerEventReason reason*/)
        {
            switch (playerEvent)
            {
                case SNet_PlayerEvent.PlayerAgentSpawned:
                    SNetEvents.OnAgentSpawned(player);
                    break;
            }
        }

        [HarmonyPatch(nameof(SNet_GlobalManager.OnPrepareForRecall))]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        internal static void Post_PrepareRecall(eBufferType bufferType)
        {
            SNetEvents.OnPrepareRecall(bufferType);
        }

        [HarmonyPatch(nameof(SNet_GlobalManager.OnRecallComplete))]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        internal static void Post_RecallComplete(eBufferType bufferType)
        {
            SNetEvents.OnRecallComplete(bufferType);
        }
    }
}