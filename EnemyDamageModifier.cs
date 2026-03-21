using System;
using System.Collections.Generic;
using System.Linq;
using FlipwitchAP.Archipelago;
using FlipwitchAP.Data;
using HarmonyLib;
using UnityEngine;

namespace FlipwitchAP;

public class EnemyDamageModifier

{
    private static string _previousArea = "";
    
    [HarmonyPatch(typeof(PlayerMovement), "inflictDamage", argumentTypes: [typeof(int), typeof(Transform)])]
    [HarmonyPrefix]
    private static void InflictDamage_ChangeDamageBasedOnAreaAndBarrier(ref int damage, Transform enemy)
    {
        var healthCount = Math.Min(7, SwitchDatabase.instance.getInt("hpUpgradeLevel"));
        var area = DetermineAreaGivenLevel();
        if (area == "NONE")
        {
            area = "Witchy Woods";
            if (_previousArea != "")
            {
                // If we can, use the previous area's area information.  Sometimes not accurate, but okay.
                area = _previousArea;
            }
        }
        if (area != "Chaos Castle")
        {
            var averageAreaDamage = DamageDatabase.OriginalOrderDamageLookup[DamageDatabase.AreaToOriginalOrder[area]];
            var damageBasedOnHealth = DamageDatabase.OriginalOrderDamageLookup[healthCount];
            var multiplier = damageBasedOnHealth / (float)averageAreaDamage;
            damage = (int)multiplier * damage;
        }
        var amount = Math.Min(2, SwitchDatabase.instance.getInt("APBarrier"));
        damage -= (int)((amount * damage) / 4f);
        _previousArea = area;
    }

    public static string DetermineAreaGivenLevel()
    {
        var scene = SwitchDatabase.instance.currentScene;
        switch (scene)
        {
            case "Jigoku_Main":
                return "Jigoku";
            case "ChaosCastle":
                return "Chaos Castle";
            case "Umiumi_Main":
                return "Umi Umi";
            case "UmiUmi_Main":
                return "Umi Umi";
            case "Tengoku_Final":
                return "Tengoku";
            case "GhostCastle_Main":
                return "Ghost Castle";
        }
        var level = "";
        try
        {
            level = SwitchDatabase.instance.currentLevel.name;
        }
        catch (Exception _)
        {
            return "NONE";
        }
        switch (scene)
        {
            case "WitchyWoods_Final" when
                !GenericInformation.AreasToHelpDefineGivenRegion["Witchy Woods"].Contains(level):
                return "Witchy Woods";
            case "Spirit_City_Final" when GenericInformation.AreasToHelpDefineGivenRegion["Spirit City Sewers"].Contains(level):
                return "Spirit City Sewers";
            case "FungalForest_Main" when
                GenericInformation.AreasToHelpDefineGivenRegion["Outside Fungal Forest"].Contains(level):
                return "Outside Fungal Forest";
            case "FungalForest_Main" when
                GenericInformation.AreasToHelpDefineGivenRegion["Slime Citadel"].Contains(level):
                return "Slime Citadel";
            default:
                Plugin.Logger.LogWarning($"We couldn't find an area for {level}.  If this area has enemies in it, report it.");
                return "NONE";
        }
    }
}