using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net.Models;
using FlipwitchAP.Archipelago;
using FlipwitchAP.Data;
using HarmonyLib;
using UnityEngine;

namespace FlipwitchAP
{
    public class GenericMethods
    {
        public static bool allowingOutsideItems = true;
        public const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

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
                    if (loadedSave.Seed != ArchipelagoClient.ServerData.Seed)
                    {
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
            var amount = Math.Min(2, SwitchDatabase.instance.getInt("APBarrier"));
            damage -= amount * damage / 4;
        }

        

        public static void SyncItemsOnLoad()
        {
            var items = Plugin.ArchipelagoClient.GetAllSentItems();
            HandleMissingItems(items);
            CutsceneHelper.hasDied = false;
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
            var switchDictionary = new Dictionary<string, int>();
            foreach (var item in items)
            {
                var switchName = "AP" + item.ItemName.Replace(" ", "") + "ItemCount";
                var currentAmount = SwitchDatabase.instance.getInt(switchName);
                if (!switchDictionary.ContainsKey(switchName))
                {
                    switchDictionary[switchName] = 0;
                }
                Plugin.Logger.LogInfo($"{switchDictionary[switchName]} vs {currentAmount}");
                if (switchDictionary[switchName] < currentAmount)
                {
                    switchDictionary[switchName] += 1;
                    continue;
                }
                switchDictionary[switchName] += 1;
                SwitchDatabase.instance.setInt(switchName, currentAmount + 1);
                ItemHelper.GiveFlipwitchItem(item.ItemName, false);
                ArchipelagoClient.ServerData.Index++;
                Plugin.Logger.LogInfo($"Gave back {item.ItemName}");
                continue;
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
                case "Tengoku_Final":
                    {
                        var world = GameObject.Find("World").transform.Find("Tengoku");
                        var freeBouncy = world.Find("4_CloudUp").Find("LevelData").Find("Bouncy_Honey (2)");
                        var extraCloudBouncy = GameObject.Instantiate(freeBouncy);
                        extraCloudBouncy.transform.parent = world.Find("4_CloudUp").Find("LevelData");
                        extraCloudBouncy.transform.position = new Vector3(23.0589f, 203.6128f, 0f);
                        return;
                    }
                case "FungalForest_Main":
                    {
                        var world = GameObject.Find("World").transform;
                        var freeBouncy = world.Find("19_MushroomClimb").Find("LevelData").Find("Bouncy_Honey (8)");
                        var plummetBouncy = GameObject.Instantiate(freeBouncy);
                        var plummet = world.Find("18_Plummet").Find("LevelData");
                        plummetBouncy.transform.parent = plummet;
                        plummetBouncy.transform.position = new Vector3(339.0535f, -49.2658f, 0f);
                        plummetBouncy.GetComponent<BouncyHoney>().force = 80;
                        return;
                    }
                case "UmiUmi_Main":
                    {
                        var world = GameObject.Find("World").transform;
                        var rewardDropBounce = world.Find("38_RewardDrop").Find("LevelData").Find("Bouncy_Honey (10)");
                        rewardDropBounce.GetComponent<BouncyHoney>().force = 60;
                        world.Find("30_DangerDrop").Find("LevelData").Find("Bouncy_Honey (14)").GetComponent<BouncyHoney>().force = 70;
                        var deepDropBounce = GameObject.Instantiate(rewardDropBounce);
                        var deepDropSpot = world.Find("11_DeepDrop").Find("LevelData");
                        deepDropBounce.parent = deepDropSpot;
                        deepDropBounce.position = new Vector3(-152.485f, -48.395f, 0f);
                        deepDropBounce.GetComponent<BouncyHoney>().force = 35;
                        return;
                    }
            }
        }
    }
}