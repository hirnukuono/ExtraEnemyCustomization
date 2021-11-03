using EECustom.Networking.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EECustom.Networking
{
    public static class NetworkManager
    {
        public static ExplosionEvent Explosion { get; private set; } = new();

        internal static void Initialize()
        {
            Explosion.Setup();
        }
    }
}
