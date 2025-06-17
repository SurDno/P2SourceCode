using System;
using System.Linq;
using System.Text;
using Facepunch.Steamworks;

namespace SteamNative
{
  internal class SteamInventory : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamInventory(BaseSteamworks steamworks, IntPtr pointer)
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

    public bool AddPromoItem(ref SteamInventoryResult_t pResultHandle, SteamItemDef_t itemDef)
    {
      return platform.ISteamInventory_AddPromoItem(ref pResultHandle.Value, itemDef.Value);
    }

    public bool AddPromoItems(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemDef_t[] pArrayItemDefs,
      uint unArrayLength)
    {
      return platform.ISteamInventory_AddPromoItems(ref pResultHandle.Value, pArrayItemDefs.Select(x => x.Value).ToArray(), unArrayLength);
    }

    public bool CheckResultSteamID(SteamInventoryResult_t resultHandle, CSteamID steamIDExpected)
    {
      return platform.ISteamInventory_CheckResultSteamID(resultHandle.Value, steamIDExpected.Value);
    }

    public bool ConsumeItem(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemInstanceID_t itemConsume,
      uint unQuantity)
    {
      return platform.ISteamInventory_ConsumeItem(ref pResultHandle.Value, itemConsume.Value, unQuantity);
    }

    public bool DeserializeResult(
      ref SteamInventoryResult_t pOutResultHandle,
      IntPtr pBuffer,
      uint unBufferSize,
      bool bRESERVED_MUST_BE_FALSE)
    {
      return platform.ISteamInventory_DeserializeResult(ref pOutResultHandle.Value, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
    }

    public void DestroyResult(SteamInventoryResult_t resultHandle)
    {
      platform.ISteamInventory_DestroyResult(resultHandle.Value);
    }

    public bool ExchangeItems(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemDef_t[] pArrayGenerate,
      uint[] punArrayGenerateQuantity,
      uint unArrayGenerateLength,
      SteamItemInstanceID_t[] pArrayDestroy,
      uint[] punArrayDestroyQuantity,
      uint unArrayDestroyLength)
    {
      return platform.ISteamInventory_ExchangeItems(ref pResultHandle.Value, pArrayGenerate.Select(x => x.Value).ToArray(), punArrayGenerateQuantity, unArrayGenerateLength, pArrayDestroy.Select(x => x.Value).ToArray(), punArrayDestroyQuantity, unArrayDestroyLength);
    }

    public bool GenerateItems(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemDef_t[] pArrayItemDefs,
      uint[] punArrayQuantity,
      uint unArrayLength)
    {
      return platform.ISteamInventory_GenerateItems(ref pResultHandle.Value, pArrayItemDefs.Select(x => x.Value).ToArray(), punArrayQuantity, unArrayLength);
    }

    public bool GetAllItems(ref SteamInventoryResult_t pResultHandle)
    {
      return platform.ISteamInventory_GetAllItems(ref pResultHandle.Value);
    }

    public unsafe SteamItemDef_t[] GetEligiblePromoItemDefinitionIDs(CSteamID steamID)
    {
      if (!platform.ISteamInventory_GetEligiblePromoItemDefinitionIDs(steamID.Value, IntPtr.Zero, out uint punItemDefIDsArraySize) || punItemDefIDsArraySize == 0U)
        return null;
      SteamItemDef_t[] steamItemDefTArray = new SteamItemDef_t[(int) punItemDefIDsArraySize];
      fixed (SteamItemDef_t* pItemDefIDs = steamItemDefTArray)
        return !platform.ISteamInventory_GetEligiblePromoItemDefinitionIDs(steamID.Value, (IntPtr) pItemDefIDs, out punItemDefIDsArraySize) ? null : steamItemDefTArray;
    }

    public unsafe SteamItemDef_t[] GetItemDefinitionIDs()
    {
      if (!platform.ISteamInventory_GetItemDefinitionIDs(IntPtr.Zero, out uint punItemDefIDsArraySize) || punItemDefIDsArraySize == 0U)
        return null;
      SteamItemDef_t[] steamItemDefTArray = new SteamItemDef_t[(int) punItemDefIDsArraySize];
      fixed (SteamItemDef_t* pItemDefIDs = steamItemDefTArray)
        return !platform.ISteamInventory_GetItemDefinitionIDs((IntPtr) pItemDefIDs, out punItemDefIDsArraySize) ? null : steamItemDefTArray;
    }

    public bool GetItemDefinitionProperty(
      SteamItemDef_t iDefinition,
      string pchPropertyName,
      out string pchValueBuffer)
    {
      pchValueBuffer = string.Empty;
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      uint punValueBufferSizeOut = 4096;
      bool definitionProperty = platform.ISteamInventory_GetItemDefinitionProperty(iDefinition.Value, pchPropertyName, stringBuilder, out punValueBufferSizeOut);
      if (!definitionProperty)
        return definitionProperty;
      pchValueBuffer = stringBuilder.ToString();
      return definitionProperty;
    }

    public bool GetItemsByID(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemInstanceID_t[] pInstanceIDs,
      uint unCountInstanceIDs)
    {
      return platform.ISteamInventory_GetItemsByID(ref pResultHandle.Value, pInstanceIDs.Select(x => x.Value).ToArray(), unCountInstanceIDs);
    }

    public bool GetResultItemProperty(
      SteamInventoryResult_t resultHandle,
      uint unItemIndex,
      string pchPropertyName,
      out string pchValueBuffer)
    {
      pchValueBuffer = string.Empty;
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      uint punValueBufferSizeOut = 4096;
      bool resultItemProperty = platform.ISteamInventory_GetResultItemProperty(resultHandle.Value, unItemIndex, pchPropertyName, stringBuilder, out punValueBufferSizeOut);
      if (!resultItemProperty)
        return resultItemProperty;
      pchValueBuffer = stringBuilder.ToString();
      return resultItemProperty;
    }

    public unsafe SteamItemDetails_t[] GetResultItems(SteamInventoryResult_t resultHandle)
    {
      if (!platform.ISteamInventory_GetResultItems(resultHandle.Value, IntPtr.Zero, out uint punOutItemsArraySize) || punOutItemsArraySize == 0U)
        return null;
      SteamItemDetails_t[] steamItemDetailsTArray = new SteamItemDetails_t[(int) punOutItemsArraySize];
      fixed (SteamItemDetails_t* pOutItemsArray = steamItemDetailsTArray)
        return !platform.ISteamInventory_GetResultItems(resultHandle.Value, (IntPtr) pOutItemsArray, out punOutItemsArraySize) ? null : steamItemDetailsTArray;
    }

    public Result GetResultStatus(SteamInventoryResult_t resultHandle)
    {
      return platform.ISteamInventory_GetResultStatus(resultHandle.Value);
    }

    public uint GetResultTimestamp(SteamInventoryResult_t resultHandle)
    {
      return platform.ISteamInventory_GetResultTimestamp(resultHandle.Value);
    }

    public bool GrantPromoItems(ref SteamInventoryResult_t pResultHandle)
    {
      return platform.ISteamInventory_GrantPromoItems(ref pResultHandle.Value);
    }

    public bool LoadItemDefinitions() => platform.ISteamInventory_LoadItemDefinitions();

    public CallbackHandle RequestEligiblePromoItemDefinitionsIDs(
      CSteamID steamID,
      Action<SteamInventoryEligiblePromoItemDefIDs_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamInventory_RequestEligiblePromoItemDefinitionsIDs(steamID.Value);
      return CallbackFunction == null ? null : SteamInventoryEligiblePromoItemDefIDs_t.CallResult(steamworks, call, CallbackFunction);
    }

    public void SendItemDropHeartbeat() => platform.ISteamInventory_SendItemDropHeartbeat();

    public bool SerializeResult(
      SteamInventoryResult_t resultHandle,
      IntPtr pOutBuffer,
      out uint punOutBufferSize)
    {
      return platform.ISteamInventory_SerializeResult(resultHandle.Value, pOutBuffer, out punOutBufferSize);
    }

    public bool TradeItems(
      ref SteamInventoryResult_t pResultHandle,
      CSteamID steamIDTradePartner,
      SteamItemInstanceID_t[] pArrayGive,
      uint[] pArrayGiveQuantity,
      uint nArrayGiveLength,
      SteamItemInstanceID_t[] pArrayGet,
      uint[] pArrayGetQuantity,
      uint nArrayGetLength)
    {
      return platform.ISteamInventory_TradeItems(ref pResultHandle.Value, steamIDTradePartner.Value, pArrayGive.Select(x => x.Value).ToArray(), pArrayGiveQuantity, nArrayGiveLength, pArrayGet.Select(x => x.Value).ToArray(), pArrayGetQuantity, nArrayGetLength);
    }

    public bool TransferItemQuantity(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemInstanceID_t itemIdSource,
      uint unQuantity,
      SteamItemInstanceID_t itemIdDest)
    {
      return platform.ISteamInventory_TransferItemQuantity(ref pResultHandle.Value, itemIdSource.Value, unQuantity, itemIdDest.Value);
    }

    public bool TriggerItemDrop(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemDef_t dropListDefinition)
    {
      return platform.ISteamInventory_TriggerItemDrop(ref pResultHandle.Value, dropListDefinition.Value);
    }
  }
}
