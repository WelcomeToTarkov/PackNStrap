/* eslint-disable @typescript-eslint/naming-convention */

import { ConfigTypes } from "@spt/models/enums/ConfigTypes";
import { DependencyContainer } from "tsyringe";
import { ILostOnDeathConfig } from "@spt/models/spt/config/ILostOnDeathConfig";
import { IPostDBLoadMod } from "@spt/models/external/IPostDBLoadMod";
import { IPreSptLoadMod } from "@spt/models/external/IPreSptLoadMod";
import { LogTextColor } from "@spt/models/spt/logging/LogTextColor";
import type { GameController } from "@spt/controllers/GameController";
import type { IEmptyRequestData } from "@spt/models/eft/common/IEmptyRequestData";
import { InRaidHelper } from "@spt/helpers/InRaidHelper";
import { IPmcData } from "@spt/models/eft/common/IPmcData";
import { Item } from "@spt/models/eft/common/tables/IItem";
import * as config from "../config/config.json";

// WTT imports
import { WTTInstanceManager } from "./WTTInstanceManager";

// Boss imports
import { CustomItemService } from "./CustomItemService";


class PackNStrap
    implements IPreSptLoadMod, IPostDBLoadMod {
    private Instance: WTTInstanceManager = new WTTInstanceManager();
    private version: string;
    private modName = "WTT-Pack 'n' Strap";

    //#region CustomBosses
    private CustomItemService: CustomItemService = new CustomItemService();

    debug = false;

    newIdMap = {
        container_smallscavcase: "0c22fc270f59b28c064e1232",
        container_toolpouch: "9543bbe8083934dc3b1b1330",
        container_smalldocscase: "c29f11b2e63a089916739c96",
        container_medpouch: "12403f74773f49be6a2d84b7",
        container_ammopouch: "ae9e418fd5d4c4eec4a0e6ea",
        container_magpouch: "440de5d056825485a0cf3a19",
        container_lunchbox: "6925918065a41e6b1e02a7d7",
        container_keyring: "2eabd4da4ab194eb168e72d3",
        belt_fannypack: "f8bd9b5115b176f3343c15ab",
        aerocorppistoleercombatbelt: "b8c4623858c113c6b723cb00",
        citadelgeneralpurposeutilitybelt: "8a7a8c543eb5f37e45953549",
        pointmanassaultbelt: "adeebd551b9ad402069733ce",
        polytechorionreconbelt: "8226ab90fb7f45d8cfa7fc3d",
        lh63russianfederationdutybelt: "942b6046d454de2606e23725",
        rebel9c: "c473791d93ca36cb24e17ff9",
        rebel9g: "f9a169d09b52ea7b50872357",
        assaullterbelt3: "aa6b05b680b18ae6fe7d8ad9",
        medicbelt: "07e5648876d6b14569c0c1c7",
        mrbgrenadierc: "37e439bcf48f216b37dd40de",
        mrbvogb: "b70d97cc28f80b524ee4ef2f",
        grenadierbelt: "60bb3baf34736c0636250b1a",
        assaultermedic: "e4c26a37cb69e8c50c61202f",
        mrblmga: "0a3e20a6db18234f8d6f218e",
        competitionbelt: "f80bdf274d83869039405ef8",
        riflemanbeltbison: "63ac5146862634e578bcb0c9",
        kitteddangler: "322eb5a82749d49003e82d98"
    };
    public preSptLoad(container: DependencyContainer): void {
        this.Instance.preSptLoad(container, this.modName);
        this.Instance.debug = this.debug;
        this.fixStupidMongoIds();
        if (!config.loseArmbandOnDeath) {
            this.keepItemsInArmbandAfterDeath();
        }
        this.CustomItemService.preSptLoad(this.Instance);

    }

    public postDBLoad(container: DependencyContainer): void {
        this.Instance.postDBLoad(container);
        this.CustomItemService.postDBLoad();
        this.Instance.logger.log(
            `[${this.modName}] Database: Loading complete.`,
            LogTextColor.GREEN
        );

        if (config.loseArmbandOnDeath) {
            const dblostondeathConfig = this.Instance.configServer.getConfig<ILostOnDeathConfig>(ConfigTypes.LOST_ON_DEATH)
            dblostondeathConfig.equipment.ArmBand = true;
        }

        if (config.addCasesToSecureContainer) {
            for (const caseId of Object.values(this.newIdMap)) {
                for (const item of Object.values(this.Instance.database.templates.items)) {
                    if (item._parent === "5448bf274bdc2dfc2f8b456a") {
                        // Log the item being processed
                        //console.log("Checking item: " + item._id);
        
                        // Check if the item is the bosscontainer
                        if (item._id === "5c0a794586f77461c458f892") {
                            //console.log("Skipping the bosscontainer");
                        } else {
                            const grids = item._props.Grids;
                            if (grids && grids.length > 0) {
                                const filters = grids[0]._props.filters[0];
                                if (filters) {
                                    if (filters.Filter === undefined) {
                                        filters.Filter = [caseId];
                                    } else {
                                        filters.Filter.push(caseId);
                                    }
                                } else {
                                    this.Instance.logger.log(
                                        `[${this.modName}] Failed to add cases to securecase filters (THEY DON'T EXIST DUE TO YOUR SVM SETTINGS). Turn off addToSecureCases in PackNStrap or load this BEFORE SVM. `,
                                        LogTextColor.YELLOW);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public fixStupidMongoIds(): void {
        // On game start, see if we need to fix issues from previous versions
        // Note: We do this as a method replacement so we can run _before_ SPT's gameStart
        this.Instance.container.afterResolution("GameController", (_, result: GameController) => {
            const originalGameStart = result.gameStart;

            result.gameStart = (url: string, info: IEmptyRequestData, sessionID: string, startTimeStampMS: number) => {
                // If there's a profile ID passed in, call our fixer method
                if (sessionID) {
                    this.fixProfile(sessionID);
                }

                // Call the original
                originalGameStart.apply(result, [url, info, sessionID, startTimeStampMS]);
            }
        });
    }

        // Handle updating the user profile between versions:
    // - Update the container IDs to the new MongoID format
    // - Look for any key cases in the user's inventory, and properly update the child key locations if we've moved them
    public fixProfile(sessionId: string) {
        const pmcProfile = this.Instance.profileHelper.getFullProfile(sessionId)?.characters?.pmc;
    
        // Do nothing if the profile isn't initialized
        if (!pmcProfile?.Inventory?.items) return;
    
        // Update the container IDs to the new MongoID format for inventory items
        pmcProfile.Inventory.items.forEach(item => {
            if (this.newIdMap[item._tpl]) {
                item._tpl = this.newIdMap[item._tpl];
                //console.log("Updated profile item to " + item._tpl);
            }
        });


    
        // Helper function to update rewards for quests
        const updateQuestRewards = (quests: any[]) => {
            if (!quests) return;
            
            quests.forEach(quest => {
                if (quest.rewards?.Success) {
                    quest.rewards.Success.forEach(reward => {
                        if (this.newIdMap[reward._tpl]) {
                            reward._tpl = this.newIdMap[reward._tpl];
                        }
                        if (Array.isArray(reward.items)) {
                            reward.items.forEach(item => {
                                if (this.newIdMap[item._tpl]) {
                                    item._tpl = this.newIdMap[item._tpl];
                                    //console.log("Updated reward item to " + item._tpl);
                                }
                            });
                        }
                    });
                }
            });
        };

        
        // Update rewards for Repeatable Quests
        pmcProfile.RepeatableQuests.forEach(questType => {
            updateQuestRewards(questType.activeQuests);
            updateQuestRewards(questType.inactiveQuests);
        });

    }

    public keepItemsInArmbandAfterDeath(): void {
        this.Instance.container.afterResolution("InRaidHelper", (_, result: any) => {
            const originalisItemKeptAfterDeath = result.isItemKeptAfterDeath;
    
            result.isItemKeptAfterDeath = (pmcData: IPmcData, itemToCheck: Item): boolean => {
                // Extract the _id of the item with slotId "ArmBand"
                const armBandItem = pmcData.Inventory.items.find(item => item.slotId === "ArmBand");
                const armBandItemId = armBandItem?._id;
    
                // Check if itemToCheck has the extracted _id as its parentId
                if (armBandItemId && itemToCheck.parentId === armBandItemId) {
                    return true; // Keep the item after death
                }
                // Otherwise, defer to the original method for other items
                return originalisItemKeptAfterDeath.call(result, pmcData, itemToCheck);
            };
        });
    }
    



}

module.exports = { mod: new PackNStrap() };
