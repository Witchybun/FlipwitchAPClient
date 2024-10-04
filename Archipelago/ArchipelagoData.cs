using System.Collections.Generic;
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

    public List<long> CheckedLocations;

    /// <summary>
    /// seed for this archipelago data. Can be used when loading a file to verify the session the player is trying to
    /// load is valid to the room it's connecting to.
    /// </summary>
    private string CLIENT_KEY = "client_version";
    private string SEED_KEY = "seed";
    private string DEATH_KEY = "death_link";
    public string ClientVersion {get; private set;}
    public int Seed {get; private set;}
    public bool DeathLink {get; private set;}
    public List<ReceivedItem> ReceivedItems {get; set;} = new List<ReceivedItem>(){};
    public SortedDictionary<long, ArchipelagoItem> ScoutedLocations = new(){};

    private Dictionary<string, object> slotData;

    public bool NeedSlotData => slotData == null;

    public ArchipelagoData()
    {
        Uri = "localhost";
        SlotName = "Player1";
        SlotId = 0;
        CheckedLocations = new();
    }

    public ArchipelagoData(string uri, string slotName, string password)
    {
        Uri = uri;
        SlotName = slotName;
        Password = password;
        CheckedLocations = new();
    }

    /// <summary>
    /// assigns the slot data and seed to our data handler. any necessary setup using this data can be done here.
    /// </summary>
    /// <param name="roomSlotData">slot data of your slot from the room</param>
    /// <param name="roomSeed">seed name of this session</param>
    public void SetupSession(Dictionary<string, object> roomSlotData)
    {
        Plugin.Logger.LogInfo("We're starting setup.");
        slotData = roomSlotData;
        Plugin.Logger.LogInfo("Set SlotData.");
        /*Seed = GetSlotSetting(SEED_KEY, 0);
        Plugin.Logger.LogInfo($"Set Seed: {Seed}.");
        ClientVersion = GetSlotSetting(CLIENT_KEY, "");
        Plugin.Logger.LogInfo($"Set ClientVersion: {ClientVersion}.");
        DeathLink = GetSlotSetting(DEATH_KEY, false);
        Plugin.Logger.LogInfo($"Set DeathLink: {DeathLink}.");*/
        Plugin.Logger.LogInfo("We're done.");
    }

    /// <summary>
    /// returns the object as a json string to be written to a file which you can then load
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
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
}