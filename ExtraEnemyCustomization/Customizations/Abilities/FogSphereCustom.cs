using System.Collections.Generic;
using EECustom.Extensions;
using Enemies;
using UnityEngine;

namespace EECustom.Customizations.Abilities
{
    public class FogSphereCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyPrefabBuiltEvent
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
        public bool EffectEnabled { get; set; } = false;
        public eEffectVolumeContents EffectContents { get; set; } = eEffectVolumeContents.Infection;
        public eEffectVolumeModification EffectModification { get; set; } = eEffectVolumeModification.Inflict;
        public float EffectScale { get; set; } = 1f;
        
        private readonly List<(EAB_FogSphere, GameObject)> _ChangedList = new();
        private readonly List<EV_Sphere> _EVSpheres = new();
        
        public override string GetProcessName()
        {
            return "FogSphere";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            if (EffectEnabled)
            {
                var EVSphere = new EV_Sphere()
                {
                    position = agent.Position,
                    minRadius = RangeMin,
                    maxRadius = RangeMax,
                    modificationScale = EffectScale,
                    invert = EffectModification == eEffectVolumeModification.Shield,
                    contents = EffectContents,
                    modification = EffectModification
                };
                if (!_EVSpheres.Contains(EVSphere))
                {
                    EffectVolumeManager.RegisterVolume(EVSphere);
                    _EVSpheres.Add(EVSphere);
                }
                else
                {
                    EffectVolumeManager.UnregisterVolume(EVSphere);
                }
                
            }
        }

        public void OnPrefabBuilt(EnemyAgent agent)
        {
            var eabFog = agent.GetComponentInChildren<EAB_FogSphere>(true);
            var fogPrefab = eabFog.m_fogSpherePrefab;
            _ChangedList.Add((eabFog, fogPrefab));

            var newFogPrefab = fogPrefab.Instantiate(agent.gameObject.transform, "newFogPrefab");
            var fogHandler = eabFog.m_fogSpherePrefab.GetComponent<FogSphereHandler>();
            
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
            }
        }
    }
}