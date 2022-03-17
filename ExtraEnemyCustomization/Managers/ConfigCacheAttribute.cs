using System;

namespace EECustom.Managers
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class ConfigCacheAttribute : Attribute
    {
    }
}