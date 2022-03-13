using EECustom.Utils;
using EECustom.Utils.JsonElements;
using Enemies;
using System;

namespace EECustom.Customizations.Strikers
{
    using EaseFunc = Func<float, float, float, float, float>;

    public sealed class StrikerTentacleCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public GPUCurvyType[] TentacleTypes { get; set; } = Array.Empty<GPUCurvyType>();
        public TentacleSettingData[] TentacleSettings { get; set; } = Array.Empty<TentacleSettingData>();

        public override string GetProcessName()
        {
            return "Tentacle";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var tentacleComps = agent.GetComponentsInChildren<MovingEnemyTentacleBase>(true);
            var isTypeEnabled = TentacleTypes.Length > 0;
            var isSettingEnabled = TentacleSettings.Length > 0;

            for (int i = 0; i < tentacleComps.Length; i++)
            {
                var tentacle = tentacleComps[i];

                if (isTypeEnabled)
                {
                    var tenType = TentacleTypes[i % TentacleTypes.Length];
                    tentacle.m_GPUCurvyType = tenType;
                    if (Logger.VerboseLogAllowed)
                        LogVerbose($" - Applied Tentacle Type!, index: {i} type: {tenType}");
                }

                if (isSettingEnabled)
                {
                    var setting = TentacleSettings[i % TentacleSettings.Length];
                    tentacle.m_easingIn = setting.GetInEaseFunction();
                    tentacle.m_easingOut = setting.GetOutEaseFunction();
                    tentacle.m_attackInDuration = setting.InDuration.GetAbsValue(tentacle.m_attackInDuration);
                    tentacle.m_attackOutDuration = setting.OutDuration.GetAbsValue(tentacle.m_attackOutDuration);
                    tentacle.m_attackHangDuration = setting.HangDuration.GetAbsValue(tentacle.m_attackHangDuration);
                    tentacle.m_splineWidthSafe = setting.SplineWidthSafe.GetAbsValue(tentacle.m_splineWidthSafe);
                    tentacle.m_splineWidthPos1 = setting.SplineWidthPos1.GetAbsValue(tentacle.m_splineWidthPos1);
                    tentacle.m_splineWidthPos2 = setting.SplineWidthPos2.GetAbsValue(tentacle.m_splineWidthPos2);
                    tentacle.m_splineHeightPos1 = setting.SplineHeightPos1.GetAbsValue(tentacle.m_splineHeightPos1);
                    tentacle.m_splineHeightPos2 = setting.SplineHeightPos2.GetAbsValue(tentacle.m_splineHeightPos2);
                    if (Logger.VerboseLogAllowed)
                        LogVerbose($" - Applied Tentacle Setting!, index: {i}");
                }
            }
        }

        public sealed class TentacleSettingData
        {
            public eEasingType InEaseType { get; set; } = eEasingType.EaseOutCirc;
            public eEasingType OutEaseType { get; set; } = eEasingType.EaseInExpo;

            public ValueBase InDuration { get; set; } = ValueBase.Unchanged;
            public ValueBase OutDuration { get; set; } = ValueBase.Unchanged;
            public ValueBase HangDuration { get; set; } = ValueBase.Unchanged;
            public ValueBase SplineWidthSafe { get; set; } = ValueBase.Unchanged;
            public ValueBase SplineWidthPos1 { get; set; } = ValueBase.Unchanged;
            public ValueBase SplineWidthPos2 { get; set; } = ValueBase.Unchanged;
            public ValueBase SplineHeightPos1 { get; set; } = ValueBase.Unchanged;
            public ValueBase SplineHeightPos2 { get; set; } = ValueBase.Unchanged;

            public EaseFunc GetInEaseFunction()
            {
                return EasingUtil.GetEaseFunction(InEaseType);
            }

            public EaseFunc GetOutEaseFunction()
            {
                return EasingUtil.GetEaseFunction(OutEaseType);
            }
        }
    }
}