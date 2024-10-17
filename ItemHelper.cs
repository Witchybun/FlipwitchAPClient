using FlipwitchAP.Data;
using FlipwitchAP.Archipelago;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Archipelago.MultiClient.Net.Enums;
using System;
using System.Diagnostics;
using UnityEngine;

namespace FlipwitchAP
{
    public class ItemHelper
    {

        public ItemHelper()
        {
            Harmony.CreateAndPatchAll(typeof(ItemHelper));
        }


        public static void GiveFlipwitchItem(string name, bool skipPopup = false)
        {
            switch (name)
            {
                case "Nothing":
                    {
                        return;
                    }
                case "Protective Barrier Upgrade":
                    {
                        var currentBarrier = SwitchDatabase.instance.getInt("APBarrier");
                        var additional = Math.Min(2, 1 + currentBarrier);
                        SwitchDatabase.instance.setInt("APBarrier", additional);
                        return;
                    }
                case "Animal Coin":
                    {
                        var currentCount = SwitchDatabase.instance.getInt("APAnimalCoin");
                        SwitchDatabase.instance.setInt("APAnimalCoin", currentCount + 1);
                        SwitchDatabase.instance.GetType().GetMethod("refreshGachaTokenCount", GenericMethods.Flags).Invoke(SwitchDatabase.instance, null);
                        return;
                    }
                case "Bunny Coin":
                    {
                        var currentCount = SwitchDatabase.instance.getInt("APBunnyCoin");
                        SwitchDatabase.instance.setInt("APBunnyCoin", currentCount + 1);
                        SwitchDatabase.instance.GetType().GetMethod("refreshGachaTokenCount", GenericMethods.Flags).Invoke(SwitchDatabase.instance, null);
                        return;
                    }
                case "Monster Coin":
                    {
                        var currentCount = SwitchDatabase.instance.getInt("APMonsterCoin");
                        SwitchDatabase.instance.setInt("APMonsterCoin", currentCount + 1);
                        SwitchDatabase.instance.GetType().GetMethod("refreshGachaTokenCount", GenericMethods.Flags).Invoke(SwitchDatabase.instance, null);
                        return;
                    }
                case "Angel & Demon Coin":
                    {
                        var currentCount = SwitchDatabase.instance.getInt("APAngelDemonCoin");
                        SwitchDatabase.instance.setInt("APAngelDemonCoin", currentCount + 1);
                        SwitchDatabase.instance.GetType().GetMethod("refreshGachaTokenCount", GenericMethods.Flags).Invoke(SwitchDatabase.instance, null);
                        return;
                    }
                case "Promotional Coin":
                    {
                        var currentCount = SwitchDatabase.instance.getInt("APPromotionalCoin");
                        SwitchDatabase.instance.setInt("APPromotionalCoin", currentCount + 1);
                        SwitchDatabase.instance.GetType().GetMethod("refreshGachaTokenCount", GenericMethods.Flags).Invoke(SwitchDatabase.instance, null);
                        return;
                    }
                case "Woods Crystal Blockade Removal":
                    {
                        SwitchDatabase.instance.setInt("APGreatFairyStory", 1);
                        return;
                    }
                case "City Crystal Blockade Removal":
                    {
                        SwitchDatabase.instance.setInt("APCityCrystalDestroyed", 1);
                        return;
                    }
                case "Peachy Peach Charge":
                    {
                        var peachGiven = SwitchDatabase.instance.getInt("PeachGiven");
                        if (peachGiven < 1)
                        {
                            var ints = (Dictionary<string, int>)SwitchDatabase.instance.GetType().GetField("ints", GenericMethods.Flags).GetValue(SwitchDatabase.instance);
                            ints["PeachGiven"] += 1;
                            SwitchDatabase.instance.GetType().GetField("ints", GenericMethods.Flags).SetValue(SwitchDatabase.instance, ints);
                            SwitchDatabase.instance.updatePeachyPeachUI();
                            SwitchDatabase.instance.givePlayerItem("PeachyPeach", false);
                            return;
                        }
                        var peachCharges = (int)SwitchDatabase.instance.GetType().GetField("playerPeachCharges", GenericMethods.Flags).GetValue(SwitchDatabase.instance);
                        SwitchDatabase.instance.setPeachCharges(peachCharges + 1);
                        SwitchDatabase.instance.upgradePendingPopup.updatePendingPopupSymbol();
                        return;
                    }
                case "Peachy Peach Upgrade":
                    {
                        var newUpgradeAmount = Math.Min(2, SwitchDatabase.instance.getInt("HealStrengthLevel") + 1);
                        SwitchDatabase.instance.setInt("HealStrengthUpgrade", newUpgradeAmount);
                        return;
                    }
                case "Health Upgrade":
                    {
                        SwitchDatabase.instance.upgradeHPLevel();
                        return;
                    }
                case "Mana Upgrade":
                    {
                        SwitchDatabase.instance.upgradeMPLevel();
                        return;
                    }
                case "Wand Upgrade":
                    {
                        var currentLevel = SwitchDatabase.instance.getInt("playerWandLevel");
                        if (currentLevel == 0)
                        {
                            SwitchDatabase.instance.setInt("playerWandLevel", 1);
                            SwitchDatabase.instance.playerMov.refreshWandLevel();
                            SwitchDatabase.instance.setInt("APPlayerWand", 1);
                        }
                        else if (currentLevel == 1)
                        {
                            SwitchDatabase.instance.setInt("playerWandLevel", 2);
                            SwitchDatabase.instance.playerMov.refreshWandLevel();
                            SwitchDatabase.instance.setInt("APPlayerWand", 2);
                        }
                        return;
                    }
                case "Loose Change":
                    {
                        var random = new System.Random(ArchipelagoClient.ServerData.Seed + DateTime.Now.Millisecond);
                        var amount = random.Next(20, 200);
                        SwitchDatabase.instance.addToCoinCoint(amount);
                        return;
                    }
                case "Healing Surge":
                    {
                        SwitchDatabase.instance.healPlayer(SwitchDatabase.instance.getPlayerHealthCap());
                        return;
                    }
                case "Mana Surge":
                    {
                        SwitchDatabase.instance.addMana(SwitchDatabase.instance.getPlayerManaCap());
                        return;
                    }
                case "Peachy Peach Recharge":
                    {
                        SwitchDatabase.instance.playerMov.restorePeachCharges();
                        return;
                    }
                case "Sexual Thoughts":
                    {
                        // Figure out how to play moan sound effects.
                        return;
                    }

            }
            if (name.Contains(" Figure #"))
            {
                var nameArray = name.Split(" Figure #");
                var genre = nameArray[0];
                var number = int.Parse(nameArray[1]);
                switch (genre)
                {
                    case "Special Promotion":
                        {
                            SwitchDatabase.instance.gachaCollections[4].gachas[0].unlocked = true;
                            return;
                        }
                    case "Animal Girls":
                        {
                            SwitchDatabase.instance.gachaCollections[0].gachas[number - 1].unlocked = true;
                            return;
                        }
                    case "Bunny Girls":
                        {
                            SwitchDatabase.instance.gachaCollections[1].gachas[number - 1].unlocked = true;
                            return;
                        }
                    case "Angels & Demons":
                        {
                            SwitchDatabase.instance.gachaCollections[2].gachas[number - 1].unlocked = true;
                            return;
                        }
                    case "Monster Girls":
                        {
                            SwitchDatabase.instance.gachaCollections[3].gachas[number - 1].unlocked = true;
                            return;
                        }
                }
                return;
            }
            else if (name == "Chaos Key Piece")
            {
                var keyItems = SwitchDatabase.instance.keyItems;
                var count = 0;
                foreach (var key in keyItems)
                {
                    if (key.Contains("ChaosKey"))
                    {
                        count += 1;
                    }
                }
                if (count >= 6)
                {
                    return;
                }
                SwitchDatabase.instance.setInt("APChaosKeyCount", count + 1);
                SwitchDatabase.instance.givePlayerItem("ChaosKey" + (count + 1).ToString());
                return;
            }
            else if (name == "Soul Fragment")
            {
                var keyItems = SwitchDatabase.instance.keyItems;
                var count = 0;
                foreach (var key in keyItems)
                {
                    if (key.Contains("SoulFragment"))
                    {
                        count += 1;
                    }
                }
                if (count >= 3)
                {
                    return;
                }
                SwitchDatabase.instance.setInt("APSoulFragmentCount", count + 1);
                var soulName = "SoulFragment" + (count + 1).ToString();
                var soulItem = SwitchDatabase.instance.itemDictionary[soulName];
                CustomGiveItemIfMethodIsOnUpdate(soulItem, soulName);
                return;
            }
            else if (name == "Summon Stone")
            {
                var keyItems = SwitchDatabase.instance.keyItems;
                var count = 0;
                foreach (var key in keyItems)
                {
                    if (key.Contains("SummonStone"))
                    {
                        count += 1;
                    }
                }
                if (count >= 3)
                {
                    return;
                }
                SwitchDatabase.instance.setInt("APSummonStoneCount", count + 1);
                var stoneName = "SummonStone" + (count + 1).ToString();
                var stoneItem = SwitchDatabase.instance.itemDictionary[stoneName];
                CustomGiveItemIfMethodIsOnUpdate(stoneItem, stoneName);
                return;
            }
            else if (name == "Gobliana's Luggage")
            {
                var luggageName = "GoblinModelLuggage";
                var luggageItem = SwitchDatabase.instance.itemDictionary[luggageName];
                CustomGiveItemIfMethodIsOnUpdate(luggageItem, luggageName);
                return;
            }
            if (FlipwitchItems.APItemToGameName.TryGetValue(name, out var trueName))
            {
                SwitchDatabase.instance.givePlayerItem(trueName, skipPopup);
                return;
            }
            Plugin.Logger.LogError($"Item {name} was not caught by any of the cases.  Doing nothing.");

        }

        public static void GiveFlipwitchItem(ReceivedItem item)
        {
            GiveFlipwitchItem(item.Game, item.LocationName, item.ItemName, item.PlayerName, item.Classification);
        }

        public static void GiveFlipwitchItem(string game, string locationName, string itemName, string slotName, ItemFlags classification)
        {
            var isNotReal = FlipwitchItems.SkipPopupItems.Contains(itemName);
            var skipPopup = isNotReal || slotName == ArchipelagoClient.ServerData.SlotName;
            GiveFlipwitchItem(itemName, skipPopup);
        }

        private static void CustomGiveItemIfMethodIsOnUpdate(Item item, string itemName)
        {
            var addItemToInvMethod = SwitchDatabase.instance.GetType().GetMethod("addItemToInv", GenericMethods.Flags);
            switch (item.itemType)
            {
                case ItemTypes.MagicalItem:
                    if (!SwitchDatabase.instance.checkForMagicItem(itemName))
                    {
                        addItemToInvMethod.Invoke(SwitchDatabase.instance, new[] { itemName });
                    }

                    break;
                case ItemTypes.Charms:
                    if (!SwitchDatabase.instance.checkForCharm(itemName))
                    {
                        addItemToInvMethod.Invoke(SwitchDatabase.instance, new[] { itemName });
                    }

                    break;
                case ItemTypes.KeyItem:
                    if (!SwitchDatabase.instance.checkForKeyItem(itemName))
                    {
                        addItemToInvMethod.Invoke(SwitchDatabase.instance, new[] { itemName });
                    }

                    break;
                case ItemTypes.Costumes:
                    if (!SwitchDatabase.instance.checkForCostume(itemName))
                    {
                        addItemToInvMethod.Invoke(SwitchDatabase.instance, new[] { itemName });
                    }

                    break;
                case ItemTypes.PlayerUpgrades:
                    if (!SwitchDatabase.instance.checkForPlayerUpgrade(itemName))
                    {
                        addItemToInvMethod.Invoke(SwitchDatabase.instance, new[] { itemName });
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}