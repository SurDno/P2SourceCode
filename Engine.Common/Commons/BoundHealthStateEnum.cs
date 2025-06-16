// Decompiled with JetBrains decompiler
// Type: Engine.Common.Commons.BoundHealthStateEnum
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Binders;

#nullable disable
namespace Engine.Common.Commons
{
  [EnumType("BoundHealthStateEnum")]
  public enum BoundHealthStateEnum
  {
    None = 0,
    Normal = 1,
    Danger = 2,
    Diseased = 3,
    Dead = 4,
    __ServiceStates = 1024, // 0x00000400
    TutorialPain = 1025, // 0x00000401
    TutorialDiagnostics = 1026, // 0x00000402
  }
}
