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
    private static readonly Dictionary<string, float> DamageMultiplierPerOrder = new();
    
    [HarmonyPatch(typeof(PlayerMovement), "inflictDamage", argumentTypes: [typeof(int), typeof(Transform)])]
    [HarmonyPrefix]
    private static void InflictDamage_ChangeDamageBasedOnAreaAndBarrier(ref int damage, Transform enemy)
    {
        var area = DetermineAreaGivenLevel();
        if (area != "Chaos Castle" && area != "NONE")
        {
            if (!DamageMultiplierPerOrder.TryGetValue(area, out var multiplier))
            {
                var lastOrder = ArchipelagoClient.ServerData.AreaOrder.Count - 1;
                if (lastOrder == -1)
                {
                    Plugin.Logger.LogWarning($"Hit by damage in an area where there is no last.");
                }
                else
                {
                    var lastArea = ArchipelagoClient.ServerData.AreaOrder.First(x => x.Value == lastOrder).Key;
                    multiplier = DamageMultiplierPerOrder[lastArea];
                    damage = (int)(damage * multiplier);
                }
            }
            else
            {
                
                damage = (int)(damage * multiplier);
            }
        }
        var amount = Math.Min(2, SwitchDatabase.instance.getInt("APBarrier"));
        damage -= (int)((amount * damage) / 4f);
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
        catch (Exception e)
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
    
    public static void AssignDamageMultiplierForAreaGivenOrder(string area)
    {
        var originalOrder = DamageDatabase.AreaToOriginalOrder[area];
        var originalMaxDamage = (float)DamageDatabase.OriginalOrderDamageLookup[originalOrder];
        var order = ArchipelagoClient.ServerData.AreaOrder[area];
        var orderMaxDamage = DamageDatabase.OriginalOrderDamageLookup[order];
        var multiplier = orderMaxDamage / originalMaxDamage;
        DamageMultiplierPerOrder[area] = multiplier;
    }
}