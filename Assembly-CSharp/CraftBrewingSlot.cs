using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Services.Inputs;
using InputServices;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftBrewingSlot : MonoBehaviour
{
  [SerializeField]
  private EntityView targetView;
  [SerializeField]
  private ProgressHideable durabilityRangeCheck;
  [SerializeField]
  private Button button;
  [SerializeField]
  private HideableView interactableView;
  [SerializeField]
  private ContainerView containerView;
  [SerializeField]
  private StringView timeText;
  [SerializeField]
  private GameObject brewConsoleTooltip;
  [SerializeField]
  private GameObject takeConsoleTooltip;
  [SerializeField]
  private Text brewingTextObject;
  private string brewingTextBuffer;
  [SerializeField]
  private GameObject selectionFrame;
  private PointerEventData pointerData;
  private GraphicRaycaster raycaster;
  private bool _isSelected = false;
  private IStorableComponent craftedItem = (IStorableComponent) null;
  private bool _IsItemCrafted = false;
  private bool _CanTakeCraft = false;
  private ItemCraftTimeView craftTime = (ItemCraftTimeView) null;

  public event Action<IInventoryComponent> CraftEvent;

  public event Action<IStorableComponent> TakeEvent;

  public event CraftBrewingSlot.OnCraft OnCraftPerformed;

  public event CraftBrewingSlot.OnCraft OnItemTaken;

  private void FireCraftEvent()
  {
    CraftBrewingSlot.OnCraft onCraftPerformed = this.OnCraftPerformed;
    if (onCraftPerformed != null)
      onCraftPerformed(this);
    this.IsItemCrafted = true;
    Action<IInventoryComponent> craftEvent = this.CraftEvent;
    if (craftEvent == null)
      return;
    craftEvent((IInventoryComponent) this.containerView.Container);
  }

  private void FireTakeEvent(IStorableComponent item)
  {
    Action<IStorableComponent> takeEvent = this.TakeEvent;
    if (takeEvent != null)
      takeEvent(item);
    CraftBrewingSlot.OnCraft onItemTaken = this.OnItemTaken;
    if (onItemTaken != null)
      onItemTaken(this);
    this.IsItemCrafted = false;
    this.CanTakeCraft = false;
    this.SetSelected(false);
  }

  public void Initialize(float durabilityThreshold)
  {
    this.durabilityRangeCheck.HiddenRange = new Vector2(0.0f, durabilityThreshold);
    this.containerView.ItemInteractEvent += new Action<IStorableComponent>(this.FireTakeEvent);
    this.button.onClick.AddListener(new UnityAction(this.FireCraftEvent));
    this.IsItemCrafted = false;
    this.SetSelected(false);
    this.craftTime = this.GetComponentInChildren<ItemCraftTimeView>();
    if (!((UnityEngine.Object) this.craftTime != (UnityEngine.Object) null))
      return;
    this.craftTime.OnItemReady += new Action(this.OnItemReady);
  }

  public void SetTarget(IEntity target, IInventoryComponent container)
  {
    this.containerView.Container = (InventoryComponent) container;
    this.targetView.Value = target;
  }

  public void SetEnabled(bool value)
  {
    this.button.interactable = value;
    this.interactableView.Visible = value;
    this.IsEnabled = value;
    if (!this.IsEnabled)
    {
      this.SetSelected(false);
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
    }
    else
    {
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
      this.brewingTextBuffer = this.brewingTextObject.text;
      if ((UnityEngine.Object) this.raycaster == (UnityEngine.Object) null)
      {
        this.raycaster = this.GetComponentInParent<GraphicRaycaster>();
        this.pointerData = new PointerEventData(EventSystem.current)
        {
          position = (Vector2) this.button.transform.position
        };
      }
    }
  }

  public void SetCraftTime(string text) => this.timeText.StringValue = text;

  private void OnItemReady()
  {
    this.CanTakeCraft = true;
    if (!this.IsSelected)
      return;
    this.takeConsoleTooltip.SetActive(true);
  }

  public bool IsVisible => this.GetComponent<HideableCouple>().Visible;

  public bool IsSelected
  {
    get => this._isSelected;
    private set
    {
      if (value == this._isSelected)
        return;
      if (value)
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.CraftTakeListener));
      else
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.CraftTakeListener));
      this._isSelected = value;
    }
  }

  public bool IsItemCrafted
  {
    get
    {
      if (!this._IsItemCrafted)
      {
        SwitchingItemView componentInChildren = this.GetComponentInChildren<SwitchingItemView>();
        if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
          this.IsItemCrafted = componentInChildren.Storable != null;
      }
      return this._IsItemCrafted;
    }
    private set => this._IsItemCrafted = value;
  }

  private bool CraftTakeListener(GameActionType type, bool down)
  {
    if (!InputService.Instance.JoystickUsed || this.IsItemCrafted && !this.CanTakeCraft || !down)
      return false;
    List<RaycastResult> source = new List<RaycastResult>();
    if (this.pointerData == null || (UnityEngine.Object) this.raycaster == (UnityEngine.Object) null)
    {
      this.raycaster = this.GetComponentInParent<GraphicRaycaster>();
      this.pointerData = new PointerEventData(EventSystem.current)
      {
        position = (Vector2) this.button.transform.position
      };
    }
    if (this.pointerData == null)
      return false;
    this.raycaster.Raycast(this.pointerData, source);
    if (source.Count != 0)
    {
      GameObject gameObject = source.First<RaycastResult>().gameObject;
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
      {
        this.pointerData = new PointerEventData(EventSystem.current)
        {
          position = (Vector2) this.button.transform.position
        };
        this.EmulateClickOnConsole(source.First<RaycastResult>(), gameObject);
        return true;
      }
    }
    return false;
  }

  private void EmulateClickOnConsole(RaycastResult raycastResult, GameObject currentOverGo)
  {
    this.pointerData.eligibleForClick = true;
    this.pointerData.delta = Vector2.zero;
    this.pointerData.dragging = false;
    this.pointerData.useDragThreshold = true;
    this.pointerData.pressPosition = this.pointerData.position;
    this.pointerData.pointerCurrentRaycast = raycastResult;
    this.pointerData.pointerPressRaycast = this.pointerData.pointerCurrentRaycast;
    GameObject gameObject = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(currentOverGo, (BaseEventData) this.pointerData, ExecuteEvents.pointerDownHandler);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      gameObject = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
    this.pointerData.clickCount = 1;
    this.pointerData.pointerPress = gameObject;
    this.pointerData.rawPointerPress = currentOverGo;
    ExecuteEvents.Execute<IPointerUpHandler>(this.pointerData.pointerPress, (BaseEventData) this.pointerData, ExecuteEvents.pointerUpHandler);
    if (!((UnityEngine.Object) this.pointerData.pointerPress == (UnityEngine.Object) ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo)) || !this.pointerData.eligibleForClick)
      return;
    ExecuteEvents.Execute<IPointerClickHandler>(this.pointerData.pointerPress, (BaseEventData) this.pointerData, ExecuteEvents.pointerClickHandler);
  }

  public bool IsEnabled { get; set; }

  public bool IsSlotAvailable => this.CanTakeCraft || this.IsItemCrafted;

  public bool CanTakeCraft
  {
    get
    {
      if ((UnityEngine.Object) this.craftTime == (UnityEngine.Object) null)
        this.craftTime = this.GetComponentInChildren<ItemCraftTimeView>();
      return (UnityEngine.Object) this.craftTime != (UnityEngine.Object) null && this.craftTime.IsItemReady;
    }
    private set => this._CanTakeCraft = value;
  }

  private void OnJoystick(bool joystick)
  {
    this.selectionFrame.SetActive(joystick && this.IsSelected);
    if (joystick)
    {
      if (this.IsSelected && !this.IsItemCrafted)
      {
        this.brewConsoleTooltip.SetActive(true);
        this.timeText.gameObject.SetActive(true);
        this.brewingTextObject.text = string.Empty;
      }
      else if (this.IsSelected && this.IsItemCrafted && this.CanTakeCraft)
        this.takeConsoleTooltip.SetActive(true);
      else
        this.timeText.gameObject.SetActive(false);
    }
    else
    {
      this.brewConsoleTooltip.SetActive(false);
      if (!this.IsItemCrafted)
      {
        this.brewingTextObject.text = this.brewingTextBuffer;
        this.timeText.gameObject.SetActive(true);
      }
      this.takeConsoleTooltip.SetActive(false);
    }
  }

  private void OnDisable()
  {
    InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
    this.SetSelected(false);
    if (!((UnityEngine.Object) this.craftTime != (UnityEngine.Object) null))
      return;
    this.craftTime.OnItemReady -= new Action(this.OnItemReady);
  }

  public void SetSelected(bool selected)
  {
    this.selectionFrame?.SetActive(selected);
    this.brewingTextObject.gameObject.SetActive(selected);
    this.IsSelected = selected;
    if (selected)
    {
      if (!InputService.Instance.JoystickUsed)
        return;
      if (this.CanTakeCraft)
      {
        this.takeConsoleTooltip.SetActive(selected);
      }
      else
      {
        this.brewConsoleTooltip.SetActive(selected);
        this.timeText.gameObject.SetActive(true);
        this.brewingTextObject.text = string.Empty;
        this.takeConsoleTooltip.SetActive(false);
      }
    }
    else
    {
      this.brewConsoleTooltip.SetActive(selected);
      this.brewingTextObject.text = this.brewingTextBuffer;
      this.timeText.gameObject.SetActive(true);
      this.takeConsoleTooltip.SetActive(false);
    }
  }

  public delegate void OnCraft(CraftBrewingSlot slot);
}
