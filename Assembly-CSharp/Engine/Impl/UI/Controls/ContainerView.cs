using System;
using Engine.Common.Components;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class ContainerView : MonoBehaviour
  {
    [Inspected(Mode = ExecuteMode.Runtime, Mutable = false)]
    private InventoryComponent container;

    public event Action<IInventoryComponent> SelectEvent;

    public event Action<IInventoryComponent> DeselectEvent;

    public event Action<IInventoryComponent> OpenBeginEvent;

    public event Action<IInventoryComponent, bool> OpenEndEvent;

    public event Action<IStorableComponent> ItemSelectEvent;

    public event Action<IStorableComponent> ItemDeselectEvent;

    public event Action<IStorableComponent> ItemInteractEvent;

    protected void FireSelectEvent()
    {
      Action<IInventoryComponent> selectEvent = SelectEvent;
      if (selectEvent == null)
        return;
      selectEvent(container);
    }

    protected void FireDeselectEvent()
    {
      Action<IInventoryComponent> deselectEvent = DeselectEvent;
      if (deselectEvent == null)
        return;
      deselectEvent(container);
    }

    protected void FireOpenBeginEvent()
    {
      Action<IInventoryComponent> openBeginEvent = OpenBeginEvent;
      if (openBeginEvent == null)
        return;
      openBeginEvent(container);
    }

    protected void FireOpenEndEvent(bool success)
    {
      Action<IInventoryComponent, bool> openEndEvent = OpenEndEvent;
      if (openEndEvent == null)
        return;
      openEndEvent(container, success);
    }

    protected void FireItemSelectEventEvent(IStorableComponent item)
    {
      Action<IStorableComponent> itemSelectEvent = ItemSelectEvent;
      if (itemSelectEvent == null)
        return;
      itemSelectEvent(item);
    }

    protected void FireItemDeselectEventEvent(IStorableComponent item)
    {
      Action<IStorableComponent> itemDeselectEvent = ItemDeselectEvent;
      if (itemDeselectEvent == null)
        return;
      itemDeselectEvent(item);
    }

    protected void FireItemInteractEventEvent(IStorableComponent item)
    {
      Action<IStorableComponent> itemInteractEvent = ItemInteractEvent;
      if (itemInteractEvent == null)
        return;
      itemInteractEvent(item);
    }

    public InventoryComponent Container
    {
      get => container;
      set
      {
        if (this.container == value)
          return;
        InventoryComponent container = this.container;
        this.container = value;
        OnContainerSet(container);
      }
    }

    protected virtual void OnContainerSet(InventoryComponent previousContainer)
    {
    }

    public virtual void SetCanOpen(bool canOpen)
    {
    }
  }
}
