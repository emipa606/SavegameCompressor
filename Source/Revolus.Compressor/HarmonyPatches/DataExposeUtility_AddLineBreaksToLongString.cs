using HarmonyLib;
using Verse;

namespace Revolus.Compressor;

[HarmonyPatch(typeof(DataExposeUtility), nameof(DataExposeUtility.AddLineBreaksToLongString))]
public static class DataExposeUtility_AddLineBreaksToLongString
{
    internal static bool Prefix(ref string __result, string str)
    {
        if (CompressorMod.Settings.pretty)
        {
            return true;
        }

        __result = str;
        return false;
    }
}