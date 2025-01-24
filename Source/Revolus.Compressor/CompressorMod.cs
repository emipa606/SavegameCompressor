using System.Reflection;
using HarmonyLib;
using Mlie;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Revolus.Compressor;

public class CompressorMod : Mod
{
    internal static CompressorSettings Settings;
    internal static bool CurrentlySavingSavegame = false;
    private static string currentVersion;

    public CompressorMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<CompressorSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
        var harmony = new Harmony(typeof(CompressorMod).AssemblyQualifiedName);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        BetterModMismatch_Window.DoPatch(harmony);
    }

    public override void DoSettingsWindowContents(Rect rect)
    {
        bool changed;

        var listing = new Listing_Standard();
        listing.Begin(rect);
        try
        {
            var oldAnchorValue = Text.Anchor;
            try
            {
                changed = Settings.ShowAndChangeSettings(listing);
            }
            finally
            {
                Text.Anchor = oldAnchorValue;
            }

            if (currentVersion != null)
            {
                listing.Gap();
                GUI.contentColor = Color.gray;
                listing.Label("SGC.ModVersion".Translate(currentVersion));
                GUI.contentColor = Color.white;
            }
        }
        finally
        {
            listing.End();
        }

        if (changed)
        {
            SoundDefOf.DragSlider.PlayOneShotOnCamera();
        }

        base.DoSettingsWindowContents(rect);
    }

    public override string SettingsCategory()
    {
        return "Savegame Compressor";
    }
}