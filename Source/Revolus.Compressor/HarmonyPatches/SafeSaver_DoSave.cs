using HarmonyLib;
using Verse;

namespace Revolus.Compressor.HarmonyPatches;

[HarmonyPatch(typeof(SafeSaver), "DoSave")]
public static class SafeSaver_DoSave
{
    internal static void Prefix(string documentElementName)
    {
        CompressorMod.CurrentlySavingSavegame = documentElementName == "savegame";
    }

    internal static void Postfix()
    {
        CompressorMod.CurrentlySavingSavegame = false;
    }
}