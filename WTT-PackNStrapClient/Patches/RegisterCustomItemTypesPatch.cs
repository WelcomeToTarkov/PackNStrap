using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using PackNStrap.Core.Items;

namespace PackNStrap.Patches
{
    internal class RegisterCustomItemTypesPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GClass3381), nameof(GClass3381.Init));
        }

        [PatchPostfix]
        public static void PatchPostfix()
        {
            RegisterCustomTypes();
        }

        private static void RegisterCustomTypes()
        {
            if (!GClass3381.List_0.Contains(typeof(CustomContainerItemClass)))
            {
                GClass3381.List_0.Add(typeof(CustomContainerItemClass));
            }

            if (!GClass3381.List_0.Contains(typeof(CustomSecureContainerClass)))
            {
                GClass3381.List_0.Add(typeof(CustomSecureContainerClass));
            }

            if (!GClass3381.List_0.Contains(typeof(CustomBeltItemClass)))
            {
                GClass3381.List_0.Add(typeof(CustomBeltItemClass));
            }
        }
    }
}