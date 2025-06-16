using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private int currentStorableCountInLoot;
    private Modes _currentMode = Modes.None;
    private StorableUI lastLootStor;
    private StorableUI lastInventoryStor;
    private int selectedInventoryIndex = -1;
    private bool MoveStorableBetweenInventories;
    private bool isLootActive = true;

    public IStorageComponent Target { get; set; }

    protected override InventoryContainerUI selectedContainer
    {
      get => _selectedContainer;
      set
      {
        if (_selectedContainer == value)
          return;
        HideClosedContainerInfo();
        if (selectedStorable != null)
        {
          selectedStorable = null;
          HideInfoWindow();
        }
        if (_selectedContainer != null)
          _selectedContainer.Button.GamepadEndHold();
        _selectedContainer = value;
        if (value != null)
        {
          ShowClosedContainerInfo(_selectedContainer.InventoryContainer);
        }
        else
        {
          unlockContainerTooltip.SetActive(false);
          openContainerTooltip.SetActive(false);
        }
        SetSelectedContainer();
      }
    }

    private void FindTargetItems()
    {
      targetItems = new List<IStorableComponent>();
      targetItems.AddRange(Target.Items);
    }

    private void ResizeTargetContainerView()
    {
      List<InventoryContainerUI> containers = new List<InventoryContainerUI>();
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in this.containers)
      {
        if (container.Value == Target)
          containers.Add(container.Key);
      }
      ResizeContainersWindow(lootContainerWindow, containers);
    }

    private void CheckLivingNpc(bool subscribe = true)
    {
      if (Target?.Owner?.GetComponent<NpcControllerComponent>() == null)
        return;
      ParametersComponent component = Target.Owner.GetComponent<ParametersComponent>();
      IParameter<bool> byName1 = component?.GetByName<bool>(ParameterNameEnum.Dead);
      if (byName1 == null || !byName1.Value)
      {
        IParameter<bool> byName2 = component?.GetByName<bool>(ParameterNameEnum.Surrender);
        if (byName2 != null)
        {
          byName2.RemoveListener(this);
          if (subscribe)
            byName2.AddListener(this);
        }
      }
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionLeft, OnChangeInventory);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionRight, OnChangeInventory);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      inventories = new List<ContainerResizableWindow>(GetComponentsInChildren<ContainerResizableWindow>());
      actors.Clear();
      actors.Add(Actor);
      actors.Add(Target);
      Build2();
      HideLockpickInfo();
      ResizeTargetContainerView();
      CheckLivingNpc();
      FindTargetItems();
    }

    protected override IEnumerator AfterEnabled()
    {
      yield return base.AfterEnabled();
      if (InputService.Instance.JoystickUsed)
        OnNavigate(Navigation.CellLeft);
    }

    protected override void OnDisable()
    {
      CheckLivingNpc(false);
      CheckEmpty();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Loot, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionLeft, OnChangeInventory);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionRight, OnChangeInventory);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, MainControl);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, new GameActionHandle(((BaseInventoryWindow<LootWindow>) this).DragListener));
      CurrentMode = Modes.None;
      Color color = inventories[1].GetComponentInChildren<Image>().color with
      {
        a = 1f
      };
      inventories[1].GetComponentInChildren<Image>().color = color;
      selectedInventoryIndex = -1;
      base.OnDisable();
    }

    protected override void Subscribe()
    {
      base.Subscribe();
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.BumperSelectionLeft, OnChangeInventory);
      service.AddListener(GameActionType.BumperSelectionRight, OnChangeInventory);
      service.AddListener(GameActionType.Submit, MainControl);
      service.RemoveListener(GameActionType.Submit, new GameActionHandle(((BaseInventoryWindow<LootWindow>) this).ContextListener));
      service.AddListener(GameActionType.Context, new GameActionHandle(((BaseInventoryWindow<LootWindow>) this).DragListener));
    }

    protected override void Unsubscribe()
    {
      base.Unsubscribe();
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.BumperSelectionLeft, OnChangeInventory);
      service.RemoveListener(GameActionType.BumperSelectionRight, OnChangeInventory);
      service.RemoveListener(GameActionType.Submit, MainControl);
      service.RemoveListener(GameActionType.Context, new GameActionHandle(((BaseInventoryWindow<LootWindow>) this).DragListener));
    }

    private Modes CurrentMode
    {
      get => _currentMode;
      set
      {
        if (_currentMode == value)
          return;
        HideContextMenu();
        HideInfoWindow();
        switch (value)
        {
          case Modes.Inventory:
            unlockContainerTooltip.SetActive(false);
            openContainerTooltip.SetActive(false);
            if (inventories[0].GetComponentsInChildren<StorableUI>().Length == 0)
              return;
            if (!isLootActive)
            {
              Color color = inventories[1].GetComponentInChildren<Image>().color with
              {
                a = 1f
              };
              inventories[1].GetComponentInChildren<Image>().color = color;
              isLootActive = true;
            }
            if (InputService.Instance.JoystickUsed)
            {
              inventories[0].SetActive(true);
              inventories[1].SetActive(false);
            }
            currentInventory = inventories[0];
            selectedInventoryIndex = 0;
            if (_currentMode == Modes.Loot)
            {
              ServiceLocator.GetService<GameActionService>();
              if (_selectedContainer != null)
                selectedContainer = null;
              if (!MoveStorableBetweenInventories)
              {
                if (selectedStorable != null)
                {
                  lastLootStor = selectedStorable;
                  selectedStorable = null;
                }
              }
              else
                MoveStorableBetweenInventories = false;
            }
            break;
          case Modes.Loot:
            if (inventories[1].GetComponentsInChildren<StorableUI>().Length == 0)
              return;
            if (InputService.Instance.JoystickUsed)
            {
              inventories[0].SetActive(false);
              inventories[1].SetActive(true);
            }
            currentInventory = inventories[1];
            selectedInventoryIndex = 1;
            if (_currentMode == Modes.Inventory || _currentMode == Modes.None)
            {
              ServiceLocator.GetService<GameActionService>();
              if (!MoveStorableBetweenInventories)
              {
                if (selectedStorable != null)
                {
                  lastInventoryStor = selectedStorable;
                  selectedStorable = null;
                }
              }
              else
                MoveStorableBetweenInventories = false;
            }
            break;
          default:
            selectedContainer = null;
            currentInventory = null;
            inventories[0].SetActive(false);
            inventories[1].SetActive(false);
            break;
        }
        _currentMode = value;
      }
    }

    private bool MainControl(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (_selectedContainer != null)
      {
        if (type == GameActionType.Submit & down)
        {
          if (_selectedContainer.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Locked || StorageUtility.GetItemAmount(Actor.Items, lockpickTemplate.Value) > 0)
          {
            _selectedContainer.Button.GamepadStartHold();
            return true;
          }
        }
        else if (type == GameActionType.Submit && !down)
        {
          _selectedContainer.Button.GamepadEndHold();
          return true;
        }
      }
      if (((!(selectedStorable != null) ? 0 : (type == GameActionType.Submit ? 1 : 0)) & (down ? 1 : 0)) == 0)
        return false;
      if (selectedStorable.Internal.Storage == Target)
      {
        InteractItem(selectedStorable.Internal);
        HideInfoWindow();
      }
      else
        ContextListener(type, down);
      return true;
    }

    protected override void OnNavigate(
      Navigation navigation)
    {
      if (selectedStorable == null && selectedContainer == null)
        CurrentMode = navigation != Navigation.CellRight && navigation != Navigation.ContainerRight ? Modes.Loot : Modes.Inventory;
      base.OnNavigate(navigation);
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (joystick)
      {
        if (selectedContainer != null)
          ShowClosedContainerInfo(selectedContainer.InventoryContainer);
        SetSelectedContainer();
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Loot, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        if (!(currentInventory != null))
          return;
        currentInventory.SetActive(true);
      }
      else
      {
        SetSelectedContainer();
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Loot, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        inventories[0].SetActive(false);
        inventories[1].SetActive(false);
      }
    }

    private bool OnChangeInventory(GameActionType type, bool down)
    {
      if (!drag.IsEnabled)
      {
        if (type == GameActionType.BumperSelectionLeft & down && CurrentMode != Modes.Loot)
        {
          CurrentMode = Modes.Loot;
          OnNavigate(Navigation.ContainerLeft);
          return true;
        }
        if (type == GameActionType.BumperSelectionRight & down && CurrentMode != Modes.Inventory)
        {
          CurrentMode = Modes.Inventory;
          OnNavigate(Navigation.ContainerRight);
          return true;
        }
      }
      return false;
    }

    protected override bool DragBegin(IStorableComponent storable)
    {
      if (drag.IsEnabled || storable == null || storable.IsDisposed || !storables.ContainsKey(storable) || selectedContainer != null || storable.Container != null && containerViews.ContainsKey(storable.Container) && !containerViews[storable.Container].ClickEnabled)
        return false;
      if (Target.Items.Contains(storable))
        OnGetLoot(storable);
      if (!base.DragBegin(storable))
        return false;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, MainControl);
      return true;
    }

    private void OnGetLoot(IStorableComponent storable)
    {
      if (targetItems == null || !targetItems.Contains(storable))
        return;
      targetItems.Remove(storable);
      Actor.GetComponent<PlayerControllerComponent>()?.OnGetLoot(Target);
    }

    protected override void Drag()
    {
      if (!drag.IsEnabled)
        return;
      base.Drag();
    }

    protected override void DragEnd(Intersect intersect, out bool isSuccess)
    {
      base.DragEnd(intersect, out isSuccess);
      if (!isSuccess)
        return;
      if (Target == null || Target.IsDisposed)
        ServiceLocator.GetService<UIService>().Pop();
      if (currentInventory != null)
      {
        StorableUI[] componentsInChildren = currentInventory.GetComponentsInChildren<StorableUI>();
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, MainControl);
        if (!componentsInChildren.Contains(selectedStorable))
        {
          MoveStorableBetweenInventories = true;
          CurrentMode = selectedInventoryIndex == 0 ? Modes.Loot : Modes.Inventory;
        }
      }
    }

    protected override bool ValidateContainer(
      IInventoryComponent container,
      IStorageComponent storage)
    {
      if (!base.ValidateContainer(container, storage))
        return false;
      bool flag = container.GetGroup() == InventoryGroup.Loot || storage == Actor;
      if (Target != null && Target.Owner != null && Target.Owner.GetComponent<NpcControllerComponent>() != null && container.Owner.Template.TemplateId == deadNpcEntityContainer.Value.Id)
      {
        IParameter<bool> byName = Target.Owner.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.Dead);
        if (byName != null)
          return byName.Value && flag;
      }
      return flag;
    }

    protected override void InteractItem(IStorableComponent storable)
    {
      if (!Target.Items.Contains(storable))
        return;
      if (InputService.Instance.JoystickUsed)
      {
        FillSelectableList(selectableList, false);
        List<GameObject> list = selectableList.Where(sel => sel.GetComponent<InventoryContainerUI>() != null).ToList();
        if (list.Count > 0)
        {
          int num = 0;
          foreach (GameObject gameObject in list)
            num += gameObject.GetComponentsInChildren<StorableUI>().ToList().Count;
          if (currentInventory.GetComponentsInChildren<StorableUI>().ToList().Count - 1 - num > 0)
          {
            foreach (GameObject gameObject in list)
              selectableList.Remove(gameObject);
          }
        }
        OnSelectObject(UISelectableHelper.SelectClosest(selectableList, lastStorablePosition));
      }
      OnGetLoot(storable);
      MoveItem(storable, Actor);
      StorableComponentUtility.PlayTakeSound(storable);
      --currentStorableCountInLoot;
    }

    public override bool HaveToFindSelected() => false;

    protected override void AdditionalAfterChangeAction()
    {
      if (selectedContainer != null)
        lastStorablePosition = selectedContainer.transform.position;
      base.AdditionalAfterChangeAction();
    }

    private InventoryContainerUI FindContainerByComponent(IInventoryComponent component)
    {
      if (component == null)
        return null;
      List<InventoryContainerUI> list = lootContainerWindow.GetComponentsInChildren<InventoryContainerUI>().ToList();
      if (list.Count == 0)
        return null;
      foreach (InventoryContainerUI containerByComponent in list)
      {
        if (containerByComponent.InventoryContainer == component)
          return containerByComponent;
      }
      return null;
    }

    protected override void OnContainerOpenEnd(IInventoryComponent container, bool complete)
    {
      base.OnContainerOpenEnd(container, complete);
      if (!complete)
        return;
      Actor.GetComponent<PlayerControllerComponent>()?.OnOpenContainer(container);
      if (_selectedContainer == null || _selectedContainer.InventoryContainer != container)
        selectedContainer = FindContainerByComponent(container);
      if (_selectedContainer == null)
        return;
      if (_selectedContainer.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open)
      {
        ShowClosedContainerInfo(_selectedContainer.InventoryContainer);
      }
      else
      {
        StorableUI componentInChildren = _selectedContainer.GetComponentInChildren<StorableUI>();
        if (componentInChildren != null)
        {
          selectedContainer = null;
          OnSelectObject(componentInChildren.gameObject);
        }
        else
        {
          FillSelectableList(selectableList, false);
          OnSelectObject(UISelectableHelper.SelectClosest(selectableList, selectedContainer.transform.position));
        }
      }
    }

    protected override void OpenEnd(InventoryContainerUI uiContainer, bool complete)
    {
      OpenEnd(uiContainer, complete, true);
    }

    public override void Initialize()
    {
      RegisterLayer<ILootWindow>(this);
      base.Initialize();
    }

    public override Type GetWindowType() => typeof (ILootWindow);

    private void CheckEmpty()
    {
      if (Target.Items.Count() != 0)
        return;
      ServiceLocator.GetService<DropBagService>().TryDestroyDropBag(Target);
    }

    protected override void UpdateContainerStates() => base.UpdateContainerStates();

    protected override void ShowClosedContainerInfo(IInventoryComponent container)
    {
      base.ShowClosedContainerInfo(container);
      unlockContainerTooltip.SetActive(container.OpenState.Value == ContainerOpenStateEnum.Locked);
      openContainerTooltip.SetActive(container.OpenState.Value == ContainerOpenStateEnum.Closed);
      if (container.OpenState.Value == ContainerOpenStateEnum.Locked)
        ShowLockpickInfo();
      else
        HideLockpickInfo();
    }

    protected override void HideClosedContainerInfo()
    {
      base.HideClosedContainerInfo();
      HideLockpickInfo();
    }

    private void ShowLockpickInfo()
    {
      lockpickTooltip.SetActive(true);
      int itemAmount = StorageUtility.GetItemAmount(Actor.Items, lockpickTemplate.Value);
      lockpickTooltip.SetItem(lockpickTemplate.Value.GetComponent<StorableComponent>());
      lockpickTooltip.SetCount(itemAmount);
    }

    private void HideLockpickInfo() => lockpickTooltip.SetActive(false);

    protected override void AddActionsToInfoWindow(
      InfoWindowNew window,
      IStorableComponent storable)
    {
      bool joystickUsed = InputService.Instance.JoystickUsed;
      window.AddActionTooltip(joystickUsed ? GameActionType.Context : GameActionType.Submit, "{StorableTooltip.Drag}");
      if (Target.Items.Contains(storable))
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
      foreach (KeyValuePair<IStorableComponent, StorableUI> storable in storables)
      {
        if (addSelected || !(storable.Value == selectedStorable))
        {
          IInventoryComponent container = storable.Key.Container;
          if (!(storable.Value == selectedStorable) && container.OpenState.Value == ContainerOpenStateEnum.Open && (!block || container == null || !(selectedStorable != null) || container != selectedStorable.Internal.Container))
          {
            Vector3 position = storable.Value.transform.position;
            if (position.x >= 0.0 && position.y >= 0.0 && position.x <= (double) Screen.width && position.y <= (double) Screen.height)
              list.Add(storable.Value.gameObject);
          }
        }
      }
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in containers)
      {
        if (container.Key.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open)
          list.Add(container.Key.gameObject);
      }
    }

    protected override Vector2 CurentNavigationPosition()
    {
      RectTransform transform = _selectedContainer != null ? _selectedContainer.transform as RectTransform : null;
      return transform != null ? transform.TransformPoint(transform.rect.center) : base.CurentNavigationPosition();
    }

    protected override void OnSelectObject(GameObject selected)
    {
      ContainerResizableWindow componentInParent = selected?.GetComponentInParent<ContainerResizableWindow>();
      if (componentInParent != null)
      {
        if (componentInParent == lootContainerWindow && CurrentMode != Modes.Loot)
          CurrentMode = Modes.Loot;
        else if (componentInParent != lootContainerWindow && CurrentMode != Modes.Inventory)
          CurrentMode = Modes.Inventory;
      }
      InventoryContainerUI component = selected?.GetComponent<InventoryContainerUI>();
      if (component != null)
      {
        selectedContainer = component;
      }
      else
      {
        if (selected != null)
          selectedContainer = null;
        base.OnSelectObject(selected);
      }
    }

    protected void SetSelectedContainer()
    {
      if (selectionFrame == null)
        return;
      selectionFrame.gameObject.SetActive(InputService.Instance.JoystickUsed && _selectedContainer != null);
      if (!(_selectedContainer != null))
        return;
      Image imageForeground = _selectedContainer.ImageForeground;
      selectionFrame.rectTransform.sizeDelta = imageForeground.rectTransform.rect.size;
      selectionFrame.rectTransform.position = imageForeground.rectTransform.TransformPoint(imageForeground.rectTransform.rect.center);
    }

    protected override void GetFirstStorable()
    {
      if (!(selectedStorable == null) || !(selectedContainer == null))
        return;
      FillSelectableList(selectableList, false);
      Vector3 origin = transform.position;
      if (currentInventory != null)
      {
        selectableList.RemoveAll(s => !s.transform.IsChildOf(currentInventory.transform));
        RectTransform component = currentInventory.GetComponent<RectTransform>();
        if (component != null)
          origin = new Vector3(0.0f, component.position.y + component.sizeDelta.y);
      }
      OnSelectObject(UISelectableHelper.SelectClosest(selectableList, origin));
    }

    private enum Modes
    {
      None,
      Inventory,
      Loot,
    }
  }
}
