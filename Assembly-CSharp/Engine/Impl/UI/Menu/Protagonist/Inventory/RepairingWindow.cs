using System;
using System.Collections.Generic;
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
    private IEntity target = null;
    private RepairableComponent targetRepairable = null;
    private IParameter<float> durabilityParameter = null;
    private RepairerComponent targetRepairer = null;
    private bool targetIsHydrant;
    private bool RepairingVisible;
    [SerializeField]
    private GameObject _repairingHint;
    private StorableUI holdableStorable = null;

    [Inspected]
    private IParameter<float> DurabilityParameter
    {
      get => durabilityParameter;
      set
      {
        if (durabilityParameter == value)
          return;
        if (durabilityParameter != null)
          durabilityParameter.RemoveListener(this);
        durabilityParameter = value;
        if (durabilityParameter != null)
          durabilityParameter.AddListener(this);
        OnInvalidate();
      }
    }

    [Inspected]
    public IEntity Target
    {
      get => target;
      set
      {
        if (target == value)
          return;
        target = value;
        if (target == null)
        {
          TargetRepairer = null;
          TargetRepairable = null;
        }
        else
        {
          TargetRepairer = target.GetComponent<RepairerComponent>();
          if (TargetRepairer == null)
            TargetRepairable = target.GetComponent<RepairableComponent>();
          TagsComponent component = target.GetComponent<TagsComponent>();
          if (component == null)
            return;
          targetIsHydrant = ((List<EntityTagEnum>) component.Tags).Contains(EntityTagEnum.Water_Supply_Hydrant);
        }
      }
    }

    [Inspected]
    private RepairableComponent TargetRepairable
    {
      get => targetRepairable;
      set
      {
        if (targetRepairable == value)
          return;
        targetRepairable = value;
        if (targetRepairable == null)
        {
          nonItemImage.gameObject.SetActive(false);
          nonItemImage.sprite = (Sprite) null;
          selectedEntityView.Value = null;
          DurabilityParameter = null;
        }
        else
        {
          if (TargetRepairer == null)
          {
            nonItemImage.sprite = targetRepairable.Settings?.NonItemImage;
            nonItemImage.gameObject.SetActive((UnityEngine.Object) nonItemImage.sprite != (UnityEngine.Object) null);
          }
          selectedEntityView.Value = targetRepairable.Owner;
          DurabilityParameter = targetRepairable.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
        }
      }
    }

    [Inspected]
    private RepairerComponent TargetRepairer
    {
      get => targetRepairer;
      set
      {
        if (targetRepairer == value)
          return;
        targetRepairer = value;
        itemSelector.Groups.Clear();
        if (targetRepairer != null)
        {
          foreach (StorableGroup repairableGroup in targetRepairer.RepairableGroups)
            itemSelector.Groups.Add(repairableGroup);
          itemSelector.Storage = Actor;
          itemSelector.gameObject.SetActive(true);
        }
        else
        {
          itemSelector.Storage = null;
          itemSelector.gameObject.SetActive(false);
        }
      }
    }

    private bool NeedRepair()
    {
      return targetRepairable.TargetLevel().MaxDurability != (double) DurabilityParameter.Value;
    }

    private bool HaveEnoughResources()
    {
      foreach (RepairableCostItem repairableCostItem in targetRepairable.TargetLevel().Сost)
      {
        if (repairableCostItem.Template.Value != null && StorageUtility.GetItemAmount(Actor.Items, repairableCostItem.Template.Value) < repairableCostItem.Count)
          return false;
      }
      return true;
    }

    public override void Initialize()
    {
      RegisterLayer((IRepairingWindow) this);
      repairButton.onClick.AddListener(new UnityAction(Repair));
      itemSelector.ValidateItemEvent += SelectorItemValid;
      itemSelector.ChangeItemEvent += OnSelectorItemChange;
      base.Initialize();
    }

    public override Type GetWindowType() => typeof (IRepairingWindow);

    protected override bool ItemIsInteresting(IStorableComponent item)
    {
      return targetRepairer != null && item.GetComponent<RepairableComponent>() != null && targetRepairer.CanRepairItem(item);
    }

    protected override bool ItemIsSelected(IStorableComponent storable)
    {
      return storable == itemSelector.Item;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      CurrentMode = Modes.None;
      CurrentSelector = SelectedSelector.None;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickLeft, new GameActionHandle(((SelectableInventoryWindow) this).ConsoleController));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickRight, new GameActionHandle(((SelectableInventoryWindow) this).ConsoleController));
      SelectorButtons = new List<Button>((IEnumerable<Button>) itemSelector.GetComponentsInChildren<Button>());
      if (itemSelector.Item == null)
        return;
      holdableStorable = GetStorableByComponent(itemSelector.Item);
    }

    protected override void OnDisable()
    {
      Target = null;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, new GameActionHandle(((SelectableInventoryWindow) this).ConsoleController));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, new GameActionHandle(((SelectableInventoryWindow) this).ConsoleController));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, RepairController);
      UnsubscribeNavigation();
      SelectorButtons.Clear();
      if ((UnityEngine.Object) holdableStorable != (UnityEngine.Object) null)
      {
        holdableStorable.HoldSelected(false);
        holdableStorable = null;
      }
      base.OnDisable();
    }

    private bool RepairController(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || !(type == GameActionType.Submit & down))
        return false;
      repairButton.onClick?.Invoke();
      HideInfoWindow();
      return true;
    }

    protected override bool ConsoleController(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || !down)
        return false;
      if ((UnityEngine.Object) holdableStorable != (UnityEngine.Object) null)
      {
        holdableStorable.HoldSelected(false);
        holdableStorable = null;
      }
      if (type == GameActionType.LStickLeft & down)
      {
        ExecuteEvents.Execute<ISubmitHandler>(SelectorButtons[0].gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        OnInvalidate();
        return true;
      }
      if (!(type == GameActionType.LStickRight & down))
        return false;
      ExecuteEvents.Execute<ISubmitHandler>(SelectorButtons[1].gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
      OnInvalidate();
      return true;
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (RepairingVisible)
        _repairingHint.SetActive(joystick);
      controlPanel.SetActive(itemSelector.gameObject.activeInHierarchy & joystick);
      if (!((UnityEngine.Object) holdableStorable != (UnityEngine.Object) null))
        return;
      holdableStorable.HoldSelected(joystick);
    }

    protected override void OnInvalidate()
    {
      base.OnInvalidate();
      RedrawCost();
      if (targetRepairable == null)
      {
        noNeedForRepairMessage.Visible = false;
        notEnoughResourcesMessage.Visible = false;
        HideableViewUtility.SetVisible(repairButton.gameObject, false);
        _repairingHint.SetActive(false);
        RepairingVisible = false;
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, RepairController);
      }
      else
      {
        bool flag1 = !NeedRepair();
        noNeedForRepairMessage.Visible = flag1;
        if (flag1)
        {
          notEnoughResourcesMessage.Visible = false;
          HideableViewUtility.SetVisible(repairButton.gameObject, false);
          _repairingHint.SetActive(false);
          RepairingVisible = false;
          ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, RepairController);
        }
        else
        {
          bool flag2 = !HaveEnoughResources();
          notEnoughResourcesMessage.Visible = flag2;
          RepairingVisible = !flag2;
          HideableViewUtility.SetVisible(repairButton.gameObject, RepairingVisible);
          _repairingHint.SetActive(RepairingVisible);
          if (RepairingVisible)
            ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, RepairController);
          else
            ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, RepairController);
        }
      }
      if (!InputService.Instance.JoystickUsed)
        return;
      HideInfoWindow();
    }

    protected override void OnItemClick(IStorableComponent storable)
    {
      if (ItemIsSelected(storable))
        return;
      itemSelector.Item = storable;
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
      TargetRepairable = newIngredient != null ? newIngredient.GetComponent<RepairableComponent>() : null;
    }

    private void RedrawCost()
    {
      if ((UnityEngine.Object) costView == (UnityEngine.Object) null)
        return;
      RepairableLevel repairableLevel = targetRepairable?.TargetLevel();
      if (repairableLevel != null)
      {
        costView.Actor = Actor;
        costView.Cost = repairableLevel.Сost;
      }
      else
      {
        costView.Actor = null;
        costView.Cost = null;
      }
    }

    private void Repair()
    {
      if (!HaveEnoughResources())
        return;
      RepairableLevel repairableLevel = targetRepairable.TargetLevel();
      foreach (RepairableCostItem repairableCostItem in repairableLevel.Сost)
      {
        IEntity removingItem = repairableCostItem.Template.Value;
        if (removingItem != null)
          RemoveItemsAmount(Actor, removingItem, repairableCostItem.Count);
      }
      DurabilityParameter.Value = repairableLevel.MaxDurability;
      PlayAudio(targetRepairable.Settings?.RepairSound);
      if (!targetIsHydrant)
        return;
      Actor.GetComponent<PlayerControllerComponent>().ComputeRepairHydrant();
    }

    private bool SelectorItemValid(ItemSelector itemSelector, IStorableComponent item)
    {
      return item.GetComponent<RepairableComponent>() != null;
    }

    public void OnParameterChanged(IParameter parameter) => OnInvalidate();
  }
}
