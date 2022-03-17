using EEC.Utils;
using HarmonyLib;
using Player;

namespace EEC.Inject
{
    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.Setup))]
    internal static class Inject_PlayerAgent_Setup
    {
        [HarmonyWrapSafe]
        internal static void Postfix(PlayerAgent __instance)
        {
            PlayerData.MaxHealth = __instance.PlayerData.health;
        }
    }
}