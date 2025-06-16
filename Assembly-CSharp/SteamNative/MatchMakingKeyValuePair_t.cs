using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct MatchMakingKeyValuePair_t
  {
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string Key;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string Value;

    public static MatchMakingKeyValuePair_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (PackSmall) Marshal.PtrToStructure(p, typeof (PackSmall)) : (MatchMakingKeyValuePair_t) Marshal.PtrToStructure(p, typeof (MatchMakingKeyValuePair_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string Key;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string Value;

      public static implicit operator MatchMakingKeyValuePair_t(
        PackSmall d)
      {
        return new MatchMakingKeyValuePair_t {
          Key = d.Key,
          Value = d.Value
        };
      }
    }
  }
}
