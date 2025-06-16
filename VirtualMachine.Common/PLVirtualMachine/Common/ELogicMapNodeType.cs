using System.ComponentModel;

namespace PLVirtualMachine.Common;

public enum ELogicMapNodeType {
	[Description("Initial")] LM_NODE_TYPE_INITIAL,
	[Description("Common")] LM_NODE_TYPE_COMMON,
	[Description("Conclusion")] LM_NODE_TYPE_CONCLUSION,
	[Description("Mission")] LM_NODE_TYPE_MISSION
}