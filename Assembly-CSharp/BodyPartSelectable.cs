// Decompiled with JetBrains decompiler
// Type: BodyPartSelectable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services.Inputs;
using Engine.Source.UI.Controls;
using InputServices;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
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
  private bool _OrganRemoved = false;
  private ItemSelector toolSelector = (ItemSelector) null;
  private bool _OrganTaken = false;
  private PointerEventData pointerData;
  private GraphicRaycaster raycaster = (GraphicRaycaster) null;

  public bool OrganRemoved
  {
    get
    {
      if ((UnityEngine.Object) this.hideableClosed == (UnityEngine.Object) null)
        this.hideableClosed = this.GetComponentInChildren<HideableProgressFading>();
      this.OrganRemoved = !this.hideableClosed.Visible && !this.OrganTaken;
      return this._OrganRemoved;
    }
    set => this._OrganRemoved = value;
  }

  public bool OrganTaken
  {
    get
    {
      if ((UnityEngine.Object) this.itemView == (UnityEngine.Object) null)
        this.itemView = this.GetComponentInChildren<SwitchingItemView>();
      if ((UnityEngine.Object) this.itemView != (UnityEngine.Object) null)
        this.OrganTaken = this.itemView.Storable == null && this.itemView.IsEmptySlotActive();
      return this._OrganTaken;
    }
    set => this._OrganTaken = value;
  }

  public HoldableButton2 uiButton { get; private set; }

  public bool IsSelected { get; private set; }

  public void SetSelected(bool selected)
  {
    this._selectedFrame.SetActive(selected);
    bool flag = selected && !this.OrganTaken;
    this._buttonExtractHint.SetActive(flag && !this.OrganRemoved && this.toolSelector.Item != null);
    this._buttonTakeHint.SetActive(flag && this.OrganRemoved);
    this.IsSelected = selected;
    if (selected)
    {
      if (!this.OrganRemoved && this.toolSelector.Item != null)
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.AutopsyOrgan));
      else if (this.OrganRemoved && !this.OrganTaken)
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.TakeOrgan));
      this._buttonExtractHint.transform.parent.position = new Vector3(this.transform.position.x, this.transform.position.y - 100f);
    }
    else
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.AutopsyOrgan));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.TakeOrgan));
      this.uiButton?.GamepadEndHold();
    }
  }

  private bool AutopsyOrgan(GameActionType type, bool down)
  {
    if (!InputService.Instance.JoystickUsed || down && this.toolSelector.Item == null)
      return false;
    if (type == GameActionType.Submit & down)
    {
      this.uiButton?.GamepadStartHold();
      return true;
    }
    if (type != GameActionType.Submit || down)
      return false;
    this.uiButton?.GamepadEndHold();
    if (this.OrganRemoved)
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.AutopsyOrgan));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.TakeOrgan));
    }
    else if (this.toolSelector.Item == null && ((IEnumerable<BodyPartSelectable>) this.transform.parent.GetComponentsInChildren<BodyPartSelectable>()).Count<BodyPartSelectable>((Func<BodyPartSelectable, bool>) (organ => organ.OrganRemoved)) == 0)
      this.SetSelected(false);
    this.OnJoystick(InputService.Instance.JoystickUsed);
    return true;
  }

  private bool TakeOrgan(GameActionType type, bool down)
  {
    if (!InputService.Instance.JoystickUsed || !(type == GameActionType.Submit & down))
      return false;
    List<RaycastResult> source = new List<RaycastResult>();
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
          position = (Vector2) this.transform.position
        };
        this.EmulateClickOnConsole(source.First<RaycastResult>(), gameObject);
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.AutopsyOrgan));
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.TakeOrgan));
        CoroutineService.Instance.WaitFrame(1, (Action) (() =>
        {
          if (this.toolSelector.Item == null && !this.OrganRemoved && ((IEnumerable<BodyPartSelectable>) this.transform.parent.GetComponentsInChildren<BodyPartSelectable>()).Count<BodyPartSelectable>((Func<BodyPartSelectable, bool>) (organ => organ.OrganRemoved)) == 0)
            this.SetSelected(false);
          this.OnJoystick(InputService.Instance.JoystickUsed);
        }));
        return true;
      }
    }
    return true;
  }

  private void EmulateClickOnConsole(RaycastResult raycastResult, GameObject currentOverGo)
  {
    this.pointerData.eligibleForClick = true;
    this.pointerData.delta = Vector2.zero;
    this.pointerData.dragging = true;
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

  private void Start() => this.uiButton = this.GetComponentInChildren<HoldableButton2>();

  private void OnEnable()
  {
    InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
    this.itemView = this.GetComponentInChildren<SwitchingItemView>();
    this.hideableClosed = this.GetComponentInChildren<HideableProgressFading>();
    if ((UnityEngine.Object) this.raycaster == (UnityEngine.Object) null)
    {
      this.raycaster = this.GetComponentInParent<GraphicRaycaster>();
      this.pointerData = new PointerEventData(EventSystem.current)
      {
        position = (Vector2) this.transform.position
      };
    }
    this.toolSelector = this.transform.parent.GetComponentInChildren<ItemSelector>();
    this.SetSelected(false);
  }

  private void OnDisable()
  {
    InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
    this.OrganRemoved = false;
    this.OrganTaken = false;
  }

  private void OnJoystick(bool joystick)
  {
    this._selectedFrame.SetActive(joystick && this.IsSelected);
    if (this.IsSelected)
    {
      bool flag = joystick && !this.OrganTaken;
      this._buttonExtractHint.SetActive(flag && !this.OrganRemoved && this.toolSelector.Item != null);
      this._buttonTakeHint.SetActive(flag && this.OrganRemoved);
    }
    if (joystick)
      return;
    this.uiButton?.GamepadEndHold();
  }

  public void FireConsoleOnEnterEvent()
  {
    if (this.OrganRemoved)
      return;
    this.uiButton.OnPointerEnter(this.pointerData);
  }

  public void FireConsoleOnExitEvent()
  {
    if (this.OrganRemoved)
      return;
    this.uiButton.OnPointerExit(this.pointerData);
  }

  private void Update()
  {
  }
}
