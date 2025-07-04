﻿using System.ComponentModel;
using Engine.Common.Binders;
using Engine.Common.Commons;

namespace Engine.Common.Components.Movable
{
  [EnumType("Area")]
  public enum AreaEnum
  {
    [Group("NavMesh"), Description("Unknown")] Unknown = 0,
    [Description("Walkable")] Walkable = 1,
    [Description("NotWalkable")] NotWalkable = 2,
    [Description("Jump")] Jump = 3,
    [Description("Indoor")] Indoor = 4,
    [Description("Outdoor")] Outdoor = 5,
    [Description("FootPath")] FootPath = 6,
    [Description("Road")] Road = 7,
    [Description("All")] All = 256,
    [Description("Road FootPath")] RoadFootPath = 257,
    [Description("Road FootPath Walkable")] RoadFootPathWalkable = 258,
    __EndMasks = 512,
    [Group("Default"), Description("Agony")] Agony = 513,
    [Description("Collectable")] Collectable = 514,
    [Description("Family")] Family = 515,
    [Description("Diseased")] Diseased = 516,
    [Description("Sick")] Sick = 517,
    [Description("PlagueCloud")] PlagueCloud = 518,
    __CustomPoints = 1024,
    [Group("PlayGrounds")] __PlayGround = 1025,
    PlayGround1 = 1026,
    PlayGround2 = 1027,
    PlayGround3 = 1028,
    [Group("Shaika")] __Shaika = 1089,
    Shaika1 = 1090,
    Shaika2 = 1091,
    Shaika3 = 1092,
    [Group("SickPlaces")] __SickPlace = 1153,
    SickPlace1 = 1154,
    SickPlace2 = 1155,
    SickPlace3 = 1156,
    [Group("Bonfires")] __Bonfires = 1217,
    Bonfires1 = 1218,
    Bonfires2 = 1219,
    Bonfires3 = 1220,
    [Category("CitizensIdles")] __CitizensIdles = 1281,
    CitizensIdles1 = 1282,
    CitizensIdles2 = 1283,
    CitizensIdles3 = 1284,
    [Category("CitizensWalkable")] __CitizensWalkable = 1345,
    CitizensWalkable1 = 1346,
    CitizensWalkable2 = 1347,
    CitizensWalkable3 = 1348,
    [Category("MarketPlace")] __MarketPlace = 1409,
    MarketPlace1 = 1410,
    MarketPlace2 = 1411,
    MarketPlace3 = 1412,
    [Category("Family")] __Family = 1473,
    FamilyAdult = 1474,
    FamilyTeen = 1475,
    FamilyKid = 1476,
    [Category("Diseased")] __Diseased = 1537,
    Diseased1 = 1538,
    Diseased2 = 1539,
    Diseased3 = 1540,
    [Category("PlagueCloud")] __PlagueCloud = 1601,
    PlagueCloudStatic = 1602,
    PlagueCloudAmbush = 1603,
    PlagueCloudHunt = 1604,
    PlagueCloudBullet = 1605,
    PlagueCloudPatrol = 1606,
    [Category("TownPatrol")] __TownPatrol = 1665,
    TownPatrolStatic = 1666,
    TownPatrolDynamic = 1667,
    TownPatrolIndoor = 1668,
    [Category("Barricades")] __Barricades = 1729,
    Barricade1 = 1730,
    Barricade2 = 1731,
    Barricade3 = 1732,
    Barricade4 = 1733,
    Barricade5 = 1734,
    Barricade6 = 1735,
    [Category("Patient")] __Patient = 1793,
    Patient1 = 1794,
    Patient2 = 1795,
    Patient3 = 1796,
    [Category("Herb")] __Herb = 1857,
    Herb1_1 = 1858,
    Herb1_2 = 1859,
    Herb2_1 = 1860,
    Herb2_2 = 1861,
    Herb3_1 = 1862,
    Herb3_2 = 1863,
    Herb_Special = 1864,
    [Category("Thug")] __Thug = 1921,
    Thug1 = 1922,
    Thug2 = 1923,
    [Category("Corpse")] __Corpse = 1985,
    Corpse1 = 1986,
    Corpse2 = 1987,
    [Category("Bride")] __Bride = 2049,
    DancingBride = 2050,
    [Category("Bull")] __Bull = 2113,
    Bull_1 = 2114,
    Bull_2 = 2115,
  }
}
