using EECustom.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EECustom.Managers
{
    public partial class ConfigManager
    {
        private static void OnConfigFileEdited_ReloadConfig(object sender, FileSystemEventArgs e)
        {
            ThreadDispatcher.Enqueue(JobComplexity.Heavy, () =>
            {
                var filename = Path.GetFileNameWithoutExtension(e.Name);
                if (_configFileNameToType.TryGetValue(filename.ToLowerInvariant(), out var type))
                {
                    filename = _configTypeToFileName[type];

                    Logger.Log($"Config File Changed: {filename}");

                    ReloadConfig();

                    //TODO: Implement Reload Individual File

                    /*
                    _configInstances[filename].Unloaded();
                    if (_configInstances[filename] is CustomizationConfig custom)
                    {
                        var settings = custom.GetAllSettings();
                        foreach (var setting in settings)
                        {
                            setting.OnConfigUnloaded();
                        }
                    }
                    LoadConfig(type);
                    Current.GenerateBuffer();
                    */
                }
            });
        }
    }
}
