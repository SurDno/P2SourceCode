// Decompiled with JetBrains decompiler
// Type: SteamNative.HServerListRequest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace SteamNative
{
  internal struct HServerListRequest
  {
    public IntPtr Value;

    public static implicit operator HServerListRequest(IntPtr value)
    {
      return new HServerListRequest() { Value = value };
    }

    public static implicit operator IntPtr(HServerListRequest value) => value.Value;
  }
}
