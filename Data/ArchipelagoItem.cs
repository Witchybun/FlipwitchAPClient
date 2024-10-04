using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using FlipwitchAP.Archipelago;
using Newtonsoft.Json;

namespace FlipwitchAP.Data
{
    public class ArchipelagoItem
    {
        public long ID;
        public string Name;
        public int SlotID;
        public string SlotName;
        public string Game;
        public ItemFlags Classification;
        public bool IsOwnItem;

        public ArchipelagoItem(ScoutedItemInfo item, bool received)
        {
            ID = item.ItemId;
            Name = item.ItemName;
            SlotID = item.Player.Slot;
            SlotName = Plugin.ArchipelagoClient.GetPlayerNameFromSlot(SlotID);
            Game = item.ItemGame;
            Classification = item.Flags;
            IsOwnItem = SlotName == ArchipelagoClient.ServerData.SlotName;
        }

        [JsonConstructor]
        public ArchipelagoItem(long id, string name, int slotID, string slotName, string game, ItemFlags classification, bool isOwnItem)
        {
            ID = id;
            Name = name;
            SlotID = slotID;
            SlotName = slotName;
            Game = game;
            Classification = classification;
            IsOwnItem = isOwnItem;
        }
    }
}