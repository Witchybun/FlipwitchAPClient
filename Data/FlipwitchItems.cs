using System.Collections.Generic;
using Archipelago.MultiClient.Net.Enums;

namespace FlipwitchAP
{
    public static class FlipwitchItems
    {
        public static readonly List<string> SkipPopupItems = new()
        {
            "Lucky Coin", "Special Promotion #1", "Crystal Blockade Removal"
        };

        public static readonly Dictionary<string, string> APItemToGameName = new()
        {
            {"Navy Witch Costume", "BlackWitchCostume"},
            {"Fairy Bubble", "FairyBubble"},
            {"Bewitched Bubble", "BewitchedBubble"},
            {"Belle's Cowbell", "Cowbell"},
            {"Rundown House Key", "RundownHouseKey"},
            {"Peachy Peach", "PeachyPeach"},

        };

        public static readonly Dictionary<string, string> APItemToCustomDescription = new()
        {
            {"Crystal Blockade Removal", "The energies of the Great Fairy!  I can sense a crystal shatter."},
            {"Special Promotion #1", "Its a lewd gachapon."},
            {"Lucky Coin", "I can use this on the gacha machines!"},
        };


        public static readonly Dictionary<ItemFlags, string> ClassificationToUseBlurb = new()
        {
            {ItemFlags.Advancement, "Seems really important!"},
            {ItemFlags.NeverExclude, "Seems pretty useful."},
            {ItemFlags.None, "Not all that useful..."},
            {ItemFlags.Trap, "Oh, fuck..."},
        };
    }
}