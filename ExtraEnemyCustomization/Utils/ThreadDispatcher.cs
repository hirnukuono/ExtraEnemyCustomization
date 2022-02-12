using EECustom.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EECustom.Utils
{
    [InjectToIl2Cpp]
    public sealed class ThreadDispatcher : MonoBehaviour
    {
        public static ThreadDispatcher Current { get; private set; } = null;

        private static readonly Queue<Action> _immediateQueue = new();
        private static readonly Queue<Action> _lightQueue = new();
        private static readonly Queue<Action> _mediumQueue = new();
        private static readonly Queue<Action> _heavyQueue = new();

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
                    lock (_immediateQueue)
                    {
                        _immediateQueue.Enqueue(action);
                    }
                    break;

                case JobComplexity.Light:
                    lock (_lightQueue)
                    {
                        _lightQueue.Enqueue(action);
                    }
                    break;

                case JobComplexity.Medium:
                    lock (_mediumQueue)
                    {
                        _mediumQueue.Enqueue(action);
                    }
                    break;

                case JobComplexity.Heavy:
                    lock (_heavyQueue)
                    {
                        _heavyQueue.Enqueue(action);
                    }
                    break;
            }
            
        }

#pragma warning disable CA1822 // Mark members as static
        internal void FixedUpdate()
        {
            lock (_immediateQueue)
            {
                while (_immediateQueue.Count > 0)
                {
                    _immediateQueue.Dequeue()?.Invoke();
                }
            }

            lock (_lightQueue)
            {
                int counter = 0;
                while (_lightQueue.Count > 0 && counter < 10)
                {
                    _lightQueue.Dequeue()?.Invoke();
                    counter++;
                }
            }

            lock (_mediumQueue)
            {
                int counter = 0;
                while (_mediumQueue.Count > 0 && counter < 5)
                {
                    _mediumQueue.Dequeue()?.Invoke();
                    counter++;
                }
            }

            lock (_heavyQueue)
            {
                int counter = 0;
                while (_heavyQueue.Count > 0 && counter < 2)
                {
                    _heavyQueue.Dequeue()?.Invoke();
                    counter++;
                }
            }
        }
#pragma warning restore CA1822 // Mark members as static
    }

    public enum JobComplexity
    {
        None,
        Light,
        Medium,
        Heavy
    }
}