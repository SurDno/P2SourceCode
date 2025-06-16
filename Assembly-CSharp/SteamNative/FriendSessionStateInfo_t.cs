// Decompiled with JetBrains decompiler
// Type: SteamNative.FriendSessionStateInfo_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct FriendSessionStateInfo_t
  {
    public uint IOnlineSessionInstances;
    public byte IPublishedToFriendsSessionInstance;

    public static FriendSessionStateInfo_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (FriendSessionStateInfo_t) (FriendSessionStateInfo_t.PackSmall) Marshal.PtrToStructure(p, typeof (FriendSessionStateInfo_t.PackSmall)) : (FriendSessionStateInfo_t) Marshal.PtrToStructure(p, typeof (FriendSessionStateInfo_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public uint IOnlineSessionInstances;
      public byte IPublishedToFriendsSessionInstance;

      public static implicit operator FriendSessionStateInfo_t(FriendSessionStateInfo_t.PackSmall d)
      {
        return new FriendSessionStateInfo_t()
        {
          IOnlineSessionInstances = d.IOnlineSessionInstances,
          IPublishedToFriendsSessionInstance = d.IPublishedToFriendsSessionInstance
        };
      }
    }
  }
}
