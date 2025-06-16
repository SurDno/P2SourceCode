// Decompiled with JetBrains decompiler
// Type: SteamNative.LaunchOptionType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal enum LaunchOptionType
  {
    None = 0,
    Default = 1,
    SafeMode = 2,
    Multiplayer = 3,
    Config = 4,
    OpenVR = 5,
    Server = 6,
    Editor = 7,
    Manual = 8,
    Benchmark = 9,
    Option1 = 10, // 0x0000000A
    Option2 = 11, // 0x0000000B
    Option3 = 12, // 0x0000000C
    OculusVR = 13, // 0x0000000D
    OpenVROverlay = 14, // 0x0000000E
    OSVR = 15, // 0x0000000F
    Dialog = 1000, // 0x000003E8
  }
}
