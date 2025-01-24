using System.IO;
using System.IO.Compression;

namespace Revolus.Compressor;

internal class CompressorStream : Stream
{
    private Stream outerStream;
    internal Stream stream;

    public CompressorStream(string filePath)
    {
        var settings = CompressorMod.Settings;

        var fileStream = new FileStream(filePath, FileMode.CreateNew);
        if (settings.level < 0)
        {
            stream = fileStream;
        }
        else
        {
            outerStream = fileStream;
            stream = new GZipStream(
                fileStream,
                settings.level > 0 ? CompressionLevel.Optimal : CompressionLevel.Fastest
            );
        }
    }

    public override bool CanRead { get; }
    public override bool CanSeek { get; }
    public override bool CanWrite { get; }
    public override long Length { get; }
    public override long Position { get; set; }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return 0;
    }

    public override void SetLength(long value)
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return 0;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
    }

    public override void Flush()
    {
        try
        {
            stream?.Flush();
        }
        finally
        {
            outerStream?.Flush();
        }
    }

    public override void Close()
    {
        var outer = outerStream;
        var inner = stream;

        outerStream = null;
        stream = null;

        try
        {
            inner?.Close();
        }
        finally
        {
            outer?.Close();
        }
    }
}