using BepInEx.Unity.IL2CPP;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace EEC.Utils.Integrations
{
    [CallConstructorOnLoad]
    public static class InjectLibUtil
    {
        public const string PLUGIN_GUID = "GTFO.InjectLib";
        public static bool IsLoaded { get; private set; } = false;
        public static JsonConverter? InjectLibConnector { get; private set; } = null; // InjectLibConverter

        static InjectLibUtil()
        {
            if (IL2CPPChainloader.Instance.Plugins.ContainsKey(PLUGIN_GUID))
            {
                IsLoaded = true;
                SetConverter();
            }
            Logger.Debug($"InjectLib is loaded: {IsLoaded}");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void SetConverter()
        {
            InjectLibConnector = new InjectLib.JsonNETInjection.Supports.InjectLibConnector();
        }
    }
}