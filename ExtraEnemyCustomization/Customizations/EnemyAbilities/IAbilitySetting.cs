using EECustom.Customizations.EnemyAbilities.Abilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.EnemyAbilities
{
    public class AbilitySettingBase
    {
        public string AbilityName { get; set; }
        public IAbility Ability;
    }
}
