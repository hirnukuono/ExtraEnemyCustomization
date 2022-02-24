using BepInEx.Logging;
using EECustom.Managers;
using Enemies;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace EECustom.Customizations
{
    public abstract class EnemyCustomBase
    {
        public string DebugName { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public TargetSetting Target { get; set; } = new TargetSetting();

        private readonly Dictionary<uint, bool> _isTargetLookup = new();

        public virtual void OnAssetLoaded()
        {
        }

        public virtual void OnConfigLoaded()
        {
        }

        public virtual void OnConfigUnloaded()
        {
        }

        [JsonIgnore]
        public IEnumerable<uint> TargetEnemyIDs => _isTargetLookup.ToArray().Where(x => x.Value == true).Select(x => x.Key); 
        public EnemyCustomBase Base => this;

        public abstract string GetProcessName();

        internal void RegisterTargetEnemyLookup(EnemyDataBlock enemyData)
        {
            if (!Enabled)
                return;

            var id = enemyData.persistentID;
            if (!_isTargetLookup.ContainsKey(id))
            {
                _isTargetLookup.Add(id, Target.IsMatch(enemyData));
            }
        }

        internal void ClearTargetLookup()
        {
            _isTargetLookup.Clear();
        }

        public bool IsTarget(EnemyAgent enemyAgent) => IsTarget(enemyAgent.EnemyDataID);

        public bool IsTarget(uint id)
        {
            if (_isTargetLookup.TryGetValue(id, out var isTarget))
                return isTarget;

            return false;
        }

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
        public string[] Categories { get; set; } = Array.Empty<string>();

        public bool IsMatch(uint enemyID)
        {
            var enemyBlock = GameDataBlockBase<EnemyDataBlock>.GetBlock(enemyID);
            return IsMatch(enemyBlock);
        }

        public bool IsMatch(EnemyDataBlock enemyBlock)
        {
            if (enemyBlock == null)
                return false;

            return Mode switch
            {
                TargetMode.PersistentID => PersistentIDs.Contains(enemyBlock.persistentID),
                TargetMode.NameEquals => enemyBlock.name?.InvariantEquals(NameParam, ignoreCase: NameIgnoreCase) ?? false,
                TargetMode.NameContains => enemyBlock.name?.InvariantContains(NameParam, ignoreCase: NameIgnoreCase) ?? false,
                TargetMode.Everything => true,
                TargetMode.CategoryAny => ConfigManager.Categories.Any(Categories, enemyBlock.persistentID),
                TargetMode.CategoryAll => ConfigManager.Categories.All(Categories, enemyBlock.persistentID),
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