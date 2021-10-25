using AssetShards;
using EECustom.Customizations.Models.Handlers;
using EECustom.Extensions;
using Enemies;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace EECustom.Customizations.Models
{
    //Original Code from Dex-EnemyGhosts
    public class SilhouetteCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyPrefabBuiltEvent
    {
        public const string PlayerPrefabPath = "ASSETS/ASSETPREFABS/CHARACTERS/CHARACTER_A.PREFAB";
        public const string PlayerGhostName = "g_set_military_01_boots_ghost";

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

            var renderers = agent.GetComponentsInChildren<Renderer>(true);
            var rendererList = new List<Renderer>();
            foreach (var renderer in renderers)
            {
                rendererList.Add(renderer);
            }

            var charMats = agent.GetComponentInChildren<CharacterMaterialHandler>().m_materialRefs;
            foreach (var matRef in charMats)
            {
                if (!matRef.HasFeature(MaterialSupport.Destruction))
                {
                    RemoveFromRendererMatRef(matRef);
                    continue;
                }
                if (!matRef.HasFeature(MaterialSupport.Destruction))
                {
                    RemoveFromRendererMatRef(matRef);
                    continue;
                }

                foreach (var comp in matRef.m_renderers)
                {
                    if (comp.name.Equals("g_leg_l")) //MINOR: Shooter's left leg is always visible for some fucking reason
                    {
                        RemoveFromRenderer(comp);
                    }
                }

                void RemoveFromRendererMatRef(MaterialRef matRef)
                {
                    foreach (var renderer in matRef.m_renderers)
                    {
                        RemoveFromRenderer(renderer);
                    }
                }

                void RemoveFromRenderer(Renderer renderer)
                {
                    var index = rendererList.FindIndex(x => x.GetInstanceID() == renderer.GetInstanceID());
                    if (index != -1)
                    {
                        rendererList.RemoveAt(index);
                    }
                }
            }

            foreach (var renderer in rendererList)
            {
                Logger.Verbose($"Silhouette Object Found! : {renderer.name}");

                var enemyGraphic = renderer.gameObject;
                var enemyGhost = enemyGraphic.Instantiate(enemyGraphic.transform, "g_ghost");
                enemyGhost.layer = LayerMask.NameToLayer("Enemy");

                _ = enemyGhost.AddComponent<EnemySilhouette>();
                var newRenderer = enemyGhost.GetComponent<Renderer>();
                newRenderer.material = _SilhouetteMaterial;
                newRenderer.material.SetVector("_ColorA", Color.clear);
                newRenderer.material.SetVector("_ColorB", Color.clear);
                newRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
                newRenderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var silManager = agent.gameObject.AddComponent<SilhouetteHandler>();
            silManager._Agent = agent;
            silManager._DefaultColor = DefaultColor;
            silManager._RequireTag = RequireTag;
            silManager._ReplaceColorWithMarker = ReplaceColorWithMarker;
            silManager._KeepOnDead = KeepOnDead;
            silManager._DeadEffectDelay = DeadEffectDelay;
        }
    }
}