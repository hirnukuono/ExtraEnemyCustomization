using BepInEx;
using BepInEx.IL2CPP;
using EECustom.Attributes;
using EECustom.Events;
using EECustom.Managers;
using EECustom.Managers.Assets;
using EECustom.Networking;
using EECustom.Utils;
using EECustom.Utils.Integrations;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnhollowerRuntimeLib;

namespace EECustom
{
    //TODO: - Tentacle Hibernation : Possible
    //TODO: - Alerts Logic Behaviour : Maybe Possible, Can't guarantee for every option though
    //TODO: - Scout Scream Damage : Possible? Maybe?
    //TODO: - Enemy Scream Infection/Damage : Ugh....What?
    //TODO: - Patrolling Hibernation : Too many works to do with this one, this is one of the long term goal
    //TODO: Refactor the CustomBase to support Phase Setting

    [BepInPlugin("GTFO.EECustomization", "EECustom", "1.4.3")]
    [BepInProcess("GTFO.exe")]
    [BepInDependency(MTFOUtil.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(MTFOPartialDataUtil.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    internal class EntryPoint : BasePlugin
    {
        public static Harmony HarmonyInstance { get; private set; }
        public static string BasePath { get; private set; }

        public override void Load()
        {
            Configuration.CreateAndBindAll();
            Logger.Initialize();

            InjectAllIl2CppType();
            CallAllAutoConstructor();

            BasePath = Path.Combine(MTFOUtil.CustomPath, "ExtraEnemyCustomization");

            HarmonyInstance = new Harmony("EEC.Harmony");
            HarmonyInstance.PatchAll();

            NetworkManager.Initialize();
            ConfigManager.Initialize();
            if (Configuration.DumpConfig)
            {
                ConfigManager.DumpDefault();
            }

            AssetEvents.AllAssetLoaded += AllAssetLoaded;
            AssetCacheManager.OutputMethod = Configuration.AssetCacheBehaviour;
        }

        private void AllAssetLoaded()
        {
            SpriteManager.Initialize();
            ThreadDispatcher.Initialize();
            AssetCacheManager.AssetLoaded();

            ConfigManager.FireAssetLoaded();
            ConfigManager.FirePrefabBuildEventAll(rebuildPrefabs: false);
        }

        public override bool Unload()
        {
            UninjectAllIl2CppType();

            HarmonyInstance.UnpatchSelf();
            ConfigManager.UnloadAllConfig(doClear: true);
            return base.Unload();
        }

        private void InjectAllIl2CppType()
        {
            Logger.Debug($"Injecting IL2CPP Types");
            var types = GetAllHandlers();

            Logger.Debug($" - Count: {types.Count()}");
            foreach (var type in types)
            {
                //Log.LogDebug($" - {type.Name}"); Class Injector already shows their type names
                if (ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                    continue;

                ClassInjector.RegisterTypeInIl2Cpp(type);
            }
        }

        private void CallAllAutoConstructor()
        {
            Logger.Debug($"Calling Necessary Static .ctors");
            var ctorsTypes = GetAllAutoConstructor();

            Logger.Debug($" - Count: {ctorsTypes.Count()}");
            foreach (var ctor in ctorsTypes)
            {
                Logger.Debug($"calling ctor of: {ctor.Name}");
                RuntimeHelpers.RunClassConstructor(ctor.TypeHandle);
            }
        }

        private void UninjectAllIl2CppType()
        {
            Logger.Debug($"Uninjecting IL2CPP Types");
            var types = GetAllHandlers();

            Logger.Debug($" - Count: {types.Count()}");
            foreach (var type in types)
            {
                if (!ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                    continue;

                //ClassInjector.UnregisterTypeInIl2Cpp(type);
            }
        }

        private IEnumerable<Type> GetAllAutoConstructor()
        {
            return GetType().Assembly.GetTypes().Where(type => type != null && Attribute.IsDefined(type, typeof(CallConstructorOnLoadAttribute)));
        }

        private IEnumerable<Type> GetAllHandlers()
        {
            return GetType().Assembly.GetTypes().Where(type => type != null && Attribute.IsDefined(type, typeof(InjectToIl2CppAttribute)));
        }
    }
}