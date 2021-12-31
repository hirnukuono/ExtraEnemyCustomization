using System;
using System.Collections.Generic;
using System.Text;
using EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers;
using Gear;
using HarmonyLib;
using Player;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject
{
    internal class Inject_PlayerBackpack
    {
        public static void Setup()
        {
            foreach (var backpack in PlayerBackpackManager.Current.m_backpacks.Values)
            {
                AddHandlerForSlot(backpack, InventorySlot.GearStandard, new EMPGunSightHandler());
                AddHandlerForSlot(backpack, InventorySlot.GearSpecial, new EMPGunSightHandler());
            }
        }

        private static void AddHandlerForSlot(PlayerBackpack backpack, InventorySlot slot, IEMPHandler handler)
        {
            if (backpack.TryGetBackpackItem(slot, out BackpackItem item)) 
            {
                if (item.Instance.gameObject.GetComponent<EMPController>() != null) Logger.Debug("Item already has controller, skipping...");
                item.Instance.gameObject.AddComponent<EMPController>().AssignHandler(handler);
            } else
            {
                Logger.Warning("Couldn't get item for slot {0}!", slot);
            }
        }
    }
}
