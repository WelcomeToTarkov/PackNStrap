

using System;
using System.Collections.Generic;
using EFT;
using EFT.InventoryLogic;
using PackNStrap.Core.Items;

namespace PackNStrap.Helpers;

public abstract class Common
{
    public static List<CustomContainerItemClass> GetMagDumpPouches(InventoryEquipment equipment, bool backpackIncluded)
    {
        if (equipment == null)
        {
            Console.WriteLine("Equipment is null.");
            return null;
        }

        List<CustomContainerItemClass> magDumpPouches = new List<CustomContainerItemClass>();
        var magDumpPouchItemId = "440de5d056825485a0cf3a19";

        // Function to search first-level items for the pouch
        void FindMagDumpPouchInItem(Item item)
        {
            if (item == null) return;

            foreach (var itemInGrid in item.GetAllItems())
            {
                if (itemInGrid is CustomContainerItemClass potentialMagDumpPouch 
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
        FindMagDumpPouchInItem(armbandSlot?.ContainedItem as CustomBeltItemClass);

        // Cast magDumpPouches to CompoundItem and return
        return magDumpPouches;
    }

    public static bool CanAcceptItems(StashGridClass grid)
    {
        Player player = PackNStrap.Player;
        // Example condition, replace with actual logic as needed
        if (player != null && player.HandsController != null && player.HandsController?.Item != null && player.HandsController?.Item?.GetCurrentMagazine() != null)
        {
            return grid.CanAccept(player.HandsController?.Item?.GetCurrentMagazine()); // Assuming `CanAcceptItems` is a property or method on `StashGridClass`
        }
        return false;
    }
}