using Agents;
using EEC.EnemyCustomizations.Models.Handlers;
using Enemies;
using GameData;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EEC.EnemyCustomizations.Models
{
    public sealed class ScannerCustom : EnemyCustomBase, IEnemyPrefabBuiltEvent, IEnemyAgentModeEvent
    {
        [JsonPropertyName("DefaultColor")]
        public Color Internal_DefaultColor { get; set; } = new(0.7f, 0.7f, 0.7f);

        [JsonPropertyName("DefaultSize")]
        public float Internal_DefaultSize { get; set; } = 1.0f;

        [JsonPropertyName("WakeupColor")]
        public Color Internal_WakeupColor { get; set; } = new(1f, 0.1f, 0.1f);

        [JsonPropertyName("WakeupSize")]
        public float Internal_WakeupSize { get; set; } = 1.0f;

        public float LerpingDuration { get; set; } = 0.5f;
        public float UpdateInterval { get; set; } = 0.15f;
        public bool OptimizeAfterAwake { get; set; } = true;

        public bool UsingDetectionColor { get; set; } = false;

        [JsonPropertyName("DetectionColor")]
        public Color Internal_DetectionColor { get; set; } = new(0.9882f, 0.4078f, 0.0f);

        [JsonPropertyName("DetectionSize")]
        public float Internal_DetectionSize { get; set; } = 1.0f;

        [JsonPropertyName("HeartbeatColor")]
        public Color Internal_HeartbeatColor { get; set; } = new(1.0f, 0.8431f, 0.8431f);

        [JsonPropertyName("HeartbeatSize")]
        public float Internal_HeartbeatSize { get; set; } = 1.0f;

        public bool UsingScoutColor { get; set; } = false;

        [JsonPropertyName("ScoutPatrolColor")]
        public Color Internal_ScoutPatrolColor { get; set; } = new(1f, 0.1f, 0.1f);

        [JsonPropertyName("ScoutPatrolSize")]
        public float Internal_ScoutPatrolSize { get; set; } = 0.5f;

        [JsonPropertyName("ScoutFeelerColor")]
        public Color Internal_ScoutFeelerColor { get; set; } = new(1f, 0.1f, 0.1f);

        [JsonPropertyName("ScoutFeelerSize")]
        public float Internal_ScoutFeelerSize { get; set; } = 1.0f;

        public ScannerColorData ColorData;

        public override string GetProcessName()
        {
            return "Scanner";
        }

        public override void OnConfigLoaded()
        {
            ColorData = new ScannerColorData()
            {
                DefaultColor = CreateNewColor(Internal_DefaultColor, Internal_DefaultSize),
                WakeupColor = CreateNewColor(Internal_WakeupColor, Internal_WakeupSize),
                DetectionColor = CreateNewColor(Internal_DetectionColor, Internal_DetectionSize),
                HeartbeatColor = CreateNewColor(Internal_HeartbeatColor, Internal_HeartbeatSize),
                PatrolColor = CreateNewColor(Internal_ScoutPatrolColor, Internal_ScoutPatrolSize),
                FeelerOutColor = CreateNewColor(Internal_ScoutFeelerColor, Internal_ScoutFeelerSize),
                InterpDuration = LerpingDuration,
                UpdateInterval = UpdateInterval,
                UsingScoutColor = UsingScoutColor,
                UsingDetectionColor = UsingDetectionColor,
                OptimizeOnAwake = OptimizeAfterAwake
            };

            if (Logger.VerboseLogAllowed)
            {
                LogVerbose("Color Initialized!");
                LogVerbose(" - Default; " + ColorData.DefaultColor.ToString());
                LogVerbose(" - Wakeup; " + ColorData.WakeupColor.ToString());
                LogVerbose(" - Detection; " + ColorData.DetectionColor.ToString());
                LogVerbose(" - Heartbeat; " + ColorData.HeartbeatColor.ToString());
                LogVerbose(" - Patrol; " + ColorData.PatrolColor.ToString());
                LogVerbose(" - Feeler; " + ColorData.FeelerOutColor.ToString());
            }

            static Color CreateNewColor(Color c, float alpha)
            {
                return new Color(c.r, c.g, c.b, alpha);
            }
        }

        public void OnPrefabBuilt(EnemyAgent agent, EnemyDataBlock enemyData)
        {
            var handler = agent.gameObject.AddOrGetComponent<ScannerHandler>();
            handler.OwnerAgent = agent;
            handler.ColorData = ColorData;
        }

        public void OnAgentModeChanged(EnemyAgent agent, AgentMode newMode)
        {
            if (agent.gameObject.TryGetComp<ScannerHandler>(out var scannerManager))
            {
                var forceUpdate = scannerManager.CurrentMode == AgentMode.Off;
                scannerManager.UpdateAgentMode(newMode, forceUpdate);
            }
        }
    }

    public struct ScannerColorData
    {
        public Color DefaultColor;
        public Color WakeupColor;
        public Color DetectionColor;
        public Color HeartbeatColor;
        public Color PatrolColor;
        public Color FeelerOutColor;

        public bool UsingDetectionColor;
        public bool UsingScoutColor;

        public float InterpDuration;
        public float UpdateInterval;

        public bool OptimizeOnAwake;
    }
}