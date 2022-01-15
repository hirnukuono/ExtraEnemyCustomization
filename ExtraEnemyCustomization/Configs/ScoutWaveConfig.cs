using EECustom.CustomSettings.DTO;

namespace EECustom.Configs
{
    public class ScoutWaveConfig : Config
    {
        public ExpeditionScoutSetting[] Expeditions { get; set; } = new ExpeditionScoutSetting[0];
        public ScoutTargetSetting[] TargetSettings { get; set; } = new ScoutTargetSetting[0];
        public ScoutWaveSetting[] WaveSettings { get; set; } = new ScoutWaveSetting[0];

        public override string FileName => "ScoutWave";
    }
}