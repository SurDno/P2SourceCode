using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Container;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Windows;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using Engine.Source.Inventory;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;
using Pingle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class LootWindow : 
    BaseInventoryWindow<LootWindow>,
    ILootWindow,
    IWindow,
    IChangeParameterListener
  {
    [SerializeField]
    private IEntitySerializable deadNpcEntityContainer;
    [SerializeField]
    private ContainerResizableWindow lootContainerWindow;
    [SerializeField]
    private IEntitySerializable lockpickTemplate;
    [SerializeField]
    private LockpickTooltip lockpickTooltip;
    [SerializeField]
    private GameObject openContainerTooltip;
    [SerializeField]
    private GameObject unlockContainerTooltip;
    private List<IStorableComponent> targetItems;
    private List<ContainerResizableWindow> inventories;
    private InventoryContainerUI _selectedContainer;
    private int currentStorableCountInLoot = 0;
    private LootWindow.Modes _currentMode = LootWindow.Modes.None;
    private StorableUI lastLootStor = (StorableUI) null;
    private StorableUI lastInventoryStor = (StorableUI) null;
    private int selectedInventoryIndex = -1;
    private bool MoveStorableBetweenInventories = false;
    private bool isLootActive = true;

    public IStorageComponent Target { get; set; }

    protected override InventoryContainerUI selectedContainer
    {
      get => this._selectedContainer;
      set
      {
        if ((UnityEngine.Object) this._selectedContainer == (UnityEngine.Object) value)
          return;
        this.HideClosedContainerInfo();
        if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
        {
          this.selectedStorable = (StorableUI) null;
          this.HideInfoWindow();
        }
        if ((UnityEngine.Object) this._selectedContainer != (UnityEngine.Object) null)
          this._selectedContainer.Button.GamepadEndHold();
        this._selectedContainer = value;
        if ((UnityEngine.Object) value != (UnityEngine.Object) null)
        {
          this.ShowClosedContainerInfo(this._selectedContainer.InventoryContainer);
        }
        else
        {
          this.unlockContainerTooltip.SetActive(false);
          this.openContainerTooltip.SetActive(false);
        }
        this.SetSelectedContainer();
      }
    }

    private void FindTargetItems()
    {
      this.targetItems = new List<IStorableComponent>();
      this.targetItems.AddRange(this.Target.Items);
    }

    private void ResizeTargetContainerView()
    {
      List<InventoryContainerUI> containers = new List<InventoryContainerUI>();
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in this.containers)
      {
        if (container.Value == this.Target)
          containers.Add(container.Key);
      }
      this.ResizeContainersWindow(this.lootContainerWindow, containers);
    }

    private void CheckLivingNpc(bool subscribe = true)
    {
      if (this.Target?.Owner?.GetComponent<NpcControllerComponent>() == null)
        return;
      ParametersComponent component = this.Target.Owner.GetComponent<ParametersComponent>();
      IParameter<bool> byName1 = component?.GetByName<bool>(ParameterNameEnum.Dead);
      if (byName1 == null || !byName1.Value)
      {
        IParameter<bool> byName2 = component?.GetByName<bool>(ParameterNameEnum.Surrender);
        if (byName2 != null)
        {
          byName2.RemoveListener((IChangeParameterListener) this);
          if (subscribe)
            byName2.AddListener((IChangeParameterListener) this);
        }
      }
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionLeft, new GameActionHandle(this.OnChangeInventory));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionRight, new GameActionHandle(this.OnChangeInventory));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      this.inventories = new List<ContainerResizableWindow>((IEnumerable<ContainerResizableWindow>) this.GetComponentsInChildren<ContainerResizableWindow>());
      this.actors.Clear();
      this.actors.Add(this.Actor);
      this.actors.Add(this.Target);
      this.Build2();
      this.HideLockpickInfo();
      this.ResizeTargetContainerView();
      this.CheckLivingNpc();
      this.FindTargetItems();
    }

    protected override IEnumerator AfterEnabled()
    {
      yield return (object) base.AfterEnabled();
      if (InputService.Instance.JoystickUsed)
        this.OnNavigate(BaseInventoryWindow<LootWindow>.Navigation.CellLeft);
    }

    protected override void OnDisable()
    {
      this.CheckLivingNpc(false);
      this.CheckEmpty();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Loot, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionLeft, new GameActionHandle(this.OnChangeInventory));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionRight, new GameActionHandle(this.OnChangeInventory));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.MainControl));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, new GameActionHandle(((BaseInventoryWindow<LootWindow>) this).DragListener));
      this.CurrentMode = LootWindow.Modes.None;
      Color color = this.inventories[1].GetComponentInChildren<Image>().color with
      {
        a = 1f
      };
      this.inventories[1].GetComponentInChildren<Image>().color = color;
      this.selectedInventoryIndex = -1;
      base.OnDisable();
    }

    protected override void Subscribe()
    {
      base.Subscribe();
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.BumperSelectionLeft, new GameActionHandle(this.OnChangeInventory));
      service.AddListener(GameActionType.BumperSelectionRight, new GameActionHandle(this.OnChangeInventory));
      service.AddListener(GameActionType.Submit, new GameActionHandle(this.MainControl));
      service.RemoveListener(GameActionType.Submit, new GameActionHandle(((BaseInventoryWindow<LootWindow>) this).ContextListener));
      service.AddListener(GameActionType.Context, new GameActionHandle(((BaseInventoryWindow<LootWindow>) this).DragListener));
    }

    protected override void Unsubscribe()
    {
      base.Unsubscribe();
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.BumperSelectionLeft, new GameActionHandle(this.OnChangeInventory));
      service.RemoveListener(GameActionType.BumperSelectionRight, new GameActionHandle(this.OnChangeInventory));
      service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.MainControl));
      service.RemoveListener(GameActionType.Context, new GameActionHandle(((BaseInventoryWindow<LootWindow>) this).DragListener));
    }

    private LootWindow.Modes CurrentMode
    {
      get => this._currentMode;
      set
      {
        if (this._currentMode == value)
          return;
        this.HideContextMenu();
        this.HideInfoWindow();
        switch (value)
        {
          case LootWindow.Modes.Inventory:
            this.unlockContainerTooltip.SetActive(false);
            this.openContainerTooltip.SetActive(false);
            if (this.inventories[0].GetComponentsInChildren<StorableUI>().Length == 0)
              return;
            if (!this.isLootActive)
            {
              Color color = this.inventories[1].GetComponentInChildren<Image>().color with
              {
                a = 1f
              };
              this.inventories[1].GetComponentInChildren<Image>().color = color;
              this.isLootActive = true;
            }
            if (InputService.Instance.JoystickUsed)
            {
              this.inventories[0].SetActive(true);
              this.inventories[1].SetActive(false);
            }
            this.currentInventory = this.inventories[0];
            this.selectedInventoryIndex = 0;
            if (this._currentMode == LootWindow.Modes.Loot)
            {
              ServiceLocator.GetService<GameActionService>();
              if ((UnityEngine.Object) this._selectedContainer != (UnityEngine.Object) null)
                this.selectedContainer = (InventoryContainerUI) null;
              if (!this.MoveStorableBetweenInventories)
              {
                if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
                {
                  this.lastLootStor = this.selectedStorable;
                  this.selectedStorable = (StorableUI) null;
                }
              }
              else
                this.MoveStorableBetweenInventories = false;
              break;
            }
            break;
          case LootWindow.Modes.Loot:
            if (this.inventories[1].GetComponentsInChildren<StorableUI>().Length == 0)
              return;
            if (InputService.Instance.JoystickUsed)
            {
              this.inventories[0].SetActive(false);
              this.inventories[1].SetActive(true);
            }
            this.currentInventory = this.inventories[1];
            this.selectedInventoryIndex = 1;
            if (this._currentMode == LootWindow.Modes.Inventory || this._currentMode == LootWindow.Modes.None)
            {
              ServiceLocator.GetService<GameActionService>();
              if (!this.MoveStorableBetweenInventories)
              {
                if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
                {
                  this.lastInventoryStor = this.selectedStorable;
                  this.selectedStorable = (StorableUI) null;
                }
              }
              else
                this.MoveStorableBetweenInventories = false;
              break;
            }
            break;
          default:
            this.selectedContainer = (InventoryContainerUI) null;
            this.currentInventory = (ContainerResizableWindow) null;
            this.inventories[0].SetActive(false);
            this.inventories[1].SetActive(false);
            break;
        }
        this._currentMode = value;
      }
    }

    private bool MainControl(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if ((UnityEngine.Object) this._selectedContainer != (UnityEngine.Object) null)
      {
        if (type == GameActionType.Submit & down)
        {
          if (this._selectedContainer.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Locked || StorageUtility.GetItemAmount(this.Actor.Items, this.lockpickTemplate.Value) > 0)
          {
            this._selectedContainer.Button.GamepadStartHold();
            return true;
          }
        }
        else if (type == GameActionType.Submit && !down)
        {
          this._selectedContainer.Button.GamepadEndHold();
          return true;
        }
      }
      if (((!((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null) ? 0 : (type == GameActionType.Submit ? 1 : 0)) & (down ? 1 : 0)) == 0)
        return false;
      if (this.selectedStorable.Internal.Storage == this.Target)
      {
        this.InteractItem(this.selectedStorable.Internal);
        this.HideInfoWindow();
      }
      else
        this.ContextListener(type, down);
      return true;
    }

    protected override void OnNavigate(
      BaseInventoryWindow<LootWindow>.Navigation navigation)
    {
      if ((UnityEngine.Object) this.selectedStorable == (UnityEngine.Object) null && (UnityEngine.Object) this.selectedContainer == (UnityEngine.Object) null)
        this.CurrentMode = navigation != BaseInventoryWindow<LootWindow>.Navigation.CellRight && navigation != BaseInventoryWindow<LootWindow>.Navigation.ContainerRight ? LootWindow.Modes.Loot : LootWindow.Modes.Inventory;
      base.OnNavigate(navigation);
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (joystick)
      {
        if ((UnityEngine.Object) this.selectedContainer != (UnityEngine.Object) null)
          this.ShowClosedContainerInfo(this.selectedContainer.InventoryContainer);
        this.SetSelectedContainer();
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Loot, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        if (!((UnityEngine.Object) this.currentInventory != (UnityEngine.Object) null))
          return;
        this.currentInventory.SetActive(true);
      }
      else
      {
        this.SetSelectedContainer();
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Loot, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        this.inventories[0].SetActive(false);
        this.inventories[1].SetActive(false);
      }
    }

    private bool OnChangeInventory(GameActionType type, bool down)
    {
      if (!this.drag.IsEnabled)
      {
        if (type == GameActionType.BumperSelectionLeft & down && this.CurrentMode != LootWindow.Modes.Loot)
        {
          this.CurrentMode = LootWindow.Modes.Loot;
          this.OnNavigate(BaseInventoryWindow<LootWindow>.Navigation.ContainerLeft);
          return true;
        }
        if (type == GameActionType.BumperSelectionRight & down && this.CurrentMode != LootWindow.Modes.Inventory)
        {
          this.CurrentMode = LootWindow.Modes.Inventory;
          this.OnNavigate(BaseInventoryWindow<LootWindow>.Navigation.ContainerRight);
          return true;
        }
      }
      return false;
    }

    protected override bool DragBegin(IStorableComponent storable)
    {
      if (this.drag.IsEnabled || storable == null || storable.IsDisposed || !this.storables.ContainsKey(storable) || (UnityEngine.Object) this.selectedContainer != (UnityEngine.Object) null || storable.Container != null && this.containerViews.ContainsKey(storable.Container) && !this.containerViews[storable.Container].ClickEnabled)
        return false;
      if (this.Target.Items.Contains<IStorableComponent>(storable))
        this.OnGetLoot(storable);
      if (!base.DragBegin(storable))
        return false;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.MainControl));
      return true;
    }

    private void OnGetLoot(IStorableComponent storable)
    {
      if (this.targetItems == null || !this.targetItems.Contains(storable))
        return;
      this.targetItems.Remove(storable);
      this.Actor.GetComponent<PlayerControllerComponent>()?.OnGetLoot(this.Target);
    }

    protected override void Drag()
    {
      if (!this.drag.IsEnabled)
        return;
      base.Drag();
    }

    protected override void DragEnd(Intersect intersect, out bool isSuccess)
    {
      base.DragEnd(intersect, out isSuccess);
      if (!isSuccess)
        return;
      if (this.Target == null || this.Target.IsDisposed)
        ServiceLocator.GetService<UIService>().Pop();
      if ((UnityEngine.Object) this.currentInventory != (UnityEngine.Object) null)
      {
        StorableUI[] componentsInChildren = this.currentInventory.GetComponentsInChildren<StorableUI>();
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.MainControl));
        if (!((IEnumerable<StorableUI>) componentsInChildren).Contains<StorableUI>(this.selectedStorable))
        {
          this.MoveStorableBetweenInventories = true;
          this.CurrentMode = this.selectedInventoryIndex == 0 ? LootWindow.Modes.Loot : LootWindow.Modes.Inventory;
        }
      }
    }

    protected override bool ValidateContainer(
      IInventoryComponent container,
      IStorageComponent storage)
    {
      if (!base.ValidateContainer(container, storage))
        return false;
      bool flag = container.GetGroup() == InventoryGroup.Loot || storage == this.Actor;
      if (this.Target != null && this.Target.Owner != null && this.Target.Owner.GetComponent<NpcControllerComponent>() != null && container.Owner.Template.TemplateId == this.deadNpcEntityContainer.Value.Id)
      {
        IParameter<bool> byName = this.Target.Owner.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.Dead);
        if (byName != null)
          return byName.Value && flag;
      }
      return flag;
    }

    protected override void InteractItem(IStorableComponent storable)
    {
      if (!this.Target.Items.Contains<IStorableComponent>(storable))
        return;
      if (InputService.Instance.JoystickUsed)
      {
        this.FillSelectableList(this.selectableList, false, false);
        List<GameObject> list = this.selectableList.Where<GameObject>((Func<GameObject, bool>) (sel => (UnityEngine.Object) sel.GetComponent<InventoryContainerUI>() != (UnityEngine.Object) null)).ToList<GameObject>();
        if (list.Count > 0)
        {
          int num = 0;
          foreach (GameObject gameObject in list)
            num += ((IEnumerable<StorableUI>) gameObject.GetComponentsInChildren<StorableUI>()).ToList<StorableUI>().Count;
          if (((IEnumerable<StorableUI>) this.currentInventory.GetComponentsInChildren<StorableUI>()).ToList<StorableUI>().Count - 1 - num > 0)
          {
            foreach (GameObject gameObject in list)
              this.selectableList.Remove(gameObject);
          }
        }
        this.OnSelectObject(UISelectableHelper.SelectClosest((IEnumerable<GameObject>) this.selectableList, this.lastStorablePosition));
      }
      this.OnGetLoot(storable);
      this.MoveItem(storable, this.Actor);
      StorableComponentUtility.PlayTakeSound(storable);
      --this.currentStorableCountInLoot;
    }

    public override bool HaveToFindSelected() => false;

    protected override void AdditionalAfterChangeAction()
    {
      if ((UnityEngine.Object) this.selectedContainer != (UnityEngine.Object) null)
        this.lastStorablePosition = this.selectedContainer.transform.position;
      base.AdditionalAfterChangeAction();
    }

    private InventoryContainerUI FindContainerByComponent(IInventoryComponent component)
    {
      if (component == null)
        return (InventoryContainerUI) null;
      List<InventoryContainerUI> list = ((IEnumerable<InventoryContainerUI>) this.lootContainerWindow.GetComponentsInChildren<InventoryContainerUI>()).ToList<InventoryContainerUI>();
      if (list.Count == 0)
        return (InventoryContainerUI) null;
      foreach (InventoryContainerUI containerByComponent in list)
      {
        if (containerByComponent.InventoryContainer == component)
          return containerByComponent;
      }
      return (InventoryContainerUI) null;
    }

    protected override void OnContainerOpenEnd(IInventoryComponent container, bool complete)
    {
      base.OnContainerOpenEnd(container, complete);
      if (!complete)
        return;
      this.Actor.GetComponent<PlayerControllerComponent>()?.OnOpenContainer(container);
      if ((UnityEngine.Object) this._selectedContainer == (UnityEngine.Object) null || this._selectedContainer.InventoryContainer != container)
        this.selectedContainer = this.FindContainerByComponent(container);
      if ((UnityEngine.Object) this._selectedContainer == (UnityEngine.Object) null)
        return;
      if (this._selectedContainer.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open)
      {
        this.ShowClosedContainerInfo(this._selectedContainer.InventoryContainer);
      }
      else
      {
        StorableUI componentInChildren = this._selectedContainer.GetComponentInChildren<StorableUI>();
        if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
        {
          this.selectedContainer = (InventoryContainerUI) null;
          this.OnSelectObject(componentInChildren.gameObject);
        }
        else
        {
          this.FillSelectableList(this.selectableList, false, false);
          this.OnSelectObject(UISelectableHelper.SelectClosest((IEnumerable<GameObject>) this.selectableList, this.selectedContainer.transform.position));
        }
      }
    }

    protected override void OpenEnd(InventoryContainerUI uiContainer, bool complete)
    {
      this.OpenEnd(uiContainer, complete, true);
    }

    public override void Initialize()
    {
      this.RegisterLayer<ILootWindow>((ILootWindow) this);
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (ILootWindow);

    private void CheckEmpty()
    {
      if (this.Target.Items.Count<IStorableComponent>() != 0)
        return;
      ServiceLocator.GetService<DropBagService>().TryDestroyDropBag(this.Target);
    }

    protected override void UpdateContainerStates() => base.UpdateContainerStates();

    protected override void ShowClosedContainerInfo(IInventoryComponent container)
    {
      base.ShowClosedContainerInfo(container);
      this.unlockContainerTooltip.SetActive(container.OpenState.Value == ContainerOpenStateEnum.Locked);
      this.openContainerTooltip.SetActive(container.OpenState.Value == ContainerOpenStateEnum.Closed);
      if (container.OpenState.Value == ContainerOpenStateEnum.Locked)
        this.ShowLockpickInfo();
      else
        this.HideLockpickInfo();
    }

    protected override void HideClosedContainerInfo()
    {
      base.HideClosedContainerInfo();
      this.HideLockpickInfo();
    }

    private void ShowLockpickInfo()
    {
      this.lockpickTooltip.SetActive(true);
      int itemAmount = StorageUtility.GetItemAmount(this.Actor.Items, this.lockpickTemplate.Value);
      this.lockpickTooltip.SetItem(this.lockpickTemplate.Value.GetComponent<StorableComponent>());
      this.lockpickTooltip.SetCount(itemAmount);
    }

    private void HideLockpickInfo() => this.lockpickTooltip.SetActive(false);

    protected override void AddActionsToInfoWindow(
      InfoWindowNew window,
      IStorableComponent storable)
    {
      bool joystickUsed = InputService.Instance.JoystickUsed;
      window.AddActionTooltip(joystickUsed ? GameActionType.Context : GameActionType.Submit, "{StorableTooltip.Drag}");
      if (this.Target.Items.Contains<IStorableComponent>(storable))
        window.AddActionTooltip(joystickUsed ? GameActionType.Submit : GameActionType.Context, "{StorableTooltip.MoveToInventory}");
      else
        window.AddActionTooltip(joystickUsed ? GameActionType.Submit : GameActionType.Context, "{StorableTooltip.ContextMenu}");
      if (!StorableComponentUtility.IsSplittable(storable))
        return;
      window.AddActionTooltip(GameActionType.Split, "{StorableTooltip.Split}");
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (((IParameter<bool>) parameter).Value)
        return;
      ServiceLocator.GetService<UIService>().Pop();
    }

    protected override void FillSelectableList(List<GameObject> list, bool block, bool addSelected = false)
    {
      list.Clear();
      foreach (KeyValuePair<IStorableComponent, StorableUI> storable in this.storables)
      {
        if (addSelected || !((UnityEngine.Object) storable.Value == (UnityEngine.Object) this.selectedStorable))
        {
          IInventoryComponent container = storable.Key.Container;
          if (!((UnityEngine.Object) storable.Value == (UnityEngine.Object) this.selectedStorable) && container.OpenState.Value == ContainerOpenStateEnum.Open && (!block || container == null || !((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null) || container != this.selectedStorable.Internal.Container))
          {
            Vector3 position = storable.Value.transform.position;
            if ((double) position.x >= 0.0 && (double) position.y >= 0.0 && (double) position.x <= (double) Screen.width && (double) position.y <= (double) Screen.height)
              list.Add(storable.Value.gameObject);
          }
        }
      }
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in this.containers)
      {
        if (container.Key.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open)
          list.Add(container.Key.gameObject);
      }
    }

    protected override Vector2 CurentNavigationPosition()
    {
      RectTransform transform = (UnityEngine.Object) this._selectedContainer != (UnityEngine.Object) null ? this._selectedContainer.transform as RectTransform : (RectTransform) null;
      return (UnityEngine.Object) transform != (UnityEngine.Object) null ? (Vector2) transform.TransformPoint((Vector3) transform.rect.center) : base.CurentNavigationPosition();
    }

    protected override void OnSelectObject(GameObject selected)
    {
      ContainerResizableWindow componentInParent = selected?.GetComponentInParent<ContainerResizableWindow>();
      if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) componentInParent == (UnityEngine.Object) this.lootContainerWindow && this.CurrentMode != LootWindow.Modes.Loot)
          this.CurrentMode = LootWindow.Modes.Loot;
        else if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) this.lootContainerWindow && this.CurrentMode != LootWindow.Modes.Inventory)
          this.CurrentMode = LootWindow.Modes.Inventory;
      }
      InventoryContainerUI component = selected?.GetComponent<InventoryContainerUI>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        this.selectedContainer = component;
      }
      else
      {
        if ((UnityEngine.Object) selected != (UnityEngine.Object) null)
          this.selectedContainer = (InventoryContainerUI) null;
        base.OnSelectObject(selected);
      }
    }

    protected void SetSelectedContainer()
    {
      if ((UnityEngine.Object) this.selectionFrame == (UnityEngine.Object) null)
        return;
      this.selectionFrame.gameObject.SetActive(InputService.Instance.JoystickUsed && (UnityEngine.Object) this._selectedContainer != (UnityEngine.Object) null);
      if (!((UnityEngine.Object) this._selectedContainer != (UnityEngine.Object) null))
        return;
      Image imageForeground = this._selectedContainer.ImageForeground;
      this.selectionFrame.rectTransform.sizeDelta = imageForeground.rectTransform.rect.size;
      this.selectionFrame.rectTransform.position = imageForeground.rectTransform.TransformPoint((Vector3) imageForeground.rectTransform.rect.center);
    }

    protected override void GetFirstStorable()
    {
      if (!((UnityEngine.Object) this.selectedStorable == (UnityEngine.Object) null) || !((UnityEngine.Object) this.selectedContainer == (UnityEngine.Object) null))
        return;
      this.FillSelectableList(this.selectableList, false, false);
      Vector3 origin = this.transform.position;
      if ((UnityEngine.Object) this.currentInventory != (UnityEngine.Object) null)
      {
        this.selectableList.RemoveAll((Predicate<GameObject>) (s => !s.transform.IsChildOf(this.currentInventory.transform)));
        RectTransform component = this.currentInventory.GetComponent<RectTransform>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          origin = new Vector3(0.0f, component.position.y + component.sizeDelta.y);
      }
      this.OnSelectObject(UISelectableHelper.SelectClosest((IEnumerable<GameObject>) this.selectableList, origin));
    }

    private enum Modes
    {
      None,
      Inventory,
      Loot,
    }
  }
}
