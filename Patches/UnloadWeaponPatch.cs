using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EFT.Communications;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using PackNStrap.Helpers;

namespace PackNStrap.Patches;

internal class UnloadWeaponPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ItemUiContext), nameof(ItemUiContext.UnloadWeapon));
    }

    [PatchPrefix]
    public static bool UnloadWeaponPrefix(ItemUiContext __instance, ref Weapon weapon, ref Task __result)
    {
        if (!GClass2064.InRaid)
        {
            return true;
        }
        Console.WriteLine($"Starting CustomUnloadWeapon for weapon: {weapon.TemplateId}");
        try
        {
            // Set the result to the Task returned by CustomUnloadWeapon
            __result = CustomUnloadWeapon(__instance, weapon);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return true;
        }
    }

    private static async Task CustomUnloadWeapon(ItemUiContext __instance, Weapon weapon)
    {
#if DEBUG
        Console.WriteLine("Entered CustomUnloadWeapon method.");
#endif
        InventoryController inventoryControllerClass = (InventoryController)AccessTools.Field(typeof(ItemUiContext), "inventoryController_0").GetValue(__instance);
        if (inventoryControllerClass == null)
        {
            Console.WriteLine("InventoryController is null.");
            return;
        }

        CompoundItem[] rightPanelItems = __instance.CompoundItem_0;

        if (!weapon.IsUnderBarrelDeviceActive)
        {
            MagazineItemClass currentMagazine = weapon.GetCurrentMagazine();
            if (currentMagazine == null)
            {
                Console.WriteLine("Current magazine is null.");
                return;
            }

            if (!__instance.method_4(weapon))
            {
                InventoryEquipment equipment = inventoryControllerClass.Inventory.Equipment;
                bool isInEquipment = equipment.Contains(currentMagazine);
#if DEBUG
                Console.WriteLine($"Current magazine is {(isInEquipment ? "in" : "not in")} equipment.");
#endif
                if (!isInEquipment && rightPanelItems == null)
                {
#if DEBUG

                    Console.WriteLine("Something went wrong. Right panel is null while mag is not from equipment.");
#endif
                }
                else
                {
                    IEnumerable<CompoundItem> enumerable;
                    if (rightPanelItems != null)
                    {
                        enumerable = (isInEquipment ? equipment.ToEnumerable<InventoryEquipment>().Concat(rightPanelItems) : rightPanelItems.Concat(equipment.ToEnumerable<InventoryEquipment>()));
                    }
                    else
                    {
                        enumerable = equipment.ToEnumerable<InventoryEquipment>();
                    }


                    IEnumerable<CompoundItem> targets = enumerable;


                    // Search for MagDumpPouch items and retrieve their grids
                    List<SimpleContainerItemClass> magDumpPouches = Helpers.Helpers.GetMagDumpPouches(equipment, false);

                    // Only add MagDumpPouches to enumerable if any were found
                    if (magDumpPouches.Any())
                    {
#if DEBUG
                        Console.WriteLine("using mag dump pouches.");
#endif
                        targets = magDumpPouches;
                    }


                    GStruct446<GInterface385> value = InteractionsHandlerClass.QuickFindAppropriatePlace(currentMagazine, inventoryControllerClass, targets, InteractionsHandlerClass.EMoveItemOrder.ForcePush, true);

                    if (value.Succeeded)
                    {
                        if (!(await ItemUiContext.smethod_0(inventoryControllerClass, currentMagazine, value, null)).Succeed)
                        {
                            HandleUnloadFailure(inventoryControllerClass, currentMagazine);
                        }
                    }
                    else
                    {
                        HandleUnloadFailure(inventoryControllerClass, currentMagazine);
                    }
                }
            }
        }
        else
        {
#if DEBUG
            Console.WriteLine("Weapon has an active under-barrel device; unload operation skipped.");
#endif
        }

        Console.WriteLine("Exiting CustomUnloadWeapon method.");
    }

    private static void HandleUnloadFailure(InventoryController inventoryControllerClass, MagazineItemClass currentMagazine)
    {
        if (!GClass2064.InRaid)
        {
            NotificationManagerClass.DisplayWarningNotification("Can't find a place for item".Localized());
        }
        else if (inventoryControllerClass.CanThrow(currentMagazine))
        {
            inventoryControllerClass.ThrowItem(currentMagazine, true);
        }

    }
}