using GTFO.API.Utilities;
using System.IO;

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
}