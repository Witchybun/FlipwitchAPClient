using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using FlipwitchAP.Data;
using Newtonsoft.Json;

namespace FlipwitchAP.Archipelago;

public class ArchipelagoData
{
    public string Uri;
    public static int SlotId;
    public string SlotName;
    public string Password;
    public int Index;
    public int InitialIndex;
    public List<long> CheckedLocations;

    private string CLIENT_KEY = "client_version";
    private string CHAOS_KEY = "shuffle_chaos_pieces";
    private string SEED_KEY = "seed";
    private string DEATH_KEY = "death_link";
    private string GENDER_KEY = "starting_gender";
    private string SHOP_KEY = "shopsanity";
    private string PRICE_KEY = "shop_prices";
    private string STAT_KEY = "stat_shuffle";
    private string CRYSTAL_KEY = "crystal_teleports";
    private string ANIMAL_KEY = "animal_order";
    private string BUNNY_KEY = "bunny_order";
    private string MONSTER_KEY = "monster_order";
    private string ANGEL_KEY = "angel_order";
    private string HINT_KEY = "hints";
    private string GACHA_KEY = "gachapon_shuffle";
    private string QUEST_KEY = "quest_for_sex";
    private string FORTUNE_KEY = "path";
    public string ClientVersion { get; private set; }
    public int Seed { get; set; }
    public Gender StartingGender { get; private set; }
    public bool ShuffleChaosPieces { get; private set; }
    public int ShopPrices { get; private set;}
    public bool CrystalTeleport { get; private set; }
    public Quest QuestForSex { get; private set; }
    public Gacha Gachapon { get; private set; }
    public bool Shopsanity { get; private set; }
    public bool Statshuffle { get; private set; }
    public bool DeathLink { get; private set; }
    public List<int> AnimalGachaOrder { get; private set; }
    public List<int> BunnyGachaOrder { get; private set; }
    public List<int> MonsterGachaOrder { get; private set; }
    public List<int> AngelGachaOrder { get; private set; }
    public Dictionary<string, string> HintLookup { get; private set; }
    public Dictionary<string, string> FortuneTellerLookup { get; private set; }
    public bool IsTherePlaythroughGenerated { get; private set; }
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
        ShuffleChaosPieces = GetSlotSetting(CHAOS_KEY, false);
        DeathLink = GetSlotSetting(DEATH_KEY, false);
        StartingGender = GetSlotSetting(GENDER_KEY, Gender.Woman);
        DeathLink = GetSlotSetting(DEATH_KEY, false);
        ShopPrices = GetSlotSetting(PRICE_KEY, 100);
        CrystalTeleport = GetSlotSetting(CRYSTAL_KEY, false);
        QuestForSex = GetSlotSetting(QUEST_KEY, Quest.Quest);
        Gachapon = GetSlotSetting(GACHA_KEY, Gacha.Off);
        Shopsanity = GetSlotSetting(SHOP_KEY, false);
        Statshuffle = GetSlotSetting(STAT_KEY, true);
        var animalOrderData = GetSlotSetting(ANIMAL_KEY, "");
        AnimalGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(animalOrderData));
        var bunnyOrderData = GetSlotSetting(BUNNY_KEY, "");
        BunnyGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(bunnyOrderData));
        var monsterOrderData = GetSlotSetting(MONSTER_KEY, "");
        MonsterGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(monsterOrderData));
        var angelOrderData = GetSlotSetting(ANGEL_KEY, "");
        AngelGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(angelOrderData));
        var hintData = GetSlotSetting(HINT_KEY, "");
        HintLookup = JsonConvert.DeserializeObject<Dictionary<string, string>>(hintData);
        var pathData = GetSlotSetting(FORTUNE_KEY, "");
        FortuneTellerLookup = JsonConvert.DeserializeObject<Dictionary<string, string>>(pathData);
        IsTherePlaythroughGenerated = FortuneTellerLookup.Any();
    }

    // Why...?
    public void RegenerateGachaOrder()
    {
        var animalOrderData = GetSlotSetting(ANIMAL_KEY, "");
        AnimalGachaOrder = ProcessGachaList(JsonConvert.DeserializeObject<List<string>>(animalOrderData));
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

    private Gacha GetSlotSetting(string key, Gacha defaultValue)
    {
        return (Gacha)(slotData.ContainsKey(key) ? Enum.Parse(typeof(Gacha), slotData[key].ToString()) : GetSlotDefaultValue(key, defaultValue));
    }

    private Quest GetSlotSetting(string key, Quest defaultValue)
    {
        return (Quest)(slotData.ContainsKey(key) ? Enum.Parse(typeof(Quest), slotData[key].ToString()) : GetSlotDefaultValue(key, defaultValue));
    }


    private string GetSlotSetting(string key, string defaultValue)
    {
        return slotData.ContainsKey(key) ? slotData[key].ToString() : GetSlotDefaultValue(key, defaultValue);
    }

    public int GetSlotSetting(string key, int defaultValue)
    {
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
        return numList;
    }

    public enum Gender
    {
        Woman = 0,
        Man = 1,
    }

    public enum Gacha
    {
        Off = 0,
        Coins = 1,
        All = 2
    }

    public enum Quest
    {
        Off = 0,
        Sensei = 1,
        Quest = 2,
        All = 3
    }
}