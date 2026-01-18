using System.Collections.Generic;
using FlipwitchAP.Data;
using FlipwitchAP.Archipelago;
using HarmonyLib;
using static FlipwitchAP.Data.FlipwitchLocations;
using System;
using UnityEngine;
using Archipelago.MultiClient.Net.Enums;
using UnityEngine.Analytics;

namespace FlipwitchAP
{
    public class LocationHelper
    {
        private const string CHEST = "Chest";
        private const string CUTSCENE = "Cutscene";
        private const string QUEST = "Quest";

        // Code for patching gacha token pickups
        [HarmonyPatch(typeof(GachaCoinCollect), "OnTriggerEnter2D")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter2D_GiveLocationInsteadofToken(GachaCoinCollect __instance, Collider2D collision)
        {
            if (ArchipelagoClient.ServerData.Gachapon == ArchipelagoData.Gacha.Off)
            {
                return true;
            }
            var collected = (bool)__instance.GetType().GetField("collected", GenericMethods.Flags).GetValue(__instance);
            bool flag = PlayerController.GetPlayerControllerFromCollider(collision) != null;
            if (!collected && flag)
            {
                __instance.GetType().GetField("collected", GenericMethods.Flags).SetValue(__instance, true);
                var switchName = (string)__instance.GetType().GetField("switchName", GenericMethods.Flags).GetValue(__instance);
                var gachaLocation = FlipwitchLocations.CoinLocations[switchName];
                SendLocation(gachaLocation);
                AkSoundEngine.PostEvent("ui_gacha_coin_pickup", __instance.gameObject);
                SwitchDatabase.instance.setInt(switchName, 1);
                CreateItemNotification(gachaLocation, null);
                __instance.gameObject.SetActive(value: false);
            }
            return false;
        }


        // Code for patching chest pickups
        [HarmonyPatch(typeof(ChestItemDrop), "giveItem")]
        [HarmonyPrefix]
        private static bool giveItem_GiveLocationInstead(ChestItemDrop __instance)
        {
            var questStatus = ArchipelagoClient.ServerData.QuestForSex;
            if ((questStatus == ArchipelagoData.Quest.Off || questStatus == ArchipelagoData.Quest.Sensei) && FlipwitchItems.QuestItems.Contains(__instance.itemName))
            {
                return true;
            }
            if (!ArchipelagoClient.ServerData.ShuffleChaosPieces && __instance.itemName.Contains("ChaosKey"))
            {
                return true;
            }
            var switchName = (string)__instance.GetType().GetField("switchName", GenericMethods.Flags).GetValue(__instance);
            SwitchDatabase.instance.setInt(switchName, 1);
            if (!FlipwitchLocations.ChestLocations.TryGetValue(__instance.itemName, out var location))
            {
                Plugin.Logger.LogWarning($"Could not find location for {__instance.itemName}");
                return true;
            }
            SendLocation(location);
            var onItemPopupClosedCallback = (Action)__instance.GetType().GetField("onItemPopupClosedCallback", GenericMethods.Flags).GetValue(__instance);
            CreateItemNotification(location, onItemPopupClosedCallback);
            return false;
        }

        // Handles case where you open a Coin Chest.  There's a weird case where chests in slime sewers use the same switch name.
        [HarmonyPatch(typeof(ChestLootDrop), "spawnItem")]
        [HarmonyPrefix]
        private static bool SpawnItem_GiveAnItemInsteadOfCoins(ChestLootDrop __instance)
        {
            var switchName = (string)__instance.GetType().GetField("switchName", GenericMethods.Flags).GetValue(__instance);
            SwitchDatabase.instance.setInt(switchName, 1);
            var chestName = switchName;
            if (!FlipwitchLocations.CoinChestLocations.TryGetValue(chestName, out var location))
            {
                Plugin.Logger.LogWarning($"Could not find location for {chestName}");
                return true;
            }
            SendLocation(location);
            var scoutedInformation = ArchipelagoClient.ServerData.ScoutedLocations[location.APLocationID];
            CreateItemNotification(location, SwitchDatabase.instance.startTrackingTimePlayed);
            return false;

        }

        // Handles case where when talking to a NPC, they give you an item.  Its mainly used for quest items.
        [HarmonyPatch(typeof(DialogueManager), "anyKeyPressed")]
        [HarmonyPrefix]
        private static bool AnyKeyPressed_SendLocationNotGetItem(DialogueManager __instance)
        {
            var queuedItemName = (string)__instance.GetType().GetField("queuedItemName", GenericMethods.Flags).GetValue(__instance);
            if (queuedItemName != "PeachyPeach" && (ArchipelagoClient.ServerData.QuestForSex == ArchipelagoData.Quest.Off ||
            ArchipelagoClient.ServerData.QuestForSex == ArchipelagoData.Quest.Sensei))
            {
                return true;
            }
            var giveItemActive = (bool)__instance.GetType().GetField("giveItemActive", GenericMethods.Flags).GetValue(__instance);
            if (queuedItemName != "" && !giveItemActive)
            {
                __instance.GetType().GetField("giveItemActive", GenericMethods.Flags).SetValue(__instance, true);
                var uiCanvas = (GameObject)__instance.GetType().GetField("UICanvas", GenericMethods.Flags).GetValue(__instance);
                uiCanvas.SetActive(false);

                var location = FlipwitchLocations.SecondaryCallLocations[queuedItemName];
                SendLocation(location);
                CreateItemNotification(location, null);
                SetSwitchesForCertainLocations(location);
                return false;
            }
            return true;
        }

        private static void SetSwitchesForCertainLocations(LocationData location)
        {
            switch (location.APLocationName)
            {
                case "WW: Gobliana's Headshot":
                    {
                        SwitchDatabase.instance.setInt("APGoblianaGaveHeadshot", 1);
                        return;
                    }
                case "WW: Legs' Business Offer":
                    {
                        SwitchDatabase.instance.setInt("APLegsGaveCard", 1);
                        return;
                    }
                case "ST: Cabaret Cafe Delicious Milk":
                    {
                        SwitchDatabase.instance.setInt("APBelleGaveMilk", 1);
                        return;
                    }
                case "AH: Angel Letter":
                    {
                        SwitchDatabase.instance.setInt("APAngelGaveLetter", 1);
                        return;
                    }
                case "CD: Demon Letter":
                    {
                        SwitchDatabase.instance.setInt("APDemonGaveLetter", 1);
                        return;
                    }
                case "FF: Fungal Deed":
                    {
                        SwitchDatabase.instance.setInt("APMushroomGaveDeed", 1);
                        return;
                    }
                case "ST: MomoRobo Server Password":
                    {
                        SwitchDatabase.instance.setInt("APMomoGavePassword", 1);
                        return;
                    }
                case "ST: Leg's Office Goblin Apartment Key":
                    {
                        SwitchDatabase.instance.setInt("APGoblianaGaveKey", 1);
                        return;
                    }
            }
        }

        // Handles edge case where the player is given an item but the original call for it is on an Update call
        // where the initial check call is from a private variable.  Avoids lag.
        [HarmonyPatch(typeof(SwitchDatabase), "givePlayerItem")]
        [HarmonyPrefix]
        private static bool GivePlayerItem_GiveAPLocationInCaseofUpdateMethodNonsense(SwitchDatabase __instance, string itemName, bool skipPopup = false, Action onPopupCloseCallback = null)
        {

            if (UpdateWhitelist.Contains(itemName))
            {
                var questStatus = ArchipelagoClient.ServerData.QuestForSex;
                if (questStatus == ArchipelagoData.Quest.Off || questStatus == ArchipelagoData.Quest.Sensei)
                {
                    return true;
                }
                var location = SecondaryCallLocations[itemName];
                SendLocation(location);
                CreateItemNotification(location, onPopupCloseCallback);
                if (location.APLocationName == "WW: Gobliana's Belongings")
                {
                    SwitchDatabase.instance.setInt("APExGaveLuggage", 1);
                }
                if (CatStatueLocations.Contains(location.APLocationName))
                {
                    var kittyCount = SwitchDatabase.instance.getInt("APCatStatueCount");
                    SwitchDatabase.instance.setInt("APCatStatueCount", kittyCount + 1);
                }
                if (SummongStoneLocations.Contains(location.APLocationName))
                {
                    var summonStoneCount = SwitchDatabase.instance.getInt("APNatashaCount");
                    SwitchDatabase.instance.setInt("APNatashaCount", summonStoneCount + 1);
                }
                return false;
            }
            return true;
        }

        // Its kind of hard to find when this call is made to give you Peachy Peach, so I just revert it when it happens.
        [HarmonyPatch(typeof(EnablePeachyPeachIngameCutscene), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_ThisIsStupid()
        {
            var ints = (Dictionary<string, int>)SwitchDatabase.instance.GetType().GetField("ints", GenericMethods.Flags).GetValue(SwitchDatabase.instance);
            ints["PeachGiven"] -= 1;
            SwitchDatabase.instance.setInt("APPeachItemGiven", 1);
            SwitchDatabase.instance.GetType().GetField("ints", GenericMethods.Flags).SetValue(SwitchDatabase.instance, ints);
            var peachyLocation = CutsceneLocations["PeachGiven"];
            SendLocation(peachyLocation);
            return true;
        }

        // Handles case where Cutscenes give you items.
        [HarmonyPatch(typeof(SetSwitchValueIngameCutscene), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_GiveLocationInsteadOfSwitch(SetSwitchValueIngameCutscene __instance)
        {
            if (!CutsceneLocations.TryGetValue(__instance.switchName, out var location))
            {
                return true;
            }
            SendLocation(location);
            CreateItemNotification(location, null);
            if (location.APLocationName == "WW: Rescue Great Fairy")
            {
                SwitchDatabase.instance.setInt("WW_CrystalBreakTriggered", 1);
                // Normally this is set from the ruins crystal being destroyed.  We do this to avoid doing it so this stops spamming.
            }
            __instance.nextPhase.gameObject.SetActive(value: true);
            __instance.gameObject.SetActive(value: false);
            return false;
        }



        // Handles the case where you complete a quest.
        [HarmonyPatch(typeof(QuestUpdatedPopup), "completeQuest")]
        [HarmonyPostfix]
        private static void CompleteQuest_GiveQuestLocation(QuestUpdatedPopup __instance)
        {
            var questStatus = ArchipelagoClient.ServerData.QuestForSex;
            if (questStatus == ArchipelagoData.Quest.Off || questStatus == ArchipelagoData.Quest.Sensei)
            {
                return;
            }
            var questName = __instance.questName.textObject.text;
            if (!QuestLocations.TryGetValue(questName, out var location))
            {
                Plugin.Logger.LogWarning($"Quest {questName} does not have an associated location, returning");
                return;
            }
            SendLocation(location);
            //Do Location bit
        }

        // Handles the case where you collide with a health container.
        [HarmonyPatch(typeof(HealthContainerUpgrade), "OnTriggerEnter2D")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter2D_GiveItemInsteadHP(HealthContainerUpgrade __instance, Collider2D collision)
        {
            if (!ArchipelagoClient.ServerData.Statshuffle)
            {
                return true;
            }
            bool flag = PlayerController.GetPlayerControllerFromCollider(collision) != null;
            var collected = (bool)__instance.GetType().GetField("collected", GenericMethods.Flags).GetValue(__instance);
            if (!collected && flag)
            {
                __instance.GetType().GetField("collected", GenericMethods.Flags).SetValue(__instance, true);
                var switchName = (string)__instance.GetType().GetField("switchName", GenericMethods.Flags).GetValue(__instance);
                var anim = (Animator)__instance.GetType().GetField("anim", GenericMethods.Flags).GetValue(__instance);
                anim.Play("HeartContainer_collect");
                if (!FlipwitchLocations.StatLocations.TryGetValue(switchName, out var location))
                {
                    switchName = switchName.Replace(",", ".");  // European players may run into a situation where this is commas
                    location = FlipwitchLocations.StatLocations[switchName];
                }
                SendLocation(location);
                CreateItemNotification(location, null);
                AkSoundEngine.PostEvent("item_hpup", __instance.gameObject);
                SwitchDatabase.instance.setInt(switchName, 1);
                SwitchDatabase.instance.healPlayer(SwitchDatabase.instance.getPlayerHealthCap());
                SwitchDatabase.instance.addMana(SwitchDatabase.instance.getPlayerManaCap());
                __instance.gameObject.SetActive(value: false);
            }
            return false;
        }

        // Handles the case when you run into a mana container.
        [HarmonyPatch(typeof(ManaContainerUpgrade), "OnTriggerEnter2D")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter2D_GiveItemInsteadMP(HealthContainerUpgrade __instance, Collider2D collision)
        {
            if (!ArchipelagoClient.ServerData.Statshuffle)
            {
                return true;
            }
            bool flag = PlayerController.GetPlayerControllerFromCollider(collision) != null;
            var collected = (bool)__instance.GetType().GetField("collected", GenericMethods.Flags).GetValue(__instance);
            if (!collected && flag)
            {
                __instance.GetType().GetField("collected", GenericMethods.Flags).SetValue(__instance, true);
                var switchName = (string)__instance.GetType().GetField("switchName", GenericMethods.Flags).GetValue(__instance);
                var anim = (Animator)__instance.GetType().GetField("anim", GenericMethods.Flags).GetValue(__instance);
                anim.Play("ManaContainer_collect");
                if (!FlipwitchLocations.StatLocations.TryGetValue(switchName, out var location))
                {
                    switchName = switchName.Replace(",", ".");  // European players may run into a situation where this is commas
                    location = FlipwitchLocations.StatLocations[switchName];
                }
                SendLocation(location);
                CreateItemNotification(location, null);
                AkSoundEngine.PostEvent("item_mpup", __instance.gameObject);
                SwitchDatabase.instance.setInt(switchName, 1);
                SwitchDatabase.instance.healPlayer(SwitchDatabase.instance.getPlayerHealthCap());
                SwitchDatabase.instance.addMana(SwitchDatabase.instance.getPlayerManaCap());
                __instance.gameObject.SetActive(value: false);
            }
            return false;
        }

        // Handles Beatrix's rewards.
        [HarmonyPatch(typeof(SwitchDatabase), "handleAllPendingUpgrades")]
        [HarmonyPrefix]
        private static bool HandleAllPendingUpgrades_SendOutChecksInstead(SwitchDatabase __instance)
        {
            if (ArchipelagoClient.ServerData.QuestForSex == ArchipelagoData.Quest.Off)
            {
                return true;
            }
            var playerWandLocationCount = __instance.getInt("APPlayerWand");
            if (__instance.getInt("APPlayerWand") < __instance.getInt("PendingWandLevel"))
            {
                var secondWandLocation = SexExperienceLocations["WW: Sexual Experience Reward - Wand Upgrade 2"];
                if (playerWandLocationCount == 1)
                {
                    SendLocation(secondWandLocation);
                    __instance.setInt("APPlayerWand", 2);
                }
                playerWandLocationCount = __instance.getInt("APPlayerWand");
                var firstWandLocation = SexExperienceLocations["WW: Sexual Experience Reward - Wand Upgrade 1"];
                if (playerWandLocationCount == 0 && !Plugin.ArchipelagoClient.IsLocationChecked(firstWandLocation.APLocationID))
                {
                    SendLocation(firstWandLocation);
                    __instance.setInt("APPlayerWand", 1);
                }

            }
            if (__instance.getInt("SexualExperienceCount") >= 16)
            {
                var firstPeachyPower = SexExperienceLocations["WW: Sexual Experience Reward - Peach Upgrade 1"];
                SendLocation(firstPeachyPower);
            }
            if (__instance.getInt("SexualExperienceCount") >= 32)
            {
                var secondPeachyPower = SexExperienceLocations["WW: Sexual Experience Reward - Peach Upgrade 2"];
                SendLocation(secondPeachyPower);
            }
            var pendingPeachCharges = __instance.getInt("PendingPeachCharges");
            var totalChargesObtained = __instance.getInt("APTotalPeachCharges");
            var newChargeCount = Math.Min(10, pendingPeachCharges + totalChargesObtained);
            __instance.setInt("APTotalPeachCharges", newChargeCount);

            SendPeachyLocations(__instance.getInt("SexualExperienceCount"));

            __instance.setInt("PendingPeachCharges", 0);
            __instance.upgradePendingPopup.updatePendingPopupSymbol();
            return false;
        }
        
        [HarmonyPatch(typeof(DestructableObject), "spawnItem")]
        [HarmonyPrefix]
        private static bool SpawnItem_SendLocationFirstTime(DestructableObject __instance)
        {
            if (!ArchipelagoClient.ServerData.Potsanity)
            {
                return true;
            }

            var level = SwitchDatabase.instance.currentLevel.name;
            var potName = __instance.gameObject.name;
            if (!FlipwitchLocations.PotLocations.TryGetValue(level, out var pots))
            {
                Plugin.Logger.LogWarning($"We couldn't find a level for this pot!  Info to give to dev: {level} : {potName}");
                return true;
            }
            if (!pots.TryGetValue(potName, out var locationData))
            {
                Plugin.Logger.LogWarning($"We couldn't find a location for this pot!  Info to give to dev: {level} : {potName}");
                return true;
            }

            if (ArchipelagoClient.ServerData.CheckedLocations.Contains(locationData.APLocationID))
            {
                return true;
            }
            SendLocation(locationData);
            return false;
        }
        
        [HarmonyPatch(typeof(Teleporter), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_SendCheckInsteadOnCrystalTeleport(Teleporter __instance, ref string ___switchName)
        {
            if (!ArchipelagoClient.ServerData.CrystalTeleport)
            {
                return true;
            }
            if (!WarpLocations.TryGetValue(___switchName, out var locationData))
            {
                Plugin.Logger.LogWarning($"The warp {___switchName} isn't sending a check.  Typo in code?");
                return true;
            }

            if (ArchipelagoClient.ServerData.CheckedLocations.Contains(locationData.APLocationID))
            {
                return true;
            }
            SendLocation(locationData);
            // do some stuff here so if the check hasn't been sent yet, send a check, otherwise, allow use if enabled.
            return false;
        }

        private static void SendPeachyLocations(int sexExperience)
        {
            var totalCharges = sexExperience / 4;
            for (var i = 0; i < totalCharges; i++)
            {
                var pickedLocationName = $"WW: Sexual Experience Reward - Peach Charge {i + 1}";
                var pickedLocation = SexExperienceLocations[pickedLocationName];
                SendLocation(pickedLocation);
            }

        }

        public static void SendLocation(LocationData locationData)
        {

            var item = ArchipelagoClient.ServerData.ScoutedLocations[locationData.APLocationID];
            if (Plugin.ArchipelagoClient.IsLocationChecked(locationData.APLocationID))
            {
                return;
            }
            Plugin.ArchipelagoClient.DetermineOwnerAndDirectlyGiveIfSelf(locationData, item);

        }

        private static void CustomInternalPopUpItem(string itemName, Action onItemPopupClosedCallback = null)
        {
            switch (itemName)
            {
                case "ChaosKey1":
                    {
                        var chaosCount = Math.Min(6, SwitchDatabase.instance.getInt("APChaosKeyCount") + 1);
                        itemName = "ChaosKey" + chaosCount.ToString();
                        break;
                    }
                case "SummonStone1":
                    {
                        var stoneCount = Math.Min(3, SwitchDatabase.instance.getInt("APSummonStoneCount") + 1);
                        itemName = "SummonStone" + stoneCount.ToString();
                        break;
                    }
                case "SoulFragment1":
                    {
                        var soulCount = Math.Min(3, SwitchDatabase.instance.getInt("APSoulFragmentCount") + 1);
                        itemName = "SoulFragment" + soulCount.ToString();
                        break;
                    }
            }

            Item item2 = SwitchDatabase.instance.itemDictionary[itemName];
            string howToUseId = item2.itemType switch
            {
                ItemTypes.MagicalItem => "UI.EquipItem.MagicalItems",
                ItemTypes.Charms => "UI.EquipItem.Charms",
                ItemTypes.KeyItem => "UI.EquipItem.KeyItems",
                ItemTypes.Costumes => "UI.EquipItem.Costumes",
                ItemTypes.PlayerUpgrades => "UI.EquipItem.PlayerUpgrades",
                _ => throw new ArgumentOutOfRangeException(),
            };
            SwitchDatabase.instance.ItemCollectPopUp.popUpItem("Item." + item2.ItemID + ".Name", "Item." + item2.ItemID + ".Description", howToUseId, item2.animatorController, onItemPopupClosedCallback);
        }

        private static void FakeInternalPopUpItem(long locationID, ArchipelagoItem scoutedInformation)
        {
            var swappedItem = "Blank";
            var name = scoutedInformation.Name;
            var swappedDesc = "";
            var swappedFlav = "UI.EquipItem.PlayerUpgrades";
            var isOwnItem = scoutedInformation.SlotName == ArchipelagoClient.ServerData.SlotName;
            if (isOwnItem)
            {
                if (scoutedInformation.Name.Contains(" Figure #"))
                {
                    swappedItem = "BewitchedBubble";
                    swappedDesc = "Its a gachapon from one of those machines in Spirit Town!";
                }
                else
                {
                    swappedItem = ItemIconDatabase.CustomItemToRealItem[scoutedInformation.Name];
                    swappedDesc = FlipwitchItems.APItemToCustomDescription[scoutedInformation.Name];
                }
            }
            else
            {
                name = $"{scoutedInformation.Game}_{scoutedInformation.Name}_{locationID}_{scoutedInformation.SlotName}";
                swappedDesc = $"An item from the world of {scoutedInformation.Game}.  It vanishes upon you touching it.";
                var singleClassification = ItemFlags.None;

                if (scoutedInformation.Classification.HasFlag(ItemFlags.Trap))
                {
                    singleClassification = ItemFlags.Trap;
                }
                else if (scoutedInformation.Classification.HasFlag(ItemFlags.NeverExclude))
                {
                    singleClassification = ItemFlags.NeverExclude;
                }
                else if (scoutedInformation.Classification.HasFlag(ItemFlags.Advancement))
                {
                    singleClassification = ItemFlags.Advancement;
                }
                swappedFlav = FlipwitchItems.ClassificationToUseBlurb[singleClassification];
            }
            var fakeAnim = SwitchDatabase.instance.itemDictionary["Blank"].animatorController;
            if (isOwnItem)
            {
                fakeAnim = SwitchDatabase.instance.itemDictionary[swappedItem].animatorController;
            }

            SwitchDatabase.instance.ItemCollectPopUp.popUpItem(name, swappedDesc, swappedFlav, fakeAnim);

        }

        public static void CreateItemNotification(LocationData location, Action onItemPopupClosedCallback)
        {
            if (!ArchipelagoClient.ServerData.ScoutedLocations.TryGetValue(location.APLocationID, out var scoutedInformation))
            {
                Plugin.Logger.LogError($"Location {location.APLocationName} does not exist in the scouted information table!");
                return;
            }
            if (scoutedInformation.IsOwnItem || scoutedInformation.Game == ArchipelagoClient.Game)
            {
                var isShrimple = !FlipwitchItems.SkipPopupItems.Contains(scoutedInformation.Name);
                if (isShrimple && !scoutedInformation.Name.Contains(" Figure #"))
                {
                    if (!FlipwitchItems.APItemToGameName.TryGetValue(scoutedInformation.Name, out var inGameName))
                    {
                        Plugin.Logger.LogError($"Tried to grab {scoutedInformation.Name} from the in-game dictionary, but could not find it!");
                        return;
                    }
                    CustomInternalPopUpItem(inGameName, onItemPopupClosedCallback);
                }
                else
                {
                    FakeInternalPopUpItem(location.APLocationID, scoutedInformation);
                }
            }
            else
            {
                FakeInternalPopUpItem(location.APLocationID, scoutedInformation);
            }
        }
    }
}