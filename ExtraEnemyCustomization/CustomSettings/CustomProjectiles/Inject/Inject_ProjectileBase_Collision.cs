using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EEC.CustomSettings.CustomProjectiles.Inject
{
    [HarmonyPatch(typeof(ProjectileBase), nameof(ProjectileBase.Collision))]
    internal static class Inject_ProjectileBase_Collision
    {
        [HarmonyWrapSafe]
        internal static void Postfix(ProjectileBase __instance)
        {
            var instanceData = CustomProjectileManager.GetInstanceData(__instance.gameObject.GetInstanceID());
            if (instanceData == null) return;

            uint soundID = instanceData.Settings.CollisionSoundID;
            if (soundID == 0u)
            {
                // Cancel vanilla sound FX (bugged, happens at origin, icky)
                if (__instance.m_maxInfection > 0)
                {
                    var players = CellSound.Current.m_dynamicPlayers;
                    players[^1].Recycle();
                    players.RemoveAt(players.Count - 1);
                }
                return;
            }

            // If sound was already made, reuse it
            if (__instance.m_maxInfection > 0)
            {
                var player = CellSound.Current.m_dynamicPlayers[^1];
                player.Stop();
                player.Post(soundID, __instance.m_myPos);
            }
            // Otherwise make our own
            else
            {
                CellSound.Post(soundID, __instance.m_myPos);
            }
        }
    }
}
