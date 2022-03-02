using AK;
using EECustom.Attributes;
using EECustom.CustomAbilities.Explosion.Handlers;
using EECustom.Events;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.CustomAbilities.Explosion
{
    [CallConstructorOnLoad]
    public static class ExplosionManager
    {
        public static readonly Color FlashColor = new (1, 0.2f, 0, 1);

        internal static ExplosionSync Sync { get; private set; } = new();

        static ExplosionManager()
        {
            Sync.Setup();
            AssetEvents.AllAssetLoaded += AssetEvents_AllAssetLoaded;
        }

        private static void AssetEvents_AllAssetLoaded()
        {
            ExplosionEffectPooling.Initialize();
        }

        public static void DoExplosion(ExplosionData data)
        {
            Sync.Send(data);
        }

        internal static void Internal_TriggerExplosion(Vector3 position, float damage, float enemyMulti, float minRange, float maxRange)
        {
            CellSound.Post(EVENTS.STICKYMINEEXPLODE, position);
            LightFlash(position);

            if (!SNet.IsMaster)
                return;

            var targets = Physics.OverlapSphere(position, maxRange, LayerManager.MASK_EXPLOSION_TARGETS);
            if (targets.Count < 1)
                return;

            DamageUtil.IncrementSearchID();
            var searchID = DamageUtil.SearchID;

            foreach (var target in targets)
            {
                var targetDamagable = target.GetComponent<IDamageable>();
                if (targetDamagable == null)
                    continue;

                targetDamagable = targetDamagable.GetBaseDamagable();
                var targetPosition = targetDamagable.GetBaseAgent()?.EyePosition ?? target.transform.position;

                if (targetDamagable.TempSearchID == searchID)
                {
                    continue;
                }
                targetDamagable.TempSearchID = searchID;

                var distance = Vector3.Distance(position, targetPosition);
                if (Physics.Linecast(position, targetPosition, out RaycastHit _, LayerManager.MASK_WORLD))
                {
                    continue;
                }

                var newDamage = CalcRangeDamage(damage, distance, minRange, maxRange);
                var enemyBase = targetDamagable.TryCast<Dam_EnemyDamageBase>();
                if (enemyBase != null)
                {
                    newDamage *= enemyMulti;
                }
                Logger.Verbose($"Explosive damage: {newDamage} out of max: {damage}, Dist: {distance}, min: {minRange}, max: {maxRange}");

                targetDamagable.ExplosionDamage(newDamage, position, Vector3.up * 1000);
            }
        }

        private static float CalcRangeDamage(float damage, float distance, float minRange, float maxRange)
        {
            var newDamage = 0.0f;
            if (distance <= minRange)
            {
                newDamage = damage;
            }
            else if (distance <= maxRange)
            {
                newDamage = Mathf.Lerp(damage, 0.0f, (distance - minRange) / (maxRange - minRange));
            }
            return newDamage;
        }

        public static void LightFlash(Vector3 pos)
        {
            ExplosionEffectPooling.TryDoEffect(new ExplosionEffectData()
            {
                position = pos,
                flashColor = FlashColor,
                intensity = 5.0f,
                range = 50.0f,
                duration = 0.05f
            });
        }
    }

    public struct ExplosionData
    {
        public Vector3 position;
        public float damage;
        public float enemyMulti;
        public float minRange;
        public float maxRange;
    }
}
