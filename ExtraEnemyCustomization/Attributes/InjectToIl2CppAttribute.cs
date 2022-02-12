using System;

namespace EECustom.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    internal sealed class InjectToIl2CppAttribute : Attribute
    {
    }
}