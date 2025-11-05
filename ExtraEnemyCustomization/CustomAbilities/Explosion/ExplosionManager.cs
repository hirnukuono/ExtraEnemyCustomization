using Agents;
using AK;
using EEC.Events;
using Player;
using SNetwork;
using UnityEngine;

namespace EEC.CustomAbilities.Explosion
{
    [CallConstructorOnLoad]
    public static class ExplosionManager
    {
        public static readonly Color FlashColor = new(1, 0.2f, 0, 1);

        internal static ExplosionSync Sync { get; private set; } = new();
        internal static ExplosionAgentSync AgentSync { get; private set; } = new();

        private static bool _usingLightFlash = true;
        private static int _hostMask = 0;
        private static int _hostOnlyMask = 0;

        static ExplosionManager()
        {
            Sync.Setup();
            AgentSync.Setup();
            AssetEvents.AllAssetLoaded += AssetEvents_AllAssetLoaded;
        }

        private static void AssetEvents_AllAssetLoaded()
        {
            ExplosionEffectPooling.Initialize();
            _usingLightFlash = Configuration.ShowExplosionEffect;
            _hostOnlyMask = LayerManager.MASK_EXPLOSION_TARGETS;
            _hostMask = LayerManager.MASK_EXPLOSION_TARGETS & ~LayerMask.GetMask("PlayerSynced");
        }

        public static void DoExplosion(ExplosionData data)
        {
            Sync.Send(data);
        }

        public static void DoExplosion(ExplosionAgentData data)
        {
            AgentSync.Send(data);
        }

        public static void DoLocalExplosion(ExplosionData data)
        {
            Internal_TriggerExplosion(
                data.position,
                data.lightColor,
                data.damage,
                data.enemyMulti,
                data.minRange,
                data.maxRange,
                data.enemyMinRange,
                data.enemyMaxRange
                );
        }

        internal static void Internal_TriggerExplosion(Vector3 position, Color lightColor, float damage, float enemyMulti, float minRange, float maxRange, float enemyMinRange, float enemyMaxRange)
        {
            CellSound.Post(EVENTS.STICKYMINEEXPLODE, position);

            if (_usingLightFlash)
                LightFlash(position, maxRange, lightColor);

            if (SNet.IsMaster)
                TriggerHostExplosion(position, damage, enemyMulti, minRange, maxRange, enemyMinRange, enemyMaxRange, _hostMask);
            else
                TriggerClientExplosion(position, damage, minRange, maxRange);
        }

        internal static void Internal_TriggerHostOnlyExplosion(Vector3 position, Color lightColor, float damage, float enemyMulti, float minRange, float maxRange, float enemyMinRange, float enemyMaxRange)
        {
            CellSound.Post(EVENTS.STICKYMINEEXPLODE, position);

            if (_usingLightFlash)
                LightFlash(position, maxRange, lightColor);

            if (SNet.IsMaster)
                TriggerHostExplosion(position, damage, enemyMulti, minRange, maxRange, enemyMinRange, enemyMaxRange, _hostOnlyMask);
        }

        private static void TriggerClientExplosion(Vector3 position, float damage, float minRange, float maxRange)
        {
            var target = PlayerManager.GetLocalPlayerAgent();
            if (target == null) return;

            Vector3 targetPosition = target.EyePosition;
            var distance = Vector3.Distance(position, targetPosition);
            if (Physics.Linecast(position, targetPosition, out RaycastHit hit, LayerManager.MASK_EXPLOSION_BLOCKERS))
                return;

            float newDamage = CalcRangeDamage(damage, distance, minRange, maxRange);
            if (newDamage == 0) return;

            Logger.Verbose($"Explosive damage: {newDamage} out of max: {damage}, Dist: {distance}, min: {minRange}, max: {maxRange}");
            var damageable = target.GetComponent<Dam_SyncedDamageBase>();
            if (newDamage < 0)
                ExplosionHeal(damageable, target, -newDamage);
            else
                damageable.ExplosionDamage(newDamage, position, Vector3.up * 1000);
        }

        private static void TriggerHostExplosion(Vector3 position, float damage, float enemyMulti, float minRange, float maxRange, float enemyMinRange, float enemyMaxRange, int mask)
        {
            var targets = Physics.OverlapSphere(position, Math.Max(maxRange, enemyMaxRange), mask);
            if (targets.Count < 1)
                return;

            DamageUtil.IncrementSearchID();
            var searchID = DamageUtil.SearchID;

            foreach (var target in targets)
            {
                if (target == null)
                    continue;

                if (target.gameObject == null)
                    continue;

                if (!target.gameObject.TryGetComp<IDamageable>(out var targetDamagable))
                    continue;

                targetDamagable = targetDamagable.GetBaseDamagable();
                if (targetDamagable == null)
                    continue;

                Vector3 targetPosition;
                var baseAgent = targetDamagable.GetBaseAgent();
                if (baseAgent != null)
                {
                    targetPosition = baseAgent.EyePosition;
                }
                else
                {
                    targetPosition = target.transform.position;
                }

                if (targetDamagable.TempSearchID == searchID)
                {
                    continue;
                }
                targetDamagable.TempSearchID = searchID;

                var distance = Vector3.Distance(position, targetPosition);
                if (Physics.Linecast(position, targetPosition, out RaycastHit hit, LayerManager.MASK_EXPLOSION_BLOCKERS))
                {
                    if (hit.collider == null)
                        continue;

                    if (hit.collider.gameObject == null)
                        continue;

                    if (hit.collider.gameObject.GetInstanceID() != target.gameObject.GetInstanceID())
                        continue;
                }

                float newDamage;
                var enemyBase = targetDamagable.TryCast<Dam_EnemyDamageBase>();
                if (enemyBase != null)
                {
                    newDamage = CalcRangeDamage(damage * enemyMulti, distance, enemyMinRange, enemyMaxRange);
                }
                else
                {
                    newDamage = CalcRangeDamage(damage, distance, minRange, maxRange);
                }

                if (newDamage == 0) continue;

                Logger.Verbose($"Explosive damage: {newDamage} out of max: {damage}, Dist: {distance}, min: {minRange}, max: {maxRange}");
                if (newDamage < 0)
                    ExplosionHeal(targetDamagable, baseAgent, -newDamage);
                else
                    targetDamagable.ExplosionDamage(newDamage, position, Vector3.up * 1000);
            }
        }

        private static void ExplosionHeal(IDamageable damageable, Agents.Agent? agent, float heal)
        {
            if (agent != null)
                ExplosionHeal(damageable.Cast<Dam_SyncedDamageBase>(), agent, heal);
        }

        private static void ExplosionHeal(Dam_SyncedDamageBase damageable, Agents.Agent agent, float heal)
        {
            var syncedBase = damageable.Cast<Dam_SyncedDamageBase>();
            syncedBase.SendSetHealth(syncedBase.Health + heal);
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

        public static void LightFlash(Vector3 pos, float range, Color lightColor)
        {
            ExplosionEffectPooling.TryDoEffect(new ExplosionEffectData()
            {
                position = pos,
                flashColor = lightColor,
                intensity = 5.0f,
                range = range,
                duration = 0.05f
            });

            if (PlayerManager.HasLocalPlayerAgent())
            {
                var localAgent = PlayerManager.GetLocalPlayerAgent();
                var dist = (localAgent.Position - pos).magnitude;
                var amp = 6.0f * Mathf.Max(0.0f, Mathf.InverseLerp(range, 0, dist));
                if (amp > 0.01f)
                {
                    PlayerManager.GetLocalPlayerAgent().FPSCamera.Shake(1.5f, amp, 0.09f);
                }
            }
        }
    }

    public struct ExplosionData
    {
        public Vector3 position;
        public float damage;
        public float enemyMulti;
        public float minRange;
        public float maxRange;
        public float enemyMinRange;
        public float enemyMaxRange;
        public Color lightColor;
    }

    public struct ExplosionAgentData
    {
        public pAgent agent;
        public bool useRagdoll;
        public float damage;
        public float enemyMulti;
        public float minRange;
        public float maxRange;
        public float enemyMinRange;
        public float enemyMaxRange;
        public Color lightColor;
    }
}