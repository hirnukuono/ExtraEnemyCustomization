using EECustom.Customizations;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace EECustom.Configs.Customizations
{
    public abstract class CustomizationConfig : Config
    {
        [JsonIgnore]
        public abstract CustomizationConfigType Type { get; }

        public virtual IEnumerable<EnemyCustomBase> GetAllSettings()
        {
            var list = GetType()?.GetProperties()?
                .Where(x => x.PropertyType != null && typeof(IEnumerable<EnemyCustomBase>).IsAssignableFrom(x.PropertyType))
                .Select(x => (IEnumerable<EnemyCustomBase>)x.GetValue(this))
                ?? null;

            if (list != null && list.Any())
            {
                var tempList = new List<EnemyCustomBase>();
                foreach (var items in list)
                {
                    tempList.AddRange(items);
                }

                return tempList;
            }

            return Enumerable.Empty<EnemyCustomBase>();
        }
    }

    public enum CustomizationConfigType
    {
        Ability,
        Detection,
        EnemyAbility,
        Model,
        Projectile,
        Property,
        Tentacle
    }
}