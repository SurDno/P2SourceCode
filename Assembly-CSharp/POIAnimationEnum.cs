using System;

[Flags]
public enum POIAnimationEnum
{
  Unknown = 1,
  S_SitAtDesk = 2,
  S_SitOnBench = 4,
  S_LeanOnWall = 8,
  S_LeanOnTable = 16,
  S_SitNearWall = 32,
  S_LieOnBed = 64,
  S_NearFire = 128,
  Q_ViewPoster = 256,
  Q_LookOutOfTheWindow = 512,
  Q_LookUnder = 1024,
  Q_LookIntoTheWindow = 2048,
  Q_ActionWithWall = 4096,
  Q_ActionWithTable = 8192,
  Q_ActionWithWardrobe = 16384,
  Q_ActionWithShelves = 32768,
  Q_ActionWithNightstand = 65536,
  Q_ActionOnFloor = 131072,
  S_ActionOnFloor = 262144,
  Q_Idle = 524288,
  Q_NearFire = 1048576,
  S_Dialog = 2097152,
  S_Loot = 4194304,
  Q_PlaygroundPlay = 8388608,
  S_PlaygroundSandbox = 16777216,
  S_PlaygroundCooperative = 33554432,
  Q_PlaygroundCooperative = 67108864,
  S_SitAtDeskRight = 134217728,
}
