using System.Collections.Generic;
using FlipwitchAP.Archipelago;
using UnityEngine;

namespace FlipwitchAP.Data;

public static class TeleportData
{
    public class DestinationData
    {
        public readonly string Scene;
        public readonly Vector2 Position;
        public DestinationData(string scene, Vector2 position)
        {
            Scene = scene;
            Position = position;
        }
    }
    
    public static readonly Dictionary<ArchipelagoData.StartArea, string> TeleportSwitchLookup = new()
    {
        {ArchipelagoData.StartArea.Beatrice, "TP_Beatrix"},
        {ArchipelagoData.StartArea.WitchyWoods, "TP_WitchyWoods"},
        {ArchipelagoData.StartArea.Spirit, "TP_SpiritCity"},
        {ArchipelagoData.StartArea.Goblin, "TP_GoblinCaves"},
        {ArchipelagoData.StartArea.Alley, "TP_ShadyAlley"},
        {ArchipelagoData.StartArea.OutsideGhost, "TP_GhostCastleEntrance"},
        {ArchipelagoData.StartArea.GhostCastle, "TP_GhostCastle"},
        {ArchipelagoData.StartArea.Jigoku, "TP_Jigoku2_MiniBridge"},
        {ArchipelagoData.StartArea.ClubDemon, "TP_ClubDemon"},
        {ArchipelagoData.StartArea.Tengoku, "TP_Tengoku"},
        {ArchipelagoData.StartArea.AngelicHallway, "TP_Angelic_Hallway"},
        {ArchipelagoData.StartArea.FungalForest, "TP_FungalForest"},
        {ArchipelagoData.StartArea.SlimeCitadel, "TP_SlimeCitadel"},
        {ArchipelagoData.StartArea.SlimyDepths, "TP_SlimyDepths"},
        {ArchipelagoData.StartArea.UmiUmi, "TP_UmiUmi"},
    };

    public static readonly Dictionary<ArchipelagoData.StartArea, DestinationData> StartingAreaToWarpInfo = new()
    {
        { ArchipelagoData.StartArea.Spirit, new DestinationData("Spirit_City_Final", new Vector2(58.5f, -34f)) },
        { ArchipelagoData.StartArea.Goblin, new DestinationData("WitchyWoods_Final", new Vector2(280f, -99f)) },
        { ArchipelagoData.StartArea.GhostCastle, new DestinationData("GhostCastle_Main", new Vector2(142.5f, 143f)) },
        { ArchipelagoData.StartArea.Jigoku, new DestinationData("Jigoku_Main", new Vector2(36f, 25f)) },
        { ArchipelagoData.StartArea.ClubDemon, new DestinationData("Jigoku_Main", new Vector2(36.5f, -135f)) },
        { ArchipelagoData.StartArea.Tengoku, new DestinationData("Tengoku_Final", new Vector2(6f, 151f)) },
        { ArchipelagoData.StartArea.SlimeCitadel, new DestinationData("FungalForest_Main", new Vector2(543f, -102f)) },
        { ArchipelagoData.StartArea.UmiUmi, new DestinationData("UmiUmi_Main", new Vector2(23f, 27f)) },
    };

}