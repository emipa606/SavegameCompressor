using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using HarmonyLib;
using Verse;

namespace Revolus.Compressor;

internal class BetterModMismatch_Window
{
    private static List<(string name, string version)> cachedModList;

    private static Assembly BetterModMismatchWindow;

    internal static void DoPatch(Harmony harmony)
    {
        BetterModMismatchWindow = (
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            where assembly.GetName().Name == "ModMisMatchWindowPatch"
            select assembly
        ).FirstOrDefault();
        if (BetterModMismatchWindow is null)
        {
            return;
        }

        Log.Message("Mod 'Better ModMismatch Window' was found -> patching");

        if (!Utils.GetType(BetterModMismatchWindow, "Madeline.ModMismatchFormatter.ModContentPackExtension",
                out var modContentPackExtension))
        {
            Log.Error("!Madeline.ModMismatchFormatter.ModContentPackExtension");
        }
        else if (!Utils.GetMethod(modContentPackExtension, "ReadModsFromSaveHeader", true,
                     out var readModsFromSaveHeader))
        {
            Log.Error("!Madeline.ModMismatchFormatter.MetaHeaderUtility ReadModsFromSaveHeader");
        }
        else if (!Utils.GetType(BetterModMismatchWindow, "Madeline.ModMismatchFormatter.MetaHeaderUtility",
                     out var metaHeaderUtility))
        {
            Log.Error("!Madeline.ModMismatchFormatter.MetaHeaderUtility");
        }
        else if (!Utils.GetMethod(metaHeaderUtility, "UpdateModVersionMetaHeader", true,
                     out var updateModVersionMetaHeader))
        {
            Log.Error("!Madeline.ModMismatchFormatter.MetaHeaderUtility UpdateModVersionMetaHeader");
        }
        else
        {
            harmony.Patch(
                readModsFromSaveHeader,
                new HarmonyMethod(typeof(BetterModMismatch_Window), nameof(ReadModsFromSaveHeader__Prefix))
            );
            harmony.Patch(
                updateModVersionMetaHeader,
                new HarmonyMethod(typeof(BetterModMismatch_Window), nameof(UpdateModVersionMetaHeader__Prefix))
            );
        }
    }

    private static List<(string name, string version)> getModsList()
    {
        var result = cachedModList;
        if (result != null)
        {
            return result;
        }

        result = [];
        cachedModList = result;

        try
        {
            Utils.GetType(BetterModMismatchWindow, "Madeline.ModMismatchFormatter.ModContentPackExtension",
                out var modContentPackExtension);
            Utils.GetMethod(modContentPackExtension, "GetMetaData", true, out var GetMetaData);

            Utils.GetType(BetterModMismatchWindow, "Madeline.ModMismatchFormatter.MetaHeaderUtility",
                out var metaHeaderUtility);
            Utils.GetMethod(metaHeaderUtility, "GetVersionFromManifestFile", true,
                out var getVersionFromManifestFile);

            foreach (var modContentPack in LoadedModManager.RunningMods)
            {
                var metaData = (ModMetaData)GetMetaData.Invoke(null, [modContentPack]);
                var version = (string)getVersionFromManifestFile.Invoke(null, [modContentPack]);
                result.Add((metaData.Name, version ?? "Unknown"));
            }
        }
        catch (Exception ex)
        {
            Log.Warning($"Cannot get modlist: {ex}");
        }

        return result;
    }

    internal static void WriteMetaHeader()
    {
        if (BetterModMismatchWindow is null)
        {
            return;
        }

        var mods = getModsList();
        if (mods.Count <= 0 || !Scribe.EnterNode("modMetaDatas"))
        {
            return;
        }

        try
        {
            foreach (var data in mods)
            {
                if (!Scribe.EnterNode("li"))
                {
                    continue;
                }

                try
                {
                    var (name, version) = data;
                    Scribe_Values.Look(ref name, "ModName");
                    Scribe_Values.Look(ref version, "version");
                }
                finally
                {
                    Scribe.ExitNode();
                }
            }
        }
        finally
        {
            Scribe.ExitNode();
        }
    }

    private static Dictionary<string, string> readModMetaData(string filePath)
    {
        return Utils.WithUncompressedXmlTextReader(filePath, xml =>
        {
            var result = new Dictionary<string, string>();
            var modMetaDatas = XDocument.Load(xml).Element("savegame")?.Element("meta")?.Element("modMetaDatas");
            if (modMetaDatas is null)
            {
                return result;
            }

            foreach (var modMetaData in modMetaDatas.Elements("li"))
            {
                var name = modMetaData.Element("ModName")?.Value;
                if (name is not null)
                {
                    result.Add(name, modMetaData.Element("version")?.Value);
                }
            }

            return result;
        });
    }

    public static bool ReadModsFromSaveHeader__Prefix(ref object __result, bool readModVersion)
    {
        if (
            !Utils.GetType(BetterModMismatchWindow, "Madeline.ModMismatchFormatter.MetaHeaderUtility",
                out var MetaHeaderUtility) ||
            !Utils.GetProp(MetaHeaderUtility, "LastAccessedSaveFilePathInLoadSelection", true,
                out var LastAccessedSaveFilePathInLoadSelection)
        )
        {
            Log.Error("'MetaHeaderUtility' not found or incompatible");
            return true;
        }

        if (
            !Utils.GetType(BetterModMismatchWindow, "Madeline.ModMismatchFormatter.Mod", out var Mod) ||
            !Utils.GetProp(Mod, "Version", false, out var ModVersion)
        )
        {
            Log.Error("'Mod' not found or incompatible");
            return true;
        }

        Dictionary<string, string> modsInSelection = null;
        if (readModVersion)
        {
            modsInSelection = readModMetaData((string)LastAccessedSaveFilePathInLoadSelection.GetValue(null));
        }

        var saveMods = new List<object>();
        var loadedModNamesList = ScribeMetaHeaderUtility.loadedModNamesList;
        var loadedModIdsList = ScribeMetaHeaderUtility.loadedModIdsList;
        for (int index = 0, count = loadedModNamesList.Count; index < count; ++index)
        {
            var id = loadedModIdsList[index];
            var name = loadedModNamesList[index];
            var mod = Activator.CreateInstance(Mod, id, name, index);
            if (readModVersion)
            {
                ModVersion.SetValue(mod, modsInSelection.TryGetValue(name) ?? "Unknown");
            }

            saveMods.Add(mod);
        }

        __result = saveMods;

        return false;
    }

    public static bool UpdateModVersionMetaHeader__Prefix()
    {
        return false;
    }
}