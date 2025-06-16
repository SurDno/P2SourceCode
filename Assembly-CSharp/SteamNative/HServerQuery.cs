// Decompiled with JetBrains decompiler
// Type: SteamNative.HServerQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal struct HServerQuery
  {
    public int Value;

    public static implicit operator HServerQuery(int value)
    {
      return new HServerQuery() { Value = value };
    }

    public static implicit operator int(HServerQuery value) => value.Value;
  }
}
