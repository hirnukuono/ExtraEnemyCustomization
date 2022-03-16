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
        public bool ProjectileExplodesOnWorld { get; set; } = true;
        public bool ProjectileExplodesOnPlayer { get; set; } = true;

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
        }
        public override void OnConfigUnloaded()
        {
            base.OnConfigUnloaded();
            if (ProjectileExplodesOnWorld)
            {
                ProjectileEvents.CollidedWorld -= ProjectileEvents_CollidedWorld;
            }
        }

        private void ProjectileEvents_CollidedWorld(ProjectileBase projectile, GameObject world)
        {
            if (projectile.TryGetOwner(out var agent))
            {
                if (IsTarget(agent))
                {
                    ProjectileData.DoExplode(projectile.transform.position);

                    if (ProjectileData.KillInflictor)
                    {
                        var damage = agent.Damage;
                        damage.ExplosionDamage(damage.HealthMax, Vector3.zero, Vector3.zero);
                    }
                }
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

        protected override void OnApplyProjectileEffect(ExplosionSetting setting, PlayerAgent player, EnemyAgent inflictor, ProjectileBase projectile)
        {
            if (ProjectileExplodesOnPlayer)
            {
                OnApplyEffect(setting, player, inflictor);
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