using FlipwitchAP.Archipelago;
using HarmonyLib;
using UnityEngine;

namespace FlipwitchAP;

public class BasicMovement
{
    // No roll unless ghost.
    [HarmonyPatch(typeof(PlayerMovement), "updateRollInput")]
    [HarmonyPrefix]
    private static bool UpdateRollInput_DontRollIfNoItem(PlayerMovement __instance, ref bool ___HasDemonDashed, ref bool ___IsHealing, ref float ___RollTimer, 
        ref bool ___IsAttacking, ref bool ___EnemyStruck, ref bool ___allowMovement, ref float ___rollCoolDown, ref bool ___IsRangeAttacking, 
        ref bool ___IsCrouching)
    {
        if (!ArchipelagoClient.ServerData.ShuffleRoll)
        {
            return true;
        }
        if (___HasDemonDashed || ___IsHealing)
        {
            return false;
        }
        if (___RollTimer > 0f)
        {
            ___RollTimer -= Time.deltaTime;
        }
        if (NewInputManager.instance.Roll.pressedThisFrame && 
            (!___IsAttacking || (___IsAttacking && ___EnemyStruck)) && !__instance.IsRolling && !__instance.climbing && ___allowMovement && ___rollCoolDown <= 0f && 
            (!___IsRangeAttacking || !___IsCrouching) && !SwitchDatabase.instance.dialogueManager.dialogueOrCutsceneOrIngameCutsceneInProgress() && 
            __instance.controller.isGrounded())
        {
            if (__instance.IsGhost)
            {
                __instance.GetType().GetMethod("_StartRoll").Invoke(__instance, [true]);
            }
            else
            {
                var canRoll = SwitchDatabase.instance.getBool("APCanRoll");
                if (canRoll) __instance.GetType().GetMethod("_StartRoll").Invoke(__instance, [true]);
            }
        }
        if (__instance.IsRolling && ___RollTimer <= 0f)
        {
            __instance.GetType().GetMethod("endRoll").Invoke(__instance, null);
        }

        return false;
    }

    [HarmonyPatch(typeof(PlayerController), "Move")]
    [HarmonyPostfix]
    private static void Move_ClaimUsedDoubleIfNoDouble(PlayerController __instance, float move, bool crouch, bool jump, 
        bool climbing, float moveY, bool isSlime, ref bool ___singleJump, ref bool ___doubleJump)
    {
        if (!ArchipelagoClient.ServerData.ShuffleDoubleJump)
        {
            return;
        }

        if (!SwitchDatabase.instance.getBool("APCanDouble"))
        {
            if (___singleJump)
            {
                ___doubleJump = true;
            }
            
        }
        
    }
}