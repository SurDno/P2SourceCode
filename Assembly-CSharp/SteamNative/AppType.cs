// Decompiled with JetBrains decompiler
// Type: SteamNative.AppType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal enum AppType
  {
    DepotOnly = -2147483648, // 0x80000000
    Invalid = 0,
    Game = 1,
    Application = 2,
    Tool = 4,
    Demo = 8,
    Media_DEPRECATED = 16, // 0x00000010
    DLC = 32, // 0x00000020
    Guide = 64, // 0x00000040
    Driver = 128, // 0x00000080
    Config = 256, // 0x00000100
    Hardware = 512, // 0x00000200
    Franchise = 1024, // 0x00000400
    Video = 2048, // 0x00000800
    Plugin = 4096, // 0x00001000
    Music = 8192, // 0x00002000
    Series = 16384, // 0x00004000
    Shortcut = 1073741824, // 0x40000000
  }
}
