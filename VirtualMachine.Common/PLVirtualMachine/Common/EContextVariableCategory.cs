using System.ComponentModel;

namespace PLVirtualMachine.Common;

public enum EContextVariableCategory {
	[Description("Game")] CONTEXT_VARIABLE_CATEGORY_GAME = 0,
	[Description("Objects")] CONTEXT_VARIABLE_CATEGORY_OBJECT = 1,
	[Description("Events")] CONTEXT_VARIABLE_CATEGORY_EVENT = 2,
	[Description("Functions")] CONTEXT_VARIABLE_CATEGORY_APIFUNCTION = 4,
	[Description("Params")] CONTEXT_VARIABLE_CATEGORY_PARAM = 8,
	[Description("Global variables")] CONTEXT_VARIABLE_CATEGORY_GLOBAL_VAR = 16,
	[Description("Local variables")] CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR = 32,
	[Description("Messages")] CONTEXT_VARIABLE_CATEGORY_MESSAGE = 64,
	[Description("States")] CONTEXT_VARIABLE_CATEGORY_STATE = 128,
	[Description("Templates")] CONTEXT_VARIABLE_CATEGORY_BLUEPRINT = 256,
	[Description("Samples")] CONTEXT_VARIABLE_CATEGORY_SAMPLE = 512,
	[Description("Statuses")] CONTEXT_VARIABLE_CATEGORY_STATUS = 1024,
	[Description("Classes")] CONTEXT_VARIABLE_CATEGORY_CLASS = 2048,
	[Description("GameText")] CONTEXT_VARIABLE_CATEGORY_TEXT = 4096,
	[Description("MindMap")] CONTEXT_VARIABLE_CATEGORY_LOGIC_MAP = 8192,
	[Description("LogicMapNode")] CONTEXT_VARIABLE_CATEGORY_LOGIC_MAP_NODE = 16383,
	[Description("GameMode")] CONTEXT_VARIABLE_CATEGORY_GAMEMODE = 32766,
	[Description("")] CONTEXT_VARIABLE_CATEGORY_ALL = 65531
}