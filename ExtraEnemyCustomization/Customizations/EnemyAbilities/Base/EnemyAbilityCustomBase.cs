using EECustom.Customizations.EnemyAbilities.Abilities;
using EECustom.Extensions;
using Enemies;
using System.Collections.Generic;

namespace EECustom.Customizations.EnemyAbilities
{
    public abstract class EnemyAbilityCustomBase<T> : EnemyCustomBase, IEnemySpawnedEvent where T : AbilitySettingBase
    {
        public T[] Abilities { get; set; } = new T[0];

        public sealed override void OnConfigLoaded()
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
            foreach (var ab in Abilities)
            {
                var newBehaviour = ab.Ability.RegisterBehaviour(agent);
                OnBehaviourAssigned(agent, newBehaviour, ab);
            }

            agent.AddOnDeadOnce(() =>
            {
                OnDead(agent);
            });
            OnSpawnedPost(agent);
        }

        public virtual void OnBehaviourAssigned(EnemyAgent agent, AbilityBehaviour behaviour, T setting)
        {

        }

        public virtual void OnSpawnedPost(EnemyAgent agent)
        {

        }

        public virtual void OnDead(EnemyAgent agent)
        {

        }
    }
}
