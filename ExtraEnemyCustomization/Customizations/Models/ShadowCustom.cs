using Enemies;
using UnityEngine;
using UnityEngine.Rendering;

namespace EECustom.Customizations.Models
{
    public sealed class ShadowCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public bool IncludeEggSack { get; set; } = false;
        public bool RequireTagForDetection { get; set; } = true;
        public bool FullyInvisible { get; set; } = false;

        public override string GetProcessName()
        {
            return "Shadow";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            agent.RequireTagForDetection = RequireTagForDetection;
            agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;

            agent.MovingCuller.Culler.Renderers.Clear();
            agent.MovingCuller.Culler.hasShadowsEnabled = true;
            agent.SetAnimatorCullingEnabled(false);

            var comps = agent.GetComponentsInChildren<Renderer>(true);
            foreach (var comp in comps)
            {
                if (!IncludeEggSack && comp.gameObject.name.Contains("Egg"))
                {
                    LogVerbose(" - Ignored EggSack Object!");
                    comp.shadowCastingMode = ShadowCastingMode.On;
                    comp.enabled = true;
                    continue;
                }

                if (FullyInvisible)
                {
                    comp.shadowCastingMode = ShadowCastingMode.Off;
                    comp.castShadows = false;
                    comp.forceRenderingOff = true;
                }
                else
                {
                    comp.castShadows = true;
                    comp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    comp.enabled = true;
                }

                var skinmeshrenderer = comp.TryCast<SkinnedMeshRenderer>();
                if (skinmeshrenderer != null)
                {
                    skinmeshrenderer.updateWhenOffscreen = true;
                }
            }
        }
    }
}