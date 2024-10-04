using FlipwitchAP.Data;
using FlipwitchAP.Archipelago;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace FlipwitchAP
{
    public class ItemHelper
    {
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        public ItemHelper()
        {
            Harmony.CreateAndPatchAll(typeof(ItemHelper));
        }
        public static void HandleReceivedItems()
        {
            bool shitToDo = ArchipelagoClient.ItemsToProcess.Count() != 0;
            while (shitToDo)
            {
                var item = ArchipelagoClient.ItemsToProcess.Dequeue();
                if (item.LocationId >= 0 && !ArchipelagoClient.ServerData.ReceivedItems.Any(x => x.PlayerId == item.PlayerId && x.LocationId == item.LocationId && item.ItemId == x.ItemId))
                {
                    ArchipelagoClient.ServerData.ReceivedItems.Add(item);
                    GiveFlipwitchItem(item);
                }
                /*else if (item.LocationId < 0)
                {
                    _log.LogInfo($"Cheated Count {cheatedCount} before increment vs {ConnectionData.CheatedCount}");
                    cheatedCount++;
                    if (cheatedCount > ConnectionData.CheatedCount)
                    {
                        ConnectionData.CheatedCount = cheatedCount;
                        ConnectionData.ReceivedItems.Add(item);
                        GiveFlipwitchItem(item);
                    }
                }*/
                shitToDo = ArchipelagoClient.ItemsToProcess.Count() != 0;
            }
        }

        public static void GiveFlipwitchItem(string name, bool skipPopup = false)
        {
            switch (name)
            {
                case "Nothing":
                    {
                        return;
                    }
                case "Lucky Coin":
                    {
                        SwitchDatabase.instance.addTokenToTokenCount(1);
                        return;
                    }
                case "Special Promotion #1":
                    {
                        SwitchDatabase.instance.gachaCollections.Find(x => x.collection == GachaCollections.Promotion).gachas[0].unlocked = true;
                        return;
                    }
                case "Crystal Blockade Removal":
                    {
                        var ints = (Dictionary<string, int>)SwitchDatabase.instance.GetType().GetField("ints", Flags).GetValue(SwitchDatabase.instance);
                        if (ints.ContainsKey("GreatFairyStory"))
                        {
                            ints["GreatFairyStory"] += 1;
                        }
                        else
                        {
                            ints["GreatFairyStory"] = 1;
                        }
                        SwitchDatabase.instance.GetType().GetField("ints", Flags).SetValue(SwitchDatabase.instance, ints);
                        return;
                    }
                case "Peachy Peach":
                    {
                        var ints = (Dictionary<string, int>)SwitchDatabase.instance.GetType().GetField("ints", Flags).GetValue(SwitchDatabase.instance);
                        ints["PeachGiven"] += 1;
                        SwitchDatabase.instance.GetType().GetField("ints", Flags).SetValue(SwitchDatabase.instance, ints);
                        SwitchDatabase.instance.updatePeachyPeachUI();
                        SwitchDatabase.instance.givePlayerItem("PeachyPeach", false);
                        return;
                    }
            }
            var trueName = FlipwitchItems.APItemToGameName.ContainsKey(name) ? FlipwitchItems.APItemToGameName[name] : name;
            SwitchDatabase.instance.givePlayerItem(trueName, skipPopup);
        }

        public static void GiveFlipwitchItem(ReceivedItem item)
        {
            var isNotReal = FlipwitchItems.SkipPopupItems.Contains(item.ItemName);
            var skipPopup = isNotReal || item.PlayerName == ArchipelagoClient.ServerData.SlotName;
            Plugin.Logger.LogInfo($"Are we skipping this?  {skipPopup}");
            GiveFlipwitchItem(item.ItemName, skipPopup);
            return;
        }

        [HarmonyPatch(typeof(PlayerMovement), "disableMovement")]
        [HarmonyPostfix]
        private static void DisableMovement_NoteChangeWithoutReflectionSpam()
        {
            Plugin.IsMovementDisabled = true;
        }

        [HarmonyPatch(typeof(PlayerMovement), "movementEnabled")]
        [HarmonyPostfix]
        private static void MovementEnabled_NoteChangeWithoutReflectionSpam()
        {
            Plugin.IsMovementDisabled = false;
        }

    }
}