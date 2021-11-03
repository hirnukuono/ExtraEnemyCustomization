using Agents;
using AK;
using EECustom.Attributes;
using EECustom.Networking;
using FX_EffectSystem;
using SNetwork;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace EECustom.Utils
{
    public static class ExplosionUtil
    {
        public static void MakeExplosion(Vector3 position, float damage, float enemyMulti, float minRange, float maxRange)
        {
            NetworkManager.Explosion.Send(new Networking.Events.ExplosionPacket()
            {
                position = position,
                damage = damage,
                enemyMulti = enemyMulti,
                minRange = minRange,
                maxRange = maxRange
            });
        }

        internal static void Internal_TriggerExplosion(Vector3 position, float damage, float enemyMulti, float minRange, float maxRange)
        {
            CellSound.Post(EVENTS.STICKYMINEEXPLODE, position);
            //_ = LightFlash(position);

            if (!SNet.IsMaster)
                return;

            var targets = Physics.OverlapSphere(position, maxRange, LayerManager.MASK_EXPLOSION_TARGETS);
            if (targets.Count < 1)
                return;

            DamageUtil.IncrementSearchID();
            var searchID = DamageUtil.SearchID;

            foreach (var target in targets)
            {
                Vector3 targetPosition = target.transform.position;

                Agent agent = target.GetComponent<Agent>();
                if (agent != null)
                {
                    targetPosition = agent.EyePosition;
                }
                Vector3 direction = (targetPosition - position).normalized;

                if (!Physics.Raycast(position, direction.normalized, out RaycastHit _, maxRange, LayerManager.MASK_EXPLOSION_BLOCKERS))
                {
                    var comp = target.GetComponent<IDamageable>();

                    if (comp == null)
                        continue;

                    if (comp.GetBaseDamagable().TempSearchID == searchID)
                        continue;

                    comp.GetBaseDamagable().TempSearchID = searchID;

                    var distance = Vector3.Distance(position, targetPosition);
                    var newDamage = 0.0f;
                    if (distance <= minRange)
                    {
                        newDamage = damage;
                    }
                    else if (distance <= maxRange)
                    {
                        newDamage = Mathf.Lerp(damage, 0.0f, (distance - minRange) / (maxRange - minRange));
                    }
                    Logger.Verbose($"Explosive damage: {newDamage} out of max: {damage}, Dist: {distance}, min: {minRange}, max: {maxRange}");

                    var enemyLimb = comp.TryCast<Dam_EnemyDamageLimb>();
                    if (enemyLimb != null)
                    {
                        comp.GetBaseDamagable().ExplosionDamage(newDamage * enemyMulti, position, Vector3.up * 1000);
                    }
                    else
                    {
                        comp.ExplosionDamage(newDamage, position, Vector3.up * 1000);
                    }
                }
            }
        }

        [Obsolete("FX_Light has issue with level Lighting", true)]
        public static async Task LightFlash(Vector3 pos)
        {
            FX_Manager.TryAllocateFXLight(out FX_PointLight light);
            light.SetColor(new Color(1, 0.2f, 0, 1));
            light.SetRange(50);
            light.m_intensity = 5;
            light.m_position = pos;
            light.m_isOn = true;
            light.UpdateData();
            light.UpdateTransform();
            await Task.Delay(50);
            FX_Manager.DeallocateFXLight(light);
        }
    }
}