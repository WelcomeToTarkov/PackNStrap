using EFT.InventoryLogic;
using HarmonyLib;
using PackNStrap.Core.Items;
using SPT.Reflection.Patching;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using PackNStrap.Helpers;

namespace BeltSlot.Patches
{
    // Create the submenu options (inventory screen)
    public class GetPrioritizedContainersPackNStrapPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GClass3372), nameof(GClass3372.GetPrioritizedContainersForLoot));
        }

        [PatchPrefix]
        public static bool Prefix(InventoryEquipment equipment, Item item, ref IEnumerable<EFT.InventoryLogic.IContainer> __result)
        {
            Slot slotTacticalVest = equipment.GetSlot(EquipmentSlot.TacticalVest);
            Slot slotBackpack = equipment.GetSlot(EquipmentSlot.Backpack);
            Slot slotPockets = equipment.GetSlot(EquipmentSlot.Pockets);
            Slot slotSecuredContainer = equipment.GetSlot(EquipmentSlot.SecuredContainer);
            Slot slotArmBand = equipment.GetSlot(EquipmentSlot.ArmBand);
            
            VestItemClass vestItemClass = slotTacticalVest.ContainedItem as VestItemClass;
            BackpackItemClass backpackItemClass = slotBackpack.ContainedItem as BackpackItemClass;
            PocketsItemClass pocketsItemClass = slotPockets.ContainedItem as PocketsItemClass;
            MobContainerItemClass mobContainerItemClass = slotSecuredContainer.ContainedItem as MobContainerItemClass;

            // Additional items for custom belt and tactical belt
            CustomBeltItemClass customBeltItemClass = slotArmBand.ContainedItem as CustomBeltItemClass;
            VestItemClass tacticalBeltItemClass = slotArmBand.ContainedItem as VestItemClass;

            // Tactical Rig Location
            IEnumerable<EFT.InventoryLogic.IContainer> enumerable;
            
            if (vestItemClass is not null)
            {
                if ((enumerable = vestItemClass.Containers) is not null)
                    goto IL_0064;
            }
            
            enumerable = Enumerable.Empty<EFT.InventoryLogic.IContainer>();
            
            IL_0064:
            IEnumerable<EFT.InventoryLogic.IContainer> enumerable2 = enumerable;

            // Backpack Location
            IEnumerable<EFT.InventoryLogic.IContainer> enumerable3;
            if (backpackItemClass is not null)
            {
                if ((enumerable3 = backpackItemClass.Containers) is not null)
                    goto IL_007B;
            }
            enumerable3 = Enumerable.Empty<EFT.InventoryLogic.IContainer>();
            IL_007B:
            IEnumerable<EFT.InventoryLogic.IContainer> enumerable4 = enumerable3;

            // Pockets Location
            IEnumerable<EFT.InventoryLogic.IContainer> enumerable5;
            if (pocketsItemClass != null)
            {
                if ((enumerable5 = pocketsItemClass.Containers) != null)
                {
                    goto IL_0094;
                }
            }
            enumerable5 = Enumerable.Empty<EFT.InventoryLogic.IContainer>();
            IL_0094:
            IEnumerable<EFT.InventoryLogic.IContainer> enumerable6 = enumerable5;

            // Secured Container Location
            IEnumerable<EFT.InventoryLogic.IContainer> enumerable7;
            if (mobContainerItemClass != null)
            {
                if ((enumerable7 = mobContainerItemClass.Containers) != null)
                {
                    goto IL_00AD;
                }
            }
            enumerable7 = Enumerable.Empty<EFT.InventoryLogic.IContainer>();
            IL_00AD:
            IEnumerable<EFT.InventoryLogic.IContainer> enumerable8 = enumerable7;

            // Additional items for custom belt and tactical belt
            // Custom belts from PackNStrap

            IEnumerable<EFT.InventoryLogic.IContainer> enumerable9;
            if(customBeltItemClass != null)
            {
                 if ((enumerable9 = customBeltItemClass.Containers) != null)
                 {
                     goto IL_00C6;
                 }
            }
            enumerable9 = Enumerable.Empty<EFT.InventoryLogic.IContainer>();
            IL_00C6:
            IEnumerable<EFT.InventoryLogic.IContainer> enumerable10 = enumerable9;

            // Tactical belts from Tactical Item Component
            IEnumerable<EFT.InventoryLogic.IContainer> enumerable11;
            if (tacticalBeltItemClass != null)
            {
                if ((enumerable11 = tacticalBeltItemClass.Containers) != null)
                {
                    goto IL_00DF;
                }
            }
            enumerable11 = Enumerable.Empty<EFT.InventoryLogic.IContainer>();
            IL_00DF:
            IEnumerable<EFT.InventoryLogic.IContainer> enumerable12 = enumerable11;
            // Belt slot containers come after the vest in looting priority
            if (item is MagazineItemClass)
            {
                // enumerable2 is chest rig, enumerable4 is backpack, enumerable6 is pockets,
                // enumerable8 is secured container, enumerable10 is custom belt, enumerable12 is tactical belt
                __result = enumerable2.Concat(enumerable10).Concat(enumerable12).Concat(enumerable6).Concat(enumerable4).Concat(enumerable8);
                return false;
            }
            if(item is AmmoItemClass)
            {
                __result = enumerable10.Concat(enumerable12).Concat(enumerable2).Concat(enumerable6).Concat(enumerable4).Concat(enumerable8);
                return false;
            }
            if (item is MoneyItemClass)
            {
                __result = enumerable8.Concat(enumerable4).Concat(enumerable2).Concat(enumerable10).Concat(enumerable12).Concat(enumerable6);
                return false;
            }
            if (item is ThrowWeapItemClass)
            {
                __result = enumerable6.Concat(enumerable10).Concat(enumerable12).Concat(enumerable2).Concat(enumerable4).Concat(enumerable8);
                return false;
            }
            __result = enumerable4.Concat(enumerable2).Concat(enumerable10).Concat(enumerable12).Concat(enumerable6).Concat(enumerable8);
            return false;
        }
    }
}