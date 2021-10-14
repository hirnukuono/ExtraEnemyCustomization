using Gear;
using Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Events
{
    public class ResourcePackEvents
    {
        public static Action<iResourcePackReceiver> OnReceiveMedi = null;
        public static Action<iResourcePackReceiver> OnReceiveAmmo = null;
        public static Action<iResourcePackReceiver> OnReceiveTool = null;
        public static Action<iResourcePackReceiver> OnReceiveDisinfect = null;
    }
}
