using EECustom.Attributes;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.Events
{
    public delegate void UnityEventHandler(GameObject caller);

    [InjectToIl2Cpp]
    public sealed class MonoBehaviourEventHandler : MonoBehaviour
    {
        [HideFromIl2Cpp]
        public event UnityEventHandler OnUpdate;

        [HideFromIl2Cpp]
        public event UnityEventHandler OnFixedUpdate;

        [HideFromIl2Cpp]
        public event UnityEventHandler OnDestroyed;

        [HideFromIl2Cpp]
        public event UnityEventHandler OnLateUpdate;

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

        internal void Update()
        {
            OnUpdate?.Invoke(gameObject);
        }

        internal void LateUpdate()
        {
            OnLateUpdate?.Invoke(gameObject);
        }

        internal void FixedUpdate()
        {
            OnFixedUpdate?.Invoke(gameObject);
        }

        internal void OnDestroy()
        {
            OnDestroyed?.Invoke(gameObject);

            OnUpdate = null;
            OnLateUpdate = null;
            OnFixedUpdate = null;
        }
    }
}