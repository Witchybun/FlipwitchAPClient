using BepInEx;
using BepInEx.Logging;
using FlipwitchAP.Archipelago;
using FlipwitchAP.Data;
using FlipwitchAP.Utils;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace FlipwitchAP;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGUID = "com.Albrekka.FlipwitchAP";
    public const string PluginName = "FlipwitchAP";
    public const string PluginVersion = "0.2.12";
    private const string APDisplayInfo = $"Archipelago v{ArchipelagoClient.APVersion}";
    public static ArchipelagoClient ArchipelagoClient { get; private set; }
    public static bool IsInGame = false;
    public static int notifCounter = 0;
    private static bool ToggleGameActions = false;
    private static InputAction ArchipelagoWindowToggle = new InputAction(binding: "<Keyboard>/f8");

    internal static new ManualLogSource Logger;


    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        SaveHelper.GrabLastConnectionInfo();
        ArchipelagoWindowToggle.Enable();
        ArchipelagoWindowToggle.performed += OnWindowTogglePressed;
        ArchipelagoClient = new ArchipelagoClient();
        ArchipelagoConsole.Awake();
        SpriteSwapHelper.GenerateData();
        PatchAll();
    }

    private void PatchAll()
    {
        Harmony.CreateAndPatchAll(typeof(ItemHelper));
        Harmony.CreateAndPatchAll(typeof(LocationHelper));
        Harmony.CreateAndPatchAll(typeof(SaveHelper));
        Harmony.CreateAndPatchAll(typeof(ShopHelper));
        Harmony.CreateAndPatchAll(typeof(GenericMethods));
        Harmony.CreateAndPatchAll(typeof(QuestFixer));
        Harmony.CreateAndPatchAll(typeof(GachaHelper));
        Harmony.CreateAndPatchAll(typeof(CollectMethods));
        Harmony.CreateAndPatchAll(typeof(DialogueHelper));
        Harmony.CreateAndPatchAll(typeof(CutsceneHelper));
        Harmony.CreateAndPatchAll(typeof(SpriteSwapHelper));
    }

    private void OnWindowTogglePressed(InputAction.CallbackContext context)
    {
        ToggleGameActions = !ToggleGameActions;
        if (ToggleGameActions)
        {
            NewInputManager.instance?.disableActions();
        }
        else
        {
            NewInputManager.instance?.enableActions();
        }
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
        SendMessageOnTimer(30);

    }

    private void SendMessageOnTimer(int time)
    {
        notifCounter += 1;
        if (notifCounter == time)
        {
            notifCounter = 0;
            // Use to bugtest.
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
        if (!IsInGame)
        {
            //ArchipelagoConsole.CreateArchipelagoMenu();
            ArchipelagoClient.Cleanup();
            ArchipelagoClient.ServerData.Index = 0;
            ArchipelagoClient.ServerData.CheckedLocations = new();
        }
        else
        {
            if (GenericInformation.SceneToAreaName.TryGetValue(scene.name, out var trueName))
            {
                ArchipelagoClient.SendCurrentScene(trueName);
            }
        }
        DialogueHelper.GenerateCurrentHintForFortuneTeller(scene.name);
    }

    // Display was shamelessly used from Hunie Pop 2's implementation, by dotsofdarkness, since I just want the window on the right.
    private void OnGUI()
    {
        // show the mod is currently loaded in the corner
        //GUI.Label(new Rect(16, 16, 300, 20), ModDisplayInfo);
        GUI.depth = 0;
        ArchipelagoConsole.OnGUI();
        if (ArchipelagoClient.Authenticated)
        {
            GUI.Window(69, new Rect(Screen.width - 300, 10, 300, 50), WindowContents, "Archipelago");
        }
        else
        {
            GUI.Window(69, new Rect(Screen.width - 300, 10, 300, 130), WindowContents, "Archipelago");
        }

        // this is a good place to create and add a bunch of debug buttons
    }

    private void WindowContents(int id)
    {
        GUI.backgroundColor = Color.black;
        string statusMessage;
        // show the Archipelago Version and whether we're connected or not
        if (ArchipelagoClient.Authenticated)
        {
            // if your game doesn't usually show the cursor this line may be necessary
            // Cursor.visible = false;


            GUI.Label(new Rect(5, 20, 300, 20), "FlipwitchAP Version (" + PluginVersion + ") : Status: Connected");
        }
        else
        {
            // if your game doesn't usually show the cursor this line may be necessary
            // Cursor.visible = true;

            statusMessage = " Status: Disconnected";
            GUI.Label(new Rect(5, 20, 300, 20), APDisplayInfo + statusMessage);
            GUI.Label(new Rect(5, 40, 150, 20), "Host: ");
            GUI.Label(new Rect(5, 60, 150, 20), "Player Name: ");
            GUI.Label(new Rect(5, 80, 150, 20), "Password: ");
            ArchipelagoClient.ServerData.Uri = GUI.TextField(new Rect(150, 40, 140, 20),
                ArchipelagoClient.ServerData.Uri);
            ArchipelagoClient.ServerData.SlotName = GUI.TextField(new Rect(150, 60, 140, 20),
                ArchipelagoClient.ServerData.SlotName);
            ArchipelagoClient.ServerData.Password = GUI.TextField(new Rect(150, 80, 140, 20),
                ArchipelagoClient.ServerData.Password);

            // requires that the player at least puts *something* in the slot name
            if (GUI.Button(new Rect(100, 105, 100, 20), "Connect") &&
                !ArchipelagoClient.ServerData.SlotName.IsNullOrWhiteSpace())
            {
                ArchipelagoClient.Connect();
            }

        }
    }

    public void StartCutsceneTrapRoutine()
    {
        if (CutsceneHelper.trapRoutineRunning)
        {
            return;
        }
        StartCoroutine(CutsceneHelper.HandleAllPendingCutsceneTraps());
    }
}
