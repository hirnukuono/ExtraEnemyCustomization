using Enemies;
using UnityEngine;
using UnityEngine.Rendering;

namespace EEC.EnemyCustomizations.Models
{
    public sealed class ShadowCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyPrefabBuiltEvent
    {
        public bool IncludeEggSack { get; set; } = false;
        public bool RequireTagForDetection { get; set; } = true;
        public bool FullyInvisible { get; set; } = false;

        public override string GetProcessName()
        {
            return "Shadow";
        }

        public void OnPrefabBuilt(EnemyAgent agent)
        {
            agent.RequireTagForDetection = RequireTagForDetection;

            var comps = agent.GetComponentsInChildren<Renderer>(true);
            foreach (var comp in comps)
            {
                if (!IncludeEggSack && comp.gameObject.name.InvariantContains("Egg"))
                {
                    if (Logger.VerboseLogAllowed)
                        LogVerbose(" - Ignored EggSack Object!");
                    comp.shadowCastingMode = ShadowCastingMode.On;
                    comp.enabled = true;
                    continue;
                }

                var skinmeshrenderer = comp.TryCast<SkinnedMeshRenderer>();
                if (skinmeshrenderer != null)
                {
                    skinmeshrenderer.updateWhenOffscreen = true;
                }

                if (FullyInvisible)
                {
                    Object.Destroy(comp);
                }
                else
                {
                    comp.castShadows = true;
                    comp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;
            agent.MovingCuller.Culler.hasShadowsEnabled = true;
            agent.SetAnimatorCullingEnabled(false);
        }
    }
}