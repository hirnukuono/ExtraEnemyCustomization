using EECustom.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EECustom.Utils
{
    [InjectToIl2Cpp]
    public class ThreadDispatcher : MonoBehaviour
    {
        public static ThreadDispatcher Current { get; private set; } = null;

        private static readonly Queue<Action> _actionQueue = new();

        internal static void Initialize()
        {
            if (Current == null)
            {
                var dispatcher = new GameObject();
                DontDestroyOnLoad(dispatcher);

                Current = dispatcher.AddComponent<ThreadDispatcher>();
            }
        }

        public static void Enqueue(Action action)
        {
            lock (_actionQueue)
            {
                _actionQueue.Enqueue(action);
            }
        }

        internal void Update()
        {
            lock (_actionQueue)
            {
                while (_actionQueue.Count > 0)
                {
                    _actionQueue.Dequeue()?.Invoke();
                }
            }
        }
    }
}