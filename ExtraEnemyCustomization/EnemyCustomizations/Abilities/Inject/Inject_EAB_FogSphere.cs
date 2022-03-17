using EECustom.EnemyCustomizations.Shared.Handlers;
using HarmonyLib;

namespace EECustom.EnemyCustomizations.Abilities.Inject
{
    [HarmonyPatch(typeof(EAB_FogSphere), nameof(EAB_FogSphere.DoTrigger))]
    internal static class Inject_EAB_FogSphere
    {
        [HarmonyWrapSafe]
        internal static void Prefix(EAB_FogSphere __instance)
        {
            if (!__instance.m_owner.TryGetProperty<SphereEffectProperty>(out var effectSetting))
                return;

            effectSetting.HandlerCount = __instance.m_activeFogSpheres.Count;
        }

        [HarmonyWrapSafe]
        internal static void Postfix(EAB_FogSphere __instance)
        {
            if (!__instance.m_owner.TryGetProperty<SphereEffectProperty>(out var effectSetting))
                return;

            if (effectSetting.HandlerCount == __instance.m_activeFogSpheres.Count)
                return;

            var handler = __instance.m_activeFogSpheres[^1];
            if (handler.gameObject.GetComponent<EffectFogSphereHandler>() != null)
                return;

            var effectHandler = handler.gameObject.AddComponent<EffectFogSphereHandler>();
            effectHandler.Handler = handler;
            effectHandler.EVSphere = effectSetting.Setting.CreateSphere(handler.transform.position, 0.0f, 0.0f);
            EffectVolumeManager.RegisterVolume(effectHandler.EVSphere);
        }
    }
}