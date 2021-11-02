using EECustom.Customizations.EnemyAbilities.Abilities;
using EECustom.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.EnemyAbilities
{
    public static class EnemyAbilityManager
    {
        private static readonly Dictionary<string, IAbility> _abilities = new();
        private static bool _allAssetLoaded = false;

        static EnemyAbilityManager()
        {
            _allAssetLoaded = AssetEvents.IsAllAssetLoaded;
            AssetEvents.AllAssetLoaded += AllAssetLoaded;
        }

        private static void AllAssetLoaded()
        {
            _allAssetLoaded = true;
            Setup();
        }

        public static void AddAbility(IAbility ability)
        {
            var key = ability.Name.ToLower();

            if (string.IsNullOrEmpty(key))
            {
                Logger.Error("EnemyAbility Name cannot be empty or null!");
                return;
            }

            if (_abilities.ContainsKey(key))
            {
                Logger.Error($"Duplicated EnemyAbility Name was detected! : \"{key}\"");
                return;
            }
                

            _abilities.Add(key, ability);
        }

        public static IAbility GetAbility(string key)
        {
            key = key.ToLower();
            _abilities.TryGetValue(key, out var ability);
            return ability;
        }

        public static bool TryGetAbility(string key, out IAbility ability)
        {
            return _abilities.TryGetValue(key.ToLower(), out ability);
        }

        public static void Setup()
        {
            if (!_allAssetLoaded)
                return;

            foreach (var ab in _abilities.Values)
                ab.Setup();
        }

        public static void Clear()
        {
            foreach (var ab in _abilities.Values)
                ab.Unload();

            _abilities.Clear();
        }
    }
}
