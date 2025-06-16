// Decompiled with JetBrains decompiler
// Type: Engine.Common.Commons.NotificationEnum
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Binders;

#nullable disable
namespace Engine.Common.Commons
{
  [EnumType("Notification")]
  public enum NotificationEnum
  {
    None = 0,
    Main_Layer = 1024, // 0x00000400
    Map = 1025, // 0x00000401
    MindMap = 1026, // 0x00000402
    Stats = 1027, // 0x00000403
    BoundCharacters = 1028, // 0x00000404
    Tooltip_Layer = 2048, // 0x00000800
    Tooltip = 2049, // 0x00000801
    Text = 2050, // 0x00000802
    LargeText = 2051, // 0x00000803
    Reputation_Layer = 3072, // 0x00000C00
    Reputation = 3073, // 0x00000C01
    Foundation = 3074, // 0x00000C02
    Item_Layer = 4096, // 0x00001000
    ItemRecieve = 4097, // 0x00001001
    ItemDrop = 4098, // 0x00001002
    ItemBroken = 4099, // 0x00001003
    Region_Layer = 5120, // 0x00001400
    Region = 5121, // 0x00001401
    MindMap_Layer = 6144, // 0x00001800
    MindMapNode = 6145, // 0x00001801
  }
}
