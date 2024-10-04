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

namespace FlipwitchAP
{
    public class LocationHelper
    {
        public LocationHelper()
        {
            Harmony.CreateAndPatchAll(typeof(LocationHelper));
        }
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        // Code for patching gacha token pickups
        [HarmonyPatch(typeof(GachaCoinCollect), "OnTriggerEnter2D")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter2D_GiveLocationInsteadofToken(GachaCoinCollect __instance, Collider2D collision)
        {
            var collected = (bool)__instance.GetType().GetField("collected", Flags).GetValue(__instance);
            bool flag = PlayerController.GetPlayerControllerFromCollider(collision) != null;
            if (!collected && flag)
            {
                __instance.GetType().GetField("collected", Flags).SetValue(__instance, true);
                var switchName = (string)__instance.GetType().GetField("switchName", Flags).GetValue(__instance);
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
            var switchName = (string)__instance.GetType().GetField("switchName", Flags).GetValue(__instance);
            SwitchDatabase.instance.setInt(switchName, 1);
            if (!FlipwitchLocations.ChestLocations.TryGetValue(__instance.itemName, out var location))
            {
                Plugin.Logger.LogWarning($"Could not find location for {__instance.itemName}");
                return true;
            }
            SendLocationGivenLocationDataSendingGift(location);
            var scoutedInformation = ArchipelagoClient.ServerData.ScoutedLocations[location.APLocationID];
            if (scoutedInformation.IsOwnItem)
            {
                var isShrimple = !FlipwitchItems.SkipPopupItems.Contains(scoutedInformation.Name);
                if (isShrimple)
                {
                    var onItemPopupClosedCallback = (Action)__instance.GetType().GetField("onItemPopupClosedCallback", Flags).GetValue(__instance);
                    var inGameName = FlipwitchItems.APItemToGameName[scoutedInformation.Name];
                    CustomInternalPopUpItem(inGameName, onItemPopupClosedCallback);
                }
                else
                {
                    FakeInternalPopUpItem(scoutedInformation);
                }
            }
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

        [HarmonyPatch(typeof(DialogueManager), "anyKeyPressed")]
        [HarmonyPrefix]
        private static bool AnyKeyPressed_SendLocationNotGetItem(DialogueManager __instance)
        {
            var queuedItemName = (string)__instance.GetType().GetField("queuedItemName", Flags).GetValue(__instance);
            var giveItemActive = (bool)__instance.GetType().GetField("giveItemActive", Flags).GetValue(__instance);
            if (queuedItemName != "" && !giveItemActive)
            {
                __instance.GetType().GetField("giveItemActive", Flags).SetValue(__instance, true);
                var uiCanvas = (GameObject)__instance.GetType().GetField("UICanvas", Flags).GetValue(__instance);
                uiCanvas.SetActive(false);

                var location = FlipwitchLocations.SecondaryCallLocations[queuedItemName];
                var scoutedInformation = ArchipelagoClient.ServerData.ScoutedLocations[location.APLocationID];
                Plugin.Logger.LogInfo($"Is {scoutedInformation.Name} my own item? {scoutedInformation.IsOwnItem}");
                if (scoutedInformation.IsOwnItem)
                {
                    var isShrimple = !FlipwitchItems.SkipPopupItems.Contains(scoutedInformation.Name);
                    if (isShrimple)
                    {
                        var randomizedName = FlipwitchItems.APItemToGameName[scoutedInformation.Name];
                        CustomInternalPopUpItem(randomizedName);
                    }
                    else
                    {
                        FakeInternalPopUpItem(scoutedInformation);
                    }
                    return false;
                }
                Plugin.Logger.LogInfo($"Tried to give you {queuedItemName}");
                return false;
            }
            return true;
        }

        private static void FakeInternalPopUpItem(ArchipelagoItem scoutedInformation)
        {
            //We make up fake information to be kicked down the road.
            var fakeAnim = SwitchDatabase.instance.itemDictionary["BewitchedBubble"].animatorController;
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

            SwitchDatabase.instance.ItemCollectPopUp.popUpItem(scoutedInformation.Name, FlipwitchItems.APItemToCustomDescription[scoutedInformation.Name], FlipwitchItems.ClassificationToUseBlurb[singleClassification], fakeAnim);

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

        [HarmonyPatch(typeof(EnablePeachyPeachIngameCutscene), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_ThisIsStupid()
        {
            var ints = (Dictionary<string, int>)SwitchDatabase.instance.GetType().GetField("ints", Flags).GetValue(SwitchDatabase.instance);
            ints["PeachGiven"] -= 1;
            SwitchDatabase.instance.GetType().GetField("ints", Flags).SetValue(SwitchDatabase.instance, ints);
            var peachyLocation = FlipwitchLocations.CutsceneLocations["PeachGiven"];
            SendLocationGivenLocationDataSendingGift(peachyLocation);
            return true;
        }

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

        [HarmonyPatch(typeof(SetGameObjectEnabledOnSwitch), "checkIfSwitchSet")]
        [HarmonyPrefix]
        private static bool CheckIfSwitchSet_UseDifferentSwitchForArchipelago(SetGameObjectEnabledOnSwitch __instance)
        {
            var wasLocationChecked = false;
            if (__instance.switchName == "PeachGiven")
            {
                var location = FlipwitchLocations.CutsceneLocations["PeachGiven"];
                wasLocationChecked = Plugin.ArchipelagoClient.IsLocationChecked(location.APLocationID);
                __instance.deactivate = wasLocationChecked;
            }
            return true;
        }

        [HarmonyPatch(typeof(LevelStateManagerSingleSwitch), "UpdateAccordingToSwitch")]
        [HarmonyPrefix]
        private static bool UpdateAccordingToSwitch_OnlyManipulateTerrain(LevelStateManagerSingleSwitch __instance, ref int switchValue)
        {
            var wasLocationChecked = false;
            if (__instance.switchName == "GreatFairyStory" && __instance.transform.Find("GiantCrystal_208x176") is not null)
            {
                var location = FlipwitchLocations.CutsceneLocations["WW_CrystalBreakTriggered"];
                wasLocationChecked = Plugin.ArchipelagoClient.IsLocationChecked(location.APLocationID);
                switchValue = wasLocationChecked ? 3 : 0;
            }
            return true;
        }

        // Code for patching the gacha obtaining system
        [HarmonyPatch(typeof(GachaSceneManager), "ejectGacha")]
        [HarmonyPrefix]
        private static bool EjectGacha_SwapBoolToggleWithLocation(GachaSceneManager __instance)
        {
            var ballsRemaining = (int)__instance.GetType().GetField("ballsRemaining", Flags).GetValue(__instance) - 1;
            __instance.GetType().GetField("ballsRemaining", Flags).SetValue(__instance, ballsRemaining--);
            if (ballsRemaining < 0)
            {
                __instance.GetType().GetField("ballsRemaining", Flags).SetValue(__instance, 0);
            }
            __instance.gachaMachineAnim.SetInteger("BallCount", ballsRemaining);
            var remainingGachasIndexes = (List<int>)__instance.GetType().GetField("remainingGachasIndexes", Flags).GetValue(__instance);
            int index = UnityEngine.Random.Range(0, remainingGachasIndexes.Count);
            var currentCollection = (GachaCollections)__instance.GetType().GetField("currentCollection", Flags).GetValue(__instance);
            var closeGachaMethod = __instance.GetType().GetMethod("closeGachaScreen", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (GachaCollection gachaCollection in SwitchDatabase.instance.gachaCollections)
            {
                if (gachaCollection.collection == currentCollection)
                {
                    SwitchDatabase.instance.setBool($"{gachaCollection.collection}_{gachaCollection.gachas[remainingGachasIndexes[index]].number}", value: true);
                    __instance.GetType().GetField("currentPrize", Flags).SetValue(__instance, gachaCollection.gachas[remainingGachasIndexes[index]]);
                    var animation = gachaCollection.gachas[remainingGachasIndexes[index]].animationName;
                    var gachaLocation = FlipwitchLocations.GachaLocations[animation];
                    SendLocationGivenLocationDataSendingGift(gachaLocation);
                    remainingGachasIndexes.Remove(remainingGachasIndexes[index]);
                    __instance.GetType().GetField("remainingGachasIndexes", Flags).SetValue(__instance, remainingGachasIndexes);
                    return false;
                }
            }
            Plugin.Logger.LogError($"Unable to find a matching gacha collection for {currentCollection}.");
            SwitchDatabase.instance.addTokenToTokenCount(1);
            closeGachaMethod.Invoke(__instance, null);
            return false;
        }

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

        public static string SendLocationGivenLocationDataSendingGift(LocationData locationData)
        {

            var item = ArchipelagoClient.ServerData.ScoutedLocations[locationData.APLocationID];
            if (Plugin.ArchipelagoClient.IsLocationChecked(locationData.APLocationID))
            {
                return "ALREADY_ACQUIRED";
            }
            DetermineOwnerAndDirectlyGiveIfSelf(locationData, item);
            return item.Name;

        }

        public static bool DetermineOwnerAndDirectlyGiveIfSelf(LocationData location, ArchipelagoItem item)
        {
            if (item.IsOwnItem) // Handle without an internet connection.
            {
                var receivedItem = new ReceivedItem(item.Game, location.APLocationName, item.Name, item.SlotName, location.APLocationID, item.ID, item.SlotID, item.Classification);
                ArchipelagoClient.ServerData.ReceivedItems.Add(receivedItem);
                ItemHelper.GiveFlipwitchItem(receivedItem);
                ArchipelagoClient.ServerData.CheckedLocations.Add(location.APLocationID);
                if (ArchipelagoClient.Authenticated)
                {
                    Plugin.ArchipelagoClient.SendLocation(location.APLocationID);
                }
            }
            else if (ArchipelagoClient.Authenticated) // If someone else's item an online, do the usual
            {
                Plugin.ArchipelagoClient.SendLocation(location.APLocationID);
                ArchipelagoClient.ServerData.CheckedLocations.Add(location.APLocationID);

            }
            else  // Otherwise just save it for syncing later.
            {
                ArchipelagoClient.ServerData.CheckedLocations.Add(location.APLocationID);
            }
            return false;
        }
    }
}