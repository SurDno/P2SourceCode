using Facepunch.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    public bool AddPromoItem(ref SteamInventoryResult_t pResultHandle, SteamItemDef_t itemDef)
    {
      return this.platform.ISteamInventory_AddPromoItem(ref pResultHandle.Value, itemDef.Value);
    }

    public bool AddPromoItems(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemDef_t[] pArrayItemDefs,
      uint unArrayLength)
    {
      return this.platform.ISteamInventory_AddPromoItems(ref pResultHandle.Value, ((IEnumerable<SteamItemDef_t>) pArrayItemDefs).Select<SteamItemDef_t, int>((Func<SteamItemDef_t, int>) (x => x.Value)).ToArray<int>(), unArrayLength);
    }

    public bool CheckResultSteamID(SteamInventoryResult_t resultHandle, CSteamID steamIDExpected)
    {
      return this.platform.ISteamInventory_CheckResultSteamID(resultHandle.Value, steamIDExpected.Value);
    }

    public bool ConsumeItem(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemInstanceID_t itemConsume,
      uint unQuantity)
    {
      return this.platform.ISteamInventory_ConsumeItem(ref pResultHandle.Value, itemConsume.Value, unQuantity);
    }

    public bool DeserializeResult(
      ref SteamInventoryResult_t pOutResultHandle,
      IntPtr pBuffer,
      uint unBufferSize,
      bool bRESERVED_MUST_BE_FALSE)
    {
      return this.platform.ISteamInventory_DeserializeResult(ref pOutResultHandle.Value, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
    }

    public void DestroyResult(SteamInventoryResult_t resultHandle)
    {
      this.platform.ISteamInventory_DestroyResult(resultHandle.Value);
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
      return this.platform.ISteamInventory_ExchangeItems(ref pResultHandle.Value, ((IEnumerable<SteamItemDef_t>) pArrayGenerate).Select<SteamItemDef_t, int>((Func<SteamItemDef_t, int>) (x => x.Value)).ToArray<int>(), punArrayGenerateQuantity, unArrayGenerateLength, ((IEnumerable<SteamItemInstanceID_t>) pArrayDestroy).Select<SteamItemInstanceID_t, ulong>((Func<SteamItemInstanceID_t, ulong>) (x => x.Value)).ToArray<ulong>(), punArrayDestroyQuantity, unArrayDestroyLength);
    }

    public bool GenerateItems(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemDef_t[] pArrayItemDefs,
      uint[] punArrayQuantity,
      uint unArrayLength)
    {
      return this.platform.ISteamInventory_GenerateItems(ref pResultHandle.Value, ((IEnumerable<SteamItemDef_t>) pArrayItemDefs).Select<SteamItemDef_t, int>((Func<SteamItemDef_t, int>) (x => x.Value)).ToArray<int>(), punArrayQuantity, unArrayLength);
    }

    public bool GetAllItems(ref SteamInventoryResult_t pResultHandle)
    {
      return this.platform.ISteamInventory_GetAllItems(ref pResultHandle.Value);
    }

    public unsafe SteamItemDef_t[] GetEligiblePromoItemDefinitionIDs(CSteamID steamID)
    {
      uint punItemDefIDsArraySize = 0;
      if (!this.platform.ISteamInventory_GetEligiblePromoItemDefinitionIDs(steamID.Value, IntPtr.Zero, out punItemDefIDsArraySize) || punItemDefIDsArraySize == 0U)
        return (SteamItemDef_t[]) null;
      SteamItemDef_t[] steamItemDefTArray = new SteamItemDef_t[(int) punItemDefIDsArraySize];
      fixed (SteamItemDef_t* pItemDefIDs = steamItemDefTArray)
        return !this.platform.ISteamInventory_GetEligiblePromoItemDefinitionIDs(steamID.Value, (IntPtr) (void*) pItemDefIDs, out punItemDefIDsArraySize) ? (SteamItemDef_t[]) null : steamItemDefTArray;
    }

    public unsafe SteamItemDef_t[] GetItemDefinitionIDs()
    {
      uint punItemDefIDsArraySize = 0;
      if (!this.platform.ISteamInventory_GetItemDefinitionIDs(IntPtr.Zero, out punItemDefIDsArraySize) || punItemDefIDsArraySize == 0U)
        return (SteamItemDef_t[]) null;
      SteamItemDef_t[] steamItemDefTArray = new SteamItemDef_t[(int) punItemDefIDsArraySize];
      fixed (SteamItemDef_t* pItemDefIDs = steamItemDefTArray)
        return !this.platform.ISteamInventory_GetItemDefinitionIDs((IntPtr) (void*) pItemDefIDs, out punItemDefIDsArraySize) ? (SteamItemDef_t[]) null : steamItemDefTArray;
    }

    public bool GetItemDefinitionProperty(
      SteamItemDef_t iDefinition,
      string pchPropertyName,
      out string pchValueBuffer)
    {
      pchValueBuffer = string.Empty;
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      uint punValueBufferSizeOut = 4096;
      bool definitionProperty = this.platform.ISteamInventory_GetItemDefinitionProperty(iDefinition.Value, pchPropertyName, stringBuilder, out punValueBufferSizeOut);
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
      return this.platform.ISteamInventory_GetItemsByID(ref pResultHandle.Value, ((IEnumerable<SteamItemInstanceID_t>) pInstanceIDs).Select<SteamItemInstanceID_t, ulong>((Func<SteamItemInstanceID_t, ulong>) (x => x.Value)).ToArray<ulong>(), unCountInstanceIDs);
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
      bool resultItemProperty = this.platform.ISteamInventory_GetResultItemProperty(resultHandle.Value, unItemIndex, pchPropertyName, stringBuilder, out punValueBufferSizeOut);
      if (!resultItemProperty)
        return resultItemProperty;
      pchValueBuffer = stringBuilder.ToString();
      return resultItemProperty;
    }

    public unsafe SteamItemDetails_t[] GetResultItems(SteamInventoryResult_t resultHandle)
    {
      uint punOutItemsArraySize = 0;
      if (!this.platform.ISteamInventory_GetResultItems(resultHandle.Value, IntPtr.Zero, out punOutItemsArraySize) || punOutItemsArraySize == 0U)
        return (SteamItemDetails_t[]) null;
      SteamItemDetails_t[] steamItemDetailsTArray = new SteamItemDetails_t[(int) punOutItemsArraySize];
      fixed (SteamItemDetails_t* pOutItemsArray = steamItemDetailsTArray)
        return !this.platform.ISteamInventory_GetResultItems(resultHandle.Value, (IntPtr) (void*) pOutItemsArray, out punOutItemsArraySize) ? (SteamItemDetails_t[]) null : steamItemDetailsTArray;
    }

    public Result GetResultStatus(SteamInventoryResult_t resultHandle)
    {
      return this.platform.ISteamInventory_GetResultStatus(resultHandle.Value);
    }

    public uint GetResultTimestamp(SteamInventoryResult_t resultHandle)
    {
      return this.platform.ISteamInventory_GetResultTimestamp(resultHandle.Value);
    }

    public bool GrantPromoItems(ref SteamInventoryResult_t pResultHandle)
    {
      return this.platform.ISteamInventory_GrantPromoItems(ref pResultHandle.Value);
    }

    public bool LoadItemDefinitions() => this.platform.ISteamInventory_LoadItemDefinitions();

    public CallbackHandle RequestEligiblePromoItemDefinitionsIDs(
      CSteamID steamID,
      Action<SteamInventoryEligiblePromoItemDefIDs_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamInventory_RequestEligiblePromoItemDefinitionsIDs(steamID.Value);
      return CallbackFunction == null ? (CallbackHandle) null : SteamInventoryEligiblePromoItemDefIDs_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public void SendItemDropHeartbeat() => this.platform.ISteamInventory_SendItemDropHeartbeat();

    public bool SerializeResult(
      SteamInventoryResult_t resultHandle,
      IntPtr pOutBuffer,
      out uint punOutBufferSize)
    {
      return this.platform.ISteamInventory_SerializeResult(resultHandle.Value, pOutBuffer, out punOutBufferSize);
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
      return this.platform.ISteamInventory_TradeItems(ref pResultHandle.Value, steamIDTradePartner.Value, ((IEnumerable<SteamItemInstanceID_t>) pArrayGive).Select<SteamItemInstanceID_t, ulong>((Func<SteamItemInstanceID_t, ulong>) (x => x.Value)).ToArray<ulong>(), pArrayGiveQuantity, nArrayGiveLength, ((IEnumerable<SteamItemInstanceID_t>) pArrayGet).Select<SteamItemInstanceID_t, ulong>((Func<SteamItemInstanceID_t, ulong>) (x => x.Value)).ToArray<ulong>(), pArrayGetQuantity, nArrayGetLength);
    }

    public bool TransferItemQuantity(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemInstanceID_t itemIdSource,
      uint unQuantity,
      SteamItemInstanceID_t itemIdDest)
    {
      return this.platform.ISteamInventory_TransferItemQuantity(ref pResultHandle.Value, itemIdSource.Value, unQuantity, itemIdDest.Value);
    }

    public bool TriggerItemDrop(
      ref SteamInventoryResult_t pResultHandle,
      SteamItemDef_t dropListDefinition)
    {
      return this.platform.ISteamInventory_TriggerItemDrop(ref pResultHandle.Value, dropListDefinition.Value);
    }
  }
}
