using EECustom.EnemyCustomizations.Shared;
using EECustom.Utils.JsonElements;
using Enemies;
using UnityEngine;

namespace EECustom.EnemyCustomizations.Abilities
{
    public sealed class FogSphereCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyPrefabBuiltEvent
    {
        public CurveWrapper ColorCurve { get; set; } = CurveWrapper.Empty;
        public Color ColorMin { get; set; } = Color.white;
        public Color ColorMax { get; set; } = Color.clear;
        public CurveWrapper IntensityCurve { get; set; } = CurveWrapper.Empty;
        public float IntensityMin { get; set; } = 1.0f;
        public float IntensityMax { get; set; } = 5.0f;
        public CurveWrapper RangeCurve { get; set; } = CurveWrapper.Empty;
        public float RangeMin { get; set; } = 1.0f;
        public float RangeMax { get; set; } = 3.0f;
        public CurveWrapper DensityCurve { get; set; } = CurveWrapper.Empty;
        public float DensityMin { get; set; } = 1.0f;
        public float DensityMax { get; set; } = 5.0f;
        public CurveWrapper DensityAmountCurve { get; set; } = CurveWrapper.Empty;
        public float DensityAmountMin { get; set; } = 0.0f;
        public float DensityAmountMax { get; set; } = 5.0f;
        public float Duration { get; set; } = 30.0f;
        public EffectVolumeSetting EffectVolume { get; set; } = new();

        public override string GetProcessName()
        {
            return "FogSphere";
        }

        public void OnPrefabBuilt(EnemyAgent agent)
        {
            var eabFog = agent.GetComponentInChildren<EAB_FogSphere>(true);
            if (eabFog == null)
                return;

            var fogPrefab = eabFog.m_fogSpherePrefab;

            var newFogPrefab = fogPrefab.Instantiate(agent.gameObject.transform, "newFogPrefab");

            if (newFogPrefab.TryGetComp<FogSphereHandler>(out var fogHandler))
            {
                fogHandler.m_colorMin = ColorMin;
                fogHandler.m_colorMax = ColorMax;
                fogHandler.m_intensityMin = IntensityMin;
                fogHandler.m_intensityMax = IntensityMax;
                fogHandler.m_rangeMin = RangeMin;
                fogHandler.m_rangeMax = RangeMax;
                fogHandler.m_densityMin = DensityMin;
                fogHandler.m_densityMax = DensityMax;
                fogHandler.m_densityAmountMin = DensityAmountMin;
                fogHandler.m_densityAmountMax = DensityAmountMax;
                fogHandler.m_totalLength = Duration;

                if (ColorCurve.TryBuildCurve(out var curve))
                    fogHandler.m_colorCurve = curve;

                if (IntensityCurve.TryBuildCurve(out curve))
                    fogHandler.m_intensityCurve = curve;

                if (RangeCurve.TryBuildCurve(out curve))
                    fogHandler.m_rangeCurve = curve;

                if (DensityCurve.TryBuildCurve(out curve))
                    fogHandler.m_densityCurve = curve;

                if (DensityAmountCurve.TryBuildCurve(out curve))
                    fogHandler.m_densityAmountCurve = curve;

                eabFog.m_fogSpherePrefab = newFogPrefab;
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            if (EffectVolume.Enabled)
            {
                var effectSetting = agent.RegisterOrGetProperty<SphereEffectProperty>();
                effectSetting.HandlerCount = 0;
                effectSetting.Setting = EffectVolume;
            }
        }
    }

    internal sealed class SphereEffectProperty
    {
        public int HandlerCount = 0;
        public EffectVolumeSetting Setting;
    }
}