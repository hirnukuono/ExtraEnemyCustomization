using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public class SpawnEnemyAbility : AbilityBase<SpawnEnemyBehaviour>
    {

    }

    public class SpawnEnemyBehaviour : AbilityBehaviour<SpawnEnemyAbility>
    {
        public override bool AllowEABAbilityWhileExecuting => false;
        public override bool IsHostOnlyBehaviour => false;
    }
}
