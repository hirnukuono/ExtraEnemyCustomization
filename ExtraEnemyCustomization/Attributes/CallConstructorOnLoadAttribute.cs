using System;

namespace EEC.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class CallConstructorOnLoadAttribute : Attribute
    {
    }
}