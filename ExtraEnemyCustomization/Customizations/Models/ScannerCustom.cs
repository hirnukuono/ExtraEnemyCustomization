using EECustom.Customizations.Models.Inject;
using EECustom.Customizations.Models.Managers;
using EECustom.Events;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public class ScannerCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public static readonly Color DefaultDetectionColor = new Color(1f, 0.1f, 0.1f, 1f);

        [JsonPropertyName("DefaultColor")]
        public Color Internal_DefaultColor { get; set; } = new Color(0.7f, 0.7f, 0.7f);
        [JsonPropertyName("DefaultSize")]
        public float Internal_DefaultSize { get; set; } = 1.0f;

        [JsonPropertyName("WakeupColor")]
        public Color Internal_WakeupColor { get; set; } = new Color(1f, 0.1f, 0.1f);
        [JsonPropertyName("WakeupSize")]
        public float Internal_WakeupSize { get; set; } = 1.0f;

        public bool UsingDetectionColor { get; set; } = false;

        [JsonPropertyName("DetectionColor")]
        public Color Internal_DetectionColor { get; set; } = new Color(0.9882f, 0.4078f, 0.0f);
        [JsonPropertyName("DetectionSize")]
        public float Internal_DetectionSize { get; set; } = 1.0f;
        [JsonPropertyName("HeartbeatColor")]
        public Color Internal_HeartbeatColor { get; set; } = new Color(1.0f, 0.8431f, 0.8431f);
        [JsonPropertyName("HeartbeatSize")]
        public float Internal_HeartbeatSize { get; set; } = 1.0f;

        public Color DefaultColor;
        public Color WakeupColor;
        public Color DetectionColor;
        public Color HeartbeatColor;

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

            LogVerbose("Color Initialized!");
            LogVerbose(" - Default; " + DefaultColor.ToString());
            LogVerbose(" - Wakeup; " + WakeupColor.ToString());
            LogVerbose(" - Detection; " + DetectionColor.ToString());
            LogVerbose(" - Heartbeat; " + HeartbeatColor.ToString());

            static Color CreateNewColor(Color c, float alpha)
            {
                return new Color(c.r, c.g, c.b, alpha);
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var scannerManager = agent.gameObject.AddComponent<ScannerManager>();
            scannerManager._Agent = agent;
            scannerManager._DefaultColor = DefaultColor;
            scannerManager._WakeupColor = WakeupColor;
            scannerManager._DetectionColor = DetectionColor;
            scannerManager._HeartbeatColor = HeartbeatColor;
            scannerManager._UsingDetectionColor = UsingDetectionColor;

            //agent.m_scannerColor;
            //EnemyScannerColorEvents.RegisterOnChanged(agent, OnColorChanged);
        }
    }
}
