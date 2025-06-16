using System.ComponentModel;
using Engine.Common.Binders;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[EnumType("GateState")]
public enum GateState {
	[Description("Closed")] Closed,
	[Description("Opened")] Opened
}