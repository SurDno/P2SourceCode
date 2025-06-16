// Decompiled with JetBrains decompiler
// Type: POIAnimationEnum
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
[Flags]
public enum POIAnimationEnum
{
  Unknown = 1,
  S_SitAtDesk = 2,
  S_SitOnBench = 4,
  S_LeanOnWall = 8,
  S_LeanOnTable = 16, // 0x00000010
  S_SitNearWall = 32, // 0x00000020
  S_LieOnBed = 64, // 0x00000040
  S_NearFire = 128, // 0x00000080
  Q_ViewPoster = 256, // 0x00000100
  Q_LookOutOfTheWindow = 512, // 0x00000200
  Q_LookUnder = 1024, // 0x00000400
  Q_LookIntoTheWindow = 2048, // 0x00000800
  Q_ActionWithWall = 4096, // 0x00001000
  Q_ActionWithTable = 8192, // 0x00002000
  Q_ActionWithWardrobe = 16384, // 0x00004000
  Q_ActionWithShelves = 32768, // 0x00008000
  Q_ActionWithNightstand = 65536, // 0x00010000
  Q_ActionOnFloor = 131072, // 0x00020000
  S_ActionOnFloor = 262144, // 0x00040000
  Q_Idle = 524288, // 0x00080000
  Q_NearFire = 1048576, // 0x00100000
  S_Dialog = 2097152, // 0x00200000
  S_Loot = 4194304, // 0x00400000
  Q_PlaygroundPlay = 8388608, // 0x00800000
  S_PlaygroundSandbox = 16777216, // 0x01000000
  S_PlaygroundCooperative = 33554432, // 0x02000000
  Q_PlaygroundCooperative = 67108864, // 0x04000000
  S_SitAtDeskRight = 134217728, // 0x08000000
}
