using EECustom.Customizations.Shared;
using EECustom.Events;
using Enemies;
using Player;
using System;
using UnityEngine;

namespace EECustom.Customizations.Abilities
{
    public sealed class ExplosiveAttackCustom : AttackCustomBase<ExplosionSetting>
    {
        public override bool DisableProjectileDamageEvent => true;

        public bool ProjectileExplodesOnWorld { get; set; } = false;
        public bool ProjectileExplodesOnPlayer { get; set; } = false;

        public override string GetProcessName()
        {
            return "ExplosiveAttack";
        }

        public override void OnConfigLoaded()
        {
            base.OnConfigLoaded();
            if (ProjectileExplodesOnWorld)
            {
                ProjectileEvents.CollidedWorld += ProjectileEvents_CollidedWorld;
            }

            if (ProjectileExplodesOnPlayer)
            {
                ProjectileEvents.CollidedPlayer += ProjectileEvents_CollidedPlayer;
            }
        }

        public override void OnConfigUnloaded()
        {
            base.OnConfigUnloaded();
            if (ProjectileExplodesOnWorld)
            {
                ProjectileEvents.CollidedWorld -= ProjectileEvents_CollidedWorld;
            }

            if (ProjectileExplodesOnPlayer)
            {
                ProjectileEvents.CollidedPlayer -= ProjectileEvents_CollidedPlayer;
            }
        }

        private void ProjectileEvents_CollidedWorld(ProjectileBase projectile, GameObject _)
            => TriggerProjectileExplosion(projectile);

        private void ProjectileEvents_CollidedPlayer(ProjectileBase projectile, PlayerAgent _)
            => TriggerProjectileExplosion(projectile);

        private void TriggerProjectileExplosion(ProjectileBase projectile)
        {
            if (!projectile.TryGetOwner(out var agent))
                return;

            if (!IsTarget(agent))
                return;

            ProjectileData.DoExplode(projectile.transform.position);

            if (ProjectileData.KillInflictor)
            {
                var damage = agent.Damage;
                damage.ExplosionDamage(damage.HealthMax, Vector3.zero, Vector3.zero);
            }
        }

        protected override void OnApplyEffect(ExplosionSetting setting, PlayerAgent player, EnemyAgent inflictor)
        {
            setting.DoExplode(player);

            if (setting.KillInflictor)
            {
                var damage = inflictor.Damage;
                damage.ExplosionDamage(damage.HealthMax, Vector3.zero, Vector3.zero);
            }
        }
    }

    [Flags]
    public enum ProjectileExplosionTarget
    {
        Player = 1,
        World = 2
    }
}