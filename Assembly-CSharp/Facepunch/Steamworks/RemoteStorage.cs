using SteamNative;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Facepunch.Steamworks
{
  public class RemoteStorage : IDisposable
  {
    internal Client client;
    internal SteamRemoteStorage native;
    private bool _filesInvalid = true;
    private readonly List<RemoteFile> _files = new List<RemoteFile>();

    private static string NormalizePath(string path)
    {
      return Platform.IsWindows ? new FileInfo("x:/" + path).FullName.Substring(3) : new FileInfo("/x/" + path).FullName.Substring(3);
    }

    internal RemoteStorage(Client c)
    {
      this.client = c;
      this.native = this.client.native.remoteStorage;
    }

    public bool IsCloudEnabledForAccount => this.native.IsCloudEnabledForAccount();

    public bool IsCloudEnabledForApp => this.native.IsCloudEnabledForApp();

    public int FileCount => this.native.GetFileCount();

    public IEnumerable<RemoteFile> Files
    {
      get
      {
        this.UpdateFiles();
        return (IEnumerable<RemoteFile>) this._files;
      }
    }

    public RemoteFile CreateFile(string path)
    {
      path = RemoteStorage.NormalizePath(path);
      this.InvalidateFiles();
      return this.Files.FirstOrDefault<RemoteFile>((Func<RemoteFile, bool>) (x => x.FileName == path)) ?? new RemoteFile(this, path, this.client.SteamId, 0);
    }

    public RemoteFile OpenFile(string path)
    {
      path = RemoteStorage.NormalizePath(path);
      this.InvalidateFiles();
      return this.Files.FirstOrDefault<RemoteFile>((Func<RemoteFile, bool>) (x => x.FileName == path));
    }

    public RemoteFile OpenSharedFile(ulong sharingId)
    {
      return new RemoteFile(this, (UGCHandle_t) sharingId);
    }

    public bool WriteString(string path, string text, Encoding encoding = null)
    {
      RemoteFile file = this.CreateFile(path);
      file.WriteAllText(text, encoding);
      return file.Exists;
    }

    public bool WriteBytes(string path, byte[] data)
    {
      RemoteFile file = this.CreateFile(path);
      file.WriteAllBytes(data);
      return file.Exists;
    }

    public string ReadString(string path, Encoding encoding = null)
    {
      return this.OpenFile(path)?.ReadAllText(encoding);
    }

    public byte[] ReadBytes(string path) => this.OpenFile(path)?.ReadAllBytes();

    internal void OnWrittenNewFile(RemoteFile file)
    {
      if (this._files.Any<RemoteFile>((Func<RemoteFile, bool>) (x => x.FileName == file.FileName)))
        return;
      this._files.Add(file);
      file.Exists = true;
      this.InvalidateFiles();
    }

    internal void InvalidateFiles() => this._filesInvalid = true;

    private void UpdateFiles()
    {
      if (!this._filesInvalid)
        return;
      this._filesInvalid = false;
      foreach (RemoteFile file in this._files)
        file.Exists = false;
      int fileCount = this.FileCount;
      for (int iFile = 0; iFile < fileCount; ++iFile)
      {
        int pnFileSizeInBytes;
        string name = RemoteStorage.NormalizePath(this.native.GetFileNameAndSize(iFile, out pnFileSizeInBytes));
        RemoteFile remoteFile = this._files.FirstOrDefault<RemoteFile>((Func<RemoteFile, bool>) (x => x.FileName == name));
        if (remoteFile == null)
        {
          remoteFile = new RemoteFile(this, name, this.client.SteamId, pnFileSizeInBytes);
          this._files.Add(remoteFile);
        }
        else
          remoteFile.SizeInBytes = pnFileSizeInBytes;
        remoteFile.Exists = true;
      }
      for (int index = this._files.Count - 1; index >= 0; --index)
      {
        if (!this._files[index].Exists)
          this._files.RemoveAt(index);
      }
    }

    public bool FileExists(string path) => this.native.FileExists(path);

    public void Dispose()
    {
      this.client = (Client) null;
      this.native = (SteamRemoteStorage) null;
    }

    public ulong QuotaUsed
    {
      get
      {
        ulong pnTotalBytes = 0;
        ulong puAvailableBytes = 0;
        return !this.native.GetQuota(out pnTotalBytes, out puAvailableBytes) ? 0UL : pnTotalBytes - puAvailableBytes;
      }
    }

    public ulong QuotaTotal
    {
      get
      {
        ulong pnTotalBytes = 0;
        ulong puAvailableBytes = 0;
        return !this.native.GetQuota(out pnTotalBytes, out puAvailableBytes) ? 0UL : pnTotalBytes;
      }
    }

    public ulong QuotaRemaining
    {
      get
      {
        ulong pnTotalBytes = 0;
        ulong puAvailableBytes = 0;
        return !this.native.GetQuota(out pnTotalBytes, out puAvailableBytes) ? 0UL : puAvailableBytes;
      }
    }
  }
}
