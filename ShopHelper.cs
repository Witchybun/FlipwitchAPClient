using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net.Enums;
using FlipwitchAP.Archipelago;
using FlipwitchAP.Data;
using HarmonyLib;
using UnityEngine;

namespace FlipwitchAP
{
    public class ShopHelper
    {
        [HarmonyPatch(typeof(StoreUI), "OnEnable")]
        [HarmonyPostfix]
        private static void OnEnable_WriteArchipelagoInfoInstead(StoreUI __instance)
        {
            if (!ArchipelagoClient.ServerData.Shopsanity)
            {
                return;
            }
            var itemListings = (List<StoreItemListing>)__instance.GetType().GetField("itemListings", GenericMethods.Flags).GetValue(__instance);
            var itemsOnOffer = (List<ItemShopListingMetaData>)__instance.GetType().GetField("itemsOnOffer", GenericMethods.Flags).GetValue(__instance);
            for (var i = 0; i < itemsOnOffer.Count; i++)
            {
                if (itemListings[i].itemNameID == "")
                {
                    continue;
                }
                if (!FlipwitchLocations.ShopLocations.TryGetValue(itemListings[i].itemNameID, out var location))
                {
                    Plugin.Logger.LogWarning($"Could not find location for {itemListings[i].itemNameID}");
                    continue;
                }
                var locationScout = ArchipelagoClient.ServerData.ScoutedLocations[location.APLocationID];
                itemListings[i].listingName.textObject.text = locationScout.Name;
                itemListings[i].listingName.translationKey = locationScout.Name;
                if (int.TryParse(itemListings[i].listingCost.text, out var _))
                {
                    var priceSetup = int.Parse(itemListings[i].listingCost.text)*ArchipelagoClient.ServerData.ShopPrices;
                    itemListings[i].listingCost.text = Math.Max(1, priceSetup/100).ToString();
                    itemListings[i].listingName.forceUpdate();
                }
                
            }
        }

        [HarmonyPatch(typeof(StoreUI), "updateInput")]
        [HarmonyPrefix]
        private static bool UpdateInput_SendLocationOnPurchase(StoreUI __instance)
        {
            if (!ArchipelagoClient.ServerData.Shopsanity)
            {
                return true;
            }
            SwitchDatabase instance = SwitchDatabase.instance;
            var popUpActivated = (bool)__instance.GetType().GetField("popUpActivated", GenericMethods.Flags).GetValue(__instance);
            if (popUpActivated && !SwitchDatabase.instance.isItemPopupActive())
            {
                SwitchDatabase.instance.playerMov.disableMovement();
                instance.dialogueManager.setUiCanvasVisibility(isVisible: true);
            }
            Vector2 vector = NewInputManager.instance.pollUiMovement();
            var itemSelected = (bool)__instance.GetType().GetField("itemSelected", GenericMethods.Flags).GetValue(__instance);
            if (itemSelected)
            {
                if (vector.y != 0f)
                {
                    var yesSelected = (bool)__instance.GetType().GetField("yesSelected", GenericMethods.Flags).GetValue(__instance);
                    __instance.GetType().GetField("yesSelected", GenericMethods.Flags).SetValue(__instance, !yesSelected);
                    if (!yesSelected)
                    {
                        __instance.yesBox.color = __instance.selectedColour;
                        __instance.noBox.color = __instance.deselectedColour;
                    }
                    else
                    {
                        __instance.yesBox.color = __instance.deselectedColour;
                        __instance.noBox.color = __instance.selectedColour;
                    }

                }
                if (NewInputManager.instance.Interact.pressedThisFrame || NewInputManager.instance.Submit.pressedThisFrame)
                {
                    var yesSelected = (bool)__instance.GetType().GetField("yesSelected", GenericMethods.Flags).GetValue(__instance);
                    __instance.yesNoBox.SetActive(value: false);
                    __instance.GetType().GetField("itemSelected", GenericMethods.Flags).SetValue(__instance, false);
                    if (yesSelected)
                    {
                        var itemListings = (List<StoreItemListing>)__instance.GetType().GetField("itemListings", GenericMethods.Flags).GetValue(__instance);
                        var currentItemIndex = (int)__instance.GetType().GetField("currentItemIndex", GenericMethods.Flags).GetValue(__instance);
                        instance.setInt("ShopPurchase_" + itemListings[currentItemIndex].itemNameID, 1);
                        itemListings[currentItemIndex].listingName.textObject.color = new Color32(90, 90, 90, byte.MaxValue);
                        itemListings[currentItemIndex].listingCost.color = new Color32(90, 90, 90, byte.MaxValue);
                        SwitchDatabase.instance.chargeCoins(int.Parse(itemListings[currentItemIndex].listingCost.text));
                        var location = FlipwitchLocations.ShopLocations[itemListings[currentItemIndex].itemNameID];
                        LocationHelper.SendLocationGivenLocationDataSendingGift(location);
                        itemListings[currentItemIndex].listingCost.text = instance.dialogueManager.getTranslationString("ShopUI.SoldText");
                        instance.dialogueManager.setUiCanvasVisibility(isVisible: false);
                        __instance.GetType().GetField("popUpActivated", GenericMethods.Flags).SetValue(__instance, true);
                        AkSoundEngine.PostEvent("purchase", __instance.gameObject);
                    }
                }
                else if ((!SwitchDatabase.instance.isItemPopupActive() && NewInputManager.instance.Cancel.pressedThisFrame) || NewInputManager.instance.EscapeMenu.pressedThisFrame)
                {
                    __instance.yesNoBox.SetActive(value: false);
                    __instance.GetType().GetField("itemSelected", GenericMethods.Flags).SetValue(__instance, false);
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(StoreUI), "updateSelectedItemInfo")]
        [HarmonyPrefix]
        private static bool UpdateSelectedItemInfo_UpdateArchipelagoItemInfo(StoreUI __instance, string itemName)
        {
            if (!ArchipelagoClient.ServerData.Shopsanity)
            {
                return true;
            }
            if (!FlipwitchLocations.ShopLocations.TryGetValue(itemName, out var location))
            {
                Plugin.Logger.LogWarning($"Could not find location for {itemName}");
                return true;
            }
            var locationScout = ArchipelagoClient.ServerData.ScoutedLocations[FlipwitchLocations.ShopLocations[itemName].APLocationID];
            var isOwnGame = locationScout.Game == ArchipelagoClient.Game;
            var shopItemName = "";
            var shopItemFlav = "";
            var shopItemDesc = "";
            if (isOwnGame)
            {
                if (FlipwitchItems.APItemToCustomDescription.TryGetValue(locationScout.Name, out var blurb))
                {
                    shopItemName = locationScout.Name;
                    shopItemFlav = blurb;
                    shopItemDesc = "UI.EquipItem.PlayerUpgrades";
                }
                else if (locationScout.Name.Contains(" Figure #"))
                {
                    shopItemName = locationScout.Name;
                    shopItemFlav = "Its a gachapon from one of those machines in Spirit Town!";
                    shopItemDesc = "UI.EquipItem.PlayerUpgrades";
                }
                else
                {
                    var trueName = FlipwitchItems.APItemToGameName[locationScout.Name];
                    shopItemName = "Item." + trueName + ".Name";
                    shopItemFlav = "Item." + trueName + ".Flavour";
                    shopItemDesc = "Item." + trueName + ".Description";
                }
            }
            else
            {
                shopItemName = locationScout.SlotName + "'s " + locationScout.Name;
                shopItemFlav = $"An item from the world of {locationScout.Game}.";
                var singleClassification = ItemFlags.None;

                if (locationScout.Classification.HasFlag(ItemFlags.Trap))
                {
                    singleClassification = ItemFlags.Trap;
                }
                else if (locationScout.Classification.HasFlag(ItemFlags.Advancement))
                {
                    singleClassification = ItemFlags.Advancement;
                }
                else if (locationScout.Classification.HasFlag(ItemFlags.NeverExclude))
                {
                    singleClassification = ItemFlags.NeverExclude;
                }
                shopItemDesc = FlipwitchItems.ClassificationToUseBlurb[singleClassification];
            }
            __instance.itemNameUI.setTranslationKeyAndUpdate(shopItemName);
            __instance.itemFlavourUI.setTranslationKeyAndUpdate(shopItemFlav);
            __instance.itemDescUI.setTranslationKeyAndUpdate(shopItemDesc);
            return false;
        }
    }
}