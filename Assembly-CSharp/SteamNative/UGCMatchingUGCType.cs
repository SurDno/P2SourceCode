// Decompiled with JetBrains decompiler
// Type: SteamNative.UGCMatchingUGCType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal enum UGCMatchingUGCType
  {
    All = -1, // 0xFFFFFFFF
    Items = 0,
    Items_Mtx = 1,
    Items_ReadyToUse = 2,
    Collections = 3,
    Artwork = 4,
    Videos = 5,
    Screenshots = 6,
    AllGuides = 7,
    WebGuides = 8,
    IntegratedGuides = 9,
    UsableInGame = 10, // 0x0000000A
    ControllerBindings = 11, // 0x0000000B
    GameManagedItems = 12, // 0x0000000C
  }
}
