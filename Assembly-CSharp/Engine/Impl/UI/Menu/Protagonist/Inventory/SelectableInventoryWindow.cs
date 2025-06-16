using System.Collections.Generic;
using System.Linq;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Windows;
using Engine.Source.Components;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;
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
    private SelectedSelector _currentSelector = SelectedSelector.None;
    private SelectedSelector LastSelector = SelectedSelector.None;
    private Modes _currentMode = Modes.None;
    private Modes prevMode = Modes.None;
    protected List<StorableUI> selectedStorables = new List<StorableUI>();

    protected SelectedSelector CurrentSelector
    {
      get => _currentSelector;
      set
      {
        if (_currentSelector == value)
          return;
        _currentSelector = value;
        switch (value)
        {
          case SelectedSelector.Top:
            if (InputService.Instance.JoystickUsed)
            {
              ingredientSelectors[0]?.SetSelection(true);
              if (ingredientSelectors.Length > 1)
                ingredientSelectors[1]?.SetSelection(false);
            }
            SelectorButtons = new List<Button>(ingredientSelectors[0]?.GetComponentsInChildren<Button>());
            break;
          case SelectedSelector.Bottom:
            if (InputService.Instance.JoystickUsed)
            {
              ingredientSelectors[0]?.SetSelection(false);
              if (ingredientSelectors.Length > 1)
                ingredientSelectors[1]?.SetSelection(true);
            }
            SelectorButtons = new List<Button>(ingredientSelectors[1]?.GetComponentsInChildren<Button>());
            break;
          default:
            ingredientSelectors[0]?.SetSelection(false);
            if (ingredientSelectors.Length > 1)
              ingredientSelectors[1]?.SetSelection(false);
            SelectorButtons.Clear();
            break;
        }
      }
    }

    protected Modes PreviousMode
    {
      get => prevMode;
      set => prevMode = value;
    }

    protected virtual Modes CurrentMode
    {
      get => _currentMode;
      set
      {
        if (_currentMode == value)
          return;
        HideInfoWindow();
        HideContextMenu();
        switch (value)
        {
          case Modes.None:
            UnsubscribeNavigation();
            CraftWindowUnsubscribe();
            CurrentSelector = SelectedSelector.None;
            break;
          case Modes.Inventory:
            if (_currentMode == Modes.Craft)
              return;
            break;
          case Modes.Craft:
            if ((_currentMode == Modes.Inventory || _currentMode == Modes.None) && selectedStorable != null)
            {
              selectedStorable.SetSelected(false);
              selectedStorable = null;
            }
            CurrentSelector = SelectedSelector.Top;
            break;
        }
        _currentMode = value;
      }
    }

    protected virtual void CraftWindowSubscribe()
    {
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickLeft, ConsoleController);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickRight, ConsoleController);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, ConsoleController);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, ConsoleController);
    }

    protected virtual void CraftWindowUnsubscribe()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, ConsoleController);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, ConsoleController);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, ConsoleController);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, ConsoleController);
    }

    protected bool SwapModes(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.BumperSelectionLeft & down)
      {
        CurrentMode = Modes.Craft;
        return true;
      }
      if (!(type == GameActionType.BumperSelectionRight & down))
        return false;
      CurrentMode = Modes.Inventory;
      return true;
    }

    private void OnSelectorItemChange(
      ItemSelector arg1,
      IStorableComponent arg2,
      IStorableComponent arg3)
    {
      if (arg2 != null)
      {
        StorableUI storableByComponent = GetStorableByComponent(arg2);
        if (storableByComponent != null)
        {
          storableByComponent.HoldSelected(false);
          if (selectedStorables.Contains(storableByComponent))
            selectedStorables.Remove(storableByComponent);
        }
      }
      if (arg3 != null)
      {
        SetStorableByComponent(arg3);
        StorableUI storableByComponent = GetStorableByComponent(arg3);
        if (storableByComponent != null)
        {
          storableByComponent.HoldSelected(true);
          selectedStorables.Add(storableByComponent);
        }
      }
      HideInfoWindow();
    }

    protected virtual bool ConsoleController(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.LStickLeft & down)
      {
        ExecuteEvents.Execute(SelectorButtons[0].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        OnInvalidate();
        HideInfoWindow();
        return true;
      }
      if (type == GameActionType.LStickRight & down)
      {
        ExecuteEvents.Execute(SelectorButtons[1].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        OnInvalidate();
        HideInfoWindow();
        return true;
      }
      if (type == GameActionType.LStickUp & down)
      {
        CurrentSelector = SelectedSelector.Top;
        return true;
      }
      if (!(type == GameActionType.LStickDown & down))
        return false;
      CurrentSelector = SelectedSelector.Bottom;
      return true;
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (CurrentSelector != SelectedSelector.None)
        ingredientSelectors[(int) CurrentSelector]?.SetSelection(joystick);
      foreach (StorableUI selectedStorable in selectedStorables)
        selectedStorable?.HoldSelected(joystick);
      OnInvalidate();
    }

    protected virtual bool ItemIsSelected(IStorableComponent storable) => false;

    protected virtual void OnItemAlternativeClick(IStorableComponent storable)
    {
      OnItemClick(storable);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      if (windowContextMenu != null)
      {
        HideContextMenu();
      }
      else
      {
        if (!intersect.IsIntersected)
          return;
        if (intersect.Storables == null)
        {
          Debug.LogError("intersect.Storables == null, Такого быть недолжно, воспроизвести, разобраться в чем дело и пофиксить нормально");
        }
        else
        {
          StorableComponent storable = intersect.Storables.FirstOrDefault();
          if (storable == null || !ItemIsInteresting(storable))
            return;
          switch (eventData.button)
          {
            case PointerEventData.InputButton.Left:
              OnItemClick(storable);
              break;
            case PointerEventData.InputButton.Right:
              OnItemAlternativeClick(storable);
              break;
          }
        }
      }
    }

    protected override void OnInvalidate()
    {
      base.OnInvalidate();
      foreach (KeyValuePair<IStorableComponent, StorableUI> storable in storables)
      {
        storable.Value.Enable(ItemIsInteresting(storable.Key));
        storable.Value.SetSelected(ItemIsSelected(storable.Key));
        storable.Value.HoldSelected(ItemIsSelected(storable.Key));
      }
      if (!InputService.Instance.JoystickUsed)
        return;
      if (ingredientSelectors.Length > 1)
        ingredientSelectors[CurrentSelector == SelectedSelector.Top ? 1 : 0].CheckButtonsForConsole();
      else
        ingredientSelectors[0].CheckButtonsForConsole();
    }

    protected virtual void OnItemClick(IStorableComponent storable)
    {
    }

    protected override void AddActionsToInfoWindow(
      InfoWindowNew window,
      IStorableComponent storable)
    {
      if (!ItemIsInteresting(storable) || InputService.Instance.JoystickUsed && CurrentMode != Modes.Inventory)
        return;
      window.AddActionTooltip(GameActionType.Submit, "{StorableTooltip.Select}");
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      CanShowInfoWindows = false;
      actors.Clear();
      actors.Add(Actor);
      Build2();
      Unsubscribe();
      UnsubscribeNavigation();
      CraftWindowSubscribe();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, CancelListener);
      CurrentMode = Modes.Craft;
      foreach (ItemSelector ingredientSelector in ingredientSelectors)
        ingredientSelector.ChangeItemEvent += OnSelectorItemChange;
      if (selectedStorable != null)
      {
        selectedStorable.SetSelected(false);
        selectedStorable.HoldSelected(false);
        selectedStorable = null;
      }
      HideInfoWindow();
      HideContextMenu();
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, CancelListener);
      CraftWindowUnsubscribe();
      foreach (ItemSelector ingredientSelector in ingredientSelectors)
        ingredientSelector.ChangeItemEvent -= OnSelectorItemChange;
      CurrentMode = Modes.None;
      CurrentSelector = SelectedSelector.None;
      selectedStorables.Clear();
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
      None = -1,
      Top = 0,
      Bottom = 1,
    }
  }
}
