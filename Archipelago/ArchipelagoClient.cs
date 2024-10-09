using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using FlipwitchAP.Utils;
using FlipwitchAP.Data;
using UnityEngine;
using static FlipwitchAP.Data.FlipwitchLocations;
using System.Collections.ObjectModel;
using BepInEx;
using Mono.Cecil.Cil;
using UnityEngine.Analytics;

namespace FlipwitchAP.Archipelago;

public class ArchipelagoClient
{
    public const string APVersion = "0.5.0";
    public const string Game = "Flipwitch Forbidden Sex Hex";

    public static bool Authenticated;
    private bool attemptingConnection;

    public static ArchipelagoData ServerData = new();
    private DeathLinkHandler DeathLinkHandler;
    private DeathLinkService DeathLinkService;
    private ArchipelagoSession session;
    public static Queue<ReceivedItem> ItemsToProcess = new();
    public readonly SortedDictionary<long, ArchipelagoItem> LocationTable = new() { };
    public static bool IsInGame = false;
    public int MaxReceivedCount = 0;

    /// <summary>
    /// call to connect to an Archipelago session. Connection info should already be set up on ServerData
    /// </summary>
    /// <returns></returns>
    public void Connect()
    {
        if (Authenticated || attemptingConnection) return;

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
        session.MessageLog.OnMessageReceived += message => ArchipelagoConsole.LogMessage(message.ToString());
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
            Authenticated = true;

            BuildLocations(seedBeforeSetup);

            DeathLinkHandler = new(session.CreateDeathLinkService(), ServerData.SlotName, ServerData.DeathLink);
            session.Locations.CompleteLocationChecksAsync(ServerData.CheckedLocations.ToArray());
            outText = $"Successfully connected to {ServerData.Uri} as {ServerData.SlotName}!";
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
        Plugin.Logger.LogInfo($"Received {item.ItemName} in situation where helper index is {helper.Index} and saved index is {ServerData.Index}");

        var receivedItem = new ReceivedItem(item, helper.Index);
        ItemsToProcess.Enqueue(receivedItem);
        SaveQueueState();
        Plugin.Logger.LogInfo($"Queued {item.ItemName}.");
        MaxReceivedCount++;
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
        if (item.IsOwnItem) // Handle without an internet connection.
        {
            ItemHelper.GiveFlipwitchItem(item.Game, location.APLocationName, item.Name, item.SlotName, item.Classification);
            ServerData.Index++;
            if (Authenticated)
            {
                session.Locations.CompleteLocationChecks(location.APLocationID);
            }
        }
        else if (Authenticated) // If someone else's item an online, do the usual
        {
            session.Locations.CompleteLocationChecks(location.APLocationID);
        }
        else if (!ServerData.CheckedLocations.Contains(location.APLocationID)) // Otherwise just save it for syncing later.
        {
            ServerData.CheckedLocations.Add(location.APLocationID);
        }
        return false;
    }

    public void SaveQueueState()
    {
        ServerData.StoredQueue = ItemsToProcess;
    }

    public void LoadQueueState()
    {
        ItemsToProcess = ServerData.StoredQueue;
    }

    public bool IsThereIndexMismatch(out List<ItemInfo> items)
    {
        var serverIndex = session.Items.Index;
        if (serverIndex != ServerData.Index)
        {
            items = session.Items.AllItemsReceived.ToList();
            GenericMethods.allowingOutsideItems = false;
            return true;
        }
        items = null;
        return false;
    }


    public string GetPlayerNameFromSlot(int slot)
    {
        return session.Players.GetPlayerName(slot);
    }

    public void SendLocation(long locationID)
    {
        session.Locations.CompleteLocationChecks(locationID);
    }

    public bool IsLocationChecked(long locationID)
    {
        return ServerData.CheckedLocations.Contains(locationID);
    }

    public List<long> AllLocationsCompletedNotedByServer()
    {
        return session.Locations.AllLocationsChecked.ToList();
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