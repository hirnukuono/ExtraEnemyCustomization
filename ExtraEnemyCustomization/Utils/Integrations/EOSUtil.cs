using BepInEx.Unity.IL2CPP;
using EOS.Modules.World.EMP;
using EOS.Modules.World.EMP.Handlers;
using System.Runtime.CompilerServices;

namespace EEC.Utils.Integrations
{
    [CallConstructorOnLoad]
    public static class EOSUtil
    {
        public const string PLUGIN_GUID = "Amor.ExcellentObjectiveSetup";
        public static bool IsLoaded { get; private set; } = false;

        private static readonly object _empSource = null!;

        static EOSUtil()
        {
            if (IL2CPPChainloader.Instance.Plugins.ContainsKey(PLUGIN_GUID))
            {
                IsLoaded = true;
                _empSource = GetEMPSource_Unsafe();
            }
        }

        public static void EnableHUDEMP()
        {
            if (IsLoaded)
                EnableHUDEMP_Unsafe();
        }

        public static void DisableHUDEMP()
        {
            if (IsLoaded)
                DisableHUDEMP_Unsafe();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static object GetEMPSource_Unsafe()
        {
            return new EMPShock(UnityEngine.Vector3.zero, float.PositiveInfinity, float.PositiveInfinity);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void EnableHUDEMP_Unsafe()
        {
            EMPPlayerHudHandler.Instance?.AddAffectedBy((IEMPSource)_empSource);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void DisableHUDEMP_Unsafe()
        {
            EMPPlayerHudHandler.Instance?.RemoveAffectedBy((IEMPSource)_empSource);
        }
    }
}