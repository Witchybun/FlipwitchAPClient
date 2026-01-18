using FlipwitchAP.Archipelago;
using FlipwitchAP.Data;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlipwitchAP;

public class TeleportHelper
{
    
    [HarmonyPatch(typeof(EndCutsceneIngame), "OnEnable")]
    [HarmonyPostfix]
    private static void OnEnable_BitchslapThePlayerToTheirRightfulPlace(EndCutsceneIngame __instance)
    {
        if (SwitchDatabase.instance.currentLevel.name != "1_EntranceSpawn" ||
            __instance.transform.parent.name != "Intro Cutscene") return;
        if (ArchipelagoClient.ServerData.StartingArea == ArchipelagoData.StartArea.Beatrice) return;
        var destination = TeleportData.StartingAreaToWarpInfo[ArchipelagoClient.ServerData.StartingArea];
        ArchipelagoClient.AP.FirstTimeWarp = true;
        SwitchDatabase.instance.levelLoader.LoadNextLevel(destination.Scene, destination.Position, SwitchDatabase.instance.playerMov);
    }

    [HarmonyPatch(typeof(Teleporter), "Update")]
    [HarmonyPrefix]
    private static bool Update_DisallowGivingWarpIfCrystalTeleport(Teleporter __instance, ref bool ___intSet)
    {
        if (!ArchipelagoClient.ServerData.CrystalTeleport)
        {
            return true;
        }
        if (!(Time.timeScale > 0f))
        {
            return false;
        }
        if (___intSet)
        {
            // Makes it so it doesn't spam try to set the bool.
            ___intSet = true;
        }
        return true;
    }
}