using System;
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
    public int InitialIndex;
    public Queue<ReceivedItem> StoredQueue = new();
    public List<long> CheckedLocations;

    private string CLIENT_KEY = "client_version";
    private string SEED_KEY = "seed";
    private string DEATH_KEY = "death_link";
    private string GENDER_KEY = "starting_gender";
    private string PRICE_KEY = "shop_prices";
    private string GACHA_KEY = "gachapon";
    private string ANIMAL_KEY = "animal_order";
    private string BUNNY_KEY = "bunny_order";
    private string MONSTER_KEY = "monster_order";
    private string ANGEL_KEY = "angel_order";
    public string ClientVersion { get; private set; }
    public int Seed { get; set; }
    public Gender StartingGender { get; set; }
    public int ShopPrices {get; set;}
    public bool GachaOn {get; set;}
    public bool DeathLink { get; private set; }
    public List<int> AnimalGachaOrder {get; private set; }
    public List<int> BunnyGachaOrder {get; private set; }
    public List<int> MonsterGachaOrder {get; private set; }
    public List<int> AngelGachaOrder {get; private set; }
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
        ShopPrices = GetSlotSetting(PRICE_KEY, 100);
        GachaOn = GetSlotSetting(GACHA_KEY, false);
        var animalOrderData = GetSlotSetting(ANIMAL_KEY, "");
        AnimalGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(animalOrderData));
        Plugin.Logger.LogInfo($"We have AnimalGacha with set [{string.Join(", ", AnimalGachaOrder)}]");
        var bunnyOrderData = GetSlotSetting(BUNNY_KEY, "");
        BunnyGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(bunnyOrderData));
        var monsterOrderData = GetSlotSetting(MONSTER_KEY, "");
        MonsterGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(monsterOrderData));
        var angelOrderData = GetSlotSetting(ANGEL_KEY, "");
        AngelGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(angelOrderData));
        StoredQueue = ArchipelagoClient.ItemsToProcess;
    }

    // Why...?
    public void RegenerateGachaOrder()
    {
        var animalOrderData = GetSlotSetting(ANIMAL_KEY, "");
        AnimalGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(animalOrderData));
        Plugin.Logger.LogInfo($"We have AnimalGacha with set [{string.Join(", ", AnimalGachaOrder)}]");
        var bunnyOrderData = GetSlotSetting(BUNNY_KEY, "");
        BunnyGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(bunnyOrderData));
        var monsterOrderData = GetSlotSetting(MONSTER_KEY, "");
        MonsterGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(monsterOrderData));
        var angelOrderData = GetSlotSetting(ANGEL_KEY, "");
        AngelGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(angelOrderData));
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

    private static List<int> ProcessGachaList(List<string> givenList)
    {
        var numList = new List<int>();
        foreach (var gacha in givenList)
        {
            var num = int.Parse(gacha.Split('#')[1]);
            numList.Add(num);
        }
        var listedItems = string.Join(", ", numList);
        Plugin.Logger.LogInfo(listedItems);
        return numList;
    }

    public enum Gender
    {
        Woman = 0,
        Man = 1,
    }
}