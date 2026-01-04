using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace BeltSlot.Patches
{
    public class EquipmentTabPatch : ModulePatch
    {
        private static FieldInfo? slotViews = AccessTools.Field(typeof(EquipmentTab), "_slotViews");
        private static FieldInfo? armbandSlot = AccessTools.Field(typeof(EquipmentTab), "_armbandSlot");

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(EquipmentTab), nameof(EquipmentTab.Awake));
        }

        [PatchPostfix]
        static void PostFix(EquipmentTab __instance)
        {

        }
    }
}
