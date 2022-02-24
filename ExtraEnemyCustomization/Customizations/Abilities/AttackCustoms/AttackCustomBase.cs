using Agents;
using EECustom.Events;
using Enemies;
using Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Abilities
{
    public abstract class AttackCustomBase<T> : EnemyCustomBase where T : new()
    {
        public T MeleeData { get; set; } = new();
        public T TentacleData { get; set; } = new();

        public override void OnConfigLoaded()
        {
            LocalPlayerDamageEvents.MeleeDamage += OnMelee;
            LocalPlayerDamageEvents.TentacleDamage += OnTentacle;
        }

        public override void OnConfigUnloaded()
        {
            LocalPlayerDamageEvents.MeleeDamage -= OnMelee;
            LocalPlayerDamageEvents.TentacleDamage -= OnTentacle;
        }

        private void OnMelee(PlayerAgent player, Agent inflictor, float damage)
        {
            Do(player, inflictor, MeleeData);
        }

        private void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            Do(player, inflictor, TentacleData);
        }

        private void Do(PlayerAgent player, Agent inflictor, T setting)
        {
            if (inflictor.TryCastToEnemyAgent(out var enemy))
            {
                if (IsTarget(enemy))
                {
                    OnApplyEffect(setting, player, enemy);
                }
            }
        }

        protected abstract void OnApplyEffect(T setting, PlayerAgent player, EnemyAgent inflicator);
    }
}
