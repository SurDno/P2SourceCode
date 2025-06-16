using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct RemoteStorageEnumerateUserSubscribedFilesResult_t
  {
    public const int CallbackId = 1314;
    public Result Result;
    public int ResultsReturned;
    public int TotalResultCount;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
    public ulong[] GPublishedFileId;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
    public uint[] GRTimeSubscribed;

    public static RemoteStorageEnumerateUserSubscribedFilesResult_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (RemoteStorageEnumerateUserSubscribedFilesResult_t) (RemoteStorageEnumerateUserSubscribedFilesResult_t.PackSmall) Marshal.PtrToStructure(p, typeof (RemoteStorageEnumerateUserSubscribedFilesResult_t.PackSmall)) : (RemoteStorageEnumerateUserSubscribedFilesResult_t) Marshal.PtrToStructure(p, typeof (RemoteStorageEnumerateUserSubscribedFilesResult_t));
    }

    public static CallbackHandle CallResult(
      BaseSteamworks steamworks,
      SteamAPICall_t call,
      Action<RemoteStorageEnumerateUserSubscribedFilesResult_t, bool> CallbackFunction)
    {
      CallbackHandle handle = new CallbackHandle();
      handle.steamworks = steamworks;
      handle.CallResultHandle = call;
      handle.CallResult = true;
      if (Config.UseThisCall)
      {
        Callback.ThisCall.Result d1 = (Callback.ThisCall.Result) ((_, p) =>
        {
          handle.Dispose();
          CallbackFunction(RemoteStorageEnumerateUserSubscribedFilesResult_t.FromPointer(p), false);
        });
        Callback.ThisCall.ResultWithInfo d2 = (Callback.ThisCall.ResultWithInfo) ((_, p, bIOFailure, hSteamAPICall) =>
        {
          if ((long) (ulong) hSteamAPICall != (long) (ulong) call)
            return;
          handle.CallResultHandle = (SteamAPICall_t) 0UL;
          handle.Dispose();
          CallbackFunction(RemoteStorageEnumerateUserSubscribedFilesResult_t.FromPointer(p), bIOFailure);
        });
        Callback.ThisCall.GetSize d3 = (Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (RemoteStorageEnumerateUserSubscribedFilesResult_t)));
        if (Platform.PackSmall)
          d3 = (Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (RemoteStorageEnumerateUserSubscribedFilesResult_t.PackSmall)));
        handle.FuncA = GCHandle.Alloc((object) d1);
        handle.FuncB = GCHandle.Alloc((object) d2);
        handle.FuncC = GCHandle.Alloc((object) d3);
        handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (Callback.VTable)));
        Callback.VTable structure = new Callback.VTable()
        {
          ResultA = Marshal.GetFunctionPointerForDelegate<Callback.ThisCall.Result>(d1),
          ResultB = Marshal.GetFunctionPointerForDelegate<Callback.ThisCall.ResultWithInfo>(d2),
          GetSize = Marshal.GetFunctionPointerForDelegate<Callback.ThisCall.GetSize>(d3)
        };
        if (Platform.IsWindows)
        {
          structure.ResultA = Marshal.GetFunctionPointerForDelegate<Callback.ThisCall.ResultWithInfo>(d2);
          structure.ResultB = Marshal.GetFunctionPointerForDelegate<Callback.ThisCall.Result>(d1);
        }
        Marshal.StructureToPtr<Callback.VTable>(structure, handle.vTablePtr, false);
      }
      else
      {
        Callback.StdCall.Result d4 = (Callback.StdCall.Result) (p =>
        {
          handle.Dispose();
          CallbackFunction(RemoteStorageEnumerateUserSubscribedFilesResult_t.FromPointer(p), false);
        });
        Callback.StdCall.ResultWithInfo d5 = (Callback.StdCall.ResultWithInfo) ((p, bIOFailure, hSteamAPICall) =>
        {
          if ((long) (ulong) hSteamAPICall != (long) (ulong) call)
            return;
          handle.CallResultHandle = (SteamAPICall_t) 0UL;
          handle.Dispose();
          CallbackFunction(RemoteStorageEnumerateUserSubscribedFilesResult_t.FromPointer(p), bIOFailure);
        });
        Callback.StdCall.GetSize d6 = (Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (RemoteStorageEnumerateUserSubscribedFilesResult_t)));
        if (Platform.PackSmall)
          d6 = (Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (RemoteStorageEnumerateUserSubscribedFilesResult_t.PackSmall)));
        handle.FuncA = GCHandle.Alloc((object) d4);
        handle.FuncB = GCHandle.Alloc((object) d5);
        handle.FuncC = GCHandle.Alloc((object) d6);
        handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (Callback.VTable)));
        Callback.VTable structure = new Callback.VTable()
        {
          ResultA = Marshal.GetFunctionPointerForDelegate<Callback.StdCall.Result>(d4),
          ResultB = Marshal.GetFunctionPointerForDelegate<Callback.StdCall.ResultWithInfo>(d5),
          GetSize = Marshal.GetFunctionPointerForDelegate<Callback.StdCall.GetSize>(d6)
        };
        if (Platform.IsWindows)
        {
          structure.ResultA = Marshal.GetFunctionPointerForDelegate<Callback.StdCall.ResultWithInfo>(d5);
          structure.ResultB = Marshal.GetFunctionPointerForDelegate<Callback.StdCall.Result>(d4);
        }
        Marshal.StructureToPtr<Callback.VTable>(structure, handle.vTablePtr, false);
      }
      handle.PinnedCallback = GCHandle.Alloc((object) new Callback()
      {
        vTablePtr = handle.vTablePtr,
        CallbackFlags = (steamworks.IsGameServer ? (byte) 2 : (byte) 0),
        CallbackId = 1314
      }, GCHandleType.Pinned);
      steamworks.native.api.SteamAPI_RegisterCallResult(handle.PinnedCallback.AddrOfPinnedObject(), call);
      return handle;
    }

    public static void RegisterCallback(
      BaseSteamworks steamworks,
      Action<RemoteStorageEnumerateUserSubscribedFilesResult_t, bool> CallbackFunction)
    {
      CallbackHandle handle = new CallbackHandle();
      handle.steamworks = steamworks;
      if (Config.UseThisCall)
      {
        Callback.ThisCall.Result d1 = (Callback.ThisCall.Result) ((_, p) => CallbackFunction(RemoteStorageEnumerateUserSubscribedFilesResult_t.FromPointer(p), false));
        Callback.ThisCall.ResultWithInfo d2 = (Callback.ThisCall.ResultWithInfo) ((_, p, bIOFailure, hSteamAPICall) => CallbackFunction(RemoteStorageEnumerateUserSubscribedFilesResult_t.FromPointer(p), bIOFailure));
        Callback.ThisCall.GetSize d3 = (Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (RemoteStorageEnumerateUserSubscribedFilesResult_t)));
        if (Platform.PackSmall)
          d3 = (Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (RemoteStorageEnumerateUserSubscribedFilesResult_t.PackSmall)));
        handle.FuncA = GCHandle.Alloc((object) d1);
        handle.FuncB = GCHandle.Alloc((object) d2);
        handle.FuncC = GCHandle.Alloc((object) d3);
        handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (Callback.VTable)));
        Callback.VTable structure = new Callback.VTable()
        {
          ResultA = Marshal.GetFunctionPointerForDelegate<Callback.ThisCall.Result>(d1),
          ResultB = Marshal.GetFunctionPointerForDelegate<Callback.ThisCall.ResultWithInfo>(d2),
          GetSize = Marshal.GetFunctionPointerForDelegate<Callback.ThisCall.GetSize>(d3)
        };
        if (Platform.IsWindows)
        {
          structure.ResultA = Marshal.GetFunctionPointerForDelegate<Callback.ThisCall.ResultWithInfo>(d2);
          structure.ResultB = Marshal.GetFunctionPointerForDelegate<Callback.ThisCall.Result>(d1);
        }
        Marshal.StructureToPtr<Callback.VTable>(structure, handle.vTablePtr, false);
      }
      else
      {
        Callback.StdCall.Result d4 = (Callback.StdCall.Result) (p => CallbackFunction(RemoteStorageEnumerateUserSubscribedFilesResult_t.FromPointer(p), false));
        Callback.StdCall.ResultWithInfo d5 = (Callback.StdCall.ResultWithInfo) ((p, bIOFailure, hSteamAPICall) => CallbackFunction(RemoteStorageEnumerateUserSubscribedFilesResult_t.FromPointer(p), bIOFailure));
        Callback.StdCall.GetSize d6 = (Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (RemoteStorageEnumerateUserSubscribedFilesResult_t)));
        if (Platform.PackSmall)
          d6 = (Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (RemoteStorageEnumerateUserSubscribedFilesResult_t.PackSmall)));
        handle.FuncA = GCHandle.Alloc((object) d4);
        handle.FuncB = GCHandle.Alloc((object) d5);
        handle.FuncC = GCHandle.Alloc((object) d6);
        handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (Callback.VTable)));
        Callback.VTable structure = new Callback.VTable()
        {
          ResultA = Marshal.GetFunctionPointerForDelegate<Callback.StdCall.Result>(d4),
          ResultB = Marshal.GetFunctionPointerForDelegate<Callback.StdCall.ResultWithInfo>(d5),
          GetSize = Marshal.GetFunctionPointerForDelegate<Callback.StdCall.GetSize>(d6)
        };
        if (Platform.IsWindows)
        {
          structure.ResultA = Marshal.GetFunctionPointerForDelegate<Callback.StdCall.ResultWithInfo>(d5);
          structure.ResultB = Marshal.GetFunctionPointerForDelegate<Callback.StdCall.Result>(d4);
        }
        Marshal.StructureToPtr<Callback.VTable>(structure, handle.vTablePtr, false);
      }
      handle.PinnedCallback = GCHandle.Alloc((object) new Callback()
      {
        vTablePtr = handle.vTablePtr,
        CallbackFlags = (steamworks.IsGameServer ? (byte) 2 : (byte) 0),
        CallbackId = 1314
      }, GCHandleType.Pinned);
      steamworks.native.api.SteamAPI_RegisterCallback(handle.PinnedCallback.AddrOfPinnedObject(), 1314);
      steamworks.RegisterCallbackHandle(handle);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public Result Result;
      public int ResultsReturned;
      public int TotalResultCount;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
      public ulong[] GPublishedFileId;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
      public uint[] GRTimeSubscribed;

      public static implicit operator RemoteStorageEnumerateUserSubscribedFilesResult_t(
        RemoteStorageEnumerateUserSubscribedFilesResult_t.PackSmall d)
      {
        return new RemoteStorageEnumerateUserSubscribedFilesResult_t()
        {
          Result = d.Result,
          ResultsReturned = d.ResultsReturned,
          TotalResultCount = d.TotalResultCount,
          GPublishedFileId = d.GPublishedFileId,
          GRTimeSubscribed = d.GRTimeSubscribed
        };
      }
    }
  }
}
