// Decompiled with JetBrains decompiler
// Type: SteamNative.HSteamPipe
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal struct HSteamPipe
  {
    public int Value;

    public static implicit operator HSteamPipe(int value)
    {
      return new HSteamPipe() { Value = value };
    }

    public static implicit operator int(HSteamPipe value) => value.Value;
  }
}
