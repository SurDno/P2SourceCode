// Decompiled with JetBrains decompiler
// Type: SteamNative.BREAKPAD_HANDLE
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace SteamNative
{
  internal struct BREAKPAD_HANDLE
  {
    public IntPtr Value;

    public static implicit operator BREAKPAD_HANDLE(IntPtr value)
    {
      return new BREAKPAD_HANDLE() { Value = value };
    }

    public static implicit operator IntPtr(BREAKPAD_HANDLE value) => value.Value;
  }
}
