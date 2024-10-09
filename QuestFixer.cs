using System.Linq;
using HarmonyLib;
using Mono.Cecil.Cil;
using UnityEngine;

namespace FlipwitchAP
{
    public class QuestFixer
    {
        public QuestFixer()
        {
            Harmony.CreateAndPatchAll(typeof(QuestFixer));
        }

        // Patches certain events that rely too much on holding the item to continue progress.
        // Instead of checking for items, it checks for custom flags set upon getting the quest item.
        [HarmonyPatch(typeof(NPCDialogueAdvanced), "Start")]
        [HarmonyPostfix]
        private static void Start_FixQuests(NPCDialogueAdvanced __instance)
        {
            Plugin.Logger.LogInfo(__instance.gameObject.name);
            switch (__instance.gameObject.name)
            {
                case "Goblin Model - Quest 1":
                    {
                        Plugin.Logger.LogInfo("Handling Gobliana.");
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
                        var introSpecialCase = new NPCDialogueAdvanced.SwitchRequirement()
                        {
                            switchName = "APGoblianaGaveHeadshot",
                            switchValue = 2,
                            comparisonOperator = ComparisonOperators.IS_LESS_THAN
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
                            legsGroups[0].switches.Add(introSpecialCase);
                            legsGroups[0].switches[1] = introModifiedQuestCheck;
                        }
                        if (!legsGroups[1].switches.Any(x => x.switchName == "APGoblianaGaveHeadshot"))
                        {
                            legsGroups[1].switches.Add(wasGivenHeadshot);
                        }
                        if (legsGroups[2].itemRequirements.Any())
                        {
                            legsGroups[2].itemRequirements.Clear();
                            legsGroups[2].switches.Add(wasGivenBusinessCard);
                            legsGroups[2].switches.Add(wasGivenHeadshot);
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
                            goblianaGroups[4].switches.Add(wasNotGivenLuggage);
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
            }
        }
    }
}