using Engine.Common.Binders;
using System.ComponentModel;

namespace Engine.Common.Components.Attacker
{
  [EnumType("AttackerFinishType")]
  public enum FinishKind
  {
    [Description("Frontal punch leading to finishing pose")] FrontPunchFinish = 1,
  }
}
