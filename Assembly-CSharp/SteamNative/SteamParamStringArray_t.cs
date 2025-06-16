using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct SteamParamStringArray_t
  {
    public IntPtr Strings;
    public int NumStrings;

    public static SteamParamStringArray_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (PackSmall) Marshal.PtrToStructure(p, typeof (PackSmall)) : (SteamParamStringArray_t) Marshal.PtrToStructure(p, typeof (SteamParamStringArray_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public IntPtr Strings;
      public int NumStrings;

      public static implicit operator SteamParamStringArray_t(PackSmall d)
      {
        return new SteamParamStringArray_t {
          Strings = d.Strings,
          NumStrings = d.NumStrings
        };
      }
    }
  }
}
