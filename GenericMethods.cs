using System;
using System.Reflection;
using FlipwitchAP.Archipelago;
using FlipwitchAP.Data;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace FlipwitchAP
{
    public class GenericMethods
    {
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
                            SwitchDatabase.instance.playerMov.GetType().GetMethod("flipGender", Flags)?.Invoke(SwitchDatabase.instance.playerMov, null);
                        }
                        SwitchDatabase.instance.givePlayerItem("GoblinCrystal", true);
                        SwitchDatabase.instance.givePlayerItem("PortablePortal", true);
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
        private static bool Update_DisallowLoadOfWrongSlot(SaveSlotSelection __instance, 
            ref SaveSlotSelection.Mode ____mode, ref int ____selectedItemIdx)
        {
            NewInputManager instance = NewInputManager.instance;
            if (!instance.Interact.pressedThisFrame && !instance.Submit.pressedThisFrame) return true;
            if (____mode != SaveSlotSelection.Mode.Continue) return true;
            var loadedSave = SaveHelper.GrabSaveDataForSlot(____selectedItemIdx);
            if (loadedSave.Seed == ArchipelagoClient.ServerData.Seed) return true;
            Plugin.Logger.LogWarning($"Slot seed is {loadedSave.Seed}, but server suggests {ArchipelagoClient.ServerData.Seed}!");
            return false;
        }

        [HarmonyPatch(typeof(MainMenu), "Update")]
        [HarmonyPrefix]
        private static bool Update_DisallowPlayIfNoConnection(MainMenu __instance, ref int ____selectedItemIdx)
        {
            if (ArchipelagoClient.Authenticated)
            {
                return true;
            }
            var instance = NewInputManager.instance;
            if (!instance.Interact.pressedThisFrame && !instance.Submit.pressedThisFrame) return true;
            switch (____selectedItemIdx)
            {
                case 0:
                case 1:
                    AkSoundEngine.PostEvent("ui_fail", __instance.gameObject);
                    return false;
                case 5:
                    Plugin.ArchipelagoClient.Cleanup();
                    break;
            }
            return true;
        }

        [HarmonyPatch(typeof(LevelActivator), "OnTriggerStay2D")]
        [HarmonyPrefix]
        private static void OnTriggerStay2D_DocumentLevelForDamageCalculations(LevelActivator __instance,
            Collider2D collision)
        {
            if (__instance.levelActivated || !(collision.name == "player") || SwitchDatabase.instance.levelLoadInProgess)
            {
                return;
            }
            if (SwitchDatabase.instance.currentScene == "MainMenu") return;
            var area = EnemyDamageModifier.DetermineAreaGivenLevel();
            if (area is "NONE" or "Chaos Castle")
            {
                return;
            }
            if (ArchipelagoClient.ServerData.AreaOrder.ContainsKey(area)) return;
            var count = ArchipelagoClient.ServerData.AreaOrder.Count;
            ArchipelagoClient.ServerData.AreaOrder[area] = count;
            EnemyDamageModifier.AssignDamageMultiplierForAreaGivenOrder(area);
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
        
        public static void CreateOrModifyHoneyBlocksForDifferentStarts(string scene)
        {
            switch (ArchipelagoClient.ServerData.StartingArea)
            {
                case ArchipelagoData.StartArea.Tengoku:
                {
                    if (scene != "FungalForest_Main") return;
                    var honeyBlock = GameObject.Find("World/44_LargeTowerRoom/LevelData/Bouncy_Honey (3)");
                    var bounce = honeyBlock.GetComponent<BouncyHoney>();
                    bounce.force = 65f;
                    break;
                }
                case ArchipelagoData.StartArea.SlimeCitadel:
                {
                    if (scene != "FungalForest_Main") return;
                    var foundBlock = GameObject.Find("World/44_LargeTowerRoom/LevelData/Bouncy_Honey (3)");
                    var plummet = GameObject.Find("World/44_LargeTowerRoom/LevelData");
                    var plummetBouncy = Object.Instantiate(foundBlock, plummet.transform);
                    plummetBouncy.transform.position = new Vector3(339.0535f, -49.2658f, 0f);
                    plummetBouncy.GetComponent<BouncyHoney>().force = 80;
                    return;
                }
            }
        }
    }
}