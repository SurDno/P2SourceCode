using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct GameServerChangeRequested_t
  {
    public const int CallbackId = 332;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string Server;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string Password;

    public static GameServerChangeRequested_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (GameServerChangeRequested_t) (GameServerChangeRequested_t.PackSmall) Marshal.PtrToStructure(p, typeof (GameServerChangeRequested_t.PackSmall)) : (GameServerChangeRequested_t) Marshal.PtrToStructure(p, typeof (GameServerChangeRequested_t));
    }

    public static void RegisterCallback(
      BaseSteamworks steamworks,
      Action<GameServerChangeRequested_t, bool> CallbackFunction)
    {
      CallbackHandle handle = new CallbackHandle();
      handle.steamworks = steamworks;
      if (Config.UseThisCall)
      {
        Callback.ThisCall.Result d1 = (Callback.ThisCall.Result) ((_, p) => CallbackFunction(GameServerChangeRequested_t.FromPointer(p), false));
        Callback.ThisCall.ResultWithInfo d2 = (Callback.ThisCall.ResultWithInfo) ((_, p, bIOFailure, hSteamAPICall) => CallbackFunction(GameServerChangeRequested_t.FromPointer(p), bIOFailure));
        Callback.ThisCall.GetSize d3 = (Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (GameServerChangeRequested_t)));
        if (Platform.PackSmall)
          d3 = (Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (GameServerChangeRequested_t.PackSmall)));
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
        Callback.StdCall.Result d4 = (Callback.StdCall.Result) (p => CallbackFunction(GameServerChangeRequested_t.FromPointer(p), false));
        Callback.StdCall.ResultWithInfo d5 = (Callback.StdCall.ResultWithInfo) ((p, bIOFailure, hSteamAPICall) => CallbackFunction(GameServerChangeRequested_t.FromPointer(p), bIOFailure));
        Callback.StdCall.GetSize d6 = (Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (GameServerChangeRequested_t)));
        if (Platform.PackSmall)
          d6 = (Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (GameServerChangeRequested_t.PackSmall)));
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
        CallbackId = 332
      }, GCHandleType.Pinned);
      steamworks.native.api.SteamAPI_RegisterCallback(handle.PinnedCallback.AddrOfPinnedObject(), 332);
      steamworks.RegisterCallbackHandle(handle);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string Server;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string Password;

      public static implicit operator GameServerChangeRequested_t(
        GameServerChangeRequested_t.PackSmall d)
      {
        return new GameServerChangeRequested_t()
        {
          Server = d.Server,
          Password = d.Password
        };
      }
    }
  }
}
