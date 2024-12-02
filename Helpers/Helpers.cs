

using System;
using System.Collections.Generic;
using System.Linq;
using EFT.InventoryLogic;

namespace PackNStrap.Helpers;

public abstract class Helpers
{
    public static List<SimpleContainerItemClass> GetMagDumpPouches(InventoryEquipment equipment, bool backpackIncluded)
    {
        if (equipment == null)
        {
            Console.WriteLine("Equipment is null.");
            return null;
        }

        List<SimpleContainerItemClass> magDumpPouches = new List<SimpleContainerItemClass>();
        var magDumpPouchItemId = "440de5d056825485a0cf3a19";

        // Function to search first-level items for the pouch
        void FindMagDumpPouchInItem(Item item)
        {
            if (item == null) return;

            foreach (var itemInGrid in item.GetAllItems())
            {
                if (itemInGrid is SimpleContainerItemClass potentialMagDumpPouch 
                    && potentialMagDumpPouch.TemplateId == magDumpPouchItemId)
                {
                    if (potentialMagDumpPouch.IsChildOf(item))
                        magDumpPouches.Add(potentialMagDumpPouch);
                }
            }
        }

        // Retrieve slots
        Slot tacticalVestSlot = equipment.GetSlot(EquipmentSlot.TacticalVest);
        Slot pocketsSlot = equipment.GetSlot(EquipmentSlot.Pockets);
        Slot backpackSlot = equipment.GetSlot(EquipmentSlot.Backpack);
        Slot armbandSlot = equipment.GetSlot(EquipmentSlot.ArmBand);

        // Check each slot for MagDumpPouches
        FindMagDumpPouchInItem(tacticalVestSlot?.ContainedItem as VestItemClass);
        FindMagDumpPouchInItem(pocketsSlot?.ContainedItem as PocketsItemClass);
        if (backpackIncluded)
            FindMagDumpPouchInItem(backpackSlot?.ContainedItem as BackpackItemClass);
        FindMagDumpPouchInItem(armbandSlot?.ContainedItem as VestItemClass);

        // Cast magDumpPouches to CompoundItem and return
        return magDumpPouches;
    }


}