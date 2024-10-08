﻿using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Enums;
using FlipwitchAP.Data;
using Newtonsoft.Json;

namespace FlipwitchAP.Archipelago;

public class ArchipelagoData
{
    public string Uri;
    public int SlotId;
    public string SlotName;
    public string Password;
    public int Index;
    public Queue<ReceivedItem> StoredQueue = new();
    public Dictionary<GachaCollections, List<int>> CompletedGacha = new();
    public int BarrierCount = 0;

    public List<long> CheckedLocations;

    private string CLIENT_KEY = "client_version";
    private string SEED_KEY = "seed";
    private string DEATH_KEY = "death_link";
    private string GENDER_KEY = "starting_gender";
    private string GACHA_KEY = "gachapon";
    public string ClientVersion { get; private set; }
    public int Seed { get; set; }
    public Gender StartingGender { get; set; }
    public bool GachaOn {get; set;}
    public bool DeathLink { get; private set; }
    public SortedDictionary<long, ArchipelagoItem> ScoutedLocations = new() { };

    private Dictionary<string, object> slotData;

    public bool NeedSlotData => slotData == null;

    public ArchipelagoData()
    {
        Uri = "localhost";
        SlotName = "Player1";
        SlotId = 0;
        CheckedLocations = new();
    }

    public void InitializeOnNewGame()
    {
        CheckedLocations = new();
        Index = 0;
    }

    public void SetupSession(Dictionary<string, object> roomSlotData)
    {
        slotData = roomSlotData;
        Seed = GetSlotSetting(SEED_KEY, 0);
        ClientVersion = GetSlotSetting(CLIENT_KEY, "");
        DeathLink = GetSlotSetting(DEATH_KEY, false);
        StartingGender = GetSlotSetting(GENDER_KEY, Gender.Woman);
        GachaOn = GetSlotSetting(GACHA_KEY, false);
        StoredQueue = ArchipelagoClient.ItemsToProcess;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    private Gender GetSlotSetting(string key, Gender defaultValue)
    {
        return (Gender)(slotData.ContainsKey(key) ? Enum.Parse(typeof(Gender), slotData[key].ToString()) : GetSlotDefaultValue(key, defaultValue));
    }

    private string GetSlotSetting(string key, string defaultValue)
    {
        return slotData.ContainsKey(key) ? slotData[key].ToString() : GetSlotDefaultValue(key, defaultValue);
    }

    public int GetSlotSetting(string key, int defaultValue)
    {
        Plugin.Logger.LogInfo($"Do we have {key} in the data?  {slotData.ContainsKey(key)}");
        if (slotData.ContainsKey(key))
        {
            Plugin.Logger.LogInfo($"Value is {(int)(long)slotData[key]}");
        }
        else
        {
            Plugin.Logger.LogInfo($"Value is zeroed out.");
        }
        return slotData.ContainsKey(key) ? (int)(long)slotData[key] : GetSlotDefaultValue(key, defaultValue);
    }

    public bool GetSlotSetting(string key, bool defaultValue)
    {
        if (slotData.ContainsKey(key) && slotData[key] != null && slotData[key] is bool boolValue)
        {
            return boolValue;
        }
        if (slotData[key] is string strValue && bool.TryParse(strValue, out var parsedValue))
        {
            return parsedValue;
        }
        if (slotData[key] is int intValue)
        {
            return intValue != 0;
        }
        if (slotData[key] is long longValue)
        {
            return longValue != 0;
        }
        if (slotData[key] is short shortValue)
        {
            return shortValue != 0;
        }

        return GetSlotDefaultValue(key, defaultValue);
    }

    private T GetSlotDefaultValue<T>(string key, T defaultValue)
    {
        Plugin.Logger.LogWarning($"SlotData did not contain expected key: \"{key}\"");
        return defaultValue;
    }

    public enum Gender
    {
        Woman = 0,
        Man = 1,
    }
}