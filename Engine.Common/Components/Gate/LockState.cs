using Engine.Common.Binders;
using System.ComponentModel;

namespace Engine.Common.Components.Gate
{
  [EnumType("GateLockState")]
  public enum LockState
  {
    [Description("Unlocked")] Unlocked,
    [Description("Locked")] Locked,
    [Description("Blocked")] Blocked,
  }
}
