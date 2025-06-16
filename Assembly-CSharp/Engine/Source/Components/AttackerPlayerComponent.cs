using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Services;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Attacker;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Impl.UI.Menu;
using Engine.Impl.UI.Menu.Protagonist.Inventory;
using Engine.Source.Commons;
using Engine.Source.Inventory;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Source.Components;

[Factory(typeof(IAttackerPlayerComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class AttackerPlayerComponent :
	EngineComponent,
	IAttackerPlayerComponent,
	IComponent,
	IUpdatable,
	IPlayerActivated {
	private WeaponKind[] avaliableWeapons = new WeaponKind[4];
	private IEntity[] avaliableWeaponItems = new IEntity[4];
	private PlayerWeaponServiceNew playerWeaponService;
	private bool wasUnholsteredWhenVisir;
	private int lastAlphaNumericalWeapon = -1;

	public int currentWeapon { get; private set; }

	public event Action<WeaponKind> WeaponHolsterStartEvent;

	public event Action<WeaponKind> WeaponUnholsterEndEvent;

	public event Action<WeaponKind, IEntity, ShotType, ShotSubtypeEnum> WeaponShootStartEvent;

	public event Action<WeaponKind, IEntity, ShotType, ShotSubtypeEnum> WeaponShootEndEvent;

	public event Action AvailableWeaponItemsChangeEvent;

	public event Action CurrentWeaponUnholstered;

	public IEntity CurrentWeaponItem => avaliableWeaponItems[currentWeapon];

	public IEnumerable<IEntity> AvailableWeaponItems => avaliableWeaponItems;

	public bool IsUnholstered { get; private set; }

	public void ResetWeapon() {
		playerWeaponService.ResetWeapon();
	}

	private bool WeaponListener(GameActionType type, bool down) {
		if (!down || !PlayerUtility.IsPlayerCanControlling)
			return false;
		switch (type) {
			case GameActionType.Weapon1:
				SetWeaponIndex(0);
				break;
			case GameActionType.Weapon2:
				SetWeaponIndex(1);
				break;
			case GameActionType.Weapon3:
				SetWeaponIndex(2);
				break;
			case GameActionType.Weapon4:
				SetWeaponIndex(3);
				break;
		}

		return true;
	}

	private void SetWeaponIndex(int index) {
		if (index == 0 && avaliableWeapons[index] == WeaponKind.Unknown)
			return;
		if (avaliableWeapons[index] == WeaponKind.Unknown) {
			currentWeapon = 0;
			if (lastAlphaNumericalWeapon == index)
				ToggleCurrentWeapon();
			else
				HandsUnholster();
		} else if (currentWeapon == index)
			ToggleCurrentWeapon();
		else {
			currentWeapon = index;
			HandsUnholster();
		}

		lastAlphaNumericalWeapon = index;
	}

	public void ToggleCurrentWeapon() {
		IsUnholstered = playerWeaponService.IsWeaponOn();
		if (IsUnholstered)
			HandsHolster();
		else
			HandsUnholster();
	}

	private void OnInventoryChanged(IStorableComponent storable, IInventoryComponent container) {
		var active = ServiceLocator.GetService<UIService>().Active;
		if (active != null && active is IBaseInventoryWindow)
			return;
		ApplyInventoryChange();
	}

	public void ApplyInventoryChange() {
		var flag1 = avaliableWeapons[1] != 0;
		var flag2 = avaliableWeapons[2] != 0;
		GetAvailiableWeapons();
		var flag3 = avaliableWeapons[1] != 0;
		var flag4 = avaliableWeapons[2] != 0;
		if (!flag1 & flag3)
			currentWeapon = 1;
		else if (flag1 && !flag3)
			currentWeapon = 0;
		else if (((flag1 || flag3 ? 0 : !flag2 ? 1 : 0) & (flag4 ? 1 : 0)) != 0)
			currentWeapon = 2;
		else if (((flag1 ? 0 : !flag3 ? 1 : 0) & (flag2 ? 1 : 0)) != 0 && !flag4)
			currentWeapon = 0;
		else if (avaliableWeapons[currentWeapon] == WeaponKind.Unknown)
			currentWeapon = avaliableWeapons[1] == 0
				? avaliableWeapons[2] == 0 ? avaliableWeapons[3] == 0 ? 0 : 3 : 2
				: 1;
		if (!(playerWeaponService != null) || !IsUnholstered || !IsUnholstered)
			return;
		playerWeaponService.SetWeapon(avaliableWeapons[currentWeapon], avaliableWeaponItems[currentWeapon]);
		if (avaliableWeapons[currentWeapon] == WeaponKind.Unknown)
			HandsHolster();
	}

	private void GetAvailiableWeapons() {
		var component = Owner.GetComponent<IStorageComponent>();
		avaliableWeapons[0] = avaliableWeapons[1] = avaliableWeapons[2] = avaliableWeapons[3] = WeaponKind.Unknown;
		avaliableWeaponItems[0] = avaliableWeaponItems[1] = avaliableWeaponItems[2] = avaliableWeaponItems[3] = null;
		foreach (var storableComponent in component.Items)
			if (storableComponent != null && storableComponent.Container != null) {
				var limitations = storableComponent.Container.GetLimitations();
				if (limitations.Contains(StorableGroup.Weapons_Hands)) {
					avaliableWeapons[0] = GetWeaponFromItem(storableComponent.Owner);
					avaliableWeaponItems[0] = storableComponent.Owner;
				}

				if (limitations.Contains(StorableGroup.Weapons_Primary)) {
					avaliableWeapons[1] = GetWeaponFromItem(storableComponent.Owner);
					avaliableWeaponItems[1] = storableComponent.Owner;
				}

				if (limitations.Contains(StorableGroup.Weapons_Secondary)) {
					avaliableWeapons[2] = GetWeaponFromItem(storableComponent.Owner);
					avaliableWeaponItems[2] = storableComponent.Owner;
				}

				if (limitations.Contains(StorableGroup.Weapons_Lamp)) {
					avaliableWeapons[3] = GetWeaponFromItem(storableComponent.Owner);
					avaliableWeaponItems[3] = storableComponent.Owner;
				}
			}

		var itemsChangeEvent = AvailableWeaponItemsChangeEvent;
		if (itemsChangeEvent == null)
			return;
		itemsChangeEvent();
	}

	private WeaponKind GetWeaponFromItem(IEntity item) {
		var component = item.GetComponent<ParametersComponent>();
		if (component != null) {
			var byName = component.GetByName<WeaponKind>(ParameterNameEnum.WeaponKind);
			if (byName != null)
				return byName.Value;
		}

		return WeaponKind.Unknown;
	}

	public void SetWeaponByIndex(Slots slot) {
		SetWeaponIndex((int)slot);
	}

	public void NextWeapon() {
		for (var index = 0; index < avaliableWeapons.Length; ++index) {
			currentWeapon = (currentWeapon + 1) % 4;
			if (avaliableWeapons[currentWeapon] != 0) {
				playerWeaponService.SetWeapon(avaliableWeapons[currentWeapon], avaliableWeaponItems[currentWeapon]);
				break;
			}
		}
	}

	public void PrevWeapon() {
		for (var index = 0; index < avaliableWeapons.Length; ++index) {
			currentWeapon = (currentWeapon - 1 + 4) % 4;
			if (avaliableWeapons[currentWeapon] != 0) {
				playerWeaponService.SetWeapon(avaliableWeapons[currentWeapon], avaliableWeaponItems[currentWeapon]);
				break;
			}
		}
	}

	public WeaponKind CurrentWeapon => playerWeaponService != null ? playerWeaponService.KindBase : WeaponKind.Unknown;

	public void SetWeapon(WeaponKind weaponKind) {
		if (weaponKind == WeaponKind.Unknown) {
			IsUnholstered = false;
			playerWeaponService.SetWeapon(weaponKind, null);
		} else {
			for (var index = 0; index < 4; ++index)
				if (weaponKind == avaliableWeapons[index]) {
					IsUnholstered = true;
					playerWeaponService.SetWeapon(avaliableWeapons[index], avaliableWeaponItems[index]);
					return;
				}

			playerWeaponService.SetWeapon(weaponKind, null);
		}
	}

	public void HandsUnholster() {
		IsUnholstered = true;
		playerWeaponService.SetWeapon(avaliableWeapons[currentWeapon], avaliableWeaponItems[currentWeapon]);
		var weaponUnholstered = CurrentWeaponUnholstered;
		if (weaponUnholstered == null)
			return;
		weaponUnholstered();
	}

	public void HandsHolster() {
		IsUnholstered = false;
		playerWeaponService.SetWeapon(WeaponKind.Unknown, null);
	}

	public void WeaponHandsUnholster() {
		if (avaliableWeapons[0] != 0) {
			IsUnholstered = true;
			playerWeaponService.SetWeapon(avaliableWeapons[0], avaliableWeaponItems[0]);
		} else
			Debug.LogError(typeof(AttackerPlayerComponent).Name + " has no weapon in slot Weapon1");
	}

	public void WeaponFirearmUnholster() {
		if (avaliableWeapons[1] != 0) {
			IsUnholstered = true;
			playerWeaponService.SetWeapon(avaliableWeapons[1], avaliableWeaponItems[1]);
		} else
			Debug.LogError(typeof(AttackerPlayerComponent).Name + " has no weapon in slot Weapon2");
	}

	public void WeaponMeleeUnholster() {
		if (avaliableWeapons[2] != 0) {
			IsUnholstered = true;
			playerWeaponService.SetWeapon(avaliableWeapons[2], avaliableWeaponItems[2]);
		} else
			Debug.LogError(typeof(AttackerPlayerComponent).Name + " has no weapon in slot Weapon3");
	}

	public void WeaponLampUnholster() {
		if (avaliableWeapons[3] != 0) {
			IsUnholstered = true;
			playerWeaponService.SetWeapon(avaliableWeapons[3], avaliableWeaponItems[3]);
		} else
			Debug.LogError(typeof(AttackerPlayerComponent).Name + " has no weapon in slot Weapon4");
	}

	public bool Reaction(IEntity target, NPCAttackKind attackKind, int randomSubkind) {
		Clear();
		return true;
	}

	private void OnGameObjectChangedEvent() {
		var owner = (IEntityView)Owner;
		if (owner.GameObject == null || playerWeaponService != null)
			return;
		playerWeaponService = owner.GameObject.GetComponent<PlayerWeaponServiceNew>();
		if (playerWeaponService == null)
			return;
		playerWeaponService.SetWeapon(WeaponKind.Unknown, null);
		playerWeaponService.WeaponHolsterStartEvent += weapon => {
			var holsterStartEvent = WeaponHolsterStartEvent;
			if (holsterStartEvent == null)
				return;
			holsterStartEvent(weapon);
		};
		playerWeaponService.WeaponUnholsterEndEvent += weapon => {
			var unholsterEndEvent = WeaponUnholsterEndEvent;
			if (unholsterEndEvent == null)
				return;
			unholsterEndEvent(weapon);
		};
		playerWeaponService.WeaponShootEvent += (weapon, weaponEntity, shotType, reactionType, shotSubtype) => {
			var weaponShootStartEvent = WeaponShootStartEvent;
			if (weaponShootStartEvent == null)
				return;
			weaponShootStartEvent(weapon, weaponEntity, shotType, shotSubtype);
		};
	}

	public void Clear() {
		if (!(((IEntityView)Owner).GameObject == null))
			;
	}

	public void ComputeUpdate() {
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
			return;
		var owner = (IEntityView)Owner;
		if (owner.GameObject == null || !(owner.GameObject.GetComponent<Animator>() == null))
			;
	}

	public void LateUpdate() {
		var owner = (IEntityView)Owner;
		if (owner.GameObject == null || owner.GameObject.GetComponent<Animator>() == null)
			return;
		var component = owner.GameObject.GetComponent<PivotPlayer>();
		if (!(component != null))
			return;
		component.ApplyWeaponTransform(playerWeaponService.KindBase);
	}

	public PlayerWeaponServiceNew GetPlayerWeaponService() {
		return playerWeaponService;
	}

	public void PlayerActivated() {
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
		((IEntityView)Owner).OnGameObjectChangedEvent += OnGameObjectChangedEvent;
		OnGameObjectChangedEvent();
		Owner.GetComponent<StorageComponent>().OnAddItemEvent += OnInventoryChanged;
		Owner.GetComponent<StorageComponent>().OnRemoveItemEvent += OnInventoryChanged;
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon1, WeaponListener);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon2, WeaponListener);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon3, WeaponListener);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon4, WeaponListener);
		GetAvailiableWeapons();
	}

	public void PlayerDeactivated() {
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
		((IEntityView)Owner).OnGameObjectChangedEvent -= OnGameObjectChangedEvent;
		Owner.GetComponent<StorageComponent>().OnAddItemEvent -= OnInventoryChanged;
		Owner.GetComponent<StorageComponent>().OnRemoveItemEvent -= OnInventoryChanged;
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon1, WeaponListener);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon2, WeaponListener);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon3, WeaponListener);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon4, WeaponListener);
	}

	public enum Slots {
		WeaponNone = -1,
		WeaponHands = 0,
		WeaponPrimary = 1,
		WeaponSecondary = 2,
		WeaponLamp = 3
	}
}