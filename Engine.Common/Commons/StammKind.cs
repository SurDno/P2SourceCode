using System.ComponentModel;
using Engine.Common.Binders;

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
