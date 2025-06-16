using System.ComponentModel;

namespace PLVirtualMachine.Common;

public enum EEventRaisingType {
	[Description("Condition")] EVENT_RAISING_TYPE_CONDITION,
	[Description("Parameter change")] EVENT_RAISING_TYPE_PARAM_CHANGE,
	[Description("Time")] EVENT_RAISING_TYPE_TIME,
	[Description("By engine")] EVENT_RAISING_TYPE_BY_ENGINE
}