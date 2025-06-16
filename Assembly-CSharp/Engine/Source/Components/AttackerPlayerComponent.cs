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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Engine.Source.Components
{
  [Factory(typeof (IAttackerPlayerComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AttackerPlayerComponent : 
    EngineComponent,
    IAttackerPlayerComponent,
    IComponent,
    IUpdatable,
    IPlayerActivated
  {
    private WeaponKind[] avaliableWeapons = new WeaponKind[4];
    private IEntity[] avaliableWeaponItems = new IEntity[4];
    private PlayerWeaponServiceNew playerWeaponService;
    private bool wasUnholsteredWhenVisir;
    private int lastAlphaNumericalWeapon = -1;

    public int currentWeapon { get; private set; } = 0;

    public event Action<WeaponKind> WeaponHolsterStartEvent;

    public event Action<WeaponKind> WeaponUnholsterEndEvent;

    public event Action<WeaponKind, IEntity, ShotType, ShotSubtypeEnum> WeaponShootStartEvent;

    public event Action<WeaponKind, IEntity, ShotType, ShotSubtypeEnum> WeaponShootEndEvent;

    public event Action AvailableWeaponItemsChangeEvent;

    public event Action CurrentWeaponUnholstered;

    public IEntity CurrentWeaponItem => this.avaliableWeaponItems[this.currentWeapon];

    public IEnumerable<IEntity> AvailableWeaponItems
    {
      get => (IEnumerable<IEntity>) this.avaliableWeaponItems;
    }

    public bool IsUnholstered { get; private set; }

    public void ResetWeapon() => this.playerWeaponService.ResetWeapon();

    private bool WeaponListener(GameActionType type, bool down)
    {
      if (!down || !PlayerUtility.IsPlayerCanControlling)
        return false;
      switch (type)
      {
        case GameActionType.Weapon1:
          this.SetWeaponIndex(0);
          break;
        case GameActionType.Weapon2:
          this.SetWeaponIndex(1);
          break;
        case GameActionType.Weapon3:
          this.SetWeaponIndex(2);
          break;
        case GameActionType.Weapon4:
          this.SetWeaponIndex(3);
          break;
      }
      return true;
    }

    private void SetWeaponIndex(int index)
    {
      if (index == 0 && this.avaliableWeapons[index] == WeaponKind.Unknown)
        return;
      if (this.avaliableWeapons[index] == WeaponKind.Unknown)
      {
        this.currentWeapon = 0;
        if (this.lastAlphaNumericalWeapon == index)
          this.ToggleCurrentWeapon();
        else
          this.HandsUnholster();
      }
      else if (this.currentWeapon == index)
      {
        this.ToggleCurrentWeapon();
      }
      else
      {
        this.currentWeapon = index;
        this.HandsUnholster();
      }
      this.lastAlphaNumericalWeapon = index;
    }

    public void ToggleCurrentWeapon()
    {
      this.IsUnholstered = this.playerWeaponService.IsWeaponOn();
      if (this.IsUnholstered)
        this.HandsHolster();
      else
        this.HandsUnholster();
    }

    private void OnInventoryChanged(IStorableComponent storable, IInventoryComponent container)
    {
      UIWindow active = ServiceLocator.GetService<UIService>().Active;
      if ((UnityEngine.Object) active != (UnityEngine.Object) null && active is IBaseInventoryWindow)
        return;
      this.ApplyInventoryChange();
    }

    public void ApplyInventoryChange()
    {
      bool flag1 = this.avaliableWeapons[1] != 0;
      bool flag2 = this.avaliableWeapons[2] != 0;
      this.GetAvailiableWeapons();
      bool flag3 = this.avaliableWeapons[1] != 0;
      bool flag4 = this.avaliableWeapons[2] != 0;
      if (!flag1 & flag3)
        this.currentWeapon = 1;
      else if (flag1 && !flag3)
        this.currentWeapon = 0;
      else if (((flag1 || flag3 ? 0 : (!flag2 ? 1 : 0)) & (flag4 ? 1 : 0)) != 0)
        this.currentWeapon = 2;
      else if (((flag1 ? 0 : (!flag3 ? 1 : 0)) & (flag2 ? 1 : 0)) != 0 && !flag4)
        this.currentWeapon = 0;
      else if (this.avaliableWeapons[this.currentWeapon] == WeaponKind.Unknown)
        this.currentWeapon = this.avaliableWeapons[1] == 0 ? (this.avaliableWeapons[2] == 0 ? (this.avaliableWeapons[3] == 0 ? 0 : 3) : 2) : 1;
      if (!((UnityEngine.Object) this.playerWeaponService != (UnityEngine.Object) null) || !this.IsUnholstered || !this.IsUnholstered)
        return;
      this.playerWeaponService.SetWeapon(this.avaliableWeapons[this.currentWeapon], this.avaliableWeaponItems[this.currentWeapon]);
      if (this.avaliableWeapons[this.currentWeapon] == WeaponKind.Unknown)
        this.HandsHolster();
    }

    private void GetAvailiableWeapons()
    {
      IStorageComponent component = this.Owner.GetComponent<IStorageComponent>();
      this.avaliableWeapons[0] = this.avaliableWeapons[1] = this.avaliableWeapons[2] = this.avaliableWeapons[3] = WeaponKind.Unknown;
      this.avaliableWeaponItems[0] = this.avaliableWeaponItems[1] = this.avaliableWeaponItems[2] = this.avaliableWeaponItems[3] = (IEntity) null;
      foreach (IStorableComponent storableComponent in component.Items)
      {
        if (storableComponent != null && storableComponent.Container != null)
        {
          IEnumerable<StorableGroup> limitations = storableComponent.Container.GetLimitations();
          if (limitations.Contains<StorableGroup>(StorableGroup.Weapons_Hands))
          {
            this.avaliableWeapons[0] = this.GetWeaponFromItem(storableComponent.Owner);
            this.avaliableWeaponItems[0] = storableComponent.Owner;
          }
          if (limitations.Contains<StorableGroup>(StorableGroup.Weapons_Primary))
          {
            this.avaliableWeapons[1] = this.GetWeaponFromItem(storableComponent.Owner);
            this.avaliableWeaponItems[1] = storableComponent.Owner;
          }
          if (limitations.Contains<StorableGroup>(StorableGroup.Weapons_Secondary))
          {
            this.avaliableWeapons[2] = this.GetWeaponFromItem(storableComponent.Owner);
            this.avaliableWeaponItems[2] = storableComponent.Owner;
          }
          if (limitations.Contains<StorableGroup>(StorableGroup.Weapons_Lamp))
          {
            this.avaliableWeapons[3] = this.GetWeaponFromItem(storableComponent.Owner);
            this.avaliableWeaponItems[3] = storableComponent.Owner;
          }
        }
      }
      Action itemsChangeEvent = this.AvailableWeaponItemsChangeEvent;
      if (itemsChangeEvent == null)
        return;
      itemsChangeEvent();
    }

    private WeaponKind GetWeaponFromItem(IEntity item)
    {
      ParametersComponent component = item.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<WeaponKind> byName = component.GetByName<WeaponKind>(ParameterNameEnum.WeaponKind);
        if (byName != null)
          return byName.Value;
      }
      return WeaponKind.Unknown;
    }

    public void SetWeaponByIndex(AttackerPlayerComponent.Slots slot)
    {
      this.SetWeaponIndex((int) slot);
    }

    public void NextWeapon()
    {
      for (int index = 0; index < this.avaliableWeapons.Length; ++index)
      {
        this.currentWeapon = (this.currentWeapon + 1) % 4;
        if (this.avaliableWeapons[this.currentWeapon] != 0)
        {
          this.playerWeaponService.SetWeapon(this.avaliableWeapons[this.currentWeapon], this.avaliableWeaponItems[this.currentWeapon]);
          break;
        }
      }
    }

    public void PrevWeapon()
    {
      for (int index = 0; index < this.avaliableWeapons.Length; ++index)
      {
        this.currentWeapon = (this.currentWeapon - 1 + 4) % 4;
        if (this.avaliableWeapons[this.currentWeapon] != 0)
        {
          this.playerWeaponService.SetWeapon(this.avaliableWeapons[this.currentWeapon], this.avaliableWeaponItems[this.currentWeapon]);
          break;
        }
      }
    }

    public WeaponKind CurrentWeapon
    {
      get
      {
        return (UnityEngine.Object) this.playerWeaponService != (UnityEngine.Object) null ? this.playerWeaponService.KindBase : WeaponKind.Unknown;
      }
    }

    public void SetWeapon(WeaponKind weaponKind)
    {
      if (weaponKind == WeaponKind.Unknown)
      {
        this.IsUnholstered = false;
        this.playerWeaponService.SetWeapon(weaponKind, (IEntity) null);
      }
      else
      {
        for (int index = 0; index < 4; ++index)
        {
          if (weaponKind == this.avaliableWeapons[index])
          {
            this.IsUnholstered = true;
            this.playerWeaponService.SetWeapon(this.avaliableWeapons[index], this.avaliableWeaponItems[index]);
            return;
          }
        }
        this.playerWeaponService.SetWeapon(weaponKind, (IEntity) null);
      }
    }

    public void HandsUnholster()
    {
      this.IsUnholstered = true;
      this.playerWeaponService.SetWeapon(this.avaliableWeapons[this.currentWeapon], this.avaliableWeaponItems[this.currentWeapon]);
      Action weaponUnholstered = this.CurrentWeaponUnholstered;
      if (weaponUnholstered == null)
        return;
      weaponUnholstered();
    }

    public void HandsHolster()
    {
      this.IsUnholstered = false;
      this.playerWeaponService.SetWeapon(WeaponKind.Unknown, (IEntity) null);
    }

    public void WeaponHandsUnholster()
    {
      if (this.avaliableWeapons[0] != 0)
      {
        this.IsUnholstered = true;
        this.playerWeaponService.SetWeapon(this.avaliableWeapons[0], this.avaliableWeaponItems[0]);
      }
      else
        Debug.LogError((object) (typeof (AttackerPlayerComponent).Name + " has no weapon in slot Weapon1"));
    }

    public void WeaponFirearmUnholster()
    {
      if (this.avaliableWeapons[1] != 0)
      {
        this.IsUnholstered = true;
        this.playerWeaponService.SetWeapon(this.avaliableWeapons[1], this.avaliableWeaponItems[1]);
      }
      else
        Debug.LogError((object) (typeof (AttackerPlayerComponent).Name + " has no weapon in slot Weapon2"));
    }

    public void WeaponMeleeUnholster()
    {
      if (this.avaliableWeapons[2] != 0)
      {
        this.IsUnholstered = true;
        this.playerWeaponService.SetWeapon(this.avaliableWeapons[2], this.avaliableWeaponItems[2]);
      }
      else
        Debug.LogError((object) (typeof (AttackerPlayerComponent).Name + " has no weapon in slot Weapon3"));
    }

    public void WeaponLampUnholster()
    {
      if (this.avaliableWeapons[3] != 0)
      {
        this.IsUnholstered = true;
        this.playerWeaponService.SetWeapon(this.avaliableWeapons[3], this.avaliableWeaponItems[3]);
      }
      else
        Debug.LogError((object) (typeof (AttackerPlayerComponent).Name + " has no weapon in slot Weapon4"));
    }

    public bool Reaction(IEntity target, NPCAttackKind attackKind, int randomSubkind)
    {
      this.Clear();
      return true;
    }

    private void OnGameObjectChangedEvent()
    {
      IEntityView owner = (IEntityView) this.Owner;
      if ((UnityEngine.Object) owner.GameObject == (UnityEngine.Object) null || (UnityEngine.Object) this.playerWeaponService != (UnityEngine.Object) null)
        return;
      this.playerWeaponService = owner.GameObject.GetComponent<PlayerWeaponServiceNew>();
      if ((UnityEngine.Object) this.playerWeaponService == (UnityEngine.Object) null)
        return;
      this.playerWeaponService.SetWeapon(WeaponKind.Unknown, (IEntity) null);
      this.playerWeaponService.WeaponHolsterStartEvent += (Action<WeaponKind>) (weapon =>
      {
        Action<WeaponKind> holsterStartEvent = this.WeaponHolsterStartEvent;
        if (holsterStartEvent == null)
          return;
        holsterStartEvent(weapon);
      });
      this.playerWeaponService.WeaponUnholsterEndEvent += (Action<WeaponKind>) (weapon =>
      {
        Action<WeaponKind> unholsterEndEvent = this.WeaponUnholsterEndEvent;
        if (unholsterEndEvent == null)
          return;
        unholsterEndEvent(weapon);
      });
      this.playerWeaponService.WeaponShootEvent += (Action<WeaponKind, IEntity, ShotType, ReactionType, ShotSubtypeEnum>) ((weapon, weaponEntity, shotType, reactionType, shotSubtype) =>
      {
        Action<WeaponKind, IEntity, ShotType, ShotSubtypeEnum> weaponShootStartEvent = this.WeaponShootStartEvent;
        if (weaponShootStartEvent == null)
          return;
        weaponShootStartEvent(weapon, weaponEntity, shotType, shotSubtype);
      });
    }

    public void Clear()
    {
      if (!((UnityEngine.Object) ((IEntityView) this.Owner).GameObject == (UnityEngine.Object) null))
        ;
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        return;
      IEntityView owner = (IEntityView) this.Owner;
      if ((UnityEngine.Object) owner.GameObject == (UnityEngine.Object) null || !((UnityEngine.Object) owner.GameObject.GetComponent<Animator>() == (UnityEngine.Object) null))
        ;
    }

    public void LateUpdate()
    {
      IEntityView owner = (IEntityView) this.Owner;
      if ((UnityEngine.Object) owner.GameObject == (UnityEngine.Object) null || (UnityEngine.Object) owner.GameObject.GetComponent<Animator>() == (UnityEngine.Object) null)
        return;
      PivotPlayer component = owner.GameObject.GetComponent<PivotPlayer>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      component.ApplyWeaponTransform(this.playerWeaponService.KindBase);
    }

    public PlayerWeaponServiceNew GetPlayerWeaponService() => this.playerWeaponService;

    public void PlayerActivated()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
      ((IEntityView) this.Owner).OnGameObjectChangedEvent += new Action(this.OnGameObjectChangedEvent);
      this.OnGameObjectChangedEvent();
      this.Owner.GetComponent<StorageComponent>().OnAddItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.OnInventoryChanged);
      this.Owner.GetComponent<StorageComponent>().OnRemoveItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.OnInventoryChanged);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon1, new GameActionHandle(this.WeaponListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon2, new GameActionHandle(this.WeaponListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon3, new GameActionHandle(this.WeaponListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon4, new GameActionHandle(this.WeaponListener));
      this.GetAvailiableWeapons();
    }

    public void PlayerDeactivated()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      ((IEntityView) this.Owner).OnGameObjectChangedEvent -= new Action(this.OnGameObjectChangedEvent);
      this.Owner.GetComponent<StorageComponent>().OnAddItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.OnInventoryChanged);
      this.Owner.GetComponent<StorageComponent>().OnRemoveItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.OnInventoryChanged);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon1, new GameActionHandle(this.WeaponListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon2, new GameActionHandle(this.WeaponListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon3, new GameActionHandle(this.WeaponListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon4, new GameActionHandle(this.WeaponListener));
    }

    public enum Slots
    {
      WeaponNone = -1, // 0xFFFFFFFF
      WeaponHands = 0,
      WeaponPrimary = 1,
      WeaponSecondary = 2,
      WeaponLamp = 3,
    }
  }
}
