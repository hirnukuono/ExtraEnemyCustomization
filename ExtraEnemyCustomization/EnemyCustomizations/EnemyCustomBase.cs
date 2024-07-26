using BepInEx.Logging;
using Enemies;
using GameData;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace EEC.EnemyCustomizations
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

        public virtual void OnTargetIDLookupBuilt()
        {
        }

        public virtual void OnConfigUnloaded()
        {
        }

        [JsonIgnore]
        public IEnumerable<uint> TargetEnemyIDs => _isTargetLookup.Where(x => x.Value == true).Select(x => x.Key);

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
}