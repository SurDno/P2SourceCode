// Decompiled with JetBrains decompiler
// Type: SteamNative.SNetSocketState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal enum SNetSocketState
  {
    Invalid = 0,
    Connected = 1,
    Initiated = 10, // 0x0000000A
    LocalCandidatesFound = 11, // 0x0000000B
    ReceivedRemoteCandidates = 12, // 0x0000000C
    ChallengeHandshake = 15, // 0x0000000F
    Disconnecting = 21, // 0x00000015
    LocalDisconnect = 22, // 0x00000016
    TimeoutDuringConnect = 23, // 0x00000017
    RemoteEndDisconnected = 24, // 0x00000018
    ConnectionBroken = 25, // 0x00000019
  }
}
