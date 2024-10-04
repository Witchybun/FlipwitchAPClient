using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using FlipwitchAP.Archipelago;
using Newtonsoft.Json;

namespace FlipwitchAP.Data
{ 
    public class ReceivedItem
    {
        public string Game { get; }
        public string LocationName { get; }
        public string ItemName { get; }
        public string PlayerName { get; }
        public long LocationId { get; }
        public long ItemId { get; }
        public long PlayerId { get; }
        public ItemFlags Classification { get; }

        [JsonConstructor]
        public ReceivedItem(string gameName, string locationName, string itemName, string playerName, long locationId, long itemId,
            long playerId, ItemFlags classification)
        {
            Game = gameName;
            LocationName = locationName;
            ItemName = itemName;
            PlayerName = playerName;
            LocationId = locationId;
            ItemId = itemId;
            PlayerId = playerId;
            Classification = classification;
        }

        public ReceivedItem(ItemInfo item)
        {
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