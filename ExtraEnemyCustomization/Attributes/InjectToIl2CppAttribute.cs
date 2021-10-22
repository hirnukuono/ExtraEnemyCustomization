using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class InjectToIl2CppAttribute : Attribute
    {
    }
}
