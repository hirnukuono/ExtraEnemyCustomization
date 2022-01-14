using EECustom.Customizations.Models.Handlers;
using Enemies;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public sealed class ScannerCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public static readonly Color DefaultDetectionColor = new(1f, 0.1f, 0.1f, 1f);

        [JsonPropertyName("DefaultColor")]
        public Color Internal_DefaultColor { get; set; } = new(0.7f, 0.7f, 0.7f);

        [JsonPropertyName("DefaultSize")]
        public float Internal_DefaultSize { get; set; } = 1.0f;

        [JsonPropertyName("WakeupColor")]
        public Color Internal_WakeupColor { get; set; } = new(1f, 0.1f, 0.1f);

        [JsonPropertyName("WakeupSize")]
        public float Internal_WakeupSize { get; set; } = 1.0f;

        public float LerpingDuration { get; set; } = 0.5f;

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

        public Color DefaultColor;
        public Color WakeupColor;
        public Color DetectionColor;
        public Color HeartbeatColor;
        public Color PatrolColor;
        public Color FeelerOutColor;

        public override string GetProcessName()
        {
            return "Scanner";
        }

        public override void OnConfigLoaded()
        {
            DefaultColor = CreateNewColor(Internal_DefaultColor, Internal_DefaultSize);
            WakeupColor = CreateNewColor(Internal_WakeupColor, Internal_WakeupSize);
            DetectionColor = CreateNewColor(Internal_DetectionColor, Internal_DetectionSize);
            HeartbeatColor = CreateNewColor(Internal_HeartbeatColor, Internal_HeartbeatSize);
            PatrolColor = CreateNewColor(Internal_ScoutPatrolColor, Internal_ScoutPatrolSize);
            FeelerOutColor = CreateNewColor(Internal_ScoutFeelerColor, Internal_ScoutFeelerSize);

            LogVerbose("Color Initialized!");
            LogVerbose(" - Default; " + DefaultColor.ToString());
            LogVerbose(" - Wakeup; " + WakeupColor.ToString());
            LogVerbose(" - Detection; " + DetectionColor.ToString());
            LogVerbose(" - Heartbeat; " + HeartbeatColor.ToString());
            LogVerbose(" - Patrol; " + PatrolColor.ToString());
            LogVerbose(" - Feeler; " + FeelerOutColor.ToString());

            static Color CreateNewColor(Color c, float alpha)
            {
                return new Color(c.r, c.g, c.b, alpha);
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var scannerManager = agent.gameObject.GetComponent<ScannerHandler>();
            if (scannerManager == null)
            {
                scannerManager = agent.gameObject.AddComponent<ScannerHandler>();
            }
            scannerManager.OwnerAgent = agent;
            scannerManager.DefaultColor = DefaultColor;
            scannerManager.WakeupColor = WakeupColor;
            scannerManager.DetectionColor = DetectionColor;
            scannerManager.HeartbeatColor = HeartbeatColor;
            scannerManager.PatrolColor = PatrolColor;
            scannerManager.FeelerColor = FeelerOutColor;
            scannerManager.UsingDetectionColor = UsingDetectionColor;
            scannerManager.UsingScoutColor = UsingScoutColor;
            scannerManager.InterpDuration = LerpingDuration;
        }
    }
}