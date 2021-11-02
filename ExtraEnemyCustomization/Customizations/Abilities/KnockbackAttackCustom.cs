using Agents;
using EECustom.Events;
using Enemies;
using Player;
using UnityEngine;

namespace EECustom.Customizations.Abilities
{
    public class KnockbackAttackCustom : EnemyCustomBase
    {
        public KnockbackData MeleeData { get; set; } = new();
        public KnockbackData TentacleData { get; set; } = new();

        public override string GetProcessName()
        {
            return "KnockbackAttack";
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
                var enemyAgent = inflictor.TryCast<EnemyAgent>();
                if (enemyAgent != null)
                    ApplyPushForce(MeleeData, player, enemyAgent);
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                var enemyAgent = inflictor.TryCast<EnemyAgent>();
                if (enemyAgent != null)
                    ApplyPushForce(TentacleData, player, enemyAgent);
            }
        }

        public void ApplyPushForce(KnockbackData data, PlayerAgent player, EnemyAgent inflictor)
        {
            var playerPos = player.Position;
            var enemyPos = inflictor.Position;
            var powerVec = playerPos - enemyPos;

            var distance = powerVec.magnitude;
            var direction = powerVec / distance;

            var velocity = direction * data.Velocity;
            var velocityZ = Vector3.up * data.VelocityZ;
            if (data.DoMultDistance)
            {
                velocity *= distance;
            }

            if (data.DoMultDistanceZ)
            {
                velocityZ *= distance;
            }

            LogVerbose("Push Force (Look): " + velocity.ToString());
            LogVerbose("Push Force (Z): " + velocityZ.ToString());

            player.Locomotion.AddExternalPushForce(velocity);

            if (data.VelocityZ != 0.0f && player.Alive)
            {
                LogVerbose("Has Z Velocity!");
                player.Locomotion.ChangeState(PlayerLocomotion.PLOC_State.Jump, true);
                player.Locomotion.VerticalVelocity = velocity + velocityZ;
            }
        }
    }

    public class KnockbackData
    {
        public float Velocity { get; set; } = 0.0f;
        public float VelocityZ { get; set; } = 0.0f;
        public bool DoMultDistance { get; set; } = false;
        public bool DoMultDistanceZ { get; set; } = false;
    }
}