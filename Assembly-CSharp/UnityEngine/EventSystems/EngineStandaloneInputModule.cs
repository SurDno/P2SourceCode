using InputServices;

namespace UnityEngine.EventSystems;

public class EngineStandaloneInputModule : EnginePointerInputModule {
	private float xPrev = 0.0f;
	private float yPrev = 0.0f;

	public override void UpdateModule() { }

	public override bool IsModuleSupported() {
		return true;
	}

	public override void ActivateModule() {
		base.ActivateModule();
		var selectedGameObject = eventSystem.currentSelectedGameObject;
		if (selectedGameObject == null)
			selectedGameObject = eventSystem.lastSelectedGameObject;
		if (selectedGameObject == null)
			selectedGameObject = eventSystem.firstSelectedGameObject;
		eventSystem.SetSelectedGameObject(selectedGameObject, GetBaseEventData());
	}

	public override void DeactivateModule() {
		base.DeactivateModule();
		ClearSelection();
	}

	public override void Process() {
		var selectedObject = SendUpdateEventToSelectedObject();
		if (eventSystem.sendNavigationEvents && !selectedObject)
			SendSubmitEventToSelectedObject();
		ProcessMouseEvent();
		ComputeJoystick();
	}

	protected bool SendSubmitEventToSelectedObject() {
		return !(eventSystem.currentSelectedGameObject == null) && GetBaseEventData().used;
	}

	protected bool SendUpdateEventToSelectedObject() {
		if (eventSystem.currentSelectedGameObject == null)
			return false;
		var baseEventData = GetBaseEventData();
		ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, baseEventData,
			ExecuteEvents.updateSelectedHandler);
		return baseEventData.used;
	}

	private void ProcessMouseEvent() {
		var pointerEventData = GetMousePointerEventData();
		var pressed = pointerEventData.AnyPressesThisFrame();
		var released = pointerEventData.AnyReleasesThisFrame();
		var eventData = pointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
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
		ExecuteEvents.ExecuteHierarchy(
			ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject),
			eventData.buttonData, ExecuteEvents.scrollHandler);
	}

	private static bool UseMouse(bool pressed, bool released, PointerEventData pointerData) {
		return pressed | released || pointerData.IsPointerMoving() || pointerData.IsScrolling();
	}

	private void ProcessMousePress(MouseButtonEventData data) {
		var buttonData = data.buttonData;
		var gameObject1 = buttonData.pointerCurrentRaycast.gameObject;
		if (data.PressedThisFrame()) {
			buttonData.eligibleForClick = true;
			buttonData.delta = Vector2.zero;
			buttonData.dragging = false;
			buttonData.useDragThreshold = true;
			buttonData.pressPosition = buttonData.position;
			buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
			DeselectIfSelectionChanged(gameObject1, buttonData);
			var gameObject2 = ExecuteEvents.ExecuteHierarchy(gameObject1, buttonData, ExecuteEvents.pointerDownHandler);
			if (gameObject2 == null)
				gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
			var unscaledTime = Time.unscaledTime;
			if (gameObject2 == buttonData.lastPress) {
				if (unscaledTime - buttonData.clickTime < 0.30000001192092896)
					++buttonData.clickCount;
				else
					buttonData.clickCount = 1;
				buttonData.clickTime = unscaledTime;
			} else
				buttonData.clickCount = 1;

			buttonData.pointerPress = gameObject2;
			buttonData.rawPointerPress = gameObject1;
			buttonData.clickTime = unscaledTime;
			buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject1);
			if (buttonData.pointerDrag != null)
				ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
		}

		if (!data.ReleasedThisFrame())
			return;
		ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
		var eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
		if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
			ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
		else if (buttonData.pointerDrag != null)
			ExecuteEvents.ExecuteHierarchy(gameObject1, buttonData, ExecuteEvents.dropHandler);
		buttonData.eligibleForClick = false;
		buttonData.pointerPress = null;
		buttonData.rawPointerPress = null;
		if (buttonData.pointerDrag != null && buttonData.dragging)
			ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
		buttonData.dragging = false;
		buttonData.pointerDrag = null;
		if (gameObject1 != buttonData.pointerEnter) {
			HandlePointerExitAndEnter(buttonData, null);
			HandlePointerExitAndEnter(buttonData, gameObject1);
		}
	}

	private void ComputeJoystick() { }
}