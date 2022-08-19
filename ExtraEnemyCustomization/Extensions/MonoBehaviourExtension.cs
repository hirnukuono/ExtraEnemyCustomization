using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Collections;
using UnityEngine;

namespace EEC
{
    public static class MonoBehaviourExtension
    {
        public static Coroutine StartCoroutine(this MonoBehaviour self, IEnumerator routine)
        {
            return self.StartCoroutine(routine.WrapToIl2Cpp());
        }
    }
}