// Decompiled with JetBrains decompiler
// Type: SteamNative.PersonaChange
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal enum PersonaChange
  {
    Name = 1,
    Status = 2,
    ComeOnline = 4,
    GoneOffline = 8,
    GamePlayed = 16, // 0x00000010
    GameServer = 32, // 0x00000020
    Avatar = 64, // 0x00000040
    JoinedSource = 128, // 0x00000080
    LeftSource = 256, // 0x00000100
    RelationshipChanged = 512, // 0x00000200
    NameFirstSet = 1024, // 0x00000400
    FacebookInfo = 2048, // 0x00000800
    Nickname = 4096, // 0x00001000
    SteamLevel = 8192, // 0x00002000
  }
}
