using System;
using HarmonyLib;
using Verse;

namespace Revolus.Compressor;

[HarmonyPatch(typeof(DataExposeUtility), nameof(DataExposeUtility.LookByteArray))]
public static class DataExposeUtility_LookByteArray
{
    internal static bool Prefix(ref byte[] arr, string label)
    {
        if (Scribe.mode != LoadSaveMode.Saving)
        {
            return true; // loading
        }

        if (arr is null)
        {
            return false; // nothing to do
        }

        if (CompressorMod.Settings.level < 0)
        {
            return true; // use default
        }

        var b64 = Convert.ToBase64String(arr);
        b64 = b64.AddLineBreaksToLongString();
        Scribe_Values.Look(ref b64, label);
        return false; // don't call original implementation
    }
}