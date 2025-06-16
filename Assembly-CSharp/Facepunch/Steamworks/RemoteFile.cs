using Facepunch.Steamworks.Callbacks;
using SteamNative;
using System;
using System.IO;
using System.Text;

namespace Facepunch.Steamworks
{
  public class RemoteFile
  {
    internal readonly RemoteStorage remoteStorage;
    private readonly bool _isUgc;
    private string _fileName;
    private int _sizeInBytes = -1;
    private UGCHandle_t _handle;
    private ulong _ownerId;
    private bool _isDownloading;
    private byte[] _downloadedData;

    public bool Exists { get; internal set; }

    public bool IsDownloading => this._isUgc && this._isDownloading && this._downloadedData == null;

    public bool IsDownloaded => !this._isUgc || this._downloadedData != null;

    public bool IsShared => this._handle.Value > 0UL;

    internal UGCHandle_t UGCHandle => this._handle;

    public ulong SharingId => this.UGCHandle.Value;

    public string FileName
    {
      get
      {
        if (this._fileName != null)
          return this._fileName;
        this.GetUGCDetails();
        return this._fileName;
      }
    }

    public ulong OwnerId
    {
      get
      {
        if (this._ownerId > 0UL)
          return this._ownerId;
        this.GetUGCDetails();
        return this._ownerId;
      }
    }

    public int SizeInBytes
    {
      get
      {
        if (this._sizeInBytes != -1)
          return this._sizeInBytes;
        if (this._isUgc)
          throw new NotImplementedException();
        this._sizeInBytes = this.remoteStorage.native.GetFileSize(this.FileName);
        return this._sizeInBytes;
      }
      internal set => this._sizeInBytes = value;
    }

    internal RemoteFile(RemoteStorage r, UGCHandle_t handle)
    {
      this.Exists = true;
      this.remoteStorage = r;
      this._isUgc = true;
      this._handle = handle;
    }

    internal RemoteFile(RemoteStorage r, string name, ulong ownerId, int sizeInBytes = -1)
    {
      this.remoteStorage = r;
      this._isUgc = false;
      this._fileName = name;
      this._ownerId = ownerId;
      this._sizeInBytes = sizeInBytes;
    }

    public RemoteFileWriteStream OpenWrite()
    {
      if (this._isUgc)
        throw new InvalidOperationException("Cannot write to a shared file.");
      return new RemoteFileWriteStream(this.remoteStorage, this);
    }

    public void WriteAllBytes(byte[] buffer)
    {
      using (RemoteFileWriteStream remoteFileWriteStream = this.OpenWrite())
        remoteFileWriteStream.Write(buffer, 0, buffer.Length);
    }

    public void WriteAllText(string text, Encoding encoding = null)
    {
      if (encoding == null)
        encoding = Encoding.UTF8;
      this.WriteAllBytes(encoding.GetBytes(text));
    }

    public bool GetDownloadProgress(out int bytesDownloaded, out int bytesExpected)
    {
      return this.remoteStorage.native.GetUGCDownloadProgress(this._handle, out bytesDownloaded, out bytesExpected);
    }

    public unsafe bool Download(RemoteFile.DownloadCallback onSuccess = null, FailureCallback onFailure = null)
    {
      if (!this._isUgc || this._isDownloading || this.IsDownloaded)
        return false;
      this._isDownloading = true;
      this.remoteStorage.native.UGCDownload(this._handle, 1000U, (Action<RemoteStorageDownloadUGCResult_t, bool>) ((result, error) =>
      {
        this._isDownloading = false;
        if (error || result.Result != SteamNative.Result.OK)
        {
          FailureCallback failureCallback = onFailure;
          if (failureCallback == null)
            return;
          failureCallback(result.Result == (SteamNative.Result) 0 ? Facepunch.Steamworks.Callbacks.Result.IOFailure : (Facepunch.Steamworks.Callbacks.Result) result.Result);
        }
        else
        {
          this._ownerId = result.SteamIDOwner;
          this._sizeInBytes = result.SizeInBytes;
          this._fileName = result.PchFileName;
          this._downloadedData = new byte[this._sizeInBytes];
          fixed (byte* pvData = this._downloadedData)
            this.remoteStorage.native.UGCRead(this._handle, (IntPtr) (void*) pvData, this._sizeInBytes, 0U, UGCReadAction.ontinueReading);
          RemoteFile.DownloadCallback downloadCallback = onSuccess;
          if (downloadCallback == null)
            return;
          downloadCallback();
        }
      }));
      return true;
    }

    public Stream OpenRead() => (Stream) new MemoryStream(this.ReadAllBytes(), false);

    public unsafe byte[] ReadAllBytes()
    {
      if (this._isUgc)
      {
        if (!this.IsDownloaded)
          throw new Exception("Cannot read a file that hasn't been downloaded.");
        return this._downloadedData;
      }
      int sizeInBytes = this.SizeInBytes;
      byte[] numArray = new byte[sizeInBytes];
      fixed (byte* pvData = numArray)
        this.remoteStorage.native.FileRead(this.FileName, (IntPtr) (void*) pvData, sizeInBytes);
      return numArray;
    }

    public string ReadAllText(Encoding encoding = null)
    {
      if (encoding == null)
        encoding = Encoding.UTF8;
      return encoding.GetString(this.ReadAllBytes());
    }

    public bool Share(RemoteFile.ShareCallback onSuccess = null, FailureCallback onFailure = null)
    {
      if (this._isUgc || this._handle.Value > 0UL)
        return false;
      this.remoteStorage.native.FileShare(this.FileName, (Action<RemoteStorageFileShareResult_t, bool>) ((result, error) =>
      {
        if (!error && result.Result == SteamNative.Result.OK)
        {
          this._handle.Value = result.File;
          RemoteFile.ShareCallback shareCallback = onSuccess;
          if (shareCallback == null)
            return;
          shareCallback();
        }
        else
        {
          FailureCallback failureCallback = onFailure;
          if (failureCallback != null)
            failureCallback(result.Result == (SteamNative.Result) 0 ? Facepunch.Steamworks.Callbacks.Result.IOFailure : (Facepunch.Steamworks.Callbacks.Result) result.Result);
        }
      }));
      return true;
    }

    public bool Delete()
    {
      if (!this.Exists || this._isUgc || !this.remoteStorage.native.FileDelete(this.FileName))
        return false;
      this.Exists = false;
      this.remoteStorage.InvalidateFiles();
      return true;
    }

    public bool Forget()
    {
      return this.Exists && !this._isUgc && this.remoteStorage.native.FileForget(this.FileName);
    }

    private void GetUGCDetails()
    {
      if (!this._isUgc)
        throw new InvalidOperationException();
      AppId_t pnAppID = new AppId_t()
      {
        Value = this.remoteStorage.native.steamworks.AppId
      };
      CSteamID pSteamIDOwner;
      this.remoteStorage.native.GetUGCDetails(this._handle, ref pnAppID, out this._fileName, out pSteamIDOwner);
      this._ownerId = pSteamIDOwner.Value;
    }

    public delegate void DownloadCallback();

    public delegate void ShareCallback();
  }
}
