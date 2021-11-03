using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using EECustom.Attributes;
using EECustom.Managers;
using EECustom.Utils.Integrations;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnhollowerRuntimeLib;

namespace EECustom
{
    //TODO: - Tentacle Hibernation : Possible
    //TODO: - Alerts Logic Behaviour : Maybe Possible, Can't guarantee for every option though
    //TODO: - Scout Scream Damage : Possible? Maybe?
    //TODO: - Enemy Scream Infection/Damage : Ugh....What?
    //TODO: - Patrolling Hibernation : Too many works to do with this one, this is one of the long term goal
    //TODO: Refactor the CustomBase to support Phase Setting

    [BepInPlugin("GTFO.EECustomization", "EECustom", "0.8.1")]
    [BepInProcess("GTFO.exe")]
    [BepInDependency(MTFOUtil.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(MTFOPartialDataUtil.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class EntryPoint : BasePlugin
    {
        public static Harmony HarmonyInstance { get; private set; }

        public override void Load()
        {
            InjectAllIl2CppType();

            Logger.LogInstance = Log;

            var useDevMsg = Config.Bind(new ConfigDefinition("Logging", "UseDevMessage"), false, new ConfigDescription("Using Dev Message for Debugging your config?"));
            var useVerbose = Config.Bind(new ConfigDefinition("Logging", "Verbose"), false, new ConfigDescription("Using Much more detailed Message for Debugging?"));

            Logger.UsingDevMessage = useDevMsg.Value;
            Logger.UsingVerbose = useVerbose.Value;

            HarmonyInstance = new Harmony("EECustomization.Harmony");
            HarmonyInstance.PatchAll();

            SpriteManager.Initialize();
            ConfigManager.Initialize();
        }

        public override bool Unload()
        {
            UninjectAllIl2CppType();

            HarmonyInstance.UnpatchAll();
            ConfigManager.UnloadConfig(doClear: true);
            return base.Unload();
        }

        private void InjectAllIl2CppType()
        {
            Log.LogDebug($"Injecting IL2CPP Types");
            var types = GetAllHandlers();

            Log.LogDebug($" - Count: {types.Count()}");
            foreach (var type in types)
            {
                //Log.LogDebug($" - {type.Name}"); Class Injector already shows their type names
                if (ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                    continue;

                ClassInjector.RegisterTypeInIl2Cpp(type);
            }
        }

        private void UninjectAllIl2CppType()
        {
            Log.LogDebug($"Uninjecting IL2CPP Types");
            var types = GetAllHandlers();

            Log.LogDebug($" - Count: {types.Count()}");
            foreach (var type in types)
            {
                if (!ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                    continue;

                //ClassInjector.UnregisterTypeInIl2Cpp(type);
            }
        }

        private IEnumerable<Type> GetAllHandlers()
        {
            return GetType().Assembly.GetTypes().Where(type => type != null && type.GetCustomAttributes(typeof(InjectToIl2CppAttribute), false).FirstOrDefault() != null);
        }
    }
}