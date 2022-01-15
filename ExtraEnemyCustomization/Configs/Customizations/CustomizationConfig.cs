using EECustom.Customizations;
using System.Text.Json.Serialization;

namespace EECustom.Configs.Customizations
{
    public abstract class CustomizationConfig : Config
    {
        [JsonIgnore]
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