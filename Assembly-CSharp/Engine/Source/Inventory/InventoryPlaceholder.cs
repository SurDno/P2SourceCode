using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Inventory
{
  [Factory(typeof (IInventoryPlaceholder))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class InventoryPlaceholder : EngineObject, IInventoryPlaceholder, IObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<IInventoryGridLimited> grid;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Name = "Cell50", Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> imageInventoryCell;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Name = "Slot80", Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> imageInventorySlot;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Name = "Slot200", Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> imageInventorySlotBig;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Name = "Info800", Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> imageInformation;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Name = "Info350", Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> imageInformationSpecial;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Name = "Sound Group", Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<ItemSoundGroup> soundGroup;

    public Typed<IInventoryGridLimited> TypedGrid => this.grid;

    [Inspected]
    public UnitySubAsset<Sprite> ImageInformation => this.imageInformation;

    [Inspected]
    public UnitySubAsset<Sprite> ImageInventoryCell => this.imageInventoryCell;

    [Inspected]
    public UnitySubAsset<Sprite> ImageInventorySlot => this.imageInventorySlot;

    [Inspected]
    public UnitySubAsset<Sprite> ImageInventorySlotBig => this.imageInventorySlotBig;

    [Inspected]
    public UnitySubAsset<Sprite> ImageInformationSpecial => this.imageInformationSpecial;

    public IInventoryGridLimited Grid => this.grid.Value;

    public ItemSoundGroup SoundGroup => this.soundGroup.Value;
  }
}
