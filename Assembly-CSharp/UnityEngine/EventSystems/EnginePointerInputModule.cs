// Decompiled with JetBrains decompiler
// Type: UnityEngine.EventSystems.EnginePointerInputModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using InputServices;
using System.Collections.Generic;

#nullable disable
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
      if (!(!this.m_PointerData.TryGetValue(id, out data) & create))
        return false;
      data = new PointerEventData(this.eventSystem)
      {
        pointerId = id
      };
      this.m_PointerData.Add(id, data);
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
      bool pointerData = this.GetPointerData(-1, out data1, true);
      data1.Reset();
      if (pointerData)
        data1.position = CursorService.Instance.Position;
      Vector2 position = CursorService.Instance.Position;
      data1.delta = position - data1.position;
      data1.position = position;
      data1.scrollDelta = Input.mouseScrollDelta;
      data1.button = PointerEventData.InputButton.Left;
      this.eventSystem.RaycastAll(data1, (List<RaycastResult>) this.m_RaycastResultCache);
      RaycastResult firstRaycast = BaseInputModule.FindFirstRaycast((List<RaycastResult>) this.m_RaycastResultCache);
      data1.pointerCurrentRaycast = firstRaycast;
      ((List<RaycastResult>) this.m_RaycastResultCache).Clear();
      PointerEventData data2;
      this.GetPointerData(-2, out data2, true);
      this.CopyFromTo(data1, data2);
      data2.button = PointerEventData.InputButton.Right;
      PointerEventData data3;
      this.GetPointerData(-3, out data3, true);
      this.CopyFromTo(data1, data3);
      data3.button = PointerEventData.InputButton.Middle;
      this.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, EnginePointerInputModule.StateForMouseButton(0), data1);
      this.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, EnginePointerInputModule.StateForMouseButton(1), data2);
      this.m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, EnginePointerInputModule.StateForMouseButton(2), data3);
      return this.m_MouseState;
    }

    protected PointerEventData GetLastPointerEventData(int id)
    {
      PointerEventData data;
      this.GetPointerData(id, out data, false);
      return data;
    }

    private static bool ShouldStartDrag(
      Vector2 pressPos,
      Vector2 currentPos,
      float threshold,
      bool useDragThreshold)
    {
      return !useDragThreshold || (double) (pressPos - currentPos).sqrMagnitude >= (double) threshold * (double) threshold;
    }

    protected virtual void ProcessMove(PointerEventData pointerEvent)
    {
      GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
      this.HandlePointerExitAndEnter(pointerEvent, gameObject);
    }

    protected virtual void ProcessDrag(PointerEventData pointerEvent)
    {
      bool flag = pointerEvent.IsPointerMoving();
      if (flag && (Object) pointerEvent.pointerDrag != (Object) null && !pointerEvent.dragging && EnginePointerInputModule.ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, (float) this.eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
      {
        ExecuteEvents.Execute<IBeginDragHandler>(pointerEvent.pointerDrag, (BaseEventData) pointerEvent, ExecuteEvents.beginDragHandler);
        pointerEvent.dragging = true;
      }
      if (!(pointerEvent.dragging & flag) || !((Object) pointerEvent.pointerDrag != (Object) null))
        return;
      if ((Object) pointerEvent.pointerPress != (Object) pointerEvent.pointerDrag)
      {
        ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, (BaseEventData) pointerEvent, ExecuteEvents.pointerUpHandler);
        pointerEvent.eligibleForClick = false;
        pointerEvent.pointerPress = (GameObject) null;
        pointerEvent.rawPointerPress = (GameObject) null;
      }
      ExecuteEvents.Execute<IDragHandler>(pointerEvent.pointerDrag, (BaseEventData) pointerEvent, ExecuteEvents.dragHandler);
    }

    public override bool IsPointerOverGameObject(int pointerId)
    {
      PointerEventData pointerEventData = this.GetLastPointerEventData(pointerId);
      return pointerEventData != null && (Object) pointerEventData.pointerEnter != (Object) null;
    }

    protected void ClearSelection()
    {
      BaseEventData baseEventData = this.GetBaseEventData();
      foreach (PointerEventData currentPointerData in this.m_PointerData.Values)
        this.HandlePointerExitAndEnter(currentPointerData, (GameObject) null);
      this.m_PointerData.Clear();
      this.eventSystem.SetSelectedGameObject((GameObject) null, baseEventData);
    }

    protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
    {
      if (!((Object) ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo) != (Object) this.eventSystem.currentSelectedGameObject))
        return;
      this.eventSystem.SetSelectedGameObject((GameObject) null, pointerEvent);
    }
  }
}
