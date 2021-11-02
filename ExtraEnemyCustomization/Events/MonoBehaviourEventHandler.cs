using EECustom.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
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

        public MonoBehaviourEventHandler(IntPtr ptr) : base(ptr)
        {
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void Update()
        {
            OnUpdate?.Invoke(gameObject);
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
