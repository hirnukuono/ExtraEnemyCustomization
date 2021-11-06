using Gear;

namespace EECustom.Events
{
    public delegate void ReceivePackHandler(iResourcePackReceiver receiver);
    public delegate void ReceiveMediHandler(iResourcePackReceiver receiver, float health);
    public delegate void ReceiveAmmoHandler(iResourcePackReceiver receiver, float main, float special);
    public delegate void ReceiveToolHandler(iResourcePackReceiver receiver, float tool);
    public delegate void ReceiveDisinfectHandler(iResourcePackReceiver receiver, float disinfect);

    public static class ResourcePackEvents
    {
        public static event ReceiveMediHandler ReceiveMedi;
        public static event ReceiveAmmoHandler ReceiveAmmo;
        public static event ReceiveToolHandler ReceiveTool;
        public static event ReceiveDisinfectHandler ReceiveDisinfect;

        internal static void OnReceiveMedi(iResourcePackReceiver receiver, float health)
        {
            ReceiveMedi?.Invoke(receiver, health);
        }

        internal static void OnReceiveAmmo(iResourcePackReceiver receiver, float main_rel, float special_rel)
        {
            ReceiveAmmo?.Invoke(receiver, main_rel, special_rel);
        }

        internal static void OnReceiveTool(iResourcePackReceiver receiver, float tool_rel)
        {
            ReceiveTool?.Invoke(receiver, tool_rel);
        }

        internal static void OnReceiveDisinfect(iResourcePackReceiver receiver, float disinfect)
        {
            ReceiveDisinfect?.Invoke(receiver, disinfect);
        }
    }
}