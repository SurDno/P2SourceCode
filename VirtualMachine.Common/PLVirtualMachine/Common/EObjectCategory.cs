// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EObjectCategory
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.ComponentModel;

#nullable disable
namespace PLVirtualMachine.Common
{
  public enum EObjectCategory
  {
    [Description("")] OBJECT_CATEGORY_NONE = 0,
    [Description("Game")] OBJECT_CATEGORY_GAME = 1,
    [Description("Characters")] OBJECT_CATEGORY_CHARACTER = 2,
    [Description("Items")] OBJECT_CATEGORY_ITEM = 4,
    [Description("Geometry")] OBJECT_CATEGORY_GEOM = 8,
    [Description("Scenes")] OBJECT_CATEGORY_WORLD_GROUP = 16, // 0x00000010
    [Description("Others")] OBJECT_CATEGORY_OTHERS = 32, // 0x00000020
    [Description("")] OBJECT_CATEGORY_ENGINE_ITEMS = 60, // 0x0000003C
    [Description("Managers")] OBJECT_CATEGORY_MANAGERS = 64, // 0x00000040
    [Description("")] OBJECT_CATEGORY_TEMPLATES = 126, // 0x0000007E
    [Description("Snapshots")] OBJECT_CATEGORY_SNAPSHOTS = 128, // 0x00000080
    [Description("Quests")] OBJECT_CATEGORY_QUEST = 256, // 0x00000100
    [Description("")] OBJECT_CATEGORY_BLUEPRINTS = 302, // 0x0000012E
    [Description("")] OBJECT_CATEGORY_LOGIC_OBJECTS = 510, // 0x000001FE
    [Description("Classes")] OBJECT_CATEGORY_CLASS = 512, // 0x00000200
    [Description("CustomTypes")] OBJECT_CATEGORY_TYPES = 1024, // 0x00000400
    [Description("Events")] OBJECT_CATEGORY_EVENT = 2048, // 0x00000800
    [Description("Graphes")] OBJECT_CATEGORY_GRAPH = 4096, // 0x00001000
    [Description("Graph elements")] OBJECT_CATEGORY_GRAPH_ELEMENT = 8192, // 0x00002000
    [Description("Functional components")] OBJECT_CATEGORY_FUNC_COMPONENT = 16384, // 0x00004000
    [Description("Game modes")] OBJECT_CATEGORY_GAME_MODE = 32768, // 0x00008000
    [Description("Folders")] OBJECT_CATEGORY_FOLDER = 65536, // 0x00010000
    OBJECT_CATEGORY_LOGIC_MAP = 131072, // 0x00020000
    [Description("")] OBJECT_CATEGORY_ALL_OBJECTS = 139263, // 0x00021FFF
  }
}
