using EECustom.Events;
using EECustom.Utils.JsonElements;
using Enemies;
using UnityEngine;

namespace EECustom.Customizations.Detections
{
    public sealed class FeelerCustom : EnemyCustomBase
    {
        public ValueBase TendrilCount { get; set; } = ValueBase.Unchanged;
        public float TendrilAngleOffset { get; set; } = 0.0f;
        public ValueBase TendrilStepAngle { get; set; } = ValueBase.Unchanged;
        public ValueBase TendrilMinYSpread { get; set; } = ValueBase.Unchanged;
        public ValueBase TendrilMaxYSpread { get; set; } = ValueBase.Unchanged;
        public ValueBase TendrilOutTimer { get; set; } = ValueBase.Unchanged;

        public ValueBase Distance { get; set; } = ValueBase.Unchanged;
        public ValueBase StepDistance { get; set; } = ValueBase.Unchanged;
        public ValueBase RetractTime { get; set; } = ValueBase.Unchanged;
        public ValueBase RetractTimeDetected { get; set; } = ValueBase.Unchanged;
        public Color NormalColor { get; set; } = Color.black;
        public Color DetectColor { get; set; } = Color.red;

        public override string GetProcessName()
        {
            return "Feeler";
        }

        public override void OnConfigLoaded()
        {
            ScoutAntennaSpawnEvent.DetectionSpawn += OnDetectionSpawn;
            ScoutAntennaSpawnEvent.AntennaSpawn += OnAntennaSpawn;
        }

        public override void OnConfigUnloaded()
        {
            ScoutAntennaSpawnEvent.DetectionSpawn -= OnDetectionSpawn;
            ScoutAntennaSpawnEvent.AntennaSpawn -= OnAntennaSpawn;
        }

        private void OnDetectionSpawn(EnemyAgent agent, ScoutAntennaDetection detection)
        {
            if (!IsTarget(agent))
                return;

            detection.m_tendrilCount = TendrilCount.GetAbsValue(detection.m_tendrilCount);
            detection.m_dirAngOffset = TendrilAngleOffset;
            detection.m_dirAngStep = TendrilStepAngle.GetAbsValue(detection.m_dirAngStep);
            detection.m_dirAngSpread_Min = TendrilMinYSpread.GetAbsValue(detection.m_dirAngSpread_Min);
            detection.m_dirAngSpread_Max = TendrilMaxYSpread.GetAbsValue(detection.m_dirAngSpread_Max);
            detection.m_timerWaitOut = TendrilOutTimer.GetAbsValue(detection.m_timerWaitOut);
        }

        private void OnAntennaSpawn(EnemyAgent agent, ScoutAntennaDetection _, ScoutAntenna ant)
        {
            if (!IsTarget(agent))
                return;

            ant.m_colorDefault = NormalColor;
            ant.m_colorDetection = DetectColor;
            ant.m_moveInTime = RetractTime.GetAbsValue(ant.m_moveInTime);
            ant.m_moveInTimeDetected = RetractTimeDetected.GetAbsValue(ant.m_moveInTimeDetected);
            ant.m_maxDistance = Distance.GetAbsValue(ant.m_maxDistance);
            ant.m_stepDistance = StepDistance.GetAbsValue(ant.m_stepDistance);
        }
    }
}