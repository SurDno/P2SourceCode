using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services.Inputs;
using Engine.Source.UI.Controls;
using InputServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BodyPartSelectable : MonoBehaviour
{
  [SerializeField]
  private GameObject _selectedFrame;
  [SerializeField]
  private GameObject _buttonExtractHint;
  [SerializeField]
  private GameObject _buttonTakeHint;
  private SwitchingItemView itemView;
  private HideableProgressFading hideableClosed;
  private bool _OrganRemoved;
  private ItemSelector toolSelector;
  private bool _OrganTaken;
  private PointerEventData pointerData;
  private GraphicRaycaster raycaster;

  public bool OrganRemoved
  {
    get
    {
      if (hideableClosed == null)
        hideableClosed = GetComponentInChildren<HideableProgressFading>();
      OrganRemoved = !hideableClosed.Visible && !OrganTaken;
      return _OrganRemoved;
    }
    set => _OrganRemoved = value;
  }

  public bool OrganTaken
  {
    get
    {
      if (itemView == null)
        itemView = GetComponentInChildren<SwitchingItemView>();
      if (itemView != null)
        OrganTaken = itemView.Storable == null && itemView.IsEmptySlotActive();
      return _OrganTaken;
    }
    set => _OrganTaken = value;
  }

  public HoldableButton2 uiButton { get; private set; }

  public bool IsSelected { get; private set; }

  public void SetSelected(bool selected)
  {
    _selectedFrame.SetActive(selected);
    bool flag = selected && !OrganTaken;
    _buttonExtractHint.SetActive(flag && !OrganRemoved && toolSelector.Item != null);
    _buttonTakeHint.SetActive(flag && OrganRemoved);
    IsSelected = selected;
    if (selected)
    {
      if (!OrganRemoved && toolSelector.Item != null)
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, AutopsyOrgan);
      else if (OrganRemoved && !OrganTaken)
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, TakeOrgan);
      _buttonExtractHint.transform.parent.position = new Vector3(transform.position.x, transform.position.y - 100f);
    }
    else
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, AutopsyOrgan);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, TakeOrgan);
      uiButton?.GamepadEndHold();
    }
  }

  private bool AutopsyOrgan(GameActionType type, bool down)
  {
    if (!InputService.Instance.JoystickUsed || down && toolSelector.Item == null)
      return false;
    if (type == GameActionType.Submit & down)
    {
      uiButton?.GamepadStartHold();
      return true;
    }
    if (type != GameActionType.Submit || down)
      return false;
    uiButton?.GamepadEndHold();
    if (OrganRemoved)
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, AutopsyOrgan);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, TakeOrgan);
    }
    else if (toolSelector.Item == null && transform.parent.GetComponentsInChildren<BodyPartSelectable>().Count(organ => organ.OrganRemoved) == 0)
      SetSelected(false);
    OnJoystick(InputService.Instance.JoystickUsed);
    return true;
  }

  private bool TakeOrgan(GameActionType type, bool down)
  {
    if (!InputService.Instance.JoystickUsed || !(type == GameActionType.Submit & down))
      return false;
    List<RaycastResult> source = [];
    if (pointerData == null)
      return false;
    raycaster.Raycast(pointerData, source);
    if (source.Count != 0)
    {
      GameObject gameObject = source.First().gameObject;
      if (gameObject != null)
      {
        pointerData = new PointerEventData(EventSystem.current)
        {
          position = transform.position
        };
        EmulateClickOnConsole(source.First(), gameObject);
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, AutopsyOrgan);
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, TakeOrgan);
        CoroutineService.Instance.WaitFrame(1, (Action) (() =>
        {
          if (toolSelector.Item == null && !OrganRemoved && transform.parent.GetComponentsInChildren<BodyPartSelectable>().Count(organ => organ.OrganRemoved) == 0)
            SetSelected(false);
          OnJoystick(InputService.Instance.JoystickUsed);
        }));
        return true;
      }
    }
    return true;
  }

  private void EmulateClickOnConsole(RaycastResult raycastResult, GameObject currentOverGo)
  {
    pointerData.eligibleForClick = true;
    pointerData.delta = Vector2.zero;
    pointerData.dragging = true;
    pointerData.useDragThreshold = true;
    pointerData.pressPosition = pointerData.position;
    pointerData.pointerCurrentRaycast = raycastResult;
    pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
    GameObject gameObject = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerData, ExecuteEvents.pointerDownHandler);
    if (gameObject == null)
      gameObject = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
    pointerData.clickCount = 1;
    pointerData.pointerPress = gameObject;
    pointerData.rawPointerPress = currentOverGo;
    ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);
    if (!(pointerData.pointerPress == ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo)) || !pointerData.eligibleForClick)
      return;
    ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerClickHandler);
  }

  private void Start() => uiButton = GetComponentInChildren<HoldableButton2>();

  private void OnEnable()
  {
    InputService.Instance.onJoystickUsedChanged += OnJoystick;
    itemView = GetComponentInChildren<SwitchingItemView>();
    hideableClosed = GetComponentInChildren<HideableProgressFading>();
    if (raycaster == null)
    {
      raycaster = GetComponentInParent<GraphicRaycaster>();
      pointerData = new PointerEventData(EventSystem.current)
      {
        position = transform.position
      };
    }
    toolSelector = transform.parent.GetComponentInChildren<ItemSelector>();
    SetSelected(false);
  }

  private void OnDisable()
  {
    InputService.Instance.onJoystickUsedChanged -= OnJoystick;
    OrganRemoved = false;
    OrganTaken = false;
  }

  private void OnJoystick(bool joystick)
  {
    _selectedFrame.SetActive(joystick && IsSelected);
    if (IsSelected)
    {
      bool flag = joystick && !OrganTaken;
      _buttonExtractHint.SetActive(flag && !OrganRemoved && toolSelector.Item != null);
      _buttonTakeHint.SetActive(flag && OrganRemoved);
    }
    if (joystick)
      return;
    uiButton?.GamepadEndHold();
  }

  public void FireConsoleOnEnterEvent()
  {
    if (OrganRemoved)
      return;
    uiButton.OnPointerEnter(pointerData);
  }

  public void FireConsoleOnExitEvent()
  {
    if (OrganRemoved)
      return;
    uiButton.OnPointerExit(pointerData);
  }

  private void Update()
  {
  }
}
