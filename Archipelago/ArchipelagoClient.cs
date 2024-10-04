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
    public static bool IsInGame = false;
    public static bool IsMovementDisabled = false;

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
                        ItemsHandlingFlags.NoItems, // TODO make sure to change this line
                        new Version(APVersion),
                        password: ServerData.Password,
                        requestSlotData: false // ServerData.NeedSlotData
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
            Plugin.Logger.LogInfo(success.SlotData is null);
            ServerData.SetupSession(success.SlotData);
            Authenticated = true;

            BuildLocations();

            DeathLinkHandler = new(session.CreateDeathLinkService(), ServerData.SlotName);
#if NET35
            session.Locations.CompleteLocationChecksAsync(null, ServerData.CheckedLocations.ToArray());
#else
            session.Locations.CompleteLocationChecksAsync(ServerData.CheckedLocations.ToArray());
#endif
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

    private void BuildLocations()
    {
        if (!ServerData.ScoutedLocations.Any())
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
#if NET35
        session?.Socket.Disconnect();
#else
        session?.Socket.DisconnectAsync();
#endif
        session = null;
        Authenticated = false;
        foreach (var tableInfo in LocationTable)
        {
            LocationTable[tableInfo.Key] = null;
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
        var item = helper.DequeueItem();
        if (helper.Index < ServerData.Index) return;

        ServerData.Index++;
        Plugin.Logger.LogInfo($"Received {item.ItemName}");
        var receivedItem = new ReceivedItem(item);
        ItemsToProcess.Enqueue(receivedItem);
    }

    public void SyncItemsReceivedOnLoad()
    {
        Plugin.Logger.LogInfo($"Attempting to give old items...");
        Plugin.Logger.LogInfo($"There's items to sift through.: {session.Items.Any()}");
        
        foreach (var item in session.Items.AllItemsReceived)
        {
            Plugin.Logger.LogInfo($"Checking out {item.ItemName}");
            if (item.LocationId >= 0 && !ServerData.ReceivedItems.Any(x => x.PlayerId == item.Player && x.LocationId == item.LocationId && item.ItemId == x.ItemId))
            {
                var receivedItem = new ReceivedItem(item);
                ServerData.ReceivedItems.Add(receivedItem);
                ItemsToProcess.Enqueue(receivedItem);
            }
        }
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
}