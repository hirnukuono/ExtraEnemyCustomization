using System;

namespace EEC
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class CallConstructorOnLoadAttribute : Attribute
    {
    }
}