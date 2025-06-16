using InputServices;

namespace UnityEngine.EventSystems
{
  public class EngineStandaloneInputModule : EnginePointerInputModule
  {
    private float xPrev = 0.0f;
    private float yPrev = 0.0f;

    public override void UpdateModule()
    {
    }

    public override bool IsModuleSupported() => true;

    public override void ActivateModule()
    {
      base.ActivateModule();
      GameObject selectedGameObject = this.eventSystem.currentSelectedGameObject;
      if ((Object) selectedGameObject == (Object) null)
        selectedGameObject = this.eventSystem.lastSelectedGameObject;
      if ((Object) selectedGameObject == (Object) null)
        selectedGameObject = this.eventSystem.firstSelectedGameObject;
      this.eventSystem.SetSelectedGameObject(selectedGameObject, this.GetBaseEventData());
    }

    public override void DeactivateModule()
    {
      base.DeactivateModule();
      ClearSelection();
    }

    public override void Process()
    {
      bool selectedObject = SendUpdateEventToSelectedObject();
      if (this.eventSystem.sendNavigationEvents && !selectedObject)
        SendSubmitEventToSelectedObject();
      ProcessMouseEvent();
      ComputeJoystick();
    }

    protected bool SendSubmitEventToSelectedObject()
    {
      return !((Object) this.eventSystem.currentSelectedGameObject == (Object) null) && this.GetBaseEventData().used;
    }

    protected bool SendUpdateEventToSelectedObject()
    {
      if ((Object) this.eventSystem.currentSelectedGameObject == (Object) null)
        return false;
      BaseEventData baseEventData = this.GetBaseEventData();
      ExecuteEvents.Execute<IUpdateSelectedHandler>(this.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
      return baseEventData.used;
    }

    private void ProcessMouseEvent()
    {
      MouseState pointerEventData = GetMousePointerEventData();
      bool pressed = pointerEventData.AnyPressesThisFrame();
      bool released = pointerEventData.AnyReleasesThisFrame();
      MouseButtonEventData eventData = pointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
      if (!UseMouse(pressed, released, eventData.buttonData))
        return;
      InputService.Instance.JoystickUsed = false;
      ProcessMousePress(eventData);
      ProcessMove(eventData.buttonData);
      ProcessDrag(eventData.buttonData);
      ProcessMousePress(pointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
      ProcessDrag(pointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
      ProcessMousePress(pointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
      ProcessDrag(pointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
      if (Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
        return;
      ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject), (BaseEventData) eventData.buttonData, ExecuteEvents.scrollHandler);
    }

    private static bool UseMouse(bool pressed, bool released, PointerEventData pointerData)
    {
      return pressed | released || pointerData.IsPointerMoving() || pointerData.IsScrolling();
    }

    private void ProcessMousePress(MouseButtonEventData data)
    {
      PointerEventData buttonData = data.buttonData;
      GameObject gameObject1 = buttonData.pointerCurrentRaycast.gameObject;
      if (data.PressedThisFrame())
      {
        buttonData.eligibleForClick = true;
        buttonData.delta = Vector2.zero;
        buttonData.dragging = false;
        buttonData.useDragThreshold = true;
        buttonData.pressPosition = buttonData.position;
        buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
        DeselectIfSelectionChanged(gameObject1, (BaseEventData) buttonData);
        GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject1, (BaseEventData) buttonData, ExecuteEvents.pointerDownHandler);
        if ((Object) gameObject2 == (Object) null)
          gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
        float unscaledTime = Time.unscaledTime;
        if ((Object) gameObject2 == (Object) buttonData.lastPress)
        {
          if (unscaledTime - buttonData.clickTime < 0.30000001192092896)
            ++buttonData.clickCount;
          else
            buttonData.clickCount = 1;
          buttonData.clickTime = unscaledTime;
        }
        else
          buttonData.clickCount = 1;
        buttonData.pointerPress = gameObject2;
        buttonData.rawPointerPress = gameObject1;
        buttonData.clickTime = unscaledTime;
        buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject1);
        if ((Object) buttonData.pointerDrag != (Object) null)
          ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.initializePotentialDrag);
      }
      if (!data.ReleasedThisFrame())
        return;
      ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerUpHandler);
      GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
      if ((Object) buttonData.pointerPress == (Object) eventHandler && buttonData.eligibleForClick)
        ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerClickHandler);
      else if ((Object) buttonData.pointerDrag != (Object) null)
        ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject1, (BaseEventData) buttonData, ExecuteEvents.dropHandler);
      buttonData.eligibleForClick = false;
      buttonData.pointerPress = (GameObject) null;
      buttonData.rawPointerPress = (GameObject) null;
      if ((Object) buttonData.pointerDrag != (Object) null && buttonData.dragging)
        ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.endDragHandler);
      buttonData.dragging = false;
      buttonData.pointerDrag = (GameObject) null;
      if ((Object) gameObject1 != (Object) buttonData.pointerEnter)
      {
        this.HandlePointerExitAndEnter(buttonData, (GameObject) null);
        this.HandlePointerExitAndEnter(buttonData, gameObject1);
      }
    }

    private void ComputeJoystick()
    {
    }
  }
}
