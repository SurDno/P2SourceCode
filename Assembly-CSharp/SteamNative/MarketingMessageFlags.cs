// Decompiled with JetBrains decompiler
// Type: SteamNative.MarketingMessageFlags
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal enum MarketingMessageFlags
  {
    None = 0,
    HighPriority = 1,
    PlatformWindows = 2,
    PlatformMac = 4,
    PlatformLinux = 8,
    PlatformRestrictions = 14, // 0x0000000E
  }
}
