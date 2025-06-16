// Decompiled with JetBrains decompiler
// Type: SteamNative.LeaderboardEntry_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct LeaderboardEntry_t
  {
    public ulong SteamIDUser;
    public int GlobalRank;
    public int Score;
    public int CDetails;
    public ulong UGC;

    public static LeaderboardEntry_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (LeaderboardEntry_t) (LeaderboardEntry_t.PackSmall) Marshal.PtrToStructure(p, typeof (LeaderboardEntry_t.PackSmall)) : (LeaderboardEntry_t) Marshal.PtrToStructure(p, typeof (LeaderboardEntry_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public ulong SteamIDUser;
      public int GlobalRank;
      public int Score;
      public int CDetails;
      public ulong UGC;

      public static implicit operator LeaderboardEntry_t(LeaderboardEntry_t.PackSmall d)
      {
        return new LeaderboardEntry_t()
        {
          SteamIDUser = d.SteamIDUser,
          GlobalRank = d.GlobalRank,
          Score = d.Score,
          CDetails = d.CDetails,
          UGC = d.UGC
        };
      }
    }
  }
}
