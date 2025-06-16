using Engine.Common.Binders;

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
