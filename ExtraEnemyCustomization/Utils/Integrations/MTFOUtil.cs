using BepInEx;
using BepInEx.IL2CPP;
using System;
using System.Linq;
using System.Reflection;

namespace EECustom.Utils.Integrations
{
    public static class MTFOUtil
    {
        public const string PLUGIN_GUID = "com.dak.MTFO";

        public static readonly SemanticVersioning.Version MTFO_V2 = new("999.999.999");
        public static string GameDataPath { get; private set; } = string.Empty;
        public static string CustomPath { get; private set; } = string.Empty;
        public static bool HasCustomContent { get; private set; } = false;
        public static bool IsLoaded { get; private set; } = false;
        public static bool HasHotReload { get; private set; } = false;

        

        static MTFOUtil()
        {
            if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(PLUGIN_GUID, out var info))
                return;

            var version = info.Metadata.Version;

            if (version < MTFO_V2)
            {
                InitMTFO_V1(info);
            }
            else if (version >= MTFO_V2)
            {
                InitMTFO_V2(info);
            }
        }

        private static void InitMTFO_V1(PluginInfo info)
        {
            try
            {
                var ddAsm = info?.Instance?.GetType()?.Assembly ?? null;

                if (ddAsm is null)
                    throw new Exception("Assembly is Missing!");

                var types = ddAsm.GetTypes();
                var cfgManagerType = types.First(t => t.Name == "ConfigManager");

                if (cfgManagerType is null)
                    throw new Exception("Unable to Find ConfigManager Class");

                var dataPathField = cfgManagerType.GetField("GameDataPath", BindingFlags.Public | BindingFlags.Static);
                var customPathField = cfgManagerType.GetField("CustomPath", BindingFlags.Public | BindingFlags.Static);
                var hasCustomField = cfgManagerType.GetField("HasCustomContent", BindingFlags.Public | BindingFlags.Static);

                if (dataPathField is null)
                    throw new Exception("Unable to Find Field: GameDataPath");

                if (customPathField is null)
                    throw new Exception("Unable to Find Field: CustomPath");

                if (hasCustomField is null)
                    throw new Exception("Unable to Find Field: HasCustomContent");

                GameDataPath = (string)dataPathField.GetValue(null);
                CustomPath = (string)customPathField.GetValue(null);
                HasCustomContent = (bool)hasCustomField.GetValue(null);
                IsLoaded = true;
            }
            catch (Exception e)
            {
                Logger.Error($"Exception thrown while reading path from DataDumper(MTFO V1): \n{e}");
            }
        }

        private static void InitMTFO_V2(PluginInfo info)
        {
            try
            {
                var ddAsm = info?.Instance?.GetType()?.Assembly ?? null;

                if (ddAsm is null)
                    throw new Exception("Assembly is Missing!");

                var types = ddAsm.GetTypes();
                var cfgManagerType = types.First(t => t.Name == "ConfigManager");

                if (cfgManagerType is null)
                    throw new Exception("Unable to Find ConfigManager Class");

                var dataPathField = cfgManagerType.GetField("GameDataPath", BindingFlags.Public | BindingFlags.Static);
                var customPathField = cfgManagerType.GetField("CustomPath", BindingFlags.Public | BindingFlags.Static);
                var hasCustomField = cfgManagerType.GetField("HasCustomContent", BindingFlags.Public | BindingFlags.Static);

                if (dataPathField is null)
                    throw new Exception("Unable to Find Field: GameDataPath");

                if (customPathField is null)
                    throw new Exception("Unable to Find Field: CustomPath");

                if (hasCustomField is null)
                    throw new Exception("Unable to Find Field: HasCustomContent");

                GameDataPath = (string)dataPathField.GetValue(null);
                CustomPath = (string)customPathField.GetValue(null);
                HasCustomContent = (bool)hasCustomField.GetValue(null);
                IsLoaded = true;
            }
            catch (Exception e)
            {
                Logger.Error($"Exception thrown while reading path from MTFO (V2): \n{e}");
            }
        }
    }
}