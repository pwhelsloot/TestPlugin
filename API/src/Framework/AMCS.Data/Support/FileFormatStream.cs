namespace AMCS.Data.Support
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.IO.Compression;
  using System.Linq;

  public class FileFormatStream : Stream
  {
    private readonly Stream innerStream;
    private readonly List<byte> headerBytes = new List<byte>();
    public FileFormat FileFormat { get; }

    public FileFormatStream(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof(stream));
      if (stream.Position != 0)
        throw new InvalidOperationException("Stream must be at position 0");

      innerStream = stream;
      ReadHeader();

      if(IsGzip())
      {
        stream.Seek(0, SeekOrigin.Begin);
        headerBytes.Clear();
        innerStream = new GZipStream(stream, CompressionMode.Decompress);
        ReadHeader();
      }

      FileFormat = FileFormatHelper.GetFileFormat(headerBytes.ToArray());
    }

    private void ReadHeader()
    {
      var tempBytes = new byte[FileFormatHelper.MaxHeaderSize];
      int actualHeaderSize = Read(tempBytes, 0, FileFormatHelper.MaxHeaderSize);
      headerBytes.AddRange(tempBytes.ToList().Take(actualHeaderSize));

    }

    private bool IsGzip()
    {
      return
        headerBytes.Count > 2 &&
        headerBytes[0] == 0x1f &&
        headerBytes[1] == 0x8b &&
        headerBytes[2] == 0x08;
    }

    public override bool CanRead => innerStream.CanRead;

    public override bool CanSeek => innerStream.CanSeek;

    public override bool CanWrite => innerStream.CanWrite;

    public override long Length => innerStream.Length;

    public override long Position { get => innerStream.Position; set => innerStream.Position = value; }

    public override void Flush()
    {
      throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (headerBytes.Count > 0)
      {
        List<byte> responseBytes = new List<byte>();
        responseBytes.AddRange(headerBytes.Take(count));
        headerBytes.RemoveRange(0, responseBytes.Count);
        if (responseBytes.Count < count)
        {
          var uncachedBytes = new byte[count - responseBytes.Count];
          int uncachedReadCount = innerStream.Read(uncachedBytes, 0, uncachedBytes.Length);
          responseBytes.AddRange(uncachedBytes.Take(uncachedReadCount));
        }

        for (int i = 0; i < responseBytes.Count; i++)
        {
          buffer[i] = responseBytes[i];
        }
        return responseBytes.Count;
      }
      return innerStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }
  }
}
