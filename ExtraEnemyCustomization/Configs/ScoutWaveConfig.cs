using EEC.CustomSettings.CustomScoutWaves;
using System;

namespace EEC.Configs
{
    public sealed class ScoutWaveConfig : Config
    {
        public ExpeditionScoutSetting[] Expeditions { get; set; } = Array.Empty<ExpeditionScoutSetting>();
        public ScoutTargetSetting[] TargetSettings { get; set; } = Array.Empty<ScoutTargetSetting>();
        public ScoutWaveSetting[] WaveSettings { get; set; } = Array.Empty<ScoutWaveSetting>();

        public override string FileName => "ScoutWave";

        public override void Loaded()
        {
            CustomScoutWaveManager.AddScoutSetting(Expeditions);
            CustomScoutWaveManager.AddTargetSetting(TargetSettings);
            CustomScoutWaveManager.AddWaveSetting(WaveSettings);
        }

        public override void Unloaded()
        {
            CustomScoutWaveManager.ClearAll();
        }
    }
}