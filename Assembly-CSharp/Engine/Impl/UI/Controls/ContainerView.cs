// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ContainerView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Source.Components;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
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
      Action<IInventoryComponent> selectEvent = this.SelectEvent;
      if (selectEvent == null)
        return;
      selectEvent((IInventoryComponent) this.container);
    }

    protected void FireDeselectEvent()
    {
      Action<IInventoryComponent> deselectEvent = this.DeselectEvent;
      if (deselectEvent == null)
        return;
      deselectEvent((IInventoryComponent) this.container);
    }

    protected void FireOpenBeginEvent()
    {
      Action<IInventoryComponent> openBeginEvent = this.OpenBeginEvent;
      if (openBeginEvent == null)
        return;
      openBeginEvent((IInventoryComponent) this.container);
    }

    protected void FireOpenEndEvent(bool success)
    {
      Action<IInventoryComponent, bool> openEndEvent = this.OpenEndEvent;
      if (openEndEvent == null)
        return;
      openEndEvent((IInventoryComponent) this.container, success);
    }

    protected void FireItemSelectEventEvent(IStorableComponent item)
    {
      Action<IStorableComponent> itemSelectEvent = this.ItemSelectEvent;
      if (itemSelectEvent == null)
        return;
      itemSelectEvent(item);
    }

    protected void FireItemDeselectEventEvent(IStorableComponent item)
    {
      Action<IStorableComponent> itemDeselectEvent = this.ItemDeselectEvent;
      if (itemDeselectEvent == null)
        return;
      itemDeselectEvent(item);
    }

    protected void FireItemInteractEventEvent(IStorableComponent item)
    {
      Action<IStorableComponent> itemInteractEvent = this.ItemInteractEvent;
      if (itemInteractEvent == null)
        return;
      itemInteractEvent(item);
    }

    public InventoryComponent Container
    {
      get => this.container;
      set
      {
        if (this.container == value)
          return;
        InventoryComponent container = this.container;
        this.container = value;
        this.OnContainerSet(container);
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
