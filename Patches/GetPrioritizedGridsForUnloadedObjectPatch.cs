﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace PackNStrap.Patches;

internal class GetPrioritizedGridsForUnloadedObjectPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(GClass3097), nameof(GClass3097.GetPrioritizedGridsForUnloadedObject));
    }

    [PatchPrefix]
    public static bool PatchPrefix(ref InventoryEquipment equipment, bool backpackIncluded, ref IEnumerable<StashGridClass> __result)
    {
        // Retrieve slots
        Slot tacticalVestSlot = equipment.GetSlot(EquipmentSlot.TacticalVest);
        Slot pocketsSlot = equipment.GetSlot(EquipmentSlot.Pockets);
        Slot backpackSlot = equipment.GetSlot(EquipmentSlot.Backpack);
        Slot armbandSlot = equipment.GetSlot(EquipmentSlot.ArmBand);

        // Handle contained items
        VestItemClass tacticalVestItem = tacticalVestSlot?.ContainedItem as VestItemClass;
        PocketsItemClass pocketsItem = pocketsSlot?.ContainedItem as PocketsItemClass;
        BackpackItemClass backpackItem = backpackSlot?.ContainedItem as BackpackItemClass;
        VestItemClass armbandItem = armbandSlot?.ContainedItem as VestItemClass;

        // Retrieve grids or empty arrays if items are null
        StashGridClass[] tacticalVestGrids = tacticalVestItem?.Grids ?? Array.Empty<StashGridClass>();
        StashGridClass[] pocketsGrids = pocketsItem?.Grids ?? Array.Empty<StashGridClass>();
        StashGridClass[] backpackGrids = backpackItem?.Grids ?? Array.Empty<StashGridClass>();
        StashGridClass[] armbandGrids = armbandItem?.Grids ?? Array.Empty<StashGridClass>();

        // Find all instances of magDumpPouch
        List<SimpleContainerItemClass> magDumpPouches = Helpers.Helpers.GetMagDumpPouches(equipment, backpackIncluded);

        // Retrieve grids for all found magDumpPouches that can accept items
        List<StashGridClass> magDumpPouchGrids = magDumpPouches
            .SelectMany(pouch => pouch.Grids ?? Array.Empty<StashGridClass>())
            .Where(CanAcceptItems) // Check if grid can accept items
            .ToList();

        if (magDumpPouchGrids.Count > 0)
        {
            Console.WriteLine("Returning only MagDumpPouch grids that can accept items.");
            __result = magDumpPouchGrids; // Return only MagDumpPouch grids if valid
            return false; // Skip the original method
        }
        else
        {
            Console.WriteLine("No valid MagDumpPouch grids found.");
        }

        // Fall back to returning other grids if no valid MagDumpPouch grids
        __result = backpackIncluded
            ? tacticalVestGrids.Concat(pocketsGrids).Concat(backpackGrids).Concat(armbandGrids)
            : tacticalVestGrids.Concat(pocketsGrids).Concat(armbandGrids);

        return false; // Skip the original method
    }

    // Method to determine if a grid can accept items
    private static bool CanAcceptItems(StashGridClass grid)
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