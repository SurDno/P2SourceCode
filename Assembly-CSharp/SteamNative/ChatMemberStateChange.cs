// Decompiled with JetBrains decompiler
// Type: SteamNative.ChatMemberStateChange
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal enum ChatMemberStateChange
  {
    Entered = 1,
    Left = 2,
    Disconnected = 4,
    Kicked = 8,
    Banned = 16, // 0x00000010
  }
}
