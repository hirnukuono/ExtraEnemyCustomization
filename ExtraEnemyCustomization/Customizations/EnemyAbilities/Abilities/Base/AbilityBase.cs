using EECustom.Events;
using Enemies;
using System.Collections.Generic;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public abstract class AbilityBase<T> : IAbility where T : AbilityBehaviour, new()
    {
        public string Name { get; set; } = string.Empty;

        private bool _isBehavioursDirty = true;
        private readonly Dictionary<ushort, AbilityBehaviour> _behaviourLookup = new();
        private readonly List<AbilityBehaviour> _behaviours = new();
        private AbilityBehaviour[] _behavioursCache = null;

        public AbilityBehaviour[] Behaviours
        {
            get
            {
                if (_isBehavioursDirty)
                {
                    _behavioursCache = _behaviours.ToArray();
                    _isBehavioursDirty = false;
                }

                return _behavioursCache;
            }
        }

        public void Setup()
        {
            OnAbilityLoaded();
        }

        public void Unload()
        {
            _behaviours.Clear();
            OnAbilityUnloaded();
        }

        public void Trigger(EnemyAgent agent)
        {
            if (TryGetBehaviour(agent, out var behaviour))
            {
                behaviour.DoTrigger();
            }
        }

        public void TriggerAll()
        {
            foreach (var behaviour in _behaviours)
            {
                behaviour.DoTrigger();
            }
        }

        public AbilityBehaviour RegisterBehaviour(EnemyAgent agent)
        {
            var id = agent.GlobalID;

            var behaviour = new T();
            behaviour.Setup(agent);
            _behaviours.Add(behaviour);
            _isBehavioursDirty = true;
            _behaviourLookup[id] = behaviour;

            OnBehaviourAssigned(agent, behaviour);

            var handler = agent.gameObject.AddComponent<MonoBehaviourEventHandler>();
            handler.OnDestroyed += (GameObject _) =>
            {
                behaviour.Unload();
                _behaviourLookup.Remove(id);
            };

            return behaviour;
        }

        public virtual void OnAbilityLoaded()
        {
        }

        public virtual void OnAbilityUnloaded()
        {
        }

        public virtual void OnBehaviourAssigned(EnemyAgent agent, T behaviour)
        {
        }

        public bool TryGetBehaviour(EnemyAgent agent, out AbilityBehaviour behaviour)
        {
            return _behaviourLookup.TryGetValue(agent.GlobalID, out behaviour);
        }
    }
}