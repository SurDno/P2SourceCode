using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Windows;
using Engine.Source.Components;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public abstract class SelectableInventoryWindow : 
    BaseInventoryWindow<SelectableInventoryWindow>,
    ISelectableInventoryWindow,
    IWindow
  {
    [SerializeField]
    protected ItemSelector[] ingredientSelectors;
    protected List<Button> SelectorButtons;
    private SelectableInventoryWindow.SelectedSelector _currentSelector = SelectableInventoryWindow.SelectedSelector.None;
    private SelectableInventoryWindow.SelectedSelector LastSelector = SelectableInventoryWindow.SelectedSelector.None;
    private SelectableInventoryWindow.Modes _currentMode = SelectableInventoryWindow.Modes.None;
    private SelectableInventoryWindow.Modes prevMode = SelectableInventoryWindow.Modes.None;
    protected List<StorableUI> selectedStorables = new List<StorableUI>();

    protected SelectableInventoryWindow.SelectedSelector CurrentSelector
    {
      get => this._currentSelector;
      set
      {
        if (this._currentSelector == value)
          return;
        this._currentSelector = value;
        switch (value)
        {
          case SelectableInventoryWindow.SelectedSelector.Top:
            if (InputService.Instance.JoystickUsed)
            {
              this.ingredientSelectors[0]?.SetSelection(true);
              if (this.ingredientSelectors.Length > 1)
                this.ingredientSelectors[1]?.SetSelection(false);
            }
            this.SelectorButtons = new List<Button>((IEnumerable<Button>) this.ingredientSelectors[0]?.GetComponentsInChildren<Button>());
            break;
          case SelectableInventoryWindow.SelectedSelector.Bottom:
            if (InputService.Instance.JoystickUsed)
            {
              this.ingredientSelectors[0]?.SetSelection(false);
              if (this.ingredientSelectors.Length > 1)
                this.ingredientSelectors[1]?.SetSelection(true);
            }
            this.SelectorButtons = new List<Button>((IEnumerable<Button>) this.ingredientSelectors[1]?.GetComponentsInChildren<Button>());
            break;
          default:
            this.ingredientSelectors[0]?.SetSelection(false);
            if (this.ingredientSelectors.Length > 1)
              this.ingredientSelectors[1]?.SetSelection(false);
            this.SelectorButtons.Clear();
            break;
        }
      }
    }

    protected SelectableInventoryWindow.Modes PreviousMode
    {
      get => this.prevMode;
      set => this.prevMode = value;
    }

    protected virtual SelectableInventoryWindow.Modes CurrentMode
    {
      get => this._currentMode;
      set
      {
        if (this._currentMode == value)
          return;
        this.HideInfoWindow();
        this.HideContextMenu();
        switch (value)
        {
          case SelectableInventoryWindow.Modes.None:
            this.UnsubscribeNavigation();
            this.CraftWindowUnsubscribe();
            this.CurrentSelector = SelectableInventoryWindow.SelectedSelector.None;
            break;
          case SelectableInventoryWindow.Modes.Inventory:
            if (this._currentMode == SelectableInventoryWindow.Modes.Craft)
              return;
            break;
          case SelectableInventoryWindow.Modes.Craft:
            if ((this._currentMode == SelectableInventoryWindow.Modes.Inventory || this._currentMode == SelectableInventoryWindow.Modes.None) && (UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
            {
              this.selectedStorable.SetSelected(false);
              this.selectedStorable = (StorableUI) null;
            }
            this.CurrentSelector = SelectableInventoryWindow.SelectedSelector.Top;
            break;
        }
        this._currentMode = value;
      }
    }

    protected virtual void CraftWindowSubscribe()
    {
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickLeft, new GameActionHandle(this.ConsoleController));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickRight, new GameActionHandle(this.ConsoleController));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, new GameActionHandle(this.ConsoleController));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, new GameActionHandle(this.ConsoleController));
    }

    protected virtual void CraftWindowUnsubscribe()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, new GameActionHandle(this.ConsoleController));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, new GameActionHandle(this.ConsoleController));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.ConsoleController));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.ConsoleController));
    }

    protected bool SwapModes(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.BumperSelectionLeft & down)
      {
        this.CurrentMode = SelectableInventoryWindow.Modes.Craft;
        return true;
      }
      if (!(type == GameActionType.BumperSelectionRight & down))
        return false;
      this.CurrentMode = SelectableInventoryWindow.Modes.Inventory;
      return true;
    }

    private void OnSelectorItemChange(
      ItemSelector arg1,
      IStorableComponent arg2,
      IStorableComponent arg3)
    {
      if (arg2 != null)
      {
        StorableUI storableByComponent = this.GetStorableByComponent(arg2);
        if ((UnityEngine.Object) storableByComponent != (UnityEngine.Object) null)
        {
          storableByComponent.HoldSelected(false);
          if (this.selectedStorables.Contains(storableByComponent))
            this.selectedStorables.Remove(storableByComponent);
        }
      }
      if (arg3 != null)
      {
        this.SetStorableByComponent(arg3);
        StorableUI storableByComponent = this.GetStorableByComponent(arg3);
        if ((UnityEngine.Object) storableByComponent != (UnityEngine.Object) null)
        {
          storableByComponent.HoldSelected(true);
          this.selectedStorables.Add(storableByComponent);
        }
      }
      this.HideInfoWindow();
    }

    protected virtual bool ConsoleController(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.LStickLeft & down)
      {
        ExecuteEvents.Execute<ISubmitHandler>(this.SelectorButtons[0].gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        this.OnInvalidate();
        this.HideInfoWindow();
        return true;
      }
      if (type == GameActionType.LStickRight & down)
      {
        ExecuteEvents.Execute<ISubmitHandler>(this.SelectorButtons[1].gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        this.OnInvalidate();
        this.HideInfoWindow();
        return true;
      }
      if (type == GameActionType.LStickUp & down)
      {
        this.CurrentSelector = SelectableInventoryWindow.SelectedSelector.Top;
        return true;
      }
      if (!(type == GameActionType.LStickDown & down))
        return false;
      this.CurrentSelector = SelectableInventoryWindow.SelectedSelector.Bottom;
      return true;
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (this.CurrentSelector != SelectableInventoryWindow.SelectedSelector.None)
        this.ingredientSelectors[(int) this.CurrentSelector]?.SetSelection(joystick);
      foreach (StorableUI selectedStorable in this.selectedStorables)
        selectedStorable?.HoldSelected(joystick);
      this.OnInvalidate();
    }

    protected virtual bool ItemIsSelected(IStorableComponent storable) => false;

    protected virtual void OnItemAlternativeClick(IStorableComponent storable)
    {
      this.OnItemClick(storable);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      if ((UnityEngine.Object) this.windowContextMenu != (UnityEngine.Object) null)
      {
        this.HideContextMenu();
      }
      else
      {
        if (!this.intersect.IsIntersected)
          return;
        if (this.intersect.Storables == null)
        {
          Debug.LogError((object) "intersect.Storables == null, Такого быть недолжно, воспроизвести, разобраться в чем дело и пофиксить нормально");
        }
        else
        {
          StorableComponent storable = this.intersect.Storables.FirstOrDefault<StorableComponent>();
          if (storable == null || !this.ItemIsInteresting((IStorableComponent) storable))
            return;
          switch (eventData.button)
          {
            case PointerEventData.InputButton.Left:
              this.OnItemClick((IStorableComponent) storable);
              break;
            case PointerEventData.InputButton.Right:
              this.OnItemAlternativeClick((IStorableComponent) storable);
              break;
          }
        }
      }
    }

    protected override void OnInvalidate()
    {
      base.OnInvalidate();
      foreach (KeyValuePair<IStorableComponent, StorableUI> storable in this.storables)
      {
        storable.Value.Enable(this.ItemIsInteresting(storable.Key));
        storable.Value.SetSelected(this.ItemIsSelected(storable.Key));
        storable.Value.HoldSelected(this.ItemIsSelected(storable.Key));
      }
      if (!InputService.Instance.JoystickUsed)
        return;
      if (this.ingredientSelectors.Length > 1)
        this.ingredientSelectors[this.CurrentSelector == SelectableInventoryWindow.SelectedSelector.Top ? 1 : 0].CheckButtonsForConsole();
      else
        this.ingredientSelectors[0].CheckButtonsForConsole();
    }

    protected virtual void OnItemClick(IStorableComponent storable)
    {
    }

    protected override void AddActionsToInfoWindow(
      InfoWindowNew window,
      IStorableComponent storable)
    {
      if (!this.ItemIsInteresting(storable) || InputService.Instance.JoystickUsed && this.CurrentMode != SelectableInventoryWindow.Modes.Inventory)
        return;
      window.AddActionTooltip(GameActionType.Submit, "{StorableTooltip.Select}");
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.CanShowInfoWindows = false;
      this.actors.Clear();
      this.actors.Add(this.Actor);
      this.Build2();
      this.Unsubscribe();
      this.UnsubscribeNavigation();
      this.CraftWindowSubscribe();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      this.CurrentMode = SelectableInventoryWindow.Modes.Craft;
      foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
        ingredientSelector.ChangeItemEvent += new Action<ItemSelector, IStorableComponent, IStorableComponent>(this.OnSelectorItemChange);
      if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
      {
        this.selectedStorable.SetSelected(false);
        this.selectedStorable.HoldSelected(false);
        this.selectedStorable = (StorableUI) null;
      }
      this.HideInfoWindow();
      this.HideContextMenu();
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      this.CraftWindowUnsubscribe();
      foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
        ingredientSelector.ChangeItemEvent -= new Action<ItemSelector, IStorableComponent, IStorableComponent>(this.OnSelectorItemChange);
      this.CurrentMode = SelectableInventoryWindow.Modes.None;
      this.CurrentSelector = SelectableInventoryWindow.SelectedSelector.None;
      this.selectedStorables.Clear();
      base.OnDisable();
    }

    protected enum Modes
    {
      None,
      Inventory,
      Craft,
    }

    protected enum SelectedSelector
    {
      None = -1, // 0xFFFFFFFF
      Top = 0,
      Bottom = 1,
    }
  }
}
