using FlipwitchAP.Data;
using HarmonyLib;
using UnityEngine;

namespace FlipwitchAP
{
    public class CollectMethods
    {
        [HarmonyPatch(typeof(ChestLootDrop), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_FixSwitchOrCollectOnChestLoot(ChestLootDrop __instance, ref string ___switchName,
ref bool ___dropped, ref bool ___playerWithinZone, ref float ___spawnCountdown)
        {
            ___switchName = ChestSwitchFixer(__instance.name, ___switchName);
            ___dropped = false;
            ___playerWithinZone = false;
            ___spawnCountdown = 0f;
            if (!FlipwitchLocations.CoinChestLocations.TryGetValue(___switchName, out var location))
            {
                return true;
            }

            if (SwitchDatabase.instance.getInt(___switchName) > 0 || Plugin.ArchipelagoClient.IsLocationChecked(location.APLocationID))
            {
                ___dropped = true;
                __instance.GetComponent<Animator>().enabled = false;
                __instance.GetComponent<InteractSymbol>().enabled = false;
                __instance.GetComponent<PlayAnimationOnInteract>().enabled = false;
            }
            else
            {
                __instance.GetComponent<Animator>().enabled = true;
                InteractSymbol component = __instance.GetComponent<InteractSymbol>();
                __instance.GetComponent<PlayAnimationOnInteract>().enabled = true;
                component.enabled = true;
                component.resetSymbol();
            }
            return false;
        }

        [HarmonyPatch(typeof(ChestItemDrop), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_FixSwitchOrCollectOnChestItem(ChestItemDrop __instance, ref string ___switchName,
        ref bool ___dropped, ref bool ___playerWithinZone, ref float ___spawnCountdown)
        {
            ___dropped = false;
            ___playerWithinZone = false;
            ___spawnCountdown = 0f;
            if (!FlipwitchLocations.ChestLocations.TryGetValue(__instance.itemName, out var location))
            {
                Plugin.Logger.LogWarning($"The switch {__instance.itemName} isn't in the dictionary.");
                return true;
            }
            var isSwitchToggled = SwitchDatabase.instance.getInt(___switchName) > 0;
            var isLocationChecked = Plugin.ArchipelagoClient.IsLocationChecked(location.APLocationID);
            if (isSwitchToggled || isLocationChecked)
            {
                ___dropped = true;
                __instance.GetComponent<Animator>().enabled = false;
                __instance.GetComponent<InteractSymbol>().enabled = false;
                __instance.GetComponent<PlayAnimationOnInteract>().enabled = false;
            }
            else
            {
                __instance.GetComponent<Animator>().enabled = true;
                InteractSymbol component = __instance.GetComponent<InteractSymbol>();
                __instance.GetComponent<PlayAnimationOnInteract>().enabled = true;
                component.enabled = true;
                component.resetSymbol();
            }
            return false;
        }

        [HarmonyPatch(typeof(GachaCoinCollect), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_ToggleCoinIfCollected(GachaCoinCollect __instance, ref string ___switchName, ref bool ___collected)
        {
            ___collected = SwitchDatabase.instance.getInt(___switchName) > 0;
            if (!FlipwitchLocations.CoinLocations.TryGetValue(___switchName, out var location))
            {
                return true;
            }
            if (___collected || Plugin.ArchipelagoClient.IsLocationChecked(location.APLocationID))
            {
                __instance.gameObject.SetActive(value: false);
            }
            return false;
        }

        [HarmonyPatch(typeof(HealthContainerUpgrade), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_ToggleHealthHeartIfCollected(HealthContainerUpgrade __instance, ref string ___switchName, ref bool ___collected)
        {
            if (!FlipwitchLocations.StatLocations.TryGetValue(___switchName, out var location))
            {
                var nonEuropeanSwitchName = ___switchName.Replace(",", ".");
                if (!FlipwitchLocations.StatLocations.TryGetValue(nonEuropeanSwitchName, out var euroLocation))
                {
                    return true;
                }
                location = euroLocation;
                
            }
            if (SwitchDatabase.instance.getInt(___switchName) > 0 || Plugin.ArchipelagoClient.IsLocationChecked(location.APLocationID))
            {
                __instance.gameObject.SetActive(value: false);
                ___collected = true;
            }
            return false;
        }

        [HarmonyPatch(typeof(ManaContainerUpgrade), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_ToggleManaHeartIfCollected(HealthContainerUpgrade __instance, ref string ___switchName, ref bool ___collected)
        {
            if (!FlipwitchLocations.StatLocations.TryGetValue(___switchName, out var location))
            {
                var nonEuropeanSwitchName = ___switchName.Replace(",", ".");
                if (!FlipwitchLocations.StatLocations.TryGetValue(nonEuropeanSwitchName, out var euroLocation))
                {
                    return true;
                }
                location = euroLocation;
                
            }
            if (SwitchDatabase.instance.getInt(___switchName) > 0 || Plugin.ArchipelagoClient.IsLocationChecked(location.APLocationID))
            {
                __instance.gameObject.SetActive(value: false);
                ___collected = true;
            }
            return false;
        }

        public static string ChestSwitchFixer(string chestName, string switchName)
        {
            if (chestName == "Drop Coins Chest" && switchName == "gc_slime_secret_coins")
            {
                return "gc_slime_secret_coins_upper";
            }
            return switchName;
        }
    }
}