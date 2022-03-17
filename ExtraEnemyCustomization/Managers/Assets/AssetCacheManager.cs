using System.Collections.Generic;
using System.IO;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace EEC.Managers.Assets
{
    public static class AssetCacheManager
    {
        public enum OutputType
        {
            None, Console, File
        }

        public static OutputType OutputMethod { get; set; } = OutputType.None;

        public static Assets<Material> Materials { get; } = new MaterialAssets();
        public static Assets<Texture3D> Texture3Ds { get; } = new();

        internal static void AssetLoaded()
        {
            Materials.Cache();
            Texture3Ds.Cache();
        }

        public sealed class MaterialAssets : Assets<Material>
        {
            public override string DoResolveNameConflict(Material obj, string currentName)
            {
                var newName = $"{currentName}::{obj.shader.name}";
                if (Contains(newName))
                {
                    newName = $"DUPLICATE ITEM";
                }
                return newName;
            }

            public override bool ShouldCache(Material mat)
            {
                if (mat == null)
                    return false;

                if (mat.shader == null)
                    return false;

                if (!ConfigManager.Global.CacheAllMaterials)
                {
                    if (!mat.shader.name.InvariantContains("EnemyFlesh"))
                        return false;
                }
                return true;
            }
        }

        public class Assets<T> where T : Object
        {
            public string Name => typeof(T).Name;

            private StreamWriter _streamWriter = null;
            private readonly Dictionary<string, T> _lookup = new();

            public virtual string DoResolveNameConflict(T obj, string currentName)
            {
                return currentName;
            }

            public virtual bool ShouldCache(T obj)
            {
                return true;
            }

            internal void Cache()
            {
                StartLog();
                var objects = Resources.FindObjectsOfTypeAll(Il2CppType.Of<T>());

                foreach (var obj in objects)
                {
                    var castedObj = obj.Cast<T>();
                    var objName = castedObj?.name ?? string.Empty;

                    if (_lookup.ContainsKey(objName))
                    {
                        objName = DoResolveNameConflict(castedObj, objName);
                        if (_lookup.ContainsKey(objName))
                            continue;
                    }

                    if (string.IsNullOrEmpty(objName))
                        continue;

                    if (!ShouldCache(castedObj))
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

            public bool Contains(string name)
            {
                return _lookup.ContainsKey(name);
            }

            private void StartLog()
            {
                switch (OutputMethod)
                {
                    case OutputType.Console:
                        Logger.Debug($"Caching {Name} Lists...");
                        break;

                    case OutputType.File:
                        var fileStream = File.Open(Path.Combine(ConfigManager.BasePath, $"_dump.{Name.ToLowerInvariant()}.txt"), FileMode.Create, FileAccess.Write);
                        _streamWriter = new StreamWriter(fileStream);
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
                        _streamWriter.WriteLine(assetName);
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
                        _streamWriter.Dispose();
                        break;
                }
            }
        }
    }
}