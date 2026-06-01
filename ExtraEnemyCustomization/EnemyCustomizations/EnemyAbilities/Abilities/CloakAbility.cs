using UnityEngine;
using UnityEngine.Rendering;
using Timer = EEC.Utils.Unity.Timer;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public sealed class CloakAbility : AbilityBase<CloakBehaviour>
    {
        public float CloakOpacity { get; set; } = 0f;
        public float CloakDuration { get; set; } = 1f;
        public float DecloakDuration { get; set; } = 1f;
        public float DecloakAfterDelay { get; set; } = -1f;
        public bool HideShadow { get; set; } = false;
        public bool RequireTagForDetectionWhileCloaking { get; set; } = true;
        public bool AllowEABAbilityWhileCloaking { get; set; } = true;

        public override void OnAbilityLoaded()
        {
            CloakDuration = Mathf.Max(CloakDuration, 0f);
            DecloakDuration = Mathf.Max(DecloakDuration, 0f);
        }
    }

    public sealed class CloakBehaviour : AbilityBehaviour<CloakAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => false;
        public override bool AllowEABAbilityWhileExecuting => Ability.AllowEABAbilityWhileCloaking;
        public override bool IsHostOnlyBehaviour => false;

        private readonly List<Handler> _handlers = new();
        private State _cloakState = State.None;
        private Timer _timer;
        private Timer _decloakTimer;
        private bool _hasDecloakTimer = false;
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
                    if (renderer.name.InvariantEquals("g_leg_l")) //MINOR: Shooter's left leg is always visible for some fucking reason
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
            _timer.Reset(Ability.CloakDuration);

            if (Ability.DecloakAfterDelay >= 0f)
            {
                _hasDecloakTimer = true;
                _decloakTimer.Reset(Ability.DecloakAfterDelay);
            }
            else
            {
                _hasDecloakTimer = false;
            }

            if (Ability.RequireTagForDetectionWhileCloaking)
            {
                Agent.RequireTagForDetection = true;
            }
        }

        protected override void OnExit()
        {
            _hasDecloakTimer = false;
            _cloakState = State.Decloaking;
            _timer.Reset(Ability.DecloakDuration);
            Agent.RequireTagForDetection = _reqTagOriginal;
        }

        protected override void OnUpdate()
        {
            float progress;
            switch (_cloakState)
            {
                case State.Cloaking:
                    progress = GetProgress();
                    _handlers.ForEach(h => h.SetProgress(1f - progress));

                    if (progress >= 1f)
                    {
                        _cloakState = State.None;
                        Agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;
                        Agent.SetAnimatorCullingEnabled(false);
                    }
                    break;

                case State.Decloaking:
                    progress = GetProgress();
                    _handlers.ForEach(h => h.SetProgress(progress));

                    if (progress >= 1f)
                    {
                        _cloakState = State.None;
                        Agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = false;
                        Agent.SetAnimatorCullingEnabled(true);
                    }
                    break;
            }

            if (_hasDecloakTimer && Executing && _decloakTimer.TickAndCheckDone())
            {
                DoExit();
            }
        }

        protected override void OnAbilityLazyUpdate()
        {
            Agent.MovingCuller.Culler.NeedsShadowRefresh = true;
        }

        private float GetProgress()
        {
            _timer.Tick();
            return _timer.Progress;
        }

        public enum State
        {
            None,
            Cloaking,
            Decloaking
        }

        private struct Handler
        {
            public bool hideShadowMode;
            public float minOpacity;
            public Renderer originalRenderer;
            public Renderer transitionRenderer;

            public void SetProgress(float p)
            {
                if (originalRenderer == null || transitionRenderer == null)
                {
                    Logger.Error($"Cloak Renderer is missing! : orig {originalRenderer == null}, trs {transitionRenderer == null}");
                    return;
                }

                if (p <= 0f)
                {
                    DisableOriginalRenderer();
                    if (minOpacity <= 0f)
                    {
                        transitionRenderer.forceRenderingOff = true;
                    }
                    else
                    {
                        transitionRenderer.forceRenderingOff = false;
                        transitionRenderer.material.color = Color.white.AlphaMultiplied(minOpacity);
                    }
                }
                else if (p >= 1f)
                {
                    EnableOriginalRenderer();
                    transitionRenderer.forceRenderingOff = true;
                    transitionRenderer.material.color = Color.white;
                }
                else
                {
                    DisableOriginalRenderer();
                    transitionRenderer.forceRenderingOff = false;
                    transitionRenderer.material.color = Color.white.AlphaMultiplied(Mathf.Lerp(minOpacity, 1f, p));
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