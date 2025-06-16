using System;
using Engine.Common.Components;
using Engine.Source.Components;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ItemView : 
  MonoBehaviour,
  IPointerDownHandler,
  IEventSystemHandler,
  IPointerEnterHandler,
  IPointerExitHandler
{
  public abstract StorableComponent Storable { get; set; }

  public virtual void SkipAnimation()
  {
  }

  public event Action<IStorableComponent> SelectEvent;

  public event Action<IStorableComponent> DeselectEvent;

  public event Action<IStorableComponent> InteractEvent;

  void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
  {
    if (Storable == null)
      return;
    Action<IStorableComponent> interactEvent = InteractEvent;
    if (interactEvent == null)
      return;
    interactEvent(Storable);
  }

  void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
  {
    if (Storable == null)
      return;
    Action<IStorableComponent> selectEvent = SelectEvent;
    if (selectEvent == null)
      return;
    selectEvent(Storable);
  }

  void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
  {
    if (Storable == null)
      return;
    Action<IStorableComponent> deselectEvent = DeselectEvent;
    if (deselectEvent == null)
      return;
    deselectEvent(Storable);
  }

  protected void FireSelectEvent(IStorableComponent item)
  {
    Action<IStorableComponent> selectEvent = SelectEvent;
    if (selectEvent == null)
      return;
    selectEvent(item);
  }

  protected void FireDeselectEvent(IStorableComponent item)
  {
    Action<IStorableComponent> deselectEvent = DeselectEvent;
    if (deselectEvent == null)
      return;
    deselectEvent(item);
  }

  protected void FireInteractEvent(IStorableComponent item)
  {
    Action<IStorableComponent> interactEvent = InteractEvent;
    if (interactEvent == null)
      return;
    interactEvent(item);
  }
}
