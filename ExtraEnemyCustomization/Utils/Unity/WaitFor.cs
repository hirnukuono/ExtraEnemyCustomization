using System;
using System.Collections.Generic;
using UnityEngine;

namespace EEC.Utils.Unity
{
    public static class WaitFor
    {
        public static readonly WaitForEndOfFrame EndOfFrame = new();
        public static readonly WaitForFixedUpdate FixedUpdate = new();
        public static readonly WaitForSecondsCollection Seconds = new();
        public static readonly WaitForSecondsRealtimeCollection SecondsRealtime = new();
    }

    public sealed class WaitForSecondsCollection : WaitForCollection<WaitForSeconds>
    {
        protected override WaitForSeconds CreateInstance(float time)
        {
            return new(time);
        }
    }

    public sealed class WaitForSecondsRealtimeCollection : WaitForCollection<WaitForSecondsRealtime>
    {
        protected override WaitForSecondsRealtime CreateInstance(float time)
        {
            return new(time);
        }
    }

    public abstract class WaitForCollection<T>
    {
        private readonly Dictionary<int, T> _lookup = new(100);
        private T _temp;

        public T this[float time]
        {
            get
            {
                int ms;
                if (time <= 0.0f)
                {
                    ms = 0; 
                }
                else
                {
                    ms = Mathf.RoundToInt(time * 1000.0f);
                }

                if (_lookup.TryGetValue(ms, out _temp))
                {
                    return _temp;
                }
                _temp = CreateInstance(time);
                _lookup[ms] = _temp;
                return _temp;
            }
        }

        protected abstract T CreateInstance(float time);
    }
}