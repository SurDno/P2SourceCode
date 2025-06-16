// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.RemoteFileWriteStream
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;
using System.IO;

#nullable disable
namespace Facepunch.Steamworks
{
  public class RemoteFileWriteStream : Stream
  {
    internal readonly RemoteStorage remoteStorage;
    private readonly UGCFileWriteStreamHandle_t _handle;
    private readonly RemoteFile _file;
    private int _written;
    private bool _closed;

    internal RemoteFileWriteStream(RemoteStorage r, RemoteFile file)
    {
      this.remoteStorage = r;
      this._handle = this.remoteStorage.native.FileWriteStreamOpen(file.FileName);
      this._file = file;
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value) => throw new NotImplementedException();

    public override unsafe void Write(byte[] buffer, int offset, int count)
    {
      if (this._closed)
        throw new ObjectDisposedException(this.ToString());
      fixed (byte* numPtr = buffer)
      {
        if (this.remoteStorage.native.FileWriteStreamWriteChunk(this._handle, (IntPtr) (void*) (numPtr + offset), count))
          this._written += count;
      }
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => (long) this._written;

    public override long Position
    {
      get => (long) this._written;
      set => throw new NotImplementedException();
    }

    public void Cancel()
    {
      if (this._closed)
        return;
      this._closed = true;
      this.remoteStorage.native.FileWriteStreamCancel(this._handle);
    }

    public override void Close()
    {
      if (this._closed)
        return;
      this._closed = true;
      this.remoteStorage.native.FileWriteStreamClose(this._handle);
      this._file.remoteStorage.OnWrittenNewFile(this._file);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.Close();
      base.Dispose(disposing);
    }
  }
}
