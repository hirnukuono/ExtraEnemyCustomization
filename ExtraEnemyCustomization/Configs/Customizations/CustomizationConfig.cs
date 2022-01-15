using EECustom.Customizations;

namespace EECustom.Configs.Customizations
{
    public abstract class CustomizationConfig
    {
        public abstract CustomizationConfigType Type { get; }
        public abstract EnemyCustomBase[] GetAllSettings();
    }

    public enum CustomizationConfigType
    {
        Ability,
        Detection,
        EnemyAbility,
        Model,
        Projectile,
        SpawnCost,
        Tentacle
    }
}