using System.ComponentModel;

namespace PLVirtualMachine.Common
{
  public enum EObjectCategory
  {
    [Description("")] OBJECT_CATEGORY_NONE = 0,
    [Description("Game")] OBJECT_CATEGORY_GAME = 1,
    [Description("Characters")] OBJECT_CATEGORY_CHARACTER = 2,
    [Description("Items")] OBJECT_CATEGORY_ITEM = 4,
    [Description("Geometry")] OBJECT_CATEGORY_GEOM = 8,
    [Description("Scenes")] OBJECT_CATEGORY_WORLD_GROUP = 16,
    [Description("Others")] OBJECT_CATEGORY_OTHERS = 32,
    [Description("")] OBJECT_CATEGORY_ENGINE_ITEMS = 60,
    [Description("Managers")] OBJECT_CATEGORY_MANAGERS = 64,
    [Description("")] OBJECT_CATEGORY_TEMPLATES = 126,
    [Description("Snapshots")] OBJECT_CATEGORY_SNAPSHOTS = 128,
    [Description("Quests")] OBJECT_CATEGORY_QUEST = 256,
    [Description("")] OBJECT_CATEGORY_BLUEPRINTS = 302,
    [Description("")] OBJECT_CATEGORY_LOGIC_OBJECTS = 510,
    [Description("Classes")] OBJECT_CATEGORY_CLASS = 512,
    [Description("CustomTypes")] OBJECT_CATEGORY_TYPES = 1024,
    [Description("Events")] OBJECT_CATEGORY_EVENT = 2048,
    [Description("Graphes")] OBJECT_CATEGORY_GRAPH = 4096,
    [Description("Graph elements")] OBJECT_CATEGORY_GRAPH_ELEMENT = 8192,
    [Description("Functional components")] OBJECT_CATEGORY_FUNC_COMPONENT = 16384,
    [Description("Game modes")] OBJECT_CATEGORY_GAME_MODE = 32768,
    [Description("Folders")] OBJECT_CATEGORY_FOLDER = 65536,
    OBJECT_CATEGORY_LOGIC_MAP = 131072,
    [Description("")] OBJECT_CATEGORY_ALL_OBJECTS = 139263,
  }
}
