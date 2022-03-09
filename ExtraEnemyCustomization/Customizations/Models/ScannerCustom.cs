using Agents;
using EECustom.Customizations.Models.Handlers;
using Enemies;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public sealed class ScannerCustom : EnemyCustomBase, IEnemyPrefabBuiltEvent, IEnemySpawnedEvent, IEnemyAgentModeEvent
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
        public float UpdateInterval { get; set; } = 0.05f;
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

            if (Logger.VerboseLogAllowed)
            {
                LogVerbose("Color Initialized!");
                LogVerbose(" - Default; " + DefaultColor.ToString());
                LogVerbose(" - Wakeup; " + WakeupColor.ToString());
                LogVerbose(" - Detection; " + DetectionColor.ToString());
                LogVerbose(" - Heartbeat; " + HeartbeatColor.ToString());
                LogVerbose(" - Patrol; " + PatrolColor.ToString());
                LogVerbose(" - Feeler; " + FeelerOutColor.ToString());
            }

            static Color CreateNewColor(Color c, float alpha)
            {
                return new Color(c.r, c.g, c.b, alpha);
            }
        }

        public void OnPrefabBuilt(EnemyAgent agent)
        {
            var handler = agent.gameObject.GetComponent<ScannerHandler>();
            if (handler == null)
            {
                agent.gameObject.AddComponent<ScannerHandler>();
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var spawnData = agent.GetSpawnData();
            switch (spawnData.mode)
            {
                //Disallow PathMove ES's Color change
                case AgentMode.Agressive:
                case AgentMode.Scout:
                    if (agent.ScannerData.m_soundIndex == -1)
                    {
                        agent.ScannerData.m_soundIndex = 0;
                    }
                    if (OptimizeAfterAwake)
                    {
                        agent.ScannerColor = WakeupColor;
                        return;
                    }
                    break;
            }

            var scannerManager = agent.gameObject.GetComponent<ScannerHandler>();
            if (scannerManager != null)
            {
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
                scannerManager.UpdateInterval = UpdateInterval;
                scannerManager.OptimizeOnAwake = OptimizeAfterAwake;
                scannerManager.UpdateAgentMode(spawnData.mode);
                scannerManager.Setup();
            }
        }

        public void OnAgentModeChanged(EnemyAgent agent, AgentMode newMode)
        {
            var scannerManager = agent.gameObject.GetComponent<ScannerHandler>();
            if (scannerManager != null)
            {
                scannerManager.UpdateAgentMode(newMode);
            }
        }
    }
}