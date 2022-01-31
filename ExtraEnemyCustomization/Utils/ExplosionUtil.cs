using AK;
using EECustom.Attributes;
using EECustom.Networking;
using FX_EffectSystem;
using SNetwork;
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
            var effectHandler = new GameObject().AddComponent<ExplosionEffectHandler>();
            effectHandler.transform.position = pos;
            effectHandler.FlashColor = new Color(1, 0.2f, 0, 1);
            effectHandler.Intensity = 5.0f;
            effectHandler.Range = 50.0f;
            effectHandler.EffectDuration = 0.05f;
        }

        [InjectToIl2Cpp]
        public class ExplosionEffectHandler : MonoBehaviour
        {
            public Color FlashColor;
            public float Range;
            public float Intensity;
            public float EffectDuration;

            private bool _lightAllocated = false;
            private FX_PointLight _light;
            private float _timer;

            internal void Start()
            {
                if (FX_Manager.TryAllocateFXLight(out _light, important: false))
                {
                    _light.SetColor(FlashColor);
                    _light.SetRange(Range);
                    _light.m_intensity = Intensity;
                    _light.m_position = transform.position;
                    _light.m_isOn = true;
                    _light.UpdateData();
                    _light.UpdateTransform();
                    _lightAllocated = true;
                    _timer = Clock.Time + EffectDuration;
                }
                else
                {
                    Destroy(this);
                }
            }

            internal void Update()
            {
                if (_timer <= Clock.Time)
                {
                    Destroy(this);
                }
            }

            internal void OnDestroy()
            {
                if (_lightAllocated)
                {
                    FX_Manager.DeallocateFXLight(_light);
                }
            }
        }
    }
}