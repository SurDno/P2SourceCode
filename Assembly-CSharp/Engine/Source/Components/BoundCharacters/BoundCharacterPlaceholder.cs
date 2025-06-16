using Engine.Common;
using Engine.Common.BoundCharacters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components.BoundCharacters;

[Factory(typeof(IBoundCharacterPlaceholder))]
[GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class BoundCharacterPlaceholder : EngineObject, IBoundCharacterPlaceholder, IObject {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> normalSprite;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> dangerSprite;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> deadSprite;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> diseasedSprite;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> undiscoveredNormalSprite;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> largeNormalSprite;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> largeDangerSprite;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> largeDeadSprite;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<Sprite> largeDiseasedSprite;

	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	public Gender Gender { get; protected set; }

	public Sprite NormalSprite => normalSprite.Value;

	public Sprite DangerSprite => dangerSprite.Value;

	public Sprite DiseasedSprite => diseasedSprite.Value;

	public Sprite DeadSprite => deadSprite.Value;

	public Sprite UndiscoveredNormalSprite => undiscoveredNormalSprite.Value;

	public Sprite LargeNormalSprite => largeNormalSprite.Value;

	public Sprite LargeDangerSprite => largeDangerSprite.Value;

	public Sprite LargeDiseasedSprite => largeDiseasedSprite.Value;

	public Sprite LargeDeadSprite => largeDeadSprite.Value;
}