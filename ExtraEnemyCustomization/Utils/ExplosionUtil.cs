using EEC.CustomAbilities.Explosion;
using System;
using UnityEngine;

namespace EEC.Utils
{
    [Obsolete("Will be moved to ExplosionManager")]
    public static class ExplosionUtil
    {
        public static void MakeExplosion(Vector3 position, float damage, float enemyMulti, float minRange, float maxRange, float enemyMinRange, float enemyMaxRange)
        {
            ExplosionManager.DoExplosion(new ExplosionData()
            {
                position = position,
                damage = damage,
                enemyMulti = enemyMulti,
                minRange = minRange,
                maxRange = maxRange,
                enemyMinRange = enemyMinRange,
                enemyMaxRange = enemyMaxRange,
                lightColor = ExplosionManager.FlashColor
            });
        }
    }
}