using System.ComponentModel;
using Engine.Common.Binders;

namespace Engine.Common.Components
{
  [EnumType("EntityTag")]
  public enum EntityTagEnum
  {
    None = 0,
    DiseasedLevels = 100, // 0x00000064
    DiseasedLevel0 = 101, // 0x00000065
    DiseasedLevel1 = 102, // 0x00000066
    DiseasedLevel2 = 103, // 0x00000067
    DiseasedLevel3 = 104, // 0x00000068
    DiseasedLevel4 = 105, // 0x00000069
    DiseasedLevel5 = 106, // 0x0000006A
    DiseasedScene = 107, // 0x0000006B
    DiseasedIndoorCorpses = 108, // 0x0000006C
    __QuestsTags = 1024, // 0x00000400
    [Category("D4-7_Event_Phantom")] __Event_Phantom = 1025, // 0x00000401
    Event_Phantom_ObjectsEnabled = 1026, // 0x00000402
    Event_Phantom_ObjectsDisabled = 1027, // 0x00000403
    Event_Phantom_OpenedDoor = 1028, // 0x00000404
    Event_Phantom_SpawnNPC = 1029, // 0x00000405
    Event_Phantom_SpawnNPCSoundOutdoor = 1030, // 0x00000406
    Event_Phantom_SpawnNPCSoundIndoor = 1031, // 0x00000407
    [Category("D4-7_Event_Marooned")] __Event_Marooned = 1089, // 0x00000441
    Event_Marooned_ObjectsEnabled = 1090, // 0x00000442
    Event_Marooned_ObjectsDisabled = 1091, // 0x00000443
    Event_Marooned_SpawnKaterina = 1092, // 0x00000444
    [Category("D2-3_Event_TheWalk")] __Event_TheWalk = 1153, // 0x00000481
    Event_TheWalk_ObjectsEnabled = 1154, // 0x00000482
    Event_TheWalk_ObjectsDisabled = 1155, // 0x00000483
    [Category("Event_Town_Reflections")] __Event_Town_Reflections = 1217, // 0x000004C1
    Event_Town_Reflections_ObjectsEnabled = 1218, // 0x000004C2
    Event_Town_Reflections_ObjectsDisabled = 1219, // 0x000004C3
    Event_Town_Reflections_SpawnNPC = 1220, // 0x000004C4
    Event_Town_Reflections_AdultsCrowd = 1221, // 0x000004C5
    Event_Town_Reflections_KidsCrowd = 1222, // 0x000004C6
    [Category("Water_Supply")] __Water_Supply = 1281, // 0x00000501
    Water_Supply_Hydrant = 1282, // 0x00000502
    Water_Supply_Barrel = 1283, // 0x00000503
    [Category("Event_BadEnding")] __Event_BadEnding = 1345, // 0x00000541
    Event_BadEnding_ObjectsEnabled = 1346, // 0x00000542
    Event_BadEnding_ObjectsDisabled = 1347, // 0x00000543
  }
}
