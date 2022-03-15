using EECustom.Managers.Properties;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom
{
    public static class ProjectileTargetingExtension
    {
        public static bool TryGetOwner(this ProjectileBase projectile, out EnemyAgent agent)
        {
            if (projectile == null)
            {
                agent = null;
                return false;
            }

            return ProjectileOwnerManager.TryGet(projectile.gameObject.GetInstanceID(), out agent);
        }

        public static bool TryGetOwner(this ProjectileTargeting projectile, out EnemyAgent agent)
        {
            if (projectile == null)
            {
                agent = null;
                return false;
            }

            return ProjectileOwnerManager.TryGet(projectile.gameObject.GetInstanceID(), out agent);
        }
    }
}
