﻿using System;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct UserAchievementIconFetched_t
  {
    public const int CallbackId = 1109;
    public ulong GameID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string AchievementName;
    [MarshalAs(UnmanagedType.I1)]
    public bool Achieved;
    public int IconHandle;

    public static UserAchievementIconFetched_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (PackSmall) Marshal.PtrToStructure(p, typeof (PackSmall)) : (UserAchievementIconFetched_t) Marshal.PtrToStructure(p, typeof (UserAchievementIconFetched_t));
    }

    public static void RegisterCallback(
      BaseSteamworks steamworks,
      Action<UserAchievementIconFetched_t, bool> CallbackFunction)
    {
      CallbackHandle handle = new CallbackHandle();
      handle.steamworks = steamworks;
      if (Config.UseThisCall)
      {
        Callback.ThisCall.Result d1 = (_, p) => CallbackFunction(FromPointer(p), false);
        Callback.ThisCall.ResultWithInfo d2 = (_, p, bIOFailure, hSteamAPICall) => CallbackFunction(FromPointer(p), bIOFailure);
        Callback.ThisCall.GetSize d3 = _ => Marshal.SizeOf(typeof (UserAchievementIconFetched_t));
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
        Callback.StdCall.GetSize d6 = () => Marshal.SizeOf(typeof (UserAchievementIconFetched_t));
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
        CallbackId = 1109
      }, GCHandleType.Pinned);
      steamworks.native.api.SteamAPI_RegisterCallback(handle.PinnedCallback.AddrOfPinnedObject(), 1109);
      steamworks.RegisterCallbackHandle(handle);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public ulong GameID;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string AchievementName;
      [MarshalAs(UnmanagedType.I1)]
      public bool Achieved;
      public int IconHandle;

      public static implicit operator UserAchievementIconFetched_t(
        PackSmall d)
      {
        return new UserAchievementIconFetched_t {
          GameID = d.GameID,
          AchievementName = d.AchievementName,
          Achieved = d.Achieved,
          IconHandle = d.IconHandle
        };
      }
    }
  }
}
