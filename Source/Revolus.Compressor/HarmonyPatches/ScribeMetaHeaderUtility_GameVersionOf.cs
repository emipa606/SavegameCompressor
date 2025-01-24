using System;
using System.IO;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Revolus.Compressor.HarmonyPatches;

[HarmonyPatch(typeof(ScribeMetaHeaderUtility), nameof(ScribeMetaHeaderUtility.GameVersionOf))]
public static class ScribeMetaHeaderUtility_GameVersionOf
{
    internal static bool Prefix(ref string __result, FileInfo file)
    {
        string result = null;
        try
        {
            result = Utils.WithUncompressedXmlTextReader(file.FullName, reader =>
                ScribeMetaHeaderUtility.ReadToMetaElement(reader) && reader.ReadToDescendant("gameVersion")
                    ? VersionControl.VersionStringWithoutRev(reader.ReadString())
                    : null);
        }
        catch (Exception ex)
        {
            Log.Error($"Exception getting game version of {file.Name}: {ex}");
        }

        __result = result;
        return false; // don't call original implementation
    }
}