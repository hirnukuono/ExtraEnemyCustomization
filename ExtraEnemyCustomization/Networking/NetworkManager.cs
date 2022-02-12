using EECustom.Networking.Events;
using System.Collections.Generic;
using System.Linq;
using System;

namespace EECustom.Networking
{
    public static class NetworkManager
    {
        public static EnemyAnimEvent EnemyAnim { get; private set; } = new();

        internal static void Initialize()
        {
            EnemyAnim.Setup();
        }
    }
}