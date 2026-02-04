using Agents;
using AK;
using EEC.CustomAbilities.Bleed;
using EEC.CustomAbilities.DrainStamina;
using EEC.CustomAbilities.Knockback;
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

        public static void DoExplosion(ExplosionPosData data)
        {
            Sync.Send(data);
        }

        public static void DoExplosion(ExplosionAgentData data)
        {
            AgentSync.Send(data);
        }

        public static void DoLocalExplosion(ExplosionPosData data)
        {
            Internal_TriggerExplosion(
                data.position,
                data.data
                );
        }

        internal static void Internal_TriggerExplosion(Vector3 position, ExplosionData data)
        {
            CellSound.Post(EVENTS.STICKYMINEEXPLODE, position);

            if (_usingLightFlash)
                LightFlash(position, data.maxRange, data.lightColor);

            if (SNet.IsMaster)
                TriggerHostExplosion(position, data, _hostMask);
            else
                TriggerClientExplosion(position, data);
        }

        internal static void Internal_TriggerHostOnlyExplosion(Vector3 position, ExplosionData data)
        {
            CellSound.Post(EVENTS.STICKYMINEEXPLODE, position);

            if (_usingLightFlash)
                LightFlash(position, data.maxRange, data.lightColor);

            if (SNet.IsMaster)
                TriggerHostExplosion(position, data, _hostOnlyMask);
        }

        private static void TriggerClientExplosion(Vector3 position, ExplosionData data)
        {
            var target = PlayerManager.GetLocalPlayerAgent();
            if (target == null) return;

            Vector3 targetPosition = target.EyePosition;
            var distance = Vector3.Distance(position, targetPosition);
            if (Physics.Linecast(position, targetPosition, LayerManager.MASK_EXPLOSION_BLOCKERS)) return;

            float rangeMod = CalcRangeMod(distance, data.minRange, data.maxRange);
            float newDamage = data.damage * rangeMod;
            if (newDamage == 0) return;

            Logger.Verbose($"Explosive damage: {newDamage} out of max: {data.damage}, Dist: {distance}, min: {data.minRange}, max: {data.maxRange}");
            DoExplosionHit(newDamage, rangeMod, data, target.Damage.Cast<IDamageable>(), position, target);
        }

        private static void TriggerHostExplosion(Vector3 position, ExplosionData data, int mask)
        {
            var targets = Physics.OverlapSphere(position, Math.Max(data.maxRange, data.enemyMaxRange), mask);
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
                float rangeMod;
                var agentType = baseAgent?.Type ?? AgentType.Decoy;
                if (agentType == AgentType.Enemy)
                {
                    rangeMod = CalcRangeMod(distance, data.enemyMinRange, data.enemyMaxRange);
                    newDamage = data.damage * data.enemyMulti * rangeMod;
                }
                else
                {
                    rangeMod = CalcRangeMod(distance, data.minRange, data.maxRange);
                    newDamage = data.damage * rangeMod;
                }

                if (newDamage == 0) continue;

                Logger.Verbose($"Explosive damage: {newDamage} out of max: {data.damage}, Dist: {distance}, min: {data.minRange}, max: {data.maxRange}");
                DoExplosionHit(newDamage, rangeMod, data, targetDamagable, position, baseAgent);
            }
        }

        private static void DoExplosionHit(float damage, float rangeMod, ExplosionData data, IDamageable damageable, Vector3 position, Agent? agent)
        {
            if (damage < 0)
                ExplosionHeal(damageable, -damage);
            else
                damageable.ExplosionDamage(damage, position, Vector3.up * 1000);

            var agentType = agent?.Type ?? AgentType.Decoy;
            if (agentType == AgentType.Player)
            {
                var player = agent!.Cast<PlayerAgent>();
                if (data.bleeding.enabled)
                {
                    var bleeding = data.bleeding.packet;
                    bleeding.damage *= rangeMod;
                    BleedManager.DoBleed(player, bleeding);
                }

                if (data.drainStamina.enabled)
                {
                    var drainStamina = data.drainStamina.packet;
                    drainStamina.amount *= rangeMod;
                    drainStamina.amountInCombat *= rangeMod;
                    DrainStaminaManager.DoDrain(player, drainStamina);
                }

                if (data.knockback.enabled)
                {
                    var knockback = data.knockback.packet;
                    knockback.velocity *= rangeMod;
                    knockback.velocityZ *= rangeMod;
                    KnockbackManager.DoKnockback(player, knockback);
                }
            }
        }

        private static void ExplosionHeal(IDamageable damageable, float heal)
        {
            var syncedBase = damageable.Cast<Dam_SyncedDamageBase>();
            syncedBase.SendSetHealth(syncedBase.Health + heal);
        }

        private static float CalcRangeMod(float distance, float minRange, float maxRange)
        {
            var mod = 0.0f;
            if (distance <= minRange)
            {
                mod = 1;
            }
            else if (distance <= maxRange)
            {
                mod = 1 - (distance - minRange) / (maxRange - minRange);
            }
            return mod;
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

    public struct ExplosionPosData
    {
        public Vector3 position;
        public ExplosionData data;
    }

    public struct ExplosionAgentData
    {
        public pAgent agent;
        public bool useRagdoll;
        public ExplosionData data;
    }

    public struct ExplosionData
    {
        public float damage;
        public float enemyMulti;
        public float minRange;
        public float maxRange;
        public float enemyMinRange;
        public float enemyMaxRange;
        public Color lightColor;
        public ExpBleedingData bleeding;
        public ExpDrainStaminaData drainStamina;
        public ExpKnockbackData knockback;
    }

    public struct ExpBleedingData
    {
        public bool enabled;
        public BleedingData packet;
    }

    public struct ExpDrainStaminaData
    {
        public bool enabled;
        public DrainStaminaData packet;
    }

    public struct ExpKnockbackData
    {
        public bool enabled;
        public KnockbackData packet;
    }
}