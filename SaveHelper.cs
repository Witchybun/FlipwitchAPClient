using System;
using System.Collections.Generic;
using System.IO;
using FlipwitchAP.Archipelago;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using BepInEx;

namespace FlipwitchAP
{
    public class SaveHelper
    {
        
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
            var dir = Path.Combine(Path.Combine(Paths.PluginPath, "FlipwitchAP"), $"Save{saveSlot}");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var savePath = Path.Combine(dir, $"Data.json");
            var newGameVerification = -1;
            if (File.Exists(savePath))
            {
                using StreamReader reader = new StreamReader(savePath);
                string text = reader.ReadToEnd();
                var loadedSave = JsonConvert.DeserializeObject<APSaveData>(text);
                newGameVerification = loadedSave.Seed;
            }
            var newAPSaveData = new APSaveData(ArchipelagoClient.ServerData.Seed,
            ArchipelagoClient.ServerData.CheckedLocations);
            string json = JsonConvert.SerializeObject(newAPSaveData);
            File.WriteAllText(savePath, json);
            Plugin.Logger.LogInfo("Save complete!");
            if (newGameVerification != newAPSaveData.Seed)
            {
                // We may be in a situation where this is a new save.  We should check.
                GenericMethods.HandleLocationDifference();
                ArchipelagoClient.AP.ReAddAllPreviousChecksToEnqueueDueToDying();
            }
            ArchipelagoClient.AP.allowOutsideItems = true;
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
                ArchipelagoClient.AP.allowOutsideItems = false;
                var loadedSave = GrabSaveDataForSlot(Save_Slot);
                ArchipelagoClient.ServerData.Seed = loadedSave.Seed;
                ArchipelagoClient.ServerData.CheckedLocations = loadedSave.CheckedLocations;

                GenericMethods.HandleLocationDifference();
                if (CutsceneHelper.hasDied)
                {
                    ArchipelagoClient.AP.ReAddAllPreviousChecksToEnqueueDueToDying();
                }
                ArchipelagoClient.AP.allowOutsideItems = true;

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
                return new APSaveData(ArchipelagoClient.ServerData.Seed, new List<long>());
            }
            using StreamReader reader = new StreamReader(savePath);
            string text = reader.ReadToEnd();
            var loadedSave = JsonConvert.DeserializeObject<APSaveData>(text);
            Plugin.Logger.LogInfo($"Save Data: Seed: {loadedSave.Seed}");
            return loadedSave;
        }

        public static void SaveCurrentConnectionData(string uri, string slotName, string password)
        {
            var savePath = Path.Combine(Path.Combine(Paths.PluginPath, "FlipwitchAP"), "LastConnectionInfo.json");
            var currentConnectionData = new ConnectionData(uri, slotName, password);
            string json = JsonConvert.SerializeObject(currentConnectionData);
            File.WriteAllText(savePath, json);
        }

        public static void GrabLastConnectionInfo()
        {
            var savePath = Path.Combine(Path.Combine(Paths.PluginPath, "FlipwitchAP"), "LastConnectionInfo.json");
            if (!File.Exists(savePath))
            {
                return;
            }
            using StreamReader reader = new StreamReader(savePath);
            string text = reader.ReadToEnd();
            var loadedConnectionData = JsonConvert.DeserializeObject<ConnectionData>(text);
            ArchipelagoClient.ServerData.Uri = loadedConnectionData.Uri;
            ArchipelagoClient.ServerData.SlotName = loadedConnectionData.SlotName;
            ArchipelagoClient.ServerData.Password = loadedConnectionData.Password;
        }
    }

    public class ConnectionData
    {
        public readonly string Uri;
        public readonly string SlotName;
        public readonly string Password;

        public ConnectionData(string uri, string slotName, string password)
        {
            Uri = uri;
            SlotName = slotName;
            Password = password;
        }
    }

    public class APSaveData
    {
        public readonly int Seed;
        public readonly List<long> CheckedLocations;

        public APSaveData()
        {
            Seed = -1;
            CheckedLocations = new();
        }
        public APSaveData(int seed, List<long> checkedLocations)
        {
            Seed = seed;
            CheckedLocations = checkedLocations;
        }
    }
}