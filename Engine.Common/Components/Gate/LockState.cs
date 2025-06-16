using System.ComponentModel;
using Engine.Common.Binders;

namespace Engine.Common.Components.Gate;

[EnumType("GateLockState")]
public enum LockState {
	[Description("Unlocked")] Unlocked,
	[Description("Locked")] Locked,
	[Description("Blocked")] Blocked
}