using EECustom.Customizations.EnemyAbilities.Abilities;

namespace EECustom.Customizations.EnemyAbilities
{
    public class AbilitySettingBase
    {
        public string AbilityName { get; set; }
        public IAbility Ability;

        public bool TryCache()
        {
            return EnemyAbilityManager.TryGetAbility(AbilityName, out Ability);
        }
    }
}
