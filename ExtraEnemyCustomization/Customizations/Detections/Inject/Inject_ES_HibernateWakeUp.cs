using Enemies;

namespace EECustom.Customizations.Detections.Inject
{
    //[HarmonyPatch(typeof(ES_HibernateWakeUp), nameof(ES_HibernateWakeUp.ActivateState))]
    internal class Inject_ES_HibernateWakeUp
    {
        private static void Postfix(ES_HibernateWakeUp __instance)
        {
            Logger.Log("Detected Wakeup! (From HibernateWakeup)");
            __instance.m_ai.m_locomotion.ScoutScream.ActivateState(__instance.m_ai.Target);
        }
    }
}