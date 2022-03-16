using Player;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Events
{
    public static class ProjectileEvents
    {
        public static event Action<ProjectileBase, GameObject> CollidedWorld;
        public static event Action<ProjectileBase, PlayerAgent> CollidedPlayer;

        internal static void OnCollisionWorld(ProjectileBase projectile, GameObject collideObject)
        {
            CollidedWorld?.Invoke(projectile, collideObject);
        }

        internal static void OnCollisionPlayer(ProjectileBase projectile, PlayerAgent collidePlayer)
        {
            CollidedPlayer?.Invoke(projectile, collidePlayer);
        }
    }
}
