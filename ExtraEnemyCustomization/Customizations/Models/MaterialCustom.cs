using Enemies;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public sealed class MaterialCustom : RevertableEnemyCustomBase, IEnemyPrefabBuiltEvent, IEnemySpawnedEvent
    {
        private readonly static Dictionary<string, Material> _matDict = new();
        private readonly static Dictionary<string, Texture3D> _tex3DDict = new();

        public static void AddToCache(string matName, Material mat)
        {
            if (!_matDict.ContainsKey(matName))
                _matDict.Add(matName, mat);
        }

        public static void AddToCacheTexture3D(string texName, Texture3D texture3D)
        {
            if (!_tex3DDict.ContainsKey(texName))
                _tex3DDict.Add(texName, texture3D);
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
                    LogError($"MATERIAL WAS NOT CACHED!: {swapSet.To}");
                    continue;
                }

                LogDev($" - Trying to Replace Material, Before: {matName} After: {newMat.name}");

                var originalMat = mat.m_material;
                PushRevertJob(() =>
                {
                    mat.m_material = originalMat;
                });

                //TODO: FIX SKIN NOISE STUFF
                var newMaterial = new Material(newMat);
                switch(swapSet.SkinNoise)
                {
                    case SkinNoiseType.ForceOn:
                        newMaterial.SetFloat("_Enable_SkinNoise", 1.0f);
                        newMaterial.EnableKeyword("ENABLE_SKIN_NOISE");
                        break;

                    case SkinNoiseType.ForceOff:
                        newMaterial.SetFloat("_Enable_SkinNoise", 0.0f);
                        newMaterial.DisableKeyword("ENABLE_SKIN_NOISE");
                        break;
                }
                var newnewMaterial = new Material(newMat);

                if (!string.IsNullOrEmpty(swapSet.SkinNoiseTexture))
                {
                    if (_tex3DDict.TryGetValue(swapSet.SkinNoiseTexture, out var tex3D))
                    {
                        newnewMaterial.SetTexture("_VolumeNoise", tex3D);
                    }
                    else
                    {
                        LogError($"TEXTURE3D WAS NOT CACHED!: {swapSet.SkinNoiseTexture}");
                    }
                }
                
                mat.m_material = newnewMaterial;
                LogVerbose(" - Replaced!");
            }
        }
    }

    public class MaterialSwapSet
    {
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public SkinNoiseType SkinNoise { get; set; } = SkinNoiseType.KeepOriginal;
        public string SkinNoiseTexture { get; set; } = string.Empty;
    }

    public enum SkinNoiseType
    {
        KeepOriginal,
        ForceOn,
        ForceOff
    }

    public struct MaterialSwapCache
    {
        public MaterialRef matref;
        public Material original;
    }
}
