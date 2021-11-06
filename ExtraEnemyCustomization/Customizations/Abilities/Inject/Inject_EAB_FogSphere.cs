using EECustom.Customizations.Shared.Handlers;
using EECustom.Utils;
using HarmonyLib;

namespace EECustom.Customizations.Abilities.Inject
{
    [HarmonyPatch(typeof(EAB_FogSphere), nameof(EAB_FogSphere.DoTrigger))]
    internal class Inject_EAB_FogSphere
    {
        private static void Prefix(EAB_FogSphere __instance)
        {
            var effectSetting = EnemyProperty<SphereEffectSetting>.Get(__instance.m_owner);
            if (effectSetting == null)
                return;

            effectSetting.HandlerCount = __instance.m_activeFogSpheres.Count;
        }

        private static void Postfix(EAB_FogSphere __instance)
        {
            var effectSetting = EnemyProperty<SphereEffectSetting>.Get(__instance.m_owner);
            if (effectSetting == null)
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