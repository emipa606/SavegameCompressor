using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Revolus.Compressor.HarmonyPatches;

[HarmonyPatch(typeof(ScribeMetaHeaderUtility), nameof(ScribeMetaHeaderUtility.WriteMetaHeader))]
public static class ScribeMetaHeaderUtility_WriteMetaHeader
{
    internal static bool Prefix()
    {
        if (!Scribe.EnterNode("meta"))
        {
            return false;
        }

        try
        {
            var gameVersion = VersionControl.CurrentVersionStringWithRev;
            Scribe_Values.Look(ref gameVersion, "gameVersion");

            var modIds = LoadedModManager.RunningMods.Select(mod => mod.PackageId).ToList();
            Scribe_Collections.Look(ref modIds, "modIds");

            var modNames = LoadedModManager.RunningMods.Select(mod => mod.Name).ToList();
            Scribe_Collections.Look(ref modNames, "modNames");

            BetterModMismatch_Window.WriteMetaHeader();
        }
        finally
        {
            Scribe.ExitNode();
        }

        return false;
    }
}