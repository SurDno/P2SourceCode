using System.ComponentModel;

namespace PLVirtualMachine.Common;

public enum EActionType {
	[Description("None")] ACTION_TYPE_NONE,
	[Description("Set param")] ACTION_TYPE_SET_PARAM,
	[Description("Set expression")] ACTION_TYPE_SET_EXPRESSION,
	[Description("Math operation")] ACTION_TYPE_MATH,
	[Description("Function call")] ACTION_TYPE_DO_FUNCTION,
	[Description("Event raising")] ACTION_TYPE_RAISE_EVENT
}