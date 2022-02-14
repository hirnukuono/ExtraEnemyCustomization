using EECustom.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace EECustom.Utils
{
    [InjectToIl2Cpp]
    public sealed class ThreadDispatcher : MonoBehaviour
    {
        public static ThreadDispatcher Current { get; private set; } = null;

        private static readonly ConcurrentQueue<Action> _immediateQueue = new();
        private static readonly ConcurrentQueue<Action> _lightQueue = new();
        private static readonly ConcurrentQueue<Action> _mediumQueue = new();
        private static readonly ConcurrentQueue<Action> _heavyQueue = new();
        private int _counter = 0;

        internal static void Initialize()
        {
            if (Current == null)
            {
                var dispatcher = new GameObject();
                DontDestroyOnLoad(dispatcher);

                Current = dispatcher.AddComponent<ThreadDispatcher>();
            }
        }

        public static void Enqueue(JobComplexity complexity, Action action)
        {
            switch (complexity)
            {
                case JobComplexity.None:
                    _immediateQueue.Enqueue(action);
                    break;

                case JobComplexity.Light:
                    _lightQueue.Enqueue(action);
                    break;

                case JobComplexity.Medium:
                    _mediumQueue.Enqueue(action);
                    break;

                case JobComplexity.Heavy:
                    _heavyQueue.Enqueue(action);
                    break;
            }
            
        }

        internal void FixedUpdate()
        {
            Action action;

            while (_immediateQueue.TryDequeue(out action))
            {
                action?.Invoke();
            }

            _counter = 0;
            while (_lightQueue.TryDequeue(out action) && _counter < 10)
            {
                action?.Invoke();
                _counter++;
            }

            _counter = 0;
            while (_mediumQueue.TryDequeue(out action) && _counter < 5)
            {
                action?.Invoke();
                _counter++;
            }

            _counter = 0;
            while (_heavyQueue.TryDequeue(out action) && _counter < 2)
            {
                action?.Invoke();
                _counter++;
            }
        }
    }

    public enum JobComplexity
    {
        None,
        Light,
        Medium,
        Heavy
    }
}