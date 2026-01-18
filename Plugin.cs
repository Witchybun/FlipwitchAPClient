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
    public const string PluginVersion = "1.0.0";
    private const string APDisplayInfo = $"Archipelago v{ArchipelagoClient.APVersion}";
    public static ArchipelagoClient ArchipelagoClient { get; private set; }
    public static int notifCounter = 0;
    private static bool _toggleGameActions;
    private static readonly InputAction ArchipelagoWindowToggle = new(binding: "<Keyboard>/f8");

    internal new static ManualLogSource Logger;


    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        SaveHelper.GrabLastConnectionInfo();
        ArchipelagoWindowToggle.Enable();
        ArchipelagoWindowToggle.performed += OnWindowTogglePressed;
        ArchipelagoClient = new ArchipelagoClient();
        ArchipelagoClient.Setup();
        ArchipelagoConsole.Awake();
        SpriteSwapHelper.GenerateData();
        PatchAll();
        //Harmony.CreateAndPatchAll(typeof(InfoCatcher));
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
        
        Harmony.CreateAndPatchAll(typeof(TeleportHelper));
        Harmony.CreateAndPatchAll(typeof(BasicMovement));

    }

    private void OnWindowTogglePressed(InputAction.CallbackContext context)
    {
        _toggleGameActions = !_toggleGameActions;
        if (_toggleGameActions)
        {
            NewInputManager.instance?.disableActions();
        }
        else
        {
            NewInputManager.instance?.enableActions();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ArchipelagoClient.IsInGame = scene.name != "MainMenu";
        GenericMethods.PatchSceneSwitchTriggers(scene.name);
        if (!ArchipelagoClient.IsInGame)
        {
            //ArchipelagoConsole.CreateArchipelagoMenu();
            ArchipelagoClient.Cleanup();
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
        if (ArchipelagoClient.AP.FirstTimeWarp)
        {
            SwitchDatabase.instance.setBool("TP_Beatrix", false);
        }
    }

    // Display was shamelessly used from Hunie Pop 2's implementation, by dotsofdarkness, since I just want the window on the right.
    private void OnGUI()
    {
        // show the mod is currently loaded in the corner
        //GUI.Label(new Rect(16, 16, 300, 20), ModDisplayInfo);
        GUI.depth = 0;
        ArchipelagoConsole.OnGUI();
        GUI.Window(69,
            ArchipelagoClient.Authenticated
                ? new Rect(Screen.width - 300, 10, 300, 50)
                : new Rect(Screen.width - 300, 10, 300, 130), WindowContents, "Archipelago");

        // this is a good place to create and add a bunch of debug buttons
    }

    private void WindowContents(int id)
    {
        GUI.backgroundColor = Color.black;
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

            var statusMessage = " Status: Disconnected";
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
