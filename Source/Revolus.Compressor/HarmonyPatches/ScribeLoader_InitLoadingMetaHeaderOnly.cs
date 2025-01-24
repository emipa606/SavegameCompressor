using System;
using System.Xml;
using HarmonyLib;
using Verse;

namespace Revolus.Compressor;

[HarmonyPatch(typeof(ScribeLoader), nameof(ScribeLoader.InitLoadingMetaHeaderOnly))]
public static class ScribeLoader_InitLoadingMetaHeaderOnly
{
    internal static bool Prefix(ref ScribeLoader __instance, string filePath)
    {
        var instance = __instance;

        if (Scribe.mode != LoadSaveMode.Inactive)
        {
            Log.Error($"Called InitLoadingMetaHeaderOnly() but current mode is {Scribe.mode}");
            Scribe.ForceStop();
        }

        try
        {
            Utils.WithUncompressedXmlTextReader<object>(filePath, xml =>
            {
                if (!ScribeMetaHeaderUtility.ReadToMetaElement(xml))
                {
                    return null;
                }

                using var reader = xml.ReadSubtree();
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(reader);

                var xmlElement = xmlDocument.CreateElement("root");
                xmlElement.AppendChild(xmlDocument.DocumentElement);

                instance.curXmlParent = xmlElement;

                return null;
            });
            Scribe.mode = LoadSaveMode.LoadingVars;
        }
        catch (Exception ex)
        {
            Log.Error($"Exception while init loading meta header: {filePath}\n{ex}");
            __instance.ForceStop();
            throw;
        }

        return false;
    }
}