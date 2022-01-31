using EECustom.Managers;
using Enemies;
using System;
using System.Linq;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public sealed class MaterialCustom : RevertableEnemyCustomBase, IEnemyPrefabBuiltEvent
    {
        public MaterialSwapSet[] MaterialSets { get; set; } = Array.Empty<MaterialSwapSet>();

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

                if (!AssetCacheManager.Materials.TryGet(swapSet.To, out var newMat))
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

                var newMaterial = new Material(newMat);
                switch (swapSet.SkinNoise)
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

                if (!string.IsNullOrEmpty(swapSet.SkinNoiseTexture))
                {
                    if (AssetCacheManager.Texture3Ds.TryGet(swapSet.SkinNoiseTexture, out var tex3D))
                    {
                        newMaterial.SetTexture("_VolumeNoise", tex3D);
                    }
                    else
                    {
                        LogError($"TEXTURE3D WAS NOT CACHED!: {swapSet.SkinNoiseTexture}");
                    }
                }

                foreach (var colorProp in swapSet.ColorProperties)
                {
                    if (!newMaterial.HasProperty(colorProp.Name))
                    {
                        LogError($"Color Property is missing: {colorProp.Name}");
                        continue;
                    }
                    newMaterial.SetColor(colorProp.Name, colorProp.Value);
                }

                foreach (var floatProp in swapSet.FloatProperties)
                {
                    if (!newMaterial.HasProperty(floatProp.Name))
                    {
                        LogError($"Float Property is missing: {floatProp.Name}");
                        continue;
                    }
                    newMaterial.SetFloat(floatProp.Name, floatProp.Value);
                }

                mat.m_material = newMaterial;
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
        public ColorSetting[] ColorProperties { get; set; } = Array.Empty<ColorSetting>();
        public FloatSetting[] FloatProperties { get; set; } = Array.Empty<FloatSetting>();

        public struct ColorSetting
        {
            public string Name { get; set; }
            public Color Value { get; set; }
        }

        public struct FloatSetting
        {
            public string Name { get; set; }
            public float Value { get; set; }
        }
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