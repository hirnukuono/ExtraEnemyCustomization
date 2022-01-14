using EECustom.Customizations.Shared;
using EECustom.Extensions;
using EECustom.Utils;
using Enemies;
using UnityEngine;

namespace EECustom.Customizations.Abilities
{
    public sealed class FogSphereCustom : RevertableEnemyCustomBase, IEnemySpawnedEvent, IEnemyPrefabBuiltEvent
    {
        public Color ColorMin { get; set; } = Color.white;
        public Color ColorMax { get; set; } = Color.clear;
        public float IntensityMin { get; set; } = 1.0f;
        public float IntensityMax { get; set; } = 5.0f;
        public float RangeMin { get; set; } = 1.0f;
        public float RangeMax { get; set; } = 3.0f;
        public float DensityMin { get; set; } = 1.0f;
        public float DensityMax { get; set; } = 5.0f;
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
            var fogHandler = newFogPrefab.GetComponent<FogSphereHandler>();

            if (fogHandler != null)
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

                eabFog.m_fogSpherePrefab = newFogPrefab;

                PushRevertJob(() =>
                {
                    eabFog.m_fogSpherePrefab = fogPrefab;
                });
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            if (EffectVolume.Enabled)
            {
                var effectSetting = EnemyProperty<SphereEffectSetting>.RegisterOrGet(agent);
                effectSetting.HandlerCount = 0;
                effectSetting.Setting = EffectVolume;
            }
        }
    }

    public class SphereEffectSetting
    {
        public int HandlerCount = 0;
        public EffectVolumeSetting Setting;
    }
}