using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Behaviours.Localization;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Windows;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using Engine.Source.Inventory;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using Engine.Source.UI.Controls;
using Engine.Source.UI.Controls.BoolViews;
using InputServices;
using Inspectors;
using Pingle;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class AutopsyWindow : BaseInventoryWindow<AutopsyWindow>, IAutopsyWindow, IWindow
  {
    [SerializeField]
    private List<UIControl> items = new List<UIControl>();
    [SerializeField]
    private ContainerTarget[] containerTargets = new ContainerTarget[0];
    [SerializeField]
    private IEntitySerializable bloodEntityContainerTemplate;
    [SerializeField]
    private IEntitySerializable bottleEntityContainerTemplate;
    [SerializeField]
    private ItemView bottleItemView;
    [SerializeField]
    private HoldableButton2 buttonDrain;
    [SerializeField]
    private GameObject noBloodMessage;
    [SerializeField]
    private GameObject noBottleMessage;
    [SerializeField]
    private ItemView toolItemView;
    [SerializeField]
    private ItemSelector toolSelector;
    [SerializeField]
    private GameObject tooltipArea;
    [SerializeField]
    private GameObject instrumentDamageItem;
    [SerializeField]
    private Text instrumentDamageDescriptionText;
    [SerializeField]
    private Text instrumentDamageValueText;
    [SerializeField]
    private GameObject organDamageItem;
    [SerializeField]
    private Text organDamageDescriptionText;
    [SerializeField]
    private Text organDamageValueText;
    [SerializeField]
    private Text instrumentTitleText;
    [SerializeField]
    private Text instrumentErrorText;
    [SerializeField]
    private AudioClip bloodOpenProgressAudio;
    [SerializeField]
    private AudioClip bloodOpenCompleteAudio;
    [SerializeField]
    private EventView organDamagedNotification;
    private IStorageComponent itemStorage;
    private IInventoryComponent selectedContainer;
    [SerializeField]
    private GameObject drainConsoleButton;
    private bool wasDrained = false;
    private Modes _currentMode = Modes.None;
    private Modes prevMode = Modes.None;
    private List<Button> SelectorButtons;
    private bool drainStarted = false;
    [SerializeField]
    private List<BodyPartSelectable> organs;
    private int currentOrganIndex = -1;
    protected StorableUI holdSelectedStorable;

    [Inspected]
    public IStorageComponent Target { get; set; }

    protected override bool ValidateContainer(
      IInventoryComponent container,
      IStorageComponent storage)
    {
      return base.ValidateContainer(container, storage) && (container.GetGroup() == InventoryGroup.Autopsy || storage == Actor);
    }

    private bool ItemIsOrgan(IStorableComponent storable)
    {
      return storable.Storage == Target && storable.Container.GetGroup() == InventoryGroup.Autopsy && !storable.Container.GetLimitations().Contains(StorableGroup.Autopsy_Instrument_Bottle);
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      Unsubscribe();
      CanShowInfoWindows = false;
      if (selectedStorable != null)
      {
        HideInfoWindow();
        selectedStorable.SetSelected(false);
        selectedStorable = null;
      }
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickLeft, SwapScalpels);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickRight, SwapScalpels);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, DrainListener);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      actors.Clear();
      actors.Add(Actor);
      Build2();
      actors.Add(Target);
      CreateContainers();
      UpdateViews();
      toolSelector.ChangeItemEvent += OnSelectorItemChange;
      CurrentMode = Modes.Autopsy;
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Autopsy, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickLeft, SwapScalpels);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickRight, SwapScalpels);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, NavigateOrgans);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, NavigateOrgans);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, NavigateOrgans);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, NavigateOrgans);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, DrainListener);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, SelectFromInventory);
      foreach (ContainerTarget containerTarget in containerTargets)
        containerTarget.View.Container = null;
      toolSelector.Storage = null;
      bottleItemView.Storable = null;
      HideableViewUtility.SetVisible(buttonDrain.gameObject, false);
      HideableViewUtility.SetVisible(bottleItemView.gameObject, false);
      HideableViewUtility.SetVisible(noBloodMessage.gameObject, false);
      HideableViewUtility.SetVisible(noBottleMessage.gameObject, false);
      toolSelector.ChangeItemEvent -= OnSelectorItemChange;
      CurrentMode = Modes.None;
      if (holdSelectedStorable != null)
      {
        holdSelectedStorable.HoldSelected(false);
        holdSelectedStorable = null;
      }
      base.OnDisable();
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
            AutopsyWindowUnsubscribe();
            foreach (BodyPartSelectable organ in organs)
              organ.SetSelected(false);
            if (holdSelectedStorable != null)
              holdSelectedStorable.HoldSelected(false);
            currentOrganIndex = -1;
            break;
          case Modes.Inventory:
            return;
          case Modes.Autopsy:
            if (_currentMode == Modes.Inventory || _currentMode == Modes.None)
            {
              UnsubscribeNavigation();
              AutopsyWindowSubscribe();
              if (selectedStorable != null)
              {
                selectedStorable.SetSelected(false);
                selectedStorable = null;
              }
              selectedStorable = GetStorableByComponent(toolSelector.Item);
              if (selectedStorable != null)
              {
                selectedStorable.SetSelected(true);
                holdSelectedStorable = selectedStorable;
              }
              else
                HideInfoWindow();
              if (currentOrganIndex == -1)
                currentOrganIndex = organs.Count - 1;
            }
            break;
        }
        _currentMode = value;
      }
    }

    private bool SwapScalpels(GameActionType type, bool down)
    {
      if (toolSelector.Item == null || !down)
        return false;
      if (type == GameActionType.RStickLeft)
      {
        ExecuteEvents.Execute(toolSelector.previousButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        OnInvalidate();
        HideInfoWindow();
        return true;
      }
      if (!(type == GameActionType.RStickRight & down))
        return false;
      ExecuteEvents.Execute(toolSelector.nextButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
      OnInvalidate();
      HideInfoWindow();
      return true;
    }

    private bool DrainListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.Context & down)
      {
        buttonDrain.GamepadStartHold();
        return true;
      }
      if (type != GameActionType.Context || down)
        return false;
      buttonDrain.GamepadEndHold();
      return true;
    }

    protected void AutopsyWindowSubscribe()
    {
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickLeft, NavigateOrgans);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickRight, NavigateOrgans);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, NavigateOrgans);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, NavigateOrgans);
    }

    protected void AutopsyWindowUnsubscribe()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, NavigateOrgans);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, NavigateOrgans);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, NavigateOrgans);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, NavigateOrgans);
    }

    private bool NavigateOrgans(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || toolSelector.Item == null && organs.Count(organ => organ.OrganRemoved) == 0 || !down)
        return false;
      Vector2 dirrection = Vector2.right;
      Vector3 position = organs[currentOrganIndex].transform.position;
      switch (type)
      {
        case GameActionType.LStickUp:
          dirrection = Vector2.up;
          break;
        case GameActionType.LStickDown:
          dirrection = Vector2.down;
          break;
        case GameActionType.LStickLeft:
          dirrection = Vector2.left;
          break;
        case GameActionType.LStickRight:
          dirrection = Vector2.right;
          break;
      }
      GameObject gameObject = toolSelector.Item == null ? UISelectableHelper.Select(organs.Where(organ => organ.OrganRemoved && !organ.OrganTaken).Select(organ => organ.gameObject), position, dirrection, false) : UISelectableHelper.Select(organs.Select(organ => organ.gameObject), position, dirrection, false);
      if (gameObject != null)
      {
        if (currentOrganIndex != -1 && organs[currentOrganIndex] != null)
        {
          organs[currentOrganIndex].SetSelected(false);
          organs[currentOrganIndex].FireConsoleOnExitEvent();
        }
        currentOrganIndex = organs.IndexOf(gameObject.GetComponent<BodyPartSelectable>());
        organs[currentOrganIndex].SetSelected(true);
        organs[currentOrganIndex].FireConsoleOnEnterEvent();
      }
      return true;
    }

    private bool SelectFromInventory(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || !down)
        return false;
      SelectItem((StorableComponent) selectedStorable.Internal);
      return true;
    }

    private void OnSelectorItemChange(
      ItemSelector arg1,
      IStorableComponent arg2,
      IStorableComponent arg3)
    {
      if (holdSelectedStorable != null)
      {
        holdSelectedStorable.HoldSelected(false);
        holdSelectedStorable = null;
      }
      if (arg3 != null)
      {
        SetStorableByComponent(arg3);
        StorableUI storableByComponent = GetStorableByComponent(arg3);
        if (storableByComponent != null)
        {
          storableByComponent.HoldSelected(true);
          holdSelectedStorable = storableByComponent;
        }
      }
      HideInfoWindow();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (joystick)
      {
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Autopsy, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        instrumentDamageItem.transform.localPosition = new Vector3(instrumentDamageItem.transform.localPosition.x, -83f);
        instrumentTitleText.transform.localPosition = new Vector3(instrumentTitleText.transform.localPosition.x, -115.5f);
        instrumentErrorText.transform.localPosition = new Vector3(instrumentErrorText.transform.localPosition.x, -285f);
        organDamageItem.transform.parent.localPosition = new Vector3(organDamageItem.transform.parent.localPosition.x, -325f);
        buttonDrain.transform.localPosition = new Vector3(300f, buttonDrain.transform.localPosition.y);
      }
      else
      {
        buttonDrain.GamepadEndHold();
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Autopsy, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        instrumentDamageItem.transform.localPosition = new Vector3(instrumentDamageItem.transform.localPosition.x, -485.5f);
        instrumentTitleText.transform.localPosition = new Vector3(instrumentTitleText.transform.localPosition.x, -453f);
        instrumentErrorText.transform.localPosition = new Vector3(instrumentErrorText.transform.localPosition.x, -453f);
        organDamageItem.transform.parent.localPosition = new Vector3(organDamageItem.transform.parent.localPosition.x, -510f);
        buttonDrain.transform.localPosition = new Vector3(275f, buttonDrain.transform.localPosition.y);
      }
      if (holdSelectedStorable != null)
        holdSelectedStorable.HoldSelected(joystick);
      if (toolSelector.Item != null || organs.Count(organ => organ.OrganRemoved) > 0)
      {
        if (currentOrganIndex >= organs.Count || currentOrganIndex < 0)
          currentOrganIndex = organs.Count - 1;
        if (organs[currentOrganIndex] != null)
        {
          organs[currentOrganIndex].SetSelected(joystick);
          if (joystick)
            organs[currentOrganIndex].FireConsoleOnEnterEvent();
          else
            organs[currentOrganIndex].FireConsoleOnExitEvent();
        }
      }
      CheckDrainEnabled();
    }

    protected override void InteractItem(IStorableComponent storable)
    {
      if (Actor.Items.Contains(storable))
        return;
      MoveItem(storable, Actor);
      StorableComponentUtility.PlayTakeSound(storable);
    }

    private void CreateContainers()
    {
      itemStorage = Target;
      foreach (ContainerTarget containerTarget in containerTargets)
      {
        if (!(containerTarget.View == null))
          containerTarget.View.Container = (InventoryComponent) StorageUtility.GetContainerByTemplate(itemStorage, containerTarget.Template.Value);
      }
      toolSelector.Storage = Actor;
      OnInvalidate();
      CheckDrainEnabled();
    }

    private void DestroyContainers()
    {
      itemStorage.Owner.Dispose();
      itemStorage = null;
    }

    private void OnContainerSelect(IInventoryComponent container)
    {
      if (selectedContainer == container)
        return;
      selectedContainer = container;
    }

    private void OnContainerDeselect(IInventoryComponent container)
    {
      if (selectedContainer != container)
        return;
      selectedContainer = null;
    }

    protected override void OnContainerOpenEnd(IInventoryComponent container, bool complete)
    {
      base.OnContainerOpenEnd(container, complete);
      if (complete)
      {
        Actor.GetComponent<PlayerControllerComponent>()?.ComputeAction(ActionEnum.Autopsy, false, Target.Owner);
        CheckOrganDestroy(container);
      }
      UpdateViews();
    }

    private void CheckOrganDestroy(IInventoryComponent container)
    {
      if (!container.GetStorage().Items.ToList().Exists(x => x.Container == container))
        return;
      IStorableComponent storableComponent = container.GetStorage().Items.First(x => x.Container == container);
      float linesVision = GetLinesVision();
      if (Random.value >= (double) Mathf.Max(container.GetDifficulty() - linesVision, 0.0f))
        return;
      if (storableComponent?.Owner != null)
        storableComponent.Owner.Dispose();
      container.Available.Value = false;
      organDamagedNotification.Invoke();
    }

    private float GetLinesVision()
    {
      float linesVision = 0.0f;
      IStorableComponent component1 = toolSelector.Item;
      ParametersComponent component2 = component1 != null ? component1.GetComponent<ParametersComponent>() : null;
      if (component2 != null)
      {
        IParameter<float> byName = component2.GetByName<float>(ParameterNameEnum.LinesVision);
        if (byName != null)
          linesVision = byName.Value;
      }
      return linesVision;
    }

    protected override void ShowClosedContainerInfo(IInventoryComponent container)
    {
      if (toolSelector.Item != null)
      {
        HideableViewUtility.SetVisible(tooltipArea, true);
        instrumentDamageDescriptionText.text = ServiceLocator.GetService<LocalizationService>().GetText("{UI.Menu.Protagonist.Autopsy.InstrumentDamage}");
        instrumentDamageValueText.text = GetInstrumentDamageString(container.GetInstrumentDamage());
        organDamageDescriptionText.text = ServiceLocator.GetService<LocalizationService>().GetText("{UI.Menu.Protagonist.Autopsy.OrganDamageChance}");
        float linesVision = GetLinesVision();
        organDamageValueText.text = GetOrganDamageString(Mathf.Max(container.GetDifficulty() - linesVision, 0.0f));
      }
      else
        HideableViewUtility.SetVisible(tooltipArea, false);
    }

    protected override void HideClosedContainerInfo()
    {
      HideableViewUtility.SetVisible(tooltipArea, false);
    }

    public override void Initialize()
    {
      RegisterLayer<IAutopsyWindow>(this);
      foreach (ContainerTarget containerTarget in containerTargets)
      {
        if (!(containerTarget.View == null))
        {
          containerTarget.View.SelectEvent += OnContainerSelect;
          containerTarget.View.DeselectEvent += OnContainerDeselect;
          containerTarget.View.OpenEndEvent += ((BaseInventoryWindow<AutopsyWindow>) this).OpenEnd;
          containerTarget.View.OpenBeginEvent += ((BaseInventoryWindow<AutopsyWindow>) this).OpenBegin;
          containerTarget.View.ItemInteractEvent += new Action<IStorableComponent>(((BaseInventoryWindow<AutopsyWindow>) this).InteractItem);
        }
      }
      buttonDrain.OpenBeginEvent += DrainBegin;
      buttonDrain.OpenEndEvent += Drain;
      toolSelector.ChangeItemEvent += (x, y, z) => UpdateViews();
      base.Initialize();
    }

    public override Type GetWindowType() => typeof (IAutopsyWindow);

    private void DrainBegin() => StartOpenAudio(bloodOpenProgressAudio);

    private IStorableComponent GetActorsBottle()
    {
      foreach (IStorableComponent actorsBottle in Actor.Items)
      {
        if (actorsBottle.Groups.Contains(StorableGroup.Autopsy_Instrument_Bottle))
          return actorsBottle;
      }
      return null;
    }

    protected override IEntity GetInstrument(StorableGroup instrumentGroup, bool brokenEnabled = false)
    {
      IStorableComponent component = toolSelector.Item;
      if (component == null || !component.Groups.Contains(instrumentGroup))
        return null;
      if (!brokenEnabled)
      {
        IParameter<float> byName = component.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
        if (byName != null && byName.Value <= 0.0)
        {
          OnJoystick(InputService.Instance.JoystickUsed);
          return null;
        }
      }
      return component.Owner;
    }

    private void Drain(bool success)
    {
      StopOpenAudio();
      if (!success)
        return;
      StartOpenAudio(bloodOpenCompleteAudio);
      List<IStorableComponent> ingredients = new List<IStorableComponent>();
      IStorableComponent itemInContainer = GetItemInContainer(StorageUtility.GetContainerByTemplate(itemStorage, bloodEntityContainerTemplate.Value), itemStorage);
      if (itemInContainer != null)
        ingredients.Add(itemInContainer);
      IStorableComponent actorsBottle = GetActorsBottle();
      if (actorsBottle != null)
        ingredients.Add(actorsBottle);
      IStorableComponent craftResult = CraftHelper.GetCraftResult(ingredients, out CraftRecipe _);
      if (craftResult != null)
      {
        foreach (IStorableComponent storable in ingredients)
        {
          if (storable.Count > 1)
          {
            --storable.Count;
          }
          else
          {
            storable.Owner.Dispose();
            RemoveItemFromView(storable);
          }
        }
        IEntity entity = ServiceLocator.GetService<IFactory>().Instantiate(craftResult.Owner);
        ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
        Actor.AddItemOrDrop(entity.GetComponent<IStorableComponent>(), null);
      }
      CheckDrainEnabled();
    }

    protected override void DragEnd(Intersect intersect)
    {
      base.DragEnd(intersect);
      CheckDrainEnabled();
    }

    private void CheckDrainEnabled()
    {
      IInventoryComponent containerByTemplate = StorageUtility.GetContainerByTemplate(itemStorage, bloodEntityContainerTemplate.Value);
      IStorableComponent itemInContainer = containerByTemplate != null ? GetItemInContainer(containerByTemplate, itemStorage) : null;
      IStorableComponent actorsBottle = GetActorsBottle();
      bottleItemView.Storable = (StorableComponent) actorsBottle;
      HideableViewUtility.SetVisible(bottleItemView.gameObject, itemInContainer != null);
      HideableViewUtility.SetVisible(noBottleMessage, itemInContainer != null && actorsBottle == null);
      HideableViewUtility.SetVisible(buttonDrain.gameObject, itemInContainer != null && actorsBottle != null);
      HideableViewUtility.SetVisible(noBloodMessage, itemInContainer == null);
      if (InputService.Instance.JoystickUsed)
        drainConsoleButton.SetActive(itemInContainer != null && actorsBottle != null);
      else
        drainConsoleButton.SetActive(false);
    }

    protected override void Update()
    {
      base.Update();
      if (selectedContainer == null)
        return;
      ShowClosedContainerInfo(selectedContainer);
    }

    protected override void UpdateContainerStates()
    {
      base.UpdateContainerStates();
      foreach (KeyValuePair<IStorableComponent, StorableUI> storable in storables)
      {
        if (storable.Key != null && storable.Value != null)
          storable.Value.ShowCount(!ItemIsOrgan(storable.Key));
      }
      foreach (ContainerTarget containerTarget in containerTargets)
      {
        if (!(containerTarget.View == null))
        {
          InventoryComponent container = containerTarget.View.Container;
          if (container == null)
            containerTarget.View.SetCanOpen(false);
          else
            containerTarget.View.SetCanOpen(CanOpenContainer(container));
        }
      }
      UpdateViews();
    }

    private void UpdateViews()
    {
      IStorableComponent storableComponent = toolSelector.Item;
      if (storableComponent != null)
      {
        LocalizationService service = ServiceLocator.GetService<LocalizationService>();
        instrumentTitleText.GetComponent<Localizer>().Signature = service.GetText(storableComponent.Title);
        HideableViewUtility.SetVisible(instrumentTitleText.gameObject, true);
        HideableViewUtility.SetVisible(instrumentErrorText.gameObject, false);
      }
      else
      {
        instrumentErrorText.GetComponent<Localizer>().Signature = "{UI.Menu.Protagonist.Autopsy.NoInstrument}";
        HideableViewUtility.SetVisible(instrumentErrorText.gameObject, true);
        HideableViewUtility.SetVisible(instrumentTitleText.gameObject, false);
      }
      CheckInterestingItems();
    }

    private string GetInstrumentDamageString(float damage)
    {
      string tag = damage < 0.2 ? (damage < 0.15 ? "{UI.Menu.Protagonist.Autopsy.LowDamage}" : "{UI.Menu.Protagonist.Autopsy.AverageDamage}") : "{UI.Menu.Protagonist.Autopsy.HighDamage}";
      return ServiceLocator.GetService<LocalizationService>().GetText(tag);
    }

    private string GetOrganDamageString(float damage)
    {
      string tag = damage < 0.5 ? (damage < 0.25 ? "{UI.Menu.Protagonist.Autopsy.LowChance}" : "{UI.Menu.Protagonist.Autopsy.AverageChance}") : "{UI.Menu.Protagonist.Autopsy.HighChance}";
      return ServiceLocator.GetService<LocalizationService>().GetText(tag);
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
        StorableComponent storable = intersect.Storables.FirstOrDefault();
        if (storable == null)
          return;
        SelectItem(storable);
      }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }

    private void SelectItem(StorableComponent storable)
    {
      if (toolSelector.Item == storable || !storable.Groups.Contains(StorableGroup.Autopsy_Instrument_Stomach))
        return;
      IParameter<float> byName = storable.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
      if (byName != null && byName.Value <= 0.0)
        return;
      toolSelector.Item = storable;
    }

    private void CheckInterestingItems()
    {
      foreach (IStorableComponent key in storables.Keys)
      {
        StorableUI storable = storables[key];
        storable.Enable(ItemIsInteresting(key));
        storable.SetSelected(key == toolSelector.Item);
      }
    }

    protected override bool ItemIsInteresting(IStorableComponent item)
    {
      if (!item.Groups.Contains(StorableGroup.Autopsy_Instrument_Stomach))
        return false;
      IParameter<float> byName = item.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
      return byName == null || byName.Value > 0.0;
    }

    protected override void AddActionsToInfoWindow(
      InfoWindowNew window,
      IStorableComponent storable)
    {
      if (!ItemIsInteresting(storable) || CurrentMode != Modes.Inventory)
        return;
      window.AddActionTooltip(GameActionType.Submit, "{StorableTooltip.Select}");
    }

    protected enum Modes
    {
      None,
      Inventory,
      Autopsy,
    }

    [Serializable]
    public struct ContainerTarget
    {
      public ContainerView View;
      public IEntitySerializable Template;
    }
  }
}
