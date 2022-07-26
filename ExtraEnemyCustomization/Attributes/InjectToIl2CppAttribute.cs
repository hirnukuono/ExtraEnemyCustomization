using System;

namespace EEC
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    internal sealed class InjectToIl2CppAttribute : Attribute
    {
    }
}