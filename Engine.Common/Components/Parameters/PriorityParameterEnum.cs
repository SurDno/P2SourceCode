// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.Parameters.PriorityParameterEnum
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Binders;

#nullable disable
namespace Engine.Common.Components.Parameters
{
  [EnumType("PriorityParameter")]
  public enum PriorityParameterEnum
  {
    None = 0,
    Default = 100, // 0x00000064
    RandomQuest = 200, // 0x000000C8
    Quest = 300, // 0x0000012C
  }
}
