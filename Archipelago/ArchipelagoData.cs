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
    public int SlotId;
    public int TeamId;
    public string SlotName;
    public string Password;
    public List<long> CheckedLocations;

    private const string CLIENT_KEY = "client_version";
    private const string CHAOS_KEY = "shuffle_chaos_pieces";
    private const string SEED_KEY = "seed";
    private const string DEATH_KEY = "death_link";
    private const string GENDER_KEY = "starting_gender";
    private const string SHOP_KEY = "shopsanity";
    private const string PRICE_KEY = "shop_prices";
    private const string STAT_KEY = "stat_shuffle";
    private const string CRYSTAL_KEY = "crystal_teleports";
    private const string ANIMAL_KEY = "animal_order";
    private const string BUNNY_KEY = "bunny_order";
    private const string MONSTER_KEY = "monster_order";
    private const string ANGEL_KEY = "angel_order";
    private const string HINT_KEY = "hints";
    private const string GACHA_KEY = "gachapon_shuffle";
    private const string QUEST_KEY = "quest_for_sex";
    private const string FORTUNE_KEY = "path";
    private const string DOUBLE_KEY = "shuffle_double_jump";
    private const string ROLL_KEY = "shuffle_dodge";
    private const string STARTING_AREA_KEY = "starting_area";
    private const string POTSANITY_KEY = "pottery_lottery";
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
    public bool ShuffleDoubleJump { get; private set; }
    public bool ShuffleRoll { get; private set; }
    public bool Potsanity { get; private set; }
    public StartArea StartingArea { get; private set; }
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
        SlotId = -1;
        TeamId = -1;
        CheckedLocations = new();
    }

    public void InitializeOnNewGame()
    {
        CheckedLocations = new();
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
        ShuffleDoubleJump = GetSlotSetting(DOUBLE_KEY, false);
        ShuffleRoll = GetSlotSetting(ROLL_KEY, false);
        Potsanity = GetSlotSetting(POTSANITY_KEY, false);
        StartingArea = GetSlotSettings(STARTING_AREA_KEY, StartArea.Beatrice);
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
        return (Gender)(slotData.TryGetValue(key, out var value) ? Enum.Parse(typeof(Gender), value.ToString()) : GetSlotDefaultValue(key, defaultValue));
    }

    private StartArea GetSlotSettings(string key, StartArea defaultValue)
    {
        return (StartArea)(slotData.TryGetValue(key, out var value) ? Enum.Parse(typeof(StartArea), value.ToString()) : GetSlotDefaultValue(key, defaultValue));
    }

    private Gacha GetSlotSetting(string key, Gacha defaultValue)
    {
        return (Gacha)(slotData.TryGetValue(key, out var value) ? Enum.Parse(typeof(Gacha), value.ToString()) : GetSlotDefaultValue(key, defaultValue));
    }

    private Quest GetSlotSetting(string key, Quest defaultValue)
    {
        return (Quest)(slotData.TryGetValue(key, out var value) ? Enum.Parse(typeof(Quest), value.ToString()) : GetSlotDefaultValue(key, defaultValue));
    }


    private string GetSlotSetting(string key, string defaultValue)
    {
        return slotData.TryGetValue(key, out var value) ? value.ToString() : GetSlotDefaultValue(key, defaultValue);
    }

    public int GetSlotSetting(string key, int defaultValue)
    {
        return slotData.TryGetValue(key, out var value) ? (int)(long)value : GetSlotDefaultValue(key, defaultValue);
    }

    private bool GetSlotSetting(string key, bool defaultValue)
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

    public enum StartArea
    {
        Beatrice = 0,
        Goblin = 1,
        Spirit = 2,
        GhostCastle = 3,
        Jigoku = 4,
        ClubDemon = 5,
        Tengoku = 6,
        SlimeCitadel = 7,
        UmiUmi = 8,
        WitchyWoods = 11,
        Alley = 12,
        OutsideGhost = 13,
        FungalForest = 14,
        AngelicHallway = 15,
        SlimyDepths = 16,
    }
    
}