using Engine.Common.Binders;

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
    __ServiceStates = 1024,
    TutorialPain = 1025,
    TutorialDiagnostics = 1026,
  }
}
