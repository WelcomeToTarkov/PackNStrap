﻿using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PackNStrap.Patches
{
    internal class ContainerSlotsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.PropertyGetter(typeof(InventoryEquipment), "ContainerSlots");
        }

        [PatchPostfix]
        public static void PatchPostfix(InventoryEquipment __instance, ref IReadOnlyList<Slot> __result)
        {
            List<Slot> newResult = __result.ToList();
            newResult.Add(__instance.GetSlot(EquipmentSlot.ArmBand));
            __result = newResult;
        }
    }

    internal class PaymentSlotsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.PropertyGetter(typeof(InventoryEquipment), "PaymentSlots");
        }

        [PatchPostfix]
        public static void PatchPostfix(InventoryEquipment __instance, ref IReadOnlyList<Slot> __result)
        {
            List<Slot> newResult = __result.ToList();
            newResult.Add(__instance.GetSlot(EquipmentSlot.ArmBand));
            __result = newResult;
        }
    }
}