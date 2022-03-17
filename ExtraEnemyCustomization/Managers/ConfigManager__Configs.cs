using EEC.Configs;
using EEC.Utils;
using EEC.Utils.Integrations;
using System;
using System.IO;
using System.Linq;

namespace EEC.Managers
{
    public static partial class ConfigManager
    {
        internal static void DumpDefault()
        {
            var dumpPath = Path.Combine(BasePath, "Dump");
            Directory.CreateDirectory(dumpPath);

            foreach (var item in _configInstances)
            {
                var file = Path.Combine(dumpPath, $"{item.Key}.json");
                File.WriteAllText(file, JSON.Serialize(item.Value.CreateBlankConfig(), item.Value.GetType()));
            }
        }

        internal static void ReloadConfig()
        {
            Logger.Log("HOT RELOADING CONFIG!");

            UnloadAllConfig(doClear: true);
            LoadAllConfig();
            GenerateBuffer();

            FireAssetLoaded();
            FirePrefabBuildEventAll(rebuildPrefabs: true);
        }

        internal static void UnloadAllConfig(bool doClear)
        {
            foreach (var config in _customizationBuffer)
            {
                config.OnConfigUnloaded();
                config.ClearTargetLookup();
            }

            foreach (var item in _configInstances)
            {
                item.Value.Unloaded();
            }

            if (doClear)
            {
                ClearConfigs();
            }
        }

        internal static void ClearConfigs()
        {
            foreach (var item in _configInstances.ToArray())
            {
                _configInstances[item.Key] = Activator.CreateInstance(item.Value.GetType()) as Config;
            }
        }

        internal static void LoadAllConfig()
        {
            if (MTFOUtil.IsLoaded && MTFOUtil.HasCustomContent)
            {
                foreach (var configType in _configTypes)
                {
                    LoadConfig(configType);
                }
            }
            else
            {
                Logger.Warning("No Custom content were found, No Customization will be applied");
            }
        }

        internal static void LoadConfig(Type configType)
        {
            try
            {
                if (!_configTypeToFileName.TryGetValue(configType, out var name))
                {
                    throw new ArgumentOutOfRangeException(nameof(configType));
                }

                Logger.Debug($"Loading '{name}' Config...");

                if (TryGetExistingConfigPath(name, out var path))
                {
                    Logger.Verbose($" - Full Path: {path}");

                    if (!TryLoadConfigData(path, configType, out var config))
                    {
                        return;
                    }

                    _configInstances[name] = config;
                    config.Loaded();
                }
                else
                {
                    Logger.Warning($"Config file for '{name}' is not exist, ignoring this config...");
                }

                var cacheProperty = _cacheProperties
                        .Where(x => x.PropertyType == configType)
                        .FirstOrDefault();

                if (cacheProperty != null)
                {
                    cacheProperty.SetValue(null, _configInstances[name]);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error Occured While reading Config from type: {configType.Name}\n{e}");
            }
        }

        private static bool TryGetExistingConfigPath(string name, out string path)
        {
            var fileName = $"{name}.jsonc";
            var filePath = Path.Combine(BasePath, fileName);

            if (File.Exists(filePath))
            {
                path = filePath;
                return true;
            }

            fileName = $"{name}.json";
            filePath = Path.Combine(BasePath, fileName);

            if (File.Exists(filePath))
            {
                path = filePath;
                return true;
            }
            else
            {
                path = string.Empty;
                return false;
            }
        }

        private static bool TryLoadConfigData(string filePath, Type type, out Config config)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    config = JSON.Deserialize(type, File.ReadAllText(filePath)) as Config;
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error($"Exception Occured While reading {filePath} file: {e}");
                    config = default;
                    return false;
                }
            }
            else
            {
                Logger.Warning($"File: {filePath} is not exist, ignoring this config...");
                config = default;
                return false;
            }
        }
    }
}