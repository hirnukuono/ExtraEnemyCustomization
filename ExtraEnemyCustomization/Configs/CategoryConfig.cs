using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EECustom.Configs
{
    public class CategoryConfig
    {
        public CategoryDefinition[] Categories { get; set; } = new CategoryDefinition[0];

        private Dictionary<string, CategoryDefinition> CategoryCache = new Dictionary<string, CategoryDefinition>();

        internal void Cache()
        {
            foreach(var category in Categories)
            {
                if (CategoryCache.ContainsKey(category.Name))
                {
                    Logger.Error($"Overlapping Category Found, Category Name: {category.Name}");
                    continue;
                }

                CategoryCache.Add(category.Name, category);
                Logger.Debug($"Category Initialized! '{category.Name}', ids: [{string.Join(", ", category.PersistentIDs)}]");
            }
        }

        public bool Any(string[] categories, uint enemyID)
        {
            foreach(var category in categories)
            {
                if (!CategoryCache.TryGetValue(category, out var categoryDef))
                {
                    Logger.Warning($"Unable to find Category with name: {category}");
                    continue;
                }

                if (categoryDef.PersistentIDs.Contains(enemyID))
                {
                    return true;
                }
            }

            return false;
        }

        public bool All(string[] categories, uint enemyID)
        {
            var result = true;
            foreach (var category in categories)
            {
                if (!CategoryCache.TryGetValue(category, out var categoryDef))
                {
                    Logger.Warning($"Unable to find Category with name: {category}");
                    result = false;
                    break;
                }

                if (!categoryDef.PersistentIDs.Contains(enemyID))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }

    public class CategoryDefinition
    {
        public string Name { get; set; }
        public uint[] PersistentIDs { get; set; } = new uint[1] { 0u };
    }
}
