using EECustom.Customizations.Models;
using EECustom.Utils.Integrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace EECustom.Managers
{
    public static class AssetCacheManager
    {
        public enum OutputType
        {
            None, Console, File
        }

        public static OutputType OutputMethod { get; set; } = OutputType.None;

        public static Assets<Material> Materials { get; } = new();
        public static Assets<Texture3D> Texture3Ds { get; } = new();

        internal static void AssetLoaded()
        {
            Materials.Cache((mat) => {
                var shaderName = mat?.shader?.name ?? string.Empty;

                if (!ConfigManager.Current.Global.CacheAllMaterials)
                {
                    if (!shaderName.Contains("EnemyFlesh"))
                        return false;
                }
                return true;
            });
            Texture3Ds.Cache(null);
        }

        public class Assets<T> where T : UnityEngine.Object
        {
            public string Name => typeof(T).Name;

            private StreamWriter _fileStream = null;
            private readonly Dictionary<string, T> _lookup = new();

            internal void Cache(Predicate<T> shouldCache)
            {
                StartLog();
                var objects = Resources.FindObjectsOfTypeAll(Il2CppType.Of<T>());

                foreach (var obj in objects)
                {
                    var castedObj = obj.Cast<T>();
                    var objName = castedObj?.name ?? string.Empty;

                    if (string.IsNullOrEmpty(objName))
                        continue;

                    if (!(shouldCache?.Invoke(castedObj) ?? true))
                        continue;

                    _lookup[objName] = castedObj;
                    DoLog(objName);
                }
                EndLog();
            }

            internal void Set(string name, T obj)
            {
                _lookup[name] = obj;
            }

            public bool TryGet(string name, out T obj)
            {
                return _lookup.TryGetValue(name, out obj);
            }

            private void StartLog()
            {
                switch (OutputMethod)
                {
                    case OutputType.Console:
                        Logger.Debug($"Caching {Name} Lists...");
                        break;

                    case OutputType.File:
                        _fileStream = new StreamWriter(File.OpenWrite(Path.Combine(ConfigManager.BasePath, $"_dump.{Name.ToLower()}.txt")));
                        break;
                }
            }

            private void DoLog(string assetName)
            {
                switch (OutputMethod)
                {
                    case OutputType.Console:
                        Logger.Debug(assetName);
                        break;

                    case OutputType.File:
                        _fileStream.WriteLine(assetName);
                        break;
                }
            }

            private void EndLog()
            {
                switch (OutputMethod)
                {
                    case OutputType.Console:
                        Logger.Debug("Done!");
                        break;

                    case OutputType.File:
                        _fileStream.Dispose();
                        break;
                }
            }
        }
    }
}
