using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PackNStrap.Patches
{
    internal class MergeContainerWithChildrenPatch : ModulePatch
    {

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.PropertyGetter(typeof(Slot), nameof(Slot.MergeContainerWithChildren));
        }

        [PatchPostfix]

        public static void PatchPostfix(Slot __instance, ref EParentMergeType __result)
        {
            if (__instance?.ID == "ArmBand")
            {
                __result = EParentMergeType.InheritFromItem;
            }
        }
    }
}