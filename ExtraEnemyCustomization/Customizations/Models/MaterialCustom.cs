using Enemies;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public class MaterialCustom : RevertableEnemyCustomBase, IEnemyPrefabBuiltEvent
    {
        private readonly static Dictionary<string, Material> _matDict = new();

        public static void AddToCache(string matName, Material mat)
        {
            if (!_matDict.ContainsKey(matName))
                _matDict.Add(matName, mat);
        }

        public MaterialSwapSet[] MaterialSets { get; set; } = new MaterialSwapSet[0];

        public override string GetProcessName()
        {
            return "Material";
        }

        public void OnPrefabBuilt(EnemyAgent agent)
        {
            var charMats = agent.GetComponentInChildren<CharacterMaterialHandler>().m_materialRefs;
            foreach (var mat in charMats)
            {
                var matName = mat.m_material.name;
                LogVerbose($" - Debug Info, Material Found: {matName}");

                var swapSet = MaterialSets.SingleOrDefault(x => x.From.Equals(matName));
                if (swapSet == null)
                    continue;

                if (!_matDict.TryGetValue(swapSet.To, out var newMat))
                {
                    Logger.Error($"MATERIAL IS NOT CACHED!: {swapSet.To}");
                    continue;
                }

                LogDev($" - Trying to Replace Material, Before: {matName} After: {newMat.name}");

                var originalMat = mat.m_material;
                PushRevertJob(() =>
                {
                    mat.m_material = originalMat;
                });

                LogVerbose(" - Replaced!");
            }
        }
    }

    public class MaterialSwapSet
    {
        public string From { get; set; } = "";
        public string To { get; set; } = "";
    }

    public struct MaterialSwapCache
    {
        public MaterialRef matref;
        public Material original;
    }
}