using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace EEC.Utils.Json.Elements
{
    [JsonConverter(typeof(BoolBaseConverter))]
    public struct BoolBase
    {
        public static readonly BoolBase False = new(BoolMode.False);
        public static readonly BoolBase True = new(BoolMode.True);
        public static readonly BoolBase Unchanged = new(BoolMode.Unchanged);

        public BoolMode Mode;

        public BoolBase(bool mode)
        {
            Mode = mode ? BoolMode.True : BoolMode.False;
        }

        public BoolBase(BoolMode mode)
        {
            Mode = mode;
        }

        public bool GetValue(bool originalValue)
        {
            switch (Mode)
            {
                case BoolMode.Unchanged:
                    return originalValue;
                case BoolMode.True:
                    return true;
                case BoolMode.False:
                    return false;
                default:
                    Logger.Error($"BoolBase.GetValue; Got Unknown Mode: {Mode}!\n{Environment.StackTrace}");
                    return originalValue;
            }
        }
    }

    public enum BoolMode
    {
        False,
        True,
        Unchanged
    }
}
