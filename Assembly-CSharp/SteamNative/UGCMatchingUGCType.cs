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
