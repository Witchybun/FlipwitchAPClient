using HarmonyLib;

namespace FlipwitchAP;

public class InfoCatcher
{
    [HarmonyPatch(typeof(DestructableObject), "spawnItem")]
    [HarmonyPostfix]
    private static void SpawnItem_WhatsIsThisAndWhere(DestructableObject __instance)
    {
        var position = SwitchDatabase.instance.currentLevel.name;
        var objectName = __instance.gameObject.name;
        Plugin.Logger.LogInfo($"Pot name: {objectName}, Level: {position}");
        
    }
    
}