// Decompiled with JetBrains decompiler
// Type: ItemView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Source.Components;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
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
    if (this.Storable == null)
      return;
    Action<IStorableComponent> interactEvent = this.InteractEvent;
    if (interactEvent == null)
      return;
    interactEvent((IStorableComponent) this.Storable);
  }

  void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
  {
    if (this.Storable == null)
      return;
    Action<IStorableComponent> selectEvent = this.SelectEvent;
    if (selectEvent == null)
      return;
    selectEvent((IStorableComponent) this.Storable);
  }

  void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
  {
    if (this.Storable == null)
      return;
    Action<IStorableComponent> deselectEvent = this.DeselectEvent;
    if (deselectEvent == null)
      return;
    deselectEvent((IStorableComponent) this.Storable);
  }

  protected void FireSelectEvent(IStorableComponent item)
  {
    Action<IStorableComponent> selectEvent = this.SelectEvent;
    if (selectEvent == null)
      return;
    selectEvent(item);
  }

  protected void FireDeselectEvent(IStorableComponent item)
  {
    Action<IStorableComponent> deselectEvent = this.DeselectEvent;
    if (deselectEvent == null)
      return;
    deselectEvent(item);
  }

  protected void FireInteractEvent(IStorableComponent item)
  {
    Action<IStorableComponent> interactEvent = this.InteractEvent;
    if (interactEvent == null)
      return;
    interactEvent(item);
  }
}
