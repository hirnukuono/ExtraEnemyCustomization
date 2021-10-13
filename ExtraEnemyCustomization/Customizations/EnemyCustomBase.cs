using BepInEx.Logging;
using EECustom.Managers;
using Enemies;
using GameData;
using System;
using System.Linq;

namespace EECustom.Customizations
{
    public abstract class EnemyCustomBase
    {
        public string DebugName { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public TargetSetting Target { get; set; } = new TargetSetting();

        public virtual void OnConfigLoaded()
        {
        }

        public abstract string GetProcessName();

        public void LogVerbose(string str)
        {
            LogFormatDebug(str, true);
        }

        public void LogDev(string str)
        {
            LogFormatDebug(str, false);
        }

        public void LogError(string str)
        {
            LogFormat(LogLevel.Error, str);
        }

        public void LogWarning(string str)
        {
            LogFormat(LogLevel.Warning, str);
        }

        private void LogFormat(LogLevel level, string str)
        {
            if (!string.IsNullOrEmpty(DebugName))
                Logger.LogInstance.Log(level, $"[{GetProcessName()}-{DebugName}] {str}");
            else
                Logger.LogInstance.Log(level, $"[{GetProcessName()}] {str}");
        }

        private void LogFormatDebug(string str, bool verbose)
        {
            string prefix;
            if (!string.IsNullOrEmpty(DebugName))
                prefix = $"[{GetProcessName()}-{DebugName}]";
            else
                prefix = $"[{GetProcessName()}]";

            if (verbose)
                Logger.Verbose($"{prefix} {str}");
            else
                Logger.Debug($"{prefix} {str}");
        }
    }

    public class TargetSetting
    {
        public TargetMode Mode { get; set; } = TargetMode.PersistentID;
        public uint[] PersistentIDs { get; set; } = new uint[1] { 0 };
        public string NameParam { get; set; } = string.Empty;
        public bool NameIgnoreCase { get; set; } = false;
        public string[] Categories { get; set; } = new string[0];

        public bool IsMatch(EnemyAgent agent)
        {
            var enemyData = GameDataBlockBase<EnemyDataBlock>.GetBlock(agent.EnemyDataID);
            var comparisonMode = NameIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            return Mode switch
            {
                TargetMode.PersistentID => PersistentIDs.Contains(agent.EnemyDataID),
                TargetMode.NameEquals => enemyData?.name?.Equals(NameParam, comparisonMode) ?? false,
                TargetMode.NameContains => enemyData?.name?.Contains(NameParam, comparisonMode) ?? false,
                TargetMode.Everything => true,
                TargetMode.CategoryAny => ConfigManager.Current.Categories.Any(Categories, agent.EnemyDataID),
                TargetMode.CategoryAll => ConfigManager.Current.Categories.All(Categories, agent.EnemyDataID),
                _ => false,
            };
        }

        public string ToDebugString()
        {
            return $"TargetDebug, Mode: {Mode}, persistentIDs: [{string.Join(", ", PersistentIDs)}], nameParam: {NameParam}, categories: [{string.Join(", ", Categories)}]";
        }
    }

    public enum TargetMode
    {
        PersistentID,
        NameEquals,
        NameContains,
        Everything,
        CategoryAny,
        CategoryAll
    }
}