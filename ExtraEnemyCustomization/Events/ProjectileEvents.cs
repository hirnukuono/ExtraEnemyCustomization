using Player;
using System;
using UnityEngine;

namespace EEC.Events
{
    public static class ProjectileEvents
    {
        public static event Action<ProjectileBase, GameObject> CollidedWorld;

        public static event Action<ProjectileBase, PlayerAgent> CollidedPlayer;

        public static event Action<ProjectileTargeting> LifeTimeDone;

        internal static void OnCollisionWorld(ProjectileBase projectile, GameObject collideObject)
        {
            CollidedWorld?.Invoke(projectile, collideObject);
        }

        internal static void OnCollisionPlayer(ProjectileBase projectile, PlayerAgent collidePlayer)
        {
            CollidedPlayer?.Invoke(projectile, collidePlayer);
        }

        internal static void OnLifeTimeDone(ProjectileTargeting projectile)
        {
            LifeTimeDone?.Invoke(projectile);
        }
    }
}