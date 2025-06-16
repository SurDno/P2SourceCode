using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SteamNative
{
  internal class SteamRemoteStorage : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamRemoteStorage(BaseSteamworks steamworks, IntPtr pointer)
    {
      this.steamworks = steamworks;
      if (Platform.IsWindows64)
        this.platform = (Platform.Interface) new Platform.Win64(pointer);
      else if (Platform.IsWindows32)
        this.platform = (Platform.Interface) new Platform.Win32(pointer);
      else if (Platform.IsLinux32)
        this.platform = (Platform.Interface) new Platform.Linux32(pointer);
      else if (Platform.IsLinux64)
      {
        this.platform = (Platform.Interface) new Platform.Linux64(pointer);
      }
      else
      {
        if (!Platform.IsOsx)
          return;
        this.platform = (Platform.Interface) new Platform.Mac(pointer);
      }
    }

    public bool IsValid => this.platform != null && this.platform.IsValid;

    public virtual void Dispose()
    {
      if (this.platform == null)
        return;
      this.platform.Dispose();
      this.platform = (Platform.Interface) null;
    }

    public CallbackHandle CommitPublishedFileUpdate(
      PublishedFileUpdateHandle_t updateHandle,
      Action<RemoteStorageUpdatePublishedFileResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_CommitPublishedFileUpdate(updateHandle.Value);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageUpdatePublishedFileResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public PublishedFileUpdateHandle_t CreatePublishedFileUpdateRequest(
      PublishedFileId_t unPublishedFileId)
    {
      return this.platform.ISteamRemoteStorage_CreatePublishedFileUpdateRequest(unPublishedFileId.Value);
    }

    public CallbackHandle DeletePublishedFile(
      PublishedFileId_t unPublishedFileId,
      Action<RemoteStorageDeletePublishedFileResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_DeletePublishedFile(unPublishedFileId.Value);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageDeletePublishedFileResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle EnumeratePublishedFilesByUserAction(
      WorkshopFileAction eAction,
      uint unStartIndex,
      Action<RemoteStorageEnumeratePublishedFilesByUserActionResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_EnumeratePublishedFilesByUserAction(eAction, unStartIndex);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageEnumeratePublishedFilesByUserActionResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle EnumeratePublishedWorkshopFiles(
      WorkshopEnumerationType eEnumerationType,
      uint unStartIndex,
      uint unCount,
      uint unDays,
      string[] pTags,
      ref SteamParamStringArray_t pUserTags,
      Action<RemoteStorageEnumerateWorkshopFilesResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t call = (SteamAPICall_t) 0UL;
      IntPtr[] source = new IntPtr[pTags.Length];
      for (int index = 0; index < pTags.Length; ++index)
        source[index] = Marshal.StringToHGlobalAnsi(pTags[index]);
      try
      {
        IntPtr destination = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * source.Length);
        Marshal.Copy(source, 0, destination, source.Length);
        call = this.platform.ISteamRemoteStorage_EnumeratePublishedWorkshopFiles(eEnumerationType, unStartIndex, unCount, unDays, ref new SteamParamStringArray_t()
        {
          Strings = destination,
          NumStrings = pTags.Length
        }, ref pUserTags);
      }
      finally
      {
        foreach (IntPtr hglobal in source)
          Marshal.FreeHGlobal(hglobal);
      }
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageEnumerateWorkshopFilesResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle EnumerateUserPublishedFiles(
      uint unStartIndex,
      Action<RemoteStorageEnumerateUserPublishedFilesResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_EnumerateUserPublishedFiles(unStartIndex);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageEnumerateUserPublishedFilesResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle EnumerateUserSharedWorkshopFiles(
      CSteamID steamId,
      uint unStartIndex,
      string[] pRequiredTags,
      ref SteamParamStringArray_t pExcludedTags,
      Action<RemoteStorageEnumerateUserPublishedFilesResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t call = (SteamAPICall_t) 0UL;
      IntPtr[] source = new IntPtr[pRequiredTags.Length];
      for (int index = 0; index < pRequiredTags.Length; ++index)
        source[index] = Marshal.StringToHGlobalAnsi(pRequiredTags[index]);
      try
      {
        IntPtr destination = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * source.Length);
        Marshal.Copy(source, 0, destination, source.Length);
        call = this.platform.ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles(steamId.Value, unStartIndex, ref new SteamParamStringArray_t()
        {
          Strings = destination,
          NumStrings = pRequiredTags.Length
        }, ref pExcludedTags);
      }
      finally
      {
        foreach (IntPtr hglobal in source)
          Marshal.FreeHGlobal(hglobal);
      }
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageEnumerateUserPublishedFilesResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle EnumerateUserSubscribedFiles(
      uint unStartIndex,
      Action<RemoteStorageEnumerateUserSubscribedFilesResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_EnumerateUserSubscribedFiles(unStartIndex);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageEnumerateUserSubscribedFilesResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool FileDelete(string pchFile) => this.platform.ISteamRemoteStorage_FileDelete(pchFile);

    public bool FileExists(string pchFile) => this.platform.ISteamRemoteStorage_FileExists(pchFile);

    public bool FileForget(string pchFile) => this.platform.ISteamRemoteStorage_FileForget(pchFile);

    public bool FilePersisted(string pchFile)
    {
      return this.platform.ISteamRemoteStorage_FilePersisted(pchFile);
    }

    public int FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
    {
      return this.platform.ISteamRemoteStorage_FileRead(pchFile, pvData, cubDataToRead);
    }

    public CallbackHandle FileReadAsync(
      string pchFile,
      uint nOffset,
      uint cubToRead,
      Action<RemoteStorageFileReadAsyncComplete_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_FileReadAsync(pchFile, nOffset, cubToRead);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageFileReadAsyncComplete_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool FileReadAsyncComplete(SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead)
    {
      return this.platform.ISteamRemoteStorage_FileReadAsyncComplete(hReadCall.Value, pvBuffer, cubToRead);
    }

    public CallbackHandle FileShare(
      string pchFile,
      Action<RemoteStorageFileShareResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_FileShare(pchFile);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageFileShareResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool FileWrite(string pchFile, IntPtr pvData, int cubData)
    {
      return this.platform.ISteamRemoteStorage_FileWrite(pchFile, pvData, cubData);
    }

    public CallbackHandle FileWriteAsync(
      string pchFile,
      IntPtr pvData,
      uint cubData,
      Action<RemoteStorageFileWriteAsyncComplete_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_FileWriteAsync(pchFile, pvData, cubData);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageFileWriteAsyncComplete_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool FileWriteStreamCancel(UGCFileWriteStreamHandle_t writeHandle)
    {
      return this.platform.ISteamRemoteStorage_FileWriteStreamCancel(writeHandle.Value);
    }

    public bool FileWriteStreamClose(UGCFileWriteStreamHandle_t writeHandle)
    {
      return this.platform.ISteamRemoteStorage_FileWriteStreamClose(writeHandle.Value);
    }

    public UGCFileWriteStreamHandle_t FileWriteStreamOpen(string pchFile)
    {
      return this.platform.ISteamRemoteStorage_FileWriteStreamOpen(pchFile);
    }

    public bool FileWriteStreamWriteChunk(
      UGCFileWriteStreamHandle_t writeHandle,
      IntPtr pvData,
      int cubData)
    {
      return this.platform.ISteamRemoteStorage_FileWriteStreamWriteChunk(writeHandle.Value, pvData, cubData);
    }

    public int GetCachedUGCCount() => this.platform.ISteamRemoteStorage_GetCachedUGCCount();

    public UGCHandle_t GetCachedUGCHandle(int iCachedContent)
    {
      return this.platform.ISteamRemoteStorage_GetCachedUGCHandle(iCachedContent);
    }

    public int GetFileCount() => this.platform.ISteamRemoteStorage_GetFileCount();

    public string GetFileNameAndSize(int iFile, out int pnFileSizeInBytes)
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamRemoteStorage_GetFileNameAndSize(iFile, out pnFileSizeInBytes));
    }

    public int GetFileSize(string pchFile)
    {
      return this.platform.ISteamRemoteStorage_GetFileSize(pchFile);
    }

    public long GetFileTimestamp(string pchFile)
    {
      return this.platform.ISteamRemoteStorage_GetFileTimestamp(pchFile);
    }

    public CallbackHandle GetPublishedFileDetails(
      PublishedFileId_t unPublishedFileId,
      uint unMaxSecondsOld,
      Action<RemoteStorageGetPublishedFileDetailsResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t publishedFileDetails = this.platform.ISteamRemoteStorage_GetPublishedFileDetails(unPublishedFileId.Value, unMaxSecondsOld);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageGetPublishedFileDetailsResult_t.CallResult(this.steamworks, publishedFileDetails, CallbackFunction);
    }

    public CallbackHandle GetPublishedItemVoteDetails(
      PublishedFileId_t unPublishedFileId,
      Action<RemoteStorageGetPublishedItemVoteDetailsResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t publishedItemVoteDetails = this.platform.ISteamRemoteStorage_GetPublishedItemVoteDetails(unPublishedFileId.Value);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageGetPublishedItemVoteDetailsResult_t.CallResult(this.steamworks, publishedItemVoteDetails, CallbackFunction);
    }

    public bool GetQuota(out ulong pnTotalBytes, out ulong puAvailableBytes)
    {
      return this.platform.ISteamRemoteStorage_GetQuota(out pnTotalBytes, out puAvailableBytes);
    }

    public RemoteStoragePlatform GetSyncPlatforms(string pchFile)
    {
      return this.platform.ISteamRemoteStorage_GetSyncPlatforms(pchFile);
    }

    public bool GetUGCDetails(
      UGCHandle_t hContent,
      ref AppId_t pnAppID,
      out string ppchName,
      out CSteamID pSteamIDOwner)
    {
      ppchName = string.Empty;
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int pnFileSizeInBytes = 4096;
      bool ugcDetails = this.platform.ISteamRemoteStorage_GetUGCDetails(hContent.Value, ref pnAppID.Value, stringBuilder, out pnFileSizeInBytes, out pSteamIDOwner.Value);
      if (!ugcDetails)
        return ugcDetails;
      ppchName = stringBuilder.ToString();
      return ugcDetails;
    }

    public bool GetUGCDownloadProgress(
      UGCHandle_t hContent,
      out int pnBytesDownloaded,
      out int pnBytesExpected)
    {
      return this.platform.ISteamRemoteStorage_GetUGCDownloadProgress(hContent.Value, out pnBytesDownloaded, out pnBytesExpected);
    }

    public CallbackHandle GetUserPublishedItemVoteDetails(
      PublishedFileId_t unPublishedFileId,
      Action<RemoteStorageGetPublishedItemVoteDetailsResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t publishedItemVoteDetails = this.platform.ISteamRemoteStorage_GetUserPublishedItemVoteDetails(unPublishedFileId.Value);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageGetPublishedItemVoteDetailsResult_t.CallResult(this.steamworks, publishedItemVoteDetails, CallbackFunction);
    }

    public bool IsCloudEnabledForAccount()
    {
      return this.platform.ISteamRemoteStorage_IsCloudEnabledForAccount();
    }

    public bool IsCloudEnabledForApp() => this.platform.ISteamRemoteStorage_IsCloudEnabledForApp();

    public CallbackHandle PublishVideo(
      WorkshopVideoProvider eVideoProvider,
      string pchVideoAccount,
      string pchVideoIdentifier,
      string pchPreviewFile,
      AppId_t nConsumerAppId,
      string pchTitle,
      string pchDescription,
      RemoteStoragePublishedFileVisibility eVisibility,
      string[] pTags,
      Action<RemoteStoragePublishFileProgress_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t call = (SteamAPICall_t) 0UL;
      IntPtr[] source = new IntPtr[pTags.Length];
      for (int index = 0; index < pTags.Length; ++index)
        source[index] = Marshal.StringToHGlobalAnsi(pTags[index]);
      try
      {
        IntPtr destination = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * source.Length);
        Marshal.Copy(source, 0, destination, source.Length);
        call = this.platform.ISteamRemoteStorage_PublishVideo(eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId.Value, pchTitle, pchDescription, eVisibility, ref new SteamParamStringArray_t()
        {
          Strings = destination,
          NumStrings = pTags.Length
        });
      }
      finally
      {
        foreach (IntPtr hglobal in source)
          Marshal.FreeHGlobal(hglobal);
      }
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStoragePublishFileProgress_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle PublishWorkshopFile(
      string pchFile,
      string pchPreviewFile,
      AppId_t nConsumerAppId,
      string pchTitle,
      string pchDescription,
      RemoteStoragePublishedFileVisibility eVisibility,
      string[] pTags,
      WorkshopFileType eWorkshopFileType,
      Action<RemoteStoragePublishFileProgress_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t call = (SteamAPICall_t) 0UL;
      IntPtr[] source = new IntPtr[pTags.Length];
      for (int index = 0; index < pTags.Length; ++index)
        source[index] = Marshal.StringToHGlobalAnsi(pTags[index]);
      try
      {
        IntPtr destination = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * source.Length);
        Marshal.Copy(source, 0, destination, source.Length);
        call = this.platform.ISteamRemoteStorage_PublishWorkshopFile(pchFile, pchPreviewFile, nConsumerAppId.Value, pchTitle, pchDescription, eVisibility, ref new SteamParamStringArray_t()
        {
          Strings = destination,
          NumStrings = pTags.Length
        }, eWorkshopFileType);
      }
      finally
      {
        foreach (IntPtr hglobal in source)
          Marshal.FreeHGlobal(hglobal);
      }
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStoragePublishFileProgress_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public void SetCloudEnabledForApp(bool bEnabled)
    {
      this.platform.ISteamRemoteStorage_SetCloudEnabledForApp(bEnabled);
    }

    public bool SetSyncPlatforms(string pchFile, RemoteStoragePlatform eRemoteStoragePlatform)
    {
      return this.platform.ISteamRemoteStorage_SetSyncPlatforms(pchFile, eRemoteStoragePlatform);
    }

    public CallbackHandle SetUserPublishedFileAction(
      PublishedFileId_t unPublishedFileId,
      WorkshopFileAction eAction,
      Action<RemoteStorageSetUserPublishedFileActionResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_SetUserPublishedFileAction(unPublishedFileId.Value, eAction);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageSetUserPublishedFileActionResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle SubscribePublishedFile(
      PublishedFileId_t unPublishedFileId,
      Action<RemoteStorageSubscribePublishedFileResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_SubscribePublishedFile(unPublishedFileId.Value);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageSubscribePublishedFileResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle UGCDownload(
      UGCHandle_t hContent,
      uint unPriority,
      Action<RemoteStorageDownloadUGCResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_UGCDownload(hContent.Value, unPriority);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageDownloadUGCResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle UGCDownloadToLocation(
      UGCHandle_t hContent,
      string pchLocation,
      uint unPriority,
      Action<RemoteStorageDownloadUGCResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t location = this.platform.ISteamRemoteStorage_UGCDownloadToLocation(hContent.Value, pchLocation, unPriority);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageDownloadUGCResult_t.CallResult(this.steamworks, location, CallbackFunction);
    }

    public int UGCRead(
      UGCHandle_t hContent,
      IntPtr pvData,
      int cubDataToRead,
      uint cOffset,
      UGCReadAction eAction)
    {
      return this.platform.ISteamRemoteStorage_UGCRead(hContent.Value, pvData, cubDataToRead, cOffset, eAction);
    }

    public CallbackHandle UnsubscribePublishedFile(
      PublishedFileId_t unPublishedFileId,
      Action<RemoteStorageUnsubscribePublishedFileResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_UnsubscribePublishedFile(unPublishedFileId.Value);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageUnsubscribePublishedFileResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool UpdatePublishedFileDescription(
      PublishedFileUpdateHandle_t updateHandle,
      string pchDescription)
    {
      return this.platform.ISteamRemoteStorage_UpdatePublishedFileDescription(updateHandle.Value, pchDescription);
    }

    public bool UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile)
    {
      return this.platform.ISteamRemoteStorage_UpdatePublishedFileFile(updateHandle.Value, pchFile);
    }

    public bool UpdatePublishedFilePreviewFile(
      PublishedFileUpdateHandle_t updateHandle,
      string pchPreviewFile)
    {
      return this.platform.ISteamRemoteStorage_UpdatePublishedFilePreviewFile(updateHandle.Value, pchPreviewFile);
    }

    public bool UpdatePublishedFileSetChangeDescription(
      PublishedFileUpdateHandle_t updateHandle,
      string pchChangeDescription)
    {
      return this.platform.ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription(updateHandle.Value, pchChangeDescription);
    }

    public bool UpdatePublishedFileTags(PublishedFileUpdateHandle_t updateHandle, string[] pTags)
    {
      IntPtr[] source = new IntPtr[pTags.Length];
      for (int index = 0; index < pTags.Length; ++index)
        source[index] = Marshal.StringToHGlobalAnsi(pTags[index]);
      try
      {
        IntPtr destination = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * source.Length);
        Marshal.Copy(source, 0, destination, source.Length);
        return this.platform.ISteamRemoteStorage_UpdatePublishedFileTags(updateHandle.Value, ref new SteamParamStringArray_t()
        {
          Strings = destination,
          NumStrings = pTags.Length
        });
      }
      finally
      {
        foreach (IntPtr hglobal in source)
          Marshal.FreeHGlobal(hglobal);
      }
    }

    public bool UpdatePublishedFileTitle(PublishedFileUpdateHandle_t updateHandle, string pchTitle)
    {
      return this.platform.ISteamRemoteStorage_UpdatePublishedFileTitle(updateHandle.Value, pchTitle);
    }

    public bool UpdatePublishedFileVisibility(
      PublishedFileUpdateHandle_t updateHandle,
      RemoteStoragePublishedFileVisibility eVisibility)
    {
      return this.platform.ISteamRemoteStorage_UpdatePublishedFileVisibility(updateHandle.Value, eVisibility);
    }

    public CallbackHandle UpdateUserPublishedItemVote(
      PublishedFileId_t unPublishedFileId,
      bool bVoteUp,
      Action<RemoteStorageUpdateUserPublishedItemVoteResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamRemoteStorage_UpdateUserPublishedItemVote(unPublishedFileId.Value, bVoteUp);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageUpdateUserPublishedItemVoteResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }
  }
}
