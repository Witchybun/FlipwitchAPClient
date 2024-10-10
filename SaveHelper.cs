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
            var newGameVerification = 0;
            if (File.Exists(savePath))
            {
                using StreamReader reader = new StreamReader(savePath);
                string text = reader.ReadToEnd();
                var loadedSave = JsonConvert.DeserializeObject<APSaveData>(text);
                newGameVerification = loadedSave.Seed;
            }
            var newAPSaveData = new APSaveData(ArchipelagoClient.ServerData.Index, ArchipelagoClient.ServerData.Seed,
            ArchipelagoClient.ServerData.CheckedLocations, ArchipelagoClient.ServerData.CompletedGacha,
            ArchipelagoClient.ServerData.BarrierCount);
            Plugin.Logger.LogInfo(newAPSaveData.Seed);
            string json = JsonConvert.SerializeObject(newAPSaveData);
            File.WriteAllText(savePath, json);
            Plugin.Logger.LogInfo("Save complete!");
            if (newGameVerification != newAPSaveData.Seed)
            {
                // We may be in a situation where this is a new save.  We should check.
                GenericMethods.HandleLocationDifference();
                GenericMethods.HandleReceivedItems();
                GenericMethods.allowingOutsideItems = false;
                Plugin.ArchipelagoClient.SaveQueueState();
            }
        }

        public static void ReadSave(int Save_Slot)
        {
            try
            {

                if (ArchipelagoClient.IsInGame)
                {
                    return; // Don't keep spam loading in situations it isn't relevant; causes data loss.
                }
                Plugin.Logger.LogInfo($"Reading save {Save_Slot}");
                var dir = Application.absoluteURL + "ArchSaves/";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var loadedSave = GrabSaveDataForSlot(Save_Slot);
                ArchipelagoClient.ServerData.Index = loadedSave.Index;
                ArchipelagoClient.ServerData.Seed = loadedSave.Seed;
                ArchipelagoClient.ServerData.CheckedLocations = loadedSave.CheckedLocations;
                ArchipelagoClient.ServerData.CompletedGacha = loadedSave.ObtainedGachas;
                GenericMethods.HandleLocationDifference();
                GenericMethods.allowingOutsideItems = false;
                Plugin.ArchipelagoClient.LoadQueueState();
                GenericMethods.SyncItemsOnLoad();
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
                Plugin.Logger.LogInfo($"Save Data: Index 0 | Seed: -1");
                return new APSaveData();
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
        public Dictionary<GachaCollections, List<int>> ObtainedGachas;
        public int BarrierCount;

        public APSaveData()
        {
            Index = 0;
            Seed = -1;
            CheckedLocations = new();
            ObtainedGachas = new();
            BarrierCount = 0;
        }
        public APSaveData(int index, int seed, List<long> checkedLocations, Dictionary<GachaCollections, List<int>> obtainedGachas, int barrierCount)
        {
            Index = index;
            Seed = seed;
            CheckedLocations = checkedLocations;
            ObtainedGachas = obtainedGachas;
            BarrierCount = barrierCount;
        }
    }
}