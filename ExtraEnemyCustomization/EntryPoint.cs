using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using EECustom.Customizations.Abilities.Managers;
using EECustom.Customizations.Models.Managers;
using EECustom.Customizations.Shooters.Managers;
using EECustom.Managers;
using EECustom.Utils;
using HarmonyLib;
using UnhollowerRuntimeLib;

namespace EECustom
{
    //TODO: Refactor the CustomBase to support Phase Setting

    [BepInPlugin("GTFO.EECustomization", "EECustom", "0.6.0")]
    [BepInProcess("GTFO.exe")]
    [BepInDependency(MTFOUtil.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(MTFOPartialDataUtil.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class EntryPoint : BasePlugin
    {
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<ShooterDistSettingManager>();
            ClassInjector.RegisterTypeInIl2Cpp<HealthRegenManager>();
            ClassInjector.RegisterTypeInIl2Cpp<PulseManager>();
            ClassInjector.RegisterTypeInIl2Cpp<ScannerManager>();

            Logger.LogInstance = Log;

            var useDevMsg = Config.Bind(new ConfigDefinition("Logging", "UseDevMessage"), false, new ConfigDescription("Using Dev Message for Debugging your config?"));
            var useVerbose = Config.Bind(new ConfigDefinition("Logging", "Verbose"), false, new ConfigDescription("Using Much more detailed Message for Debugging?"));

            Logger.UsingDevMessage = useDevMsg.Value;
            Logger.UsingVerbose = useVerbose.Value;

            var harmony = new Harmony("EECustomization.Harmony");
            harmony.PatchAll();

            ConfigManager.Initialize();
            SpriteManager.Initialize();
        }
    }
}