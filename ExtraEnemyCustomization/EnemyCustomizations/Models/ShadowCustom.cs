using Enemies;
using FluffyUnderware.DevTools.Extensions;
using GameData;
using IRF;
using Player;
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

        private static readonly string[] g_SER = new[] { "g_body_shadow", "g_shooter", "g_tank", "g_birther", "g_pouncer_shadow" };

        public override string GetProcessName()
        {
            return "Shadow";
        }

        public void OnPrefabBuilt(EnemyAgent agent, EnemyDataBlock enemyData)
        {
            if (Type != ShadowType.LegacyShadows)
                return;

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
                    Debug.Log("fd eec uus shadowenemyrenderer menee sisään");
                    comp.gameObject.AddComponent<ShadowEnemyRenderer>(); //Love you mccad00 from gtfo unofficial modding
                }
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            Debug.Log($"fd eec onprefabbuilt {Type} {IncludeThermals}");
            if (Type == ShadowType.LegacyShadows)
            {
                agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;
                agent.MovingCuller.Culler.hasShadowsEnabled = true;
                agent.SetAnimatorCullingEnabled(false);
            }
            else
                TSF_SpawnEnemy(agent);
        }

        public void TSF_SpawnEnemy(EnemyAgent agent)
        {
            agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;
            agent.MovingCuller.Culler.hasShadowsEnabled = true;
            agent.SetAnimatorCullingEnabled(false);
            agent.RequireTagForDetection = RequireTagForDetection;

            foreach (var compIRF in agent.GetComponentsInChildren<InstancedRenderFeature>(true))
                compIRF.enabled = false;

            if (IncludeThermals)
                agent.gameObject.AddComponent<FixEnemySER>().Attach(agent, IncludeThermals);

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

                SkinnedMeshRenderer? skinnedMeshRenderer = comp.TryCast<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                    if (g_SER.Contains(skinnedMeshRenderer.gameObject.name.ToLower()))
                        skinnedMeshRenderer.updateWhenOffscreen = true;

                if (comp.enabled)
                    comp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;

                if (!comp.name.ToLower().Contains("egg", StringComparison.InvariantCultureIgnoreCase) && !comp.name.ToLower().Contains("fleshsack", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                agent.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;
                comp.receiveShadows = false;
                comp.castShadows = false;
                comp.shadowCastingMode = ShadowCastingMode.Off;
                comp.enabled = false;
                var go1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                var go2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go1.name = "fd_egg_sphere1";
                go2.name = "fd_egg_sphere2_shadow";
                go1.transform.parent = comp.transform;
                go1.transform.localScale = Vector3.one;
                go1.transform.localPosition = comp.transform.localPosition;
                go2.transform.parent = comp.transform;
                go2.transform.localScale = Vector3.one;
                go2.transform.localPosition = comp.transform.localPosition;

                var mr1 = go1.GetComponent<MeshRenderer>();
                var mr2 = go2.GetComponent<MeshRenderer>();
                Material mat1 = Object.Instantiate(mr1.sharedMaterial);
                Material mat2 = Object.Instantiate(mr1.sharedMaterial);

                mr1.material = mat1;
                mr2.material = mat2;
                mr1.material.SetTexture("_MainTex", comp.sharedMaterial.mainTexture);
                mr1.shadowCastingMode = ShadowCastingMode.Off;
                mr1.castShadows = false;
                mr1.material.color = new Color(1, 1, 1, 1);
                mr1.material.shader = Shader.Find("Cell/Enemy/EnemyFlesh_CD");
                mr1.material.enableInstancing = false;
                mr2.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                mr2.castShadows = true;
                mr2.receiveShadows = false;
                mr2.material.color = new Color(1, 1, 1, 1);
                mr2.material.shader = Shader.Find("Cell/Enemy/EnemyFlesh_CD");
                mr2.material.enableInstancing = false;

                var co1 = go1.GetComponent<SphereCollider>();
                co1.enabled = false;
                var co2 = go2.GetComponent<SphereCollider>();
                co2.enabled = false;
                var r1 = go1.GetComponent<Renderer>();
                var r2 = go2.GetComponent<Renderer>();

                try
                {
                    foreach (var tmp in agent.MaterialHandler.m_materialRefs)
                        tmp.m_renderers.Add(r2);
                }
                catch (Exception e)
                {
                    Debug.LogError($"cmh error {e}");
                }

                r1.shadowCastingMode = ShadowCastingMode.Off;
                r2.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                agent.MovingCuller.CullBucket.Renderers.Remove(comp);
                agent.MovingCuller.CullBucket.Renderers.Add(r1);
                agent.MovingCuller.Culler.hasShadowsEnabled = true;
                agent.MovingCuller.CullBucket.ShadowRenderers.Add(r2);
                comp.gameObject.AddComponent<FixShadows>().Attach(agent, comp, r1, r2, TumorVisibleFromBehind);
            }
            agent.MovingCuller.CullBucket.ComputeTotalBounds();
            agent.MovingCuller.CullBucket.NeedsShadowRefresh = true;
        }

        public class FixEnemySER : MonoBehaviour
        {
            EnemyAgent enemy;
            List<MeshRenderer> sammuta = new();
            List<SkinnedMeshRenderer> sammuta2 = new();
            SkinnedMeshRenderer smr = null;
            ShadowEnemyRenderer ser = null;
            List<ShadowEnemyRenderer> sers = new();
            //Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppArrayBase<ShadowEnemyRenderer> sers;
            bool initialized;

            public void Update()
            {
                if (!initialized) return;
                if (enemy == null) return;
                if (sers.Count == 0) return;
                // main loop
                foreach (var jee in sers) if (!jee.gameObject.activeSelf) jee.gameObject.SetActive(true);
                if (!this.enemy.Alive) return;
                foreach (var tmp in sammuta) tmp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                foreach (var tmp in sammuta2) tmp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }

            public void Attach(EnemyAgent enemy, bool thermalShadows)
            {
                Debug.Log("fd eec enemyser attach");
                this.enemy = enemy;
                var tmpoerr = enemy.GetComponentsInChildren<ShadowEnemyRenderer>();
                if (tmpoerr != null) foreach (var oerr in tmpoerr) sers.Add(oerr);
                if (sers.Count > 0)
                {
                    // natural shadow
                    Debug.Log($"fd eec naturalshadow!");
                    this.initialized = true;
                    return;
                }
                // unnatural shadows
                var gaaa = enemy.GetComponentsInChildren<MeshRenderer>();
                foreach (var guuu in gaaa) if (guuu.gameObject.name.ToLower() == "infested") sammuta.Add(guuu);
                var list = enemy.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var skinmeshrenderer in list)
                {
                    if (skinmeshrenderer.gameObject.name.ToLower() == "g_pouncer") sammuta2.Add(skinmeshrenderer);
                    if (skinmeshrenderer.gameObject.name.ToLower() == "g_body") sammuta2.Add(skinmeshrenderer);
                }

                foreach (var tmp in sammuta) tmp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                foreach (var tmp in sammuta2) tmp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;

                if (thermalShadows)
                {
                    Debug.Log($"fd eec unnatural thermalshadow {thermalShadows}");

                    var list2 = this.enemy.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                    foreach (var tmp in list2)
                    {
                        if (g_SER.Contains(tmp.gameObject.name.ToLower()))
                        {
                            Debug.Log($"fd eec name match {tmp.gameObject.name.ToLower()}");
                            tmp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                            this.ser = tmp.gameObject.AddComponent<ShadowEnemyRenderer>();
                            this.ser.Renderer = tmp;
                            this.sers.Add(this.ser);
                        }
                    }
                    Debug.Log($"fd eec sers count {this.sers.Count}");
                    if (this.sers.Count > 0)
                    {
                        this.ser.MovingCuller = this.enemy.MovingCuller;
                        this.initialized = true;
                    }
                }
            }
        }

        public class FixShadows : MonoBehaviour
        {
            Renderer orig;
            EnemyAgent enemy;
            Renderer mr1;
            Renderer mr2;
            bool destroyed = false;
            bool initialized = false;
            List<Renderer> renderers = new();
            float jee;
            Dam_EnemyDamageLimb_Custom bulb;
            bool tempset;
            bool visibleFromBehind;
            bool unhide = false;
            PlayerAgent plr = PlayerManager.GetLocalPlayerAgent();
            float debugreporttime;

            public void Attach(EnemyAgent enemy, Renderer rend, Renderer sphere, Renderer shadow, bool visibleFromBehind)
            {
                Debug.Log("fd eec tumor attach");
                this.jee = Time.realtimeSinceStartup;
                this.enemy = enemy;
                this.mr1 = sphere;
                this.mr2 = shadow;
                this.orig = rend;
                foreach (var re in enemy.GetComponentsInChildren<Renderer>()) renderers.Add(re);
                this.bulb = this.orig.GetComponentInParent<Dam_EnemyDamageLimb_Custom>();
                if (bulb != null) bulb.add_OnLimbDestroyed((Action)this.Done);
                enemy.SetAnimatorCullingEnabled(false);
                initialized = true;
                this.visibleFromBehind = visibleFromBehind;
            }

            public void Done()
            {
                this.mr2.forceRenderingOff = true;
                this.mr2.castShadows = false;
                this.mr2.receiveShadows = false;
                this.enemy.MovingCuller.CullBucket.ShadowRenderers.Remove(mr2);
                foreach (var asd in this.enemy.MaterialHandler.m_materialRefs) try { asd.m_renderers.Remove(mr2); } catch (Exception e) { };
                this.enemy.MovingCuller.CullBucket.ComputeTotalBounds();
                this.enemy.MovingCuller.CullBucket.NeedsShadowRefresh = true;
                this.destroyed = true;


            }
            public void Update()
            {
                if (destroyed) return;
                if (!initialized) return;
                if (Time.realtimeSinceStartup - this.jee < 1) return;
                if (enemy == null) return;
                if (!enemy.Alive)
                {
                    this.enemy.MovingCuller.CullBucket.Renderers.Remove(mr1);
                    this.enemy.MovingCuller.CullBucket.ShadowRenderers.Remove(mr2);
                    foreach (var tmp in enemy.MaterialHandler.m_materialRefs) tmp.m_renderers.Remove(mr2);
                    destroyed = true;
                    return;
                }

                // main loop

                unhide = false;

                if (this.mr1.shadowCastingMode != ShadowCastingMode.Off) this.mr1.shadowCastingMode = ShadowCastingMode.Off;

                if (visibleFromBehind)
                {
                    // calculate angle from enemy to player, if 175-185, plop tumor visible
                    Vector3 enemyLookDirection = enemy.transform.forward;
                    Vector3 playerRelativeDirection = (plr.transform.position - enemy.transform.position).normalized;
                    float angle = Vector3.Angle(enemyLookDirection, playerRelativeDirection);

                    if (angle > 168) unhide = true;
                    if (unhide)
                    {
                        this.mr1.forceRenderingOff = false;
                        return;
                    }
                }
                if (!this.mr1.forceRenderingOff) this.mr1.forceRenderingOff = true;
                foreach (var re in this.renderers)
                {
                    if (re.shadowCastingMode != ShadowCastingMode.ShadowsOnly) re.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    if (!re.castShadows) re.castShadows = true;
                    if (!this.enemy.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow) this.enemy.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;
                    if (this.enemy.MovingCuller.m_animatorCullingEnabled) this.enemy.SetAnimatorCullingEnabled(false);
                    if (!this.enemy.MovingCuller.Culler.hasShadowsEnabled) this.enemy.MovingCuller.Culler.hasShadowsEnabled = true;
                    if (this.mr2.shadowCastingMode != ShadowCastingMode.ShadowsOnly) this.mr2.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }
            }
        }
    }
}