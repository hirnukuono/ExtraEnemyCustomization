using EECustom.Customizations;
using EECustom.Managers;
using EECustom.Utils;
using Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace EECustom.API
{
    public static class EECCustomizationAPI
    {
        public static event Action<EnemyAgent> SpawnCustomizationDone;

        internal static void OnSpawnCustomizationDone_Internal(EnemyAgent agent)
        {
            SpawnCustomizationDone?.Invoke(agent);
        }

        public static bool HasCustomization(uint enemyID, string customizationTypeName)
        {
            var list = GetCustomizationsOf(enemyID, customizationTypeName);
            return list.Any();
        }

        public static bool TryGetCustomizations(uint enemyID, string customizationTypeName, out IEnumerable<EnemyCustomBase> customs)
        {
            var list = GetCustomizationsOf(enemyID, customizationTypeName);
            if (list.Any())
            {
                customs = list;
                return true;
            }
            else
            {
                customs = Enumerable.Empty<EnemyCustomBase>();
                return false;
            }
        }

        public static bool TryGetCustomizationJsons(uint enemyID, string customizationTypeName, out IEnumerable<string> jsons)
        {
            var list = GetCustomizationsOf(enemyID, customizationTypeName);
            if (list.Any())
            {
                var tempList = new List<string>();
                foreach (var item in list)
                {
                    tempList.Add(JsonSerializer.Serialize(item, item.GetType()));
                }
                jsons = tempList;
                return true;
            }
            else
            {
                jsons = Enumerable.Empty<string>();
                return false;
            }
        }

        public static IEnumerable<EnemyCustomBase> GetCustomizationsOf(uint enemyID, string customizationTypeName)
        {
            if (ConfigManager.Current == null)
                return Enumerable.Empty<EnemyCustomBase>();

            var buffer = ConfigManager.Current.CustomizationBuffer;

            if (!buffer.Any())
                return Enumerable.Empty<EnemyCustomBase>();

            var list = buffer.Where(x => x.GetType().Name.InvariantEquals(customizationTypeName, ignoreCase: true))
                .Where(x => x.Enabled && x.Target.IsMatch(enemyID));

            if (list != null)
                return list;
            else
                return Enumerable.Empty<EnemyCustomBase>();
        }
    }
}
