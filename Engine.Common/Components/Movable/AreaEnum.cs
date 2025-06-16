// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.Movable.AreaEnum
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Binders;
using Engine.Common.Commons;
using System.ComponentModel;

#nullable disable
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
    [Description("All")] All = 256, // 0x00000100
    [Description("Road FootPath")] RoadFootPath = 257, // 0x00000101
    [Description("Road FootPath Walkable")] RoadFootPathWalkable = 258, // 0x00000102
    __EndMasks = 512, // 0x00000200
    [Group("Default"), Description("Agony")] Agony = 513, // 0x00000201
    [Description("Collectable")] Collectable = 514, // 0x00000202
    [Description("Family")] Family = 515, // 0x00000203
    [Description("Diseased")] Diseased = 516, // 0x00000204
    [Description("Sick")] Sick = 517, // 0x00000205
    [Description("PlagueCloud")] PlagueCloud = 518, // 0x00000206
    __CustomPoints = 1024, // 0x00000400
    [Group("PlayGrounds")] __PlayGround = 1025, // 0x00000401
    PlayGround1 = 1026, // 0x00000402
    PlayGround2 = 1027, // 0x00000403
    PlayGround3 = 1028, // 0x00000404
    [Group("Shaika")] __Shaika = 1089, // 0x00000441
    Shaika1 = 1090, // 0x00000442
    Shaika2 = 1091, // 0x00000443
    Shaika3 = 1092, // 0x00000444
    [Group("SickPlaces")] __SickPlace = 1153, // 0x00000481
    SickPlace1 = 1154, // 0x00000482
    SickPlace2 = 1155, // 0x00000483
    SickPlace3 = 1156, // 0x00000484
    [Group("Bonfires")] __Bonfires = 1217, // 0x000004C1
    Bonfires1 = 1218, // 0x000004C2
    Bonfires2 = 1219, // 0x000004C3
    Bonfires3 = 1220, // 0x000004C4
    [Category("CitizensIdles")] __CitizensIdles = 1281, // 0x00000501
    CitizensIdles1 = 1282, // 0x00000502
    CitizensIdles2 = 1283, // 0x00000503
    CitizensIdles3 = 1284, // 0x00000504
    [Category("CitizensWalkable")] __CitizensWalkable = 1345, // 0x00000541
    CitizensWalkable1 = 1346, // 0x00000542
    CitizensWalkable2 = 1347, // 0x00000543
    CitizensWalkable3 = 1348, // 0x00000544
    [Category("MarketPlace")] __MarketPlace = 1409, // 0x00000581
    MarketPlace1 = 1410, // 0x00000582
    MarketPlace2 = 1411, // 0x00000583
    MarketPlace3 = 1412, // 0x00000584
    [Category("Family")] __Family = 1473, // 0x000005C1
    FamilyAdult = 1474, // 0x000005C2
    FamilyTeen = 1475, // 0x000005C3
    FamilyKid = 1476, // 0x000005C4
    [Category("Diseased")] __Diseased = 1537, // 0x00000601
    Diseased1 = 1538, // 0x00000602
    Diseased2 = 1539, // 0x00000603
    Diseased3 = 1540, // 0x00000604
    [Category("PlagueCloud")] __PlagueCloud = 1601, // 0x00000641
    PlagueCloudStatic = 1602, // 0x00000642
    PlagueCloudAmbush = 1603, // 0x00000643
    PlagueCloudHunt = 1604, // 0x00000644
    PlagueCloudBullet = 1605, // 0x00000645
    PlagueCloudPatrol = 1606, // 0x00000646
    [Category("TownPatrol")] __TownPatrol = 1665, // 0x00000681
    TownPatrolStatic = 1666, // 0x00000682
    TownPatrolDynamic = 1667, // 0x00000683
    TownPatrolIndoor = 1668, // 0x00000684
    [Category("Barricades")] __Barricades = 1729, // 0x000006C1
    Barricade1 = 1730, // 0x000006C2
    Barricade2 = 1731, // 0x000006C3
    Barricade3 = 1732, // 0x000006C4
    Barricade4 = 1733, // 0x000006C5
    Barricade5 = 1734, // 0x000006C6
    Barricade6 = 1735, // 0x000006C7
    [Category("Patient")] __Patient = 1793, // 0x00000701
    Patient1 = 1794, // 0x00000702
    Patient2 = 1795, // 0x00000703
    Patient3 = 1796, // 0x00000704
    [Category("Herb")] __Herb = 1857, // 0x00000741
    Herb1_1 = 1858, // 0x00000742
    Herb1_2 = 1859, // 0x00000743
    Herb2_1 = 1860, // 0x00000744
    Herb2_2 = 1861, // 0x00000745
    Herb3_1 = 1862, // 0x00000746
    Herb3_2 = 1863, // 0x00000747
    Herb_Special = 1864, // 0x00000748
    [Category("Thug")] __Thug = 1921, // 0x00000781
    Thug1 = 1922, // 0x00000782
    Thug2 = 1923, // 0x00000783
    [Category("Corpse")] __Corpse = 1985, // 0x000007C1
    Corpse1 = 1986, // 0x000007C2
    Corpse2 = 1987, // 0x000007C3
    [Category("Bride")] __Bride = 2049, // 0x00000801
    DancingBride = 2050, // 0x00000802
    [Category("Bull")] __Bull = 2113, // 0x00000841
    Bull_1 = 2114, // 0x00000842
    Bull_2 = 2115, // 0x00000843
  }
}
