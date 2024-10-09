using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using FlipwitchAP.Archipelago;
using Newtonsoft.Json;

namespace FlipwitchAP.Data
{ 
    public class ReceivedItem
    {
        public int Index {get; }
        public string Game { get; }
        public string LocationName { get; }
        public string ItemName { get; }
        public string PlayerName { get; }
        public long LocationId { get; }
        public long ItemId { get; }
        public long PlayerId { get; }
        public ItemFlags Classification { get; }

        public ReceivedItem(ItemInfo item, int index)
        {
            Index = index;
            var playerName = Plugin.ArchipelagoClient.GetPlayerNameFromSlot(item.Player);
            Game = item.ItemGame;
            LocationName = item.LocationName;
            ItemName = item.ItemName;
            PlayerName = playerName;
            LocationId = item.LocationId;
            ItemId = item.ItemId;
            PlayerId = item.Player;
            Classification = item.Flags;
        }
    }
}