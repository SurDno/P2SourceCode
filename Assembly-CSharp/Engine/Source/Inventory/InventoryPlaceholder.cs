using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Inventory;

[Factory(typeof(IInventoryPlaceholder))]
[GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class InventoryPlaceholder : EngineObject, IInventoryPlaceholder, IObject {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected Typed<IInventoryGridLimited> grid;

	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected(Name = "Cell50", Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> imageInventoryCell;

	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected(Name = "Slot80", Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> imageInventorySlot;

	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected(Name = "Slot200", Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> imageInventorySlotBig;

	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected(Name = "Info800", Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> imageInformation;

	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected(Name = "Info350", Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> imageInformationSpecial;

	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy()]
	[Inspected(Name = "Sound Group", Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<ItemSoundGroup> soundGroup;

	public Typed<IInventoryGridLimited> TypedGrid => grid;

	[Inspected] public UnitySubAsset<Sprite> ImageInformation => imageInformation;

	[Inspected] public UnitySubAsset<Sprite> ImageInventoryCell => imageInventoryCell;

	[Inspected] public UnitySubAsset<Sprite> ImageInventorySlot => imageInventorySlot;

	[Inspected] public UnitySubAsset<Sprite> ImageInventorySlotBig => imageInventorySlotBig;

	[Inspected] public UnitySubAsset<Sprite> ImageInformationSpecial => imageInformationSpecial;

	public IInventoryGridLimited Grid => grid.Value;

	public ItemSoundGroup SoundGroup => soundGroup.Value;
}