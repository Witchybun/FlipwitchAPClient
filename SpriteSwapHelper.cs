using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Archipelago.MultiClient.Net.Enums;
using FlipwitchAP.Archipelago;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace FlipwitchAP
{
    public class SpriteSwapHelper
    {
        public class SpriteData
        {
            public readonly string Description;
            public readonly Sprite Sprite;
            public SpriteData(string description, Sprite sprite)
            {
                Description = description;
                Sprite = sprite;
            }
        }

        public class ItemSpriteAlias
        {
            public string AliasName { get; set; } = "";

            public List<string> ItemNames { get; set; } = new List<string>();

            public ItemSpriteAlias()
            {
            }
        }

        public class ItemSpriteAliases
        {
            public List<ItemSpriteAlias> Aliases { get; set; } = new List<ItemSpriteAlias>();

            public ItemSpriteAliases()
            {
            }
        }

        private class itemDescription
        {
            public string Name;
            public string Description;
        }

        private static readonly Dictionary<string, Dictionary<string, SpriteData>> GameToSpriteData = new() { };

        [HarmonyPatch(typeof(ItemCollectPopup), "popUpItem")]
        [HarmonyPrefix]
        private static void popUpItem_RenderCustomItemIfSpriteExists(ref string itemNameId, ref string itemDescId, string howToUseId, RuntimeAnimatorController animator, Action onPopupCloseCallback = null)
        {
            var itemPickupObject = SwitchDatabase.instance.transform.Find("Main UI").Find("Item Pickup Object");
            if (!itemNameId.Contains("###"))
            {
                itemPickupObject.Find("Item Pickup Image").gameObject.SetActive(true);
                var apPicture = itemPickupObject.Find("ArchipelagoItemSprite");
                if (apPicture is not null)
                {
                    apPicture.gameObject.SetActive(false);
                }
                return;
            }
            var nameArray = itemNameId.Split("###");
            var game = nameArray[0];
            if (game.Contains("Manual") && game.Contains("_"))
            {
                game = game.Split("_")[1];
            }
            var gameLookup = CleanString(game);
            var item = nameArray[1];
            var lookup = CleanString(item);
            int.TryParse(nameArray[2], out var locationID);
            var player = nameArray[3];
            itemNameId = $"{player}'s {item}";
            Plugin.Logger.LogInfo($"Info: Item Name ID: {itemNameId}, Game: {game}, After Cleanup: {gameLookup}, Item: {item}, After Cleanup: {lookup}");
            if (!DoesSpriteExist(gameLookup, lookup, out var lookupName))
            {
                if (lookupName == "ArchipelagoGeneric")
                {
                    var type = ReturnClassificationEnd(locationID);
                    RenderArchipelagoSprite("Archipelago", type, itemPickupObject);
                    itemDescId = $"An item from the world of {game}.";
                    itemPickupObject.Find("Item Pickup Image").gameObject.SetActive(false);
                    return;
                }

                if (lookupName == "Generic")
                {
                    RenderArchipelagoSprite(game, lookupName, itemPickupObject);
                    itemDescId = $"An item from the world of {game}.";
                    itemPickupObject.Find("Item Pickup Image").gameObject.SetActive(false);
                }
            }

            if (GameToSpriteData[gameLookup].TryGetValue(lookup, out var spriteInfo))
            {
                itemDescId = spriteInfo.Description;
                RenderArchipelagoSprite(game, lookup, itemPickupObject);
                itemPickupObject.Find("Item Pickup Image").gameObject.SetActive(false);
                
            }
            else
            {
                RenderArchipelagoSprite(game, "Generic", itemPickupObject);
                itemDescId = $"An item from the world of {game}.";
                itemPickupObject.Find("Item Pickup Image").gameObject.SetActive(false);
                
            }
        }

        private static string CleanString(string str)
        {
            var sb = new StringBuilder();
            foreach (var t in str)
            {
                if ((t is >= '0' and <= '9' or >= 'A' and <= 'z' or ' '))
                {
                    sb.Append(t);
                }
            }
            return sb.ToString();
        }

        public static void GenerateData()
        {
            var plugins = BepInEx.Paths.PluginPath;
            var dir = Path.Combine(Path.Combine(plugins, "FlipwitchAP"), "Custom Assets");
            if (!Directory.Exists(dir))
            {
                return;
            }
            var allFolders = Directory.GetDirectories(dir);
            var archipelagoFolders = allFolders.Where(x => Path.GetFileName(x.TrimEnd(Path.DirectorySeparatorChar)) == "Archipelago");
            if (!archipelagoFolders.Any())
            {
                return;
            }
            MakeGenericArchipelagoItemsFirst(archipelagoFolders.First());
            var folders = allFolders.Where(x => Path.GetFileName(x.TrimEnd(Path.DirectorySeparatorChar)) != "Archipelago");
            foreach (var folder in folders)
            {
                var gameName = Path.GetFileName(folder.TrimEnd(Path.DirectorySeparatorChar));
                var aliasPath = Path.Combine(folder, "aliases.json");
                var descriptionPath = Path.Combine(folder, "descriptions.json");
                var aliasGroup = new ItemSpriteAliases();
                var aliases = new Dictionary<string, List<string>>();
                var descriptionList = new List<itemDescription>();
                var descriptions = new Dictionary<string, string>();
                if (File.Exists(aliasPath))
                {
                    var aliasText = File.ReadAllText(aliasPath);
                    aliasGroup = JsonConvert.DeserializeObject<ItemSpriteAliases>(aliasText);
                    foreach (var alias in aliasGroup.Aliases)
                    {
                        aliases[alias.AliasName] = alias.ItemNames;
                    }
                }
                if (File.Exists(descriptionPath))
                {
                    var descriptionText = File.ReadAllText(descriptionPath);
                    descriptionList = JsonConvert.DeserializeObject<List<itemDescription>>(descriptionText);
                    descriptions = CreateLookupForDescription(descriptionList, aliases);
                }
                var pictures = Directory.GetFiles(folder).Select(Path.GetFileName);
                var cleanGameName = CleanString(gameName);
                GameToSpriteData[cleanGameName] = new();
                foreach (var picture in pictures)
                {
                    if (Path.GetExtension(picture).ToUpperInvariant() != ".PNG")
                    {
                        continue;
                    }
                    var picturePath = Path.Combine(folder, picture);
                    if (!picture.Contains("_"))
                    {
                        TryMakeGenericIcon(cleanGameName, picturePath);
                        continue;
                    }
                    var nameArray = Path.GetFileNameWithoutExtension(picture).Split("_");
                    var itemGame = nameArray[0];
                    var gameLookup = CleanString(itemGame);
                    var itemName = nameArray[1];
                    var lookup = CleanString(itemName);
                    if (itemGame != gameName)
                    {
                        continue;
                    }

                    var bytes = File.ReadAllBytes(picturePath);
                    var texture = new Texture2D(2, 2);
                    texture.LoadImage(bytes);
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
                    var itemDescription = $"An item from the world of {gameName}.";
                    var newSpriteData = new SpriteData("", new Sprite());
                    if (aliases.TryGetValue(itemName, out var alias))
                    {
                        foreach (var item in alias)
                        {
                            descriptions.TryGetValue(itemName, out itemDescription);
                            descriptions.TryGetValue(item, out itemDescription);
                            newSpriteData = new SpriteData(itemDescription, sprite);
                            lookup = CleanString(item);
                            GameToSpriteData[gameLookup][lookup] = newSpriteData;
                        }
                        continue;
                    }

                    if (!descriptions.TryGetValue(itemName, out itemDescription) && descriptions.TryGetValue("Generic", out var description))
                    {
                        itemDescription = description;
                    }
                    newSpriteData = new SpriteData(itemDescription, sprite);
                    GameToSpriteData[gameLookup][lookup] = newSpriteData;
                }
            }
        }

        private static Dictionary<string, string> CreateLookupForDescription(List<itemDescription> itemDescriptions, Dictionary<string, List<string>> aliases)
        {
            var result = new Dictionary<string, string>();
            foreach (var item in itemDescriptions)
            {
                if (aliases.TryGetValue(item.Name, out var alias))
                {
                    foreach (var name in alias)
                    {
                        result[name] = item.Description;
                    }
                    continue;
                }
                result[item.Name] = item.Description;
            }
            return result;
        }

        private static void MakeGenericArchipelagoItemsFirst(string path)
        {
            var pictures = Directory.GetFiles(path).Select(Path.GetFileName);
            GameToSpriteData["Archipelago"] = new();
            foreach (var picture in pictures)
            {
                if (Path.GetExtension(picture).ToUpperInvariant() != ".PNG")
                {
                    continue;
                }
                var picturePath = Path.Combine(path, picture);
                var nameArray = Path.GetFileNameWithoutExtension(picture).Split("_");
                var itemName = nameArray[1];

                var bytes = File.ReadAllBytes(picturePath);
                var texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
                var itemDescription = $"An item from an Archipelago.";
                var newSpriteData = new SpriteData(itemDescription, sprite);
                newSpriteData = new SpriteData(itemDescription, sprite);
                GameToSpriteData["Archipelago"][itemName] = newSpriteData;
            }
        }

        private static void TryMakeGenericIcon(string gameName, string picturePath)
        {
            if (gameName == "Archipelago")
            {
                GameToSpriteData[gameName]["Generic"] = GameToSpriteData["Archipelago"]["UsefulItem"];
                return;
            }
            var bytes = File.ReadAllBytes(picturePath);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
            var itemDescription = $"An item from the world of {gameName}.";
            var newSpriteData = new SpriteData(itemDescription, sprite);
            GameToSpriteData[gameName]["Generic"] = newSpriteData;
        }

        private static bool DoesSpriteExist(string gameName, string itemName, out string lookupName)
        {
            if (!GameToSpriteData.TryGetValue(gameName, out var gameInfo))
            {
                lookupName = "ArchipelagoGeneric";
                return false;
            }

            if (!gameInfo.ContainsKey(itemName))
            {
                lookupName = "Generic";
                return false;
            }
            lookupName = itemName;
            return true;
        }

        private static string ReturnClassificationEnd(long locationID)
        {
            if (!ArchipelagoClient.ServerData.ScoutedLocations.TryGetValue(locationID, out var locationData))
            {
                return "UsefulItem";
            }
            if (locationData.Classification.HasFlag(ItemFlags.Trap))
            {
                return "TrapItem";
            }
            else if (locationData.Classification.HasFlag(ItemFlags.Advancement))
            {
                return "ProgressionItem";
            }
            else if (locationData.Classification.HasFlag(ItemFlags.NeverExclude))
            {
                return "UsefulItem";
            }
            return "FillerItem";
        }

        private static SpriteData GetSpriteForItem(string gameName, string itemName)
        {
            var gameLookup = CleanString(gameName);
            if (!GameToSpriteData.ContainsKey(gameLookup))
            {
                Plugin.Logger.LogWarning($"No game for {gameLookup}, returning Archipelago");
                return GameToSpriteData["Archipelago"]["UsefulItem"];
            }
            var gameInfo = GameToSpriteData[gameLookup];
            return !gameInfo.TryGetValue(itemName, out var item) ? gameInfo["Generic"] : item;
        }

        private static void RenderArchipelagoSprite(string gameName, string itemName, Transform parent)
        {
            var spriteData = GetSpriteForItem(gameName, itemName);
            var sprite = spriteData.Sprite;
            var possibleArchipelagoObject = parent.Find("ArchipelagoItemSprite");
            if (possibleArchipelagoObject is null)
            {
                var archipelagoObject = new GameObject();
                archipelagoObject.name = "ArchipelagoItemSprite";
                archipelagoObject.transform.SetParent(parent);
                var rect = archipelagoObject.AddComponent<RectTransform>();
                var image = archipelagoObject.AddComponent<Image>();
                rect.anchoredPosition = new Vector2(0f, 71f);
                rect.position = new Vector3(800, 509.1667f, 0);
                rect.localPosition = new Vector3(0, 71, 0);
                var worstScale = MathF.Min(32f / sprite.texture.width, 32f / sprite.texture.height);
                rect.localScale = new Vector3(worstScale, worstScale, worstScale);
                image.sprite = sprite;
                archipelagoObject.SetActive(true);
            }
            else
            {
                var archipelagoObject = possibleArchipelagoObject.gameObject;
                var rect = possibleArchipelagoObject.GetComponent<RectTransform>();
                var image = possibleArchipelagoObject.GetComponent<Image>();
                rect.anchoredPosition = new Vector2(0f, 71f);
                rect.position = new Vector3(800, 509.1667f, 0);
                rect.localPosition = new Vector3(0, 71, 0);
                var worstScale = MathF.Min(32f / sprite.texture.width, 32f / sprite.texture.height);
                rect.localScale = new Vector3(worstScale, worstScale, worstScale);
                image.sprite = sprite;
                archipelagoObject.SetActive(true);
            }
        }
    }
}