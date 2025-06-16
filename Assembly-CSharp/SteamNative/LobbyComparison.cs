// Decompiled with JetBrains decompiler
// Type: SteamNative.LobbyComparison
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal enum LobbyComparison
  {
    EqualToOrLessThan = -2, // 0xFFFFFFFE
    LessThan = -1, // 0xFFFFFFFF
    Equal = 0,
    GreaterThan = 1,
    EqualToOrGreaterThan = 2,
    NotEqual = 3,
  }
}
