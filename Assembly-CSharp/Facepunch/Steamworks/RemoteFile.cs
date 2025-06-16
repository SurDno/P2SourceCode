using System;
using System.IO;
using System.Text;
using Facepunch.Steamworks.Callbacks;
using SteamNative;
using Result = SteamNative.Result;

namespace Facepunch.Steamworks;

public class RemoteFile {
	internal readonly RemoteStorage remoteStorage;
	private readonly bool _isUgc;
	private string _fileName;
	private int _sizeInBytes = -1;
	private UGCHandle_t _handle;
	private ulong _ownerId;
	private bool _isDownloading;
	private byte[] _downloadedData;

	public bool Exists { get; internal set; }

	public bool IsDownloading => _isUgc && _isDownloading && _downloadedData == null;

	public bool IsDownloaded => !_isUgc || _downloadedData != null;

	public bool IsShared => _handle.Value > 0UL;

	internal UGCHandle_t UGCHandle => _handle;

	public ulong SharingId => UGCHandle.Value;

	public string FileName {
		get {
			if (_fileName != null)
				return _fileName;
			GetUGCDetails();
			return _fileName;
		}
	}

	public ulong OwnerId {
		get {
			if (_ownerId > 0UL)
				return _ownerId;
			GetUGCDetails();
			return _ownerId;
		}
	}

	public int SizeInBytes {
		get {
			if (_sizeInBytes != -1)
				return _sizeInBytes;
			if (_isUgc)
				throw new NotImplementedException();
			_sizeInBytes = remoteStorage.native.GetFileSize(FileName);
			return _sizeInBytes;
		}
		internal set => _sizeInBytes = value;
	}

	internal RemoteFile(RemoteStorage r, UGCHandle_t handle) {
		Exists = true;
		remoteStorage = r;
		_isUgc = true;
		_handle = handle;
	}

	internal RemoteFile(RemoteStorage r, string name, ulong ownerId, int sizeInBytes = -1) {
		remoteStorage = r;
		_isUgc = false;
		_fileName = name;
		_ownerId = ownerId;
		_sizeInBytes = sizeInBytes;
	}

	public RemoteFileWriteStream OpenWrite() {
		if (_isUgc)
			throw new InvalidOperationException("Cannot write to a shared file.");
		return new RemoteFileWriteStream(remoteStorage, this);
	}

	public void WriteAllBytes(byte[] buffer) {
		using (var remoteFileWriteStream = OpenWrite()) {
			remoteFileWriteStream.Write(buffer, 0, buffer.Length);
		}
	}

	public void WriteAllText(string text, Encoding encoding = null) {
		if (encoding == null)
			encoding = Encoding.UTF8;
		WriteAllBytes(encoding.GetBytes(text));
	}

	public bool GetDownloadProgress(out int bytesDownloaded, out int bytesExpected) {
		return remoteStorage.native.GetUGCDownloadProgress(_handle, out bytesDownloaded, out bytesExpected);
	}

	public unsafe bool Download(DownloadCallback onSuccess = null, FailureCallback onFailure = null) {
		if (!_isUgc || _isDownloading || IsDownloaded)
			return false;
		_isDownloading = true;
		remoteStorage.native.UGCDownload(_handle, 1000U, (result, error) => {
			_isDownloading = false;
			if (error || result.Result != Result.OK) {
				var failureCallback = onFailure;
				if (failureCallback == null)
					return;
				failureCallback(result.Result == 0 ? Callbacks.Result.IOFailure : (Callbacks.Result)result.Result);
			} else {
				_ownerId = result.SteamIDOwner;
				_sizeInBytes = result.SizeInBytes;
				_fileName = result.PchFileName;
				_downloadedData = new byte[_sizeInBytes];
				fixed (byte* pvData = _downloadedData) {
					remoteStorage.native.UGCRead(_handle, (IntPtr)pvData, _sizeInBytes, 0U,
						UGCReadAction.ontinueReading);
				}

				var downloadCallback = onSuccess;
				if (downloadCallback == null)
					return;
				downloadCallback();
			}
		});
		return true;
	}

	public Stream OpenRead() {
		return new MemoryStream(ReadAllBytes(), false);
	}

	public unsafe byte[] ReadAllBytes() {
		if (_isUgc) {
			if (!IsDownloaded)
				throw new Exception("Cannot read a file that hasn't been downloaded.");
			return _downloadedData;
		}

		var sizeInBytes = SizeInBytes;
		var numArray = new byte[sizeInBytes];
		fixed (byte* pvData = numArray) {
			remoteStorage.native.FileRead(FileName, (IntPtr)pvData, sizeInBytes);
		}

		return numArray;
	}

	public string ReadAllText(Encoding encoding = null) {
		if (encoding == null)
			encoding = Encoding.UTF8;
		return encoding.GetString(ReadAllBytes());
	}

	public bool Share(ShareCallback onSuccess = null, FailureCallback onFailure = null) {
		if (_isUgc || _handle.Value > 0UL)
			return false;
		remoteStorage.native.FileShare(FileName, (result, error) => {
			if (!error && result.Result == Result.OK) {
				_handle.Value = result.File;
				var shareCallback = onSuccess;
				if (shareCallback == null)
					return;
				shareCallback();
			} else {
				var failureCallback = onFailure;
				if (failureCallback != null)
					failureCallback(result.Result == 0 ? Callbacks.Result.IOFailure : (Callbacks.Result)result.Result);
			}
		});
		return true;
	}

	public bool Delete() {
		if (!Exists || _isUgc || !remoteStorage.native.FileDelete(FileName))
			return false;
		Exists = false;
		remoteStorage.InvalidateFiles();
		return true;
	}

	public bool Forget() {
		return Exists && !_isUgc && remoteStorage.native.FileForget(FileName);
	}

	private void GetUGCDetails() {
		if (!_isUgc)
			throw new InvalidOperationException();
		var pnAppID = new AppId_t {
			Value = remoteStorage.native.steamworks.AppId
		};
		CSteamID pSteamIDOwner;
		remoteStorage.native.GetUGCDetails(_handle, ref pnAppID, out _fileName, out pSteamIDOwner);
		_ownerId = pSteamIDOwner.Value;
	}

	public delegate void DownloadCallback();

	public delegate void ShareCallback();
}