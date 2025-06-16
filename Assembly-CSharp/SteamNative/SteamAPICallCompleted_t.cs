using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct SteamAPICallCompleted_t
  {
    public const int CallbackId = 703;
    public ulong AsyncCall;
    public int Callback;
    public uint ParamCount;

    public static SteamAPICallCompleted_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (SteamAPICallCompleted_t) (SteamAPICallCompleted_t.PackSmall) Marshal.PtrToStructure(p, typeof (SteamAPICallCompleted_t.PackSmall)) : (SteamAPICallCompleted_t) Marshal.PtrToStructure(p, typeof (SteamAPICallCompleted_t));
    }

    public static void RegisterCallback(
      BaseSteamworks steamworks,
      Action<SteamAPICallCompleted_t, bool> CallbackFunction)
    {
      CallbackHandle handle = new CallbackHandle();
      handle.steamworks = steamworks;
      if (Config.UseThisCall)
      {
        SteamNative.Callback.ThisCall.Result d1 = (SteamNative.Callback.ThisCall.Result) ((_, p) => CallbackFunction(SteamAPICallCompleted_t.FromPointer(p), false));
        SteamNative.Callback.ThisCall.ResultWithInfo d2 = (SteamNative.Callback.ThisCall.ResultWithInfo) ((_, p, bIOFailure, hSteamAPICall) => CallbackFunction(SteamAPICallCompleted_t.FromPointer(p), bIOFailure));
        SteamNative.Callback.ThisCall.GetSize d3 = (SteamNative.Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (SteamAPICallCompleted_t)));
        if (Platform.PackSmall)
          d3 = (SteamNative.Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (SteamAPICallCompleted_t.PackSmall)));
        handle.FuncA = GCHandle.Alloc((object) d1);
        handle.FuncB = GCHandle.Alloc((object) d2);
        handle.FuncC = GCHandle.Alloc((object) d3);
        handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (SteamNative.Callback.VTable)));
        SteamNative.Callback.VTable structure = new SteamNative.Callback.VTable()
        {
          ResultA = Marshal.GetFunctionPointerForDelegate<SteamNative.Callback.ThisCall.Result>(d1),
          ResultB = Marshal.GetFunctionPointerForDelegate<SteamNative.Callback.ThisCall.ResultWithInfo>(d2),
          GetSize = Marshal.GetFunctionPointerForDelegate<SteamNative.Callback.ThisCall.GetSize>(d3)
        };
        if (Platform.IsWindows)
        {
          structure.ResultA = Marshal.GetFunctionPointerForDelegate<SteamNative.Callback.ThisCall.ResultWithInfo>(d2);
          structure.ResultB = Marshal.GetFunctionPointerForDelegate<SteamNative.Callback.ThisCall.Result>(d1);
        }
        Marshal.StructureToPtr<SteamNative.Callback.VTable>(structure, handle.vTablePtr, false);
      }
      else
      {
        SteamNative.Callback.StdCall.Result d4 = (SteamNative.Callback.StdCall.Result) (p => CallbackFunction(SteamAPICallCompleted_t.FromPointer(p), false));
        SteamNative.Callback.StdCall.ResultWithInfo d5 = (SteamNative.Callback.StdCall.ResultWithInfo) ((p, bIOFailure, hSteamAPICall) => CallbackFunction(SteamAPICallCompleted_t.FromPointer(p), bIOFailure));
        SteamNative.Callback.StdCall.GetSize d6 = (SteamNative.Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (SteamAPICallCompleted_t)));
        if (Platform.PackSmall)
          d6 = (SteamNative.Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (SteamAPICallCompleted_t.PackSmall)));
        handle.FuncA = GCHandle.Alloc((object) d4);
        handle.FuncB = GCHandle.Alloc((object) d5);
        handle.FuncC = GCHandle.Alloc((object) d6);
        handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (SteamNative.Callback.VTable)));
        SteamNative.Callback.VTable structure = new SteamNative.Callback.VTable()
        {
          ResultA = Marshal.GetFunctionPointerForDelegate<SteamNative.Callback.StdCall.Result>(d4),
          ResultB = Marshal.GetFunctionPointerForDelegate<SteamNative.Callback.StdCall.ResultWithInfo>(d5),
          GetSize = Marshal.GetFunctionPointerForDelegate<SteamNative.Callback.StdCall.GetSize>(d6)
        };
        if (Platform.IsWindows)
        {
          structure.ResultA = Marshal.GetFunctionPointerForDelegate<SteamNative.Callback.StdCall.ResultWithInfo>(d5);
          structure.ResultB = Marshal.GetFunctionPointerForDelegate<SteamNative.Callback.StdCall.Result>(d4);
        }
        Marshal.StructureToPtr<SteamNative.Callback.VTable>(structure, handle.vTablePtr, false);
      }
      handle.PinnedCallback = GCHandle.Alloc((object) new SteamNative.Callback()
      {
        vTablePtr = handle.vTablePtr,
        CallbackFlags = (steamworks.IsGameServer ? (byte) 2 : (byte) 0),
        CallbackId = 703
      }, GCHandleType.Pinned);
      steamworks.native.api.SteamAPI_RegisterCallback(handle.PinnedCallback.AddrOfPinnedObject(), 703);
      steamworks.RegisterCallbackHandle(handle);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public ulong AsyncCall;
      public int Callback;
      public uint ParamCount;

      public static implicit operator SteamAPICallCompleted_t(SteamAPICallCompleted_t.PackSmall d)
      {
        return new SteamAPICallCompleted_t()
        {
          AsyncCall = d.AsyncCall,
          Callback = d.Callback,
          ParamCount = d.ParamCount
        };
      }
    }
  }
}
