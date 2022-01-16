using EECustom.Customizations;
using GameData;
using LevelGeneration;

namespace EECustom.CustomSettings.DTO
{
    public class ExpeditionScoutSetting
    {
        public string[] Targets { get; set; } = new string[0]; //A* //*1 //A1
        public ScoutWaveSet[] ScoutSettings { get; set; } = new ScoutWaveSet[0];

        public bool IsMatch(eRundownTier tier, int index)
        {
            var tierStr = tier switch
            {
                eRundownTier.TierA => "A",
                eRundownTier.TierB => "B",
                eRundownTier.TierC => "C",
                eRundownTier.TierD => "D",
                eRundownTier.TierE => "E",
                _ => "?"
            };

            foreach (var target in Targets)
            {
                if (target.Equals("*"))
                    return true;

                var targetExpStr = target.Trim().ToUpper();
                if (targetExpStr.Length < 2)
                    continue;

                var settingTierStr = targetExpStr.Substring(0, 1);
                var settingNumStr = targetExpStr[1..];
                var num = -1;

                switch (settingTierStr)
                {
                    case "A":
                    case "B":
                    case "C":
                    case "D":
                    case "E":
                        settingTierStr = targetExpStr.Substring(0, 1);
                        break;

                    case "*":
                        settingTierStr = string.Empty;
                        break;

                    default:
                        continue;
                }

                if (string.IsNullOrEmpty(settingTierStr) || settingTierStr.Equals(tierStr))
                {
                    if (settingNumStr.Equals("*"))
                        return true;

                    if (int.TryParse(settingNumStr, out num))
                        if (num == index + 1)
                            return true;
                }
            }

            return false;
        }
    }

    public class ScoutWaveSet
    {
        public string TargetSetting { get; set; } = string.Empty;
        public string WaveSetting { get; set; } = string.Empty;
    }

    public class ScoutTargetSetting
    {
        public string Name { get; set; } = string.Empty;
        public TargetSetting Target { get; set; } = new TargetSetting();
    }

    public class ScoutWaveSetting
    {
        public string Name { get; set; } = string.Empty;
        public WaveSetting[][] Waves { get; set; } = new WaveSetting[0][];
        public float[] WeightsOverride { get; set; } = new float[0];
    }

    public class WaveSetting
    {
        public uint WaveSettingID { get; set; } = 0u;
        public uint WavePopulationID { get; set; } = 0u;
        public float Delay { get; set; } = 0.0f;
        public bool StopWaveOnDeath { get; set; } = false;

        public SpawnNodeSetting SpawnSetting = new()
        {
            SpawnType = WaveSpawnType.ClosestAlive,
            NodeType = SpawnNodeType.Scout
        };
    }

    public class SpawnNodeSetting
    {
        public WaveSpawnType SpawnType { get; set; } = WaveSpawnType.ClosestAlive;
        public SpawnNodeType NodeType { get; set; } = SpawnNodeType.Scout;
        public LG_LayerType Layer { get; set; } = LG_LayerType.MainLayer;
        public eLocalZoneIndex LocalIndex { get; set; } = eLocalZoneIndex.Zone_0;
        public int AreaIndex { get; set; } = 0;
    }

    public enum WaveSpawnType
    {
        ClosestAlive,
        ClosestNoBetweenPlayers,
        InNode,
        InNodeZone
    }

    public enum SpawnNodeType
    {
        Scout,
        FromArea,
        FromElevatorArea
    }
}