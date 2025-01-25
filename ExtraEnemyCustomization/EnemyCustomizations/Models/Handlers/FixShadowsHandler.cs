using Enemies;
using Player;
using UnityEngine.Rendering;
using UnityEngine;

namespace EEC.EnemyCustomizations.Models.Handlers
{
    [InjectToIl2Cpp]
    public sealed class FixEnemySER : MonoBehaviour
    {
        EnemyAgent enemy;
        List<MeshRenderer> sammuta = new();
        List<SkinnedMeshRenderer> sammuta2 = new();
        ShadowEnemyRenderer ser = null;
        List<ShadowEnemyRenderer> sers = new();
        bool initialized;

        public void Update()
        {
            if (!initialized) return;
            if (enemy == null) return;
            if (sers.Count == 0) return;
            // main loop
            foreach (var jee in sers) if (!jee.gameObject.activeSelf) jee.gameObject.SetActive(true);
            if (!enemy.Alive) return;
            foreach (var tmp in sammuta) tmp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            foreach (var tmp in sammuta2) tmp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }

        public void Initialize(EnemyAgent enemy, bool thermalShadows)
        {
            //Debug.Log("fd eec enemyser attach");
            this.enemy = enemy;
            var tmpoerr = enemy.GetComponentsInChildren<ShadowEnemyRenderer>();
            if (tmpoerr != null) foreach (var oerr in tmpoerr) sers.Add(oerr);
            if (sers.Count > 0)
            {
                // natural shadow
                //Debug.Log($"fd eec naturalshadow!");
                initialized = true;
                return;
            }

            // unnatural shadows
            foreach (var guuu in enemy.GetComponentsInChildren<MeshRenderer>())
                if (guuu.gameObject.name.ToLower() == "infested")
                    sammuta.Add(guuu);

            foreach (var skinmeshrenderer in enemy.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (skinmeshrenderer.gameObject.name.ToLower() == "g_pouncer") sammuta2.Add(skinmeshrenderer);
                if (skinmeshrenderer.gameObject.name.ToLower() == "g_body") sammuta2.Add(skinmeshrenderer);
            }

            foreach (var tmp in sammuta) tmp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            foreach (var tmp in sammuta2) tmp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;

            if (thermalShadows)
            {
                //Debug.Log($"fd eec unnatural thermalshadow {thermalShadows}");

                var list2 = enemy.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                foreach (var tmp in list2)
                {
                    if (ShadowCustom.g_SER.Contains(tmp.gameObject.name.ToLower()))
                    {
                        //Debug.Log($"fd eec name match {tmp.gameObject.name.ToLower()}");
                        tmp.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                        ser = tmp.gameObject.AddComponent<ShadowEnemyRenderer>();
                        ser.Renderer = tmp;
                        sers.Add(ser);
                    }
                }
                //Debug.Log($"fd eec sers count {this.sers.Count}");
                if (sers.Count > 0)
                {
                    ser.MovingCuller = enemy.MovingCuller;
                    initialized = true;
                }
            }
        }
    }

    [InjectToIl2Cpp]
    public sealed class FixShadows : MonoBehaviour
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
        bool visibleFromBehind;
        bool unhide = false;
        PlayerAgent plr = PlayerManager.GetLocalPlayerAgent();

        public void Initialize(EnemyAgent enemy, Renderer rend, Renderer sphere, Renderer shadow, bool visibleFromBehind)
        {
            //Debug.Log("fd eec tumor attach");
            jee = Time.realtimeSinceStartup;
            this.enemy = enemy;
            mr1 = sphere;
            mr1.gameObject.AddComponent<ShadowEnemyRenderer>();
            mr2 = shadow;
            orig = rend;
            foreach (var re in enemy.GetComponentsInChildren<Renderer>()) renderers.Add(re);
            bulb = orig.GetComponentInParent<Dam_EnemyDamageLimb_Custom>();
            if (bulb != null) bulb.add_OnLimbDestroyed((Action)Done);
            enemy.SetAnimatorCullingEnabled(false);
            initialized = true;
            this.visibleFromBehind = visibleFromBehind;
        }

        public void Done()
        {
            mr2.forceRenderingOff = true;
            mr2.castShadows = false;
            mr2.receiveShadows = false;
            enemy.MovingCuller.CullBucket.ShadowRenderers.Remove(mr2);
            foreach (var asd in enemy.MaterialHandler.m_materialRefs) try { asd.m_renderers.Remove(mr2); } catch (Exception e) { };
            enemy.MovingCuller.CullBucket.ComputeTotalBounds();
            enemy.MovingCuller.CullBucket.NeedsShadowRefresh = true;
            destroyed = true;
        }

        public void Update()
        {
            if (destroyed) return;
            if (!initialized) return;
            if (Time.realtimeSinceStartup - jee < 1) return;
            if (enemy == null) return;
            if (!enemy.Alive)
            {
                enemy.MovingCuller.CullBucket.Renderers.Remove(mr1);
                enemy.MovingCuller.CullBucket.ShadowRenderers.Remove(mr2);
                foreach (var tmp in enemy.MaterialHandler.m_materialRefs) tmp.m_renderers.Remove(mr2);
                destroyed = true;
                return;
            }

            // main loop

            unhide = false;

            if (mr1.shadowCastingMode != ShadowCastingMode.Off) mr1.shadowCastingMode = ShadowCastingMode.Off;

            if (visibleFromBehind)
            {
                // calculate angle from enemy to player, if 175-185, plop tumor visible
                Vector3 enemyLookDirection = enemy.transform.forward;
                Vector3 playerRelativeDirection = (plr.transform.position - enemy.transform.position).normalized;
                float angle = Vector3.Angle(enemyLookDirection, playerRelativeDirection);

                if (angle > 168) unhide = true;
                if (unhide)
                {
                    mr1.forceRenderingOff = false;
                    return;
                }
            }

            if (!mr1.forceRenderingOff) mr1.forceRenderingOff = true;

            foreach (var re in renderers)
            {
                if (re.shadowCastingMode != ShadowCastingMode.ShadowsOnly) re.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                if (!re.castShadows) re.castShadows = true;
                if (!enemy.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow) enemy.MovingCuller.m_disableAnimatorCullingWhenRenderingShadow = true;
                if (enemy.MovingCuller.m_animatorCullingEnabled) enemy.SetAnimatorCullingEnabled(false);
                if (!enemy.MovingCuller.Culler.hasShadowsEnabled) enemy.MovingCuller.Culler.hasShadowsEnabled = true;
                if (mr2.shadowCastingMode != ShadowCastingMode.ShadowsOnly) mr2.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }
    }
}
