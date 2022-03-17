using EEC.CustomAbilities.EMP.Handlers;
using Gear;
using Player;

namespace EEC.CustomAbilities.EMP.Inject
{
    internal class Inject_PlayerBackpack
    {
        public static void Setup()
        {
            foreach (var backpack in PlayerBackpackManager.Current.m_backpacks.Values)
            {
                AddHandlerForSlot(backpack, InventorySlot.GearStandard, new EMPGunSightHandler());
                AddHandlerForSlot(backpack, InventorySlot.GearSpecial, new EMPGunSightHandler());
                AddToolHandler(backpack);
            }
        }

        private static void AddToolHandler(PlayerBackpack backpack)
        {
            if (backpack.TryGetBackpackItem(InventorySlot.GearClass, out BackpackItem item))
            {
                if (item.Instance.gameObject.GetComponent<EMPController>() != null) Logger.Debug("Item already has controller, skipping...");
                if (item.Instance.GetComponent<EnemyScanner>() != null) item.Instance.gameObject.AddComponent<EMPController>().AssignHandler(new EMPBioTrackerHandler());
            }
            else
            {
                Logger.Warning("Couldn't get item for slot {0}!", InventorySlot.GearClass);
            }
        }

        private static void AddHandlerForSlot(PlayerBackpack backpack, InventorySlot slot, IEMPHandler handler)
        {
            if (backpack.TryGetBackpackItem(slot, out BackpackItem item))
            {
                if (item.Instance.gameObject.GetComponent<EMPController>() != null) Logger.Debug("Item already has controller, skipping...");
                item.Instance.gameObject.AddComponent<EMPController>().AssignHandler(handler);
            }
            else
            {
                Logger.Warning("Couldn't get item for slot {0}!", slot);
            }
        }
    }
}