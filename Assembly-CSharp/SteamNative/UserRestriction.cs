// Decompiled with JetBrains decompiler
// Type: SteamNative.UserRestriction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal enum UserRestriction
  {
    None = 0,
    Unknown = 1,
    AnyChat = 2,
    VoiceChat = 4,
    GroupChat = 8,
    Rating = 16, // 0x00000010
    GameInvites = 32, // 0x00000020
    Trading = 64, // 0x00000040
  }
}
