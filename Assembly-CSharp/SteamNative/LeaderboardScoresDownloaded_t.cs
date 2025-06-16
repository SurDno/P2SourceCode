using System;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct LeaderboardScoresDownloaded_t
  {
    public const int CallbackId = 1105;
    public ulong SteamLeaderboard;
    public ulong SteamLeaderboardEntries;
    public int CEntryCount;

    public static LeaderboardScoresDownloaded_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (PackSmall) Marshal.PtrToStructure(p, typeof (PackSmall)) : (LeaderboardScoresDownloaded_t) Marshal.PtrToStructure(p, typeof (LeaderboardScoresDownloaded_t));
    }

    public static CallbackHandle CallResult(
      BaseSteamworks steamworks,
      SteamAPICall_t call,
      Action<LeaderboardScoresDownloaded_t, bool> CallbackFunction)
    {
      CallbackHandle handle = new CallbackHandle();
      handle.steamworks = steamworks;
      handle.CallResultHandle = call;
      handle.CallResult = true;
      if (Config.UseThisCall)
      {
        Callback.ThisCall.Result d1 = (_, p) =>
        {
          handle.Dispose();
          CallbackFunction(FromPointer(p), false);
        };
        Callback.ThisCall.ResultWithInfo d2 = (_, p, bIOFailure, hSteamAPICall) =>
        {
          if ((long) (ulong) hSteamAPICall != (long) (ulong) call)
            return;
          handle.CallResultHandle = 0UL;
          handle.Dispose();
          CallbackFunction(FromPointer(p), bIOFailure);
        };
        Callback.ThisCall.GetSize d3 = _ => Marshal.SizeOf(typeof (LeaderboardScoresDownloaded_t));
        if (Platform.PackSmall)
          d3 = _ => Marshal.SizeOf(typeof (PackSmall));
        handle.FuncA = GCHandle.Alloc(d1);
        handle.FuncB = GCHandle.Alloc(d2);
        handle.FuncC = GCHandle.Alloc(d3);
        handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (Callback.VTable)));
        Callback.VTable structure = new Callback.VTable {
          ResultA = Marshal.GetFunctionPointerForDelegate(d1),
          ResultB = Marshal.GetFunctionPointerForDelegate(d2),
          GetSize = Marshal.GetFunctionPointerForDelegate(d3)
        };
        if (Platform.IsWindows)
        {
          structure.ResultA = Marshal.GetFunctionPointerForDelegate(d2);
          structure.ResultB = Marshal.GetFunctionPointerForDelegate(d1);
        }
        Marshal.StructureToPtr(structure, handle.vTablePtr, false);
      }
      else
      {
        Callback.StdCall.Result d4 = p =>
        {
          handle.Dispose();
          CallbackFunction(FromPointer(p), false);
        };
        Callback.StdCall.ResultWithInfo d5 = (p, bIOFailure, hSteamAPICall) =>
        {
          if ((long) (ulong) hSteamAPICall != (long) (ulong) call)
            return;
          handle.CallResultHandle = 0UL;
          handle.Dispose();
          CallbackFunction(FromPointer(p), bIOFailure);
        };
        Callback.StdCall.GetSize d6 = () => Marshal.SizeOf(typeof (LeaderboardScoresDownloaded_t));
        if (Platform.PackSmall)
          d6 = () => Marshal.SizeOf(typeof (PackSmall));
        handle.FuncA = GCHandle.Alloc(d4);
        handle.FuncB = GCHandle.Alloc(d5);
        handle.FuncC = GCHandle.Alloc(d6);
        handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (Callback.VTable)));
        Callback.VTable structure = new Callback.VTable {
          ResultA = Marshal.GetFunctionPointerForDelegate(d4),
          ResultB = Marshal.GetFunctionPointerForDelegate(d5),
          GetSize = Marshal.GetFunctionPointerForDelegate(d6)
        };
        if (Platform.IsWindows)
        {
          structure.ResultA = Marshal.GetFunctionPointerForDelegate(d5);
          structure.ResultB = Marshal.GetFunctionPointerForDelegate(d4);
        }
        Marshal.StructureToPtr(structure, handle.vTablePtr, false);
      }
      handle.PinnedCallback = GCHandle.Alloc(new Callback {
        vTablePtr = handle.vTablePtr,
        CallbackFlags = (steamworks.IsGameServer ? (byte) 2 : (byte) 0),
        CallbackId = 1105
      }, GCHandleType.Pinned);
      steamworks.native.api.SteamAPI_RegisterCallResult(handle.PinnedCallback.AddrOfPinnedObject(), call);
      return handle;
    }

    public static void RegisterCallback(
      BaseSteamworks steamworks,
      Action<LeaderboardScoresDownloaded_t, bool> CallbackFunction)
    {
      CallbackHandle handle = new CallbackHandle();
      handle.steamworks = steamworks;
      if (Config.UseThisCall)
      {
        Callback.ThisCall.Result d1 = (_, p) => CallbackFunction(FromPointer(p), false);
        Callback.ThisCall.ResultWithInfo d2 = (_, p, bIOFailure, hSteamAPICall) => CallbackFunction(FromPointer(p), bIOFailure);
        Callback.ThisCall.GetSize d3 = _ => Marshal.SizeOf(typeof (LeaderboardScoresDownloaded_t));
        if (Platform.PackSmall)
          d3 = _ => Marshal.SizeOf(typeof (PackSmall));
        handle.FuncA = GCHandle.Alloc(d1);
        handle.FuncB = GCHandle.Alloc(d2);
        handle.FuncC = GCHandle.Alloc(d3);
        handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (Callback.VTable)));
        Callback.VTable structure = new Callback.VTable {
          ResultA = Marshal.GetFunctionPointerForDelegate(d1),
          ResultB = Marshal.GetFunctionPointerForDelegate(d2),
          GetSize = Marshal.GetFunctionPointerForDelegate(d3)
        };
        if (Platform.IsWindows)
        {
          structure.ResultA = Marshal.GetFunctionPointerForDelegate(d2);
          structure.ResultB = Marshal.GetFunctionPointerForDelegate(d1);
        }
        Marshal.StructureToPtr(structure, handle.vTablePtr, false);
      }
      else
      {
        Callback.StdCall.Result d4 = p => CallbackFunction(FromPointer(p), false);
        Callback.StdCall.ResultWithInfo d5 = (p, bIOFailure, hSteamAPICall) => CallbackFunction(FromPointer(p), bIOFailure);
        Callback.StdCall.GetSize d6 = () => Marshal.SizeOf(typeof (LeaderboardScoresDownloaded_t));
        if (Platform.PackSmall)
          d6 = () => Marshal.SizeOf(typeof (PackSmall));
        handle.FuncA = GCHandle.Alloc(d4);
        handle.FuncB = GCHandle.Alloc(d5);
        handle.FuncC = GCHandle.Alloc(d6);
        handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (Callback.VTable)));
        Callback.VTable structure = new Callback.VTable {
          ResultA = Marshal.GetFunctionPointerForDelegate(d4),
          ResultB = Marshal.GetFunctionPointerForDelegate(d5),
          GetSize = Marshal.GetFunctionPointerForDelegate(d6)
        };
        if (Platform.IsWindows)
        {
          structure.ResultA = Marshal.GetFunctionPointerForDelegate(d5);
          structure.ResultB = Marshal.GetFunctionPointerForDelegate(d4);
        }
        Marshal.StructureToPtr(structure, handle.vTablePtr, false);
      }
      handle.PinnedCallback = GCHandle.Alloc(new Callback {
        vTablePtr = handle.vTablePtr,
        CallbackFlags = (steamworks.IsGameServer ? (byte) 2 : (byte) 0),
        CallbackId = 1105
      }, GCHandleType.Pinned);
      steamworks.native.api.SteamAPI_RegisterCallback(handle.PinnedCallback.AddrOfPinnedObject(), 1105);
      steamworks.RegisterCallbackHandle(handle);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public ulong SteamLeaderboard;
      public ulong SteamLeaderboardEntries;
      public int CEntryCount;

      public static implicit operator LeaderboardScoresDownloaded_t(
        PackSmall d)
      {
        return new LeaderboardScoresDownloaded_t {
          SteamLeaderboard = d.SteamLeaderboard,
          SteamLeaderboardEntries = d.SteamLeaderboardEntries,
          CEntryCount = d.CEntryCount
        };
      }
    }
  }
}
