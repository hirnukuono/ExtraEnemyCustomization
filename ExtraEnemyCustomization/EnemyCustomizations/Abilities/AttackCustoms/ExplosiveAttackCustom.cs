using EEC.EnemyCustomizations.Shared;
using EEC.Events;
using Enemies;
using Player;
using System;
using UnityEngine;

namespace EEC.EnemyCustomizations.Abilities
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
            if (!IsOwnerDestroyed(projectile, out var owner))
            {
                if (!IsTarget(owner))
                    return;

                ProjectileData.DoExplode(projectile.transform.position);

                if (ProjectileData.KillInflictor)
                {
                    var damage = owner.Damage;
                    damage.ExplosionDamage(damage.HealthMax, Vector3.zero, Vector3.zero);
                }
            }
            else if (projectile.TryGetOwnerEnemyDataID(out var ownerID))
            {
                if (!IsTarget(ownerID))
                    return;

                ProjectileData.DoExplode(projectile.transform.position);
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

        private static bool IsOwnerDestroyed(ProjectileBase projectile, out EnemyAgent agent)
        {
            if (!projectile.TryGetOwner(out agent))
                return true;

            if (agent == null)
                return true;

            if (agent.WasCollected)
                return true;

            return false;
        }
    }

    [Flags]
    public enum ProjectileExplosionTarget
    {
        Player = 1,
        World = 2
    }
}