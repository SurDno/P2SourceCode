// Decompiled with JetBrains decompiler
// Type: SteamNative.RTime32
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal struct RTime32
  {
    public uint Value;

    public static implicit operator RTime32(uint value)
    {
      return new RTime32() { Value = value };
    }

    public static implicit operator uint(RTime32 value) => value.Value;
  }
}
