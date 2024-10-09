using System;
using System.Collections.Generic;
using FlipwitchAP.Archipelago;
using UnityEngine.UI;

namespace FlipwitchAP.Data
{
    public class ItemIconDatabase
    {
        public static readonly Dictionary<string, string> CustomItemToRealItem = new(){
            {"Lucky Coin", "FortuneCat"}, 
            {"Woods Crystal Blockade Removal", "GoblinCrystal"},
            {"City Crystal Blockade Removal", "GoblinCrystal"},
            {"Loose Change", "GoblinModelLuggage"},
            {"Health Upgrade", "HeartGem"},
            {"Mana Upgrade", "StarGem"},
            {"Wand Upgrade", "GlassWand"},
            {"Healing Surge", "HeartGem"},
            {"Mana Surge", "StarGem"},
            {"Peachy Peach Recharge", "PeachyPeach"},
            {"Sexual Thoughts", "GlassWand"},
            {"Peachy Peach Upgrade", "PeachyPeach"},
        };

        private static readonly List<string> Rings = new()
        {
            "RingofSun", "RingofMoon"
        };

        private static readonly List<string> Masks = new()
        {
            
            "AlchemistCostume", "RedWizardCostume", "BlackWitchCostume", "BunnyCostume", "CatCostume", "FairyCostume", 
            "MaidCostume", "MikoCostume", "NunCostume", "NurseCostume", "PoliceCostume", "AnglerCostume",
            "FarmerCostume", "GoblinCostume", "PigCostume", "PostmanCostume", "RatCostume", "PriestCostume"
        };

        private static readonly List<string> Talismans = new()
        {
            "CursedTalisman", "FirstFrogTalisman", "SecondFrogTalisman"
        };

        private static readonly List<string> Letters = new()
        {
            "AngelLetter", "DemonLetter"
        };

        public static string GiveSpecialIconGivenItemname(string itemName)
        {
            var random = new Random(ArchipelagoClient.ServerData.Seed + DateTime.Now.Millisecond);
            if (itemName.Contains("Sword", StringComparison.OrdinalIgnoreCase) || itemName.Contains("Dagger") || itemName.Contains("Blade"))
            {
                return "SacrificialDagger";
            }
            else if (itemName.Contains("Scythe"))
            {
                return "HauntedScythe";
            }
            else if (itemName.Contains("Rose") || itemName.Contains("Scissors") || itemName.Contains("Cutter"))
            {
                return "RoseCutters";
            }
            else if (itemName.Contains("Bunny Hood"))
            {
                return "BunnyCostume";
            }
            else if (itemName.Contains("Mask"))
            {
                return Masks[random.Next(0, Masks.Count - 1)];
            }
            else if (itemName.Contains("Talisman"))
            {
                return Talismans[random.Next(0, Talismans.Count - 1)];
            }
            else if (itemName.Contains("Letter"))
            {
                return Letters[random.Next(0, Letters.Count - 1)];
            }
            else if (itemName.Contains("Boss Key") || itemName.Contains("Big Key"))
            {
                return "DemonBossKey";
            }
            else if (itemName.Contains("Key"))
            {
                return "RundownHouseKey";
            }
            else if (itemName.Contains("Heart"))
            {
                return "HeartGem";
            }
            else if (itemName.Contains("Milk") || itemName.Contains("Romani"))
            {
                return "BelleMilkshake";
            }
            else if (itemName.Contains("Wine") || itemName.Contains("Elixir"))
            {
                return "Wine";
            }
            else if (itemName.Contains("Potion"))
            {
                return "SlimeSentry";
            }
            else if (itemName.Contains("Bottle"))
            {
                return "DeliciousMilk";
            }
            else if (itemName.Contains("Ring"))
            {
                return Rings[random.Next(0, Rings.Count - 1)];
            }
            else if (itemName.Contains("Orb") || itemName.Contains("Pearl"))
            {
                return "FairyBubble";
            }
            else if (itemName.Contains("Deed") || itemName.Contains("Paper"))
            {
                return "FungalForestDeed";
            }
            else if (itemName.Contains("Mushroom"))
            {
                return "MagicalMushroom";
            }
            else if (itemName.Contains("Briefcase") || itemName.Contains("Luggage"))
            {
                return "GoblinModelLuggage";
            }
            else if (itemName.Contains("Feather"))
            {
                return "HarpyFeather";
            }
            else if (itemName.Contains("Bag"))
            {
                return "UnstableBomb";
            }
            else if (itemName.Contains("Bomb"))
            {
                return "GoblinBomb";
            }
            else if (itemName.Contains("Scale"))
            {
                return "MermaidScale";
            }
            else if (itemName.Contains("Halo"))
            {
                return "LegendaryHalo";
            }
            else if (itemName.Contains("Card"))
            {
                return "GoblinBusinessCard";
            }
            else if (itemName.Contains("Picture"))
            {
                return "GoblinHeadshot";
            }
            else if (itemName.Contains("Gun"))
            {
                return "ArcaneCannon";
            }
            else if (itemName.Contains("Crystal"))
            {
                return "Goblin Crystal";
            }
            else if (itemName.Contains("Fruit") || itemName.Contains("Sapling"))
            {
                return "PeachyPeach";
            }
            else if (itemName.Contains("Vegetable") || itemName.Contains("Seed"))
            {
                return "HeavenlyDaikon";
            }
            else if (itemName.Contains("Slime") || itemName.Contains("Goo"))
            {
                return "SlimeTransform";
            }
            else if (itemName.Contains("Soul"))
            {
                return "GhostForm";
            }
            else if (itemName.Contains("Hairpin"))
            {
                return "MagneticHairpin";
            }
            else if (itemName.Contains("Flame"))
            {
                return "DemonicCuff";
            }
            else if (itemName.Contains("Panties"))
            {
                return "FrillyPanties";
            }
            else if (itemName.Contains("Opal"))
            {
                return "MermaidScale";
            }
            else if (itemName.Contains("Wing"))
            {
                return "DemonDash";
            }
            else if (itemName.Contains("Butterfly"))
            {
                return "FlutterknifeGarter";
            }
            else if (itemName.Contains("Star"))
            {
                return "StarGem";
            }
            return "BewitchedBubble";
        }
    }
}