using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public interface IAbility
    {
        string Name { get; set; }
        void Setup();
        void Unload();
        void Trigger(EnemyAgent agent);
        void TriggerAll();
        AbilityBehaviour RegisterBehaviour(EnemyAgent agent);
        bool TryGetBehaviour(EnemyAgent agent, out AbilityBehaviour behaviour);
        AbilityBehaviour[] Behaviours { get; }
    }
}
