using Engine.Common.Binders;
using System.ComponentModel;

namespace Engine.Common.Commons
{
  [EnumType("StammKind")]
  public enum StammKind
  {
    [Description("Unknown")] Unknown,
    [Description("Grey")] Grey,
    [Description("Yellow")] Yellow,
    [Description("Red")] Red,
  }
}
