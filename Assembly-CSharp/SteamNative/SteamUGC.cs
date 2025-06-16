using System;
using System.Runtime.InteropServices;
using System.Text;
using Facepunch.Steamworks;

namespace SteamNative;

internal class SteamUGC : IDisposable {
	internal Platform.Interface platform;
	internal BaseSteamworks steamworks;

	internal SteamUGC(BaseSteamworks steamworks, IntPtr pointer) {
		this.steamworks = steamworks;
		if (Platform.IsWindows64)
			platform = (Platform.Interface)new Platform.Win64(pointer);
		else if (Platform.IsWindows32)
			platform = (Platform.Interface)new Platform.Win32(pointer);
		else if (Platform.IsLinux32)
			platform = (Platform.Interface)new Platform.Linux32(pointer);
		else if (Platform.IsLinux64)
			platform = (Platform.Interface)new Platform.Linux64(pointer);
		else {
			if (!Platform.IsOsx)
				return;
			platform = (Platform.Interface)new Platform.Mac(pointer);
		}
	}

	public bool IsValid => platform != null && platform.IsValid;

	public virtual void Dispose() {
		if (platform == null)
			return;
		platform.Dispose();
		platform = (Platform.Interface)null;
	}

	public CallbackHandle AddDependency(
		PublishedFileId_t nParentPublishedFileID,
		PublishedFileId_t nChildPublishedFileID,
		Action<AddUGCDependencyResult_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUGC_AddDependency(nParentPublishedFileID.Value, nChildPublishedFileID.Value);
		return CallbackFunction == null
			? null
			: AddUGCDependencyResult_t.CallResult(steamworks, call, CallbackFunction);
	}

	public bool AddExcludedTag(UGCQueryHandle_t handle, string pTagName) {
		return platform.ISteamUGC_AddExcludedTag(handle.Value, pTagName);
	}

	public bool AddItemKeyValueTag(UGCUpdateHandle_t handle, string pchKey, string pchValue) {
		return platform.ISteamUGC_AddItemKeyValueTag(handle.Value, pchKey, pchValue);
	}

	public bool AddItemPreviewFile(
		UGCUpdateHandle_t handle,
		string pszPreviewFile,
		ItemPreviewType type) {
		return platform.ISteamUGC_AddItemPreviewFile(handle.Value, pszPreviewFile, type);
	}

	public bool AddItemPreviewVideo(UGCUpdateHandle_t handle, string pszVideoID) {
		return platform.ISteamUGC_AddItemPreviewVideo(handle.Value, pszVideoID);
	}

	public CallbackHandle AddItemToFavorites(
		AppId_t nAppId,
		PublishedFileId_t nPublishedFileID,
		Action<UserFavoriteItemsListChanged_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var favorites = platform.ISteamUGC_AddItemToFavorites(nAppId.Value, nPublishedFileID.Value);
		return CallbackFunction == null
			? null
			: UserFavoriteItemsListChanged_t.CallResult(steamworks, favorites, CallbackFunction);
	}

	public bool AddRequiredKeyValueTag(UGCQueryHandle_t handle, string pKey, string pValue) {
		return platform.ISteamUGC_AddRequiredKeyValueTag(handle.Value, pKey, pValue);
	}

	public bool AddRequiredTag(UGCQueryHandle_t handle, string pTagName) {
		return platform.ISteamUGC_AddRequiredTag(handle.Value, pTagName);
	}

	public bool BInitWorkshopForGameServer(DepotId_t unWorkshopDepotID, string pszFolder) {
		return platform.ISteamUGC_BInitWorkshopForGameServer(unWorkshopDepotID.Value, pszFolder);
	}

	public CallbackHandle CreateItem(
		AppId_t nConsumerAppId,
		WorkshopFileType eFileType,
		Action<CreateItemResult_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUGC_CreateItem(nConsumerAppId.Value, eFileType);
		return CallbackFunction == null ? null : CreateItemResult_t.CallResult(steamworks, call, CallbackFunction);
	}

	public UGCQueryHandle_t CreateQueryAllUGCRequest(
		UGCQuery eQueryType,
		UGCMatchingUGCType eMatchingeMatchingUGCTypeFileType,
		AppId_t nCreatorAppID,
		AppId_t nConsumerAppID,
		uint unPage) {
		return platform.ISteamUGC_CreateQueryAllUGCRequest(eQueryType, eMatchingeMatchingUGCTypeFileType,
			nCreatorAppID.Value, nConsumerAppID.Value, unPage);
	}

	public unsafe UGCQueryHandle_t CreateQueryUGCDetailsRequest(
		PublishedFileId_t[] pvecPublishedFileID) {
		var length = (uint)pvecPublishedFileID.Length;
		fixed (PublishedFileId_t* pvecPublishedFileID1 = pvecPublishedFileID) {
			return platform.ISteamUGC_CreateQueryUGCDetailsRequest((IntPtr)pvecPublishedFileID1, length);
		}
	}

	public UGCQueryHandle_t CreateQueryUserUGCRequest(
		AccountID_t unAccountID,
		UserUGCList eListType,
		UGCMatchingUGCType eMatchingUGCType,
		UserUGCListSortOrder eSortOrder,
		AppId_t nCreatorAppID,
		AppId_t nConsumerAppID,
		uint unPage) {
		return platform.ISteamUGC_CreateQueryUserUGCRequest(unAccountID.Value, eListType, eMatchingUGCType, eSortOrder,
			nCreatorAppID.Value, nConsumerAppID.Value, unPage);
	}

	public bool DownloadItem(PublishedFileId_t nPublishedFileID, bool bHighPriority) {
		return platform.ISteamUGC_DownloadItem(nPublishedFileID.Value, bHighPriority);
	}

	public bool GetItemDownloadInfo(
		PublishedFileId_t nPublishedFileID,
		out ulong punBytesDownloaded,
		out ulong punBytesTotal) {
		return platform.ISteamUGC_GetItemDownloadInfo(nPublishedFileID.Value, out punBytesDownloaded,
			out punBytesTotal);
	}

	public bool GetItemInstallInfo(
		PublishedFileId_t nPublishedFileID,
		out ulong punSizeOnDisk,
		out string pchFolder,
		out uint punTimeStamp) {
		pchFolder = string.Empty;
		var stringBuilder = Helpers.TakeStringBuilder();
		uint cchFolderSize = 4096;
		var itemInstallInfo = platform.ISteamUGC_GetItemInstallInfo(nPublishedFileID.Value, out punSizeOnDisk,
			stringBuilder, cchFolderSize, out punTimeStamp);
		if (!itemInstallInfo)
			return itemInstallInfo;
		pchFolder = stringBuilder.ToString();
		return itemInstallInfo;
	}

	public uint GetItemState(PublishedFileId_t nPublishedFileID) {
		return platform.ISteamUGC_GetItemState(nPublishedFileID.Value);
	}

	public ItemUpdateStatus GetItemUpdateProgress(
		UGCUpdateHandle_t handle,
		out ulong punBytesProcessed,
		out ulong punBytesTotal) {
		return platform.ISteamUGC_GetItemUpdateProgress(handle.Value, out punBytesProcessed, out punBytesTotal);
	}

	public uint GetNumSubscribedItems() {
		return platform.ISteamUGC_GetNumSubscribedItems();
	}

	public bool GetQueryUGCAdditionalPreview(
		UGCQueryHandle_t handle,
		uint index,
		uint previewIndex,
		out string pchURLOrVideoID,
		out string pchOriginalFileName,
		out ItemPreviewType pPreviewType) {
		pchURLOrVideoID = string.Empty;
		var stringBuilder1 = Helpers.TakeStringBuilder();
		uint cchURLSize = 4096;
		pchOriginalFileName = string.Empty;
		var stringBuilder2 = Helpers.TakeStringBuilder();
		uint cchOriginalFileNameSize = 4096;
		var additionalPreview = platform.ISteamUGC_GetQueryUGCAdditionalPreview(handle.Value, index, previewIndex,
			stringBuilder1, cchURLSize, stringBuilder2, cchOriginalFileNameSize, out pPreviewType);
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
		uint cMaxEntries) {
		return platform.ISteamUGC_GetQueryUGCChildren(handle.Value, index, (IntPtr)pvecPublishedFileID, cMaxEntries);
	}

	public bool GetQueryUGCKeyValueTag(
		UGCQueryHandle_t handle,
		uint index,
		uint keyValueTagIndex,
		out string pchKey,
		out string pchValue) {
		pchKey = string.Empty;
		var stringBuilder1 = Helpers.TakeStringBuilder();
		uint cchKeySize = 4096;
		pchValue = string.Empty;
		var stringBuilder2 = Helpers.TakeStringBuilder();
		uint cchValueSize = 4096;
		var queryUgcKeyValueTag = platform.ISteamUGC_GetQueryUGCKeyValueTag(handle.Value, index, keyValueTagIndex,
			stringBuilder1, cchKeySize, stringBuilder2, cchValueSize);
		if (!queryUgcKeyValueTag)
			return queryUgcKeyValueTag;
		pchValue = stringBuilder2.ToString();
		if (!queryUgcKeyValueTag)
			return queryUgcKeyValueTag;
		pchKey = stringBuilder1.ToString();
		return queryUgcKeyValueTag;
	}

	public bool GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, out string pchMetadata) {
		pchMetadata = string.Empty;
		var stringBuilder = Helpers.TakeStringBuilder();
		uint cchMetadatasize = 4096;
		var queryUgcMetadata =
			platform.ISteamUGC_GetQueryUGCMetadata(handle.Value, index, stringBuilder, cchMetadatasize);
		if (!queryUgcMetadata)
			return queryUgcMetadata;
		pchMetadata = stringBuilder.ToString();
		return queryUgcMetadata;
	}

	public uint GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index) {
		return platform.ISteamUGC_GetQueryUGCNumAdditionalPreviews(handle.Value, index);
	}

	public uint GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index) {
		return platform.ISteamUGC_GetQueryUGCNumKeyValueTags(handle.Value, index);
	}

	public bool GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, out string pchURL) {
		pchURL = string.Empty;
		var stringBuilder = Helpers.TakeStringBuilder();
		uint cchURLSize = 4096;
		var queryUgcPreviewUrl =
			platform.ISteamUGC_GetQueryUGCPreviewURL(handle.Value, index, stringBuilder, cchURLSize);
		if (!queryUgcPreviewUrl)
			return queryUgcPreviewUrl;
		pchURL = stringBuilder.ToString();
		return queryUgcPreviewUrl;
	}

	public bool GetQueryUGCResult(
		UGCQueryHandle_t handle,
		uint index,
		ref SteamUGCDetails_t pDetails) {
		return platform.ISteamUGC_GetQueryUGCResult(handle.Value, index, ref pDetails);
	}

	public bool GetQueryUGCStatistic(
		UGCQueryHandle_t handle,
		uint index,
		ItemStatistic eStatType,
		out ulong pStatValue) {
		return platform.ISteamUGC_GetQueryUGCStatistic(handle.Value, index, eStatType, out pStatValue);
	}

	public unsafe uint GetSubscribedItems(PublishedFileId_t* pvecPublishedFileID, uint cMaxEntries) {
		return platform.ISteamUGC_GetSubscribedItems((IntPtr)pvecPublishedFileID, cMaxEntries);
	}

	public CallbackHandle GetUserItemVote(
		PublishedFileId_t nPublishedFileID,
		Action<GetUserItemVoteResult_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var userItemVote = platform.ISteamUGC_GetUserItemVote(nPublishedFileID.Value);
		return CallbackFunction == null
			? null
			: GetUserItemVoteResult_t.CallResult(steamworks, userItemVote, CallbackFunction);
	}

	public bool ReleaseQueryUGCRequest(UGCQueryHandle_t handle) {
		return platform.ISteamUGC_ReleaseQueryUGCRequest(handle.Value);
	}

	public CallbackHandle RemoveDependency(
		PublishedFileId_t nParentPublishedFileID,
		PublishedFileId_t nChildPublishedFileID,
		Action<RemoveUGCDependencyResult_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUGC_RemoveDependency(nParentPublishedFileID.Value, nChildPublishedFileID.Value);
		return CallbackFunction == null
			? null
			: RemoveUGCDependencyResult_t.CallResult(steamworks, call, CallbackFunction);
	}

	public CallbackHandle RemoveItemFromFavorites(
		AppId_t nAppId,
		PublishedFileId_t nPublishedFileID,
		Action<UserFavoriteItemsListChanged_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUGC_RemoveItemFromFavorites(nAppId.Value, nPublishedFileID.Value);
		return CallbackFunction == null
			? null
			: UserFavoriteItemsListChanged_t.CallResult(steamworks, call, CallbackFunction);
	}

	public bool RemoveItemKeyValueTags(UGCUpdateHandle_t handle, string pchKey) {
		return platform.ISteamUGC_RemoveItemKeyValueTags(handle.Value, pchKey);
	}

	public bool RemoveItemPreview(UGCUpdateHandle_t handle, uint index) {
		return platform.ISteamUGC_RemoveItemPreview(handle.Value, index);
	}

	public SteamAPICall_t RequestUGCDetails(
		PublishedFileId_t nPublishedFileID,
		uint unMaxAgeSeconds) {
		return platform.ISteamUGC_RequestUGCDetails(nPublishedFileID.Value, unMaxAgeSeconds);
	}

	public CallbackHandle SendQueryUGCRequest(
		UGCQueryHandle_t handle,
		Action<SteamUGCQueryCompleted_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUGC_SendQueryUGCRequest(handle.Value);
		return CallbackFunction == null
			? null
			: SteamUGCQueryCompleted_t.CallResult(steamworks, call, CallbackFunction);
	}

	public bool SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds) {
		return platform.ISteamUGC_SetAllowCachedResponse(handle.Value, unMaxAgeSeconds);
	}

	public bool SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName) {
		return platform.ISteamUGC_SetCloudFileNameFilter(handle.Value, pMatchCloudFileName);
	}

	public bool SetItemContent(UGCUpdateHandle_t handle, string pszContentFolder) {
		return platform.ISteamUGC_SetItemContent(handle.Value, pszContentFolder);
	}

	public bool SetItemDescription(UGCUpdateHandle_t handle, string pchDescription) {
		return platform.ISteamUGC_SetItemDescription(handle.Value, pchDescription);
	}

	public bool SetItemMetadata(UGCUpdateHandle_t handle, string pchMetaData) {
		return platform.ISteamUGC_SetItemMetadata(handle.Value, pchMetaData);
	}

	public bool SetItemPreview(UGCUpdateHandle_t handle, string pszPreviewFile) {
		return platform.ISteamUGC_SetItemPreview(handle.Value, pszPreviewFile);
	}

	public bool SetItemTags(UGCUpdateHandle_t updateHandle, string[] pTags) {
		var source = new IntPtr[pTags.Length];
		for (var index = 0; index < pTags.Length; ++index)
			source[index] = Marshal.StringToHGlobalAnsi(pTags[index]);
		try {
			var destination = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * source.Length);
			Marshal.Copy(source, 0, destination, source.Length);
			var steamParamStringArray = new SteamParamStringArray_t {
				Strings = destination,
				NumStrings = pTags.Length
			};
			return platform.ISteamUGC_SetItemTags(updateHandle.Value, ref steamParamStringArray);
		} finally {
			foreach (var hglobal in source)
				Marshal.FreeHGlobal(hglobal);
		}
	}

	public bool SetItemTitle(UGCUpdateHandle_t handle, string pchTitle) {
		return platform.ISteamUGC_SetItemTitle(handle.Value, pchTitle);
	}

	public bool SetItemUpdateLanguage(UGCUpdateHandle_t handle, string pchLanguage) {
		return platform.ISteamUGC_SetItemUpdateLanguage(handle.Value, pchLanguage);
	}

	public bool SetItemVisibility(
		UGCUpdateHandle_t handle,
		RemoteStoragePublishedFileVisibility eVisibility) {
		return platform.ISteamUGC_SetItemVisibility(handle.Value, eVisibility);
	}

	public bool SetLanguage(UGCQueryHandle_t handle, string pchLanguage) {
		return platform.ISteamUGC_SetLanguage(handle.Value, pchLanguage);
	}

	public bool SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag) {
		return platform.ISteamUGC_SetMatchAnyTag(handle.Value, bMatchAnyTag);
	}

	public bool SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays) {
		return platform.ISteamUGC_SetRankedByTrendDays(handle.Value, unDays);
	}

	public bool SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews) {
		return platform.ISteamUGC_SetReturnAdditionalPreviews(handle.Value, bReturnAdditionalPreviews);
	}

	public bool SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren) {
		return platform.ISteamUGC_SetReturnChildren(handle.Value, bReturnChildren);
	}

	public bool SetReturnKeyValueTags(UGCQueryHandle_t handle, bool bReturnKeyValueTags) {
		return platform.ISteamUGC_SetReturnKeyValueTags(handle.Value, bReturnKeyValueTags);
	}

	public bool SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription) {
		return platform.ISteamUGC_SetReturnLongDescription(handle.Value, bReturnLongDescription);
	}

	public bool SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata) {
		return platform.ISteamUGC_SetReturnMetadata(handle.Value, bReturnMetadata);
	}

	public bool SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs) {
		return platform.ISteamUGC_SetReturnOnlyIDs(handle.Value, bReturnOnlyIDs);
	}

	public bool SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays) {
		return platform.ISteamUGC_SetReturnPlaytimeStats(handle.Value, unDays);
	}

	public bool SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly) {
		return platform.ISteamUGC_SetReturnTotalOnly(handle.Value, bReturnTotalOnly);
	}

	public bool SetSearchText(UGCQueryHandle_t handle, string pSearchText) {
		return platform.ISteamUGC_SetSearchText(handle.Value, pSearchText);
	}

	public CallbackHandle SetUserItemVote(
		PublishedFileId_t nPublishedFileID,
		bool bVoteUp,
		Action<SetUserItemVoteResult_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUGC_SetUserItemVote(nPublishedFileID.Value, bVoteUp);
		return CallbackFunction == null ? null : SetUserItemVoteResult_t.CallResult(steamworks, call, CallbackFunction);
	}

	public UGCUpdateHandle_t StartItemUpdate(
		AppId_t nConsumerAppId,
		PublishedFileId_t nPublishedFileID) {
		return platform.ISteamUGC_StartItemUpdate(nConsumerAppId.Value, nPublishedFileID.Value);
	}

	public unsafe CallbackHandle StartPlaytimeTracking(
		PublishedFileId_t[] pvecPublishedFileID,
		Action<StartPlaytimeTrackingResult_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var length = (uint)pvecPublishedFileID.Length;
		SteamAPICall_t call;
		fixed (PublishedFileId_t* pvecPublishedFileID1 = pvecPublishedFileID) {
			call = platform.ISteamUGC_StartPlaytimeTracking((IntPtr)pvecPublishedFileID1, length);
		}

		return CallbackFunction == null
			? null
			: StartPlaytimeTrackingResult_t.CallResult(steamworks, call, CallbackFunction);
	}

	public unsafe CallbackHandle StopPlaytimeTracking(
		PublishedFileId_t[] pvecPublishedFileID,
		Action<StopPlaytimeTrackingResult_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var length = (uint)pvecPublishedFileID.Length;
		SteamAPICall_t call;
		fixed (PublishedFileId_t* pvecPublishedFileID1 = pvecPublishedFileID) {
			call = platform.ISteamUGC_StopPlaytimeTracking((IntPtr)pvecPublishedFileID1, length);
		}

		return CallbackFunction == null
			? null
			: StopPlaytimeTrackingResult_t.CallResult(steamworks, call, CallbackFunction);
	}

	public CallbackHandle StopPlaytimeTrackingForAllItems(
		Action<StopPlaytimeTrackingResult_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUGC_StopPlaytimeTrackingForAllItems();
		return CallbackFunction == null
			? null
			: StopPlaytimeTrackingResult_t.CallResult(steamworks, call, CallbackFunction);
	}

	public CallbackHandle SubmitItemUpdate(
		UGCUpdateHandle_t handle,
		string pchChangeNote,
		Action<SubmitItemUpdateResult_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUGC_SubmitItemUpdate(handle.Value, pchChangeNote);
		return CallbackFunction == null
			? null
			: SubmitItemUpdateResult_t.CallResult(steamworks, call, CallbackFunction);
	}

	public CallbackHandle SubscribeItem(
		PublishedFileId_t nPublishedFileID,
		Action<RemoteStorageSubscribePublishedFileResult_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUGC_SubscribeItem(nPublishedFileID.Value);
		return CallbackFunction == null
			? null
			: RemoteStorageSubscribePublishedFileResult_t.CallResult(steamworks, call, CallbackFunction);
	}

	public void SuspendDownloads(bool bSuspend) {
		platform.ISteamUGC_SuspendDownloads(bSuspend);
	}

	public CallbackHandle UnsubscribeItem(
		PublishedFileId_t nPublishedFileID,
		Action<RemoteStorageUnsubscribePublishedFileResult_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUGC_UnsubscribeItem(nPublishedFileID.Value);
		return CallbackFunction == null
			? null
			: RemoteStorageUnsubscribePublishedFileResult_t.CallResult(steamworks, call, CallbackFunction);
	}

	public bool UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, string pszPreviewFile) {
		return platform.ISteamUGC_UpdateItemPreviewFile(handle.Value, index, pszPreviewFile);
	}

	public bool UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, string pszVideoID) {
		return platform.ISteamUGC_UpdateItemPreviewVideo(handle.Value, index, pszVideoID);
	}
}