// Decompiled with JetBrains decompiler
// Type: SteamNative.HTTPRequestHandle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal struct HTTPRequestHandle
  {
    public uint Value;

    public static implicit operator HTTPRequestHandle(uint value)
    {
      return new HTTPRequestHandle() { Value = value };
    }

    public static implicit operator uint(HTTPRequestHandle value) => value.Value;
  }
}
