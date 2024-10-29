using System.Collections.Generic;
using BepInEx.Logging;
using AK.Wwise;
using HarmonyLib;
using UnityEngine;
using System;
using FlipwitchAP.Archipelago;
using FlipwitchAP.Data;
using System.Linq;

namespace FlipwitchAP
{
    public class DialogueHelper
    {
        public static Dictionary<string, string> CurrentGameHintDialogue = new();

        [HarmonyPatch(typeof(DialogueManager), "changeToLanguage")]
        [HarmonyPostfix]
        private static void ChangeToLanguage_ResetHintsIfCalled()
        {
            if (!ArchipelagoClient.Authenticated)
            {
                return;
            }
            UpdateDialogueToHaveHints(SwitchDatabase.instance.dialogueManager);
        }


        [HarmonyPatch(typeof(DialogueManager), "TypeText")]
        [HarmonyPostfix]
        public static void TypeText_SendSignalForHintAlso(TMPResizeText currentTextResizer, ref List<string> ___currentKeys,  ref int ___keyIndex)
        {
            var key = ___currentKeys[___keyIndex];
            var item = "";
            if (!Dialogue.TranslationKeytoHintMessage.Keys.Contains(key))
            {
                return;
            }
            foreach (var itemPair in Dialogue.RelevantItemToRelevantKeys)
            {
                if (itemPair.Value.Contains(key))
                {
                    item = itemPair.Key;
                    break;
                }
            }
            if (item == "") return;
            if  (Dialogue.HintNameToItemName.TryGetValue(item, out var hintGameItem))
            {
                SwitchDatabase.instance.setInt("AP" + hintGameItem + "HintFound", 1);
                return;
            }
            else if (FlipwitchItems.APItemToGameName.TryGetValue(item, out var gameItem))
            {
                SwitchDatabase.instance.setInt("AP" + gameItem + "HintFound", 1);
                return;
            }
        }

        

        [HarmonyPatch(typeof(DialogueManager), "getTranslationString")]
        [HarmonyPrefix]
        private static bool GetTranslationString_GetHintInstead(string id, ref string __result)
        {
            if (!string.IsNullOrEmpty(id) && CurrentGameHintDialogue.ContainsKey(id))
            {
                Plugin.Logger.LogInfo("Hi");
                __result = CurrentGameHintDialogue[id];
                return false;
            }
            return true;
        }

        public static void UpdateDialogueToHaveHints(DialogueManager dialogueManager)
        {
            /*if (!ArchipelagoClient.Authenticated)
            {
                return;
            }*/
            UpdateDialogueToHaveHints();
            var translationDictionaryField = dialogueManager.GetType().GetField("translationStrings", GenericMethods.Flags);
            var translationDictionary = (Dictionary<string, string>) translationDictionaryField.GetValue(dialogueManager);
            foreach (var hintPair in ArchipelagoClient.ServerData.HintLookup)
            {
                if (Dialogue.RelevantItemToRelevantKeys.TryGetValue(hintPair.Key, out var relevantTranslations))
                {
                    var hint = hintPair.Value;
                    foreach (var translationKey in relevantTranslations)
                    {
                        if (!Dialogue.TranslationKeytoHintMessage.TryGetValue(translationKey, out var message))
                        {
                            continue;
                        }
                        if (translationKey == "SubPlague.1.4")
                        {
                            hint.Replace("ss", "th").Replace("s", "th").Replace("S", "Th").Replace("Z", "th").Replace("z", "th");
                        }
                        var newMessage = string.Format(message, hint);
                        translationDictionary[translationKey] = newMessage;
                    }
                }
            }
            translationDictionary = HandleGroupedMessages(translationDictionary);
            translationDictionaryField.SetValue(dialogueManager, translationDictionary);
        }

        public static void UpdateDialogueToHaveHints()
        {
            foreach (var hintPair in ArchipelagoClient.ServerData.HintLookup)
            {
                if (Dialogue.RelevantItemToRelevantKeys.TryGetValue(hintPair.Key, out var relevantTranslations))
                {
                    var hint = hintPair.Value;
                    foreach (var translationKey in relevantTranslations)
                    {
                        if (!Dialogue.TranslationKeytoHintMessage.TryGetValue(translationKey, out var message))
                        {
                            continue;
                        }
                        if (translationKey == "SubPlague.1.4")
                        {
                            hint.Replace("ss", "th").Replace("s", "th").Replace("S", "Th").Replace("Z", "th").Replace("z", "th");
                        }
                        var newMessage = string.Format(message, hint);
                        CurrentGameHintDialogue[translationKey] = newMessage;
                    }
                }
            }
            CurrentGameHintDialogue = HandleGroupedMessages(CurrentGameHintDialogue);
        }

        private static Dictionary<string, string> HandleGroupedMessages(Dictionary<string, string> translationDictionary)
        {
            var unluckyCat = "UnluckyCat.1.2";
            var summonStone = "PhilosopherStone.1.3";
            var catDialogueFormat = Dialogue.TranslationKeytoHintMessage[unluckyCat];
            var soul2Hint = ArchipelagoClient.ServerData.HintLookup["Soul Fragment 2"];
            var soul3Hint = ArchipelagoClient.ServerData.HintLookup["Soul Fragment 3"];
            var summonMessageFormat = Dialogue.TranslationKeytoHintMessage[summonStone];
            var summon2Hint = ArchipelagoClient.ServerData.HintLookup["Summon Stone 2"];
            var summon3Hint = ArchipelagoClient.ServerData.HintLookup["Summon Stone 3"];
            var unluckyMessage2 = string.Format(catDialogueFormat, soul2Hint, soul3Hint);
            var summonMessage = string.Format(summonMessageFormat, summon2Hint, summon3Hint);
            translationDictionary[unluckyCat] = unluckyMessage2;
            translationDictionary[summonStone] = summonMessage;
            return translationDictionary;
        }
    }
}