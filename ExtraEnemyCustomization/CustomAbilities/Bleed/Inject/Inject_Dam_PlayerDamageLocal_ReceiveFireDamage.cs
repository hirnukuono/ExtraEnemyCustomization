using AK;
using EEC;
using EEC.Managers;
using HarmonyLib;
using Player;
using UnityEngine;

namespace ExtraEnemyCustomization.CustomAbilities.Bleed.Inject
{
    [HarmonyPatch(typeof(Dam_PlayerDamageLocal), nameof(Dam_PlayerDamageLocal.ReceiveFireDamage))]
    internal static class Inject_Dam_PlayerDamageLocal_ReceiveFireDamage
    {
        // Values pulled from FPSCamera.AddHitReact
        private const float FLASH_COOLDOWN = 0.3f;
        private const float MIN_DAMAGE_FLASH = 0.96f;
        private const float DAMAGE_TO_FLASH_MOD = 1.5f;
        private const float HURT_VOICELINE_THRESHOLD = 0.3f;
        private const float VOICELINE_COOLDOWN = 3f;

        private static float _lastFXTime;
        private static bool Prefix(Dam_PlayerDamageLocal __instance, pSmallDamageData data)
        {
            float damage = data.damage.Get(__instance.HealthMax);
            __instance.OnIncomingDamage(damage, damage);

            if (ConfigManager.Global.CanBleedAimPunch)
                __instance.Hitreact(damage, Vector3.zero, true, Configuration.PlayDialogueFromBleed);
            else
                TryScreenFlash(damage, __instance);
            return false;
        }

        // FPSCamera.HitReact modified to remove spring, so bleed cannot prevent normal attack aim punch.
        private static void TryScreenFlash(float damage, Dam_PlayerDamageLocal __instance)
        {
            var time = Clock.Time;
            if (time <= _lastFXTime) return;

            var owner = __instance.Owner;
            var camera = owner.FPSCamera;
            if (Clock.Time <= camera.m_hitreactTimer) return;

            if (Configuration.PlayDialogueFromBleed && time > __instance.m_damageVoiceTimer)
            {
                if (__instance.GetHealthRel() < HURT_VOICELINE_THRESHOLD)
                    PlayerVoiceManager.WantToSayAndStartDialog(owner.CharacterID, EVENTS.PLAY_LOWHEALTHGRUNT01_1A, 99u);
                else
                    PlayerVoiceManager.WantToSayAndStartDialog(owner.CharacterID, EVENTS.PLAY_LOWHEALTHGRUNT01_1A, 27u);

                __instance.m_damageVoiceTimer = time + VOICELINE_COOLDOWN;
            }

            var direction = UnityEngine.Random.onUnitSphere;
            var forward = Vector3.Dot(direction, camera.m_orgParent.forward);
            var up = Vector3.Dot(direction, camera.m_orgParent.up) * -1;
            var right = Vector3.Dot(direction, camera.m_orgParent.right);
            _lastFXTime = time + FLASH_COOLDOWN;
            float flashAmount = Math.Min(damage / 2f, MIN_DAMAGE_FLASH) * DAMAGE_TO_FLASH_MOD;
            camera.m_fpsDamageFeedback.AddDamage(flashAmount, forward, up, right);
        }
    }
}
