using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SteamNative;

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
      client = c;
      native = client.native.remoteStorage;
    }

    public bool IsCloudEnabledForAccount => native.IsCloudEnabledForAccount();

    public bool IsCloudEnabledForApp => native.IsCloudEnabledForApp();

    public int FileCount => native.GetFileCount();

    public IEnumerable<RemoteFile> Files
    {
      get
      {
        UpdateFiles();
        return _files;
      }
    }

    public RemoteFile CreateFile(string path)
    {
      path = NormalizePath(path);
      InvalidateFiles();
      return Files.FirstOrDefault(x => x.FileName == path) ?? new RemoteFile(this, path, client.SteamId, 0);
    }

    public RemoteFile OpenFile(string path)
    {
      path = NormalizePath(path);
      InvalidateFiles();
      return Files.FirstOrDefault(x => x.FileName == path);
    }

    public RemoteFile OpenSharedFile(ulong sharingId)
    {
      return new RemoteFile(this, sharingId);
    }

    public bool WriteString(string path, string text, Encoding encoding = null)
    {
      RemoteFile file = CreateFile(path);
      file.WriteAllText(text, encoding);
      return file.Exists;
    }

    public bool WriteBytes(string path, byte[] data)
    {
      RemoteFile file = CreateFile(path);
      file.WriteAllBytes(data);
      return file.Exists;
    }

    public string ReadString(string path, Encoding encoding = null)
    {
      return OpenFile(path)?.ReadAllText(encoding);
    }

    public byte[] ReadBytes(string path) => OpenFile(path)?.ReadAllBytes();

    internal void OnWrittenNewFile(RemoteFile file)
    {
      if (_files.Any(x => x.FileName == file.FileName))
        return;
      _files.Add(file);
      file.Exists = true;
      InvalidateFiles();
    }

    internal void InvalidateFiles() => _filesInvalid = true;

    private void UpdateFiles()
    {
      if (!_filesInvalid)
        return;
      _filesInvalid = false;
      foreach (RemoteFile file in _files)
        file.Exists = false;
      int fileCount = FileCount;
      for (int iFile = 0; iFile < fileCount; ++iFile)
      {
        int pnFileSizeInBytes;
        string name = NormalizePath(native.GetFileNameAndSize(iFile, out pnFileSizeInBytes));
        RemoteFile remoteFile = _files.FirstOrDefault(x => x.FileName == name);
        if (remoteFile == null)
        {
          remoteFile = new RemoteFile(this, name, client.SteamId, pnFileSizeInBytes);
          _files.Add(remoteFile);
        }
        else
          remoteFile.SizeInBytes = pnFileSizeInBytes;
        remoteFile.Exists = true;
      }
      for (int index = _files.Count - 1; index >= 0; --index)
      {
        if (!_files[index].Exists)
          _files.RemoveAt(index);
      }
    }

    public bool FileExists(string path) => native.FileExists(path);

    public void Dispose()
    {
      client = null;
      native = null;
    }

    public ulong QuotaUsed
    {
      get
      {
        ulong pnTotalBytes = 0;
        ulong puAvailableBytes = 0;
        return !native.GetQuota(out pnTotalBytes, out puAvailableBytes) ? 0UL : pnTotalBytes - puAvailableBytes;
      }
    }

    public ulong QuotaTotal
    {
      get
      {
        ulong pnTotalBytes = 0;
        ulong puAvailableBytes = 0;
        return !native.GetQuota(out pnTotalBytes, out puAvailableBytes) ? 0UL : pnTotalBytes;
      }
    }

    public ulong QuotaRemaining
    {
      get
      {
        ulong pnTotalBytes = 0;
        ulong puAvailableBytes = 0;
        return !native.GetQuota(out pnTotalBytes, out puAvailableBytes) ? 0UL : puAvailableBytes;
      }
    }
  }
}
