using System;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct FavoritesListChanged_t
  {
    public const int CallbackId = 502;
    public uint IP;
    public uint QueryPort;
    public uint ConnPort;
    public uint AppID;
    public uint Flags;
    [MarshalAs(UnmanagedType.I1)]
    public bool Add;
    public uint AccountId;

    public static FavoritesListChanged_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (PackSmall) Marshal.PtrToStructure(p, typeof (PackSmall)) : (FavoritesListChanged_t) Marshal.PtrToStructure(p, typeof (FavoritesListChanged_t));
    }

    public static void RegisterCallback(
      BaseSteamworks steamworks,
      Action<FavoritesListChanged_t, bool> CallbackFunction)
    {
      CallbackHandle handle = new CallbackHandle();
      handle.steamworks = steamworks;
      if (Config.UseThisCall)
      {
        Callback.ThisCall.Result d1 = (_, p) => CallbackFunction(FromPointer(p), false);
        Callback.ThisCall.ResultWithInfo d2 = (_, p, bIOFailure, hSteamAPICall) => CallbackFunction(FromPointer(p), bIOFailure);
        Callback.ThisCall.GetSize d3 = _ => Marshal.SizeOf(typeof (FavoritesListChanged_t));
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
        Callback.StdCall.GetSize d6 = () => Marshal.SizeOf(typeof (FavoritesListChanged_t));
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
        CallbackId = 502
      }, GCHandleType.Pinned);
      steamworks.native.api.SteamAPI_RegisterCallback(handle.PinnedCallback.AddrOfPinnedObject(), 502);
      steamworks.RegisterCallbackHandle(handle);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public uint IP;
      public uint QueryPort;
      public uint ConnPort;
      public uint AppID;
      public uint Flags;
      [MarshalAs(UnmanagedType.I1)]
      public bool Add;
      public uint AccountId;

      public static implicit operator FavoritesListChanged_t(PackSmall d)
      {
        return new FavoritesListChanged_t {
          IP = d.IP,
          QueryPort = d.QueryPort,
          ConnPort = d.ConnPort,
          AppID = d.AppID,
          Flags = d.Flags,
          Add = d.Add,
          AccountId = d.AccountId
        };
      }
    }
  }
}
