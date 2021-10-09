using AssetShards;
using EECustom.Customizations.Models.Managers;
using EECustom.Events;
using EECustom.Extensions;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace EECustom.Customizations.Models
{
    //Original Code from Dex-EnemyGhosts
    public class SilhouetteCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyPrefabBuiltEvent
    {
        public static readonly string PlayerPrefabPath = "ASSETS/ASSETPREFABS/CHARACTERS/CHARACTER_A.PREFAB";
        public static readonly string PlayerGhostName = "g_set_military_01_boots_ghost";

        public Color DefaultColor { get; set; } = Color.red;
        public bool RequireTag { get; set; } = true;
        public bool ReplaceColorWithMarker { get; set; } = true;
        public bool KeepOnDead { get; set; } = false;
        public float DeadEffectDelay { get; set; } = 0.75f;

        private static bool _MaterialCached = false;
        private static Material _SilhouetteMaterial;

        public override string GetProcessName()
        {
            return "Silhouette";
        }

        public void OnPrefabBuilt(EnemyAgent agent)
        {
            if (!_MaterialCached)
            {
                var playerPrefab = AssetShardManager.s_loadedAssetsLookup[PlayerPrefabPath.ToUpper()].Cast<GameObject>();
                var playerGhost = playerPrefab.FindChild(PlayerGhostName);
                var playerGhostRenderer = playerGhost.GetComponent<SkinnedMeshRenderer>();
                _SilhouetteMaterial = playerGhostRenderer.material;
                _MaterialCached = true;
            }

            
            var comps = agent.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var comp in comps)
            {
                var enemyGraphic = comp.gameObject;
                var enemyGhost = enemyGraphic.Instantiate(comp.gameObject.transform, "g_ghost");
                enemyGhost.layer = LayerMask.NameToLayer("Enemy");

                _ = enemyGhost.AddComponent<EnemySilhouette>();
                var newRenderer = enemyGhost.GetComponent<SkinnedMeshRenderer>();
                newRenderer.material = _SilhouetteMaterial;
                newRenderer.material.SetVector("_ColorA", Color.clear);
                newRenderer.material.SetVector("_ColorB", Color.clear);
                newRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
                newRenderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
                newRenderer.castShadows = false;
                newRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var silManager = agent.gameObject.AddComponent<SilhouetteManager>();
            silManager._Agent = agent;
            silManager._DefaultColor = DefaultColor;
            silManager._RequireTag = RequireTag;
            silManager._ReplaceColorWithMarker = ReplaceColorWithMarker;
            silManager._KeepOnDead = KeepOnDead;
            silManager._DeadEffectDelay = DeadEffectDelay;
        }
    }
}
