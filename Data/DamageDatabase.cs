using System.Collections.Generic;

namespace FlipwitchAP.Data;

public static class DamageDatabase
{
    public static readonly Dictionary<int, int> OriginalOrderDamageLookup = new()
    {
        { 0, 15 },
        { 1, 20 },
        { 2, 25 },
        { 3, 30 },
        { 4, 35 },
        { 5, 40 },
        { 6, 45 },
        { 7, 50 },
    };
    
    public static readonly Dictionary<string, int> AreaToOriginalOrder = new()
    {
        { "Witchy Woods", 0 },
        { "Ghost Castle", 1 },
        { "Jigoku", 2 },
        { "Tengoku", 3 },
        { "Outside Fungal Forest", 4 },
        { "Slime Citadel", 5 },
        { "Spirit City Sewers", 6 },
        { "Umi Umi", 7 },
    };
}