using AK;
using EECustom.Attributes;
using EECustom.CustomAbilities.Explosion;
using EECustom.Networking;
using FX_EffectSystem;
using SNetwork;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace EECustom.Utils
{
    [Obsolete("Will be moved to ExplosionManager")]
    public static class ExplosionUtil
    {
        public static void MakeExplosion(Vector3 position, float damage, float enemyMulti, float minRange, float maxRange)
        {
            ExplosionManager.DoExplosion(new ExplosionData()
            {
                position = position,
                damage = damage,
                enemyMulti = enemyMulti,
                minRange = minRange,
                maxRange = maxRange
            });
        }
    }
}