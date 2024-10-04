using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FlipwitchAP.Data;
using FlipwitchAP.Archipelago;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;

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
        private static void LoadGame_AlsoLoadArchipelagoState(int saveSlotIdx, bool storageOnly, Action onSceneLoadComplete = null, Action onSceneLoadStart = null)
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

            var newAPSaveData = new APSaveData()
            {
                Index = ArchipelagoClient.ServerData.Index,
                CheckedLocations = ArchipelagoClient.ServerData.CheckedLocations,
                ObtainedItems = ArchipelagoClient.ServerData.ReceivedItems,
                ScoutedLocations = ArchipelagoClient.ServerData.ScoutedLocations,
            };
            string json = JsonConvert.SerializeObject(newAPSaveData);
            File.WriteAllText(savePath, json);
            Plugin.Logger.LogInfo("Save complete!");
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
                var savePath = Path.Combine(dir, $"Save{Save_Slot}.json");
                if (File.Exists(savePath))
                {
                    using StreamReader reader = new StreamReader(savePath);
                    string text = reader.ReadToEnd();
                    var loadedSave = JsonConvert.DeserializeObject<APSaveData>(text);
                    ArchipelagoClient.ServerData.Index = loadedSave.Index;
                    ArchipelagoClient.ServerData.CheckedLocations = loadedSave.CheckedLocations;
                    ArchipelagoClient.ServerData.ReceivedItems = loadedSave.ObtainedItems;
                    ArchipelagoClient.ServerData.ScoutedLocations = loadedSave.ScoutedLocations;
                    Plugin.ArchipelagoClient.SyncItemsReceivedOnLoad();
                    return;
                }

                Plugin.Logger.LogError("SAVE not found");

            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Failed to parse json for save {Save_Slot}");
                Plugin.Logger.LogError($"{ex}");
            }
        }
    }

    internal class APSaveData
    {
        public int Index;
        public List<long> CheckedLocations;
        public List<ReceivedItem> ObtainedItems;
        public SortedDictionary<long, ArchipelagoItem> ScoutedLocations;
    }
}