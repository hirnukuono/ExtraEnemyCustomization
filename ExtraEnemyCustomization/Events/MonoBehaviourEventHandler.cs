using EECustom.Attributes;
using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.Events
{
    public delegate void UnityEventHandler(GameObject caller);

    [InjectToIl2Cpp]
    public class MonoBehaviourEventHandler : MonoBehaviour
    {
        [HideFromIl2Cpp]
        public event UnityEventHandler OnUpdate;
        [HideFromIl2Cpp]
        public event UnityEventHandler OnFixedUpdate;
        [HideFromIl2Cpp]
        public event UnityEventHandler OnDestroyed;
        [HideFromIl2Cpp]
        public event UnityEventHandler OnLateUpdate;

        public MonoBehaviourEventHandler(IntPtr ptr) : base(ptr)
        {
        }

        public static void AttatchToObject(GameObject obj, UnityEventHandler onUpdate = null, UnityEventHandler onLateUpdate = null, UnityEventHandler onFixedUpdate = null, UnityEventHandler onDestroyed = null)
        {
            var handler = obj.AddComponent<MonoBehaviourEventHandler>();
            if (onUpdate != null)
                handler.OnUpdate += onUpdate;

            if (onLateUpdate != null)
                handler.OnLateUpdate += onLateUpdate;

            if (onFixedUpdate != null)
                handler.OnFixedUpdate += onFixedUpdate;

            if (onDestroyed != null)
                handler.OnDestroyed += onDestroyed;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void Update()
        {
            OnUpdate?.Invoke(gameObject);
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke(gameObject);
        }


        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke(gameObject);
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke(gameObject);
        }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
