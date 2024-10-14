using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using FlipwitchAP.Archipelago;
using FlipwitchAP.Utils;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlipwitchAP;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGUID = "com.Albrekka.FlipwitchAP";
    public const string PluginName = "FlipwitchAP";
    public const string PluginVersion = "0.1.5";
    public const string ModDisplayInfo = $"{PluginName} v{PluginVersion}";
    private const string APDisplayInfo = $"Archipelago v{ArchipelagoClient.APVersion}";
    public static ArchipelagoClient ArchipelagoClient { get; private set; }
    public static bool IsInGame = false;
    public static bool IsMovementDisabled = false;

    internal static new ManualLogSource Logger;
    public ItemHelper ItemHelper;
    public LocationHelper LocationHelper;
    public SaveHelper SaveHelper;
    public ShopHelper ShopHelper;
    public GenericMethods GenericMethods;
    public QuestFixer QuestFixer;


    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        ArchipelagoClient = new ArchipelagoClient();
        ArchipelagoConsole.Awake();

        ItemHelper = new ItemHelper();
        LocationHelper = new LocationHelper();
        SaveHelper = new SaveHelper();
        ShopHelper = new ShopHelper();
        GenericMethods = new GenericMethods();
        QuestFixer = new QuestFixer();

    }

    private void Update()
    {
        if (
            !SwitchDatabase.instance.gamePaused &&
            !SwitchDatabase.instance.dialogueManager.dialogueOrCutsceneOrIngameCutsceneInProgress() &&
            !SwitchDatabase.instance.isItemPopupActive() &&
            IsInGame
            )
        {
            ArchipelagoClient.ReceiveViolation();
            GenericMethods.HandleReceivedItems();
        }
        if (SwitchDatabase.instance.dialogueManager.cutsceneInProgress() && SwitchDatabase.instance.playerMov.isDead())
        {
            ArchipelagoClient.KillEveryone();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsInGame = scene.name != "MainMenu";
        GenericMethods.PatchSceneSwitchTriggers(scene.name);
        GenericMethods.SoftlockSparer(scene.name);
        if (!IsInGame)
        {
            //ArchipelagoConsole.CreateArchipelagoMenu();
            ArchipelagoClient.Cleanup();
            ArchipelagoClient.ServerData.Index = 0;
            ArchipelagoClient.ServerData.CheckedLocations = new();
        }
    }

    private void OnGUI()
    {
        // show the mod is currently loaded in the corner
        GUI.Label(new Rect(16, 16, 300, 20), ModDisplayInfo);
        ArchipelagoConsole.OnGUI();

        string statusMessage;
        // show the Archipelago Version and whether we're connected or not
        if (ArchipelagoClient.Authenticated)
        {
            // if your game doesn't usually show the cursor this line may be necessary
            // Cursor.visible = false;

            statusMessage = " Status: Connected";
            GUI.Label(new Rect(16, 50, 300, 20), APDisplayInfo + statusMessage);
        }
        else
        {
            // if your game doesn't usually show the cursor this line may be necessary
            // Cursor.visible = true;

            statusMessage = " Status: Disconnected";
            GUI.Label(new Rect(16, 50, 300, 20), APDisplayInfo + statusMessage);
            GUI.Label(new Rect(16, 70, 150, 20), "Host: ");
            GUI.Label(new Rect(16, 90, 150, 20), "Player Name: ");
            GUI.Label(new Rect(16, 110, 150, 20), "Password: ");

            ArchipelagoClient.ServerData.Uri = GUI.TextField(new Rect(150, 70, 150, 20),
                ArchipelagoClient.ServerData.Uri);
            ArchipelagoClient.ServerData.SlotName = GUI.TextField(new Rect(150, 90, 150, 20),
                ArchipelagoClient.ServerData.SlotName);
            ArchipelagoClient.ServerData.Password = GUI.TextField(new Rect(150, 110, 150, 20),
                ArchipelagoClient.ServerData.Password);

            // requires that the player at least puts *something* in the slot name
            if (GUI.Button(new Rect(16, 130, 100, 20), "Connect") &&
                !ArchipelagoClient.ServerData.SlotName.IsNullOrWhiteSpace())
            {
                ArchipelagoClient.Connect();
            }
        }
        // this is a good place to create and add a bunch of debug buttons
    }
}
