using Agents;
using EECustom.Events;
using EECustom.Utils;
using Enemies;
using Player;
using System.Collections.Generic;

namespace EECustom.Customizations.Abilities
{
    public class ExplosiveAttackCustom : EnemyCustomBase
    {
        public ExplosiveAttackData MeleeData { get; set; } = new();
        public ExplosiveAttackData TentacleData { get; set; } = new();

        public override string GetProcessName()
        {
            return "ExplosiveAttack";
        }

        public override void OnConfigLoaded()
        {
            LocalPlayerDamageEvents.OnMeleeDamage += OnMelee;
            LocalPlayerDamageEvents.OnTentacleDamage += OnTentacle;
        }

        public void OnMelee(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                DoExplode(MeleeData, player, inflictor);
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                DoExplode(TentacleData, player, inflictor);
            }
        }

        private void DoExplode(ExplosiveAttackData data, PlayerAgent player, Agent _)
        {
            var maxDamage = data.Damage.GetAbsValue(PlayerData.MaxHealth);
            if (maxDamage > 0.0f)
            {
                ExplosionUtil.TriggerExplodion(player.Position, maxDamage, data.EnemyDamageMulti, data.MinRange, data.MaxRange);

                var noise = new NM_NoiseData()
                {
                    noiseMaker = null,
                    position = player.Position,
                    radiusMin = data.NoiseMinRange,
                    radiusMax = data.NoiseMaxRange,
                    yScale = 1,
                    node = player.CourseNode,
                    type = NM_NoiseType.Detectable,
                    includeToNeightbourAreas = true,
                    raycastFirstNode = false
                };
                NoiseManager.MakeNoise(noise);
            }
        }
    }

    public class ExplosiveAttackData
    {
        public ValueBase Damage { get; set; } = ValueBase.Zero;
        public float EnemyDamageMulti { get; set; } = 1.0f;
        public float MinRange { get; set; } = 2.0f;
        public float MaxRange { get; set; } = 5.0f;
        public float NoiseMinRange { get; set; } = 5.0f;
        public float NoiseMaxRange { get; set; } = 10.0f;
    }
}