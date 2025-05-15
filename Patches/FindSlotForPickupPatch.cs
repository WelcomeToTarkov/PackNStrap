using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Collections.Generic;
using System.Reflection;
using PackNStrap.Core.Items;

namespace CustomBelt.Patches
{
    internal class FindSlotForPickupPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(
                typeof(GClass3169), 
                nameof(GClass3169.FindSlotToPickUp),
                new[] { typeof(InventoryEquipment), typeof(Item) }
            );
        }

        [PatchPostfix]
        public static void Postfix(
            ref ItemAddress __result,
            GClass3169 __instance,    // GClass3169 container
            InventoryEquipment equipment,
            Item item)
        {
            if (__result != null || !(item is CustomBeltItemClass))
            {
                return;
            }

            foreach (var slot in GClass3169.equipmentSlot_8) // ArmBand slots
            {
                var equipmentSlot = equipment.GetSlot(slot);
                if (equipmentSlot.Deleted || !equipmentSlot.CheckCompatibility(item))
                {
                    continue;
                }

                var address = equipmentSlot.FindLocationForItem(item, out _);
                if (address != null)
                {
                    __result = address;
                    return;
                }
            }
        }
    }
}