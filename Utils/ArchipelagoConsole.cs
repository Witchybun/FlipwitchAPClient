using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Enums;
using BepInEx;
using FlipwitchAP.Archipelago;
using UnityEngine;
using Archipelago.MultiClient.Net.MessageLog.Parts;

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

    public const string PROGRESSION_COLOR_DEFAULT = "#AF99EF";
    public const string USEFUL_COLOR_DEFAULT = "#6D8BE8";
    public const string FILLER_COLOR_DEFAULT = "#00EEEE";
    public const string TRAP_COLOR_DEFAULT = "#FA8072";
    public const string CHEAT_COLOR_DEFAULT = "#FF0000";
    public const string GIFT_COLOR_DEFAULT = "#9DAE11";
    public const string PLAYER_COLOR_DEFAULT = "#00FF00";
    public const string OTHER_PLAYER_COLOR_DEFAULT = "#86DA86";

    private static GameObject ArchipelagoMenu = new();

    public static void Awake()
    {
        UpdateWindow();
    }

    public static void LogMessage(LogMessage logMessage)
    {
        if (logMessage.ToString().IsNullOrWhiteSpace()) return;
        var message = logMessage.ToString();
        switch (logMessage)
        {
            case HintItemSendLogMessage hintMessage:
                {
                    var receiver = "";
                    if (!hintMessage.IsSenderTheActivePlayer && !hintMessage.IsReceiverTheActivePlayer)
                    {
                        return; // Who asked
                    }
                    if (hintMessage.IsSenderTheActivePlayer)
                    {
                        receiver = hintMessage.Receiver.Name;
                    }
                    else if (hintMessage.IsReceiverTheActivePlayer)
                    {
                        receiver = $"<color={PLAYER_COLOR_DEFAULT}>" + hintMessage.Receiver.Name + "</color>";
                    }
                    var item = ColorTextBasedOnProgression(hintMessage.Item);
                    message = "<color=#FF0000>[HINT]</color> " + receiver + " wants " + item + " from " + hintMessage.Item.LocationName + ".";
                    break;
                }
            case ItemSendLogMessage itemSendMessage:
                {
                    if (!itemSendMessage.IsReceiverTheActivePlayer && !itemSendMessage.IsSenderTheActivePlayer)
                    {
                        return;  // Who asked
                    }
                    var sender = "";
                    var receiver = "";
                    if (itemSendMessage.IsReceiverTheActivePlayer)
                    {
                        sender = $"<color={OTHER_PLAYER_COLOR_DEFAULT}>" + itemSendMessage.Sender.Name + "</color>";
                        receiver = $"<color={PLAYER_COLOR_DEFAULT}>" + itemSendMessage.Receiver.Name + "</color>";
                    }
                    else if (itemSendMessage.IsSenderTheActivePlayer)
                    {
                        sender = $"<color={PLAYER_COLOR_DEFAULT}>" + itemSendMessage.Sender.Name + "</color>";
                        receiver = $"<color={OTHER_PLAYER_COLOR_DEFAULT}>" + itemSendMessage.Receiver.Name + "</color>";
                    }
                    var item = ColorTextBasedOnProgression(itemSendMessage.Item);
                    message = sender + " sent " + item + " to " + receiver + " (" + itemSendMessage.Item.LocationName + ")";
                    if (itemSendMessage.Receiver.Name != itemSendMessage.Sender.Name && itemSendMessage.IsReceiverTheActivePlayer)
                    {
                        PlaySoundBasedOnClassification(itemSendMessage.Item.Flags);
                    }
                    break;
                }
        }
        LogMessage(message);
        
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
    
    public static string ColorTextBasedOnProgression(ItemInfo item)
    {
        if (item.Flags.HasFlag(ItemFlags.Trap))
        {
            return $"<color={TRAP_COLOR_DEFAULT}>" + item.ItemName + "</color>";
        }
        else if (item.Flags.HasFlag(ItemFlags.Advancement))
        {
            return $"<color={PROGRESSION_COLOR_DEFAULT}>" + item.ItemName + "</color>";
        }
        else if (item.Flags.HasFlag(ItemFlags.NeverExclude))
        {
            return $"<color={USEFUL_COLOR_DEFAULT}>" + item.ItemName + "</color>";
        }
        return $"<color={FILLER_COLOR_DEFAULT}>" + item.ItemName + "</color>";
    }

    private static void PlaySoundBasedOnClassification(ItemFlags flags)
    {
        var playerObject = SwitchDatabase.instance.playerMov.gameObject;
        if (flags.HasFlag(ItemFlags.Trap))
        {
            AkSoundEngine.PostEvent("boss_crystal_fall", playerObject);
            return;
        }
        else if (flags.HasFlag(ItemFlags.Advancement))
        {
            AkSoundEngine.PostEvent("purchase", playerObject);
            return;
        }
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
        textStyle.richText = true;
        window = new Rect(Screen.width / 2 - width / 2, 0, width, height);
        scroll = new Rect(0, 0, width * 0.9f, scrollDepth);
        scrollView = new Vector2(0, scrollDepth);
        text = new Rect(0, 0, width, scrollDepth);

        textStyle.alignment = TextAnchor.LowerLeft;
        textStyle.fontSize = Hidden ? (int)(Screen.height * 0.0165f) : (int)(Screen.height * 0.0185f);
        textStyle.normal.textColor = UnityEngine.Color.white;
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