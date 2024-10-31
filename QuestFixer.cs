using System.Linq;
using FlipwitchAP.Data;
using HarmonyLib;
using UnityEngine;

namespace FlipwitchAP
{
    public class QuestFixer
    {
        // Patches certain events that rely too much on holding the item to continue progress.
        // Instead of checking for items, it checks for custom flags set upon getting the quest item.
        [HarmonyPatch(typeof(NPCDialogueAdvanced), "Start")]
        [HarmonyPostfix]
        private static void Start_FixQuests(NPCDialogueAdvanced __instance)
        {
            switch (__instance.gameObject.name)
            {
                case "Goblin Model - Quest 1":
                    {
                        var wasGivenHeadshot = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APGoblianaGaveHeadshot",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasNotGivenheadshot = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APGoblianaGaveHeadshot",
                            switchValue = 0,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasNotGivenBusinessCard = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APLegsGaveCard",
                            switchValue = 0,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasGivenBusinessCard = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APLegsGaveCard",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var goblianaGroups = __instance.dialogueGroups;
                        if (goblianaGroups[0].itemRequirements.Any())
                        {
                            goblianaGroups[0].itemRequirements.Clear();
                            goblianaGroups[0].switches.Add(wasNotGivenheadshot);
                            goblianaGroups[0].switches.Add(wasNotGivenBusinessCard);
                        }
                        if (goblianaGroups[1].itemRequirements.Any())
                        {
                            goblianaGroups[1].itemRequirements.RemoveAt(0);
                            goblianaGroups[1].switches.Add(wasGivenHeadshot);
                        }
                        if (!goblianaGroups[2].switches.Any(x => x.switchName == "APLegsGaveCard"))
                        {
                            goblianaGroups[2].switches.Add(wasGivenBusinessCard);
                        }
                        var firstState = goblianaGroups[1];
                        var secondState = goblianaGroups[2];
                        goblianaGroups[1] = secondState;
                        goblianaGroups[2] = firstState;
                        return;
                    }
                case "Seedy Goblin Quest 1":
                    {
                        var wasGivenHeadshot = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APGoblianaGaveHeadshot",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasNotGivenBusinessCard = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APLegsGaveCard",
                            switchValue = 0,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasGivenBusinessCard = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APLegsGaveCard",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var neverMetLegs = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "GoblinModelQuest1",
                            switchValue = 0,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var introModifiedQuestCheck = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "GoblinModelQuest1",
                            switchValue = 2,
                            comparisonOperator = ComparisonOperators.IS_LESS_THAN
                        };
                        var legsGroups = __instance.dialogueGroups;
                        if (legsGroups[0].itemRequirements.Any())
                        {
                            legsGroups[0].itemRequirements.Clear();
                            legsGroups[0].switches.Add(wasNotGivenBusinessCard);
                        }
                        if (!legsGroups[1].switches.Any(x => x.switchName == "APGoblianaGaveHeadshot"))
                        {
                            legsGroups[1].switches.Add(wasGivenHeadshot);
                        }
                        if (legsGroups[2].itemRequirements.Any())
                        {
                            legsGroups[2].itemRequirements.RemoveAt(1);
                        }
                        if (!legsGroups[3].switches.Any(x => x.switchName == "APGoblianaGaveHeadshot"))
                        {
                            legsGroups[3].switches.Add(wasGivenHeadshot);
                        }
                        if (legsGroups[4].itemRequirements.Any())
                        {
                            legsGroups[4].itemRequirements.Clear();
                            legsGroups[4].switches.Add(wasGivenBusinessCard);
                        }
                        //0 -> legs says hi first time.
                        //1 -> legs says hi, but you have the headshot already
                        //2 -> legs reminds the player to find someone
                        //3 -> player has the headshot; doesn't check if they met legs yet
                        //4 -> legs notes that the player should give the card
                        /*var firstState = legsGroups[0];
                        var secondState = legsGroups[1];
                        var thirdState = legsGroups[2];
                        var fourthState = legsGroups[3];
                        var lastState = legsGroups[4];
                        legsGroups[0] = lastState;
                        legsGroups[1] = fourthState;
                        legsGroups[2] = firstState;
                        legsGroups[3] = secondState;
                        legsGroups[4] = thirdState;*/
                        return;
                    }
                case "CowGirl - Quest 2":
                    {
                        var belleGroups = __instance.dialogueGroups;
                        var wasGivenMilk = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APBelleGaveMilk",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        if (belleGroups[4].itemRequirements.Any())
                        {
                            belleGroups[4].itemRequirements.Clear();
                            belleGroups[4].switches.Add(wasGivenMilk);
                        }
                        return;
                    }
                case "Food Merchant Quest":
                    {
                        var wasGivenMilkshake = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APKyoniGaveMilkshake",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasGivenMilk = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APBelleGaveMilk",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var kyoniGroups = __instance.dialogueGroups;
                        if (kyoniGroups[0].itemRequirements.Any())
                        {
                            kyoniGroups[0].itemRequirements.Clear();
                            kyoniGroups[0].switches.Add(wasGivenMilkshake);
                        }
                        if (!kyoniGroups[3].switches.Any())
                        {
                            kyoniGroups[3].switches.Add(wasGivenMilk);
                        }
                        return;
                    }
                case "Pink Angel":
                    {
                        var wasGivenDemonLetter = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APDemonGaveLetter",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasGivenAngelLetter = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APAngelGaveLetter",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var angelGroups = __instance.dialogueGroups;
                        if (!angelGroups[1].switches.Any(x => x.switchName == "APDemonGaveLetter"))
                        {
                            angelGroups[1].switches.Add(wasGivenDemonLetter);
                        }
                        if (angelGroups[2].itemRequirements.Any())
                        {
                            angelGroups[2].itemRequirements.Clear();
                            angelGroups[2].switches.Add(wasGivenAngelLetter);
                        }
                        if (angelGroups[3].itemRequirements.Any())
                        {
                            angelGroups[3].itemRequirements.Clear();
                            angelGroups[3].switches.Add(wasGivenAngelLetter);
                        }
                        if (!angelGroups[4].switches.Any(x => x.switchName == "APDemonGaveLetter"))
                        {
                            angelGroups[4].switches.Add(wasGivenAngelLetter);
                            angelGroups[4].switches.Add(wasGivenDemonLetter);
                        }
                        return;
                    }
                case "Bad Boy":
                    {
                        var wasGivenDemonLetter = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APDemonGaveLetter",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasGivenAngelLetter = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APAngelGaveLetter",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var demonGroups = __instance.dialogueGroups;
                        if (demonGroups[3].itemRequirements.Any())
                        {
                            demonGroups[3].itemRequirements.Clear();
                            demonGroups[3].switches.Add(wasGivenDemonLetter);
                        }
                        if (!demonGroups[4].switches.Any(x => x.switchName == "APAngelGaveLetter"))
                        {
                            demonGroups[4].switches.Add(wasGivenAngelLetter);
                        }
                        return;
                    }
                case "Elf Maid":
                    {
                        var wasNotGivenDeed = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APMushroomGaveDeed",
                            switchValue = 0,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasGivenDeed = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APMushroomGaveDeed",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var maidGroups = __instance.dialogueGroups;
                        if (maidGroups[2].itemRequirements.Any())
                        {
                            maidGroups[2].itemRequirements.Clear();
                            maidGroups[2].switches.Add(wasNotGivenDeed);
                        }
                        if (!maidGroups[3].switches.Any(x => x.switchName == "APMushroomGaveDeed"))
                        {
                            maidGroups[3].switches.Add(wasGivenDeed);
                        }
                        return;
                    }
                case "MomoBot Server Disabled":
                    {
                        var wasNotGivenPassword = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APMomoGavePassword",
                            switchValue = 0,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasGivenPassword = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APMomoGavePassword",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var momoGroups = __instance.dialogueGroups;
                        if (momoGroups[0].itemRequirements.Any())
                        {
                            momoGroups[0].itemRequirements.Clear();
                            momoGroups[0].switches.Add(wasGivenPassword);
                        }
                        if (momoGroups[1].itemRequirements.Any())
                        {
                            momoGroups[1].itemRequirements.Clear();
                            momoGroups[1].switches.Add(wasNotGivenPassword);
                        }
                        return;
                    }
                case "MomoBot Server Enabled":
                    {
                        var wasNotGivenPassword = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APMomoGavePassword",
                            switchValue = 0,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasGivenPassword = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APMomoGavePassword",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var momoGroups = __instance.dialogueGroups;
                        if (momoGroups[0].itemRequirements.Any())
                        {
                            momoGroups[0].itemRequirements.Clear();
                            momoGroups[0].switches.Add(wasNotGivenPassword);
                        }
                        if (momoGroups[1].itemRequirements.Any())
                        {
                            momoGroups[1].itemRequirements.Clear();
                            momoGroups[1].switches.Add(wasGivenPassword);
                        }
                        return;
                    }
                case "Server":
                    {
                        var wasNotGivenPassword = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APMomoGavePassword",
                            switchValue = 0,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasGivenPassword = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APMomoGavePassword",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var serverGroups = __instance.dialogueGroups;
                        if (serverGroups[0].itemRequirements.Any())
                        {
                            serverGroups[0].itemRequirements.Clear();
                            serverGroups[0].switches.Add(wasNotGivenPassword);
                        }
                        if (!serverGroups[1].switches.Any(x => x.switchName == "APMomoGavePassword"))
                        {
                            serverGroups[1].switches.Add(wasGivenPassword);
                        }
                        return;
                    }
                case "Goblin Model - Quest 2":
                    {
                        var wasNotGivenKey = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APGoblianaGaveKey",
                            switchValue = 0,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasGivenKey = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APGoblianaGaveKey",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasNotGivenLuggage = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APExGaveLuggage",
                            switchValue = 0,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var wasGivenLuggage = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APExGaveLuggage",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var goblianaGroups = __instance.dialogueGroups;
                        if (goblianaGroups[1].itemRequirements.Any())
                        {
                            goblianaGroups[1].itemRequirements.Clear();
                            goblianaGroups[1].switches.Add(wasGivenLuggage);
                        }
                        if (goblianaGroups[2].itemRequirements.Any())
                        {
                            goblianaGroups[2].itemRequirements.Clear();
                            goblianaGroups[2].switches.Add(wasGivenKey);
                        }
                        if (goblianaGroups[4].itemRequirements.Any())
                        {
                            goblianaGroups[4].itemRequirements.Clear();
                            goblianaGroups[4].switches.Add(wasNotGivenKey);
                        }
                        if (goblianaGroups[5].itemRequirements.Any())
                        {
                            goblianaGroups[5].switches.Add(wasGivenKey);
                        }
                        if (!goblianaGroups[6].switches.Any(x => x.switchName == "APExGaveLuggage"))
                        {
                            goblianaGroups[6].switches.Add(wasGivenLuggage);
                        }
                        return;
                    }
                case "Flipwitch_Sensei":
                    {
                        if (__instance.gameObject.scene.name != "WitchyWoods_Final")
                        {
                            return;
                        }
                        var areWeInMainSpot = GameObject.Find("WitchyWoods").transform.Find("Grid").Find("World").Find("1_EntranceSpawn").Find("LevelData").gameObject.activeSelf;
                        if (!areWeInMainSpot)
                        {
                            return;
                        }
                        var senseiGroups = __instance.dialogueGroups;
                        var receivedNoWandUpgrade = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APPlayerWand",
                            switchValue = 1,
                            comparisonOperator = ComparisonOperators.IS_LESS_THAN
                        };
                        var receivedFirstWandNotSecond = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APPlayerWand",
                            switchValue = 2,
                            comparisonOperator = ComparisonOperators.IS_LESS_THAN
                        };

                        if (!senseiGroups[4].switches.Any(x => x.switchName == "APPlayerWand"))
                        {
                            senseiGroups[4].switches[1] = receivedNoWandUpgrade;
                        }
                        if (!senseiGroups[5].switches.Any(x => x.switchName == "APPlayerWand"))
                        {
                            senseiGroups[5].switches[1] = receivedNoWandUpgrade;
                        }
                        if (!senseiGroups[6].switches.Any(x => x.switchName == "APPlayerWand"))
                        {
                            senseiGroups[6].switches[1] = receivedFirstWandNotSecond;
                        }
                        if (!senseiGroups[7].switches.Any(x => x.switchName == "APPlayerWand"))
                        {
                            senseiGroups[7].switches[1] = receivedFirstWandNotSecond;
                        }

                        // 4 is first wand upgrade but wrong
                        // 5 is first wand upgrade
                        // 6 is second but you're a woman
                        // 7 is second but you're a man
                        return;
                    }
                case "Lucky Cat Dialogue":
                    {
                        var receivedSouls = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APCatStatueCount",
                            switchValue = 2,
                            comparisonOperator = ComparisonOperators.IS_MORE_THAN
                        };
                        var catGroups = __instance.dialogueGroups;
                        if (!catGroups[1].switches.Any(x => x.switchName == "APCatStatueCount"))
                        {
                            catGroups[1].switches.Add(receivedSouls);
                        }
                        return;
                    }
                case "Philosopher's Stone Dialogue":
                    {
                        var receivedStones = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APSummonStoneCount",
                            switchValue = 2,
                            comparisonOperator = ComparisonOperators.IS_MORE_THAN
                        };
                        var stoneGroups = __instance.dialogueGroups;
                        if (!stoneGroups[1].switches.Any(x => x.switchName == "APSummonStoneCount"))
                        {
                            stoneGroups[1].switches.Add(receivedStones);
                        }
                        return;
                    }
                case "FortuneTeller":
                    {
                        var madeUpShit = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APMYFUGGENDICC",
                            switchValue = 999,
                            comparisonOperator = ComparisonOperators.IS_EQUAL_TO
                        };
                        var fortuneGroups = __instance.dialogueGroups;
                        if (!fortuneGroups[0].switches.Any(x => x.switchName == "APMYFUGGENDICC"))
                        {
                            fortuneGroups[0].switches.Add(madeUpShit);
                        }
                        if (!fortuneGroups[2].switches.Any(x => x.switchName == "APMYFUGGENDICC"))
                        {
                            fortuneGroups[2].itemRequirements.Clear();
                        }
                        for (var i = 3; i < 9; i++)
                        {
                            if (!fortuneGroups[i].switches.Any(x => x.switchName == "APMYFUGGENDICC"))
                            {
                                fortuneGroups[i].switches.Add(madeUpShit);
                            }
                        }
                        DialogueHelper.GenerateCurrentHintForFortuneTeller(__instance.gameObject.scene.name);
                        return;
                    }
            }
        }
    }
}