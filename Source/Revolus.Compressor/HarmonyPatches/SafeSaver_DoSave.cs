using HarmonyLib;
using Verse;

namespace Revolus.Compressor.HarmonyPatches;

[HarmonyPatch(typeof(SafeSaver), "DoSave")]
public static class SafeSaver_DoSave
{
    internal static bool Prefix()
    {
        CompressorMod.CurrentlySavingSavegame = true;
        return true; // proceed to original implementation
    }

    internal static void Postfix()
    {
        CompressorMod.CurrentlySavingSavegame = false;
    }
}