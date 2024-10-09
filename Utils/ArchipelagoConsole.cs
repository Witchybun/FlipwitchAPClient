using System.Collections.Generic;
using System.Linq;
using BepInEx;
using FlipwitchAP.Archipelago;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlipwitchAP.Utils;

// shamelessly stolen from oc2-modding https://github.com/toasterparty/oc2-modding/blob/main/OC2Modding/GameLog.cs
public static class ArchipelagoConsole
{
    public static bool Hidden = true;

    private static List<string> logLines = new();
    private static Vector2 scrollView;
    private static Rect window;
    private static Rect scroll;
    private static Rect text;
    private static Rect hideShowButton;

    private static GUIStyle textStyle = new();
    private static string scrollText = "";
    private static float lastUpdateTime = Time.time;
    private const int MaxLogLines = 80;
    private const float HideTimeout = 15f;

    private static string CommandText = "!help";
    private static Rect CommandTextRect;
    private static Rect SendCommandButton;

    private static GameObject ArchipelagoMenu = new();

    public static void Awake()
    {
        UpdateWindow();
    }

    public static void LogMessage(string message)
    {
        if (message.IsNullOrWhiteSpace()) return;

        if (logLines.Count == MaxLogLines)
        {
            logLines.RemoveAt(0);
        }
        logLines.Add(message);
        Plugin.Logger.LogMessage(message);
        lastUpdateTime = Time.time;
        UpdateWindow();
    }

    public static void OnGUI()
    {
        if (logLines.Count == 0) return;

        if (!Hidden || Time.time - lastUpdateTime < HideTimeout)
        {
            scrollView = GUI.BeginScrollView(window, scrollView, scroll);
            GUI.Box(text, "");
            GUI.Box(text, scrollText, textStyle);
            GUI.EndScrollView();
        }

        if (GUI.Button(hideShowButton, Hidden ? "Show" : "Hide"))
        {
            Hidden = !Hidden;
            UpdateWindow();
        }
        
        // draw client/server commands entry
        if (Hidden || !ArchipelagoClient.Authenticated) return;

        CommandText = GUI.TextField(CommandTextRect, CommandText);
        if (!CommandText.IsNullOrWhiteSpace() && GUI.Button(SendCommandButton, "Send"))
        {
            Plugin.ArchipelagoClient.SendMessage(CommandText);
            CommandText = "";
        }
    }

    public static void CreateArchipelagoMenu()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            return;
        }
        var mainMenuUi = GameObject.Find("MainMenuUi");
        ArchipelagoMenu.transform.parent = mainMenuUi.transform;
        var mainMenu = mainMenuUi.transform.Find("Menu").Find("SettingsMenu");
        ArchipelagoMenu = GameObject.Instantiate(mainMenu.gameObject);
        ArchipelagoMenu.name = "ArchipelagoMenu";
        GameObject.Destroy(ArchipelagoMenu.GetComponent<SettingsMenu>());
        GameObject.Destroy(ArchipelagoMenu.transform.Find("ControlsPanel"));
        ArchipelagoMenu.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "Archipelago";
        ArchipelagoMenu.transform.Find("OptionsPanel").name = "ArchipelagoPanel";
        var host = ArchipelagoMenu.transform.Find("ArchipelagoPanel").Find("MasterRow");
        CreateInputField(host, "Host", "ScrollArea", "HostInput", "localhost:38281");
        var slot = ArchipelagoMenu.transform.Find("ArchipelagoPanel").Find("MusicRow");
        CreateInputField(slot, "Slot", "ScrollArea", "SlotInput", "Player1");
        var password = ArchipelagoMenu.transform.Find("ArchipelagoPanel").Find("AmbienceRow");
        CreateInputField(password, "Password", "ScrollArea", "PasswordInput", "");
        

    }

    private static void CreateInputField(Transform mainTransform, string mainName,  string transformToFind, string transformedTitle, string placeholderText)
    {
        mainTransform.name = mainName + "Row";
        mainTransform.Find("Name").Find("Text").GetComponent<TextMeshProUGUI>().text = mainName;
        var transformToTransform = mainTransform.Find(transformToFind);
        transformToTransform.name = transformedTitle;
        foreach (Transform child in transformToTransform)
        {
            GameObject.Destroy(child);
        }
        transformToTransform.gameObject.AddComponent<TMP_InputField>();
        transformToTransform.gameObject.AddComponent<TextMeshProUGUI>();

        var hostInput = transformToTransform.GetComponent<TMP_InputField>();
        hostInput.caretColor = new Color (1f, 1f, 1f, 1f);
        Plugin.Logger.LogInfo($"State of nullity: {mainTransform.Find("Name").Find("Text").GetComponent<TextMeshProUGUI>().font is null}");
        hostInput.SetGlobalFontAsset(mainTransform.Find("Name").Find("Text").GetComponent<TextMeshProUGUI>().font);
        hostInput.textViewport = transformToTransform.GetComponent<RectTransform>();
        hostInput.textComponent = transformToTransform.GetComponent<TextMeshProUGUI>();

        var placeholder = new GameObject();
        placeholder.transform.position = transformToTransform.position;
        placeholder.transform.parent = transformToTransform;
        placeholder.AddComponent<TextMeshProUGUI>();
        placeholder.AddComponent<RectTransform>();
        hostInput.placeholder = placeholder.GetComponent<TextMeshProUGUI>();
        placeholder.GetComponent<TextMeshProUGUI>().text = placeholderText;
    }

    public static void UpdateWindow()
    {
        scrollText = "";

        if (Hidden)
        {
            if (logLines.Count > 0)
            {
                scrollText = logLines[logLines.Count - 1];
            }
        }
        else
        {
            for (var i = 0; i < logLines.Count; i++)
            {
                scrollText += "> ";
                scrollText += logLines.ElementAt(i);
                if (i < logLines.Count - 1)
                {
                    scrollText += "\n\n";
                }
            }
        }

        var width = (int)(Screen.width * 0.4f);
        int height;
        int scrollDepth;
        if (Hidden)
        {
            height = (int)(Screen.height * 0.03f);
            scrollDepth = height;
        }
        else
        {
            height = (int)(Screen.height * 0.3f);
            scrollDepth = height * 10;
        }

        window = new Rect(Screen.width / 2 - width / 2, 0, width, height);
        scroll = new Rect(0, 0, width * 0.9f, scrollDepth);
        scrollView = new Vector2(0, scrollDepth);
        text = new Rect(0, 0, width, scrollDepth);

        textStyle.alignment = TextAnchor.LowerLeft;
        textStyle.fontSize = Hidden ? (int)(Screen.height * 0.0165f) : (int)(Screen.height * 0.0185f);
        textStyle.normal.textColor = Color.white;
        textStyle.wordWrap = !Hidden;

        var xPadding = (int)(Screen.width * 0.01f);
        var yPadding = (int)(Screen.height * 0.01f);

        textStyle.padding = Hidden
            ? new RectOffset(xPadding / 2, xPadding / 2, yPadding / 2, yPadding / 2)
            : new RectOffset(xPadding, xPadding, yPadding, yPadding);

        var buttonWidth = (int)(Screen.width * 0.12f);
        var buttonHeight = (int)(Screen.height * 0.03f);

        hideShowButton = new Rect(Screen.width / 2 + width / 2 + buttonWidth / 3, Screen.height * 0.004f, buttonWidth,
            buttonHeight);

        // draw server command text field and button
        width = (int)(Screen.width * 0.4f);
        var xPos = (int)(Screen.width / 2.0f - width / 2.0f);
        var yPos = (int)(Screen.height * 0.307f);
        height = (int)(Screen.height * 0.022f);

        CommandTextRect = new Rect(xPos, yPos, width, height);

        width = (int)(Screen.width * 0.035f);
        yPos += (int)(Screen.height * 0.03f);
        SendCommandButton = new Rect(xPos, yPos, width, height);
    }
}