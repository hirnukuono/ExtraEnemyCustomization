using AK;
using AssetShards;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public class FogSphereAbility : AbilityBase<FogSphereBehaviour>
    {
        public const string EAB_FOG_PREFAB = "Assets/AssetPrefabs/Characters/Enemies/Abilities/EAB_FogSphere.prefab";

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

        private GameObject _fogSpherePrefab;

        public override void OnAbilityLoaded()
        {
            var eabFogPrefab = AssetShardManager.GetLoadedAsset(EAB_FOG_PREFAB, false);
            var eabFog = eabFogPrefab.Cast<GameObject>().GetComponent<EAB_FogSphere>();
            _fogSpherePrefab = GameObject.Instantiate(eabFog.m_fogSpherePrefab).Cast<GameObject>();

            var fogHandler = _fogSpherePrefab.GetComponent<FogSphereHandler>();
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

            GameObject.DontDestroyOnLoad(_fogSpherePrefab);
        }

        public override void OnAbilityUnloaded()
        {
            GameObject.Destroy(_fogSpherePrefab);
        }

        public override void OnBehaviourAssigned(EnemyAgent agent, FogSphereBehaviour behaviour)
        {
            behaviour.FogPrefab = _fogSpherePrefab;
        }
    }

    public class SomeBehaviour : AbilityBehaviour
    {
        public override bool AllowEABAbilityWhileExecuting => throw new NotImplementedException();

        public override bool IsHostOnlyBehaviour => throw new NotImplementedException();

        protected override void OnSetup()
        {
            base.OnSetup();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
        }

        protected override void OnAbilityUpdate()
        {
            base.OnAbilityUpdate();
        }

        protected override void OnAbilityLazyUpdate()
        {
            base.OnAbilityLazyUpdate();
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        protected override void OnDead()
        {
            base.OnDead();
        }

        protected override void OnLimbDestroyed()
        {
            base.OnLimbDestroyed();
        }

        protected override void OnHitreact()
        {
            base.OnHitreact();
        }

        protected override void OnTakeDamage(float damage)
        {
            base.OnTakeDamage(damage);
        }
    }

    public class FogSphereBehaviour : AbilityBehaviour
    {
        public GameObject FogPrefab;
        private readonly List<FogSphereHandler> _fogSphereHandlers = new();

        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => false;

        protected override void OnEnter()
        {
            Agent.Sound.Post(EVENTS.BIRTHER_FOG_BALL_APPEAR);
            var fogObject = UnityEngine.Object.Instantiate(FogPrefab, Agent.Position, Quaternion.identity);
            var handler = fogObject.GetComponent<FogSphereHandler>();
            if (handler.Play())
            {
                _fogSphereHandlers.Add(handler);
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
