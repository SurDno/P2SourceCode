using System.ComponentModel;
using Engine.Common.Binders;

namespace Engine.Common.Components;

[EnumType("EntityTag")]
public enum EntityTagEnum {
	None = 0,
	DiseasedLevels = 100,
	DiseasedLevel0 = 101,
	DiseasedLevel1 = 102,
	DiseasedLevel2 = 103,
	DiseasedLevel3 = 104,
	DiseasedLevel4 = 105,
	DiseasedLevel5 = 106,
	DiseasedScene = 107,
	DiseasedIndoorCorpses = 108,
	__QuestsTags = 1024,
	[Category("D4-7_Event_Phantom")] __Event_Phantom = 1025,
	Event_Phantom_ObjectsEnabled = 1026,
	Event_Phantom_ObjectsDisabled = 1027,
	Event_Phantom_OpenedDoor = 1028,
	Event_Phantom_SpawnNPC = 1029,
	Event_Phantom_SpawnNPCSoundOutdoor = 1030,
	Event_Phantom_SpawnNPCSoundIndoor = 1031,
	[Category("D4-7_Event_Marooned")] __Event_Marooned = 1089,
	Event_Marooned_ObjectsEnabled = 1090,
	Event_Marooned_ObjectsDisabled = 1091,
	Event_Marooned_SpawnKaterina = 1092,
	[Category("D2-3_Event_TheWalk")] __Event_TheWalk = 1153,
	Event_TheWalk_ObjectsEnabled = 1154,
	Event_TheWalk_ObjectsDisabled = 1155,
	[Category("Event_Town_Reflections")] __Event_Town_Reflections = 1217,
	Event_Town_Reflections_ObjectsEnabled = 1218,
	Event_Town_Reflections_ObjectsDisabled = 1219,
	Event_Town_Reflections_SpawnNPC = 1220,
	Event_Town_Reflections_AdultsCrowd = 1221,
	Event_Town_Reflections_KidsCrowd = 1222,
	[Category("Water_Supply")] __Water_Supply = 1281,
	Water_Supply_Hydrant = 1282,
	Water_Supply_Barrel = 1283,
	[Category("Event_BadEnding")] __Event_BadEnding = 1345,
	Event_BadEnding_ObjectsEnabled = 1346,
	Event_BadEnding_ObjectsDisabled = 1347
}