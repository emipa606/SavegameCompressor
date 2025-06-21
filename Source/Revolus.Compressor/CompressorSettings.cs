using System;
using UnityEngine;
using Verse;

namespace Revolus.Compressor;

internal class CompressorSettings : ModSettings
{
    private const bool prettyDefault = true;

    private const int levelDefault = 0;
    internal int Level = levelDefault;
    internal bool Pretty = prettyDefault;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref Pretty, "pretty", prettyDefault);
        Scribe_Values.Look(ref Level, "level");
        base.ExposeData();
    }

    internal bool ShowAndChangeSettings(Listing_Standard listing)
    {
        int levelNew, levelOld = Math.Max(-1, Math.Min(Level, +1));
        bool prettyNew, prettyOld = Pretty;

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
            Pretty = prettyNew;
            changed = true;
        }

        if (levelNew == levelOld)
        {
            return changed;
        }

        Level = levelNew;

        return true;
    }
}