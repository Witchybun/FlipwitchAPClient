using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FlipwitchAP.Archipelago;
using HarmonyLib;
using UnityEngine;

namespace FlipwitchAP
{
    public class CutsceneHelper
    {

        public static bool hasDied = false;
        public static bool currentlyCutsceneTrapped = false;
        public static bool trapRoutineRunning = false;
        public static Queue<string> cutsceneTrapQueue = new();

        [HarmonyPatch(typeof(Cutscene), "unlockCutscene")]
        [HarmonyPrefix]
        private static bool UnlockCutscene_DontGiveSexualExperienceIfOnAll(Cutscene __instance, string cutsceneUnlockedID, bool countTowardsSexualExperiences)
        {
            var isSexShuffled = ArchipelagoClient.ServerData.QuestForSex == ArchipelagoData.Quest.All;
            SwitchDatabase instance = SwitchDatabase.instance;
            instance.setBool(cutsceneUnlockedID, value: true);
            bool flag = false;
            for (int i = 0; i < __instance.cutscenes.Count; i++)
            {
                if (cutsceneUnlockedID == __instance.cutscenes[i].cutsceneUnlockedID && !__instance.cutscenes[i].unlocked)
                {
                    __instance.cutscenes[i].unlockCutscene(unlock: true);
                    if (countTowardsSexualExperiences)
                    {
                        flag = true;
                    }
                }
            }
            if (flag && !isSexShuffled)
            {
                instance.setInt("SexualExperienceCount", instance.getInt("SexualExperienceCount") + 1);
            }
            CheckUnlockRewardsForArchipelago();
            return false;
        }

        [HarmonyPatch(typeof(Cutscene), "endCutscene")]
        [HarmonyPostfix]
        private static void EndCutscene_AlsoEndCutsceneTrap()
        {
            currentlyCutsceneTrapped = false;
        }

        [HarmonyPatch(typeof(Cutscene), "calculateUnlockedRewards")]
        [HarmonyPrefix]
        private static bool CalculateUnlockedRewards_CalculateUsingOwnUnlockCount(Cutscene __instance)
        {
            int playerPeachChargesCap = SwitchDatabase.instance.getInt("APTotalPeachCharges");
            int num = SwitchDatabase.instance.getInt("SexualExperienceCount") / 4;
            int @int = SwitchDatabase.instance.getInt("PendingPeachCharges");
            SwitchDatabase.instance.setInt("AppliedAndPendingPeaches", playerPeachChargesCap + num);
            SwitchDatabase.instance.setInt("PendingPeachCharges", num - playerPeachChargesCap);
            UnityEngine.Debug.Log("SexualExperienceCount: " + SwitchDatabase.instance.getInt("SexualExperienceCount"));
            if (num >= 2)
            {
                SwitchDatabase.instance.setInt("PendingWandLevel", 1);
            }
            if (num >= 6)
            {
                SwitchDatabase.instance.setInt("PendingWandLevel", 2);
            }
            if (@int < SwitchDatabase.instance.getInt("PendingPeachCharges"))
            {
                SwitchDatabase.instance.triggerUpgradePendingPopup();
            }
            SwitchDatabase.instance.upgradePendingPopup.updatePendingPopupSymbol();
            return false;
        }

        [HarmonyPatch(typeof(GameOverManager), "updateAnim")]
        [HarmonyPostfix]
        private static void EndCutscene_AddExtraEffects()
        {
            if (ArchipelagoClient.ServerData.DeathLink && !ArchipelagoClient.PlayerWasDeathlinked)
            {
                Plugin.ArchipelagoClient.KillEveryone();
            }
            hasDied = true;
            ArchipelagoClient.PlayerWasDeathlinked = false;
        }

        public static IEnumerator HandleAllPendingCutsceneTraps()
        {
            trapRoutineRunning = true;
            while (true)
            {
                if (currentlyCutsceneTrapped)
                {
                    yield return new WaitForSeconds(5f);
                }
                if (!cutsceneTrapQueue.Any()) break;
                var _ = cutsceneTrapQueue.Dequeue();
                currentlyCutsceneTrapped = true;
                var random = new System.Random(ArchipelagoClient.ServerData.Seed + DateTime.Now.Millisecond);
                var chosenLength = Plugin.ArchipelagoClient.CutsceneIDsForTraps.Count;
                var chosenCutscene = Plugin.ArchipelagoClient.CutsceneIDsForTraps[random.Next(0, chosenLength - 1)];
                SwitchDatabase.instance.cutsceneManager.startCutscene(chosenCutscene, false);
            }
            yield return null;
            trapRoutineRunning = false;
        }

        public static void CheckUnlockRewardsForArchipelago()
        {
            CalculateUnlockedRewards_CalculateUsingOwnUnlockCount(SwitchDatabase.instance.cutsceneManager);
        }
    }
}