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
        private readonly Dictionary<float, T> _lookup = new(50);
        private T _temp;

        public T this[float time]
        {
            get
            {
                if (_lookup.TryGetValue(time, out _temp))
                {
                    return _temp;
                }
                _temp = CreateInstance(time);
                _lookup[time] = _temp;
                return _temp;
            }
        }

        protected abstract T CreateInstance(float time);
    }
}