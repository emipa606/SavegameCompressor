using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using HarmonyLib;
using Verse;

namespace Revolus.Compressor;

[HarmonyPatch(typeof(ScribeSaver), nameof(ScribeSaver.InitSaving))]
public static class ScribeSaver_InitSaving
{
    public static bool Prefix(ref ScribeSaver __instance, string filePath,
        string documentElementName)
    {
        if (!CompressorMod.CurrentlySavingSavegame)
        {
            return true; // use original implementation for everything but save games
        }

        var instance = __instance;

        if (!GotField("writer", out var writerField) || !GotField("saveStream", out var saveStreamField))
        {
            Log.Error("Verse.ScribeSaver is incompatible. Not Compressing.");
            return true;
        }

        if (Scribe.mode != LoadSaveMode.Inactive)
        {
            Log.Error($"Called InitSaving() but current mode is {Scribe.mode}");
            Scribe.ForceStop();
        }

        if (GotField("curPath", out var curPathField) && curPathField.GetValue(instance) != null)
        {
            Log.Error("Current path is not null in InitSaving");

            curPathField.SetValue(instance, null);

            if (GotField("savedNodes", out var savedNodesField))
            {
                ((HashSet<string>)savedNodesField.GetValue(instance)).Clear();
            }

            if (GotField("nextListElementTemporaryId", out var nextListElementTemporaryIdField))
            {
                nextListElementTemporaryIdField.SetValue(instance, 0);
            }
        }

        var compressedStream = new CompressorStream(filePath);
        saveStreamField.SetValue(instance, compressedStream);

        try
        {
            Scribe.mode = LoadSaveMode.Saving;

            var settings = CompressorMod.Settings;

            var xmlWriterSettings = new XmlWriterSettings();
            if (settings.pretty)
            {
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.IndentChars = "\t";
            }

            var xmlWriter = XmlWriter.Create(compressedStream.stream, xmlWriterSettings);
            writerField.SetValue(instance, xmlWriter);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement(documentElementName);
        }
        catch (Exception ex)
        {
            Log.Error($"Exception while init saving file: {filePath}\n{ex}");
            instance.ForceStop();
            throw;
        }

        return false; // don't call original implementation

        bool GotField(string name, out FieldInfo result)
        {
            var field = typeof(ScribeSaver).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            result = field;
            return field is not null;
        }
    }
}