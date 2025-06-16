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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class AutopsyWindow : BaseInventoryWindow<AutopsyWindow>, IAutopsyWindow, IWindow
  {
    [SerializeField]
    private List<UIControl> items = new List<UIControl>();
    [SerializeField]
    private AutopsyWindow.ContainerTarget[] containerTargets = new AutopsyWindow.ContainerTarget[0];
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
    private IInventoryComponent selectedContainer = (IInventoryComponent) null;
    [SerializeField]
    private GameObject drainConsoleButton;
    private bool wasDrained = false;
    private AutopsyWindow.Modes _currentMode = AutopsyWindow.Modes.None;
    private AutopsyWindow.Modes prevMode = AutopsyWindow.Modes.None;
    private List<Button> SelectorButtons;
    private bool drainStarted = false;
    [SerializeField]
    private List<BodyPartSelectable> organs;
    private int currentOrganIndex = -1;
    protected StorableUI holdSelectedStorable = (StorableUI) null;

    [Inspected]
    public IStorageComponent Target { get; set; }

    protected override bool ValidateContainer(
      IInventoryComponent container,
      IStorageComponent storage)
    {
      return base.ValidateContainer(container, storage) && (container.GetGroup() == InventoryGroup.Autopsy || storage == this.Actor);
    }

    private bool ItemIsOrgan(IStorableComponent storable)
    {
      return storable.Storage == this.Target && storable.Container.GetGroup() == InventoryGroup.Autopsy && !storable.Container.GetLimitations().Contains<StorableGroup>(StorableGroup.Autopsy_Instrument_Bottle);
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.Unsubscribe();
      this.CanShowInfoWindows = false;
      if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
      {
        this.HideInfoWindow();
        this.selectedStorable.SetSelected(false);
        this.selectedStorable = (StorableUI) null;
      }
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickLeft, new GameActionHandle(this.SwapScalpels));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickRight, new GameActionHandle(this.SwapScalpels));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, new GameActionHandle(this.DrainListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      this.actors.Clear();
      this.actors.Add(this.Actor);
      this.Build2();
      this.actors.Add(this.Target);
      this.CreateContainers();
      this.UpdateViews();
      this.toolSelector.ChangeItemEvent += new Action<ItemSelector, IStorableComponent, IStorableComponent>(this.OnSelectorItemChange);
      this.CurrentMode = AutopsyWindow.Modes.Autopsy;
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Autopsy, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickLeft, new GameActionHandle(this.SwapScalpels));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickRight, new GameActionHandle(this.SwapScalpels));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, new GameActionHandle(this.NavigateOrgans));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, new GameActionHandle(this.NavigateOrgans));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.NavigateOrgans));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.NavigateOrgans));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, new GameActionHandle(this.DrainListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.SelectFromInventory));
      foreach (AutopsyWindow.ContainerTarget containerTarget in this.containerTargets)
        containerTarget.View.Container = (InventoryComponent) null;
      this.toolSelector.Storage = (IStorageComponent) null;
      this.bottleItemView.Storable = (StorableComponent) null;
      HideableViewUtility.SetVisible(this.buttonDrain.gameObject, false);
      HideableViewUtility.SetVisible(this.bottleItemView.gameObject, false);
      HideableViewUtility.SetVisible(this.noBloodMessage.gameObject, false);
      HideableViewUtility.SetVisible(this.noBottleMessage.gameObject, false);
      this.toolSelector.ChangeItemEvent -= new Action<ItemSelector, IStorableComponent, IStorableComponent>(this.OnSelectorItemChange);
      this.CurrentMode = AutopsyWindow.Modes.None;
      if ((UnityEngine.Object) this.holdSelectedStorable != (UnityEngine.Object) null)
      {
        this.holdSelectedStorable.HoldSelected(false);
        this.holdSelectedStorable = (StorableUI) null;
      }
      base.OnDisable();
    }

    protected AutopsyWindow.Modes PreviousMode
    {
      get => this.prevMode;
      set => this.prevMode = value;
    }

    protected virtual AutopsyWindow.Modes CurrentMode
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
          case AutopsyWindow.Modes.None:
            this.AutopsyWindowUnsubscribe();
            foreach (BodyPartSelectable organ in this.organs)
              organ.SetSelected(false);
            if ((UnityEngine.Object) this.holdSelectedStorable != (UnityEngine.Object) null)
              this.holdSelectedStorable.HoldSelected(false);
            this.currentOrganIndex = -1;
            break;
          case AutopsyWindow.Modes.Inventory:
            return;
          case AutopsyWindow.Modes.Autopsy:
            if (this._currentMode == AutopsyWindow.Modes.Inventory || this._currentMode == AutopsyWindow.Modes.None)
            {
              this.UnsubscribeNavigation();
              this.AutopsyWindowSubscribe();
              if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
              {
                this.selectedStorable.SetSelected(false);
                this.selectedStorable = (StorableUI) null;
              }
              this.selectedStorable = this.GetStorableByComponent(this.toolSelector.Item);
              if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
              {
                this.selectedStorable.SetSelected(true);
                this.holdSelectedStorable = this.selectedStorable;
              }
              else
                this.HideInfoWindow();
              if (this.currentOrganIndex == -1)
                this.currentOrganIndex = this.organs.Count - 1;
              break;
            }
            break;
        }
        this._currentMode = value;
      }
    }

    private bool SwapScalpels(GameActionType type, bool down)
    {
      if (this.toolSelector.Item == null || !down)
        return false;
      if (type == GameActionType.RStickLeft)
      {
        ExecuteEvents.Execute<ISubmitHandler>(this.toolSelector.previousButton.gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        this.OnInvalidate();
        this.HideInfoWindow();
        return true;
      }
      if (!(type == GameActionType.RStickRight & down))
        return false;
      ExecuteEvents.Execute<ISubmitHandler>(this.toolSelector.nextButton.gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
      this.OnInvalidate();
      this.HideInfoWindow();
      return true;
    }

    private bool DrainListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.Context & down)
      {
        this.buttonDrain.GamepadStartHold();
        return true;
      }
      if (type != GameActionType.Context || down)
        return false;
      this.buttonDrain.GamepadEndHold();
      return true;
    }

    protected void AutopsyWindowSubscribe()
    {
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickLeft, new GameActionHandle(this.NavigateOrgans));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickRight, new GameActionHandle(this.NavigateOrgans));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, new GameActionHandle(this.NavigateOrgans));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, new GameActionHandle(this.NavigateOrgans));
    }

    protected void AutopsyWindowUnsubscribe()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, new GameActionHandle(this.NavigateOrgans));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, new GameActionHandle(this.NavigateOrgans));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.NavigateOrgans));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.NavigateOrgans));
    }

    private bool NavigateOrgans(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || this.toolSelector.Item == null && this.organs.Count<BodyPartSelectable>((Func<BodyPartSelectable, bool>) (organ => organ.OrganRemoved)) == 0 || !down)
        return false;
      Vector2 dirrection = Vector2.right;
      Vector3 position = this.organs[this.currentOrganIndex].transform.position;
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
      GameObject gameObject = this.toolSelector.Item == null ? UISelectableHelper.Select(this.organs.Where<BodyPartSelectable>((Func<BodyPartSelectable, bool>) (organ => organ.OrganRemoved && !organ.OrganTaken)).Select<BodyPartSelectable, GameObject>((Func<BodyPartSelectable, GameObject>) (organ => organ.gameObject)), position, (Vector3) dirrection, false) : UISelectableHelper.Select(this.organs.Select<BodyPartSelectable, GameObject>((Func<BodyPartSelectable, GameObject>) (organ => organ.gameObject)), position, (Vector3) dirrection, false);
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
      {
        if (this.currentOrganIndex != -1 && (UnityEngine.Object) this.organs[this.currentOrganIndex] != (UnityEngine.Object) null)
        {
          this.organs[this.currentOrganIndex].SetSelected(false);
          this.organs[this.currentOrganIndex].FireConsoleOnExitEvent();
        }
        this.currentOrganIndex = this.organs.IndexOf(gameObject.GetComponent<BodyPartSelectable>());
        this.organs[this.currentOrganIndex].SetSelected(true);
        this.organs[this.currentOrganIndex].FireConsoleOnEnterEvent();
      }
      return true;
    }

    private bool SelectFromInventory(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || !down)
        return false;
      this.SelectItem((StorableComponent) this.selectedStorable.Internal);
      return true;
    }

    private void OnSelectorItemChange(
      ItemSelector arg1,
      IStorableComponent arg2,
      IStorableComponent arg3)
    {
      if ((UnityEngine.Object) this.holdSelectedStorable != (UnityEngine.Object) null)
      {
        this.holdSelectedStorable.HoldSelected(false);
        this.holdSelectedStorable = (StorableUI) null;
      }
      if (arg3 != null)
      {
        this.SetStorableByComponent(arg3);
        StorableUI storableByComponent = this.GetStorableByComponent(arg3);
        if ((UnityEngine.Object) storableByComponent != (UnityEngine.Object) null)
        {
          storableByComponent.HoldSelected(true);
          this.holdSelectedStorable = storableByComponent;
        }
      }
      this.HideInfoWindow();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (joystick)
      {
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Autopsy, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        this.instrumentDamageItem.transform.localPosition = new Vector3(this.instrumentDamageItem.transform.localPosition.x, -83f);
        this.instrumentTitleText.transform.localPosition = new Vector3(this.instrumentTitleText.transform.localPosition.x, -115.5f);
        this.instrumentErrorText.transform.localPosition = new Vector3(this.instrumentErrorText.transform.localPosition.x, -285f);
        this.organDamageItem.transform.parent.localPosition = new Vector3(this.organDamageItem.transform.parent.localPosition.x, -325f);
        this.buttonDrain.transform.localPosition = new Vector3(300f, this.buttonDrain.transform.localPosition.y);
      }
      else
      {
        this.buttonDrain.GamepadEndHold();
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Autopsy, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        this.instrumentDamageItem.transform.localPosition = new Vector3(this.instrumentDamageItem.transform.localPosition.x, -485.5f);
        this.instrumentTitleText.transform.localPosition = new Vector3(this.instrumentTitleText.transform.localPosition.x, -453f);
        this.instrumentErrorText.transform.localPosition = new Vector3(this.instrumentErrorText.transform.localPosition.x, -453f);
        this.organDamageItem.transform.parent.localPosition = new Vector3(this.organDamageItem.transform.parent.localPosition.x, -510f);
        this.buttonDrain.transform.localPosition = new Vector3(275f, this.buttonDrain.transform.localPosition.y);
      }
      if ((UnityEngine.Object) this.holdSelectedStorable != (UnityEngine.Object) null)
        this.holdSelectedStorable.HoldSelected(joystick);
      if (this.toolSelector.Item != null || this.organs.Count<BodyPartSelectable>((Func<BodyPartSelectable, bool>) (organ => organ.OrganRemoved)) > 0)
      {
        if (this.currentOrganIndex >= this.organs.Count || this.currentOrganIndex < 0)
          this.currentOrganIndex = this.organs.Count - 1;
        if ((UnityEngine.Object) this.organs[this.currentOrganIndex] != (UnityEngine.Object) null)
        {
          this.organs[this.currentOrganIndex].SetSelected(joystick);
          if (joystick)
            this.organs[this.currentOrganIndex].FireConsoleOnEnterEvent();
          else
            this.organs[this.currentOrganIndex].FireConsoleOnExitEvent();
        }
      }
      this.CheckDrainEnabled();
    }

    protected override void InteractItem(IStorableComponent storable)
    {
      if (this.Actor.Items.Contains<IStorableComponent>(storable))
        return;
      this.MoveItem(storable, this.Actor);
      StorableComponentUtility.PlayTakeSound(storable);
    }

    private void CreateContainers()
    {
      this.itemStorage = this.Target;
      foreach (AutopsyWindow.ContainerTarget containerTarget in this.containerTargets)
      {
        if (!((UnityEngine.Object) containerTarget.View == (UnityEngine.Object) null))
          containerTarget.View.Container = (InventoryComponent) StorageUtility.GetContainerByTemplate(this.itemStorage, containerTarget.Template.Value);
      }
      this.toolSelector.Storage = this.Actor;
      this.OnInvalidate();
      this.CheckDrainEnabled();
    }

    private void DestroyContainers()
    {
      this.itemStorage.Owner.Dispose();
      this.itemStorage = (IStorageComponent) null;
    }

    private void OnContainerSelect(IInventoryComponent container)
    {
      if (this.selectedContainer == container)
        return;
      this.selectedContainer = container;
    }

    private void OnContainerDeselect(IInventoryComponent container)
    {
      if (this.selectedContainer != container)
        return;
      this.selectedContainer = (IInventoryComponent) null;
    }

    protected override void OnContainerOpenEnd(IInventoryComponent container, bool complete)
    {
      base.OnContainerOpenEnd(container, complete);
      if (complete)
      {
        this.Actor.GetComponent<PlayerControllerComponent>()?.ComputeAction(ActionEnum.Autopsy, false, this.Target.Owner);
        this.CheckOrganDestroy(container);
      }
      this.UpdateViews();
    }

    private void CheckOrganDestroy(IInventoryComponent container)
    {
      if (!container.GetStorage().Items.ToList<IStorableComponent>().Exists((Predicate<IStorableComponent>) (x => x.Container == container)))
        return;
      IStorableComponent storableComponent = container.GetStorage().Items.First<IStorableComponent>((Func<IStorableComponent, bool>) (x => x.Container == container));
      float linesVision = this.GetLinesVision();
      if ((double) UnityEngine.Random.value >= (double) Mathf.Max(container.GetDifficulty() - linesVision, 0.0f))
        return;
      if (storableComponent?.Owner != null)
        storableComponent.Owner.Dispose();
      container.Available.Value = false;
      this.organDamagedNotification.Invoke();
    }

    private float GetLinesVision()
    {
      float linesVision = 0.0f;
      IStorableComponent component1 = this.toolSelector.Item;
      ParametersComponent component2 = component1 != null ? component1.GetComponent<ParametersComponent>() : (ParametersComponent) null;
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
      if (this.toolSelector.Item != null)
      {
        HideableViewUtility.SetVisible(this.tooltipArea, true);
        this.instrumentDamageDescriptionText.text = ServiceLocator.GetService<LocalizationService>().GetText("{UI.Menu.Protagonist.Autopsy.InstrumentDamage}");
        this.instrumentDamageValueText.text = this.GetInstrumentDamageString(container.GetInstrumentDamage());
        this.organDamageDescriptionText.text = ServiceLocator.GetService<LocalizationService>().GetText("{UI.Menu.Protagonist.Autopsy.OrganDamageChance}");
        float linesVision = this.GetLinesVision();
        this.organDamageValueText.text = this.GetOrganDamageString(Mathf.Max(container.GetDifficulty() - linesVision, 0.0f));
      }
      else
        HideableViewUtility.SetVisible(this.tooltipArea, false);
    }

    protected override void HideClosedContainerInfo()
    {
      HideableViewUtility.SetVisible(this.tooltipArea, false);
    }

    public override void Initialize()
    {
      this.RegisterLayer<IAutopsyWindow>((IAutopsyWindow) this);
      foreach (AutopsyWindow.ContainerTarget containerTarget in this.containerTargets)
      {
        if (!((UnityEngine.Object) containerTarget.View == (UnityEngine.Object) null))
        {
          containerTarget.View.SelectEvent += new Action<IInventoryComponent>(this.OnContainerSelect);
          containerTarget.View.DeselectEvent += new Action<IInventoryComponent>(this.OnContainerDeselect);
          containerTarget.View.OpenEndEvent += new Action<IInventoryComponent, bool>(((BaseInventoryWindow<AutopsyWindow>) this).OpenEnd);
          containerTarget.View.OpenBeginEvent += new Action<IInventoryComponent>(((BaseInventoryWindow<AutopsyWindow>) this).OpenBegin);
          containerTarget.View.ItemInteractEvent += new Action<IStorableComponent>(((BaseInventoryWindow<AutopsyWindow>) this).InteractItem);
        }
      }
      this.buttonDrain.OpenBeginEvent += new Action(this.DrainBegin);
      this.buttonDrain.OpenEndEvent += new Action<bool>(this.Drain);
      this.toolSelector.ChangeItemEvent += (Action<ItemSelector, IStorableComponent, IStorableComponent>) ((x, y, z) => this.UpdateViews());
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (IAutopsyWindow);

    private void DrainBegin() => this.StartOpenAudio(this.bloodOpenProgressAudio);

    private IStorableComponent GetActorsBottle()
    {
      foreach (IStorableComponent actorsBottle in this.Actor.Items)
      {
        if (actorsBottle.Groups.Contains<StorableGroup>(StorableGroup.Autopsy_Instrument_Bottle))
          return actorsBottle;
      }
      return (IStorableComponent) null;
    }

    protected override IEntity GetInstrument(StorableGroup instrumentGroup, bool brokenEnabled = false)
    {
      IStorableComponent component = this.toolSelector.Item;
      if (component == null || !component.Groups.Contains<StorableGroup>(instrumentGroup))
        return (IEntity) null;
      if (!brokenEnabled)
      {
        IParameter<float> byName = component.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
        if (byName != null && (double) byName.Value <= 0.0)
        {
          this.OnJoystick(InputService.Instance.JoystickUsed);
          return (IEntity) null;
        }
      }
      return component.Owner;
    }

    private void Drain(bool success)
    {
      this.StopOpenAudio();
      if (!success)
        return;
      this.StartOpenAudio(this.bloodOpenCompleteAudio);
      List<IStorableComponent> ingredients = new List<IStorableComponent>();
      IStorableComponent itemInContainer = this.GetItemInContainer(StorageUtility.GetContainerByTemplate(this.itemStorage, this.bloodEntityContainerTemplate.Value), this.itemStorage);
      if (itemInContainer != null)
        ingredients.Add(itemInContainer);
      IStorableComponent actorsBottle = this.GetActorsBottle();
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
            this.RemoveItemFromView(storable);
          }
        }
        IEntity entity = ServiceLocator.GetService<IFactory>().Instantiate<IEntity>(craftResult.Owner);
        ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
        this.Actor.AddItemOrDrop(entity.GetComponent<IStorableComponent>(), (IInventoryComponent) null);
      }
      this.CheckDrainEnabled();
    }

    protected override void DragEnd(Intersect intersect)
    {
      base.DragEnd(intersect);
      this.CheckDrainEnabled();
    }

    private void CheckDrainEnabled()
    {
      IInventoryComponent containerByTemplate = StorageUtility.GetContainerByTemplate(this.itemStorage, this.bloodEntityContainerTemplate.Value);
      IStorableComponent itemInContainer = containerByTemplate != null ? this.GetItemInContainer(containerByTemplate, this.itemStorage) : (IStorableComponent) null;
      IStorableComponent actorsBottle = this.GetActorsBottle();
      this.bottleItemView.Storable = (StorableComponent) actorsBottle;
      HideableViewUtility.SetVisible(this.bottleItemView.gameObject, itemInContainer != null);
      HideableViewUtility.SetVisible(this.noBottleMessage, itemInContainer != null && actorsBottle == null);
      HideableViewUtility.SetVisible(this.buttonDrain.gameObject, itemInContainer != null && actorsBottle != null);
      HideableViewUtility.SetVisible(this.noBloodMessage, itemInContainer == null);
      if (InputService.Instance.JoystickUsed)
        this.drainConsoleButton.SetActive(itemInContainer != null && actorsBottle != null);
      else
        this.drainConsoleButton.SetActive(false);
    }

    protected override void Update()
    {
      base.Update();
      if (this.selectedContainer == null)
        return;
      this.ShowClosedContainerInfo(this.selectedContainer);
    }

    protected override void UpdateContainerStates()
    {
      base.UpdateContainerStates();
      foreach (KeyValuePair<IStorableComponent, StorableUI> storable in this.storables)
      {
        if (storable.Key != null && (UnityEngine.Object) storable.Value != (UnityEngine.Object) null)
          storable.Value.ShowCount(!this.ItemIsOrgan(storable.Key));
      }
      foreach (AutopsyWindow.ContainerTarget containerTarget in this.containerTargets)
      {
        if (!((UnityEngine.Object) containerTarget.View == (UnityEngine.Object) null))
        {
          InventoryComponent container = containerTarget.View.Container;
          if (container == null)
            containerTarget.View.SetCanOpen(false);
          else
            containerTarget.View.SetCanOpen(this.CanOpenContainer((IInventoryComponent) container));
        }
      }
      this.UpdateViews();
    }

    private void UpdateViews()
    {
      IStorableComponent storableComponent = this.toolSelector.Item;
      if (storableComponent != null)
      {
        LocalizationService service = ServiceLocator.GetService<LocalizationService>();
        this.instrumentTitleText.GetComponent<Localizer>().Signature = service.GetText(storableComponent.Title);
        HideableViewUtility.SetVisible(this.instrumentTitleText.gameObject, true);
        HideableViewUtility.SetVisible(this.instrumentErrorText.gameObject, false);
      }
      else
      {
        this.instrumentErrorText.GetComponent<Localizer>().Signature = "{UI.Menu.Protagonist.Autopsy.NoInstrument}";
        HideableViewUtility.SetVisible(this.instrumentErrorText.gameObject, true);
        HideableViewUtility.SetVisible(this.instrumentTitleText.gameObject, false);
      }
      this.CheckInterestingItems();
    }

    private string GetInstrumentDamageString(float damage)
    {
      string tag = (double) damage < 0.2 ? ((double) damage < 0.15 ? "{UI.Menu.Protagonist.Autopsy.LowDamage}" : "{UI.Menu.Protagonist.Autopsy.AverageDamage}") : "{UI.Menu.Protagonist.Autopsy.HighDamage}";
      return ServiceLocator.GetService<LocalizationService>().GetText(tag);
    }

    private string GetOrganDamageString(float damage)
    {
      string tag = (double) damage < 0.5 ? ((double) damage < 0.25 ? "{UI.Menu.Protagonist.Autopsy.LowChance}" : "{UI.Menu.Protagonist.Autopsy.AverageChance}") : "{UI.Menu.Protagonist.Autopsy.HighChance}";
      return ServiceLocator.GetService<LocalizationService>().GetText(tag);
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
        StorableComponent storable = this.intersect.Storables.FirstOrDefault<StorableComponent>();
        if (storable == null)
          return;
        this.SelectItem(storable);
      }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }

    private void SelectItem(StorableComponent storable)
    {
      if (this.toolSelector.Item == storable || !storable.Groups.Contains<StorableGroup>(StorableGroup.Autopsy_Instrument_Stomach))
        return;
      IParameter<float> byName = storable.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
      if (byName != null && (double) byName.Value <= 0.0)
        return;
      this.toolSelector.Item = (IStorableComponent) storable;
    }

    private void CheckInterestingItems()
    {
      foreach (IStorableComponent key in this.storables.Keys)
      {
        StorableUI storable = this.storables[key];
        storable.Enable(this.ItemIsInteresting(key));
        storable.SetSelected(key == this.toolSelector.Item);
      }
    }

    protected override bool ItemIsInteresting(IStorableComponent item)
    {
      if (!item.Groups.Contains<StorableGroup>(StorableGroup.Autopsy_Instrument_Stomach))
        return false;
      IParameter<float> byName = item.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
      return byName == null || (double) byName.Value > 0.0;
    }

    protected override void AddActionsToInfoWindow(
      InfoWindowNew window,
      IStorableComponent storable)
    {
      if (!this.ItemIsInteresting(storable) || this.CurrentMode != AutopsyWindow.Modes.Inventory)
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
