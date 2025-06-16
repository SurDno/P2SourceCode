using Engine.Common.Binders;

namespace Engine.Common.Components.Parameters
{
  [EnumType("PriorityParameter")]
  public enum PriorityParameterEnum
  {
    None = 0,
    Default = 100,
    RandomQuest = 200,
    Quest = 300,
  }
}
