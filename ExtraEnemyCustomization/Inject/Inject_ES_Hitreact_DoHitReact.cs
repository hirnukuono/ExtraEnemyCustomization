using EEC.Events;
using HarmonyLib;
using UnityEngine;

namespace EEC.Inject
{
    // The earliest point at which enemy.Alive is changed from false to true.
    // All other "dead" functions execute after this function finishes executing, so postfix is safe.
    [HarmonyPatch]
    internal static class Inject_ES_Hitreact_DoHitReact
    {
        [HarmonyPatch(typeof(ES_Hitreact), nameof(ES_Hitreact.DoHitReact))]
        [HarmonyWrapSafe]
        [HarmonyPrefix]
        internal static void Pre_HitReact(ES_Hitreact __instance, ref bool __state)
        {
            __state = __instance.m_enemyAgent.Alive;
        }

        [HarmonyPatch(typeof(ES_Hitreact), nameof(ES_Hitreact.DoHitReact))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        internal static void Post_HitReact(ES_Hitreact __instance, bool __state)
        {
            var agent = __instance.m_enemyAgent;
            if (__state != agent.Alive)
                EnemyEvents.OnKilled(agent);
        }

        [HarmonyPatch(typeof(ES_HitreactFlyer), nameof(ES_HitreactFlyer.DoHitReact), new Type[]{
            typeof(int),
            typeof(ES_HitreactType),
            typeof(ImpactDirection),
            typeof(float),
            typeof(bool),
            typeof(Vector3),
            typeof(Vector3)
        })]
        [HarmonyWrapSafe]
        [HarmonyPrefix]
        internal static void Pre_HitReactFlyer(ES_HitreactFlyer __instance, ref bool __state)
        {
            __state = __instance.m_enemyAgent.Alive;
        }

        [HarmonyPatch(typeof(ES_HitreactFlyer), nameof(ES_HitreactFlyer.DoHitReact), new Type[]{
            typeof(int),
            typeof(ES_HitreactType),
            typeof(ImpactDirection),
            typeof(float),
            typeof(bool),
            typeof(Vector3),
            typeof(Vector3)
        })]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        internal static void Post_HitReactFlyer(ES_HitreactFlyer __instance, bool __state)
        {
            var agent = __instance.m_enemyAgent;
            if (__state != agent.Alive)
                EnemyEvents.OnKilled(agent);
        }
    }
}