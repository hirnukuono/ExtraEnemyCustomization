using EEC.EnemyCustomizations.Models.Handlers;
using Enemies;
using GameData;
using IRF;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace EEC.EnemyCustomizations.Models
{
    public enum ShadowType : byte
    {
        LegacyShadows,
        NewShadows
    }

    public sealed class ShadowCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyPrefabBuiltEvent
    {
        public bool IncludeEggSack { get; set; } = false;
        public bool RequireTagForDetection { get; set; } = false;
        public bool FullyInvisible { get; set; } = false;
        public ShadowType Type { get; set; } = ShadowType.LegacyShadows;
        public bool IncludeThermals { get; set; } = true;
        public bool TumorVisibleFromBehind { get; set; } = false;

        public static readonly string[] g_SER = new[] { "g_body_shadow", "g_shooter", "g_tank", "g_birther", "g_pouncer_shadow" };

        public override string GetProcessName()
        {
            return "Shadow";
        }

        public void OnPrefabBuilt(EnemyAgent agent, EnemyDataBlock enemyData)
        {
            if (Type != ShadowType.LegacyShadows) return;

            agent.RequireTagForDetection = RequireTagForDetection;

            var comps = agent.GetComponentsInChildren<Renderer>(true);
            foreach (var comp in comps)
            {
                if (!IncludeEggSack && comp.gameObject.name.InvariantContains("Egg"))
                {
                    if (Logger.VerboseLogAllowed) LogVerbose(" - Ignored EggSack Object!");
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
                    comp.gameObject.AddComponent<ShadowEnemyRenderer>(); //Love you mccad00 from gtfo unofficial modding
                }
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            if (Type == ShadowType.LegacyShadows)
            {
                agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;
                agent.MovingCuller.Culler.hasShadowsEnabled = true;
                agent.SetAnimatorCullingEnabled(false);
            }
            else
            {
                TSF_SpawnEnemy(agent); // love you hirnu from gtfo modding
            }
        }

        public void TSF_SpawnEnemy(EnemyAgent agent)
        {
            agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;
            agent.MovingCuller.Culler.hasShadowsEnabled = true;
            agent.SetAnimatorCullingEnabled(false);
            agent.RequireTagForDetection = RequireTagForDetection;

            foreach (var compIRF in agent.GetComponentsInChildren<InstancedRenderFeature>(true))
            {
                compIRF.enabled = false;
            }

            if (IncludeThermals)
            {
                agent.gameObject.AddComponent<FixEnemySER>().Initialize(agent, IncludeThermals);
            }

            foreach (var comp in agent.GetComponentsInChildren<Renderer>(true))
            {
                if (FullyInvisible)
                {
                    Object.Destroy(comp);
                    continue;
                }

                if (IncludeEggSack)
                {
                    comp.receiveShadows = true;
                    comp.castShadows = true;
                    comp.shadowCastingMode = ShadowCastingMode.On;
                }

                var skinnedMeshRenderer = comp.TryCast<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null && g_SER.Contains(skinnedMeshRenderer.gameObject.name.ToLower()))
                {
                    skinnedMeshRenderer.updateWhenOffscreen = true;
                }

                comp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;

                if (!comp.name.InvariantContains("Egg", ignoreCase: true) && !comp.name.InvariantContains("FleshSack", ignoreCase: true))
                {
                    continue;
                }

                agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;
                comp.receiveShadows = false;
                comp.castShadows = false;
                comp.shadowCastingMode = ShadowCastingMode.Off;
                comp.enabled = false;

                var eggSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                var shadowSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                eggSphere.name = "fd_egg_sphere1";
                shadowSphere.name = "fd_egg_sphere2_shadow";
                eggSphere.transform.parent = comp.transform;
                eggSphere.transform.localScale = Vector3.one;
                eggSphere.transform.localPosition = comp.transform.localPosition;
                shadowSphere.transform.parent = comp.transform;
                shadowSphere.transform.localScale = Vector3.one;
                shadowSphere.transform.localPosition = comp.transform.localPosition;

                var eggMR = eggSphere.GetComponent<MeshRenderer>();
                var shadowMR = shadowSphere.GetComponent<MeshRenderer>();
                eggMR.material = Object.Instantiate(eggMR.sharedMaterial);
                shadowMR.material = Object.Instantiate(eggMR.sharedMaterial);
                eggMR.material.SetTexture("_MainTex", comp.sharedMaterial.mainTexture);
                eggMR.shadowCastingMode = ShadowCastingMode.Off;
                eggMR.castShadows = false;
                eggMR.material.color = new Color(1, 1, 1, 1);
                eggMR.material.shader = Shader.Find("Cell/Enemy/EnemyFlesh_CD");
                eggMR.material.enableInstancing = false;
                shadowMR.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                shadowMR.castShadows = true;
                shadowMR.receiveShadows = false;
                shadowMR.material.color = new Color(1, 1, 1, 1);
                shadowMR.material.shader = Shader.Find("Cell/Enemy/EnemyFlesh_CD");
                shadowMR.material.enableInstancing = false;

                eggSphere.GetComponent<SphereCollider>().enabled = false;
                shadowSphere.GetComponent<SphereCollider>().enabled = false;
                var eggRenderer = eggSphere.GetComponent<Renderer>();
                var shadowRenderer = shadowSphere.GetComponent<Renderer>();

                try
                {
                    foreach (var matlRef in agent.MaterialHandler.m_materialRefs)
                    {
                        matlRef.m_renderers.Add(shadowRenderer);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"cmh error {e}");
                }

                eggRenderer.shadowCastingMode = ShadowCastingMode.Off;
                shadowRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;

                agent.MovingCuller.CullBucket.Renderers.Remove(comp);
                agent.MovingCuller.CullBucket.Renderers.Add(eggRenderer);
                agent.MovingCuller.Culler.hasShadowsEnabled = true;
                agent.MovingCuller.CullBucket.ShadowRenderers.Add(shadowRenderer);

                comp.gameObject.AddComponent<FixShadows>().Initialize(agent, comp, eggRenderer, shadowRenderer, TumorVisibleFromBehind);
            }

            agent.MovingCuller.CullBucket.ComputeTotalBounds();
            agent.MovingCuller.CullBucket.NeedsShadowRefresh = true;
        }
    }
}
