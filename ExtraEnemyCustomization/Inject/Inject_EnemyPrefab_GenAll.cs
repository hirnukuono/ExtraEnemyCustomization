using EECustom.Customizations.Models;
using EECustom.Managers;
using Enemies;
using HarmonyLib;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(EnemyPrefabManager), nameof(EnemyPrefabManager.GenerateAllEnemyPrefabs))]
    internal static class Inject_EnemyPrefab_GenAll
    {
        [HarmonyWrapSafe]
        public static void Prefix()
        {
            Logger.Debug("== List of Material that can be used for Materials Parameters ==");

            //TODO: Replace this to AssetShardManager Code
            var fullmats = Resources.FindObjectsOfTypeAll(Il2CppType.Of<Material>());
            var fulltex3Ds = Resources.FindObjectsOfTypeAll(Il2CppType.Of<Texture3D>());

            foreach (var obj in fullmats)
            {
                var mat = obj.Cast<Material>();
                var matName = mat?.name ?? string.Empty;
                var shaderName = mat?.shader?.name ?? string.Empty;

                if (string.IsNullOrEmpty(matName))
                    continue;

                if (!ConfigManager.Current.ModelCustom.CacheAllMaterials)
                {
                    if (!shaderName.Contains("EnemyFlesh"))
                        continue;
                }

                MaterialCustom.AddToCache(matName, mat);
                Logger.Debug(matName);
            }

            Logger.Debug("== End of List ==");
            Logger.Debug("== List of Texture3D that can be used for SkinNoiseTexture Parameters ==");

            foreach (var obj in fulltex3Ds)
            {
                var tex3D = obj.Cast<Texture3D>();
                var texName = tex3D?.name ?? string.Empty;

                if (string.IsNullOrEmpty(texName))
                    continue;

                MaterialCustom.AddToCacheTexture3D(texName, tex3D);
                Logger.Debug(texName);
            }

            Logger.Debug("== End of List ==");
        }
    }
}