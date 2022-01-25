using EECustom.Utils;
using Enemies;
using HarmonyLib;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(EnemyLocomotion), nameof(EnemyLocomotion.HashAnimationParams))]
    internal static class Inject_EnemyLocomotion
    {
        [HarmonyWrapSafe]
        public static void Postfix()
        {
            EnemyAnimUtil.Initialize();
        }
    }
}