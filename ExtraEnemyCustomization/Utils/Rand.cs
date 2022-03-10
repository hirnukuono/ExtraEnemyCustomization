using System;

namespace EECustom.Utils
{
    public static class Rand
    {
        public const int InclusiveDoublePrecision = 10000;
        public const double InclusiveDoubleConversion = 1.0 / InclusiveDoublePrecision;

        private static readonly Random _rand = new();

        public static Random CreateInstance()
        {
            return new Random(_rand.Next());
        }

        public static T ItemOf<T>(T[] array)
        {
            return array[IndexOf(array)];
        }

        public static int IndexOf(Array array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            var length = array.Length;
            if (length == 0)
                throw new ArgumentException("Array Length was zero!", nameof(array));

            return _rand.Next(0, array.Length);
        }

        public static int Index(int length)
        {
            return _rand.Next(0, length);
        }

        public static int Range(int min, int max)
        {
            return _rand.Next(min, max);
        }

        public static float Range(float min, float max)
        {
            return NextFloat() * (max - min) + min;
        }

        public static int RangeInclusive(int min, int max)
        {
            return _rand.Next(min, max + 1);
        }

        public static float RangeInclusive(float min, float max)
        {
            return NextFloatInclusive() * (max - min) + min;
        }

        public static float NextFloatInclusive()
        {
            return Math.Clamp((float)NextDoubleInclusive(), 0.0f, 1.0f);
        }

        public static double NextDoubleInclusive()
        {
            int baseNumber = _rand.Next(0, InclusiveDoublePrecision + 1);
            return Math.Clamp(baseNumber * InclusiveDoubleConversion, 0.0, 1.0);
        }

        public static float NextFloat()
        {
            return (float)_rand.NextDouble();
        }

        public static bool CanDoBy(float chance01)
        {
            return NextFloat() <= chance01;
        }

        public static int NextInt()
        {
            return _rand.Next();
        }
    }
}