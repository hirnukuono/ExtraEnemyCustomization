using EEC.Configs;
using EEC.Configs.Customizations;
using EEC.EnemyCustomizations;
using EEC.Utils.Json;
using GTFO.API.Utilities;

namespace EEC.Managers
{
    public static partial class ConfigManager
    {
        private static void LiveEdit_FileChanged(LiveEditEventArgs e)
        {
            

            var fileExtension = Path.GetExtension(e.FullPath);
            if (fileExtension.InvariantEquals(".json", ignoreCase: true) ||
                fileExtension.InvariantEquals(".jsonc", ignoreCase: true))
            {
                LiveEdit.TryReadFileContent(e.FullPath, (content) =>
                {
                    var filename = Path.GetFileNameWithoutExtension(e.FullPath);
                    if (_configFileNameToType.TryGetValue(filename.ToLowerInvariant(), out var type))
                    {
                        filename = _configTypeToFileName[type];

                        Logger.Log($"Config File Changed: {filename}");

                        _configInstances[filename].Unloaded();

                        if (_configInstances[filename] is CustomizationConfig oldCustom)
                        {
                            var oldSettings = oldCustom.GetAllSettings();
                            foreach (var setting in oldSettings)
                            {
                                setting.OnConfigUnloaded();
                            }
                        }

                        // Not using LoadConfig since it re-reads the file.
                        // We are using a file handle already, so that errors.
                        try
                        {
                            Config config = (Config) JSON.Deserialize(type, content);
                            _configInstances[filename] = config;
                            config.Loaded();

                            var cacheProperty = _cacheProperties
                            .Where(x => x.PropertyType == type)
                            .FirstOrDefault();
                            cacheProperty?.SetValue(null, _configInstances[filename]);
                        }
                        catch (Exception exc)
                        {
                            Logger.Error($"Exception occurred while reading {e.FullPath} file: {exc}");
                        }

                        if (_configInstances[filename] is CustomizationConfig custom)
                        {
                            try
                            {
                                var newSettings = ((CustomizationConfig)_configInstances[filename]).GetAllSettings();
                                bool hasPrefabEvent = false;
                                foreach (var setting in newSettings)
                                {
                                    setting.OnAssetLoaded();
                                    if (!hasPrefabEvent && setting is IEnemyPrefabBuiltEvent)
                                        hasPrefabEvent = true;
                                }
                                GenerateBuffer();
                                FirePrefabBuildEventAll(rebuildPrefabs: hasPrefabEvent, firePrefabEvents: hasPrefabEvent);
                            }
                            catch (Exception exc)
                            {
                                Logger.Error($"Exception occurred while re-applying configs: {exc}");
                            }
                        }
                    }
                });
            }
        }
    }
}