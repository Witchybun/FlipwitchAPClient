using System.Collections.Generic;
using System.Reflection;
using FlipwitchAP.Data;
using FlipwitchAP.Archipelago;
using HarmonyLib;
using static FlipwitchAP.Data.FlipwitchLocations;
using System;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Exceptions;

namespace FlipwitchAP
{
    public class LocationHelper
    {
        public LocationHelper()
        {
            Harmony.CreateAndPatchAll(typeof(LocationHelper));
        }

        // Code for patching gacha token pickups
        [HarmonyPatch(typeof(GachaCoinCollect), "OnTriggerEnter2D")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter2D_GiveLocationInsteadofToken(GachaCoinCollect __instance, Collider2D collision)
        {
            var collected = (bool)__instance.GetType().GetField("collected", GenericMethods.Flags).GetValue(__instance);
            bool flag = PlayerController.GetPlayerControllerFromCollider(collision) != null;
            if (!collected && flag)
            {
                __instance.GetType().GetField("collected", GenericMethods.Flags).SetValue(__instance, true);
                var switchName = (string)__instance.GetType().GetField("switchName", GenericMethods.Flags).GetValue(__instance);
                var gachaLocation = FlipwitchLocations.CoinLocations[switchName];
                SendLocationGivenLocationDataSendingGift(gachaLocation);
                AkSoundEngine.PostEvent("ui_gacha_coin_pickup", __instance.gameObject);
                SwitchDatabase.instance.setInt(switchName, 1);
                __instance.gameObject.SetActive(value: false);
            }
            return false;
        }


        // Code for patching chest pickups
        [HarmonyPatch(typeof(ChestItemDrop), "giveItem")]
        [HarmonyPrefix]
        private static bool giveItem_GiveLocationInstead(ChestItemDrop __instance)
        {
            var switchName = (string)__instance.GetType().GetField("switchName", GenericMethods.Flags).GetValue(__instance);
            SwitchDatabase.instance.setInt(switchName, 1);
            if (!FlipwitchLocations.ChestLocations.TryGetValue(__instance.itemName, out var location))
            {
                Plugin.Logger.LogWarning($"Could not find location for {__instance.itemName}");
                return true;
            }
            SendLocationGivenLocationDataSendingGift(location);
            var onItemPopupClosedCallback = (Action)__instance.GetType().GetField("onItemPopupClosedCallback", GenericMethods.Flags).GetValue(__instance);
            CreateItemNotification(location, onItemPopupClosedCallback);
            return false;
        }

        // Changes how text looks when opening a chest
        [HarmonyPatch(typeof(ItemCollectPopup), "popUpItem")]
        [HarmonyPrefix]
        private static bool PopUpItem_ChangeToArchipelagoScoutWhenRelevant(ItemCollectPopup __instance, string itemNameId, string itemDescId, string howToUseId, RuntimeAnimatorController animator, Action onPopupCloseCallback = null)
        {
            Plugin.Logger.LogInfo($"We have {itemNameId}, {itemDescId}, {howToUseId}");
            return true;
        }

        // Handles case where you open a Coin Chest.  There's a weird case where chests in slime sewers use the same switch name.
        [HarmonyPatch(typeof(ChestLootDrop), "spawnItem")]
        [HarmonyPrefix]
        private static bool SpawnItem_GiveAnItemInsteadOfCoins(ChestLootDrop __instance)
        {
            var switchName = (string)__instance.GetType().GetField("switchName", GenericMethods.Flags).GetValue(__instance);
            SwitchDatabase.instance.setInt(switchName, 1);
            var chestName = switchName;
            if (switchName == "gc_slime_secret_coins")
            {
                var objectName = __instance.name;
                switch (objectName)
                {
                    case "Drop Coins Chest":
                        {
                            chestName = switchName + "_1";
                            break;
                        }
                    case "Drop Coins Chest (2)":
                        {
                            chestName = switchName + "_2";
                            break;
                        }
                    case "Drop Coins Chest (3)":
                        {
                            chestName = switchName + "_3";
                            break;
                        }
                }
            }
            if (!FlipwitchLocations.CoinChestLocations.TryGetValue(chestName, out var location))
            {
                Plugin.Logger.LogWarning($"Could not find location for {chestName}");
                return true;
            }
            SendLocationGivenLocationDataSendingGift(location);
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
            var giveItemActive = (bool)__instance.GetType().GetField("giveItemActive", GenericMethods.Flags).GetValue(__instance);
            if (queuedItemName != "" && !giveItemActive)
            {
                __instance.GetType().GetField("giveItemActive", GenericMethods.Flags).SetValue(__instance, true);
                var uiCanvas = (GameObject)__instance.GetType().GetField("UICanvas", GenericMethods.Flags).GetValue(__instance);
                uiCanvas.SetActive(false);

                var location = FlipwitchLocations.SecondaryCallLocations[queuedItemName];
                SendLocationGivenLocationDataSendingGift(location);
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

            }
        }

        // Handles edge case where the player is given an item but the original call for it is on an Update call
        // where the initial check call is from a private variable.  Avoids lag.
        [HarmonyPatch(typeof(SwitchDatabase), "givePlayerItem")]
        [HarmonyPrefix]
        private static bool GivePlayerItem_GiveAPLocationInCaseofUpdateMethodNonsense(SwitchDatabase __instance, string itemName, bool skipPopup = false, Action onPopupCloseCallback = null)
        {
            if (FlipwitchLocations.UpdateWhitelist.Contains(itemName))
            {
                var location = SecondaryCallLocations[itemName];
                SendLocationGivenLocationDataSendingGift(location);
                CreateItemNotification(location, onPopupCloseCallback);
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
            SwitchDatabase.instance.addInt("APPeachItemGiven", 0);
            SwitchDatabase.instance.GetType().GetField("ints", GenericMethods.Flags).SetValue(SwitchDatabase.instance, ints);
            var peachyLocation = FlipwitchLocations.CutsceneLocations["PeachGiven"];
            SendLocationGivenLocationDataSendingGift(peachyLocation);
            return true;
        }

        // Handles case where Cutscenes give you items.
        [HarmonyPatch(typeof(SetSwitchValueIngameCutscene), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_GiveLocationInsteadOfSwitch(SetSwitchValueIngameCutscene __instance)
        {
            Plugin.Logger.LogInfo(__instance.switchName);
            if (!FlipwitchLocations.CutsceneLocations.TryGetValue(__instance.switchName, out var location))
            {
                return true;
            }
            SendLocationGivenLocationDataSendingGift(location);
            __instance.nextPhase.gameObject.SetActive(value: true);
            __instance.gameObject.SetActive(value: false);
            return false;
        }

        // Code for loading machine based on archipelago-based rolls, not what you actually have.
        [HarmonyPatch(typeof(GachaSceneManager), "loadMachine")]
        [HarmonyPostfix]
        private static void LoadMachine_ReloadMachineBasedOnLocationsNotItems(GachaSceneManager __instance, GachaCollections collection)
        {
            if (!ArchipelagoClient.ServerData.GachaOn)
            {
                return;
            }
            foreach (GachaCollection gachaCollection in SwitchDatabase.instance.gachaCollections)
            {
                if (gachaCollection.collection != collection)
                {
                    continue;
                }
                var initialBalls = collection == GachaCollections.Promotion ? 1 : 10;
                __instance.GetType().GetField("ballsRemaining", GenericMethods.Flags).SetValue(__instance, initialBalls);
                __instance.GetType().GetField("remainingGachasIndexes", GenericMethods.Flags).SetValue(__instance, new List<int>());
                int num = 0;
                __instance.sticker.sprite = gachaCollection.gachaSticker;
                if (!ArchipelagoClient.ServerData.CompletedGacha.ContainsKey(collection))
                {
                    ArchipelagoClient.ServerData.CompletedGacha[collection] = new();
                }
                foreach (Gacha gacha in gachaCollection.gachas)
                {
                    if (ArchipelagoClient.ServerData.CompletedGacha[collection].Contains(gacha.number))
                    {
                        var currentBalls = (int)__instance.GetType().GetField("ballsRemaining", GenericMethods.Flags).GetValue(__instance);
                        __instance.GetType().GetField("ballsRemaining", GenericMethods.Flags).SetValue(__instance, currentBalls - 1);
                    }
                    else
                    {
                        var currentIndexes = (List<int>)__instance.GetType().GetField("remainingGachasIndexes", GenericMethods.Flags).GetValue(__instance);
                        var newIndexes = new List<int>();
                        foreach (var index in currentIndexes)
                        {
                            Plugin.Logger.LogInfo($"Index {index} is in the collection.");
                            newIndexes.Add(index);
                        }
                        newIndexes.Add(num);
                        __instance.GetType().GetField("remainingGachasIndexes", GenericMethods.Flags).SetValue(__instance, newIndexes);
                    }
                    num++;
                }
                var ballsRemaining = (int)__instance.GetType().GetField("ballsRemaining", GenericMethods.Flags).GetValue(__instance);
                __instance.gachaMachineAnim.SetInteger("BallCount", ballsRemaining);
                __instance.GetType().GetMethod("updateTokenDisplay", GenericMethods.Flags).Invoke(__instance, null);
            }
        }

        // Code for patching the gacha obtaining system
        [HarmonyPatch(typeof(GachaSceneManager), "ejectGacha")]
        [HarmonyPrefix]
        private static bool EjectGacha_SwapBoolToggleWithLocation(GachaSceneManager __instance)
        {
            if (!ArchipelagoClient.ServerData.GachaOn)
            {
                return true;
            }
            var ballsRemaining = (int)__instance.GetType().GetField("ballsRemaining", GenericMethods.Flags).GetValue(__instance) - 1;
            __instance.GetType().GetField("ballsRemaining", GenericMethods.Flags).SetValue(__instance, ballsRemaining--);
            if (ballsRemaining < 0)
            {
                __instance.GetType().GetField("ballsRemaining", GenericMethods.Flags).SetValue(__instance, 0);
            }
            __instance.gachaMachineAnim.SetInteger("BallCount", ballsRemaining);
            var remainingGachasIndexes = (List<int>)__instance.GetType().GetField("remainingGachasIndexes", GenericMethods.Flags).GetValue(__instance);
            foreach (var mrow in remainingGachasIndexes)
            {
                Plugin.Logger.LogInfo($"{mrow}");
            }
            Plugin.Logger.LogInfo($"{remainingGachasIndexes.Count}");
            int index = UnityEngine.Random.Range(0, remainingGachasIndexes.Count);
            var currentCollection = (GachaCollections)__instance.GetType().GetField("currentCollection", GenericMethods.Flags).GetValue(__instance);
            var closeGachaMethod = __instance.GetType().GetMethod("closeGachaScreen", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (GachaCollection gachaCollection in SwitchDatabase.instance.gachaCollections)
            {
                if (gachaCollection.collection == currentCollection)
                {
                    SwitchDatabase.instance.setBool($"{gachaCollection.collection}_{gachaCollection.gachas[remainingGachasIndexes[index]].number}", value: true);
                    __instance.GetType().GetField("currentPrize", GenericMethods.Flags).SetValue(__instance, gachaCollection.gachas[remainingGachasIndexes[index]]);
                    var animation = gachaCollection.gachas[remainingGachasIndexes[index]].animationName;
                    var gachaLocation = FlipwitchLocations.GachaLocations[animation];
                    SendLocationGivenLocationDataSendingGift(gachaLocation);
                    var currentCollectionType = (GachaCollections)__instance.GetType().GetField("currentCollection", GenericMethods.Flags).GetValue(__instance);
                    if (ArchipelagoClient.ServerData.CompletedGacha[currentCollectionType] is null)
                    {
                        ArchipelagoClient.ServerData.CompletedGacha[currentCollectionType] = new();
                    }
                    ArchipelagoClient.ServerData.CompletedGacha[currentCollectionType].Add(index);
                    remainingGachasIndexes.Remove(remainingGachasIndexes[index]);
                    __instance.GetType().GetField("remainingGachasIndexes", GenericMethods.Flags).SetValue(__instance, remainingGachasIndexes);
                    return false;
                }
            }
            Plugin.Logger.LogError($"Unable to find a matching gacha collection for {currentCollection}.");
            SwitchDatabase.instance.addTokenToTokenCount(1);
            closeGachaMethod.Invoke(__instance, null);
            return false;
        }

        // Handles the case where you complete a quest.
        [HarmonyPatch(typeof(QuestUpdatedPopup), "completeQuest")]
        [HarmonyPostfix]
        private static void CompleteQuest_GiveQuestLocation(QuestUpdatedPopup __instance)
        {
            var questName = __instance.questName.textObject.text;
            if (!FlipwitchLocations.QuestLocations.TryGetValue(questName, out var location))
            {
                Plugin.Logger.LogWarning($"Quest {questName} does not have an associated location, returning");
                return;
            }
            SendLocationGivenLocationDataSendingGift(location);
            //Do Location bit
        }

        // Handles the case where you collide with a health container.
        [HarmonyPatch(typeof(HealthContainerUpgrade), "OnTriggerEnter2D")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter2D_GiveItemInsteadHP(HealthContainerUpgrade __instance, Collider2D collision)
        {
            bool flag = PlayerController.GetPlayerControllerFromCollider(collision) != null;
            var collected = (bool)__instance.GetType().GetField("collected", GenericMethods.Flags).GetValue(__instance);
            if (!collected && flag)
            {
                __instance.GetType().GetField("collected", GenericMethods.Flags).SetValue(__instance, true);
                var switchName = (string)__instance.GetType().GetField("switchName", GenericMethods.Flags).GetValue(__instance);
                var anim = (Animator)__instance.GetType().GetField("anim", GenericMethods.Flags).GetValue(__instance);
                anim.Play("HeartContainer_collect");
                var location = FlipwitchLocations.StatLocations[switchName];
                SendLocationGivenLocationDataSendingGift(location);
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
            bool flag = PlayerController.GetPlayerControllerFromCollider(collision) != null;
            var collected = (bool)__instance.GetType().GetField("collected", GenericMethods.Flags).GetValue(__instance);
            if (!collected && flag)
            {
                __instance.GetType().GetField("collected", GenericMethods.Flags).SetValue(__instance, true);
                var switchName = (string)__instance.GetType().GetField("switchName", GenericMethods.Flags).GetValue(__instance);
                var anim = (Animator)__instance.GetType().GetField("anim", GenericMethods.Flags).GetValue(__instance);
                anim.Play("ManaContainer_collect");
                var location = FlipwitchLocations.StatLocations[switchName];
                SendLocationGivenLocationDataSendingGift(location);
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
            var amountOfSex = __instance.getInt("SexualExperienceCount");
            foreach (var location in FlipwitchLocations.SexExperienceLocations)
            {
                if (amountOfSex >= int.Parse(location.Value.PrimaryCallName))
                {
                    SendLocationGivenLocationDataSendingGift(location.Value);
                }
            }
            return false;
        }

        public static void SendLocationGivenLocationDataSendingGift(LocationData locationData)
        {

            var item = ArchipelagoClient.ServerData.ScoutedLocations[locationData.APLocationID];
            if (Plugin.ArchipelagoClient.IsLocationChecked(locationData.APLocationID))
            {
                return;
            }
            Plugin.ArchipelagoClient.DetermineOwnerAndDirectlyGiveIfSelf(locationData, item);
            return;

        }

        private static void CustomInternalPopUpItem(string itemName, Action onItemPopupClosedCallback = null)
        {
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

        private static void FakeInternalPopUpItem(ArchipelagoItem scoutedInformation)
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
                name = scoutedInformation.SlotName + "'s " + scoutedInformation.Name;
                swappedItem = ItemIconDatabase.GiveSpecialIconGivenItemname(scoutedInformation.Name);
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
            var fakeAnim = SwitchDatabase.instance.itemDictionary[swappedItem].animatorController;


            SwitchDatabase.instance.ItemCollectPopUp.popUpItem(name, swappedDesc, swappedFlav, fakeAnim);

        }

        public static void CreateItemNotification(LocationData location, Action onItemPopupClosedCallback)
        {
            var scoutedInformation = ArchipelagoClient.ServerData.ScoutedLocations[location.APLocationID];
            if (scoutedInformation.IsOwnItem)
            {
                var isShrimple = !FlipwitchItems.SkipPopupItems.Contains(scoutedInformation.Name);
                if (isShrimple && !scoutedInformation.Name.Contains (" Figure #"))
                {
                    var inGameName = FlipwitchItems.APItemToGameName[scoutedInformation.Name];
                    CustomInternalPopUpItem(inGameName, onItemPopupClosedCallback);
                }
                else
                {
                    FakeInternalPopUpItem(scoutedInformation);
                }
            }
            else
            {
                FakeInternalPopUpItem(scoutedInformation);
            }
        }
    }
}