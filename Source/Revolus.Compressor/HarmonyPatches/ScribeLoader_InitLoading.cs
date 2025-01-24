using System;
using System.Xml;
using HarmonyLib;
using Verse;

namespace Revolus.Compressor;

[HarmonyPatch(typeof(ScribeLoader), nameof(ScribeLoader.InitLoading))]
public static class ScribeLoader_InitLoading
{
    internal static bool Prefix(ref ScribeLoader __instance, string filePath)
    {
        var instance = __instance;

        if (Scribe.mode != 0)
        {
            Log.Error($"Called InitLoading() but current mode is {Scribe.mode}");
            Scribe.ForceStop();
        }

        if (instance.curParent != null)
        {
            Log.Error("Current parent is not null in InitLoading");
            instance.curParent = null;
        }

        if (instance.curPathRelToParent != null)
        {
            Log.Error("Current path relative to parent is not null in InitLoading");
            instance.curPathRelToParent = null;
        }

        try
        {
            instance.curXmlParent = Utils.WithUncompressedXmlTextReader(filePath, reader =>
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(reader);
                return xmlDocument.DocumentElement;
            });
            Scribe.mode = LoadSaveMode.LoadingVars;
        }
        catch (Exception ex)
        {
            Log.Error($"Exception while init loading file: {filePath}\n{ex}");
            instance.ForceStop();
            throw;
        }

        return false;
    }
}