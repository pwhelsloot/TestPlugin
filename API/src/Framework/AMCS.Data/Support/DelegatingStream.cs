using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AMCS.Data.Support
{
  /// <summary>
  /// Stream that delegates all calls to a base stream.
  /// </summary>
  /// <remarks>
  /// The use of this class is to customize streams in one way or the other.
  /// All methods here are virtual, and have a default implementation that does
  /// the right thing. If you e.g. want to make a stream that can't be disposed,
  /// you just instantiate a DelegatingStream with owner set to false. If you,
  /// e.g. for unit testing, want a stream that cannot be seeked, you inherit
  /// from this class and just override the CanSeek property to always return false.
  /// </remarks>
  public class DelegatingStream : Stream
  {
    private readonly bool owner;

    public Stream BaseStream { get; }

    public override bool CanRead => BaseStream.CanRead;

    public override bool CanSeek => BaseStream.CanSeek;

    public override bool CanTimeout => BaseStream.CanTimeout;

    public override bool CanWrite => BaseStream.CanWrite;

    public override long Length => BaseStream.Length;

    public override long Position
    {
      get => BaseStream.Position;
      set => BaseStream.Position = value;
    }

    public override int ReadTimeout
    {
      get => BaseStream.ReadTimeout;
      set => BaseStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
      get => BaseStream.WriteTimeout;
      set => BaseStream.WriteTimeout = value;
    }

    public DelegatingStream(Stream baseStream)
      : this(baseStream, true)
    {
    }

    public DelegatingStream(Stream baseStream, bool owner)
    {
      this.owner = owner;
      BaseStream = baseStream;
    }

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
      return BaseStream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    public override void Close()
    {
      BaseStream.Close();
    }

    public override void Flush()
    {
      BaseStream.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
      return BaseStream.FlushAsync(cancellationToken);
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
      return BaseStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      return BaseStream.EndRead(asyncResult);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      return BaseStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
      return BaseStream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
      BaseStream.EndWrite(asyncResult);
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      return BaseStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return BaseStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      BaseStream.SetLength(value);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      return BaseStream.Read(buffer, offset, count);
    }

    public override int ReadByte()
    {
      return BaseStream.ReadByte();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      BaseStream.Write(buffer, offset, count);
    }

    public override void WriteByte(byte value)
    {
      BaseStream.WriteByte(value);
    }

    protected override void Dispose(bool disposing)
    {
      if (owner)
        BaseStream.Dispose();

      base.Dispose(disposing);
    }
  }
}
