// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamParamStringArray_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct SteamParamStringArray_t
  {
    public IntPtr Strings;
    public int NumStrings;

    public static SteamParamStringArray_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (SteamParamStringArray_t) (SteamParamStringArray_t.PackSmall) Marshal.PtrToStructure(p, typeof (SteamParamStringArray_t.PackSmall)) : (SteamParamStringArray_t) Marshal.PtrToStructure(p, typeof (SteamParamStringArray_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public IntPtr Strings;
      public int NumStrings;

      public static implicit operator SteamParamStringArray_t(SteamParamStringArray_t.PackSmall d)
      {
        return new SteamParamStringArray_t()
        {
          Strings = d.Strings,
          NumStrings = d.NumStrings
        };
      }
    }
  }
}
