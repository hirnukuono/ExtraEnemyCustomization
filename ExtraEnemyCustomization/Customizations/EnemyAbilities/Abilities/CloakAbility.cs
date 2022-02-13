using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EECustom.Extensions;
using UnityEngine.Rendering;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public sealed class CloakAbility : AbilityBase<CloakBehaviour>
    {
        public float CloakOpacity { get; set; } = 0.0f;
        public float CloakDuration { get; set; } = 1.0f;
        public float DecloakDuration { get; set; } = 1.0f;
        public float DecloakAfterDelay { get; set; } = -1.0f;
        public bool HideShadow { get; set; } = false;
        public bool RequireTagForDetectionWhileCloaking { get; set; } = true;
        public bool AllowEABAbilityWhileCloaking { get; set; } = true;

        public override void OnAbilityLoaded()
        {
            CloakDuration = Mathf.Max(CloakDuration, 0.0f);
            DecloakDuration = Mathf.Max(DecloakDuration, 0.0f);
        }
    }

    public class CloakBehaviour : AbilityBehaviour<CloakAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => false;
        public override bool AllowEABAbilityWhileExecuting => Ability.AllowEABAbilityWhileCloaking;
        public override bool IsHostOnlyBehaviour => false;

        private readonly List<Handler> _handlers = new();
        private State _cloakState = State.None;
        private float _startTime = 0.0f;
        private float _endTime = 0.0f;
        private float _decloakTimer = float.MaxValue;
        private bool _reqTagOriginal = false;

        protected override void OnSetup()
        {
            _reqTagOriginal = Agent.RequireTagForDetection;

            foreach (var matRef in Agent.MaterialHandler.m_materialRefs)
            {
                if (!matRef.HasFeature(MaterialSupport.Destruction))
                    continue;

                foreach (var renderer in matRef.m_renderers)
                {
                    if (renderer.name.Equals("g_leg_l")) //MINOR: Shooter's left leg is always visible for some fucking reason
                    {
                        continue;
                    }

                    var trsRenderer = renderer.gameObject.Instantiate(renderer.gameObject.transform, "TransparentRenderer").GetComponent<Renderer>();
                    trsRenderer.material.shader = Shader.Find("GTFO/Glass");
                    trsRenderer.material.SetTexture("_MainTex", matRef.m_material.GetTexture("_MainTex"));
                    trsRenderer.material.SetTexture("_MetallicGlossMap", Texture2D.blackTexture);
                    trsRenderer.forceRenderingOff = true;
                    Agent.MovingCuller.Culler.AddRenderer(trsRenderer);
                    _handlers.Add(new Handler()
                    {
                        hideShadowMode = Ability.HideShadow,
                        minOpacity = Ability.CloakOpacity,
                        originalRenderer = renderer,
                        transitionRenderer = trsRenderer
                    });
                }
            }
        }

        protected override void OnEnter()
        {
            _cloakState = State.Cloaking;
            _startTime = Clock.Time;
            _endTime = Clock.Time + Ability.CloakDuration;

            if (Ability.DecloakAfterDelay >= 0.0f)
            {
                _decloakTimer = Clock.Time + Ability.DecloakAfterDelay;
            }

            if (Ability.RequireTagForDetectionWhileCloaking)
            {
                Agent.RequireTagForDetection = true;
            }
        }

        protected override void OnExit()
        {
            _cloakState = State.Decloaking;
            _startTime = Clock.Time;
            _endTime = Clock.Time + Ability.DecloakDuration;
            Agent.RequireTagForDetection = _reqTagOriginal;
        }

        protected override void OnUpdate()
        {
            float progress;
            switch (_cloakState)
            {
                case State.Cloaking:
                    progress = GetProgress();
                    _handlers.ForEach(h => h.SetProgress(1.0f - progress));

                    if (progress >= 1.0f)
                    {
                        _cloakState = State.None;
                        Agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;
                        Agent.SetAnimatorCullingEnabled(false);
                    }
                    break;

                case State.Decloaking:
                    progress = GetProgress();
                    _handlers.ForEach(h => h.SetProgress(progress));

                    if (progress >= 1.0f)
                    {
                        _cloakState = State.None;
                        Agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = false;
                        Agent.SetAnimatorCullingEnabled(true);
                    }
                    break;
            }

            if (_decloakTimer <= Clock.Time && Executing)
            {
                _decloakTimer = float.MaxValue;
                DoExit();
            }
        }

        protected override void OnAbilityLazyUpdate()
        {
            Agent.MovingCuller.Culler.NeedsShadowRefresh = true;
        }

        private float GetProgress()
        {
            return Mathf.InverseLerp(_startTime, _endTime, Clock.Time);
        }

        public enum State
        {
            None,
            Cloaking,
            Decloaking
        }

        public struct Handler
        {
            public bool hideShadowMode;
            public float minOpacity;
            public Renderer originalRenderer;
            public Renderer transitionRenderer;

            public void SetProgress(float p)
            {
                if (p <= 0.0f)
                {
                    DisableOriginalRenderer();
                    if (minOpacity <= 0.0f)
                    {
                        transitionRenderer.forceRenderingOff = true;
                    }
                    else
                    {
                        transitionRenderer.forceRenderingOff = false;
                        transitionRenderer.material.color = Color.white.AlphaMultiplied(minOpacity);
                    }
                }
                else if (p >= 1.0f)
                {
                    EnableOriginalRenderer();
                    transitionRenderer.forceRenderingOff = true;
                    transitionRenderer.material.color = Color.white;
                }
                else
                {
                    DisableOriginalRenderer();
                    transitionRenderer.forceRenderingOff = false;
                    transitionRenderer.material.color = Color.white.AlphaMultiplied(Mathf.Lerp(minOpacity, 1.0f, p));
                }
            }

            private void EnableOriginalRenderer()
            {
                originalRenderer.shadowCastingMode = ShadowCastingMode.On;
                originalRenderer.forceRenderingOff = false;
            }

            private void DisableOriginalRenderer()
            {
                if (hideShadowMode)
                {
                    originalRenderer.shadowCastingMode = ShadowCastingMode.Off;
                    originalRenderer.forceRenderingOff = true;
                }
                else
                {
                    originalRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    originalRenderer.forceRenderingOff = false;
                }
            }
        }
    }
}
