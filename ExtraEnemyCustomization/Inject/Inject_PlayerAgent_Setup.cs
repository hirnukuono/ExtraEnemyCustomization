using EECustom.Utils;
using HarmonyLib;
using Player;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.Setup))]
    internal class Inject_PlayerAgent_Setup
    {
        [HarmonyWrapSafe]
        internal static void Postfix(PlayerAgent __instance)
        {
            PlayerData.MaxHealth = __instance.PlayerData.health;
        }
    }
}