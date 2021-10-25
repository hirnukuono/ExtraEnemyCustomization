using Gear;
using System;

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