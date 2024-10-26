using System;
using System.Collections.Generic;
using System.Linq;
using FlipwitchAP.Archipelago;
using FlipwitchAP.Data;
using HarmonyLib;
using UnityEngine;

namespace FlipwitchAP
{
    public class GachaHelper
    {
        private static GachaPhase apPhase;
        public GachaHelper()
        {
            Harmony.CreateAndPatchAll(typeof(GachaHelper));
        }

        // Patch the enum call.  Fuck this.
        [HarmonyPatch(typeof(GachaSceneManager), "gachaWaiting")]
        [HarmonyPrefix]
        private static bool GachaWaiting_ChangeEnum(GachaSceneManager __instance)
        {
            __instance.prizeArrow.SetActive(value: true);
            apPhase = GachaPhase.INPUT_OPEN_PRIZE;
            return false;
        }

        // Patch the top number displaying the correct type of coin instead
        [HarmonyPatch(typeof(GachaSceneManager), "updateTokenDisplay")]
        [HarmonyPrefix]
        private static bool UpdateTokenDisplay_DisplayAPCoinCountInstead(GachaSceneManager __instance)
        {
            var currentCollection = (GachaCollections)__instance.GetType().GetField("currentCollection", GenericMethods.Flags).GetValue(__instance);
            var coinName = ObtainCoinName(currentCollection);
            int gachaTokenCount = SwitchDatabase.instance.getInt("AP" + coinName);
            if (gachaTokenCount == 0)
            {
                __instance.tokenCounter.color = Color.red;
            }
            else
            {
                __instance.tokenCounter.color = Color.white;
            }
            __instance.tokenCounter.text = gachaTokenCount.ToString();
            return false;
        }

        // Code for loading machine based on archipelago-based rolls, not what you actually have.
        [HarmonyPatch(typeof(GachaSceneManager), "loadMachine")]
        [HarmonyPostfix]
        private static void LoadMachine_ReloadMachineBasedOnLocationsNotItems(GachaSceneManager __instance, GachaCollections collection, 
        ref int ___ballsRemaining, ref List<int> ___remainingGachasIndexes)
        {
            apPhase = GachaPhase.INPUT_PAY;
            foreach (GachaCollection gachaCollection in SwitchDatabase.instance.gachaCollections)
            {
                if (gachaCollection.collection != collection)
                {
                    continue;
                }
                var currentGachaList = ObtainRandomOrderList(collection);
                var currentListCount = currentGachaList.Count;
                var thisCollection = collection;
                var currentGachaName = thisCollection.ToString();
                var currentGachaState = SwitchDatabase.instance.getInt(currentGachaName);
                var initialBalls = gachaCollection.gachas.Count;
                ___remainingGachasIndexes = currentGachaList;
                __instance.sticker.sprite = gachaCollection.gachaSticker;

                ___ballsRemaining = initialBalls - currentGachaState;
                __instance.gachaMachineAnim.SetInteger("BallCount", ___ballsRemaining);
                __instance.GetType().GetMethod("updateTokenDisplay", GenericMethods.Flags).Invoke(__instance, null);
            }
        }

        // Code for patching the gacha obtaining system
        [HarmonyPatch(typeof(GachaSceneManager), "ejectGacha")]
        [HarmonyPrefix]
        private static bool EjectGacha_SwapBoolToggleWithLocation(GachaSceneManager __instance, ref GachaCollections ___currentCollection, 
        ref int ___ballsRemaining)
        {
            ___ballsRemaining--;
            if (___ballsRemaining < 0)
            {
                ___ballsRemaining = 0;
            }
            var ballCountAnimParam = (string)__instance.GetType().GetField("ballCountAnimParam", GenericMethods.Flags).GetValue(__instance);
            __instance.gachaMachineAnim.SetInteger(ballCountAnimParam, ___ballsRemaining);
            var chosenGachaName = ___currentCollection.ToString();
            var currentGachaState = SwitchDatabase.instance.getInt("AP" + chosenGachaName);
            int randomIndex = currentGachaState;
            var closeGachaMethod = __instance.GetType().GetMethod("closeGachaScreen", GenericMethods.Flags);
            foreach (GachaCollection gachaCollection in SwitchDatabase.instance.gachaCollections)
            {
                if (gachaCollection.collection == ___currentCollection)
                {
                    var PickedGachaList = ObtainRandomOrderList(___currentCollection);
                    var elementsInList = string.Join(", ", PickedGachaList);
                    int index = PickedGachaList.ElementAt(randomIndex);
                    index -= 1;
                    var chosenGacha = gachaCollection.gachas[index];
                    SwitchDatabase.instance.setBool($"{gachaCollection.collection}_{chosenGacha.number}", value: true);
                    __instance.GetType().GetField("currentPrize", GenericMethods.Flags).SetValue(__instance, chosenGacha);
                    var animation = gachaCollection.gachas[index].animationName;
                    var gachaLocation = FlipwitchLocations.GachaLocations[animation];
                    if (ArchipelagoClient.ServerData.GachaOn)
                    {
                        LocationHelper.SendLocationGivenLocationDataSendingGift(gachaLocation);
                    }
                    SwitchDatabase.instance.setInt("AP" + chosenGachaName, currentGachaState + 1);
                    __instance.GetType().GetMethod("updateTokenDisplay", GenericMethods.Flags).Invoke(__instance, null);
                    return false;
                }
            }
            Plugin.Logger.LogError($"Unable to find a matching gacha collection for {___currentCollection}.");
            SwitchDatabase.instance.addTokenToTokenCount(1);
            closeGachaMethod.Invoke(__instance, null);
            return false;
        }

        //Update the update...eugh.
        [HarmonyPatch(typeof(GachaSceneManager), "Update")]
        [HarmonyPrefix]
        private static bool Update_AlterGachaBasedOnNewCoinSystem(GachaSceneManager __instance, ref float ___closePrizeCooldown)
        {
            switch (apPhase)
            {
                case GachaPhase.INPUT_PAY:
                    if (NewInputManager.instance.Cancel.pressedThisFrame || NewInputManager.instance.EscapeMenu.pressedThisFrame)
                    {
                        var closeGachaScreenCall = __instance.GetType().GetMethod("closeGachaScreen", GenericMethods.Flags);
                        closeGachaScreenCall.Invoke(__instance, null);
                    }
                    else
                    {
                        if (!NewInputManager.instance.Submit.pressedThisFrame && !NewInputManager.instance.Interact.pressedThisFrame)
                        {
                            break;
                        }
                        var ballsRemaining = (int)__instance.GetType().GetField("ballsRemaining", GenericMethods.Flags).GetValue(__instance);
                        var currentCollection = (GachaCollections)__instance.GetType().GetField("currentCollection", GenericMethods.Flags).GetValue(__instance);
                        var getCoinCountForGivenState = GetCoinCountForGivenCollection(currentCollection);
                        if (ballsRemaining > 0 && (SwitchDatabase.instance.DEBUG_infiniteGachaCoins || getCoinCountForGivenState > 0))
                        {
                            if (!SwitchDatabase.instance.DEBUG_infiniteGachaCoins)
                            {
                                RemoveTokenGivenCollection(currentCollection);
                                SwitchDatabase.instance.GetType().GetMethod("refreshGachaTokenCount", GenericMethods.Flags).Invoke(SwitchDatabase.instance, null);
                            }
                            var updateTokenDisplay = __instance.GetType().GetMethod("updateTokenDisplay", GenericMethods.Flags);
                            updateTokenDisplay.Invoke(__instance, null);
                            __instance.slotArrow.SetActive(value: false);
                            AkSoundEngine.PostEvent("ui_gacha_holdcoin_stop", __instance.gameObject);
                            AkSoundEngine.PostEvent("ui_gacha_dropcoin", __instance.gameObject);
                            var startGachaAnimParam = (string)__instance.GetType().GetField("startGachaAnimParam", GenericMethods.Flags).GetValue(__instance);
                            var gachaStateParamStart = (int)__instance.GetType().GetField("gachaStateParamStart", GenericMethods.Flags).GetValue(__instance);
                            __instance.screenAnim.SetInteger(startGachaAnimParam, gachaStateParamStart);
                            apPhase = GachaPhase.PAID;
                        }
                        else
                        {
                            AkSoundEngine.PostEvent("ui_fail", __instance.gameObject);
                        }
                    }
                    break;
                case GachaPhase.INPUT_OPEN_PRIZE:
                    if (NewInputManager.instance.Submit.pressedThisFrame || NewInputManager.instance.Interact.pressedThisFrame)
                    {
                        AkSoundEngine.PostEvent("ui_gacha_open", __instance.gameObject);
                        __instance.prizeArrow.SetActive(value: false);
                        var startGachaAnimParam = (string)__instance.GetType().GetField("startGachaAnimParam", GenericMethods.Flags).GetValue(__instance);
                        var gachaStateParamOpen = (int)__instance.GetType().GetField("gachaStateParamOpen", GenericMethods.Flags).GetValue(__instance);
                        __instance.screenAnim.SetInteger(startGachaAnimParam, gachaStateParamOpen);
                        __instance.GetType().GetField("closePrizeCooldown", GenericMethods.Flags).SetValue(__instance, 0f);
                        apPhase = GachaPhase.PRIZE_OPENING;
                        GachaSceneManager.checkForAchievementCompletion();
                    }
                    break;
                case GachaPhase.PRIZE_OPENING:
                    if (___closePrizeCooldown > 0f)
                    {
                        ___closePrizeCooldown -= Time.unscaledDeltaTime;
                        if (!(___closePrizeCooldown > 0f))
                        {
                            ___closePrizeCooldown = 0f;
                            apPhase = GachaPhase.INPUT_CLOSE_PRIZE;
                        }
                    }
                    break;
                case GachaPhase.INPUT_CLOSE_PRIZE:
                    if (NewInputManager.instance.anyKeyPressed())
                    {
                        var prizeAnimator = (Animator)__instance.GetType().GetField("prizeAnimator", GenericMethods.Flags).GetValue(__instance);
                        prizeAnimator.gameObject.SetActive(value: false);
                        __instance.slotArrow.SetActive(value: true);
                        __instance.fade.color = new Color(0f, 0f, 0f, 0f);
                        apPhase = GachaPhase.INPUT_PAY;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                case GachaPhase.PAID:
                    break;
            }
            return false;
        }

        [HarmonyPatch(typeof(KeyItemScreen), "initialiseDisplay")]
        [HarmonyPostfix]
        private static void InitializeDisplay_WriteSpecialGachaInstead(KeyItemScreen __instance)
        {
            var animalCount = GetStringFromInt(SwitchDatabase.instance.getInt("APAnimalCoin"));
            var bunnyCount = GetStringFromInt(SwitchDatabase.instance.getInt("APBunnyCoin"));
            var monsterCount = GetStringFromInt(SwitchDatabase.instance.getInt("APMonsterCoin"));
            var angelCount = GetStringFromInt(SwitchDatabase.instance.getInt("APAngelDemonCoin"));
            var promotionCount = GetStringFromInt(SwitchDatabase.instance.getInt("APPromotionalCoin"));
            var constructedString = animalCount + bunnyCount + monsterCount + angelCount + promotionCount;
            __instance.gachaCoinCount.text = constructedString;
        }

        [HarmonyPatch(typeof(SwitchDatabase), "refreshGachaTokenCount")]
        [HarmonyPostfix]
        private static void RefreshGachaTokenCount_WriteSpecialGachaInstead(SwitchDatabase __instance)
        {
            var animalCount = GetStringFromInt(SwitchDatabase.instance.getInt("APAnimalCoin"));
            var bunnyCount = GetStringFromInt(SwitchDatabase.instance.getInt("APBunnyCoin"));
            var monsterCount = GetStringFromInt(SwitchDatabase.instance.getInt("APMonsterCoin"));
            var angelCount = GetStringFromInt(SwitchDatabase.instance.getInt("APAngelDemonCoin"));
            var promotionCount = GetStringFromInt(SwitchDatabase.instance.getInt("APPromotionalCoin"));
            var constructedString = animalCount + bunnyCount + monsterCount + angelCount + promotionCount;
            __instance.gachaToken_text.text = constructedString;
        }

        private static string GetStringFromInt(int count)
        {
            if (count > 9)
            {
                return "A";
            }
            if (count < 1)
            {
                return "-";
            }
            return count.ToString();
        }

        private static List<int> ObtainRandomOrderList(GachaCollections currentCollection)
        {
            switch (currentCollection)
            {
                case GachaCollections.AnimalGirls:
                    {
                        var gachaOrder = ArchipelagoClient.ServerData.AnimalGachaOrder;
                        if (gachaOrder.Count < 10 || gachaOrder.Contains(0))
                        {
                            ArchipelagoClient.ServerData.RegenerateGachaOrder();
                            gachaOrder = ArchipelagoClient.ServerData.AnimalGachaOrder;
                        }
                        return gachaOrder;
                    }
                case GachaCollections.Bunnys:
                    {
                        var gachaOrder = ArchipelagoClient.ServerData.BunnyGachaOrder;
                        if (gachaOrder.Count < 10 || gachaOrder.Contains(0))
                        {
                            ArchipelagoClient.ServerData.RegenerateGachaOrder();
                            gachaOrder = ArchipelagoClient.ServerData.BunnyGachaOrder;
                        }
                        return gachaOrder;
                    }
                case GachaCollections.Monsters:
                    {
                        var gachaOrder = ArchipelagoClient.ServerData.MonsterGachaOrder;
                        if (gachaOrder.Count < 10 || gachaOrder.Contains(0))
                        {
                            ArchipelagoClient.ServerData.RegenerateGachaOrder();
                            gachaOrder = ArchipelagoClient.ServerData.MonsterGachaOrder;
                        }
                        return gachaOrder;
                    }
                case GachaCollections.AngelsAndDemons:
                    {
                        var gachaOrder = ArchipelagoClient.ServerData.AngelGachaOrder;
                        if (gachaOrder.Count < 10 || gachaOrder.Contains(0))
                        {
                            ArchipelagoClient.ServerData.RegenerateGachaOrder();
                            gachaOrder = ArchipelagoClient.ServerData.AngelGachaOrder;
                        }
                        return gachaOrder;
                    }
                case GachaCollections.Promotion:
                    {
                        var gachaOrder = new List<int>() { 1 };
                        if (gachaOrder.Count < 1 || gachaOrder.Contains(0))
                        {
                            ArchipelagoClient.ServerData.RegenerateGachaOrder();
                            gachaOrder = new List<int>() { 1 };
                        }
                        return gachaOrder;
                    }
            }
            throw new ArgumentOutOfRangeException();
        }

        private static string ObtainCoinName(GachaCollections currentCollection)
        {
            switch (currentCollection)
            {
                case GachaCollections.AnimalGirls:
                    {
                        return "AnimalCoin";
                    }
                case GachaCollections.Bunnys:
                    {
                        return "BunnyCoin";
                    }
                case GachaCollections.Monsters:
                    {
                        return "MonsterCoin";
                    }
                case GachaCollections.AngelsAndDemons:
                    {
                        return "AngelDemonCoin";
                    }
            }
            return "PromotionalCoin";
        }

        private static int GetCoinCountForGivenCollection(GachaCollections collection)
        {
            switch (collection)
            {
                case GachaCollections.AnimalGirls:
                    {
                        return SwitchDatabase.instance.getInt("APAnimalCoin");
                    }
                case GachaCollections.Bunnys:
                    {
                        return SwitchDatabase.instance.getInt("APBunnyCoin");
                    }
                case GachaCollections.Monsters:
                    {
                        return SwitchDatabase.instance.getInt("APMonsterCoin");
                    }
                case GachaCollections.AngelsAndDemons:
                    {
                        return SwitchDatabase.instance.getInt("APAngelDemonCoin");
                    }
            }
            return SwitchDatabase.instance.getInt("APPromotionalCoin");
        }

        private static void RemoveTokenGivenCollection(GachaCollections collection)
        {
            switch (collection)
            {
                case GachaCollections.AnimalGirls:
                    {
                        var coin = SwitchDatabase.instance.getInt("APAnimalCoin");
                        var reducedCoin = Math.Max(0, coin - 1);
                        SwitchDatabase.instance.setInt("APAnimalCoin", reducedCoin);
                        return;
                    }
                case GachaCollections.Bunnys:
                    {
                        var coin = SwitchDatabase.instance.getInt("APBunnyCoin");
                        var reducedCoin = Math.Max(0, coin - 1);
                        SwitchDatabase.instance.setInt("APBunnyCoin", reducedCoin);
                        return;
                    }
                case GachaCollections.Monsters:
                    {
                        var coin = SwitchDatabase.instance.getInt("APMonsterCoin");
                        var reducedCoin = Math.Max(0, coin - 1);
                        SwitchDatabase.instance.setInt("APMonsterCoin", reducedCoin);
                        return;
                    }
                case GachaCollections.AngelsAndDemons:
                    {
                        var coin = SwitchDatabase.instance.getInt("APAngelDemonCoin");
                        var reducedCoin = Math.Max(0, coin - 1);
                        SwitchDatabase.instance.setInt("APAngelDemonCoin", reducedCoin);
                        return;
                    }
                case GachaCollections.Promotion:
                    {
                        var coin = SwitchDatabase.instance.getInt("APPromotionalCoin");
                        var reducedCoin = Math.Max(0, coin - 1);
                        SwitchDatabase.instance.setInt("APPromotionalCoin", reducedCoin);
                        return;
                    }
            }
        }

        private enum GachaPhase
        {
            INPUT_PAY,
            PAID,
            INPUT_OPEN_PRIZE,
            PRIZE_OPENING,
            INPUT_CLOSE_PRIZE
        }
    }
}