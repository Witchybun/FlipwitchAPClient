using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace FlipwitchAP.Data
{
    public static class FlipwitchLocations
    {
        public const long LOCATION_INIT_ID = 0;
        public static Dictionary<string, LocationData> ChestLocations = new();
        public static Dictionary<string, LocationData> CutsceneLocations = new();
        public static Dictionary<string, LocationData> CoinLocations = new();
        public static Dictionary<string, LocationData> GachaLocations = new();
        public static Dictionary<string, LocationData> QuestLocations = new();
        public static Dictionary<string, LocationData> ShopLocations = new();
        public static Dictionary<string, LocationData> SecondaryCallLocations = new();

        public class LocationData
        {
            public long APLocationID { get; set; }
            public string APLocationName { get; set; }
            public string PrimaryCallName { get; set; }
            public string SecondaryCallName { get; set; }
            public string Type { get; set; }
            public bool IgnoreLocationHandler { get; set; }

            public LocationData(long locationID, string apLocationName, string primaryCallName, string type, bool ignoreLocationHandler = false, string secondaryCallName = "")
            {
                APLocationID = LOCATION_INIT_ID + locationID;
                APLocationName = apLocationName;
                PrimaryCallName = primaryCallName;
                SecondaryCallName = secondaryCallName;
                Type = type;
                IgnoreLocationHandler = ignoreLocationHandler;

            }

            public LocationData()
            {
                APLocationID = -1;
                APLocationName = null;
                PrimaryCallName = null;
                SecondaryCallName = null;
                Type = null;
                IgnoreLocationHandler = true;
            }
        }

        public static readonly List<LocationData> APLocationData = new()
        {
            CreateLocation(1, "WW: Beatrix's Introduction Gift", "PeachGiven", "Cutscene", secondaryCallName: "PeachyPeach"),
            CreateLocation(2, "WW: Rundown House Chest", "RundownHouseKey", "Chest"),
            CreateLocation(3, "WW: Rescue Fairy", "WW_CrystalBreakTriggered", "Cutscene"),
            CreateLocation(4, "WW: Chest Guarded by Rundown House", "Cowbell", "Chest"),
            CreateLocation(51, "WW: Above Lucky Machine", "gacha_tutorial", "Gacha Token"),
            CreateLocation(52, "WW: Above Rundown House", "ww_gacha_cowjunction", "Gacha Token"),
            CreateLocation(53, "WW: Last Pon in Gacha Machine", "Gachas_Promotion_1", "Gacha Machine"),
            CreateLocation(101, "Quest: Magic Mentor", "Magical Mentor", "Quest", secondaryCallName: "BewitchedBubble"),
            CreateLocation(102, "Quest: Need My Cowbell", "Need My Cowbell", "Quest", ""),
            CreateLocation(151, "ST: Outside Shop Only Item", "FairyBubble", "Shop"),
            CreateLocation(201, "WW: Home Spare Clothes", "BlackWitchCostume", "Chest"),
        };

        private static LocationData CreateLocation(int id, string locationName, string primaryCallName, string type, string secondaryCallName = "")
        {
            var location = new LocationData(id, locationName, primaryCallName, type, secondaryCallName: secondaryCallName);
            if (secondaryCallName != "")
            {
                SecondaryCallLocations[secondaryCallName] = location;
            }
            switch (type)
            {
                case "Chest":
                    {
                        ChestLocations[primaryCallName] = location;
                        break;
                    }
                case "Cutscene":
                    {
                        CutsceneLocations[primaryCallName] = location;
                        break;
                    }
                case "Gacha Token":
                    {
                        CoinLocations[primaryCallName] = location;
                        break;
                    }
                case "Gacha Machine":
                    {
                        GachaLocations[primaryCallName] = location;
                        break;
                    }
                case "Shop":
                    {
                        ShopLocations[primaryCallName] = location;
                        break;
                    }
                case "Quest":
                    {
                        QuestLocations[primaryCallName] = location;
                        break;
                    }
            }
            return location;
        }
    }
}