using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net.Models;
using FlipwitchAP.Archipelago;
using FlipwitchAP.Data;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlipwitchAP
{
    public class GenericMethods
    {
        public static bool allowingOutsideItems = true;
        public const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        public GenericMethods()
        {
            Harmony.CreateAndPatchAll(typeof(GenericMethods));
        }

        [HarmonyPatch(typeof(MainMenuScreen), "StartNewGame")]
        [HarmonyPrefix]
        private static void StartNewGame_InitializeIndexAndLocations()
        {
            ArchipelagoClient.ServerData.InitializeOnNewGame();
        }

        [HarmonyPatch(typeof(BeginCutsceneIngame), "OnEnable")]
        [HarmonyPostfix]
        private static void OnEnable_DoAdditionalThings(BeginCutsceneIngame __instance)
        {
            switch (__instance.transform.parent.name)
            {
                case "Intro Cutscene":
                    {
                        if (ArchipelagoClient.ServerData.StartingGender == ArchipelagoData.Gender.Man)
                        {
                            SwitchDatabase.instance.playerMov.GetType().GetMethod("flipGender", Flags).Invoke(SwitchDatabase.instance.playerMov, null);
                        }
                        return;
                    }
                case "Chaos Witch Defeated For Real":
                    {
                        Plugin.ArchipelagoClient.SendVictory();
                        return;
                    }

            }
        }

        [HarmonyPatch(typeof(SaveSlotSelection), "Update")]
        [HarmonyPrefix]
        private static bool Update_DisallowLoadOfWrongSlot(SaveSlotSelection __instance)
        {
            NewInputManager instance = NewInputManager.instance;
            if (instance.Interact.pressedThisFrame || instance.Submit.pressedThisFrame)
            {
                var mode = (SaveSlotSelection.Mode)__instance.GetType().GetField("_mode", Flags).GetValue(__instance);
                if (mode == SaveSlotSelection.Mode.Continue)
                {

                    var select = (int)__instance.GetType().GetField("_selectedItemIdx", Flags).GetValue(__instance);
                    var loadedSave = SaveHelper.GrabSaveDataForSlot(select);
                    Plugin.Logger.LogInfo($"On {select}, Comparing {loadedSave.Seed} with {ArchipelagoClient.ServerData.Seed}");
                    if (loadedSave.Seed != ArchipelagoClient.ServerData.Seed)
                    {
                        Plugin.Logger.LogInfo("Test Success");
                        return false;
                    }
                    return true;
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(MainMenu), "Update")]
        [HarmonyPrefix]
        private static bool Update_DisallowPlayIfNoConnection(MainMenu __instance)
        {
            if (ArchipelagoClient.Authenticated)
            {
                return true;
            }
            NewInputManager instance = NewInputManager.instance;
            if (instance.Interact.pressedThisFrame || instance.Submit.pressedThisFrame)
            {
                var select = (int)__instance.GetType().GetField("_selectedItemIdx", Flags).GetValue(__instance);

                switch (select)
                {
                    case 0:
                        AkSoundEngine.PostEvent("ui_fail", __instance.gameObject);
                        return false;
                    case 1:
                        AkSoundEngine.PostEvent("ui_fail", __instance.gameObject);
                        return false;
                    case 5:
                        Plugin.ArchipelagoClient.Cleanup();
                        return true;
                }
                return true;
            }
            return true;
        }

        [HarmonyPatch(typeof(PlayerMovement), "inflictDamage", argumentTypes: new System.Type[] { typeof(int), typeof(Transform) })]
        [HarmonyPrefix]
        private static void InflictDamage_ReduceDamageBasedOnBarrier(ref int damage, Transform enemy)
        {
            for (var i = 0; i < SwitchDatabase.instance.getInt("APBarrier"); i++)
            {
                damage /= 2;
            }
        }

        [HarmonyPatch(typeof(Cutscene), "calculateUnlockedRewards")]
        [HarmonyPrefix]
        private static bool CalculateUnlockedRewards_CalculateUsingOwnUnlockCount(Cutscene __instance)
        {
            int playerPeachChargesCap = SwitchDatabase.instance.getInt("APTotalPeachCharges");
            int num = SwitchDatabase.instance.getInt("SexualExperienceCount") / 4;
            int @int = SwitchDatabase.instance.getInt("PendingPeachCharges");
            SwitchDatabase.instance.setInt("AppliedAndPendingPeaches", playerPeachChargesCap + num);
            SwitchDatabase.instance.setInt("PendingPeachCharges", num - playerPeachChargesCap);
            Debug.Log("SexualExperienceCount: " + SwitchDatabase.instance.getInt("SexualExperienceCount"));
            if (num >= 2)
            {
                SwitchDatabase.instance.setInt("PendingWandLevel", 1);
            }
            if (num >= 6)
            {
                SwitchDatabase.instance.setInt("PendingWandLevel", 2);
            }
            if (@int < SwitchDatabase.instance.getInt("PendingPeachCharges"))
            {
                SwitchDatabase.instance.triggerUpgradePendingPopup();
            }
            SwitchDatabase.instance.upgradePendingPopup.updatePendingPopupSymbol();
            return false;
        }

        public static void SyncItemsOnLoad()
        {
            if (Plugin.ArchipelagoClient.IsThereIndexMismatch(out var items))
            {
                allowingOutsideItems = false;
                HandleMissingItems(items);
            }
            allowingOutsideItems = true;
        }

        public static void HandleReceivedItems()
        {
            if (!allowingOutsideItems)
            {
                return;
            }
            bool itemsNeedProcessing = true;
            while (itemsNeedProcessing)
            {
                if (ArchipelagoClient.ItemsToProcess.Count() == 0)
                {
                    itemsNeedProcessing = false;
                    break;
                }
                // Reflects the old method in OnItemReceived
                // If we can get a better UI made, this can be toned down some.
                var item = ArchipelagoClient.ItemsToProcess.Dequeue();
                Plugin.Logger.LogInfo($"Determining whether or not to give the player {item.ItemName}.");
                Plugin.Logger.LogInfo($"{item.Index} vs {ArchipelagoClient.ServerData.Index}.");
                if (item.Index < ArchipelagoClient.ServerData.Index)
                {
                    continue;
                }
                ItemHelper.GiveFlipwitchItem(item);
                ArchipelagoClient.ServerData.Index++;
            }
        }

        public static void HandleMissingItems(List<ItemInfo> items)
        {
            var num = 0;
            foreach (var item in items)
            {
                num++;
                if (num < ArchipelagoClient.ServerData.Index)
                {
                    continue;
                }
                var receivedItem = new ReceivedItem(item, num);
                ArchipelagoClient.ItemsToProcess.Enqueue(receivedItem);
                Plugin.Logger.LogInfo($"{item.ItemName} wasn't in the Queue, adding it back.");
            }
        }

        public static void HandleLocationDifference()
        {
            var locationsToVerify = Plugin.ArchipelagoClient.AllLocationsCompletedNotedByServer();
            foreach (var location in locationsToVerify)
            {
                if (ArchipelagoClient.ServerData.CheckedLocations.Contains(location))
                {
                    continue;
                }
                ArchipelagoClient.ServerData.CheckedLocations.Add(location);
                var switchToFlip = FlipwitchLocations.IDToLocation[location].SwitchToFlip;
                if (switchToFlip != "")
                {
                    SwitchDatabase.instance.setInt(switchToFlip, 1);
                    Plugin.Logger.LogInfo($"{switchToFlip} set to {SwitchDatabase.instance.getInt(switchToFlip)}");
                }
            }
        }

        public static void PatchSceneSwitchTriggers(string sceneName)
        {
            switch (sceneName)
            {
                case "WitchyWoods_Final":
                    {
                        var world = GameObject.Find("WitchyWoods").transform.Find("Grid").Find("World");
                        var peachCutsceneData = world.Find("2_MiniBridge").Find("LevelData").Find("Peachy Peach Cutscene Manager").GetComponent<SetGameObjectEnabledOnSwitch>();
                        peachCutsceneData.switchName = "APPeachItemGiven";
                        var crystalCutsceneData = world.Find("16_FairyRuins").Find("LevelData").GetChild(1).GetComponent<LevelStateManagerSingleSwitch>();
                        crystalCutsceneData.switchName = "APGreatFairyStory";
                        break;
                    }
                case "Spirit_City_Final":
                    {
                        var stairwellSwitches = GameObject.Find("World").transform.Find("4_CityStairwell").Find("LevelData").Find("Switch Driven Data - Crystal Gone");
                        stairwellSwitches.GetComponent<LevelStateManagerSingleSwitch>().switchName = "APCityCrystalDestroyed";
                        var cemeterySwitches = GameObject.Find("World").transform.Find("10_CemeteryA").Find("LevelData").Find("Switch Driven Data");
                        cemeterySwitches.GetComponent<LevelStateManagerSingleSwitch>().switchName = "APCityCrystalDestroyed";
                        if (SwitchDatabase.instance.getInt("APCityCrystalDestroyed") > 0)
                        {
                            stairwellSwitches.GetChild(0).gameObject.SetActive(false);
                            stairwellSwitches.GetChild(1).gameObject.SetActive(false);
                            cemeterySwitches.GetChild(0).gameObject.SetActive(false);
                            cemeterySwitches.GetChild(1).gameObject.SetActive(false);
                        }
                        var bestGirlSwitch = GameObject.Find("World").transform.Find("CabaretCafe").Find("LevelData").Find("Cabaret Owner Complete Manager");
                        bestGirlSwitch.GetComponent<SetGameObjectEnabledOnSwitch>().switchName = "APCabaretComplete";
                        return;
                    }
            }
        }

        public static void SoftlockSparer(string sceneName)
        {
            switch (sceneName)
            {
                case "Jigoku_Main":
                    {
                        var world = GameObject.Find("Grid").transform.Find("World");
                        var antHell = world.Find("16_AntHill").Find("LevelData");
                        var bouncy = antHell.Find("Bouncy_Honey (2)").gameObject;
                        var antBounce = GameObject.Instantiate(bouncy);
                        antBounce.transform.parent = antHell;
                        antBounce.transform.position = new Vector3(142.8934f, -95.395f, 0f);

                        var firstDrop = world.Find("4_FirstDrop").Find("LevelData");
                        var firstBounce = GameObject.Instantiate(bouncy);
                        firstBounce.transform.parent = firstDrop;
                        firstBounce.transform.position = new Vector3(-87.1152f, -38.395f, 0f);
                        firstBounce.GetComponent<BouncyHoney>().force = 40;

                        var orangeRoom = world.Find("ClubDemon").Find("53_OrangeStairs").Find("LevelData");
                        orangeRoom.Find("Bouncy_Honey (33)").gameObject.GetComponent<BouncyHoney>().force = 50;

                        var purpleOrangeBouncy = GameObject.Instantiate(bouncy);
                        var purpleOrangeRoom = world.Find("ClubDemon").Find("40_PurpleOrange").Find("LevelData");
                        purpleOrangeBouncy.transform.parent = purpleOrangeRoom;
                        purpleOrangeBouncy.transform.position = new Vector3(-9.99f, -225.4299f, 0.1f);
                        purpleOrangeBouncy.gameObject.GetComponent<BouncyHoney>().force = 55;
                        return;
                    }
            }
        }
    }
}