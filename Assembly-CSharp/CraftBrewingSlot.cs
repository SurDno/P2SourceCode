using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Services.Inputs;
using InputServices;

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
  private bool _isSelected;
  private IStorableComponent craftedItem = null;
  private bool _IsItemCrafted;
  private bool _CanTakeCraft;
  private ItemCraftTimeView craftTime = null;

  public event Action<IInventoryComponent> CraftEvent;

  public event Action<IStorableComponent> TakeEvent;

  public event OnCraft OnCraftPerformed;

  public event OnCraft OnItemTaken;

  private void FireCraftEvent()
  {
    OnCraft onCraftPerformed = OnCraftPerformed;
    if (onCraftPerformed != null)
      onCraftPerformed(this);
    IsItemCrafted = true;
    Action<IInventoryComponent> craftEvent = CraftEvent;
    if (craftEvent == null)
      return;
    craftEvent(containerView.Container);
  }

  private void FireTakeEvent(IStorableComponent item)
  {
    Action<IStorableComponent> takeEvent = TakeEvent;
    if (takeEvent != null)
      takeEvent(item);
    OnCraft onItemTaken = OnItemTaken;
    if (onItemTaken != null)
      onItemTaken(this);
    IsItemCrafted = false;
    CanTakeCraft = false;
    SetSelected(false);
  }

  public void Initialize(float durabilityThreshold)
  {
    durabilityRangeCheck.HiddenRange = new Vector2(0.0f, durabilityThreshold);
    containerView.ItemInteractEvent += FireTakeEvent;
    button.onClick.AddListener(new UnityAction(FireCraftEvent));
    IsItemCrafted = false;
    SetSelected(false);
    craftTime = this.GetComponentInChildren<ItemCraftTimeView>();
    if (!((UnityEngine.Object) craftTime != (UnityEngine.Object) null))
      return;
    craftTime.OnItemReady += OnItemReady;
  }

  public void SetTarget(IEntity target, IInventoryComponent container)
  {
    containerView.Container = (InventoryComponent) container;
    targetView.Value = target;
  }

  public void SetEnabled(bool value)
  {
    button.interactable = value;
    interactableView.Visible = value;
    IsEnabled = value;
    if (!IsEnabled)
    {
      SetSelected(false);
      InputService.Instance.onJoystickUsedChanged -= OnJoystick;
    }
    else
    {
      InputService.Instance.onJoystickUsedChanged += OnJoystick;
      brewingTextBuffer = brewingTextObject.text;
      if ((UnityEngine.Object) raycaster == (UnityEngine.Object) null)
      {
        raycaster = this.GetComponentInParent<GraphicRaycaster>();
        pointerData = new PointerEventData(EventSystem.current)
        {
          position = (Vector2) button.transform.position
        };
      }
    }
  }

  public void SetCraftTime(string text) => timeText.StringValue = text;

  private void OnItemReady()
  {
    CanTakeCraft = true;
    if (!IsSelected)
      return;
    takeConsoleTooltip.SetActive(true);
  }

  public bool IsVisible => this.GetComponent<HideableCouple>().Visible;

  public bool IsSelected
  {
    get => _isSelected;
    private set
    {
      if (value == _isSelected)
        return;
      if (value)
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, CraftTakeListener);
      else
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, CraftTakeListener);
      _isSelected = value;
    }
  }

  public bool IsItemCrafted
  {
    get
    {
      if (!_IsItemCrafted)
      {
        SwitchingItemView componentInChildren = this.GetComponentInChildren<SwitchingItemView>();
        if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
          IsItemCrafted = componentInChildren.Storable != null;
      }
      return _IsItemCrafted;
    }
    private set => _IsItemCrafted = value;
  }

  private bool CraftTakeListener(GameActionType type, bool down)
  {
    if (!InputService.Instance.JoystickUsed || IsItemCrafted && !CanTakeCraft || !down)
      return false;
    List<RaycastResult> source = new List<RaycastResult>();
    if (pointerData == null || (UnityEngine.Object) raycaster == (UnityEngine.Object) null)
    {
      raycaster = this.GetComponentInParent<GraphicRaycaster>();
      pointerData = new PointerEventData(EventSystem.current)
      {
        position = (Vector2) button.transform.position
      };
    }
    if (pointerData == null)
      return false;
    raycaster.Raycast(pointerData, source);
    if (source.Count != 0)
    {
      GameObject gameObject = source.First().gameObject;
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
      {
        pointerData = new PointerEventData(EventSystem.current)
        {
          position = (Vector2) button.transform.position
        };
        EmulateClickOnConsole(source.First(), gameObject);
        return true;
      }
    }
    return false;
  }

  private void EmulateClickOnConsole(RaycastResult raycastResult, GameObject currentOverGo)
  {
    pointerData.eligibleForClick = true;
    pointerData.delta = Vector2.zero;
    pointerData.dragging = false;
    pointerData.useDragThreshold = true;
    pointerData.pressPosition = pointerData.position;
    pointerData.pointerCurrentRaycast = raycastResult;
    pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
    GameObject gameObject = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(currentOverGo, (BaseEventData) pointerData, ExecuteEvents.pointerDownHandler);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      gameObject = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
    pointerData.clickCount = 1;
    pointerData.pointerPress = gameObject;
    pointerData.rawPointerPress = currentOverGo;
    ExecuteEvents.Execute<IPointerUpHandler>(pointerData.pointerPress, (BaseEventData) pointerData, ExecuteEvents.pointerUpHandler);
    if (!((UnityEngine.Object) pointerData.pointerPress == (UnityEngine.Object) ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo)) || !pointerData.eligibleForClick)
      return;
    ExecuteEvents.Execute<IPointerClickHandler>(pointerData.pointerPress, (BaseEventData) pointerData, ExecuteEvents.pointerClickHandler);
  }

  public bool IsEnabled { get; set; }

  public bool IsSlotAvailable => CanTakeCraft || IsItemCrafted;

  public bool CanTakeCraft
  {
    get
    {
      if ((UnityEngine.Object) craftTime == (UnityEngine.Object) null)
        craftTime = this.GetComponentInChildren<ItemCraftTimeView>();
      return (UnityEngine.Object) craftTime != (UnityEngine.Object) null && craftTime.IsItemReady;
    }
    private set => _CanTakeCraft = value;
  }

  private void OnJoystick(bool joystick)
  {
    selectionFrame.SetActive(joystick && IsSelected);
    if (joystick)
    {
      if (IsSelected && !IsItemCrafted)
      {
        brewConsoleTooltip.SetActive(true);
        timeText.gameObject.SetActive(true);
        brewingTextObject.text = string.Empty;
      }
      else if (IsSelected && IsItemCrafted && CanTakeCraft)
        takeConsoleTooltip.SetActive(true);
      else
        timeText.gameObject.SetActive(false);
    }
    else
    {
      brewConsoleTooltip.SetActive(false);
      if (!IsItemCrafted)
      {
        brewingTextObject.text = brewingTextBuffer;
        timeText.gameObject.SetActive(true);
      }
      takeConsoleTooltip.SetActive(false);
    }
  }

  private void OnDisable()
  {
    InputService.Instance.onJoystickUsedChanged -= OnJoystick;
    SetSelected(false);
    if (!((UnityEngine.Object) craftTime != (UnityEngine.Object) null))
      return;
    craftTime.OnItemReady -= OnItemReady;
  }

  public void SetSelected(bool selected)
  {
    selectionFrame?.SetActive(selected);
    brewingTextObject.gameObject.SetActive(selected);
    IsSelected = selected;
    if (selected)
    {
      if (!InputService.Instance.JoystickUsed)
        return;
      if (CanTakeCraft)
      {
        takeConsoleTooltip.SetActive(selected);
      }
      else
      {
        brewConsoleTooltip.SetActive(selected);
        timeText.gameObject.SetActive(true);
        brewingTextObject.text = string.Empty;
        takeConsoleTooltip.SetActive(false);
      }
    }
    else
    {
      brewConsoleTooltip.SetActive(selected);
      brewingTextObject.text = brewingTextBuffer;
      timeText.gameObject.SetActive(true);
      takeConsoleTooltip.SetActive(false);
    }
  }

  public delegate void OnCraft(CraftBrewingSlot slot);
}
