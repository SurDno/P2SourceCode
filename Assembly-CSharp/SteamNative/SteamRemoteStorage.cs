using System;
using System.Runtime.InteropServices;
using System.Text;
using Facepunch.Steamworks;

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
        platform = (Platform.Interface) new Platform.Win64(pointer);
      else if (Platform.IsWindows32)
        platform = (Platform.Interface) new Platform.Win32(pointer);
      else if (Platform.IsLinux32)
        platform = (Platform.Interface) new Platform.Linux32(pointer);
      else if (Platform.IsLinux64)
      {
        platform = (Platform.Interface) new Platform.Linux64(pointer);
      }
      else
      {
        if (!Platform.IsOsx)
          return;
        platform = (Platform.Interface) new Platform.Mac(pointer);
      }
    }

    public bool IsValid => platform != null && platform.IsValid;

    public virtual void Dispose()
    {
      if (platform == null)
        return;
      platform.Dispose();
      platform = (Platform.Interface) null;
    }

    public CallbackHandle CommitPublishedFileUpdate(
      PublishedFileUpdateHandle_t updateHandle,
      Action<RemoteStorageUpdatePublishedFileResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_CommitPublishedFileUpdate(updateHandle.Value);
      return CallbackFunction == null ? null : RemoteStorageUpdatePublishedFileResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public PublishedFileUpdateHandle_t CreatePublishedFileUpdateRequest(
      PublishedFileId_t unPublishedFileId)
    {
      return platform.ISteamRemoteStorage_CreatePublishedFileUpdateRequest(unPublishedFileId.Value);
    }

    public CallbackHandle DeletePublishedFile(
      PublishedFileId_t unPublishedFileId,
      Action<RemoteStorageDeletePublishedFileResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_DeletePublishedFile(unPublishedFileId.Value);
      return CallbackFunction == null ? null : RemoteStorageDeletePublishedFileResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public CallbackHandle EnumeratePublishedFilesByUserAction(
      WorkshopFileAction eAction,
      uint unStartIndex,
      Action<RemoteStorageEnumeratePublishedFilesByUserActionResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_EnumeratePublishedFilesByUserAction(eAction, unStartIndex);
      return CallbackFunction == null ? null : RemoteStorageEnumeratePublishedFilesByUserActionResult_t.CallResult(steamworks, call, CallbackFunction);
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
      SteamAPICall_t call = 0UL;
      IntPtr[] source = new IntPtr[pTags.Length];
      for (int index = 0; index < pTags.Length; ++index)
        source[index] = Marshal.StringToHGlobalAnsi(pTags[index]);
      try
      {
        IntPtr destination = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * source.Length);
        Marshal.Copy(source, 0, destination, source.Length);
        call = platform.ISteamRemoteStorage_EnumeratePublishedWorkshopFiles(eEnumerationType, unStartIndex, unCount, unDays, ref new SteamParamStringArray_t {
          Strings = destination,
          NumStrings = pTags.Length
        }, ref pUserTags);
      }
      finally
      {
        foreach (IntPtr hglobal in source)
          Marshal.FreeHGlobal(hglobal);
      }
      return CallbackFunction == null ? null : RemoteStorageEnumerateWorkshopFilesResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public CallbackHandle EnumerateUserPublishedFiles(
      uint unStartIndex,
      Action<RemoteStorageEnumerateUserPublishedFilesResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_EnumerateUserPublishedFiles(unStartIndex);
      return CallbackFunction == null ? null : RemoteStorageEnumerateUserPublishedFilesResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public CallbackHandle EnumerateUserSharedWorkshopFiles(
      CSteamID steamId,
      uint unStartIndex,
      string[] pRequiredTags,
      ref SteamParamStringArray_t pExcludedTags,
      Action<RemoteStorageEnumerateUserPublishedFilesResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t call = 0UL;
      IntPtr[] source = new IntPtr[pRequiredTags.Length];
      for (int index = 0; index < pRequiredTags.Length; ++index)
        source[index] = Marshal.StringToHGlobalAnsi(pRequiredTags[index]);
      try
      {
        IntPtr destination = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * source.Length);
        Marshal.Copy(source, 0, destination, source.Length);
        call = platform.ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles(steamId.Value, unStartIndex, ref new SteamParamStringArray_t {
          Strings = destination,
          NumStrings = pRequiredTags.Length
        }, ref pExcludedTags);
      }
      finally
      {
        foreach (IntPtr hglobal in source)
          Marshal.FreeHGlobal(hglobal);
      }
      return CallbackFunction == null ? null : RemoteStorageEnumerateUserPublishedFilesResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public CallbackHandle EnumerateUserSubscribedFiles(
      uint unStartIndex,
      Action<RemoteStorageEnumerateUserSubscribedFilesResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_EnumerateUserSubscribedFiles(unStartIndex);
      return CallbackFunction == null ? null : RemoteStorageEnumerateUserSubscribedFilesResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public bool FileDelete(string pchFile) => platform.ISteamRemoteStorage_FileDelete(pchFile);

    public bool FileExists(string pchFile) => platform.ISteamRemoteStorage_FileExists(pchFile);

    public bool FileForget(string pchFile) => platform.ISteamRemoteStorage_FileForget(pchFile);

    public bool FilePersisted(string pchFile)
    {
      return platform.ISteamRemoteStorage_FilePersisted(pchFile);
    }

    public int FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
    {
      return platform.ISteamRemoteStorage_FileRead(pchFile, pvData, cubDataToRead);
    }

    public CallbackHandle FileReadAsync(
      string pchFile,
      uint nOffset,
      uint cubToRead,
      Action<RemoteStorageFileReadAsyncComplete_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_FileReadAsync(pchFile, nOffset, cubToRead);
      return CallbackFunction == null ? null : RemoteStorageFileReadAsyncComplete_t.CallResult(steamworks, call, CallbackFunction);
    }

    public bool FileReadAsyncComplete(SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead)
    {
      return platform.ISteamRemoteStorage_FileReadAsyncComplete(hReadCall.Value, pvBuffer, cubToRead);
    }

    public CallbackHandle FileShare(
      string pchFile,
      Action<RemoteStorageFileShareResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_FileShare(pchFile);
      return CallbackFunction == null ? null : RemoteStorageFileShareResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public bool FileWrite(string pchFile, IntPtr pvData, int cubData)
    {
      return platform.ISteamRemoteStorage_FileWrite(pchFile, pvData, cubData);
    }

    public CallbackHandle FileWriteAsync(
      string pchFile,
      IntPtr pvData,
      uint cubData,
      Action<RemoteStorageFileWriteAsyncComplete_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_FileWriteAsync(pchFile, pvData, cubData);
      return CallbackFunction == null ? null : RemoteStorageFileWriteAsyncComplete_t.CallResult(steamworks, call, CallbackFunction);
    }

    public bool FileWriteStreamCancel(UGCFileWriteStreamHandle_t writeHandle)
    {
      return platform.ISteamRemoteStorage_FileWriteStreamCancel(writeHandle.Value);
    }

    public bool FileWriteStreamClose(UGCFileWriteStreamHandle_t writeHandle)
    {
      return platform.ISteamRemoteStorage_FileWriteStreamClose(writeHandle.Value);
    }

    public UGCFileWriteStreamHandle_t FileWriteStreamOpen(string pchFile)
    {
      return platform.ISteamRemoteStorage_FileWriteStreamOpen(pchFile);
    }

    public bool FileWriteStreamWriteChunk(
      UGCFileWriteStreamHandle_t writeHandle,
      IntPtr pvData,
      int cubData)
    {
      return platform.ISteamRemoteStorage_FileWriteStreamWriteChunk(writeHandle.Value, pvData, cubData);
    }

    public int GetCachedUGCCount() => platform.ISteamRemoteStorage_GetCachedUGCCount();

    public UGCHandle_t GetCachedUGCHandle(int iCachedContent)
    {
      return platform.ISteamRemoteStorage_GetCachedUGCHandle(iCachedContent);
    }

    public int GetFileCount() => platform.ISteamRemoteStorage_GetFileCount();

    public string GetFileNameAndSize(int iFile, out int pnFileSizeInBytes)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamRemoteStorage_GetFileNameAndSize(iFile, out pnFileSizeInBytes));
    }

    public int GetFileSize(string pchFile)
    {
      return platform.ISteamRemoteStorage_GetFileSize(pchFile);
    }

    public long GetFileTimestamp(string pchFile)
    {
      return platform.ISteamRemoteStorage_GetFileTimestamp(pchFile);
    }

    public CallbackHandle GetPublishedFileDetails(
      PublishedFileId_t unPublishedFileId,
      uint unMaxSecondsOld,
      Action<RemoteStorageGetPublishedFileDetailsResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t publishedFileDetails = platform.ISteamRemoteStorage_GetPublishedFileDetails(unPublishedFileId.Value, unMaxSecondsOld);
      return CallbackFunction == null ? null : RemoteStorageGetPublishedFileDetailsResult_t.CallResult(steamworks, publishedFileDetails, CallbackFunction);
    }

    public CallbackHandle GetPublishedItemVoteDetails(
      PublishedFileId_t unPublishedFileId,
      Action<RemoteStorageGetPublishedItemVoteDetailsResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t publishedItemVoteDetails = platform.ISteamRemoteStorage_GetPublishedItemVoteDetails(unPublishedFileId.Value);
      return CallbackFunction == null ? null : RemoteStorageGetPublishedItemVoteDetailsResult_t.CallResult(steamworks, publishedItemVoteDetails, CallbackFunction);
    }

    public bool GetQuota(out ulong pnTotalBytes, out ulong puAvailableBytes)
    {
      return platform.ISteamRemoteStorage_GetQuota(out pnTotalBytes, out puAvailableBytes);
    }

    public RemoteStoragePlatform GetSyncPlatforms(string pchFile)
    {
      return platform.ISteamRemoteStorage_GetSyncPlatforms(pchFile);
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
      bool ugcDetails = platform.ISteamRemoteStorage_GetUGCDetails(hContent.Value, ref pnAppID.Value, stringBuilder, out pnFileSizeInBytes, out pSteamIDOwner.Value);
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
      return platform.ISteamRemoteStorage_GetUGCDownloadProgress(hContent.Value, out pnBytesDownloaded, out pnBytesExpected);
    }

    public CallbackHandle GetUserPublishedItemVoteDetails(
      PublishedFileId_t unPublishedFileId,
      Action<RemoteStorageGetPublishedItemVoteDetailsResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t publishedItemVoteDetails = platform.ISteamRemoteStorage_GetUserPublishedItemVoteDetails(unPublishedFileId.Value);
      return CallbackFunction == null ? null : RemoteStorageGetPublishedItemVoteDetailsResult_t.CallResult(steamworks, publishedItemVoteDetails, CallbackFunction);
    }

    public bool IsCloudEnabledForAccount()
    {
      return platform.ISteamRemoteStorage_IsCloudEnabledForAccount();
    }

    public bool IsCloudEnabledForApp() => platform.ISteamRemoteStorage_IsCloudEnabledForApp();

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
      SteamAPICall_t call = 0UL;
      IntPtr[] source = new IntPtr[pTags.Length];
      for (int index = 0; index < pTags.Length; ++index)
        source[index] = Marshal.StringToHGlobalAnsi(pTags[index]);
      try
      {
        IntPtr destination = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * source.Length);
        Marshal.Copy(source, 0, destination, source.Length);
        call = platform.ISteamRemoteStorage_PublishVideo(eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId.Value, pchTitle, pchDescription, eVisibility, ref new SteamParamStringArray_t {
          Strings = destination,
          NumStrings = pTags.Length
        });
      }
      finally
      {
        foreach (IntPtr hglobal in source)
          Marshal.FreeHGlobal(hglobal);
      }
      return CallbackFunction == null ? null : RemoteStoragePublishFileProgress_t.CallResult(steamworks, call, CallbackFunction);
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
      SteamAPICall_t call = 0UL;
      IntPtr[] source = new IntPtr[pTags.Length];
      for (int index = 0; index < pTags.Length; ++index)
        source[index] = Marshal.StringToHGlobalAnsi(pTags[index]);
      try
      {
        IntPtr destination = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * source.Length);
        Marshal.Copy(source, 0, destination, source.Length);
        call = platform.ISteamRemoteStorage_PublishWorkshopFile(pchFile, pchPreviewFile, nConsumerAppId.Value, pchTitle, pchDescription, eVisibility, ref new SteamParamStringArray_t {
          Strings = destination,
          NumStrings = pTags.Length
        }, eWorkshopFileType);
      }
      finally
      {
        foreach (IntPtr hglobal in source)
          Marshal.FreeHGlobal(hglobal);
      }
      return CallbackFunction == null ? null : RemoteStoragePublishFileProgress_t.CallResult(steamworks, call, CallbackFunction);
    }

    public void SetCloudEnabledForApp(bool bEnabled)
    {
      platform.ISteamRemoteStorage_SetCloudEnabledForApp(bEnabled);
    }

    public bool SetSyncPlatforms(string pchFile, RemoteStoragePlatform eRemoteStoragePlatform)
    {
      return platform.ISteamRemoteStorage_SetSyncPlatforms(pchFile, eRemoteStoragePlatform);
    }

    public CallbackHandle SetUserPublishedFileAction(
      PublishedFileId_t unPublishedFileId,
      WorkshopFileAction eAction,
      Action<RemoteStorageSetUserPublishedFileActionResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_SetUserPublishedFileAction(unPublishedFileId.Value, eAction);
      return CallbackFunction == null ? null : RemoteStorageSetUserPublishedFileActionResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public CallbackHandle SubscribePublishedFile(
      PublishedFileId_t unPublishedFileId,
      Action<RemoteStorageSubscribePublishedFileResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_SubscribePublishedFile(unPublishedFileId.Value);
      return CallbackFunction == null ? null : RemoteStorageSubscribePublishedFileResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public CallbackHandle UGCDownload(
      UGCHandle_t hContent,
      uint unPriority,
      Action<RemoteStorageDownloadUGCResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_UGCDownload(hContent.Value, unPriority);
      return CallbackFunction == null ? null : RemoteStorageDownloadUGCResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public CallbackHandle UGCDownloadToLocation(
      UGCHandle_t hContent,
      string pchLocation,
      uint unPriority,
      Action<RemoteStorageDownloadUGCResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t location = platform.ISteamRemoteStorage_UGCDownloadToLocation(hContent.Value, pchLocation, unPriority);
      return CallbackFunction == null ? null : RemoteStorageDownloadUGCResult_t.CallResult(steamworks, location, CallbackFunction);
    }

    public int UGCRead(
      UGCHandle_t hContent,
      IntPtr pvData,
      int cubDataToRead,
      uint cOffset,
      UGCReadAction eAction)
    {
      return platform.ISteamRemoteStorage_UGCRead(hContent.Value, pvData, cubDataToRead, cOffset, eAction);
    }

    public CallbackHandle UnsubscribePublishedFile(
      PublishedFileId_t unPublishedFileId,
      Action<RemoteStorageUnsubscribePublishedFileResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_UnsubscribePublishedFile(unPublishedFileId.Value);
      return CallbackFunction == null ? null : RemoteStorageUnsubscribePublishedFileResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public bool UpdatePublishedFileDescription(
      PublishedFileUpdateHandle_t updateHandle,
      string pchDescription)
    {
      return platform.ISteamRemoteStorage_UpdatePublishedFileDescription(updateHandle.Value, pchDescription);
    }

    public bool UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile)
    {
      return platform.ISteamRemoteStorage_UpdatePublishedFileFile(updateHandle.Value, pchFile);
    }

    public bool UpdatePublishedFilePreviewFile(
      PublishedFileUpdateHandle_t updateHandle,
      string pchPreviewFile)
    {
      return platform.ISteamRemoteStorage_UpdatePublishedFilePreviewFile(updateHandle.Value, pchPreviewFile);
    }

    public bool UpdatePublishedFileSetChangeDescription(
      PublishedFileUpdateHandle_t updateHandle,
      string pchChangeDescription)
    {
      return platform.ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription(updateHandle.Value, pchChangeDescription);
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
        return platform.ISteamRemoteStorage_UpdatePublishedFileTags(updateHandle.Value, ref new SteamParamStringArray_t {
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
      return platform.ISteamRemoteStorage_UpdatePublishedFileTitle(updateHandle.Value, pchTitle);
    }

    public bool UpdatePublishedFileVisibility(
      PublishedFileUpdateHandle_t updateHandle,
      RemoteStoragePublishedFileVisibility eVisibility)
    {
      return platform.ISteamRemoteStorage_UpdatePublishedFileVisibility(updateHandle.Value, eVisibility);
    }

    public CallbackHandle UpdateUserPublishedItemVote(
      PublishedFileId_t unPublishedFileId,
      bool bVoteUp,
      Action<RemoteStorageUpdateUserPublishedItemVoteResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamRemoteStorage_UpdateUserPublishedItemVote(unPublishedFileId.Value, bVoteUp);
      return CallbackFunction == null ? null : RemoteStorageUpdateUserPublishedItemVoteResult_t.CallResult(steamworks, call, CallbackFunction);
    }
  }
}
