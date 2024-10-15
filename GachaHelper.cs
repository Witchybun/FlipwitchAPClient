using System;
using System.Collections.Generic;
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
            Plugin.Logger.LogInfo(currentCollection);
            var coinName = ObtainCoinName(currentCollection);
            int gachaTokenCount = SwitchDatabase.instance.getInt("AP" + coinName);
            Plugin.Logger.LogInfo($"{coinName}: {gachaTokenCount}");
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
        private static void LoadMachine_ReloadMachineBasedOnLocationsNotItems(GachaSceneManager __instance, GachaCollections collection)
        {
            if (!ArchipelagoClient.ServerData.GachaOn)
            {
                return;
            }
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
                var removeBalls = 0;
                for (var k = 0; k < currentGachaState; k++)
                {
                    removeBalls += 1;
                    currentGachaList.RemoveAt(0);
                }
                var initialBalls = gachaCollection.gachas.Count;
                __instance.GetType().GetField("ballsRemaining", GenericMethods.Flags).SetValue(__instance, initialBalls);
                __instance.GetType().GetField("remainingGachasIndexes", GenericMethods.Flags).SetValue(__instance, new List<int>());
                __instance.sticker.sprite = gachaCollection.gachaSticker;


                __instance.GetType().GetField("remainingGachasIndexes", GenericMethods.Flags).SetValue(__instance, currentGachaList);
                var ballsRemaining = initialBalls - removeBalls;
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
            var ballCountAnimParam = (string)__instance.GetType().GetField("ballCountAnimParam", GenericMethods.Flags).GetValue(__instance);
            __instance.gachaMachineAnim.SetInteger(ballCountAnimParam, ballsRemaining);
            var remainingGachasIndexes = (List<int>)__instance.GetType().GetField("remainingGachasIndexes", GenericMethods.Flags).GetValue(__instance);
            var currentCollection = (GachaCollections)__instance.GetType().GetField("currentCollection", GenericMethods.Flags).GetValue(__instance);
            var chosenGachaName = currentCollection.ToString();
            var currentGachaState = SwitchDatabase.instance.getInt(chosenGachaName);
            int randomIndex = currentGachaState;
            var closeGachaMethod = __instance.GetType().GetMethod("closeGachaScreen", GenericMethods.Flags);
            foreach (GachaCollection gachaCollection in SwitchDatabase.instance.gachaCollections)
            {
                if (gachaCollection.collection == currentCollection)
                {
                    var PickedGachaList = ObtainRandomOrderList(currentCollection);
                    Plugin.Logger.LogInfo($"Chose {PickedGachaList.ToString()} with size {PickedGachaList.Count}");
                    int index = PickedGachaList[randomIndex] - 1;
                    Plugin.Logger.LogInfo($"From {randomIndex}, we pull {index}");
                    var chosenGacha = gachaCollection.gachas[index];
                    Plugin.Logger.LogInfo($"This gets us {chosenGacha.animationName}");
                    SwitchDatabase.instance.setBool($"{gachaCollection.collection}_{chosenGacha.number}", value: true);
                    __instance.GetType().GetField("currentPrize", GenericMethods.Flags).SetValue(__instance, chosenGacha);
                    var animation = gachaCollection.gachas[index].animationName;
                    var gachaLocation = FlipwitchLocations.GachaLocations[animation];
                    LocationHelper.SendLocationGivenLocationDataSendingGift(gachaLocation);
                    SwitchDatabase.instance.setInt(chosenGachaName, currentGachaState + 1);
                    return false;
                }
            }
            Plugin.Logger.LogError($"Unable to find a matching gacha collection for {currentCollection}.");
            SwitchDatabase.instance.addTokenToTokenCount(1);
            closeGachaMethod.Invoke(__instance, null);
            return false;
        }

        //Update the update...eugh.
        [HarmonyPatch(typeof(GachaSceneManager), "Update")]
        [HarmonyPrefix]
        private static bool Update_AlterGachaBasedOnNewCoinSystem(GachaSceneManager __instance)
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
                    var closePrizeCooldown = (float)__instance.GetType().GetField("closePrizeCooldown", GenericMethods.Flags).GetValue(__instance);
                    if (closePrizeCooldown > 0f)
                    {
                        closePrizeCooldown -= Time.unscaledDeltaTime;
                        if (!(closePrizeCooldown > 0f))
                        {
                            closePrizeCooldown = 0f;
                            apPhase = GachaPhase.INPUT_CLOSE_PRIZE;
                        }
                    }
                    __instance.GetType().GetField("closePrizeCooldown", GenericMethods.Flags).SetValue(__instance, closePrizeCooldown);
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

        private static List<int> ObtainRandomOrderList(GachaCollections currentCollection)
        {
            switch (currentCollection)
            {
                case GachaCollections.AnimalGirls:
                    {
                        return ArchipelagoClient.ServerData.AnimalGachaOrder;
                    }
                case GachaCollections.Bunnys:
                    {
                        return ArchipelagoClient.ServerData.AnimalGachaOrder;
                    }
                case GachaCollections.Monsters:
                    {
                        return ArchipelagoClient.ServerData.MonsterGachaOrder;
                    }
                case GachaCollections.AngelsAndDemons:
                    {
                        return ArchipelagoClient.ServerData.AngelGachaOrder;
                    }
            }
            return new List<int>() { 0 };
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
            Plugin.Logger.LogInfo($"Removing coin from {collection.ToString()}");
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