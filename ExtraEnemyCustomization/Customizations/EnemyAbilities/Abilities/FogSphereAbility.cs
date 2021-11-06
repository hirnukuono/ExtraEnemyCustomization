using AssetShards;
using EECustom.Customizations.Shared;
using EECustom.Customizations.Shared.Handlers;
using System.Collections.Generic;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public class FogSphereAbility : AbilityBase<FogSphereBehaviour>
    {
        public const string EAB_FOG_PREFAB = "Assets/AssetPrefabs/Characters/Enemies/Abilities/EAB_FogSphere.prefab";

        public uint SoundEventID { get; set; } = 0u;
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

        public GameObject FogSpherePrefab;

        public override void OnAbilityLoaded()
        {
            var eabFogPrefab = AssetShardManager.GetLoadedAsset(EAB_FOG_PREFAB, false);
            var eabFog = eabFogPrefab.Cast<GameObject>().GetComponent<EAB_FogSphere>();
            FogSpherePrefab = GameObject.Instantiate(eabFog.m_fogSpherePrefab).Cast<GameObject>();

            var fogHandler = FogSpherePrefab.GetComponent<FogSphereHandler>();
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

            GameObject.DontDestroyOnLoad(FogSpherePrefab);
        }

        public override void OnAbilityUnloaded()
        {
            GameObject.Destroy(FogSpherePrefab);
        }
    }

    public class FogSphereBehaviour : AbilityBehaviour<FogSphereAbility>
    {
        private readonly List<FogSphereHandler> _fogSphereHandlers = new();

        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => false;

        protected override void OnEnter()
        {
            if (Ability.SoundEventID != 0u)
            {
                Agent.Sound.Post(Ability.SoundEventID);
            }

            var fogObject = UnityEngine.Object.Instantiate(Ability.FogSpherePrefab, Agent.Position, Quaternion.identity);
            var handler = fogObject.GetComponent<FogSphereHandler>();
            if (handler.Play())
            {
                _fogSphereHandlers.Add(handler);
                if (Ability.EffectVolume.Enabled)
                {
                    var effectHandler = handler.gameObject.AddComponent<EffectFogSphereHandler>();
                    effectHandler.Handler = handler;
                    effectHandler.EVSphere = Ability.EffectVolume.CreateSphere(handler.transform.position, 0.0f, 0.0f);
                    EffectVolumeManager.RegisterVolume(effectHandler.EVSphere);
                }
            }

            DoExit();
        }

        protected override void OnUpdate()
        {
            int num = _fogSphereHandlers.Count - 1;
            if (num > -1)
            {
                for (int i = num; i > -1; i--)
                {
                    var handler = _fogSphereHandlers[i];
                    if (handler == null)
                    {
                        _fogSphereHandlers.RemoveAt(i);
                    }
                    else if (!handler.enabled)
                    {
                        _fogSphereHandlers.RemoveAt(i);
                        UnityEngine.Object.DestroyImmediate(handler.gameObject);
                    }
                }
                return;
            }
        }
    }
}
