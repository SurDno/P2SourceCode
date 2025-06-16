using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct SteamUGCDetails_t
  {
    public ulong PublishedFileId;
    public Result Result;
    public WorkshopFileType FileType;
    public uint CreatorAppID;
    public uint ConsumerAppID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
    public string Title;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
    public string Description;
    public ulong SteamIDOwner;
    public uint TimeCreated;
    public uint TimeUpdated;
    public uint TimeAddedToUserList;
    public RemoteStoragePublishedFileVisibility Visibility;
    [MarshalAs(UnmanagedType.I1)]
    public bool Banned;
    [MarshalAs(UnmanagedType.I1)]
    public bool AcceptedForUse;
    [MarshalAs(UnmanagedType.I1)]
    public bool TagsTruncated;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1025)]
    public string Tags;
    public ulong File;
    public ulong PreviewFile;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string PchFileName;
    public int FileSize;
    public int PreviewFileSize;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string URL;
    public uint VotesUp;
    public uint VotesDown;
    public float Score;
    public uint NumChildren;

    public static SteamUGCDetails_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (PackSmall) Marshal.PtrToStructure(p, typeof (PackSmall)) : (SteamUGCDetails_t) Marshal.PtrToStructure(p, typeof (SteamUGCDetails_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public ulong PublishedFileId;
      public Result Result;
      public WorkshopFileType FileType;
      public uint CreatorAppID;
      public uint ConsumerAppID;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
      public string Title;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
      public string Description;
      public ulong SteamIDOwner;
      public uint TimeCreated;
      public uint TimeUpdated;
      public uint TimeAddedToUserList;
      public RemoteStoragePublishedFileVisibility Visibility;
      [MarshalAs(UnmanagedType.I1)]
      public bool Banned;
      [MarshalAs(UnmanagedType.I1)]
      public bool AcceptedForUse;
      [MarshalAs(UnmanagedType.I1)]
      public bool TagsTruncated;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1025)]
      public string Tags;
      public ulong File;
      public ulong PreviewFile;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string PchFileName;
      public int FileSize;
      public int PreviewFileSize;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string URL;
      public uint VotesUp;
      public uint VotesDown;
      public float Score;
      public uint NumChildren;

      public static implicit operator SteamUGCDetails_t(PackSmall d)
      {
        return new SteamUGCDetails_t {
          PublishedFileId = d.PublishedFileId,
          Result = d.Result,
          FileType = d.FileType,
          CreatorAppID = d.CreatorAppID,
          ConsumerAppID = d.ConsumerAppID,
          Title = d.Title,
          Description = d.Description,
          SteamIDOwner = d.SteamIDOwner,
          TimeCreated = d.TimeCreated,
          TimeUpdated = d.TimeUpdated,
          TimeAddedToUserList = d.TimeAddedToUserList,
          Visibility = d.Visibility,
          Banned = d.Banned,
          AcceptedForUse = d.AcceptedForUse,
          TagsTruncated = d.TagsTruncated,
          Tags = d.Tags,
          File = d.File,
          PreviewFile = d.PreviewFile,
          PchFileName = d.PchFileName,
          FileSize = d.FileSize,
          PreviewFileSize = d.PreviewFileSize,
          URL = d.URL,
          VotesUp = d.VotesUp,
          VotesDown = d.VotesDown,
          Score = d.Score,
          NumChildren = d.NumChildren
        };
      }
    }
  }
}
