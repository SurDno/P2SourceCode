// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamUGC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace SteamNative
{
  internal class SteamUGC : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamUGC(BaseSteamworks steamworks, IntPtr pointer)
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

    public CallbackHandle AddDependency(
      PublishedFileId_t nParentPublishedFileID,
      PublishedFileId_t nChildPublishedFileID,
      Action<AddUGCDependencyResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUGC_AddDependency(nParentPublishedFileID.Value, nChildPublishedFileID.Value);
      return CallbackFunction == null ? (CallbackHandle) null : AddUGCDependencyResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool AddExcludedTag(UGCQueryHandle_t handle, string pTagName)
    {
      return this.platform.ISteamUGC_AddExcludedTag(handle.Value, pTagName);
    }

    public bool AddItemKeyValueTag(UGCUpdateHandle_t handle, string pchKey, string pchValue)
    {
      return this.platform.ISteamUGC_AddItemKeyValueTag(handle.Value, pchKey, pchValue);
    }

    public bool AddItemPreviewFile(
      UGCUpdateHandle_t handle,
      string pszPreviewFile,
      ItemPreviewType type)
    {
      return this.platform.ISteamUGC_AddItemPreviewFile(handle.Value, pszPreviewFile, type);
    }

    public bool AddItemPreviewVideo(UGCUpdateHandle_t handle, string pszVideoID)
    {
      return this.platform.ISteamUGC_AddItemPreviewVideo(handle.Value, pszVideoID);
    }

    public CallbackHandle AddItemToFavorites(
      AppId_t nAppId,
      PublishedFileId_t nPublishedFileID,
      Action<UserFavoriteItemsListChanged_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t favorites = this.platform.ISteamUGC_AddItemToFavorites(nAppId.Value, nPublishedFileID.Value);
      return CallbackFunction == null ? (CallbackHandle) null : UserFavoriteItemsListChanged_t.CallResult(this.steamworks, favorites, CallbackFunction);
    }

    public bool AddRequiredKeyValueTag(UGCQueryHandle_t handle, string pKey, string pValue)
    {
      return this.platform.ISteamUGC_AddRequiredKeyValueTag(handle.Value, pKey, pValue);
    }

    public bool AddRequiredTag(UGCQueryHandle_t handle, string pTagName)
    {
      return this.platform.ISteamUGC_AddRequiredTag(handle.Value, pTagName);
    }

    public bool BInitWorkshopForGameServer(DepotId_t unWorkshopDepotID, string pszFolder)
    {
      return this.platform.ISteamUGC_BInitWorkshopForGameServer(unWorkshopDepotID.Value, pszFolder);
    }

    public CallbackHandle CreateItem(
      AppId_t nConsumerAppId,
      WorkshopFileType eFileType,
      Action<CreateItemResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUGC_CreateItem(nConsumerAppId.Value, eFileType);
      return CallbackFunction == null ? (CallbackHandle) null : CreateItemResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public UGCQueryHandle_t CreateQueryAllUGCRequest(
      UGCQuery eQueryType,
      UGCMatchingUGCType eMatchingeMatchingUGCTypeFileType,
      AppId_t nCreatorAppID,
      AppId_t nConsumerAppID,
      uint unPage)
    {
      return this.platform.ISteamUGC_CreateQueryAllUGCRequest(eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID.Value, nConsumerAppID.Value, unPage);
    }

    public unsafe UGCQueryHandle_t CreateQueryUGCDetailsRequest(
      PublishedFileId_t[] pvecPublishedFileID)
    {
      uint length = (uint) pvecPublishedFileID.Length;
      fixed (PublishedFileId_t* pvecPublishedFileID1 = pvecPublishedFileID)
        return this.platform.ISteamUGC_CreateQueryUGCDetailsRequest((IntPtr) (void*) pvecPublishedFileID1, length);
    }

    public UGCQueryHandle_t CreateQueryUserUGCRequest(
      AccountID_t unAccountID,
      UserUGCList eListType,
      UGCMatchingUGCType eMatchingUGCType,
      UserUGCListSortOrder eSortOrder,
      AppId_t nCreatorAppID,
      AppId_t nConsumerAppID,
      uint unPage)
    {
      return this.platform.ISteamUGC_CreateQueryUserUGCRequest(unAccountID.Value, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID.Value, nConsumerAppID.Value, unPage);
    }

    public bool DownloadItem(PublishedFileId_t nPublishedFileID, bool bHighPriority)
    {
      return this.platform.ISteamUGC_DownloadItem(nPublishedFileID.Value, bHighPriority);
    }

    public bool GetItemDownloadInfo(
      PublishedFileId_t nPublishedFileID,
      out ulong punBytesDownloaded,
      out ulong punBytesTotal)
    {
      return this.platform.ISteamUGC_GetItemDownloadInfo(nPublishedFileID.Value, out punBytesDownloaded, out punBytesTotal);
    }

    public bool GetItemInstallInfo(
      PublishedFileId_t nPublishedFileID,
      out ulong punSizeOnDisk,
      out string pchFolder,
      out uint punTimeStamp)
    {
      pchFolder = string.Empty;
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      uint cchFolderSize = 4096;
      bool itemInstallInfo = this.platform.ISteamUGC_GetItemInstallInfo(nPublishedFileID.Value, out punSizeOnDisk, stringBuilder, cchFolderSize, out punTimeStamp);
      if (!itemInstallInfo)
        return itemInstallInfo;
      pchFolder = stringBuilder.ToString();
      return itemInstallInfo;
    }

    public uint GetItemState(PublishedFileId_t nPublishedFileID)
    {
      return this.platform.ISteamUGC_GetItemState(nPublishedFileID.Value);
    }

    public ItemUpdateStatus GetItemUpdateProgress(
      UGCUpdateHandle_t handle,
      out ulong punBytesProcessed,
      out ulong punBytesTotal)
    {
      return this.platform.ISteamUGC_GetItemUpdateProgress(handle.Value, out punBytesProcessed, out punBytesTotal);
    }

    public uint GetNumSubscribedItems() => this.platform.ISteamUGC_GetNumSubscribedItems();

    public bool GetQueryUGCAdditionalPreview(
      UGCQueryHandle_t handle,
      uint index,
      uint previewIndex,
      out string pchURLOrVideoID,
      out string pchOriginalFileName,
      out ItemPreviewType pPreviewType)
    {
      pchURLOrVideoID = string.Empty;
      StringBuilder stringBuilder1 = Helpers.TakeStringBuilder();
      uint cchURLSize = 4096;
      pchOriginalFileName = string.Empty;
      StringBuilder stringBuilder2 = Helpers.TakeStringBuilder();
      uint cchOriginalFileNameSize = 4096;
      bool additionalPreview = this.platform.ISteamUGC_GetQueryUGCAdditionalPreview(handle.Value, index, previewIndex, stringBuilder1, cchURLSize, stringBuilder2, cchOriginalFileNameSize, out pPreviewType);
      if (!additionalPreview)
        return additionalPreview;
      pchOriginalFileName = stringBuilder2.ToString();
      if (!additionalPreview)
        return additionalPreview;
      pchURLOrVideoID = stringBuilder1.ToString();
      return additionalPreview;
    }

    public unsafe bool GetQueryUGCChildren(
      UGCQueryHandle_t handle,
      uint index,
      PublishedFileId_t* pvecPublishedFileID,
      uint cMaxEntries)
    {
      return this.platform.ISteamUGC_GetQueryUGCChildren(handle.Value, index, (IntPtr) (void*) pvecPublishedFileID, cMaxEntries);
    }

    public bool GetQueryUGCKeyValueTag(
      UGCQueryHandle_t handle,
      uint index,
      uint keyValueTagIndex,
      out string pchKey,
      out string pchValue)
    {
      pchKey = string.Empty;
      StringBuilder stringBuilder1 = Helpers.TakeStringBuilder();
      uint cchKeySize = 4096;
      pchValue = string.Empty;
      StringBuilder stringBuilder2 = Helpers.TakeStringBuilder();
      uint cchValueSize = 4096;
      bool queryUgcKeyValueTag = this.platform.ISteamUGC_GetQueryUGCKeyValueTag(handle.Value, index, keyValueTagIndex, stringBuilder1, cchKeySize, stringBuilder2, cchValueSize);
      if (!queryUgcKeyValueTag)
        return queryUgcKeyValueTag;
      pchValue = stringBuilder2.ToString();
      if (!queryUgcKeyValueTag)
        return queryUgcKeyValueTag;
      pchKey = stringBuilder1.ToString();
      return queryUgcKeyValueTag;
    }

    public bool GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, out string pchMetadata)
    {
      pchMetadata = string.Empty;
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      uint cchMetadatasize = 4096;
      bool queryUgcMetadata = this.platform.ISteamUGC_GetQueryUGCMetadata(handle.Value, index, stringBuilder, cchMetadatasize);
      if (!queryUgcMetadata)
        return queryUgcMetadata;
      pchMetadata = stringBuilder.ToString();
      return queryUgcMetadata;
    }

    public uint GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index)
    {
      return this.platform.ISteamUGC_GetQueryUGCNumAdditionalPreviews(handle.Value, index);
    }

    public uint GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index)
    {
      return this.platform.ISteamUGC_GetQueryUGCNumKeyValueTags(handle.Value, index);
    }

    public bool GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, out string pchURL)
    {
      pchURL = string.Empty;
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      uint cchURLSize = 4096;
      bool queryUgcPreviewUrl = this.platform.ISteamUGC_GetQueryUGCPreviewURL(handle.Value, index, stringBuilder, cchURLSize);
      if (!queryUgcPreviewUrl)
        return queryUgcPreviewUrl;
      pchURL = stringBuilder.ToString();
      return queryUgcPreviewUrl;
    }

    public bool GetQueryUGCResult(
      UGCQueryHandle_t handle,
      uint index,
      ref SteamUGCDetails_t pDetails)
    {
      return this.platform.ISteamUGC_GetQueryUGCResult(handle.Value, index, ref pDetails);
    }

    public bool GetQueryUGCStatistic(
      UGCQueryHandle_t handle,
      uint index,
      ItemStatistic eStatType,
      out ulong pStatValue)
    {
      return this.platform.ISteamUGC_GetQueryUGCStatistic(handle.Value, index, eStatType, out pStatValue);
    }

    public unsafe uint GetSubscribedItems(PublishedFileId_t* pvecPublishedFileID, uint cMaxEntries)
    {
      return this.platform.ISteamUGC_GetSubscribedItems((IntPtr) (void*) pvecPublishedFileID, cMaxEntries);
    }

    public CallbackHandle GetUserItemVote(
      PublishedFileId_t nPublishedFileID,
      Action<GetUserItemVoteResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t userItemVote = this.platform.ISteamUGC_GetUserItemVote(nPublishedFileID.Value);
      return CallbackFunction == null ? (CallbackHandle) null : GetUserItemVoteResult_t.CallResult(this.steamworks, userItemVote, CallbackFunction);
    }

    public bool ReleaseQueryUGCRequest(UGCQueryHandle_t handle)
    {
      return this.platform.ISteamUGC_ReleaseQueryUGCRequest(handle.Value);
    }

    public CallbackHandle RemoveDependency(
      PublishedFileId_t nParentPublishedFileID,
      PublishedFileId_t nChildPublishedFileID,
      Action<RemoveUGCDependencyResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUGC_RemoveDependency(nParentPublishedFileID.Value, nChildPublishedFileID.Value);
      return CallbackFunction == null ? (CallbackHandle) null : RemoveUGCDependencyResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle RemoveItemFromFavorites(
      AppId_t nAppId,
      PublishedFileId_t nPublishedFileID,
      Action<UserFavoriteItemsListChanged_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUGC_RemoveItemFromFavorites(nAppId.Value, nPublishedFileID.Value);
      return CallbackFunction == null ? (CallbackHandle) null : UserFavoriteItemsListChanged_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool RemoveItemKeyValueTags(UGCUpdateHandle_t handle, string pchKey)
    {
      return this.platform.ISteamUGC_RemoveItemKeyValueTags(handle.Value, pchKey);
    }

    public bool RemoveItemPreview(UGCUpdateHandle_t handle, uint index)
    {
      return this.platform.ISteamUGC_RemoveItemPreview(handle.Value, index);
    }

    public SteamAPICall_t RequestUGCDetails(
      PublishedFileId_t nPublishedFileID,
      uint unMaxAgeSeconds)
    {
      return this.platform.ISteamUGC_RequestUGCDetails(nPublishedFileID.Value, unMaxAgeSeconds);
    }

    public CallbackHandle SendQueryUGCRequest(
      UGCQueryHandle_t handle,
      Action<SteamUGCQueryCompleted_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUGC_SendQueryUGCRequest(handle.Value);
      return CallbackFunction == null ? (CallbackHandle) null : SteamUGCQueryCompleted_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds)
    {
      return this.platform.ISteamUGC_SetAllowCachedResponse(handle.Value, unMaxAgeSeconds);
    }

    public bool SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName)
    {
      return this.platform.ISteamUGC_SetCloudFileNameFilter(handle.Value, pMatchCloudFileName);
    }

    public bool SetItemContent(UGCUpdateHandle_t handle, string pszContentFolder)
    {
      return this.platform.ISteamUGC_SetItemContent(handle.Value, pszContentFolder);
    }

    public bool SetItemDescription(UGCUpdateHandle_t handle, string pchDescription)
    {
      return this.platform.ISteamUGC_SetItemDescription(handle.Value, pchDescription);
    }

    public bool SetItemMetadata(UGCUpdateHandle_t handle, string pchMetaData)
    {
      return this.platform.ISteamUGC_SetItemMetadata(handle.Value, pchMetaData);
    }

    public bool SetItemPreview(UGCUpdateHandle_t handle, string pszPreviewFile)
    {
      return this.platform.ISteamUGC_SetItemPreview(handle.Value, pszPreviewFile);
    }

    public bool SetItemTags(UGCUpdateHandle_t updateHandle, string[] pTags)
    {
      IntPtr[] source = new IntPtr[pTags.Length];
      for (int index = 0; index < pTags.Length; ++index)
        source[index] = Marshal.StringToHGlobalAnsi(pTags[index]);
      try
      {
        IntPtr destination = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * source.Length);
        Marshal.Copy(source, 0, destination, source.Length);
        return this.platform.ISteamUGC_SetItemTags(updateHandle.Value, ref new SteamParamStringArray_t()
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

    public bool SetItemTitle(UGCUpdateHandle_t handle, string pchTitle)
    {
      return this.platform.ISteamUGC_SetItemTitle(handle.Value, pchTitle);
    }

    public bool SetItemUpdateLanguage(UGCUpdateHandle_t handle, string pchLanguage)
    {
      return this.platform.ISteamUGC_SetItemUpdateLanguage(handle.Value, pchLanguage);
    }

    public bool SetItemVisibility(
      UGCUpdateHandle_t handle,
      RemoteStoragePublishedFileVisibility eVisibility)
    {
      return this.platform.ISteamUGC_SetItemVisibility(handle.Value, eVisibility);
    }

    public bool SetLanguage(UGCQueryHandle_t handle, string pchLanguage)
    {
      return this.platform.ISteamUGC_SetLanguage(handle.Value, pchLanguage);
    }

    public bool SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag)
    {
      return this.platform.ISteamUGC_SetMatchAnyTag(handle.Value, bMatchAnyTag);
    }

    public bool SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays)
    {
      return this.platform.ISteamUGC_SetRankedByTrendDays(handle.Value, unDays);
    }

    public bool SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews)
    {
      return this.platform.ISteamUGC_SetReturnAdditionalPreviews(handle.Value, bReturnAdditionalPreviews);
    }

    public bool SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren)
    {
      return this.platform.ISteamUGC_SetReturnChildren(handle.Value, bReturnChildren);
    }

    public bool SetReturnKeyValueTags(UGCQueryHandle_t handle, bool bReturnKeyValueTags)
    {
      return this.platform.ISteamUGC_SetReturnKeyValueTags(handle.Value, bReturnKeyValueTags);
    }

    public bool SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription)
    {
      return this.platform.ISteamUGC_SetReturnLongDescription(handle.Value, bReturnLongDescription);
    }

    public bool SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata)
    {
      return this.platform.ISteamUGC_SetReturnMetadata(handle.Value, bReturnMetadata);
    }

    public bool SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs)
    {
      return this.platform.ISteamUGC_SetReturnOnlyIDs(handle.Value, bReturnOnlyIDs);
    }

    public bool SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays)
    {
      return this.platform.ISteamUGC_SetReturnPlaytimeStats(handle.Value, unDays);
    }

    public bool SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly)
    {
      return this.platform.ISteamUGC_SetReturnTotalOnly(handle.Value, bReturnTotalOnly);
    }

    public bool SetSearchText(UGCQueryHandle_t handle, string pSearchText)
    {
      return this.platform.ISteamUGC_SetSearchText(handle.Value, pSearchText);
    }

    public CallbackHandle SetUserItemVote(
      PublishedFileId_t nPublishedFileID,
      bool bVoteUp,
      Action<SetUserItemVoteResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUGC_SetUserItemVote(nPublishedFileID.Value, bVoteUp);
      return CallbackFunction == null ? (CallbackHandle) null : SetUserItemVoteResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public UGCUpdateHandle_t StartItemUpdate(
      AppId_t nConsumerAppId,
      PublishedFileId_t nPublishedFileID)
    {
      return this.platform.ISteamUGC_StartItemUpdate(nConsumerAppId.Value, nPublishedFileID.Value);
    }

    public unsafe CallbackHandle StartPlaytimeTracking(
      PublishedFileId_t[] pvecPublishedFileID,
      Action<StartPlaytimeTrackingResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      uint length = (uint) pvecPublishedFileID.Length;
      SteamAPICall_t call;
      fixed (PublishedFileId_t* pvecPublishedFileID1 = pvecPublishedFileID)
        call = this.platform.ISteamUGC_StartPlaytimeTracking((IntPtr) (void*) pvecPublishedFileID1, length);
      return CallbackFunction == null ? (CallbackHandle) null : StartPlaytimeTrackingResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public unsafe CallbackHandle StopPlaytimeTracking(
      PublishedFileId_t[] pvecPublishedFileID,
      Action<StopPlaytimeTrackingResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      uint length = (uint) pvecPublishedFileID.Length;
      SteamAPICall_t call;
      fixed (PublishedFileId_t* pvecPublishedFileID1 = pvecPublishedFileID)
        call = this.platform.ISteamUGC_StopPlaytimeTracking((IntPtr) (void*) pvecPublishedFileID1, length);
      return CallbackFunction == null ? (CallbackHandle) null : StopPlaytimeTrackingResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle StopPlaytimeTrackingForAllItems(
      Action<StopPlaytimeTrackingResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUGC_StopPlaytimeTrackingForAllItems();
      return CallbackFunction == null ? (CallbackHandle) null : StopPlaytimeTrackingResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle SubmitItemUpdate(
      UGCUpdateHandle_t handle,
      string pchChangeNote,
      Action<SubmitItemUpdateResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUGC_SubmitItemUpdate(handle.Value, pchChangeNote);
      return CallbackFunction == null ? (CallbackHandle) null : SubmitItemUpdateResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle SubscribeItem(
      PublishedFileId_t nPublishedFileID,
      Action<RemoteStorageSubscribePublishedFileResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUGC_SubscribeItem(nPublishedFileID.Value);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageSubscribePublishedFileResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public void SuspendDownloads(bool bSuspend)
    {
      this.platform.ISteamUGC_SuspendDownloads(bSuspend);
    }

    public CallbackHandle UnsubscribeItem(
      PublishedFileId_t nPublishedFileID,
      Action<RemoteStorageUnsubscribePublishedFileResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUGC_UnsubscribeItem(nPublishedFileID.Value);
      return CallbackFunction == null ? (CallbackHandle) null : RemoteStorageUnsubscribePublishedFileResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, string pszPreviewFile)
    {
      return this.platform.ISteamUGC_UpdateItemPreviewFile(handle.Value, index, pszPreviewFile);
    }

    public bool UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, string pszVideoID)
    {
      return this.platform.ISteamUGC_UpdateItemPreviewVideo(handle.Value, index, pszVideoID);
    }
  }
}
