// Decompiled with JetBrains decompiler
// Type: SteamNative.gameserveritem_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  internal struct gameserveritem_t
  {
    public servernetadr_t NetAdr;
    public int Ping;
    [MarshalAs(UnmanagedType.I1)]
    public bool HadSuccessfulResponse;
    [MarshalAs(UnmanagedType.I1)]
    public bool DoNotRefresh;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string GameDir;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string Map;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string GameDescription;
    public uint AppID;
    public int Players;
    public int MaxPlayers;
    public int BotPlayers;
    [MarshalAs(UnmanagedType.I1)]
    public bool Password;
    [MarshalAs(UnmanagedType.I1)]
    public bool Secure;
    public uint TimeLastPlayed;
    public int ServerVersion;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string ServerName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string GameTags;
    public ulong SteamID;

    public static gameserveritem_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (gameserveritem_t) (gameserveritem_t.PackSmall) Marshal.PtrToStructure(p, typeof (gameserveritem_t.PackSmall)) : (gameserveritem_t) Marshal.PtrToStructure(p, typeof (gameserveritem_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public servernetadr_t NetAdr;
      public int Ping;
      [MarshalAs(UnmanagedType.I1)]
      public bool HadSuccessfulResponse;
      [MarshalAs(UnmanagedType.I1)]
      public bool DoNotRefresh;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string GameDir;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string Map;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string GameDescription;
      public uint AppID;
      public int Players;
      public int MaxPlayers;
      public int BotPlayers;
      [MarshalAs(UnmanagedType.I1)]
      public bool Password;
      [MarshalAs(UnmanagedType.I1)]
      public bool Secure;
      public uint TimeLastPlayed;
      public int ServerVersion;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string ServerName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string GameTags;
      public ulong SteamID;

      public static implicit operator gameserveritem_t(gameserveritem_t.PackSmall d)
      {
        return new gameserveritem_t()
        {
          NetAdr = d.NetAdr,
          Ping = d.Ping,
          HadSuccessfulResponse = d.HadSuccessfulResponse,
          DoNotRefresh = d.DoNotRefresh,
          GameDir = d.GameDir,
          Map = d.Map,
          GameDescription = d.GameDescription,
          AppID = d.AppID,
          Players = d.Players,
          MaxPlayers = d.MaxPlayers,
          BotPlayers = d.BotPlayers,
          Password = d.Password,
          Secure = d.Secure,
          TimeLastPlayed = d.TimeLastPlayed,
          ServerVersion = d.ServerVersion,
          ServerName = d.ServerName,
          GameTags = d.GameTags,
          SteamID = d.SteamID
        };
      }
    }
  }
}
