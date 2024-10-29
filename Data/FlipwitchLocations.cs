using System.Collections.Generic;

namespace FlipwitchAP.Data
{
    public static class FlipwitchLocations
    {
        public const long LOCATION_INIT_ID = 0;
        public static Dictionary<string, LocationData> NameToLocation = new();
        public static Dictionary<string, LocationData> ChestLocations = new();
        public static Dictionary<string, LocationData> CoinChestLocations = new();
        public static Dictionary<string, LocationData> CutsceneLocations = new();
        public static Dictionary<string, LocationData> CoinLocations = new();
        public static Dictionary<string, LocationData> GachaLocations = new();
        public static Dictionary<string, LocationData> QuestLocations = new();
        public static Dictionary<string, LocationData> ShopLocations = new();
        public static Dictionary<string, LocationData> StatLocations = new();
        public static Dictionary<string, LocationData> SecondaryCallLocations = new();
        public static Dictionary<string, LocationData> SexExperienceLocations = new();
        public static Dictionary<long, LocationData> IDToLocation = new();

        private const string CHEST = "Chest";
        private const string COINCHEST = "Coin Chest";
        private const string CUTSCENE = "Cutscene";
        private const string QUEST = "Quest";
        private const string STAT = "Stat";
        private const string SEX = "Sex Experience";
        private const string GACHA = "Gacha";
        private const string GACHAMACHINE = "Gacha Machine";
        private const string SHOP = "Shop";

        public class LocationData
        {
            public long APLocationID { get; set; }
            public string APLocationName { get; set; }
            public string PrimaryCallName { get; set; }
            public string SecondaryCallName { get; set; }
            public string Type { get; set; }
            public string SwitchToFlip { get; set; }
            public bool IgnoreLocationHandler { get; set; }

            public LocationData(long locationID, string apLocationName, string primaryCallName, string type, string switchToFlip = "", bool ignoreLocationHandler = false, string secondaryCallName = "")
            {
                APLocationID = LOCATION_INIT_ID + locationID;
                APLocationName = apLocationName;
                PrimaryCallName = primaryCallName;
                SecondaryCallName = secondaryCallName;
                Type = type;
                SwitchToFlip = switchToFlip;
                IgnoreLocationHandler = ignoreLocationHandler;

            }

            public LocationData()
            {
                APLocationID = -1;
                APLocationName = null;
                PrimaryCallName = null;
                SecondaryCallName = null;
                Type = null;
                SwitchToFlip = null;
                IgnoreLocationHandler = true;
            }
        }

        public static readonly List<LocationData> APLocationData = new()
        {
            CreateLocation(1, "WW: Feminine Spare Clothes Chest", "BlackWitchCostume", CHEST),
            CreateLocation(2, "WW: Masculine Spare Clothes Chest", "RedWizardCostume", CHEST),
            CreateLocation(3, "WW: Beatrix's Introduction Gift", "PeachGiven", CUTSCENE, secondaryCallName: "PeachyPeach", switchToFlip: "APPeachItemGiven"),
            CreateLocation(4, "WW: Rundown House Save Hidden Chest", "ww_tiny_secret_coins", COINCHEST),
            CreateLocation(5, "WW: Mimic Chest Key Room Chest 1", "ww_coins_savestatue", COINCHEST),
            CreateLocation(6, "WW: Secret Cave Chest", "ww_double_jump_coins", COINCHEST),
            CreateLocation(7, "WW: Mimi's Hidden Chest", "ww_mimic_coins", COINCHEST),
            CreateLocation(8, "WW: Rundown House Chest", "RundownHouseKey", CHEST),
            CreateLocation(9, "WW: Past Man Cave Chest", "gc_coins_highjump", COINCHEST),
            CreateLocation(10, "WW: Gerry G. Atric's Chest", "GoblinBomb", CHEST),
            CreateLocation(11, "WW: Hidden Alcove", "ww_secret_nook_coins", COINCHEST),
            CreateLocation(12, "WW: Beside Flip Platform", "ww_ladderroom_coins", COINCHEST),
            CreateLocation(13, "WW: After Chaos Fight Chest", "ww_horseshoe_coins", COINCHEST),
            CreateLocation(14, "WW: Boss Key Room Chest", "GoblinCrystal", CHEST),
            CreateLocation(15, "WW: Boss Key Room Big Chest", "GoblinBossKey", CHEST),
            CreateLocation(16, "WW: Goblin Queen Chaos Piece", "ChaosKey1", CHEST),
            CreateLocation(17, "WW: Flying Fairy Chest", "MagneticHairpin", CHEST),
            CreateLocation(18, "WW: Great Fairy's Reward", "Dummy2", QUEST, secondaryCallName: "FairyCostume"),

            CreateLocation(31, "ST: Shop Roof Chest", "spiritcity_itemshop_coins", COINCHEST),
            CreateLocation(32, "ST: Ancient Being Chest", "sc_democracy_coins", COINCHEST),
            CreateLocation(33, "ST: Ghostly Castle Key Chest", "GhostCastleKey", CHEST),
            CreateLocation(34, "ST: Below Cemetery", "sc_waterfall_coins", COINCHEST),
            CreateLocation(35, "ST: Banana Apartment", "sc_coins_jackpot", COINCHEST),
            CreateLocation(36, "ST: Abandoned Homes Green House Chest", "sc_interiorslime_coins", COINCHEST),
            CreateLocation(37, "ST: Lone House Chest", "sc_angeljumpinterior_coins", COINCHEST),

            CreateLocation(61, "SS: Hidden Pipe Chest", "scp_hiddencoins", COINCHEST),
            CreateLocation(62, "SS: Side Room Chest", "scp_pipearches_coins", COINCHEST),
            CreateLocation(63, "SS: Rat Costume Chest 1", "scp_lastpipe_coins3", COINCHEST),
            CreateLocation(64, "SS: Rat Costume Chest 2", "RatCostume", CHEST),
            CreateLocation(65, "SS: Rat Costume Chest 3", "scp_lastpipe_coins2", COINCHEST),
            CreateLocation(66, "SS: Shady Gauntlet Chest", "scp_tallpipe_coin", COINCHEST),
            CreateLocation(67, "SS: Elf Merchant Chest", "sc_smallpipecorner_coins", COINCHEST),
            CreateLocation(68, "SS: Dr. Witch Doctor Big Chest", "MermaidScale", CHEST),
            CreateLocation(69, "SS: Dr. Witch Doctor Tutorial Chest", "sc_coins_mermaid_tutorial", COINCHEST),

            CreateLocation(91, "GC: Below Entrance Chest", "gc_crypt_coins", COINCHEST),
            CreateLocation(92, "GC: Slime Form Room Chest 1", "gc_slime_secret_coins_upper", COINCHEST), // All three chests have the same switchName?
            CreateLocation(93, "GC: Slime Form Room Chest 2", "gc_slime_secret_coins", COINCHEST), // All three chests have the same switchName?
            CreateLocation(94, "GC: Slime Form Room Chest 3", "gc_slime_secret_coins2", COINCHEST), // All three chests have the same switchName?
            CreateLocation(95, "GC: Giant Room, Flip Magic Platform", "RoseGardenKey", CHEST),
            CreateLocation(96, "GC: Hidden Ledge", "gc_entryway_coins", COINCHEST),
            CreateLocation(97, "GC: Elf Merchant Chest", "gc_largegardens_coins", COINCHEST),
            CreateLocation(98, "GC: Secret Garden Key Chest", "SecretGardenKey", CHEST),
            CreateLocation(99, "GC: Hidden Wall", "gc_windowroom_coins", COINCHEST),
            CreateLocation(100, "GC: Along the Path", "gc_mid_point_coins", COINCHEST),
            CreateLocation(101, "GC: Behind the Vines", "gc_ghost_hallway_coins", COINCHEST),
            CreateLocation(102, "GC: Hidden Shrub", "gc_dropdown_coins", COINCHEST),
            CreateLocation(103, "GC: Ghost Form Chest", "GhostTransform", CHEST),
            CreateLocation(104, "GC: Thimble Chest", "gc_savestatue_coins", COINCHEST),
            CreateLocation(105, "GC: Ghost Boss Chaos Key Piece", "ChaosKey2", CHEST),

            CreateLocation(121, "JG: Hidden Flip Magic Room", "j_entrance_coins", COINCHEST),
            CreateLocation(122, "JG: Slime Form Room", "j_superslimeysecret_coins", COINCHEST),
            CreateLocation(123, "JG: Early Ledge", "j_entranceway_coins", COINCHEST),
            CreateLocation(124, "JG: Secret Spring Room Chest", "j_piercing_earrings", COINCHEST),
            CreateLocation(125, "JG: Beast Key Chest", "DemonSubBossKey", CHEST),
            CreateLocation(126, "JG: Hidden Ledge", "j_lavariver_coins", COINCHEST),
            CreateLocation(127, "JG: Annahell Chest", "j_minishrine_coins", COINCHEST),
            CreateLocation(128, "JG: Hot Guy Chest", "CollapsedTempleKey", CHEST),
            CreateLocation(129, "JG: Far Ledge", "j_themound_coins", COINCHEST),
            CreateLocation(130, "JG: Hidden Flip Magic Ledge Chest", "j_lava_jump_coins", COINCHEST),
            CreateLocation(131, "JG: Demon Wings Chest", "DemonDash", CHEST),
            CreateLocation(132, "JG: Demon Wings Tutorial", "j_demondashtutorial_coins", COINCHEST),
            CreateLocation(133, "JG: Northern Cat Shrine Chest", "j_secret_blue_coins", COINCHEST),

            CreateLocation(151, "CD: Club Demon Entrance", "j_clubdemonentrance_coins", COINCHEST),
            CreateLocation(152, "CD: Under The Table", "j_clubentrance_coins", COINCHEST),
            CreateLocation(153, "CD: Club Door Room", "ClubDoor2Key", CHEST),
            CreateLocation(154, "CD: Flip Magic Chest", "j_purplestairs_coins", COINCHEST),
            CreateLocation(155, "CD: Club Key Room Chest", "ClubDoor1Key", CHEST),
            CreateLocation(156, "CD: Demon Boss Key Chest", "DemonBossKey", CHEST),
            CreateLocation(157, "CD: Demon Boss Chaos Key Piece", "ChaosKey3", CHEST),

            CreateLocation(181, "TG: Early Coins Chest", "tt_ladderstairs_coins", COINCHEST),
            CreateLocation(182, "TG: Hidden Foliage", "tt_gender_elevator_coins", COINCHEST),
            CreateLocation(183, "TG: Birby", "HarpyFeather", CHEST),
            CreateLocation(184, "TG: Flip Magic Chest", "tt_largetowerroom_coins", COINCHEST),
            CreateLocation(185, "TG: Secret Alcove", "tt_secretzone_coins", COINCHEST),

            CreateLocation(211, "AH: Starting Coins", "t_tengokuentrance_coins", COINCHEST),
            CreateLocation(212, "AH: Elf Merchant Chest", "t_behindcloud_coins", COINCHEST),
            CreateLocation(213, "AH: Thimble Chest", "FlutterknifeGarter", CHEST),
            CreateLocation(214, "AH: Below Thimble", "t_moregenders_coins", COINCHEST),
            CreateLocation(215, "AH: Gabrielle Chest", "t_switchystatue_coins", COINCHEST),
            CreateLocation(216, "AH: Behind The Vines", "t_mazeup_coins", COINCHEST),
            CreateLocation(217, "AH: Angel Feathers Chest", "AngelJump", CHEST),
            CreateLocation(218, "AH: Below Boss Room Chest", "t_flowergarden_coins", COINCHEST),
            CreateLocation(219, "AH: Angelica Chaos Key Piece", "ChaosKey4", CHEST),

            CreateLocation(241, "FF: Lower Pit", "ff_dropright_coins", COINCHEST),
            CreateLocation(242, "FF: Past Chaos Fight", "ff_movingplatforms_coins", COINCHEST),
            CreateLocation(243, "FF: Closed Off Coins", "ff_plummet_coins", COINCHEST),
            CreateLocation(244, "FF: Between The Thorns", "ff_circleback_coins", COINCHEST),
            CreateLocation(245, "FF: Elf Merchant Chest", "sc_woodplatform_coins", COINCHEST),
            CreateLocation(246, "FF: Guarded by Mushrooms", "ff_mushroomtower_coins", COINCHEST),
            CreateLocation(247, "FF: Fungal Door Key Chest", "ForgottenFungalDoorKey", CHEST),
            CreateLocation(248, "FF: Slime Form Chest", "SlimeTransform", CHEST),
            CreateLocation(249, "FF: Slime Citadel Key Chest", "SlimeCitadelKey", CHEST),
            CreateLocation(250, "FF: Slime Tutorial Chest", "sc_slimetutorial_coins", COINCHEST),

            CreateLocation(271, "SC: Citadel Entrance", "sc_slimeentrance_coins", COINCHEST),
            CreateLocation(272, "SC: Secret Room", "sc_supersecretcoins_100", COINCHEST),
            CreateLocation(273, "SC: Lone Room", "sc_supersmallslimesecret_coins", COINCHEST),
            CreateLocation(274, "SC: Slimy Sub Boss Key Chest", "SlimySubBossKey", CHEST),
            CreateLocation(275, "SC: Near Stone Chest", "sc_philosopher_coins", COINCHEST),
            CreateLocation(276, "SC: Hidden Tunnel", "sc_longsecret_coins", COINCHEST),
            CreateLocation(277, "SC: Slurp Slime Boss Key Chest", "SlimeBossKey", CHEST),
            CreateLocation(278, "SC: Slimy Princess Chaos Key Piece", "ChaosKey6", CHEST),

            CreateLocation(301, "UU: Save Coin Chest", "uu_coins_stair_jump", COINCHEST),
            CreateLocation(302, "UU: Angler Costume Chest", "AnglerCostume", CHEST),
            CreateLocation(303, "UU: Flip Magic Room Chest", "u_waterhall_coins", COINCHEST),
            CreateLocation(304, "UU: Sacrificial Dagger Chest", "SacrificialDagger", CHEST),
            CreateLocation(305, "UU: Mermaid's Chest", "uu_diving_coins", COINCHEST),
            CreateLocation(306, "UU: Chaos Fight Chest", "uu_surfacing_coins", COINCHEST),
            CreateLocation(307, "UU: Path to Octrina", "uu_coralzone_coins", COINCHEST),
            CreateLocation(308, "UU: Octrina Chest", "FrogBossKey", CHEST),
            CreateLocation(309, "UU: Frog Boss Chaos Key Piece", "ChaosKey5", CHEST),

            CreateLocation(331, "CC: Early Coins Chest", "cc_entryway_coins", COINCHEST),
            CreateLocation(332, "CC: Jigoku Path Chest", "cc_jigokupyramidcoins", COINCHEST),
            CreateLocation(333, "CC: Near Chaos Sanctum", "cc_secretgem_coins", COINCHEST),
            CreateLocation(334, "CC: Elf Merchant Chest", "cc_bluetriangle_coins", COINCHEST),
            CreateLocation(335, "CC: Slime Citadel Path Chest", "cc_slimepipes_coins", COINCHEST),
            CreateLocation(336, "CC: Big Jump Chest", "cc_umi_coins", COINCHEST),
            CreateLocation(337, "CC: Pandora Chaos Sanctum Key", "ChaosSanctumKey", CHEST),
            
            CreateLocation(361, "WW: Boss Key Room HP Upgrade", "105.96-117.866442_CaveHeartUndergroundHealth Upgrade (1)", STAT),
            CreateLocation(362, "WW: Goblin Queen MP Upgrade", "248.54-185.386447_GoblinTreasureUndergroundMana Upgrade", STAT),
            CreateLocation(363, "ST: Women's Bathroom HP Upgrade", "18.5853457.6282Bathroom_FemaleWorldHealth Upgrade", STAT),
            CreateLocation(364, "ST: Men's Bathroom MP Upgrade", "26.9015833.63543Bathroom_MaleWorldMana Upgrade", STAT),
            CreateLocation(365, "SS: Shady Gauntlet HP Upgrade", "209.78-159.5814_TallPipePipeworldHealth Upgrade", STAT),
            CreateLocation(366, "SS: Dr. Witch Doctor MP Upgrade", "313.94-208.99511_PipeChestPipeworldMana Upgrade", STAT),
            CreateLocation(367, "GC: Ghost Boss MP Upgrade", "-45.7701833.9395113_GhostTreasureWorldMana Upgrade", STAT),
            CreateLocation(368, "GC: Ghostly Gauntlet", "-86.04183.8935_BellTowerWorldHealth Upgrade", STAT),
            CreateLocation(369, "CD: Demonic Gauntlet", "-146.95-195.9256_BlueRewardClubDemonHealth Upgrade", STAT),
            CreateLocation(370, "CD: Demon Boss MP Upgrade", "36.98-152.5650_DemonBossRightClubDemonMana Upgrade", STAT),
            CreateLocation(371, "AH: Angelic Gauntlet", "-359.9889222.55928_TopStoneTengokuHealth Upgrade", STAT),
            CreateLocation(372, "AH: Angelica MP Upgrade", "42.41261.807710_ConnectorTengokuMana Upgrade", STAT),
            CreateLocation(373, "FF: Past Fungella", "238.5322-40.318542_TinyRoomWorldMana Upgrade (1)", STAT),
            CreateLocation(374, "FF: Fungal Gauntlet", "306.7147-38.2432619_MushroomClimbWorldHealth Upgrade (1)", STAT),
            CreateLocation(375, "SC: Slimy Gauntlet", "577.8729-152.997124_SlimyPrizeSlimeCitadelHealth Upgrade", STAT),
            CreateLocation(376, "SC: Slimy Princess MP Upgrade", "303.4169-222.008931_EndRoomSlimeCitadelMana Upgrade", STAT),
            CreateLocation(377, "UU: Watery Gauntlet", "-212.3212-150.174823_DeadEndWorldHealth Upgrade", STAT),
            CreateLocation(378, "UU: Frog Boss MP Upgrade", "82.6517-150.64234_FrogTreasureWorldMana Upgrade", STAT),
            CreateLocation(379, "CC: Big Jump HP Upgrade", "217.944.8911_CC_UmiChaosPyramidHealth Upgrade", STAT),
            CreateLocation(380, "CC: Pandora MP Upgrade", "450.011.37530_CC_SubBossChaosPyramidMana Upgrade", STAT),
            
            CreateLocation(401, "ST: Outside Shop Item", "FairyBubble", SHOP),
            CreateLocation(402, "ST: Shop Item 1", "FortuneCat", SHOP),
            CreateLocation(403, "ST: Shop Item 2", "HeartNecklace", SHOP),
            CreateLocation(404, "ST: Shop Item 3", "StarBracelet", SHOP),
            CreateLocation(405, "ST: Shop Item 4", "FrillyPanties", SHOP),
            CreateLocation(406, "ST: Fashion Shop 1", "CatCostume", SHOP),
            CreateLocation(407, "ST: Fashion Shop 2", "GoblinCostume", SHOP),
            CreateLocation(408, "SS: Elf Merchant Item 1", "CursedTalisman", SHOP),
            CreateLocation(409, "SS: Elf Merchant Item 2", "PortablePortal", SHOP),
            CreateLocation(410, "GC: Elf Merchant Item", "DisarmingBell", SHOP),
            CreateLocation(411, "GC: Thimble Costume 1", "NunCostume", SHOP),
            CreateLocation(412, "GC: Thimble Costume 2", "PriestCostume", SHOP),
            CreateLocation(413, "JG: Elf Merchant Item 1", "DemonicCuff", SHOP),
            CreateLocation(414, "JG: Elf Merchant Item 2", "FirstFrogTalisman", SHOP),
            CreateLocation(415, "CD: Thimble Costume 1", "MikoCostume", SHOP),
            CreateLocation(416, "CD: Thimble Costume 2", "FarmerCostume", SHOP),
            CreateLocation(417, "AH: Elf Merchant Item 1", "MindMushroom", SHOP),
            CreateLocation(418, "AH: Elf Merchant Item 2", "RingofMoon", SHOP),
            CreateLocation(419, "AH: Thimble Costume 1", "PostmanCostume", SHOP),
            CreateLocation(420, "AH: Thimble Costume 2", "NurseCostume", SHOP),
            CreateLocation(421, "FF: Thimble Costume 1", "MaidCostume", SHOP),
            CreateLocation(422, "FF: Thimble Costume 2", "PigCostume", SHOP),
            CreateLocation(423, "FF: Elf Merchant Item 1", "MagicalMushroom", SHOP),
            CreateLocation(424, "FF: Elf Merchant Item 2", "RingofSun", SHOP),
            CreateLocation(425, "SC: Thimble Costume 1", "PoliceCostume", SHOP),
            CreateLocation(426, "SC: Thimble Costume 2", "AlchemistCostume", SHOP),
            CreateLocation(427, "SC: Elf Merchant Item 1", "SlimySentry", SHOP),
            CreateLocation(428, "SC: Elf Merchant Item 2", "SecondFrogTalisman", SHOP),
            CreateLocation(429, "CC: Elf Merchant Item 1", "HauntedScythe", SHOP),

            CreateLocation(451, "Quest: Magic Mentor", "Magical Mentor", QUEST, secondaryCallName: "BewitchedBubble"),
            CreateLocation(452, "Quest: Need My Cowbell", "Need My Cowbell", QUEST),    
            CreateLocation(453, "Mimi: Giant Chest Key", "Giant Chest Key", QUEST),  
            CreateLocation(454, "Gobliana: A Model Goblin", "A Model Goblin", QUEST),  
            CreateLocation(455, "Fairy: Fairy Mushrooms", "Fairy Mushrooms", QUEST),    
            CreateLocation(456, "MomoRobo: Out of Service", "Out of Service", QUEST), 
            CreateLocation(457, "Bunny Club: The Bunny Club", "The Bunny Club", QUEST),  
            CreateLocation(458, "Bunny Club: Find the Silky Slime", "Find the Silky Slime", QUEST),    
            CreateLocation(459, "Milk & Cream: Panty Raid", "Panty Raid", QUEST),  
            CreateLocation(460, "Unlucky Cat Statue: Unlucky Cat Statue", "Unlucky Cat Statue", QUEST), 
            CreateLocation(461, "Annahell: Harvest Season", "Harvest Season", QUEST),   
            CreateLocation(462, "Gabrielle: Long Distance Relationship", "Long Distance Relationship", QUEST), 
            CreateLocation(463, "Natasha: Summoning Stones", "Summoning Stones", QUEST),  
            CreateLocation(464, "Keroku: Seamen, With an A", "Seamen, With an A", QUEST),  
            CreateLocation(465, "Ratchel: Cardio Day", "Cardio Day", QUEST),  
            CreateLocation(466, "Ancient Being: Stop Democracy", "Stop Democracy", QUEST), 
            CreateLocation(467, "Clinic: Medical Emergency", "Medical Emergency", QUEST),  
            CreateLocation(468, "Rover: Let the Dog Out", "Let the Dog Out", QUEST),   
            CreateLocation(469, "Cabaret Cafe: Rat Problem", "Rat Problem", QUEST),  
            CreateLocation(470, "Cabaret Cafe: Ghost Hunters", "Ghost Hunters", QUEST),  
            CreateLocation(471, "Cabaret Cafe: Haunted Bathroom", "Haunted Bathroom", QUEST),  
            CreateLocation(472, "Cabaret Cafe: Ectogasm", "Ectogasm", QUEST),  
            CreateLocation(473, "Cabaret Cafe: Jelly Mushroom", "Jelly Mushroom", QUEST),  
            CreateLocation(474, "Cabaret Cafe: Booze Bunny", "Booze Bunny", QUEST),  
            CreateLocation(475, "Cabaret Cafe: Help Wanted", "Help Wanted", QUEST),  
            CreateLocation(476, "Cabaret Cafe: Deluxe Milkshake", "Deluxe Milkshake", QUEST),  
            CreateLocation(477, "Cabaret Cafe: Boned", "Boned", QUEST),  
            CreateLocation(478, "Cabaret Cafe: Legendary Chewtoy", "Legendary Chewtoy", QUEST),  
            CreateLocation(479, "Pig Mansion: Tatil's Tale", "Tatil's Tale", QUEST),  
            CreateLocation(480, "Pig Mansion: Signing Bonus", "Signing Bonus", QUEST),  

            CreateLocation(481, "Leg's Office: Emotional Baggage", "Emotional Baggage", QUEST),  
            CreateLocation(482, "Leg's Office: Dirty Debut", "Dirty Debut", QUEST),  
            CreateLocation(483, "Kyoni's Shop: Devilicious!", "Devilicious", QUEST),  
            CreateLocation(484, "Kyoni's Shop: What's a Daikon?", "What's a Daikon?", QUEST),  
            CreateLocation(485, "Dusty: Alley Cat", "Alley Cat", QUEST),  
            CreateLocation(486, "Cult of Whorus: Priestess of Whorus", "Priestess of Whorus", QUEST),  
            CreateLocation(487, "Cult of Whorus: A Priest's Duties", "A Priest's Duties", QUEST),  
            CreateLocation(488, "Goblin Princess: Goblin Stud", "Goblin Stud", QUEST),
            CreateLocation(489, "WW: Belle's Cowbell Chest", "Cowbell", CHEST),
            CreateLocation(490, "WW: Red Wine Chest", "Wine", CHEST),
            CreateLocation(491, "WW: Gobliana's Belongings", "DummyGB", QUEST, secondaryCallName: "GoblinModelLuggage"),
            CreateLocation(492, "WW: Legs' Business Offer", "Dummy1", QUEST, secondaryCallName: "GoblinBusinessCard"),
            CreateLocation(493, "WW: Gobliana's Headshot", "", QUEST, secondaryCallName: "GoblinHeadshot"),
            CreateLocation(494, "ST: Cabaret Cafe Girls Room Chest", "sc_cabaret_coins", COINCHEST),
            CreateLocation(495, "ST: Cabaret Cafe Delicious Milk", "DummyDM", QUEST, secondaryCallName: "DeliciousMilk"),
            CreateLocation(496, "ST: Cabaret Cafe Cherry Apartment Key", "DummyCK", QUEST, secondaryCallName: "CherryKey"),
            CreateLocation(497, "ST: Cabaret Cafe VIP Chest", "BunnyCostume", CHEST),
            CreateLocation(498, "ST: Leg's Office Goblin Apartment Key", "DummyGAK", QUEST, secondaryCallName: "GoblinModelApartmentKey"),
            CreateLocation(499, "ST: MomoRobo Server Password", "DummyMBAP", QUEST, secondaryCallName: "MomoBotAdminPassword"),
            CreateLocation(500, "ST: Pig Mansion Fungal Key", "DummyFK", QUEST, secondaryCallName: "FungalKey"),
            CreateLocation(501, "ST: Pig Mansion Maid Contract", "MaidContract", CHEST),
            CreateLocation(502, "ST: Special Milkshake", "DummyBM", QUEST, secondaryCallName: "BelleMilkshake"),
            CreateLocation(503, "GC: Willow The Whiff", "CatGirlsClothes", CHEST),
            CreateLocation(504, "JG: Nearby Cat Shrine", "DummySF3", QUEST, secondaryCallName: "SoulFragment3"),
            CreateLocation(505, "JG: Northern Cat Shrine", "DummySF1", QUEST, secondaryCallName: "SoulFragment1"),
            CreateLocation(506, "CD: Cat Shrine", "DummySL2", QUEST, secondaryCallName: "SoulFragment2"),
            CreateLocation(507, "CD: Demon Letter", "DummyDL", QUEST, secondaryCallName: "DemonLetter"),
            CreateLocation(508, "AH: Cloudia", "LegendaryHalo", CHEST),
            CreateLocation(509, "AH: Angel Letter", "DummyAL", QUEST, secondaryCallName: "AngelLetter"),
            CreateLocation(510, "FF: Heavenly Daikon", "HeavenlyDaikon", CHEST),
            CreateLocation(511, "FF: Blue Jelly Mushroom Chest", "GlassWand", CHEST),
            CreateLocation(512, "FF: Fungal Deed", "DummyFD", QUEST, secondaryCallName: "FungalForestDeed"),
            CreateLocation(513, "SC: Silky Slime Chest", "SilkySlime", CHEST),
            CreateLocation(514, "SC: Silky Slime Summoning Stone", "DummySS3", QUEST, secondaryCallName: "SummonStone3"),
            CreateLocation(515, "SC: Secret Room Past Spring Summoning Stone", "DummySS1", QUEST, secondaryCallName: "SummonStone1"),
            CreateLocation(516, "SC: Slurp Summoning Stone", "DummySS2", QUEST, secondaryCallName: "SummonStone2"),
            CreateLocation(517, "WW: Mimic Chest Key Room Chest 2", "MimicKey", CHEST),
            CreateLocation(518, "ST: Abandoned Homes 02 House Chest", "AbandonedApartmentKey", CHEST),
            CreateLocation(519, "ST: Chaos Fight", "HellishDango", CHEST),

            CreateLocation(551, "WW: Sexual Experience Reward - Peach Charge 1", "4", SEX),
            CreateLocation(552, "WW: Sexual Experience Reward - Peach Charge 2", "8", SEX),
            CreateLocation(553, "WW: Sexual Experience Reward - Wand Upgrade 1", "8", SEX),
            CreateLocation(554, "WW: Sexual Experience Reward - Peach Charge 3", "12", SEX),
            CreateLocation(555, "WW: Sexual Experience Reward - Peach Charge 4", "16", SEX),
            CreateLocation(556, "WW: Sexual Experience Reward - Peach Upgrade 1", "16", SEX),
            CreateLocation(557, "WW: Sexual Experience Reward - Peach Charge 5", "20", SEX),
            CreateLocation(558, "WW: Sexual Experience Reward - Peach Charge 6", "24", SEX),
            CreateLocation(559, "WW: Sexual Experience Reward - Wand Upgrade 2", "24", SEX),
            CreateLocation(560, "WW: Sexual Experience Reward - Peach Charge 7", "28", SEX),
            CreateLocation(561, "WW: Sexual Experience Reward - Peach Charge 8", "32", SEX),
            CreateLocation(562, "WW: Sexual Experience Reward - Peach Upgrade 2", "32", SEX),
            CreateLocation(563, "WW: Sexual Experience Reward - Peach Charge 9", "36", SEX),
            CreateLocation(564, "WW: Sexual Experience Reward - Peach Charge 10", "40", SEX),

            CreateLocation(601, "Gacha: Special Promotion #1", "Gachas_Promotion_1", GACHAMACHINE),
            CreateLocation(602, "Gacha: Animal Girls #1", "Gachas_AnimalGirls_1", GACHAMACHINE),
            CreateLocation(603, "Gacha: Animal Girls #2", "Gachas_AnimalGirls_2", GACHAMACHINE),
            CreateLocation(604, "Gacha: Animal Girls #3", "Gachas_AnimalGirls_3", GACHAMACHINE),
            CreateLocation(605, "Gacha: Animal Girls #4", "Gachas_AnimalGirls_4", GACHAMACHINE),
            CreateLocation(606, "Gacha: Animal Girls #5", "Gachas_AnimalGirls_5", GACHAMACHINE),
            CreateLocation(607, "Gacha: Animal Girls #6", "Gachas_AnimalGirls_6", GACHAMACHINE),
            CreateLocation(608, "Gacha: Animal Girls #7", "Gachas_AnimalGirls_7", GACHAMACHINE),
            CreateLocation(609, "Gacha: Animal Girls #8", "Gachas_AnimalGirls_8", GACHAMACHINE),
            CreateLocation(610, "Gacha: Animal Girls #9", "Gachas_AnimalGirls_9", GACHAMACHINE),
            CreateLocation(611, "Gacha: Animal Girls #10", "Gachas_AnimalGirls_10", GACHAMACHINE),
            CreateLocation(612, "Gacha: Bunny Girls #1", "Gachas_Bunny_1", GACHAMACHINE),
            CreateLocation(613, "Gacha: Bunny Girls #2", "Gachas_Bunny_2", GACHAMACHINE),
            CreateLocation(614, "Gacha: Bunny Girls #3", "Gachas_Bunny_3", GACHAMACHINE),
            CreateLocation(615, "Gacha: Bunny Girls #4", "Gachas_Bunny_4", GACHAMACHINE),
            CreateLocation(616, "Gacha: Bunny Girls #5", "Gachas_Bunny_5", GACHAMACHINE),
            CreateLocation(617, "Gacha: Bunny Girls #6", "Gachas_Bunny_6", GACHAMACHINE),
            CreateLocation(618, "Gacha: Bunny Girls #7", "Gachas_Bunny_7", GACHAMACHINE),
            CreateLocation(619, "Gacha: Bunny Girls #8", "Gachas_Bunny_8", GACHAMACHINE),
            CreateLocation(620, "Gacha: Bunny Girls #9", "Gachas_Bunny_9", GACHAMACHINE),
            CreateLocation(621, "Gacha: Bunny Girls #10", "Gachas_Bunny_10", GACHAMACHINE),
            CreateLocation(622, "Gacha: Angels And Demons #1", "Gachas_DemonsAngels_1", GACHAMACHINE),
            CreateLocation(623, "Gacha: Angels And Demons #2", "Gachas_DemonsAngels_2", GACHAMACHINE),
            CreateLocation(624, "Gacha: Angels And Demons #3", "Gachas_DemonsAngels_3", GACHAMACHINE),
            CreateLocation(625, "Gacha: Angels And Demons #4", "Gachas_DemonsAngels_4", GACHAMACHINE),
            CreateLocation(626, "Gacha: Angels And Demons #5", "Gachas_DemonsAngels_5", GACHAMACHINE),
            CreateLocation(627, "Gacha: Angels And Demons #6", "Gachas_DemonsAngels_6", GACHAMACHINE),
            CreateLocation(628, "Gacha: Angels And Demons #7", "Gachas_DemonsAngels_7", GACHAMACHINE),
            CreateLocation(629, "Gacha: Angels And Demons #8", "Gachas_DemonsAngels_8", GACHAMACHINE),
            CreateLocation(630, "Gacha: Angels And Demons #9", "Gachas_DemonsAngels_9", GACHAMACHINE),
            CreateLocation(631, "Gacha: Angels And Demons #10", "Gachas_DemonsAngels_10", GACHAMACHINE),
            CreateLocation(632, "Gacha: Monster Girls #1", "Gachas_Monsters_1", GACHAMACHINE),
            CreateLocation(633, "Gacha: Monster Girls #2", "Gachas_Monsters_2", GACHAMACHINE),
            CreateLocation(634, "Gacha: Monster Girls #3", "Gachas_Monsters_3", GACHAMACHINE),
            CreateLocation(635, "Gacha: Monster Girls #4", "Gachas_Monsters_4", GACHAMACHINE),
            CreateLocation(636, "Gacha: Monster Girls #5", "Gachas_Monsters_5", GACHAMACHINE),
            CreateLocation(637, "Gacha: Monster Girls #6", "Gachas_Monsters_6", GACHAMACHINE),
            CreateLocation(638, "Gacha: Monster Girls #7", "Gachas_Monsters_7", GACHAMACHINE),
            CreateLocation(639, "Gacha: Monster Girls #8", "Gachas_Monsters_8", GACHAMACHINE),
            CreateLocation(640, "Gacha: Monster Girls #9", "Gachas_Monsters_9", GACHAMACHINE),
            CreateLocation(641, "Gacha: Monster Girls #10", "Gachas_Monsters_10", GACHAMACHINE),

            CreateLocation(651, "WW: Genesis' Coin", "gacha_tutorial", GACHA),
            CreateLocation(652, "WW: Secret Cave Coin", "ww_double_jump_gacha", GACHA),
            CreateLocation(653, "WW: Secret Alcove", "ww_gacha_thorns", GACHA),
            CreateLocation(654, "WW: Above Rundown House", "ww_gacha_cowjunction", GACHA),
            CreateLocation(655, "WW: Post Fight Coin", "ww_goblinqueenexit", GACHA),
            CreateLocation(656, "WW: Hidden Spring Room Coin", "ww_gacha_fairy_secret", GACHA),
            CreateLocation(657, "WW: Before Great Fairy", "ww_gacha_goblincamp", GACHA),
            CreateLocation(658, "ST: Coin Above Restrooms", "spiritcity_bell_gacha", GACHA),
            CreateLocation(659, "ST: Behind Alley", "sc_slimeshadysecret_gacha", GACHA),
            CreateLocation(660, "ST: Abandoned Homes 01 House Coin", "sc_interiorhouse11_gacha", GACHA),
            CreateLocation(661, "ST: Abandoned Homes 6 House Coin", "sc_interiorhouse1_gacha", GACHA),
            CreateLocation(662, "SS: Side Room Coin", "scp_pipearches_gacha", GACHA),
            CreateLocation(663, "SS: Ratchel Coin", "scp_ratgirl_gacha", GACHA),
            CreateLocation(664, "GC: Giant Room, Blind Jump Coin", "gc_middleplatform_gacha", GACHA),
            CreateLocation(665, "GC: Up the Ladder", "gc_angel_jump_gacha", GACHA),
            CreateLocation(666, "GC: Hidden Spring Room", "gc_hidden_tower_gacha", GACHA),
            CreateLocation(667, "GC: Across the Boss Room", "gc_ghost_boss_gacha", GACHA),
            CreateLocation(668, "JG: Secret Spring Room Coin", "j_uppersecret_gacha", GACHA),
            CreateLocation(669, "JG: Hidden Hole", "j_gacha_skullsecret", GACHA),
            CreateLocation(670, "JG: Near Elf Merchant", "j_evilroom_gacha", GACHA),
            CreateLocation(671, "JG: Northern Cat Shrine Coin", "j_lavajump_gacha", GACHA),
            CreateLocation(672, "CD: Flip Magic Coin", "gacha_clubdemonpuzzle", GACHA),
            CreateLocation(673, "CD: Club Key Room Coin", "j_club_room_yellow_gacha", GACHA),
            CreateLocation(674, "TG: Early Gacha Coin", "tt_gacha_mossy", GACHA),
            CreateLocation(675, "TG: Hidden Flip Magic Spring", "tt_largeroom_gacha", GACHA),
            CreateLocation(676, "TG: Hidden Ledge", "tt_largebehindleaves_gacha", GACHA),
            CreateLocation(677, "AH: Hidden Foliage 1", "t_treegarden_gacha", GACHA),
            CreateLocation(678, "AH: Hidden Foliage 2", "t_yellowdoor_gacha", GACHA),
            CreateLocation(679, "AH: Flip Magic Room", "t_minicloud_gacha", GACHA),
            CreateLocation(680, "AH: Below Boss Room Coin", "t_flowergarden_gacha", GACHA),
            CreateLocation(681, "FF: Flip Magic Coin", "ff_gachatoken_puzzle", GACHA),
            CreateLocation(682, "FF: Secret Fungus Room", "ff_secretarea_gacha", GACHA),
            CreateLocation(683, "SC: Small Detour", "sc_shroomplatform_gacha", GACHA),
            CreateLocation(684, "SC: Across the Key", "sc_neoneggplant_gacha", GACHA),
            CreateLocation(685, "SC: Secret Room Past Spring Coin", "sc_secretstoneroom_gacha", GACHA),
            CreateLocation(686, "UU: Early Gacha Coin", "u_coralreef_gacha", GACHA),
            CreateLocation(687, "UU: Far Corner", "u_coralroom_gacha", GACHA),
            CreateLocation(688, "UU: Flip Magic Room 2 Coin", "u_deepdrop_gacha", GACHA),
            CreateLocation(689, "UU: Above Save Room", "u_waterpillars_gacha", GACHA),
            CreateLocation(690, "UU: Path Near Frog Boss", "u_treasure_gacha", GACHA),
            CreateLocation(691, "CC: Outside Castle Coin", "cc_entryway_gacha", GACHA),
            CreateLocation(692, "CC: Goblin/Fungal Path Coin", "cc_jigokup_gacha", GACHA),
            CreateLocation(693, "CC: Ghost Castle Path Coin", "cc_fork_gacha", GACHA),
            CreateLocation(694, "CC: Fungal Area Coin", "cc_umimushroom_gacha", GACHA),
            
            
        };

        private static LocationData CreateLocation(int id, string locationName, string primaryCallName, string type, string switchToFlip = "", string secondaryCallName = "")
        {
            var location = new LocationData(id, locationName, primaryCallName, type, secondaryCallName: secondaryCallName, switchToFlip: switchToFlip);
            NameToLocation[location.APLocationName] = location;
            IDToLocation[id] = location;
            if (secondaryCallName != "")
            {
                SecondaryCallLocations[secondaryCallName] = location;
            }
            switch (type)
            {
                case CHEST:
                    {
                        ChestLocations[primaryCallName] = location;
                        break;
                    }
                case COINCHEST:
                    {
                        CoinChestLocations[primaryCallName] = location;
                        break;
                    }
                case CUTSCENE:
                    {
                        CutsceneLocations[primaryCallName] = location;
                        break;
                    }
                case GACHA:
                    {
                        CoinLocations[primaryCallName] = location;
                        break;
                    }
                case GACHAMACHINE:
                    {
                        GachaLocations[primaryCallName] = location;
                        break;
                    }
                case SHOP:
                    {
                        ShopLocations[primaryCallName] = location;
                        break;
                    }
                case QUEST:
                    {
                        if (location.PrimaryCallName.Contains("Dummy"))
                        {
                            break;
                        }
                        QuestLocations[primaryCallName] = location;
                        break;
                    }
                case STAT:
                    {
                        StatLocations[primaryCallName] = location;
                        break;
                    }
                case SEX:
                    {
                        SexExperienceLocations[locationName] = location;
                        break;
                    }
            }
            return location;
        }

        public static readonly List<string> UpdateWhitelist = new(){
            "GoblinModelLuggage", "SoulFragment1", "SoulFragment2", "SoulFragment3", "SummonStone1", "SummonStone2", "SummonStone3"
        };

        public static readonly List<string> CatStatueLocations = new(){
            "CD: Cat Shrine", "JG: Nearby Cat Shrine", "JG: Northern Cat Shrine"
        };

        public static readonly List<string> SummongStoneLocations = new(){
            "SC: Silky Slime Summoning Stone", "SC: Secret Room Past Spring Summoning Stone", "SC: Slurp Summoning Stone"
        };
    }
}