using EECustom.Utils;
using Enemies;
using HarmonyLib;
using LevelGeneration;
using System;

namespace EECustom.Customizations.Abilities.Inject
{
    [HarmonyPatch(typeof(EB_InCombat_MoveToNextNode_DestroyDoor), nameof(EB_InCombat_MoveToNextNode_DestroyDoor.UpdateBehaviour))]
    internal static class Inject_EB_DestroyDoor
    {
        public static bool ShouldOverride = false;
        public static float GlobalTimer = 0.0f;

        public const float MinTime = 0.5f;
        public const float MaxTime = 1.0f;
        public const float Range = MaxTime - MinTime;

        [HarmonyWrapSafe]
        internal static void Postfix(EB_InCombat_MoveToNextNode_DestroyDoor __instance)
        {
            if (!ShouldOverride)
                return;

            var enemyAgent = __instance.m_ai.m_enemyAgent;

            if (enemyAgent.TryGetProperty<DoorBreakerSetting>(out var breakerProp))
            {
                if (breakerProp.UseGlobalTimer && breakerProp.Config._globalTimer < Clock.ExpeditionProgressionTime)
                {
                    if (DoDamageDoor(__instance, breakerProp.Damage))
                        breakerProp.Config._globalTimer = Clock.ExpeditionProgressionTime + Rand.Range(breakerProp.MinDelay, breakerProp.MaxDelay);
                }
                else if (!breakerProp.UseGlobalTimer && breakerProp.Timer < Clock.ExpeditionProgressionTime)
                {
                    if (DoDamageDoor(__instance, breakerProp.Damage))
                        breakerProp.Timer = Clock.ExpeditionProgressionTime + Rand.Range(breakerProp.MinDelay, breakerProp.MaxDelay);
                }
            }
            else
            {
                if (GlobalTimer >= Clock.ExpeditionProgressionTime)
                    return;

                if (DoDamageDoor(__instance, 1.0f))
                    GlobalTimer = Clock.ExpeditionProgressionTime + (Rand.NextFloat() * Range) + MinTime;
            }
        }

        private static bool DoDamageDoor(EB_InCombat_MoveToNextNode_DestroyDoor context, float damage)
        {
            var enemyAgent = context.m_ai.Agent;
            context.m_ai.Agent.TargetLookDir = context.m_ai.m_courseNavigation.m_onPortalPosition - enemyAgent.Position;
            context.m_ai.Agent.TargetLookDir.Normalize();
            var distance = enemyAgent.TargetLookDir.sqrMagnitude;

            if (distance >= 6.25f)
                return false;

            var weakdoor = context.m_ai.m_courseNavigation.m_navPortal.m_door.TryCast<LG_WeakDoor>();
            if (weakdoor == null)
                return false;

            weakdoor.m_sync.AttemptDoorInteraction(eDoorInteractionType.DoDamage, damage, 0f, enemyAgent.Position, null);
            return true;
        }
    }
}