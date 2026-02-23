using System.Collections.Generic;

namespace FlipwitchAP.Data
{
    public class GenericInformation
    {
        public static Dictionary<string, string> SceneToAreaName = new()
        {
            {"WitchyWoods_Final", "Witchy Woods"},
            {"Spirit_City_Final", "Spirit City"},
            {"GhostCastle_Main", "Ghost Castle"},
            {"FungalForest_Main", "Fungal Forest"},
            {"Jigoku_Main", "Jigoku"},
            {"Tengoku_Final", "Angelic Hallway"},
            {"ChaosCastle", "Chaos Castle"},
            {"Umiumi_Main", "Umi Umi"},
        };
        
        public static readonly Dictionary<string, List<string>> AreasToHelpDefineGivenRegion = new()
        {
            {
                "Witchy Woods", new()
                {
                    "17_SpiritCityBridge", "16_FairyRuins"
                }
            },
            {
                "Spirit City Sewers", new()
                {
                    "2_PipeDrop", "3_SmallPipe", "4_PipeHall", "5_PipePlatforms", "6_PipeArches", "7_LongPipe", 
                    "9_PipeStairs", "10_PipeSubBoss", "12_BottomPipe", "11_PipeChest", "7_LongPipe (1)", "ShadyInterior (1)"
                }
            },
            {"Ghost Castle", new()
            {
                "0_GhostStart", "1_GhostEntrance", "27_Crypt", "2_GhostCastleDoor", "22_ClubDemonRd", "41_FungalForest", 
                "42_PathwaytoUmiUm"
            }},
            {"Outside Fungal Forest", new()
            {
                "1_FungalEntrance", "2_DropLeft", "3_DropRight", "4_MiniBridge", "16_CuteHall", "40_MiniDrop", 
                "41_ShroomRoom", "17_MovingPlatforms", "18_Plummet", "19_MushroomClimb", "31_MiniLadder", "30_OnTop", "23_MovingEast",
                "29_MushroomAlter", "24_GenderLifts", "22_LongPuzzle", "35_LongCellar", "25_PinkMushrooms", "21_JournyDown", 
                "20_FlatRoom", "33_JumpRoom", "34_MushroomPillars", "36_ClassicJumps", "6_VerticalJunction", "38_MushroomBridges"
            }},
            {"Tengoku", new()
            {
                "7_TowerEntrance", "8_BrickHall", "9_MossyRoom", "10_LadderStairs", "11_Hook", "12_Platforms", 
                "13_LargeRoom", "15_SquareDrop", "52_PinkRoom", "49_PrincessStart", "50_SubBoss", "44_LargeTowerRoom", 
                "45_TowerHall", "48_TowerSecret", "47_TowerWide", "46_BigTower", "14_StairJunction"
            }},
            {"Slime Citadel", new ()
            {
                "1_SlimeEntrance", "2_PhoneBooth", "3_Podium", "25_OrangeShrooms", "20_Ramp", "4_NeonMushrooms", 
                "5_Slimy", "12_BunnyDropDown", "6_EndRoom", "7_DropDown", "8_NeonBanana", "9_DoubleBananas", 
                "10_Pillars", "11_Reward", "19_Candle", "13_SexyStatue", "18_SimpleStairs", "17_Climb", 
                "16_PeachEggplantRight", "36_CuteRoom", "14_ShroomPlatform", "15_SlimeGap", "38_Lanterns", "37_Picnic", 
                "41_CuteRoom2", "27_MiniBridge", "28_MiddleRoom", "32_NatureHall", "26_TallRoom", "29_LongHallway", 
                "31_EndRoom", "33_StatueSisters", "34_CandleHall", "35_Fruity", "40_LongSecret", "25_OrangeShrooms", 
                "39_Secret", "20_Ramp", "21_ShroomZag", "22_StairsDown", "23_Hallway", "24_SlimyPrize"
            }},
        };
    }
}