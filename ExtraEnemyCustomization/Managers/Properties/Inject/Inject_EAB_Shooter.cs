using Agents;
using HarmonyLib;

namespace EECustom.Managers.Properties.Inject
{
    [HarmonyPatch(typeof(EAB_ProjectileShooter), nameof(EAB_ProjectileShooter.FireAtAgent))]
    internal static class Inject_EAB_Shooter
    {
        public static Agent LastAgent = null;

        internal static void Prefix(EAB_ProjectileShooter __instance)
        {
            LastAgent = __instance.m_owner;
        }

        internal static void Postfix()
        {
            LastAgent = null;
        }
    }

    [HarmonyPatch(typeof(EAB_ProjectileShooterSquidBoss), nameof(EAB_ProjectileShooterSquidBoss.FireAtAgent))]
    internal static class Inject_EAB_ShooterSquidBoss
    {
        internal static void Prefix(EAB_ProjectileShooterSquidBoss __instance)
        {
            Inject_EAB_Shooter.LastAgent = __instance.m_owner;
        }

        internal static void Postfix()
        {
            Inject_EAB_Shooter.LastAgent = null;
        }
    }
}