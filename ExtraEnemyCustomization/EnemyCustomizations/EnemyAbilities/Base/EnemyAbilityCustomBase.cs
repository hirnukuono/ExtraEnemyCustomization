using EEC.EnemyCustomizations.EnemyAbilities.Abilities;
using Enemies;
using GTFO.API.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EEC.EnemyCustomizations.EnemyAbilities
{
    public abstract class EnemyAbilityCustomBase<T> : EnemyCustomBase, IEnemySpawnedEvent where T : AbilitySettingBase
    {
        public T[] Abilities { get; set; } = Array.Empty<T>();

        public override sealed void OnConfigLoaded()
        {
            Abilities = CacheAbilities(Abilities);
            OnConfigLoadedPost();
        }

        protected T[] CacheAbilities(T[] settings)
        {
            var settingsList = new List<T>(settings);
            foreach (var ab in settings)
            {
                if (!ab.TryCache())
                {
                    LogError($"Key: [{ab.AbilityName}] was missing, unable to apply ability!");
                    settingsList.Remove(ab);
                    continue;
                }
            }

            return settingsList.ToArray();
        }

        public virtual void OnConfigLoadedPost()
        {
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var length = Abilities.Length;
            T ability;
            AbilityBehaviour newBehaviour;
            for (int i = 0; i < length; i++)
            {
                ability = Abilities[i];
                newBehaviour = ability.Ability.RegisterBehaviour(agent);
                OnBehaviourAssigned(agent, newBehaviour, ability);
            }

            OnSpawnedPost(agent);
        }

        public static void DoTriggerDelayed(IAbility ability, EnemyAgent agent, float delay)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay((int)Math.Round(delay * 1000.0f));
                ThreadDispatcher.Dispatch(() =>
                {
                    ability?.TriggerSync(agent);
                });
            });
        }

        public virtual void OnBehaviourAssigned(EnemyAgent agent, AbilityBehaviour behaviour, T setting)
        {
        }

        public virtual void OnSpawnedPost(EnemyAgent agent)
        {
        }
    }
}