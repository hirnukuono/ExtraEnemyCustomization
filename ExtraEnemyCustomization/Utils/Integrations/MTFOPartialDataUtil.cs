using BepInEx.IL2CPP;
using System;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace EEC.Utils.Integrations
{
    public static class MTFOPartialDataUtil
    {
        public const string PLUGIN_GUID = "MTFO.Extension.PartialBlocks";

        public delegate bool TryGetDelegate(string guid, out uint id);

        public static JsonConverter PersistentIDConverter { get; private set; } = null;
        public static JsonConverter LocalizedTextConverter { get; private set; } = null;
        public static bool IsLoaded { get; private set; } = false;
        public static bool Initialized { get; private set; } = false;
        public static string PartialDataPath { get; private set; } = string.Empty;
        public static string ConfigPath { get; private set; } = string.Empty;

        private static readonly TryGetDelegate _tryGetIDDelegate;

        static MTFOPartialDataUtil()
        {
            if (IL2CPPChainloader.Instance.Plugins.TryGetValue(PLUGIN_GUID, out var info))
            {
                try
                {
                    var ddAsm = info?.Instance?.GetType()?.Assembly ?? null;

                    if (ddAsm is null)
                        throw new Exception("Assembly is Missing!");

                    var types = ddAsm.GetTypes();
                    var converterType = types.First(t => t.Name == "PersistentIDConverter");
                    if (converterType is null)
                        throw new Exception("Unable to Find PersistentIDConverter Class");

                    var dataManager = types.First(t => t.Name == "PartialDataManager");
                    if (dataManager is null)
                        throw new Exception("Unable to Find PartialDataManager Class");

                    var initProp = dataManager.GetProperty("Initialized", BindingFlags.Public | BindingFlags.Static);
                    var dataPathProp = dataManager.GetProperty("PartialDataPath", BindingFlags.Public | BindingFlags.Static);
                    var configPathProp = dataManager.GetProperty("ConfigPath", BindingFlags.Public | BindingFlags.Static);

                    if (initProp is null)
                        throw new Exception("Unable to Find Property: Initialized");

                    if (dataPathProp is null)
                        throw new Exception("Unable to Find Property: PartialDataPath");

                    if (configPathProp is null)
                        throw new Exception("Unable to Find Field: ConfigPath");

                    Initialized = (bool)initProp.GetValue(null);
                    PartialDataPath = (string)dataPathProp.GetValue(null);
                    ConfigPath = (string)configPathProp.GetValue(null);

                    var persistentIDManager = types.First(t => t.Name == "PersistentIDManager");
                    if (persistentIDManager is null)
                        throw new Exception("Unable to Find PersistentIDManager Class");

                    var tryGetDelegate = persistentIDManager.GetMethod("TryGetId", BindingFlags.Public | BindingFlags.Static);
                    _tryGetIDDelegate = (TryGetDelegate)tryGetDelegate.CreateDelegate(typeof(TryGetDelegate));

                    PersistentIDConverter = (JsonConverter)Activator.CreateInstance(converterType);
                    IsLoaded = true;
                }
                catch (Exception e)
                {
                    Logger.Error($"Exception thrown while reading data from MTFO_Extension_PartialData:\n{e}");
                }
            }
        }

        public static bool TryGetId(string guid, out uint id)
        {
            if (!IsLoaded)
            {
                id = 0;
                return false;
            }

            if (!Initialized)
            {
                id = 0;
                return false;
            }

            if (_tryGetIDDelegate == null)
            {
                id = 0;
                return false;
            }

            return _tryGetIDDelegate.Invoke(guid, out id);
        }
    }
}