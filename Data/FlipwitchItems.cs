using System.Collections.Generic;
using Archipelago.MultiClient.Net.Enums;

namespace FlipwitchAP
{
    public static class FlipwitchItems
    {
        public static readonly List<string> SkipPopupItems = new()
        {
            "Lucky Coin", "Woods Crystal Blockade Removal", "City Crystal Blockade Removal", "Loose Change",
            "Healing Surge", "Mana Surge", "Peachy Peach Recharge", "Sexual Thoughts", "Mana Upgrade", "Health Upgrade", 
            "Wand Upgrade", "Protective Barrier Upgrade", "Peachy Peach Charge", "Peachy Peach Upgrade",
            "Animal Coin", "Bunny Coin", "Monster Coin", "Angel & Demon Coin", "Promotional Coin", "Sexual Experience"
        };

        public static readonly List<string> QuestItems = new()
        {
            "FairyBubble", "BewitchedBubble", "GoblinHeadshot", "GoblinBusinessCard", "GoblinModelLuggage", "MimicKey", "DeliciousMilk", 
            "GlassWand", "BelleMilkshake", "CatGirlsClothes", "MomoBotAdminPassword", "Cowbell", "LegendaryHalo", "Wine", 
            "CherryKey", "VIPKey", "AngelLetter", "DemonLetter", "FungalKey", "FungalForestDeed", "AbandonedApartmentKey", 
            "HellishDango", "HeavenlyDaikon", "SummonStone1", "SummonStone2", "SummonStone3", "SoulFragment1", "SoulFragment2", "SoulFragment3", 
            "BunnyCostume", "MaidContract", "SilkySlime"
        };

        public static readonly Dictionary<string, string> APItemToGameName = new()
        {
            {"Nothing", "Blank"},
            {"Fairy Bubble", "FairyBubble"},
            {"Peachy Peach", "PeachyPeach"},
            {"Goblin Bomb", "GoblinBomb"},
            {"Demonic Cuff", "DemonicCuff"},
            {"Magical Mushroom", "MagicalMushroom"},
            {"Harpy Feather", "HarpyFeather"},
            {"Slime Sentry", "SlimySentry"},
            {"Frilly Panties", "FrillyPanties"},
            {"Yellow Frog Talisman", "FirstFrogTalisman"},
            {"Blue Frog Talisman", "SecondFrogTalisman"},
            {"Cursed Talisman", "CursedTalisman"},
            {"Disarming Bell", "DisarmingBell"},
            {"Ring Of The Sun", "RingofSun"},
            {"Ring Of The Moon", "RingofMoon"},
            {"Sacrificial Dagger", "SacrificialDagger"},
            {"Bewitched Bubble", "BewitchedBubble"},
            {"Goblin Crystal", "GoblinCrystal"},
            {"Ghostly Castle Key", "GhostCastleKey"},
            {"Magnetic Hairpin", "MagneticHairpin"},
            {"Demon Wings", "DemonDash"},
            {"Angel Feathers", "AngelJump"},
            {"Alchemist Costume", "AlchemistCostume"},
            {"Red Wizard Costume", "RedWizardCostume"},
            {"Navy Witch Costume", "BlackWitchCostume"},
            {"Bunny Costume", "BunnyCostume"},
            {"Cat Costume", "CatCostume"},
            {"Fairy Costume", "FairyCostume"},
            {"Maid Costume", "MaidCostume"},
            {"Miko Costume", "MikoCostume"},
            {"Nun Costume", "NunCostume"},
            {"Nurse Costume", "NurseCostume"},
            {"Dominating Costume", "PoliceCostume"},
            {"Angler Costume", "AnglerCostume"},
            {"Farmer Costume", "FarmerCostume"},
            {"Goblin Costume", "GoblinCostume"},
            {"Pigman Costume", "PigCostume"},
            {"Postman Costume", "PostmanCostume"},
            {"Rat Costume", "RatCostume"},
            {"Priest Costume", "PriestCostume"},
            {"Goblin Headshot", "GoblinHeadshot"},
            {"Business Card", "GoblinBusinessCard"},
            {"Goblin Apartment Key", "GoblinModelApartmentKey"},
            {"Gobliana's Luggage", "GoblinModelLuggage"},
            {"Mimic Chest Key", "MimicKey"},
            {"Delicious Milk", "DeliciousMilk"},
            {"Blue Jelly Mushroom", "GlassWand"},
            {"Belle's Milkshake", "BelleMilkshake"},
            {"Bundle of Clothes", "CatGirlsClothes"},
            {"Momo Server Admin Password", "MomoBotAdminPassword"},
            {"Belle's Cowbell", "Cowbell"},
            {"Legendary Halo", "LegendaryHalo"},
            {"Red Wine", "Wine"},
            {"Cherry Apartment Key", "CherryKey"},
            {"VIP Key", "VIPKey"},
            {"Frog Boss Key", "FrogBossKey"},
            {"Rundown House Key", "RundownHouseKey"},
            {"Rose Garden Key", "RoseGardenKey"},
            {"Secret Garden Key", "SecretGardenKey"},
            {"Mermaid Scale", "MermaidScale"},
            {"Slime Boss Key", "SlimeBossKey"},
            {"Collapsed Temple Key", "CollapsedTempleKey"},
            {"Demon Boss Key", "DemonBossKey"},
            {"The Beast's Key", "DemonSubBossKey"},
            {"Demon Club Door Key", "ClubDoor1Key"},
            {"Secret Club Door Key", "ClubDoor2Key"},
            {"Angel Letter", "AngelLetter"},
            {"Demon Letter", "DemonLetter"},
            {"Haunted Scythe", "HauntedScythe"},
            {"Fortune Cat", "FortuneCat"},
            {"Heart Necklace", "HeartNecklace"},
            {"Star Bracelet", "StarBracelet"},
            {"Mind Mushroom", "MindMushroom"},
            {"Maid Contract", "MaidContract"},
            {"Fungal Key", "FungalKey"},
            {"Deed to Fungal Forest", "FungalForestDeed"},
            {"Forgotten Fungal Door Key", "ForgottenFungalDoorKey"},
            {"Slime Citadel Key", "SlimeCitadelKey"},
            {"Goblin Queen Key", "GoblinBossKey"},
            {"Silky Slime", "SilkySlime"},
            {"Chaos Sanctum Key", "ChaosSanctumKey"},
            {"Ghost Form", "GhostTransform"},
            {"Slime Form", "SlimeTransform"},
            {"Abandoned Apartment Key", "AbandonedApartmentKey"},
            {"Hellish Dango", "HellishDango"},
            {"Heavenly Daikon", "HeavenlyDaikon"},
            {"Slimy Sub Boss Key", "SlimySubBossKey"},
            {"Portable Portal", "PortablePortal"},
            {"Flutterknife Garter", "FlutterknifeGarter"},
            {"Chaos Key Piece", "ChaosKey1"},
            {"Summon Stone", "SummonStone1"},
            {"Soul Fragment", "SoulFragment1"},
        };

        public static readonly Dictionary<string, string> APItemToCustomDescription = new()
        {
            {"Woods Crystal Blockade Removal", "The energies of the Great Fairy!  I can sense a crystal shatter."},
            {"City Crystal Blockade Removal", "The power of the Queen Goblin!  I can sense several crystals shattering."},
            {"Animal Coin", "I can use this on the gacha machines!"},
            {"Bunny Coin", "I can use this on the gacha machines!"},
            {"Monster Coin", "I can use this on the gacha machines!"},
            {"Angel & Demon Coin", "I can use this on the gacha machines!"},
            {"Promotional Coin", "I can use this on the gacha machines!"},
            {"Loose Change", "Its quite a bit of gold!"},
            {"Health Upgrade", "I feel a lot tougher!"},
            {"Mana Upgrade", "I feel a lot smarter!"},
            {"Wand Upgrade", "I feel power swelling within my wand!"},
            {"Healing Surge", "My wounds are completely healed!"},
            {"Mana Surge", "I can cast my magicks once again!"},
            {"Peachy Peach Recharge", "My Peachy Peach was fully restored!"},
            {"Peachy Peach Charge", "My Peachy Peach has gained another charge!"},
            {"Peachy Peach Upgrade", "My Peachy Peach is even more powerful!"},
            {"Sexual Thoughts", "I really can't stop thinking about that one time..."},
            {"Protective Barrier Upgrade", "I can feel a lustful aura surround me!"},
            {"Sexual Experience", "Wow, they really showed me a good time~"}
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