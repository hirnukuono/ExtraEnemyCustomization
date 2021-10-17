using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using EECustom.Customizations.Abilities.Handlers;
using EECustom.Customizations.Models.Handlers;
using EECustom.Customizations.Shooters.Handlers;
using EECustom.CustomSettings.Handlers;
using EECustom.Managers;
using EECustom.Utils;
using HarmonyLib;
using UnhollowerRuntimeLib;

namespace EECustom
{
    //TODO: Refactor the CustomBase to support Phase Setting

    [BepInPlugin("GTFO.EECustomization", "EECustom", "0.8.1")]
    [BepInProcess("GTFO.exe")]
    [BepInDependency(MTFOUtil.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(MTFOPartialDataUtil.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class EntryPoint : BasePlugin
    {
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<ShooterDistSettingHandler>();
            ClassInjector.RegisterTypeInIl2Cpp<HealthRegenHandler>();
            ClassInjector.RegisterTypeInIl2Cpp<PulseHandler>();
            ClassInjector.RegisterTypeInIl2Cpp<ScannerHandler>();
            ClassInjector.RegisterTypeInIl2Cpp<EnemySilhouette>();
            ClassInjector.RegisterTypeInIl2Cpp<SilhouetteHandler>();
            ClassInjector.RegisterTypeInIl2Cpp<ExplosiveProjectileHandler>();
            ClassInjector.RegisterTypeInIl2Cpp<BleedingHandler>();
            ClassInjector.RegisterTypeInIl2Cpp<EffectFogSphereHandler>();

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