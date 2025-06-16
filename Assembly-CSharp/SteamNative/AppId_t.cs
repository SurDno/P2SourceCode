// Decompiled with JetBrains decompiler
// Type: SteamNative.AppId_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal struct AppId_t
  {
    public uint Value;

    public static implicit operator AppId_t(uint value)
    {
      return new AppId_t() { Value = value };
    }

    public static implicit operator uint(AppId_t value) => value.Value;
  }
}
