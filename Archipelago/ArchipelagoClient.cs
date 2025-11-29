using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using FlipwitchAP.Utils;
using FlipwitchAP.Data;
using static FlipwitchAP.Data.FlipwitchLocations;
using System.Collections.ObjectModel;
using BepInEx.Logging;

namespace FlipwitchAP.Archipelago;

public class ArchipelagoClient
{
    public const string APVersion = "0.5.0";
    public const string Game = "Flipwitch Forbidden Sex Hex";

    public static bool Authenticated;
    private bool attemptingConnection;

    public static ArchipelagoData ServerData = new();
    private DeathLinkHandler DeathLinkHandler;
    private ArchipelagoSession session;
    public static Queue<ReceivedItem> ItemsToProcess = new();
    public readonly SortedDictionary<long, ArchipelagoItem> LocationTable = new() { };
    public readonly List<string> CutsceneIDsForTraps = new();
    public static bool IsInGame = false;
    public static bool PlayerWasDeathlinked = false;

    /// <summary>
    /// call to connect to an Archipelago session. Connection info should already be set up on ServerData
    /// </summary>
    /// <returns></returns>
    public void Connect()
    {
        if (Authenticated || attemptingConnection) return;
        attemptingConnection = true;

        try
        {
            session = ArchipelagoSessionFactory.CreateSession(ServerData.Uri);
            SetupSession();
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
        }
        TryConnect();
    }

    /// <summary>
    /// add handlers for Archipelago events
    /// </summary>
    private void SetupSession()
    {
        session.MessageLog.OnMessageReceived += message => ArchipelagoConsole.LogMessage(message);
        session.Items.ItemReceived += OnItemReceived;
        session.Socket.ErrorReceived += OnSessionErrorReceived;
        session.Socket.SocketClosed += OnSessionSocketClosed;
        session.Locations.CheckedLocationsUpdated += OnCheckedLocationsUpdated;
    }


    /// <summary>
    /// attempt to connect to the server with our connection info
    /// </summary>
    private void TryConnect()
    {
        try
        {
            // it's safe to thread this function call but unity notoriously hates threading so do not use excessively
            ThreadPool.QueueUserWorkItem(
                _ => HandleConnectResult(
                    session.TryConnectAndLogin(
                        Game,
                        ServerData.SlotName,
                        ItemsHandlingFlags.AllItems, // TODO make sure to change this line
                        new Version(APVersion),
                        password: ServerData.Password,
                        requestSlotData: true // ServerData.NeedSlotData
                    )));
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
            HandleConnectResult(new LoginFailure(e.ToString()));
            attemptingConnection = false;
        }
    }

    /// <summary>
    /// handle the connection result and do things
    /// </summary>
    /// <param name="result"></param>
    private void HandleConnectResult(LoginResult result)
    {
        string outText;
        if (result.Successful)
        {
            var success = (LoginSuccessful)result;
            if (!APVersionIsAcceptable(success.SlotData, out var apworldVersion))
            {
                var outMes = $"Version mismatch.  APWorld: {apworldVersion}, Client: {Plugin.PluginVersion}";
                ArchipelagoConsole.LogMessage(outMes);
                Authenticated = false;
                Disconnect();
                attemptingConnection = false;
            }
            var seedBeforeSetup = ServerData.Seed;
            ServerData.SetupSession(success.SlotData);
            BuildLocations(seedBeforeSetup);
            DeathLinkHandler = new(session.CreateDeathLinkService(), ServerData.SlotName, ServerData.DeathLink);
            session.Locations.CompleteLocationChecksAsync(ServerData.CheckedLocations.ToArray());
            outText = $"Successfully connected to {ServerData.Uri} as {ServerData.SlotName}!";
            SaveHelper.SaveCurrentConnectionData(ServerData.Uri, ServerData.SlotName, ServerData.Password);
            DialogueHelper.UpdateDialogueToHaveHints(SwitchDatabase.instance.dialogueManager);
            //GrabAllTrapOrientedCutscenes();
            Authenticated = true;
        }
        else
        {
            var failure = (LoginFailure)result;
            outText = $"Failed to connect to {ServerData.Uri} as {ServerData.SlotName}.";
            outText = failure.Errors.Aggregate(outText, (current, error) => current + $"\n    {error}");

            Plugin.Logger.LogError(outText);

            Authenticated = false;
            Disconnect();
        }
        ArchipelagoConsole.LogMessage(outText);
        attemptingConnection = false;
    }

    private bool APVersionIsAcceptable(Dictionary<string, object> slotData, out string version)
    {
        var apworldVersion = (string)slotData["client_version"];
        var apworldVersionArray = apworldVersion.Split('.');
        var clientVersionArray = Plugin.PluginVersion.Split('.');
        if (apworldVersionArray[1] != clientVersionArray[1])
        {
            version = apworldVersion;
            return false;
        }
        version = "";
        return true;
    }

    private void BuildLocations(int seed)
    {
        if (ServerData.Seed != seed)
        {
            BuildLocationTable();
            // Scout unchecked locations
            var uncheckedLocationIDs = from locationID in LocationTable.Keys select locationID;
            var locations = session.Locations.AllLocations;
            foreach (var location in uncheckedLocationIDs)
            {
                if (locations.Contains(location))
                {
                    continue;
                }
                Plugin.Logger.LogWarning($"There's a location you're trying to scout that isn't there!  Location: {location}");
            }
            Task<Dictionary<long, ScoutedItemInfo>> scoutedInfoTask = Task.Run(async () => await session.Locations.ScoutLocationsAsync(false, uncheckedLocationIDs.ToArray()));
            //Task<LocationInfoPacket> locationInfoTask = Task.Run(async () => await Session.Locations.ScoutLocationsAsync(false, uncheckedLocationIDs.ToArray()));
            if (scoutedInfoTask.IsFaulted)
            {
                Plugin.Logger.LogError(scoutedInfoTask.Exception.GetBaseException().Message);
                return;
            }
            var scoutedInfo = scoutedInfoTask.Result;

            foreach (var item in scoutedInfo.Values)
            {
                int locationID = (int)item.LocationId;
                LocationTable[locationID] = new ArchipelagoItem(item, false);
            }
            ServerData.ScoutedLocations = LocationTable;
        }
    }

    private void BuildLocationTable()
    {
        List<int> locations = new();
        foreach (var location in FlipwitchLocations.APLocationData)
        {
            if (location.IgnoreLocationHandler == true)
            {
                continue;
            }
            // There's like one coin chest in cabaret lol
            if (location.APLocationID == 494 && (ServerData.QuestForSex == ArchipelagoData.Quest.Off || ServerData.QuestForSex == ArchipelagoData.Quest.Sensei))
            {
                continue;
            }
            else if (location.PrimaryCallName.Contains("ChaosKey") && !ServerData.ShuffleChaosPieces)
            {
                continue;
            }
            else if (FlipwitchItems.QuestItems.Contains(location.PrimaryCallName) && (ServerData.QuestForSex == ArchipelagoData.Quest.Off || ServerData.QuestForSex == ArchipelagoData.Quest.Sensei))
            {
                continue;
            }
            else if (location.Type == QUEST && (ServerData.QuestForSex == ArchipelagoData.Quest.Off || ServerData.QuestForSex == ArchipelagoData.Quest.Sensei))
            {
                continue;
            }
            else if (location.Type == SEX && ServerData.QuestForSex == ArchipelagoData.Quest.Off)
            {
                continue;
            }
            else if (location.Type == GACHA && ServerData.Gachapon == ArchipelagoData.Gacha.Off)
            {
                continue;
            }
            else if (location.Type == GACHAMACHINE && ServerData.Gachapon != ArchipelagoData.Gacha.All)
            {
                continue;
            }
            else if (location.Type == SHOP && !ServerData.Shopsanity)
            {
                continue;
            }
            else if (location.Type == STAT && !ServerData.Statshuffle)
            {
                continue;
            }
            locations.Add((int)location.APLocationID);
        }

        foreach (var id in locations)
        {
            LocationTable[id] = null;
        }
    }

    /// <summary>
    /// something we wrong or we need to properly disconnect from the server. cleanup and re null our session
    /// </summary>
    private void Disconnect()
    {
        Plugin.Logger.LogDebug("disconnecting from server...");
        session?.Socket.DisconnectAsync();
        session = null;
        Authenticated = false;
    }

    public void Cleanup()
    {
        if (Authenticated)
        {
            Disconnect();
        }
        
    }

    public void SendMessage(string message)
    {
        session.Socket.SendPacketAsync(new SayPacket { Text = message });
    }

    /// <summary>
    /// we received an item so reward it here
    /// </summary>
    /// <param name="helper">item helper which we can grab our item from</param>
    private void OnItemReceived(ReceivedItemsHelper helper)
    {
        // Since we connect before picking a save, Index is always 0.  So this will always pull every item.
        // Once we get to a state where picking the save moves to connecting, we can move to less calls.
        var item = helper.DequeueItem();
        var receivedItem = new ReceivedItem(item, helper.Index);
        ItemsToProcess.Enqueue(receivedItem);
    }

    /// <summary>
    /// something went wrong with our socket connection
    /// </summary>
    /// <param name="e">thrown exception from our socket</param>
    /// <param name="message">message received from the server</param>
    private void OnSessionErrorReceived(Exception e, string message)
    {
        Plugin.Logger.LogError(e);
        ArchipelagoConsole.LogMessage(message);
    }

    /// <summary>
    /// something went wrong closing our connection. disconnect and clean up
    /// </summary>
    /// <param name="reason"></param>
    private void OnSessionSocketClosed(string reason)
    {
        Plugin.Logger.LogError($"Connection to Archipelago lost: {reason}");
        Disconnect();
    }

    private void OnCheckedLocationsUpdated(ReadOnlyCollection<long> newCheckedLocations)
    {
        foreach (var location in newCheckedLocations)
        {
            if (ServerData.CheckedLocations.Contains(location))
            {
                continue;
            }
            ServerData.CheckedLocations.Add(location);
        }
    }

    public bool DetermineOwnerAndDirectlyGiveIfSelf(LocationData location, ArchipelagoItem item)
    {
        if (Authenticated) // If someone else's item an online, do the usual
        {
            session.Locations.CompleteLocationChecks(location.APLocationID);
        }
        else if (!ServerData.CheckedLocations.Contains(location.APLocationID)) // Otherwise just save it for syncing later.
        {
            ServerData.CheckedLocations.Add(location.APLocationID);
        }
        return false;
    }

    public void GrabAllTrapOrientedCutscenes()
    {
        CutsceneIDsForTraps.Clear();
        foreach (var cutscene in SwitchDatabase.instance.cutsceneManager.cutscenes)
        {
            if (cutscene.cutsceneUnlockedID.Contains("Reward") && cutscene.animName.Contains("_0"))
            {
                CutsceneIDsForTraps.Add(cutscene.animName);
            }
        }
    }

    public List<ItemInfo> GetAllSentItems()
    {
        return session.Items.AllItemsReceived.ToList();
    }


    public string GetPlayerNameFromSlot(int slot)
    {
        if (slot < 0) return "CheaterBozo";
        return session.Players.GetPlayerName(slot);
    }

    public void SendLocation(long locationID)
    {
        session.Locations.CompleteLocationChecks(locationID);
    }

    // Have a fallback.
    public bool IsLocationChecked(long locationID)
    {
        if (ServerData.CheckedLocations.Contains(locationID))
        {
            return true;
        }
        else if (session.Locations.AllLocationsChecked.Contains(locationID))
        {
            return true;
        }
        return false;
    }

    public bool IsLocationChecked(string locationName)
    {
        var locationID = FlipwitchLocations.NameToLocation[locationName].APLocationID;
        return IsLocationChecked(locationID);
    }

    public List<long> AllLocationsCompletedNotedByServer()
    {
        return session.Locations.AllLocationsChecked.ToList();
    }

    public void SendCurrentScene(string sceneName)
    {
        session.DataStorage["FlipwitchZone_" + ArchipelagoData.SlotId] = sceneName;
    }

    public void ReceiveViolation()
    {
        DeathLinkHandler.KillPlayer();
    }

    public void KillEveryone()
    {
        DeathLinkHandler.SendDeathLink();
    }

    public void SendVictory()
    {
        session.SetGoalAchieved();
    }
}