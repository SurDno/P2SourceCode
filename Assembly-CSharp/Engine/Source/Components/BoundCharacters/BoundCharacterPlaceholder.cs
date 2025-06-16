using Engine.Common;
using Engine.Common.BoundCharacters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components.BoundCharacters
{
  [Factory(typeof (IBoundCharacterPlaceholder))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class BoundCharacterPlaceholder : EngineObject, IBoundCharacterPlaceholder, IObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> normalSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> dangerSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> deadSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> diseasedSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> undiscoveredNormalSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> largeNormalSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> largeDangerSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> largeDeadSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> largeDiseasedSprite;

    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public Gender Gender { get; protected set; }

    public Sprite NormalSprite => this.normalSprite.Value;

    public Sprite DangerSprite => this.dangerSprite.Value;

    public Sprite DiseasedSprite => this.diseasedSprite.Value;

    public Sprite DeadSprite => this.deadSprite.Value;

    public Sprite UndiscoveredNormalSprite => this.undiscoveredNormalSprite.Value;

    public Sprite LargeNormalSprite => this.largeNormalSprite.Value;

    public Sprite LargeDangerSprite => this.largeDangerSprite.Value;

    public Sprite LargeDiseasedSprite => this.largeDiseasedSprite.Value;

    public Sprite LargeDeadSprite => this.largeDeadSprite.Value;
  }
}
