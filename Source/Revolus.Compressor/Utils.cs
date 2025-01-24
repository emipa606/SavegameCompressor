using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml;

namespace Revolus.Compressor;

internal class Utils
{
    internal static T WithUncompressedXmlTextReader<T>(string filePath, Func<XmlTextReader, T> action)
    {
        using var maybeCompressedStream = new FileStream(filePath, FileMode.Open);
        var headerBuffer = new byte[3];
        if (maybeCompressedStream.Read(headerBuffer, 0, 3) < 3)
        {
            throw new Exception($"The input file is truncated or empty: {filePath}");
        }

        maybeCompressedStream.Seek(0, SeekOrigin.Begin);

        var isCompressed = headerBuffer[0] == 0x1f && headerBuffer[1] == 0x8b && headerBuffer[2] == 0x08;
        if (!isCompressed)
        {
            return DoAction(maybeCompressedStream);
        }

        using var uncompressedStream = new GZipStream(maybeCompressedStream, CompressionMode.Decompress);
        return DoAction(uncompressedStream);

        T DoAction(Stream uncompressedBytesStream)
        {
            using var textReader = new StreamReader(uncompressedBytesStream);
            using var xmlTextReader = new XmlTextReader(textReader);
            return action(xmlTextReader);
        }
    }

    internal static bool GetType(Assembly assembly, string name, out Type type)
    {
        var val = assembly.GetType(name, false);
        type = val;
        return val is not null;
    }

    internal static bool GetMethod(Type type, string name, bool staticOnly, out MethodInfo info)
    {
        var bindingAttr = BindingFlags.Public | BindingFlags.NonPublic |
                          (staticOnly ? BindingFlags.Static : BindingFlags.Instance);
        var val = type.GetMethod(name, bindingAttr);
        info = val;
        return val is not null;
    }

    internal static bool GetProp(Type type, string name, bool staticOnly, out PropertyInfo info)
    {
        var bindingAttr = BindingFlags.Public | BindingFlags.NonPublic |
                          (staticOnly ? BindingFlags.Static : BindingFlags.Instance);
        var val = type.GetProperty(name, bindingAttr);
        info = val;
        return val is not null;
    }
}