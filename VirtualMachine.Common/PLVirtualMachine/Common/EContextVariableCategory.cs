// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EContextVariableCategory
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.ComponentModel;

#nullable disable
namespace PLVirtualMachine.Common
{
  public enum EContextVariableCategory
  {
    [Description("Game")] CONTEXT_VARIABLE_CATEGORY_GAME = 0,
    [Description("Objects")] CONTEXT_VARIABLE_CATEGORY_OBJECT = 1,
    [Description("Events")] CONTEXT_VARIABLE_CATEGORY_EVENT = 2,
    [Description("Functions")] CONTEXT_VARIABLE_CATEGORY_APIFUNCTION = 4,
    [Description("Params")] CONTEXT_VARIABLE_CATEGORY_PARAM = 8,
    [Description("Global variables")] CONTEXT_VARIABLE_CATEGORY_GLOBAL_VAR = 16, // 0x00000010
    [Description("Local variables")] CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR = 32, // 0x00000020
    [Description("Messages")] CONTEXT_VARIABLE_CATEGORY_MESSAGE = 64, // 0x00000040
    [Description("States")] CONTEXT_VARIABLE_CATEGORY_STATE = 128, // 0x00000080
    [Description("Templates")] CONTEXT_VARIABLE_CATEGORY_BLUEPRINT = 256, // 0x00000100
    [Description("Samples")] CONTEXT_VARIABLE_CATEGORY_SAMPLE = 512, // 0x00000200
    [Description("Statuses")] CONTEXT_VARIABLE_CATEGORY_STATUS = 1024, // 0x00000400
    [Description("Classes")] CONTEXT_VARIABLE_CATEGORY_CLASS = 2048, // 0x00000800
    [Description("GameText")] CONTEXT_VARIABLE_CATEGORY_TEXT = 4096, // 0x00001000
    [Description("MindMap")] CONTEXT_VARIABLE_CATEGORY_LOGIC_MAP = 8192, // 0x00002000
    [Description("LogicMapNode")] CONTEXT_VARIABLE_CATEGORY_LOGIC_MAP_NODE = 16383, // 0x00003FFF
    [Description("GameMode")] CONTEXT_VARIABLE_CATEGORY_GAMEMODE = 32766, // 0x00007FFE
    [Description("")] CONTEXT_VARIABLE_CATEGORY_ALL = 65531, // 0x0000FFFB
  }
}
