using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FlipwitchAP.Data;
using FlipwitchAP.Archipelago;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using System.Data;
using BepInEx;

namespace FlipwitchAP
{
    public class SaveHelper
    {
        public SaveHelper()
        {
            Harmony.CreateAndPatchAll(typeof(SaveHelper));
        }
        [HarmonyPatch(typeof(SwitchDatabase), "saveGame")]
        [HarmonyPostfix]
        private static void SaveGame_AlsoSaveArchipelagoState(int saveSlotIdx)
        {
            SaveData(saveSlotIdx);
        }

        [HarmonyPatch(typeof(SwitchDatabase), "loadGame")]
        [HarmonyPostfix]
        private static void LoadGame_AlsoLoadArchipelagoState(int saveSlotIdx, bool storageOnly, Action onSceneLoadComplete = null,
        Action onSceneLoadStart = null)
        {
            ReadSave(saveSlotIdx);
        }

        public static void SaveData(int saveSlot)
        {
            var dir = Application.absoluteURL + "ArchSaves/";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var savePath = Path.Combine(dir, $"Save{saveSlot}.json");
            Plugin.Logger.LogInfo($"Saving to {savePath}...");
            var newGameVerification = -1;
            if (File.Exists(savePath))
            {
                using StreamReader reader = new StreamReader(savePath);
                string text = reader.ReadToEnd();
                var loadedSave = JsonConvert.DeserializeObject<APSaveData>(text);
                newGameVerification = loadedSave.Seed;
            }
            var newAPSaveData = new APSaveData(ArchipelagoClient.ServerData.Index, ArchipelagoClient.ServerData.Seed,
            ArchipelagoClient.ServerData.CheckedLocations);
            ArchipelagoClient.ServerData.InitialIndex = ArchipelagoClient.ServerData.Index;
            string json = JsonConvert.SerializeObject(newAPSaveData);
            File.WriteAllText(savePath, json);
            Plugin.Logger.LogInfo("Save complete!");
            if (newGameVerification != newAPSaveData.Seed)
            {
                // We may be in a situation where this is a new save.  We should check.
                GenericMethods.HandleLocationDifference();
                GenericMethods.HandleReceivedItems();
            }
            GenericMethods.allowingOutsideItems = true;
        }

        public static void ReadSave(int Save_Slot)
        {
            try
            {
                Plugin.Logger.LogInfo($"Reading save {Save_Slot}");
                var dir = Application.absoluteURL + "ArchSaves/";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                GenericMethods.allowingOutsideItems = false;
                var loadedSave = GrabSaveDataForSlot(Save_Slot);
                ArchipelagoClient.ServerData.Index = loadedSave.Index;
                ArchipelagoClient.ServerData.InitialIndex = loadedSave.Index;
                ArchipelagoClient.ServerData.Seed = loadedSave.Seed;
                ArchipelagoClient.ServerData.CheckedLocations = loadedSave.CheckedLocations;
                
                GenericMethods.HandleLocationDifference();
                if (GenericMethods.hasDied)
                {
                    GenericMethods.SyncItemsOnLoad();
                }
                GenericMethods.allowingOutsideItems = true;
                return;

            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Failed to parse json for save {Save_Slot}");
                Plugin.Logger.LogError($"{ex}");
            }
        }

        public static APSaveData GrabSaveDataForSlot(int slot)
        {
            var dir = Application.absoluteURL + "ArchSaves/";
            var savePath = Path.Combine(dir, $"Save{slot}.json");
            Plugin.Logger.LogInfo($"State of file existence: {File.Exists(savePath)}");
            if (!File.Exists(savePath))
            {
                return new APSaveData(1, ArchipelagoClient.ServerData.Seed, new List<long>());
            }
            using StreamReader reader = new StreamReader(savePath);
            string text = reader.ReadToEnd();
            var loadedSave = JsonConvert.DeserializeObject<APSaveData>(text);
            Plugin.Logger.LogInfo($"Save Data: Index {loadedSave.Index} | Seed: {loadedSave.Seed}");
            return loadedSave;
        }
    }

    public class APSaveData
    {
        public int Index;
        public int Seed;
        public List<long> CheckedLocations;

        public APSaveData()
        {
            Index = 0;
            Seed = -1;
            CheckedLocations = new();
        }
        public APSaveData(int index, int seed, List<long> checkedLocations)
        {
            Index = index;
            Seed = seed;
            CheckedLocations = checkedLocations;
        }
    }
}