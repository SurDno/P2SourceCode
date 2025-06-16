using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Effects.Values;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class CloseCombatAbilityController : IAbilityController, IAbilityValueContainer {
	[DataReadProxy(Name = "Punch")]
	[DataWriteProxy(Name = "Punch")]
	[CopyableProxy]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ShotType punchType;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ShotSubtypeEnum punchSubtype;

	[DataReadProxy(Name = "weapon")]
	[DataWriteProxy(Name = "weapon")]
	[CopyableProxy()]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected WeaponEnum weaponKind;

	[Inspected] private StorableComponent storable;
	private AbilityItem abilityItem;
	private EnemyBase attacker;
	private IEntity itemOwner;
	private Dictionary<AbilityValueNameEnum, IAbilityValue> values = new();

	public ReactionType ReactionType { get; set; }

	public WeaponEnum WeaponKind => weaponKind;

	public void Initialise(AbilityItem abilityItem) {
		this.abilityItem = abilityItem;
		Subcribe();
	}

	private void Subcribe() {
		Unsubscribe();
		storable = abilityItem.Ability.Owner.GetComponent<StorableComponent>();
		if (storable == null)
			return;
		storable.ChangeStorageEvent += ChangeStorageEvent;
		if (storable.Storage == null)
			return;
		itemOwner = storable.Storage.Owner;
		if (itemOwner == null)
			return;
		((IEntityView)itemOwner).OnGameObjectChangedEvent += OnGameObjectChanged;
		var gameObject = ((IEntityView)itemOwner).GameObject;
		if (gameObject == null)
			return;
		var abilityCache = abilityItem.Ability.AbilityCache;
		if (abilityCache.EnemyBase != null && abilityCache.EnemyBase.gameObject == gameObject)
			attacker = abilityCache.EnemyBase;
		else
			abilityCache.EnemyBase = attacker = gameObject.GetComponentNonAlloc<EnemyBase>();
		if (attacker == null)
			return;
		attacker.PunchEvent += OnPunchEvent;
	}

	private void Unsubscribe() {
		if (attacker != null) {
			attacker.PunchEvent -= OnPunchEvent;
			attacker = null;
		}

		if (itemOwner != null) {
			((IEntityView)itemOwner).OnGameObjectChangedEvent -= OnGameObjectChanged;
			itemOwner = null;
		}

		if (storable == null)
			return;
		storable.ChangeStorageEvent -= ChangeStorageEvent;
		storable = null;
	}

	public void Shutdown() {
		if (storable != null)
			abilityItem.Active = false;
		Unsubscribe();
	}

	private void ChangeStorageEvent(IStorableComponent sender) {
		Subcribe();
	}

	private void OnGameObjectChanged() {
		Subcribe();
	}

	private void OnPunchEvent(
		IEntity weaponEntity,
		ShotType punch,
		ReactionType reactionType,
		WeaponEnum weapon,
		ShotSubtypeEnum subtype = ShotSubtypeEnum.None) {
		if (weapon != weaponKind || (weaponEntity != null && weaponEntity != storable.Owner) ||
		    (punch != punchType && punchType != ShotType.None) ||
		    (subtype != punchSubtype && punchSubtype != ShotSubtypeEnum.None))
			return;
		ReactionType = reactionType;
		PrepareData();
		abilityItem.Active = true;
		abilityItem.Active = false;
	}

	private void PrepareData() {
		values.Clear();
		var component = ((IEntityView)itemOwner).GameObject.GetComponent<NPCWeaponService>();
		if (component == null)
			return;
		var knifeSpeed = component.KnifeSpeed;
		var knifePosition = component.KnifePosition;
		var camera = GameCamera.Instance.Camera;
		var vector3 = GameCamera.Instance.CameraTransform.InverseTransformVector(knifeSpeed);
		var magnitude = vector3.magnitude;
		var num1 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
		var viewportPoint = camera.WorldToViewportPoint(knifePosition);
		if (viewportPoint.z < 0.0)
			viewportPoint *= -1f;
		var num2 = Mathf.Clamp01(viewportPoint.x) - 0.5f;
		var num3 = Mathf.Clamp01(viewportPoint.y) - 0.5f;
		var max = 0.4f;
		var vector2 = new Vector2(Mathf.Clamp(num2, -max, max), Mathf.Clamp(num3, -max, max));
		values.Add(AbilityValueNameEnum.ScarPosition, new AbilityValue<Vector2> {
			Value = vector2
		});
		values.Add(AbilityValueNameEnum.ScarAngle, new AbilityValue<float> {
			Value = num1
		});
		values.Add(AbilityValueNameEnum.ScarScale, new AbilityValue<float> {
			Value = 0.5f
		});
	}

	public IAbilityValue<T> GetAbilityValue<T>(AbilityValueNameEnum parameter) where T : struct {
		IAbilityValue abilityValue;
		values.TryGetValue(parameter, out abilityValue);
		return abilityValue as IAbilityValue<T>;
	}
}