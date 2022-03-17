using Gear;

namespace EEC.Events
{
    public delegate void InventoryEventHandler(GearPartFlashlight flashlight);

    public static class InventoryEvents
    {
        public static event InventoryEventHandler ItemWielded;

        internal static void OnWieldItem(GearPartFlashlight flashlight)
        {
            ItemWielded?.Invoke(flashlight);
        }
    }
}