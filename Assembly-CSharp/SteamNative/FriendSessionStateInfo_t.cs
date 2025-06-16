using System;
using System.Runtime.InteropServices;

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
