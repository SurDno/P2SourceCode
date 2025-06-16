using Engine.Common.Binders;

namespace Engine.Common.Commons;

[EnumType("Notification")]
public enum NotificationEnum {
	None = 0,
	Main_Layer = 1024,
	Map = 1025,
	MindMap = 1026,
	Stats = 1027,
	BoundCharacters = 1028,
	Tooltip_Layer = 2048,
	Tooltip = 2049,
	Text = 2050,
	LargeText = 2051,
	Reputation_Layer = 3072,
	Reputation = 3073,
	Foundation = 3074,
	Item_Layer = 4096,
	ItemRecieve = 4097,
	ItemDrop = 4098,
	ItemBroken = 4099,
	Region_Layer = 5120,
	Region = 5121,
	MindMap_Layer = 6144,
	MindMapNode = 6145
}