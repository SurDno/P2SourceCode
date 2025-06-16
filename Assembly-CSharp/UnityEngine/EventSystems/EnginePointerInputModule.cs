using System.Collections.Generic;
using InputServices;

namespace UnityEngine.EventSystems
{
  public abstract class EnginePointerInputModule : BaseInputModule
  {
    public const int kMouseLeftId = -1;
    public const int kMouseRightId = -2;
    public const int kMouseMiddleId = -3;
    protected Dictionary<int, PointerEventData> m_PointerData = new Dictionary<int, PointerEventData>();
    private readonly MouseState m_MouseState = new MouseState();

    protected bool GetPointerData(int id, out PointerEventData data, bool create)
    {
      if (!(!m_PointerData.TryGetValue(id, out data) & create))
        return false;
      data = new PointerEventData(eventSystem)
      {
        pointerId = id
      };
      m_PointerData.Add(id, data);
      return true;
    }

    private void CopyFromTo(PointerEventData from, PointerEventData to)
    {
      to.position = from.position;
      to.delta = from.delta;
      to.scrollDelta = from.scrollDelta;
      to.pointerCurrentRaycast = from.pointerCurrentRaycast;
      to.pointerEnter = from.pointerEnter;
    }

    protected static PointerEventData.FramePressState StateForMouseButton(int buttonId)
    {
      bool mouseButtonDown = Input.GetMouseButtonDown(buttonId);
      bool mouseButtonUp = Input.GetMouseButtonUp(buttonId);
      if (mouseButtonDown & mouseButtonUp)
        return PointerEventData.FramePressState.PressedAndReleased;
      if (mouseButtonDown)
        return PointerEventData.FramePressState.Pressed;
      return mouseButtonUp ? PointerEventData.FramePressState.Released : PointerEventData.FramePressState.NotChanged;
    }

    protected virtual MouseState GetMousePointerEventData()
    {
      PointerEventData data1;
      bool pointerData = GetPointerData(-1, out data1, true);
      data1.Reset();
      if (pointerData)
        data1.position = CursorService.Instance.Position;
      Vector2 position = CursorService.Instance.Position;
      data1.delta = position - data1.position;
      data1.position = position;
      data1.scrollDelta = Input.mouseScrollDelta;
      data1.button = PointerEventData.InputButton.Left;
      eventSystem.RaycastAll(data1, m_RaycastResultCache);
      RaycastResult firstRaycast = FindFirstRaycast(m_RaycastResultCache);
      data1.pointerCurrentRaycast = firstRaycast;
      m_RaycastResultCache.Clear();
      PointerEventData data2;
      GetPointerData(-2, out data2, true);
      CopyFromTo(data1, data2);
      data2.button = PointerEventData.InputButton.Right;
      PointerEventData data3;
      GetPointerData(-3, out data3, true);
      CopyFromTo(data1, data3);
      data3.button = PointerEventData.InputButton.Middle;
      m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), data1);
      m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), data2);
      m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), data3);
      return m_MouseState;
    }

    protected PointerEventData GetLastPointerEventData(int id)
    {
      PointerEventData data;
      GetPointerData(id, out data, false);
      return data;
    }

    private static bool ShouldStartDrag(
      Vector2 pressPos,
      Vector2 currentPos,
      float threshold,
      bool useDragThreshold)
    {
      return !useDragThreshold || (pressPos - currentPos).sqrMagnitude >= threshold * (double) threshold;
    }

    protected virtual void ProcessMove(PointerEventData pointerEvent)
    {
      GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
      HandlePointerExitAndEnter(pointerEvent, gameObject);
    }

    protected virtual void ProcessDrag(PointerEventData pointerEvent)
    {
      bool flag = pointerEvent.IsPointerMoving();
      if (flag && pointerEvent.pointerDrag != null && !pointerEvent.dragging && ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
      {
        ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
        pointerEvent.dragging = true;
      }
      if (!(pointerEvent.dragging & flag) || !(pointerEvent.pointerDrag != null))
        return;
      if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
      {
        ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
        pointerEvent.eligibleForClick = false;
        pointerEvent.pointerPress = null;
        pointerEvent.rawPointerPress = null;
      }
      ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
    }

    public override bool IsPointerOverGameObject(int pointerId)
    {
      PointerEventData pointerEventData = GetLastPointerEventData(pointerId);
      return pointerEventData != null && pointerEventData.pointerEnter != null;
    }

    protected void ClearSelection()
    {
      BaseEventData baseEventData = GetBaseEventData();
      foreach (PointerEventData currentPointerData in m_PointerData.Values)
        HandlePointerExitAndEnter(currentPointerData, null);
      m_PointerData.Clear();
      eventSystem.SetSelectedGameObject(null, baseEventData);
    }

    protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
    {
      if (!(ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo) != eventSystem.currentSelectedGameObject))
        return;
      eventSystem.SetSelectedGameObject(null, pointerEvent);
    }
  }
}
