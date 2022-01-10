using BepInEx.Logging;
using EECustom.Extensions;
using EECustom.Managers;
using Enemies;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EECustom.Customizations
{
    public abstract class EnemyCustomBase
    {
        public string DebugName { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public TargetSetting Target { get; set; } = new TargetSetting();

        private readonly Dictionary<ushort, bool> _isTargetLookup = new();

        public virtual void OnConfigLoaded()
        {

        }

        public virtual void OnConfigUnloaded()
        {

        }

        public EnemyCustomBase Base => this;

        public abstract string GetProcessName();

        internal void RegisterTargetLookup(EnemyAgent enemyAgent)
        {
            if (!Enabled)
                return;

            var id = enemyAgent.GlobalID;
            if (!_isTargetLookup.ContainsKey(id))
            {
                _isTargetLookup.Add(id, Target.IsMatch(enemyAgent.EnemyDataID));

                enemyAgent.AddOnDeadOnce(() =>
                {
                    _isTargetLookup.Remove(id);
                });
            }
        }

        internal void ClearTargetLookup()
        {
            _isTargetLookup.Clear();
        }

        public bool IsTarget(EnemyAgent enemyAgent) => IsTarget(enemyAgent.GlobalID);

        public bool IsTarget(ushort id)
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
        public string[] Categories { get; set; } = new string[0];

        public bool IsMatch(uint enemyID)
        {
            var enemyBlock = GameDataBlockBase<EnemyDataBlock>.GetBlock(enemyID);
            return IsMatch(enemyBlock);
        }

        public bool IsMatch(EnemyDataBlock enemyBlock)
        {
            if (enemyBlock == null)
                return false;

            var comparisonMode = NameIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            return Mode switch
            {
                TargetMode.PersistentID => PersistentIDs.Contains(enemyBlock.persistentID),
                TargetMode.NameEquals => enemyBlock.name?.Equals(NameParam, comparisonMode) ?? false,
                TargetMode.NameContains => enemyBlock.name?.Contains(NameParam, comparisonMode) ?? false,
                TargetMode.Everything => true,
                TargetMode.CategoryAny => ConfigManager.Current.Categories.Any(Categories, enemyBlock.persistentID),
                TargetMode.CategoryAll => ConfigManager.Current.Categories.All(Categories, enemyBlock.persistentID),
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