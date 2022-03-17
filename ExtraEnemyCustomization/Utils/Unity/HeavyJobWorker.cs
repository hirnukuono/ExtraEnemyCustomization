using EEC.Attributes;
using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace EEC.Utils.Unity
{
    [InjectToIl2Cpp]
    public sealed class HeavyJobWorker : MonoBehaviour
    {
        public static HeavyJobWorker Current { get; private set; } = null;

        private static readonly ConcurrentQueue<Action> _queue = new();

        internal static void Initialize()
        {
            if (Current == null)
            {
                var dispatcher = new GameObject();
                DontDestroyOnLoad(dispatcher);

                Current = dispatcher.AddComponent<HeavyJobWorker>();
            }
        }

        public static void Enqueue(Action action)
        {
            if (action == null)
                return;

            _queue.Enqueue(action);
        }

        private Action _tempAction;
        private void Update()
        {
            if (_queue.TryDequeue(out _tempAction))
            {
                _tempAction?.Invoke();
            }
        }
    }
}