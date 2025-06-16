using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct RemoteStorageGetPublishedFileDetailsResult_t
  {
    public const int CallbackId = 1318;
    public Result Result;
    public ulong PublishedFileId;
    public uint CreatorAppID;
    public uint ConsumerAppID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
    public string Title;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
    public string Description;
    public ulong File;
    public ulong PreviewFile;
    public ulong SteamIDOwner;
    public uint TimeCreated;
    public uint TimeUpdated;
    public RemoteStoragePublishedFileVisibility Visibility;
    [MarshalAs(UnmanagedType.I1)]
    public bool Banned;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1025)]
    public string Tags;
    [MarshalAs(UnmanagedType.I1)]
    public bool TagsTruncated;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string PchFileName;
    public int FileSize;
    public int PreviewFileSize;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string URL;
    public WorkshopFileType FileType;
    [MarshalAs(UnmanagedType.I1)]
    public bool AcceptedForUse;

    public static RemoteStorageGetPublishedFileDetailsResult_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (RemoteStorageGetPublishedFileDetailsResult_t) (RemoteStorageGetPublishedFileDetailsResult_t.PackSmall) Marshal.PtrToStructure(p, typeof (RemoteStorageGetPublishedFileDetailsResult_t.PackSmall)) : (RemoteStorageGetPublishedFileDetailsResult_t) Marshal.PtrToStructure(p, typeof (RemoteStorageGetPublishedFileDetailsResult_t));
    }

    public static CallbackHandle CallResult(
      BaseSteamworks steamworks,
      SteamAPICall_t call,
      Action<RemoteStorageGetPublishedFileDetailsResult_t, bool> CallbackFunction)
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
          CallbackFunction(RemoteStorageGetPublishedFileDetailsResult_t.FromPointer(p), false);
        });
        Callback.ThisCall.ResultWithInfo d2 = (Callback.ThisCall.ResultWithInfo) ((_, p, bIOFailure, hSteamAPICall) =>
        {
          if ((long) (ulong) hSteamAPICall != (long) (ulong) call)
            return;
          handle.CallResultHandle = (SteamAPICall_t) 0UL;
          handle.Dispose();
          CallbackFunction(RemoteStorageGetPublishedFileDetailsResult_t.FromPointer(p), bIOFailure);
        });
        Callback.ThisCall.GetSize d3 = (Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (RemoteStorageGetPublishedFileDetailsResult_t)));
        if (Platform.PackSmall)
          d3 = (Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (RemoteStorageGetPublishedFileDetailsResult_t.PackSmall)));
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
          CallbackFunction(RemoteStorageGetPublishedFileDetailsResult_t.FromPointer(p), false);
        });
        Callback.StdCall.ResultWithInfo d5 = (Callback.StdCall.ResultWithInfo) ((p, bIOFailure, hSteamAPICall) =>
        {
          if ((long) (ulong) hSteamAPICall != (long) (ulong) call)
            return;
          handle.CallResultHandle = (SteamAPICall_t) 0UL;
          handle.Dispose();
          CallbackFunction(RemoteStorageGetPublishedFileDetailsResult_t.FromPointer(p), bIOFailure);
        });
        Callback.StdCall.GetSize d6 = (Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (RemoteStorageGetPublishedFileDetailsResult_t)));
        if (Platform.PackSmall)
          d6 = (Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (RemoteStorageGetPublishedFileDetailsResult_t.PackSmall)));
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
        CallbackId = 1318
      }, GCHandleType.Pinned);
      steamworks.native.api.SteamAPI_RegisterCallResult(handle.PinnedCallback.AddrOfPinnedObject(), call);
      return handle;
    }

    public static void RegisterCallback(
      BaseSteamworks steamworks,
      Action<RemoteStorageGetPublishedFileDetailsResult_t, bool> CallbackFunction)
    {
      CallbackHandle handle = new CallbackHandle();
      handle.steamworks = steamworks;
      if (Config.UseThisCall)
      {
        Callback.ThisCall.Result d1 = (Callback.ThisCall.Result) ((_, p) => CallbackFunction(RemoteStorageGetPublishedFileDetailsResult_t.FromPointer(p), false));
        Callback.ThisCall.ResultWithInfo d2 = (Callback.ThisCall.ResultWithInfo) ((_, p, bIOFailure, hSteamAPICall) => CallbackFunction(RemoteStorageGetPublishedFileDetailsResult_t.FromPointer(p), bIOFailure));
        Callback.ThisCall.GetSize d3 = (Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (RemoteStorageGetPublishedFileDetailsResult_t)));
        if (Platform.PackSmall)
          d3 = (Callback.ThisCall.GetSize) (_ => Marshal.SizeOf(typeof (RemoteStorageGetPublishedFileDetailsResult_t.PackSmall)));
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
        Callback.StdCall.Result d4 = (Callback.StdCall.Result) (p => CallbackFunction(RemoteStorageGetPublishedFileDetailsResult_t.FromPointer(p), false));
        Callback.StdCall.ResultWithInfo d5 = (Callback.StdCall.ResultWithInfo) ((p, bIOFailure, hSteamAPICall) => CallbackFunction(RemoteStorageGetPublishedFileDetailsResult_t.FromPointer(p), bIOFailure));
        Callback.StdCall.GetSize d6 = (Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (RemoteStorageGetPublishedFileDetailsResult_t)));
        if (Platform.PackSmall)
          d6 = (Callback.StdCall.GetSize) (() => Marshal.SizeOf(typeof (RemoteStorageGetPublishedFileDetailsResult_t.PackSmall)));
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
        CallbackId = 1318
      }, GCHandleType.Pinned);
      steamworks.native.api.SteamAPI_RegisterCallback(handle.PinnedCallback.AddrOfPinnedObject(), 1318);
      steamworks.RegisterCallbackHandle(handle);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public Result Result;
      public ulong PublishedFileId;
      public uint CreatorAppID;
      public uint ConsumerAppID;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
      public string Title;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
      public string Description;
      public ulong File;
      public ulong PreviewFile;
      public ulong SteamIDOwner;
      public uint TimeCreated;
      public uint TimeUpdated;
      public RemoteStoragePublishedFileVisibility Visibility;
      [MarshalAs(UnmanagedType.I1)]
      public bool Banned;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1025)]
      public string Tags;
      [MarshalAs(UnmanagedType.I1)]
      public bool TagsTruncated;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string PchFileName;
      public int FileSize;
      public int PreviewFileSize;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string URL;
      public WorkshopFileType FileType;
      [MarshalAs(UnmanagedType.I1)]
      public bool AcceptedForUse;

      public static implicit operator RemoteStorageGetPublishedFileDetailsResult_t(
        RemoteStorageGetPublishedFileDetailsResult_t.PackSmall d)
      {
        return new RemoteStorageGetPublishedFileDetailsResult_t()
        {
          Result = d.Result,
          PublishedFileId = d.PublishedFileId,
          CreatorAppID = d.CreatorAppID,
          ConsumerAppID = d.ConsumerAppID,
          Title = d.Title,
          Description = d.Description,
          File = d.File,
          PreviewFile = d.PreviewFile,
          SteamIDOwner = d.SteamIDOwner,
          TimeCreated = d.TimeCreated,
          TimeUpdated = d.TimeUpdated,
          Visibility = d.Visibility,
          Banned = d.Banned,
          Tags = d.Tags,
          TagsTruncated = d.TagsTruncated,
          PchFileName = d.PchFileName,
          FileSize = d.FileSize,
          PreviewFileSize = d.PreviewFileSize,
          URL = d.URL,
          FileType = d.FileType,
          AcceptedForUse = d.AcceptedForUse
        };
      }
    }
  }
}
