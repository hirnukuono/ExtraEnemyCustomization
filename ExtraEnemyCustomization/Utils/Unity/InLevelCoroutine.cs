using BepInEx.Unity.IL2CPP.Utils.Collections;
using EEC.Events;
using EEC.Networking;
using SNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EEC.Utils.Unity
{
    [CallConstructorOnLoad]
    public static class InLevelCoroutine
    {
        private static InLevelCoroutineHandler _handler;

        static InLevelCoroutine()
        {
            AssetEvents.AllAssetLoaded += AssetEvents_AllAssetLoaded;
            LevelEvents.LevelCleanup += LevelEvents_LevelCleanup;
            SNetEvents.PrepareRecall += SNetEvents_PrepareRecall;
        }

        private static void AssetEvents_AllAssetLoaded()
        {
            if (_handler == null)
            {
                var mgrObj = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(mgrObj);
                _handler = mgrObj.AddComponent<InLevelCoroutineHandler>();
            }
        }
        private static void SNetEvents_PrepareRecall(eBufferType _) => StopAll();
        private static void LevelEvents_LevelCleanup() => StopAll();

        public static Coroutine Start(IEnumerator coroutine)
        {
            if (_handler != null && GameStateManager.IsInExpedition)
            {
                return _handler.StartCoroutine(coroutine.WrapToIl2Cpp());
            }
            return null;
        }

        public static void Stop(Coroutine coroutine)
        {
            if (_handler != null && GameStateManager.IsInExpedition)
            {
                _handler.StopCoroutine(coroutine);
            }
        }

        public static void StopAll()
        {
            if (_handler != null)
            {
                _handler.StopAllCoroutines();
            }
        }

        [InjectToIl2Cpp]
        private sealed class InLevelCoroutineHandler : MonoBehaviour
        {

        }
    }
}
