// Decompiled with JetBrains decompiler
// Type: SteamNative.MatchMakingKeyValuePair_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
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
      return Platform.PackSmall ? (MatchMakingKeyValuePair_t) (MatchMakingKeyValuePair_t.PackSmall) Marshal.PtrToStructure(p, typeof (MatchMakingKeyValuePair_t.PackSmall)) : (MatchMakingKeyValuePair_t) Marshal.PtrToStructure(p, typeof (MatchMakingKeyValuePair_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string Key;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string Value;

      public static implicit operator MatchMakingKeyValuePair_t(
        MatchMakingKeyValuePair_t.PackSmall d)
      {
        return new MatchMakingKeyValuePair_t()
        {
          Key = d.Key,
          Value = d.Value
        };
      }
    }
  }
}
