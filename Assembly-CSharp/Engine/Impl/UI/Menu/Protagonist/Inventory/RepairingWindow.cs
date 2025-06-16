using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Components.Repairing;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using Engine.Source.UI.Controls.BoolViews;
using InputServices;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class RepairingWindow : 
    SelectableInventoryWindow,
    IRepairingWindow,
    ISelectableInventoryWindow,
    IWindow,
    IChangeParameterListener
  {
    [Space]
    [SerializeField]
    private RepairingCostView costView;
    [SerializeField]
    private ItemSelector itemSelector;
    [SerializeField]
    private Image nonItemImage;
    [SerializeField]
    [FormerlySerializedAs("selecteEntityView")]
    private EntityView selectedEntityView;
    [SerializeField]
    private Button repairButton;
    [SerializeField]
    private HideableView noTargetMessage;
    [SerializeField]
    private HideableView noNeedForRepairMessage;
    [SerializeField]
    private HideableView notEnoughResourcesMessage;
    private IEntity target = (IEntity) null;
    private RepairableComponent targetRepairable = (RepairableComponent) null;
    private IParameter<float> durabilityParameter = (IParameter<float>) null;
    private RepairerComponent targetRepairer = (RepairerComponent) null;
    private bool targetIsHydrant;
    private bool RepairingVisible = false;
    [SerializeField]
    private GameObject _repairingHint;
    private StorableUI holdableStorable = (StorableUI) null;

    [Inspected]
    private IParameter<float> DurabilityParameter
    {
      get => this.durabilityParameter;
      set
      {
        if (this.durabilityParameter == value)
          return;
        if (this.durabilityParameter != null)
          this.durabilityParameter.RemoveListener((IChangeParameterListener) this);
        this.durabilityParameter = value;
        if (this.durabilityParameter != null)
          this.durabilityParameter.AddListener((IChangeParameterListener) this);
        this.OnInvalidate();
      }
    }

    [Inspected]
    public IEntity Target
    {
      get => this.target;
      set
      {
        if (this.target == value)
          return;
        this.target = value;
        if (this.target == null)
        {
          this.TargetRepairer = (RepairerComponent) null;
          this.TargetRepairable = (RepairableComponent) null;
        }
        else
        {
          this.TargetRepairer = this.target.GetComponent<RepairerComponent>();
          if (this.TargetRepairer == null)
            this.TargetRepairable = this.target.GetComponent<RepairableComponent>();
          TagsComponent component = this.target.GetComponent<TagsComponent>();
          if (component == null)
            return;
          this.targetIsHydrant = ((List<EntityTagEnum>) component.Tags).Contains(EntityTagEnum.Water_Supply_Hydrant);
        }
      }
    }

    [Inspected]
    private RepairableComponent TargetRepairable
    {
      get => this.targetRepairable;
      set
      {
        if (this.targetRepairable == value)
          return;
        this.targetRepairable = value;
        if (this.targetRepairable == null)
        {
          this.nonItemImage.gameObject.SetActive(false);
          this.nonItemImage.sprite = (Sprite) null;
          this.selectedEntityView.Value = (IEntity) null;
          this.DurabilityParameter = (IParameter<float>) null;
        }
        else
        {
          if (this.TargetRepairer == null)
          {
            this.nonItemImage.sprite = this.targetRepairable.Settings?.NonItemImage;
            this.nonItemImage.gameObject.SetActive((UnityEngine.Object) this.nonItemImage.sprite != (UnityEngine.Object) null);
          }
          this.selectedEntityView.Value = this.targetRepairable.Owner;
          this.DurabilityParameter = this.targetRepairable.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
        }
      }
    }

    [Inspected]
    private RepairerComponent TargetRepairer
    {
      get => this.targetRepairer;
      set
      {
        if (this.targetRepairer == value)
          return;
        this.targetRepairer = value;
        this.itemSelector.Groups.Clear();
        if (this.targetRepairer != null)
        {
          foreach (StorableGroup repairableGroup in this.targetRepairer.RepairableGroups)
            this.itemSelector.Groups.Add(repairableGroup);
          this.itemSelector.Storage = this.Actor;
          this.itemSelector.gameObject.SetActive(true);
        }
        else
        {
          this.itemSelector.Storage = (IStorageComponent) null;
          this.itemSelector.gameObject.SetActive(false);
        }
      }
    }

    private bool NeedRepair()
    {
      return (double) this.targetRepairable.TargetLevel().MaxDurability != (double) this.DurabilityParameter.Value;
    }

    private bool HaveEnoughResources()
    {
      foreach (RepairableCostItem repairableCostItem in this.targetRepairable.TargetLevel().Сost)
      {
        if (repairableCostItem.Template.Value != null && StorageUtility.GetItemAmount(this.Actor.Items, repairableCostItem.Template.Value) < repairableCostItem.Count)
          return false;
      }
      return true;
    }

    public override void Initialize()
    {
      this.RegisterLayer<IRepairingWindow>((IRepairingWindow) this);
      this.repairButton.onClick.AddListener(new UnityAction(this.Repair));
      this.itemSelector.ValidateItemEvent += new Func<ItemSelector, IStorableComponent, bool>(this.SelectorItemValid);
      this.itemSelector.ChangeItemEvent += new Action<ItemSelector, IStorableComponent, IStorableComponent>(this.OnSelectorItemChange);
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (IRepairingWindow);

    protected override bool ItemIsInteresting(IStorableComponent item)
    {
      return this.targetRepairer != null && item.GetComponent<RepairableComponent>() != null && this.targetRepairer.CanRepairItem(item);
    }

    protected override bool ItemIsSelected(IStorableComponent storable)
    {
      return storable == this.itemSelector.Item;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.CurrentMode = SelectableInventoryWindow.Modes.None;
      this.CurrentSelector = SelectableInventoryWindow.SelectedSelector.None;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickLeft, new GameActionHandle(((SelectableInventoryWindow) this).ConsoleController));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickRight, new GameActionHandle(((SelectableInventoryWindow) this).ConsoleController));
      this.SelectorButtons = new List<Button>((IEnumerable<Button>) this.itemSelector.GetComponentsInChildren<Button>());
      if (this.itemSelector.Item == null)
        return;
      this.holdableStorable = this.GetStorableByComponent(this.itemSelector.Item);
    }

    protected override void OnDisable()
    {
      this.Target = (IEntity) null;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, new GameActionHandle(((SelectableInventoryWindow) this).ConsoleController));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, new GameActionHandle(((SelectableInventoryWindow) this).ConsoleController));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.RepairController));
      this.UnsubscribeNavigation();
      this.SelectorButtons.Clear();
      if ((UnityEngine.Object) this.holdableStorable != (UnityEngine.Object) null)
      {
        this.holdableStorable.HoldSelected(false);
        this.holdableStorable = (StorableUI) null;
      }
      base.OnDisable();
    }

    private bool RepairController(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || !(type == GameActionType.Submit & down))
        return false;
      this.repairButton.onClick?.Invoke();
      this.HideInfoWindow();
      return true;
    }

    protected override bool ConsoleController(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || !down)
        return false;
      if ((UnityEngine.Object) this.holdableStorable != (UnityEngine.Object) null)
      {
        this.holdableStorable.HoldSelected(false);
        this.holdableStorable = (StorableUI) null;
      }
      if (type == GameActionType.LStickLeft & down)
      {
        ExecuteEvents.Execute<ISubmitHandler>(this.SelectorButtons[0].gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        this.OnInvalidate();
        return true;
      }
      if (!(type == GameActionType.LStickRight & down))
        return false;
      ExecuteEvents.Execute<ISubmitHandler>(this.SelectorButtons[1].gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
      this.OnInvalidate();
      return true;
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (this.RepairingVisible)
        this._repairingHint.SetActive(joystick);
      this.controlPanel.SetActive(this.itemSelector.gameObject.activeInHierarchy & joystick);
      if (!((UnityEngine.Object) this.holdableStorable != (UnityEngine.Object) null))
        return;
      this.holdableStorable.HoldSelected(joystick);
    }

    protected override void OnInvalidate()
    {
      base.OnInvalidate();
      this.RedrawCost();
      if (this.targetRepairable == null)
      {
        this.noNeedForRepairMessage.Visible = false;
        this.notEnoughResourcesMessage.Visible = false;
        HideableViewUtility.SetVisible(this.repairButton.gameObject, false);
        this._repairingHint.SetActive(false);
        this.RepairingVisible = false;
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.RepairController));
      }
      else
      {
        bool flag1 = !this.NeedRepair();
        this.noNeedForRepairMessage.Visible = flag1;
        if (flag1)
        {
          this.notEnoughResourcesMessage.Visible = false;
          HideableViewUtility.SetVisible(this.repairButton.gameObject, false);
          this._repairingHint.SetActive(false);
          this.RepairingVisible = false;
          ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.RepairController));
        }
        else
        {
          bool flag2 = !this.HaveEnoughResources();
          this.notEnoughResourcesMessage.Visible = flag2;
          this.RepairingVisible = !flag2;
          HideableViewUtility.SetVisible(this.repairButton.gameObject, this.RepairingVisible);
          this._repairingHint.SetActive(this.RepairingVisible);
          if (this.RepairingVisible)
            ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.RepairController));
          else
            ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.RepairController));
        }
      }
      if (!InputService.Instance.JoystickUsed)
        return;
      this.HideInfoWindow();
    }

    protected override void OnItemClick(IStorableComponent storable)
    {
      if (this.ItemIsSelected(storable))
        return;
      this.itemSelector.Item = storable;
    }

    private void OnSelectorItemChange(
      ItemSelector itemSelector,
      IStorableComponent prevIngredient,
      IStorableComponent newIngredient)
    {
      if (prevIngredient != null)
        StorableComponentUtility.PlayPutSound(prevIngredient);
      if (newIngredient != null)
        StorableComponentUtility.PlayTakeSound(newIngredient);
      this.TargetRepairable = newIngredient != null ? newIngredient.GetComponent<RepairableComponent>() : (RepairableComponent) null;
    }

    private void RedrawCost()
    {
      if ((UnityEngine.Object) this.costView == (UnityEngine.Object) null)
        return;
      RepairableLevel repairableLevel = this.targetRepairable?.TargetLevel();
      if (repairableLevel != null)
      {
        this.costView.Actor = this.Actor;
        this.costView.Cost = repairableLevel.Сost;
      }
      else
      {
        this.costView.Actor = (IStorageComponent) null;
        this.costView.Cost = (List<RepairableCostItem>) null;
      }
    }

    private void Repair()
    {
      if (!this.HaveEnoughResources())
        return;
      RepairableLevel repairableLevel = this.targetRepairable.TargetLevel();
      foreach (RepairableCostItem repairableCostItem in repairableLevel.Сost)
      {
        IEntity removingItem = repairableCostItem.Template.Value;
        if (removingItem != null)
          this.RemoveItemsAmount(this.Actor, removingItem, repairableCostItem.Count);
      }
      this.DurabilityParameter.Value = repairableLevel.MaxDurability;
      this.PlayAudio(this.targetRepairable.Settings?.RepairSound);
      if (!this.targetIsHydrant)
        return;
      this.Actor.GetComponent<PlayerControllerComponent>().ComputeRepairHydrant();
    }

    private bool SelectorItemValid(ItemSelector itemSelector, IStorableComponent item)
    {
      return item.GetComponent<RepairableComponent>() != null;
    }

    public void OnParameterChanged(IParameter parameter) => this.OnInvalidate();
  }
}
