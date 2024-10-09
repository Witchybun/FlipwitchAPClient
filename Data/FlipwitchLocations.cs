using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace FlipwitchAP.Data
{
    public static class FlipwitchLocations
    {
        public const long LOCATION_INIT_ID = 0;
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
            CreateLocation(4, "WW: Genesis' Coin", "gacha_tutorial", GACHA),
            CreateLocation(5, "WW: Rundown House Save Hidden Chest", "ww_tiny_secret_coins", COINCHEST),
            CreateLocation(6, "WW: Red Wine Chest", "Wine", CHEST),
            CreateLocation(7, "WW: Mimic Chest Key Room Chest 1", "ww_coins_savestatue", COINCHEST),
            CreateLocation(8, "WW: Mimic Chest Key Room Chest 2", "MimicKey", CHEST),
            CreateLocation(9, "WW: Secret Cave Coin", "ww_double_jump_gacha", GACHA),
            CreateLocation(10, "WW: Secret Cave Chest", "ww_double_jump_coins", COINCHEST),
            CreateLocation(11, "WW: Mimi's Hidden Chest", "ww_mimic_coins", COINCHEST),
            CreateLocation(12, "WW: Gobliana's Belongings", "DummyGB", QUEST, secondaryCallName: "GoblinModelLuggage"),
            CreateLocation(13, "WW: Secret Alcove", "ww_gacha_thorns", GACHA),
            CreateLocation(14, "WW: Belle's Cowbell Chest", "Cowbell", CHEST),
            CreateLocation(15, "WW: Rundown House Chest", "RundownHouseKey", CHEST),
            CreateLocation(16, "WW: Above Rundown House", "ww_gacha_cowjunction", GACHA),
            CreateLocation(17, "WW: Rescue Great Fairy", "WW_CrystalBreakTriggered", CUTSCENE),
            CreateLocation(18, "WW: Legs' Business Offer", "Dummy1", QUEST, secondaryCallName: "GoblinBusinessCard"),
            CreateLocation(19, "WW: Past Man Cave Chest", "gc_coins_highjump", COINCHEST),
            CreateLocation(20, "WW: Gerry G. Atric's Chest", "GoblinBomb", CHEST),
            CreateLocation(21, "WW: Hidden Alcove", "ww_secret_nook_coins", COINCHEST),
            CreateLocation(22, "WW: Beside Flip Platform", "ww_ladderroom_coins", COINCHEST),
            CreateLocation(23, "WW: After Chaos Fight Chest", "ww_horseshoe_coins", COINCHEST),
            CreateLocation(24, "WW: Boss Key Room Chest", "GoblinCrystal", CHEST),
            CreateLocation(25, "WW: Boss Key Room Big Chest", "GoblinBossKey", CHEST),
            CreateLocation(26, "WW: Boss Key Room HP Upgrade", "105.96-117.866442_CaveHeartUndergroundHealth Upgrade (1)", STAT),
            CreateLocation(27, "WW: Goblin Queen MP Upgrade", "248.54-185.386447_GoblinTreasureUndergroundMana Upgrade", STAT),
            CreateLocation(28, "WW: Goblin Queen Chaos Piece", "ChaosKey1", CHEST),
            CreateLocation(29, "WW: Goblin Queen", "GoblinBossDefeated", CUTSCENE),
            CreateLocation(30, "WW: Post Fight Coin", "ww_goblinqueenexit", GACHA),
            CreateLocation(31, "WW: Flying Fairy Chest", "MagneticHairpin", CHEST),
            CreateLocation(32, "WW: Hidden Spring Room Coin", "ww_gacha_fairy_secret", GACHA),
            CreateLocation(33, "WW: Sexual Experience Reward 1", "4", SEX),
            CreateLocation(34, "WW: Sexual Experience Reward 2", "8", SEX),
            CreateLocation(35, "WW: Sexual Experience Reward 3", "8", SEX),
            CreateLocation(36, "WW: Sexual Experience Reward 4", "12", SEX),
            CreateLocation(37, "WW: Sexual Experience Reward 5", "16", SEX),
            CreateLocation(38, "WW: Sexual Experience Reward 6", "16", SEX),
            CreateLocation(39, "WW: Sexual Experience Reward 7", "20", SEX),
            CreateLocation(40, "WW: Sexual Experience Reward 8", "24", SEX),
            CreateLocation(41, "WW: Sexual Experience Reward 9", "24", SEX),
            CreateLocation(42, "WW: Sexual Experience Reward 10", "28", SEX),
            CreateLocation(43, "WW: Sexual Experience Reward 11", "32", SEX),
            CreateLocation(44, "WW: Sexual Experience Reward 12", "32", SEX),
            CreateLocation(45, "WW: Sexual Experience Reward 13", "36", SEX),
            CreateLocation(46, "WW: Sexual Experience Reward 14", "40", SEX),
            CreateLocation(47, "WW: Sexual Experience Reward 15", "40", SEX),
            CreateLocation(48, "WW: Great Fairy's Reward", "Dummy2", QUEST, secondaryCallName: "FairyCostume"),
            CreateLocation(49, "WW: Before Great Fairy", "ww_gacha_goblincamp", GACHA),
            CreateLocation(50, "WW: Gobliana's Headshot", "", QUEST, secondaryCallName: "GoblinHeadshot"),

            CreateLocation(61, "ST: Outside Shop Item", "FairyBubble", SHOP),
            CreateLocation(62, "ST: Women's Bathroom HP Upgrade", "18.5853457.6282Bathroom_FemaleWorldHealth Upgrade", STAT),
            CreateLocation(63, "ST: Men's Bathroom MP Upgrade", "26.9015833.63543Bathroom_MaleWorldMana Upgrade", STAT),
            CreateLocation(64, "ST: Coin Above Restrooms", "spiritcity_bell_gacha", GACHA),
            CreateLocation(65, "ST: Cabaret Cafe Girls Room Chest", "sc_cabaret_coins", COINCHEST),
            CreateLocation(66, "ST: Cabaret Cafe Delicious Milk", "DummyDM", QUEST, secondaryCallName: "DeliciousMilk"),
            CreateLocation(67, "ST: Cabaret Cafe Cherry Apartment Key", "DummyCK", QUEST, secondaryCallName: "CherryKey"),
            CreateLocation(68, "ST: Cabaret Cafe VIP Chest", "BunnyCostume", CHEST),
            CreateLocation(69, "ST: Shop Item 1", "FortuneCat", SHOP),
            CreateLocation(70, "ST: Shop Item 2", "HeartNecklace", SHOP),
            CreateLocation(71, "ST: Shop Item 3", "StarBracelet", SHOP),
            CreateLocation(72, "ST: Shop Item 4", "FrillyPanties", SHOP),
            CreateLocation(73, "ST: Shop Roof Chest", "spiritcity_itemshop_coins", COINCHEST),
            CreateLocation(74, "ST: Fashion Shop 1", "CatCostume", SHOP),
            CreateLocation(75, "ST: Fashion Shop 2", "GoblinCostume", SHOP),
            CreateLocation(76, "ST: Ancient Being Chest", "sc_democracy_coins", COINCHEST),
            CreateLocation(77, "ST: Ghostly Castle Key Chest", "GhostCastleKey", CHEST),
            CreateLocation(78, "ST: Below Cemetery", "sc_waterfall_coins", COINCHEST),
            CreateLocation(79, "ST: Chaos Fight", "HellishDango", CHEST),
            CreateLocation(80, "ST: Leg's Office Goblin Apartment Key", "DummyGAK", QUEST, secondaryCallName: "GoblinModelApartmentKey"),
            CreateLocation(81, "ST: MomoRobo Server Password", "DummyMBAP", QUEST, secondaryCallName: "MomoBotAdminPassword"),
            CreateLocation(82, "ST: Banana Apartment", "sc_coins_jackpot", COINCHEST),
            CreateLocation(83, "ST: Behind Alley", "sc_slimeshadysecret_gacha", GACHA),
            CreateLocation(84, "ST: Abandoned Homes 02 House Chest", "AbandonedApartmentKey", CHEST),
            CreateLocation(85, "ST: Abandoned Homes 01 House Coin", "sc_interiorhouse11_gacha", GACHA),
            CreateLocation(86, "ST: Abandoned Homes 6 House Coin", "sc_interiorhouse1_gacha", GACHA),
            CreateLocation(87, "ST: Abandoned Homes Green House Chest", "sc_interiorslime_coins", COINCHEST),
            CreateLocation(88, "ST: Lone House Chest", "sc_angeljumpinterior_coins", COINCHEST),
            CreateLocation(89, "ST: Pig Mansion Fungal Key", "DummyFK", QUEST, secondaryCallName: "FungalKey"),
            CreateLocation(90, "ST: Pig Mansion Maid Contract", "MaidContract", CHEST),

            CreateLocation(101, "SS: Hidden Pipe Chest", "scp_hiddencoins", COINCHEST),
            CreateLocation(102, "SS: Side Room Coin", "scp_pipearches_gacha", GACHA),
            CreateLocation(103, "SS: Side Room Chest", "scp_pipearches_coins", COINCHEST),
            CreateLocation(104, "SS: Ratchel Coin", "scp_ratgirl_gacha", GACHA),
            CreateLocation(105, "SS: Rat Costume Chest 1", "scp_lastpipe_coins3", COINCHEST),
            CreateLocation(106, "SS: Rat Costume Chest 2", "RatCostume", CHEST),
            CreateLocation(107, "SS: Rat Costume Chest 3", "scp_lastpipe_coins2", COINCHEST),
            CreateLocation(108, "SS: Shady Gauntlet HP Upgrade", "209.78-159.5814_TallPipePipeworldHealth Upgrade", STAT),
            CreateLocation(109, "SS: Shady Gauntlet Chest", "scp_tallpipe_coin", COINCHEST),
            CreateLocation(110, "SS: Elf Merchant Item 1", "CursedTalisman", SHOP),
            CreateLocation(111, "SS: Elf Merchant Item 2", "PortablePortal", SHOP),
            CreateLocation(112, "SS: Elf Merchant Chest", "sc_smallpipecorner_coins", COINCHEST),
            CreateLocation(113, "SS: Dr. Witch Doctor Big Chest", "MermaidScale", CHEST),
            CreateLocation(114, "SS: Dr. Witch Doctor MP Upgrade", "313.94-208.99511_PipeChestPipeworldMana Upgrade", STAT),
            CreateLocation(115, "SS: Dr. Witch Doctor Tutorial Chest", "sc_coins_mermaid_tutorial", COINCHEST),

            CreateLocation(151, "GC: Below Entrance Chest", "gc_crypt_coins", COINCHEST),
            CreateLocation(152, "GC: Slime Form Room Chest 1", "gc_slime_secret_coins_1", COINCHEST), // All three chests have the same switchName?
            CreateLocation(153, "GC: Slime Form Room Chest 2", "gc_slime_secret_coins_2", COINCHEST), // All three chests have the same switchName?
            CreateLocation(154, "GC: Slime Form Room Chest 3", "gc_slime_secret_coins_3", COINCHEST), // All three chests have the same switchName?
            CreateLocation(155, "GC: Giant Room, Flip Magic Platform", "RoseGardenKey", CHEST),
            CreateLocation(156, "GC: Giant Room, Blind Jump Coin", "gc_middleplatform_gacha", GACHA),
            CreateLocation(157, "GC: Up the Ladder", "gc_angel_jump_gacha", GACHA),
            CreateLocation(158, "GC: Hidden Ledge", "gc_entryway_coins", COINCHEST),
            CreateLocation(159, "GC: Elf Merchant Item", "DisarmingBell", SHOP),
            CreateLocation(160, "GC: Elf Merchant Chest", "gc_largegardens_coins", COINCHEST),
            CreateLocation(161, "GC: Secret Garden Key Chest", "SecretGardenKey", CHEST),
            CreateLocation(162, "GC: Hidden Wall", "gc_windowroom_coins", CHEST),
            CreateLocation(163, "GC: Along the Path", "gc_mid_point_coins", COINCHEST),
            CreateLocation(164, "GC: Hidden Spring Room", "gc_hidden_tower_gacha", GACHA),
            CreateLocation(165, "GC: Behind the Vines", "gc_ghost_hallway_coins", COINCHEST),
            CreateLocation(166, "GC: Across the Boss Room", "gc_ghost_boss_gacha", GACHA),
            CreateLocation(167, "GC: Hidden Shrub", "gc_dropdown_coins", CHEST),
            CreateLocation(168, "GC: Ghost Form Chest", "GhostTransform", CHEST),
            CreateLocation(169, "GC: Thimble Costume 1", "NunCostume", SHOP),
            CreateLocation(170, "GC: Thimble Costume 2", "PriestCostume", SHOP),
            CreateLocation(171, "GC: Thimble Chest", "gc_savestatue_coins", COINCHEST),
            CreateLocation(172, "GC: Willow The Whiff", "CatGirlsClothes", CHEST),
            CreateLocation(173, "GC: Ghost Boss Chaos Key Piece", "ChaosKey2", CHEST),
            CreateLocation(174, "GC: Ghost Boss MP Upgrade", "-45.7701833.9395113_GhostTreasureWorldMana Upgrade", STAT),
            CreateLocation(175, "GC: Ghostly Gauntlet", "-86.04183.8935_BellTowerWorldHealth Upgrade", STAT),

            CreateLocation(201, "JG: Hidden Flip Magic Room", "j_entrance_coins", COINCHEST),
            CreateLocation(202, "JG: Slime Form Room", "j_superslimeysecret_coins", COINCHEST),
            CreateLocation(203, "JG: Early Ledge", "j_entranceway_coins", COINCHEST),
            CreateLocation(204, "JG: Secret Spring Room Chest", "j_piercing_earrings", COINCHEST),
            CreateLocation(205, "JG: Secret Spring Room Coin", "j_uppersecret_gacha", GACHA),
            CreateLocation(206, "JG: Nearby Cat Shrine", "DummySF3", QUEST, secondaryCallName: "SoulFragment3"),
            CreateLocation(207, "JG: Beast Key Chest", "DemonSubBossKey", CHEST),
            CreateLocation(208, "JG: Hidden Ledge", "j_lavariver_coins", COINCHEST),
            CreateLocation(209, "JG: Annahell Chest", "j_minishrine_coins", COINCHEST),
            CreateLocation(210, "JG: Hidden Hole", "j_gacha_skullsecret", GACHA),
            CreateLocation(211, "JG: Near Elf Merchant", "j_evilroom_gacha", GACHA),
            CreateLocation(212, "JG: Elf Merchant Item 1", "DemonicCuff", SHOP),
            CreateLocation(213, "JG: Elf Merchant Item 2", "FirstFrogTalisman", SHOP),
            CreateLocation(214, "JG: Hot Guy Chest", "CollapsedTempleKey", CHEST),
            CreateLocation(215, "JG: Far Ledge", "j_themound_coins", COINCHEST),
            CreateLocation(216, "JG: Hidden Flip Magic Ledge Chest", "j_lava_jump_coins", COINCHEST),
            CreateLocation(217, "JG: Demon Wings Chest", "DemonDash", CHEST),
            CreateLocation(218, "JG: Demon Wings Tutorial", "j_demondashtutorial_coins", COINCHEST),
            CreateLocation(219, "JG: Northern Cat Shrine Chest", "j_secret_blue_coins", COINCHEST),
            CreateLocation(220, "JG: Northern Cat Shrine Coin", "j_lavajump_gacha", GACHA),
            CreateLocation(221, "JG: Northern Cat Shrine", "DummySF1", QUEST, secondaryCallName: "SoulFragment1"),

            CreateLocation(251, "CD: Club Demon Entrance", "j_clubdemonentrance_coins", COINCHEST),
            CreateLocation(252, "CD: Under The Table", "j_clubentrance_coins", COINCHEST),
            CreateLocation(253, "CD: Club Door Room", "ClubDoor2Key", CHEST),
            CreateLocation(254, "CD: Flip Magic Chest", "j_purplestairs_coins", COINCHEST),
            CreateLocation(255, "CD: Flip Magic Coin", "gacha_clubdemonpuzzle", GACHA),
            CreateLocation(256, "CD: Thimble Costume 1", "MikoCostume", SHOP),
            CreateLocation(257, "CD: Thimble Costume 2", "FarmerCostume", SHOP),
            CreateLocation(258, "CD: Demonic Gauntlet", "-146.95-195.9256_BlueRewardClubDemonHealth Upgrade", STAT),
            CreateLocation(259, "CD: Club Key Room Chest", "ClubDoor1Key", CHEST),
            CreateLocation(260, "CD: Club Key Room Coin", "j_club_room_yellow_gacha", GACHA),
            CreateLocation(261, "CD: Cat Shrine", "DummySL2", QUEST, secondaryCallName: "SoulFragment2"),
            CreateLocation(262, "CD: Demon Boss Key Chest", "DemonBossKey", CHEST),
            CreateLocation(263, "CD: Demon Boss Chaos Key Piece", "ChaosKey3", CHEST),
            CreateLocation(264, "CD: Demon Boss MP Upgrade", "36.98-152.5650_DemonBossRightClubDemonMana Upgrade", STAT),
            CreateLocation(265, "CD: Demon Letter", "DummyDL", QUEST, secondaryCallName: "DemonLetter"),

            CreateLocation(301, "TG: Early Gacha Coin", "tt_gacha_mossy", GACHA),
            CreateLocation(302, "TG: Early Coins Chest", "tt_ladderstairs_coins", COINCHEST),
            CreateLocation(303, "TG: Hidden Foliage", "tt_gender_elevator_coins", COINCHEST),
            CreateLocation(304, "TG: Hidden Flip Magic Spring", "tt_largeroom_gacha", GACHA),
            CreateLocation(305, "TG: Birby", "HarpyFeather", CHEST),
            CreateLocation(306, "TG: Flip Magic Chest", "tt_largetowerroom_coins", COINCHEST),
            CreateLocation(307, "TG: Hidden Ledge", "tt_largebehindleaves_gacha", GACHA),
            CreateLocation(308, "TG: Secret Alcove", "tt_secretzone_coins", COINCHEST),

            CreateLocation(351, "AH: Starting Coins", "t_tengokuentrance_coins", COINCHEST),
            CreateLocation(352, "AH: Hidden Foliage 1", "t_treegarden_gacha", GACHA),
            CreateLocation(353, "AH: Hidden Foliage 2", "t_yellowdoor_gacha", GACHA),
            CreateLocation(354, "AH: Elf Merchant Item 1", "MindMushroom", SHOP),
            CreateLocation(355, "AH: Elf Merchant Item 2", "RingofMoon", SHOP),
            CreateLocation(356, "AH: Elf Merchant Chest", "t_behindcloud_coins", CHEST),
            CreateLocation(357, "AH: Cloudia", "LegendaryHalo", CHEST),
            CreateLocation(358, "AH: Angelic Gauntlet", "-359.9889222.55928_TopStoneTengokuHealth Upgrade", STAT),
            CreateLocation(359, "AH: Thimble Costume 1", "PostmanCostume", SHOP),
            CreateLocation(360, "AH: Thimble Costume 2", "NurseCostume", SHOP),
            CreateLocation(361, "AH: Thimble Chest", "FlutterknifeGarter", CHEST),
            CreateLocation(362, "AH: Below Thimble", "t_moregenders_coins", COINCHEST),
            CreateLocation(363, "AH: Gabrielle Chest", "t_switchystatue_coins", COINCHEST),
            CreateLocation(364, "AH: Behind The Vines", "t_mazeup_coins", COINCHEST),
            CreateLocation(365, "AH: Flip Magic Room", "t_minicloud_gacha", GACHA),
            CreateLocation(366, "AH: Angel Feathers Chest", "AngelJump", CHEST),
            CreateLocation(367, "AH: Below Boss Room Chest", "t_flowergarden_coins", COINCHEST),
            CreateLocation(368, "AH: Below Boss Room Coin", "t_flowergarden_gacha", GACHA),
            CreateLocation(369, "AH: Angelica Chaos Key Piece", "ChaosKey4", CHEST),
            CreateLocation(370, "AH: Angelica MP Upgrade", "42.41261.807710_ConnectorTengokuMana Upgrade", STAT),
            CreateLocation(371, "AH: Angel Letter", "DummyAL", QUEST, secondaryCallName: "AngelLetter"),

            CreateLocation(401, "FF: Thimble Costume 1", "MaidCostume", SHOP),
            CreateLocation(402, "FF: Thimble Costume 2", "PigCostume", SHOP),
            CreateLocation(403, "FF: Lower Pit", "ff_dropright_coins", COINCHEST),
            CreateLocation(404, "FF: Flip Magic Coin", "ff_gachatoken_puzzle", GACHA),
            CreateLocation(405, "FF: Fungal Deed", "DummyFD", QUEST, secondaryCallName: "FungalForestDeed"),
            CreateLocation(406, "FF: Past Fungella", "238.5322-40.318542_TinyRoomWorldMana Upgrade (1)", STAT),
            CreateLocation(407, "FF: Heavenly Daikon", "HeavenlyDaikon", CHEST),
            CreateLocation(408, "FF: Past Chaos Fight", "ff_movingplatforms_coins", COINCHEST),
            CreateLocation(409, "FF: Closed Off Coins", "ff_plummet_coins", COINCHEST),
            CreateLocation(410, "FF: Between The Thorns", "ff_circleback_coins", COINCHEST),
            CreateLocation(411, "FF: Elf Merchant Item 1", "MagicalMushroom", SHOP),
            CreateLocation(412, "FF: Elf Merchant Item 2", "RingofSun", SHOP),
            CreateLocation(413, "FF: Elf Merchant Chest", "sc_woodplatform_coins", COINCHEST),
            CreateLocation(414, "FF: Guarded by Mushrooms", "ff_mushroomtower_coins", COINCHEST),
            CreateLocation(415, "FF: Fungal Gauntlet", "306.7147-38.2432619_MushroomClimbWorldHealth Upgrade (1)", STAT),
            CreateLocation(416, "FF: Blue Jelly Mushroom Chest", "GlassWand", CHEST),
            CreateLocation(417, "FF: Fungal Door Key Chest", "ForgottenFungalDoorKey", CHEST),
            CreateLocation(418, "FF: Secret Fungus Room", "ff_secretarea_gacha", GACHA),
            CreateLocation(419, "FF: Slime Form Chest", "SlimeTransform", CHEST),
            CreateLocation(420, "FF: Slime Citadel Key Chest", "SlimeCitadelKey", CHEST),
            CreateLocation(421, "FF: Slime Tutorial Chest", "sc_slimetutorial_coins", COINCHEST),

            CreateLocation(451, "SC: Citadel Entrance", "sc_slimeentrance_coins", COINCHEST),
            CreateLocation(452, "SC: Secret Room", "sc_supersecretcoins_100", COINCHEST),
            CreateLocation(453, "SC: Lone Room", "sc_supersmallslimesecret_coins", COINCHEST),
            CreateLocation(454, "SC: Silky Slime Chest", "SilkySlime", CHEST),
            CreateLocation(455, "SC: Silky Slime Summoning Stone", "DummySS3", QUEST, secondaryCallName: "SummonStone3"),
            CreateLocation(456, "SC: Small Detour", "sc_shroomplatform_gacha", GACHA),
            CreateLocation(457, "SC: Slimy Sub Boss Key Chest", "SlimySubBossKey", CHEST),
            CreateLocation(458, "SC: Across the Key", "sc_neoneggplant_gacha", GACHA),
            CreateLocation(459, "SC: Thimble Costume 1", "PoliceCostume", SHOP),
            CreateLocation(460, "SC: Thimble Costume 2", "AlchemistCostume", SHOP),
            CreateLocation(461, "SC: Slimy Gauntlet", "577.8729-152.997124_SlimyPrizeSlimeCitadelHealth Upgrade", STAT),
            CreateLocation(462, "SC: Near Stone Chest", "sc_philosopher_coins", COINCHEST),
            CreateLocation(463, "SC: Secret Room Past Spring Coin", "sc_secretstoneroom_gacha", GACHA),
            CreateLocation(464, "SC: Secret Room Past Spring Summoning Stone", "DummySS1", QUEST, secondaryCallName: "SummonStone1"),
            CreateLocation(465, "SC: Hidden Tunnel", "sc_longsecret_coins", COINCHEST),
            CreateLocation(466, "SC: Elf Merchant Item 1", "SlimySentry", SHOP),
            CreateLocation(467, "SC: Elf Merchant Item 2", "SecondFrogTalisman", SHOP),
            CreateLocation(468, "SC: Slurp Slime Boss Key Chest", "SlimeBossKey", CHEST),
            CreateLocation(469, "SC: Slurp Summoning Stone", "DummySS2", QUEST, secondaryCallName: "SummonStone2"),
            CreateLocation(470, "SC: Slimy Princess Chaos Key Piece", "ChaosKey6", CHEST),
            CreateLocation(471, "SC: Slimy Princess MP Upgrade", "303.4169-222.008931_EndRoomSlimeCitadelMana Upgrade", STAT),

            CreateLocation(501, "UU: Early Gacha Coin", "u_coralreef_gacha", GACHA),
            CreateLocation(502, "UU: Save Coin Chest", "uu_coins_stair_jump", COINCHEST),
            CreateLocation(503, "UU: Angler Costume Chest", "AnglerCostume", CHEST),
            CreateLocation(504, "UU: Flip Magic Room Chest", "u_waterhall_coins", COINCHEST),
            CreateLocation(505, "UU: Far Corner", "u_coralroom_gacha", GACHA),
            CreateLocation(506, "UU: Sacrificial Dagger Chest", "SacrificialDagger", CHEST),
            CreateLocation(507, "UU: Flip Magic Room 2 Coin", "u_deepdrop_gacha", GACHA),
            CreateLocation(508, "UU: Mermaid's Chest", "uu_diving_coins", COINCHEST),
            CreateLocation(509, "UU: Chaos Fight Chest", "uu_surfacing_coins", COINCHEST),
            CreateLocation(510, "UU: Above Save Room", "u_waterpillars_gacha", GACHA),
            CreateLocation(511, "UU: Path to Octrina", "uu_coralzone_coins", COINCHEST),
            CreateLocation(512, "UU: Octrina Chest", "FrogBossKey", CHEST),
            CreateLocation(513, "UU: Watery Gauntlet", "-212.3212-150.174823_DeadEndWorldHealth Upgrade", STAT),
            CreateLocation(514, "UU: Path Near Frog Boss", "u_treasure_gacha", GACHA),
            CreateLocation(515, "UU: Frog Boss Chaos Key Piece", "ChaosKey5", CHEST),
            CreateLocation(516, "UU: Frog Boss MP Upgrade", "82.6517-150.64234_FrogTreasureWorldMana Upgrade", STAT),

            CreateLocation(551, "CC: Outside Castle Coin", "cc_entryway_gacha", GACHA),
            CreateLocation(552, "CC: Early Coins Chest", "cc_entryway_coins", COINCHEST),
            CreateLocation(553, "CC: Goblin/Fungal Path Coin", "cc_jigokup_gacha", GACHA),
            CreateLocation(554, "CC: Ghost Castle Path Coin", "cc_fork_gacha", GACHA),
            CreateLocation(555, "CC: Jigoku Path Chest", "cc_jigokupyramidcoins", COINCHEST),
            CreateLocation(556, "CC: Near Chaos Sanctum", "cc_secretgem_coins", COINCHEST),
            CreateLocation(557, "CC: Elf Merchant Item 1", "HauntedScythe", SHOP),
            CreateLocation(558, "CC: Elf Merchant Chest", "cc_bluetriangle_coins", COINCHEST),
            CreateLocation(559, "CC: Slime Citadel Path Chest", "cc_slimepipes_coins", COINCHEST),
            CreateLocation(560, "CC: Fungal Area Coin", "cc_umimushroom_gacha", GACHA),
            CreateLocation(561, "CC: Big Jump HP Upgrade", "217.944.8911_CC_UmiChaosPyramidHealth Upgrade", STAT),
            CreateLocation(562, "CC: Big Jump Chest", "cc_umi_coins", COINCHEST),
            CreateLocation(563, "CC: Pandora Chaos Sanctum Key", "ChaosSanctumKey", CHEST),
            CreateLocation(564, "CC: Pandora MP Upgrade", "450.011.37530_CC_SubBossChaosPyramidMana Upgrade", STAT),

            CreateLocation(601, "Quest: Magic Mentor", "Magical Mentor", QUEST, secondaryCallName: "BewitchedBubble"),
            CreateLocation(602, "Quest: Need My Cowbell", "Need My Cowbell", QUEST),    
            CreateLocation(603, "Mimi: Giant Chest Key", "Giant Chest Key", QUEST),  
            CreateLocation(604, "Gobliana: A Model Goblin", "A Model Goblin", QUEST),  
            CreateLocation(605, "Fairy: Fairy Mushrooms", "Fairy Mushrooms", QUEST),    
            CreateLocation(606, "MomoRobo: Out of Service", "Out of Service", QUEST), 
            CreateLocation(607, "Bunny Club: The Bunny Club", "The Bunny Club", QUEST),  
            CreateLocation(608, "Bunny Club: Find the Silky Slime", "Find the Silky Slime", QUEST),    
            CreateLocation(609, "Milk & Cream: Panty Raid", "Panty Raid", QUEST),  
            CreateLocation(610, "Unlucky Cat Statue: Unlucky Cat", "Unlucky Cat", QUEST), 
            CreateLocation(611, "Annahell: Harvest Season", "Harvest Season", QUEST),   
            CreateLocation(612, "Gabrielle: Long Distance Relationship", "Long Distance Relationship", QUEST), 
            CreateLocation(613, "Natasha: Summoning Stones", "Summoning Stones", QUEST),  
            CreateLocation(614, "Keroku: Seamen, With an A", "Seamen, With an A", QUEST),  
            CreateLocation(615, "Ratchel: Cardio Day", "Cardio Day", QUEST),  
            CreateLocation(616, "Ancient Being: Stop Democracy", "Stop Democracy", QUEST), 
            CreateLocation(617, "Clinic: Medical Emergency", "Medical Emergency", QUEST),  
            CreateLocation(618, "Rover: Let the Dog Out", "Let the Dog Out", QUEST),   
            CreateLocation(619, "Cabaret Cafe: Rat Problem", "Rat Problem", QUEST),  
            CreateLocation(620, "Cabaret Cafe: Ghost Hunters", "Ghost Hunters", QUEST),  
            CreateLocation(621, "Cabaret Cafe: Haunted Bathroom", "Haunted Bathroom", QUEST),  
            CreateLocation(622, "Cabaret Cafe: Ectogasm", "Ectogasm", QUEST),  
            CreateLocation(623, "Cabaret Cafe: Jelly Mushroom", "Jelly Mushroom", QUEST),  
            CreateLocation(624, "Cabaret Cafe: Booze Bunny", "Booze Bunny", QUEST),  
            CreateLocation(625, "Cabaret Cafe: Help Wanted", "Help Wanted", QUEST),  
            CreateLocation(626, "Cabaret Cafe: Deluxe Milkshake", "Deluxe Milkshake", QUEST),  
            CreateLocation(627, "Cabaret Cafe: Boned", "Boned", QUEST),  
            CreateLocation(628, "Cabaret Cafe: Legendary Chewtoy", "Legendary Chewtoy", QUEST),  
            CreateLocation(629, "Pig Mansion: Tatil's Tale", "Tatil's Tale", QUEST),  
            CreateLocation(630, "Pig Mansion: Signing Bonus", "Signing Bonus", QUEST),  
            CreateLocation(631, "Leg's Office: Emotional Baggage", "Emotional Baggage", QUEST),  
            CreateLocation(632, "Leg's Office: Dirty Debut", "Dirty Debut", QUEST),  
            CreateLocation(633, "Kyoni's Shop: Belle's Milkshake", "Belle's Milkshake", QUEST),  
            CreateLocation(634, "Kyoni's Shop: Devilicious", "Devilicious", QUEST),  
            CreateLocation(635, "Kyoni's Shop: What's a Daikon?", "What's a Daikon?", QUEST),  
            CreateLocation(636, "Dusty: Alley Cat", "Alley Cat", QUEST),  
            CreateLocation(637, "Cult of Whorus: Priestess of Whorus", "Priestess of Whorus", QUEST),  
            CreateLocation(638, "Cult of Whorus: A Priest's Duties", "A Priest's Duties", QUEST),  
            CreateLocation(639, "Goblin Princess: Goblin Stud", "Goblin Stud", QUEST),

            CreateLocation(651, "Gacha: Special Promotion #1", "Gachas_Promotion_1", GACHAMACHINE),
            CreateLocation(652, "Gacha: Animal Girls #1", "Gachas_AnimalGirls_1", GACHAMACHINE),
            CreateLocation(653, "Gacha: Animal Girls #2", "Gachas_AnimalGirls_2", GACHAMACHINE),
            CreateLocation(654, "Gacha: Animal Girls #3", "Gachas_AnimalGirls_3", GACHAMACHINE),
            CreateLocation(655, "Gacha: Animal Girls #4", "Gachas_AnimalGirls_4", GACHAMACHINE),
            CreateLocation(656, "Gacha: Animal Girls #5", "Gachas_AnimalGirls_5", GACHAMACHINE),
            CreateLocation(657, "Gacha: Animal Girls #6", "Gachas_AnimalGirls_6", GACHAMACHINE),
            CreateLocation(658, "Gacha: Animal Girls #7", "Gachas_AnimalGirls_7", GACHAMACHINE),
            CreateLocation(659, "Gacha: Animal Girls #8", "Gachas_AnimalGirls_8", GACHAMACHINE),
            CreateLocation(660, "Gacha: Animal Girls #9", "Gachas_AnimalGirls_9", GACHAMACHINE),
            CreateLocation(661, "Gacha: Animal Girls #10", "Gachas_AnimalGirls_10", GACHAMACHINE),
            CreateLocation(662, "Gacha: Bunny Girls #1", "Gachas_Bunny_1", GACHAMACHINE),
            CreateLocation(663, "Gacha: Bunny Girls #2", "Gachas_Bunny_2", GACHAMACHINE),
            CreateLocation(664, "Gacha: Bunny Girls #3", "Gachas_Bunny_3", GACHAMACHINE),
            CreateLocation(665, "Gacha: Bunny Girls #4", "Gachas_Bunny_4", GACHAMACHINE),
            CreateLocation(666, "Gacha: Bunny Girls #5", "Gachas_Bunny_5", GACHAMACHINE),
            CreateLocation(667, "Gacha: Bunny Girls #6", "Gachas_Bunny_6", GACHAMACHINE),
            CreateLocation(668, "Gacha: Bunny Girls #7", "Gachas_Bunny_7", GACHAMACHINE),
            CreateLocation(669, "Gacha: Bunny Girls #8", "Gachas_Bunny_8", GACHAMACHINE),
            CreateLocation(670, "Gacha: Bunny Girls #9", "Gachas_Bunny_9", GACHAMACHINE),
            CreateLocation(671, "Gacha: Bunny Girls #10", "Gachas_Bunny_10", GACHAMACHINE),
            CreateLocation(672, "Gacha: Angels And Demons #1", "Gachas_DemonsAngels_1", GACHAMACHINE),
            CreateLocation(673, "Gacha: Angels And Demons #2", "Gachas_DemonsAngels_2", GACHAMACHINE),
            CreateLocation(674, "Gacha: Angels And Demons #3", "Gachas_DemonsAngels_3", GACHAMACHINE),
            CreateLocation(675, "Gacha: Angels And Demons #4", "Gachas_DemonsAngels_4", GACHAMACHINE),
            CreateLocation(676, "Gacha: Angels And Demons #5", "Gachas_DemonsAngels_5", GACHAMACHINE),
            CreateLocation(677, "Gacha: Angels And Demons #6", "Gachas_DemonsAngels_6", GACHAMACHINE),
            CreateLocation(678, "Gacha: Angels And Demons #7", "Gachas_DemonsAngels_7", GACHAMACHINE),
            CreateLocation(679, "Gacha: Angels And Demons #8", "Gachas_DemonsAngels_8", GACHAMACHINE),
            CreateLocation(680, "Gacha: Angels And Demons #9", "Gachas_DemonsAngels_9", GACHAMACHINE),
            CreateLocation(681, "Gacha: Angels And Demons #10", "Gachas_DemonsAngels_10", GACHAMACHINE),
            CreateLocation(682, "Gacha: Monster Girls #1", "Gachas_Monsters_1", GACHAMACHINE),
            CreateLocation(683, "Gacha: Monster Girls #2", "Gachas_Monsters_2", GACHAMACHINE),
            CreateLocation(684, "Gacha: Monster Girls #3", "Gachas_Monsters_3", GACHAMACHINE),
            CreateLocation(685, "Gacha: Monster Girls #4", "Gachas_Monsters_4", GACHAMACHINE),
            CreateLocation(686, "Gacha: Monster Girls #5", "Gachas_Monsters_5", GACHAMACHINE),
            CreateLocation(687, "Gacha: Monster Girls #6", "Gachas_Monsters_6", GACHAMACHINE),
            CreateLocation(688, "Gacha: Monster Girls #7", "Gachas_Monsters_7", GACHAMACHINE),
            CreateLocation(689, "Gacha: Monster Girls #8", "Gachas_Monsters_8", GACHAMACHINE),
            CreateLocation(690, "Gacha: Monster Girls #9", "Gachas_Monsters_9", GACHAMACHINE),
            CreateLocation(691, "Gacha: Monster Girls #10", "Gachas_Monsters_10", GACHAMACHINE),

            
            
            
        };

        private static LocationData CreateLocation(int id, string locationName, string primaryCallName, string type, string switchToFlip = "", string secondaryCallName = "")
        {
            var location = new LocationData(id, locationName, primaryCallName, type, secondaryCallName: secondaryCallName, switchToFlip: switchToFlip);
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
    }
}