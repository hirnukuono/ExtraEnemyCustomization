using EEC.Attributes;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace EEC.Events
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
            var handler = obj.AddOrGetComponent<MonoBehaviourEventHandler>();

            if (onUpdate != null)
                handler.OnUpdate += onUpdate;

            if (onLateUpdate != null)
                handler.OnLateUpdate += onLateUpdate;

            if (onFixedUpdate != null)
                handler.OnFixedUpdate += onFixedUpdate;

            if (onDestroyed != null)
                handler.OnDestroyed += onDestroyed;
        }

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

            OnUpdate = null;
            OnLateUpdate = null;
            OnFixedUpdate = null;
            OnDestroyed = null;
        }
    }
}