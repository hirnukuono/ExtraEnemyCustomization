using EECustom.Utils;
using Enemies;
using HarmonyLib;
using LevelGeneration;
using System;

namespace EECustom.Customizations.Abilities.Inject
{
    [HarmonyPatch(typeof(EB_InCombat_MoveToNextNode_DestroyDoor), nameof(EB_InCombat_MoveToNextNode_DestroyDoor.UpdateBehaviour))]
    internal class Inject_EB_DestroyDoor
    {
        internal static bool ShouldOverride = false;
        internal static float GlobalTimer = 0.0f;

        private const float MinTime = 0.5f;
        private const float MaxTime = 1.0f;
        private const float Range = MaxTime - MinTime;

        private static readonly Random _random = new();

        [HarmonyWrapSafe]
        public static void Postfix(EB_InCombat_MoveToNextNode_DestroyDoor __instance)
        {
            if (!ShouldOverride)
                return;

            var enemyAgent = __instance.m_ai.Agent.Cast<EnemyAgent>();

            var breakerProp = EnemyProperty<DoorBreakerSetting>.Get(enemyAgent);
            if (breakerProp != null)
            {
                if (breakerProp.UseGlobalTimer && breakerProp.Config.GlobalTimer < Clock.Time)
                {
                    if (DoDamageDoor(__instance, breakerProp.Damage))
                        breakerProp.Config.GlobalTimer = Clock.Time + RandomRange(breakerProp.MinDelay, breakerProp.MaxDelay);
                }
                else if (!breakerProp.UseGlobalTimer && breakerProp.Timer < Clock.Time)
                {
                    if (DoDamageDoor(__instance, breakerProp.Damage))
                        breakerProp.Timer = Clock.Time + RandomRange(breakerProp.MinDelay, breakerProp.MaxDelay);
                }
            }
            else
            {
                if (GlobalTimer >= Clock.Time)
                    return;

                if (DoDamageDoor(__instance, 1.0f))
                    GlobalTimer = Clock.Time + ((float)_Random.NextDouble() * Range) + MinTime;
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

        private static float RandomRange(float min, float max)
        {
            return ((float)_Random.NextDouble() * max - min) + min;
        }
    }
}