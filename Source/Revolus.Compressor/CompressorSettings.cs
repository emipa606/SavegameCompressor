using System;
using UnityEngine;
using Verse;

namespace Revolus.Compressor;

internal class CompressorSettings : ModSettings
{
    internal static readonly bool prettyDefault = true;

    internal static readonly int levelDefault = 0;
    internal int level = levelDefault;
    internal bool pretty = prettyDefault;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref pretty, "pretty", prettyDefault);
        Scribe_Values.Look(ref level, "level", levelDefault);
        base.ExposeData();
    }

    internal bool ShowAndChangeSettings(Listing_Standard listing)
    {
        int levelNew, levelOld = Math.Max(-1, Math.Min(level, +1));
        bool prettyNew, prettyOld = pretty;

        {
            var wholeRect = listing.GetRect(Text.LineHeight);
            var labelRect = wholeRect.LeftHalf().Rounded();
            var dataRect = wholeRect.RightHalf().Rounded();
            var dataSelectRect = dataRect.LeftHalf().Rounded();
            var dataDescRect = dataRect.RightHalf().Rounded();

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(labelRect, "SGC.Compressionlevel".Translate());

            Text.Anchor = TextAnchor.MiddleCenter;
            levelNew = Mathf.RoundToInt(Widgets.HorizontalSlider(dataSelectRect, levelOld, -1, +1));

            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(
                dataDescRect,
                levelNew < 0 ? "SGC.Uncompressed".Translate() :
                levelNew > 0 ? "SGC.BestCompression".Translate() : "SGC.FastestCompression".Translate()
            );

            listing.Gap(listing.verticalSpacing + Text.LineHeight);
        }

        {
            var wholeRect = listing.GetRect(Text.LineHeight);
            var labelRect = wholeRect.LeftHalf().Rounded();
            var valueRect = wholeRect.RightHalf().Rounded();

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(labelRect, "SGC.PrettyPrint".Translate());

            Text.Anchor = TextAnchor.MiddleLeft;
            prettyNew = prettyOld;
            Widgets.CheckboxLabeled(valueRect, "", ref prettyNew, placeCheckboxNearText: true);

            listing.Gap(listing.verticalSpacing + Text.LineHeight);
        }

        var changed = false;
        if (prettyNew != prettyOld)
        {
            pretty = prettyNew;
            changed = true;
        }

        if (levelNew == levelOld)
        {
            return changed;
        }

        level = levelNew;

        return true;
    }
}