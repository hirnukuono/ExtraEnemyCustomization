using EECustom.Attributes;
using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace EECustom.Utils
{
    [InjectToIl2Cpp]
    public sealed class ThreadDispatcher : MonoBehaviour
    {
        public static ThreadDispatcher Current { get; private set; } = null;

        private static readonly ConcurrentQueue<Action> _queue = new();
        private static readonly ConcurrentQueue<Action> _heavyQueue = new();

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
            if (action == null)
                return;

            _queue.Enqueue(action);
        }

        public static void EnqueueHeavy(Action action)
        {
            if (action == null)
                return;

            _heavyQueue.Enqueue(action);
        }

        private void Update()
        {
            Action action;
            while (_queue.TryDequeue(out action))
            {
                action.Invoke();
            }

            if (_heavyQueue.TryDequeue(out action))
            {
                action.Invoke();
            }
        }
    }
}