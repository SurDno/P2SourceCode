using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Source.Components;
using Engine.Source.UI.Controls;
using System;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SingleItemContainerView : ContainerView, IChangeParameterListener
  {
    [SerializeField]
    private ItemView storableView;
    [SerializeField]
    private HoldableButton2 buttonClosed;
    [SerializeField]
    private HideableView hideableView;
    [SerializeField]
    private HideableView hideableViewOpen;
    [SerializeField]
    private HideableView hideableViewClosed;

    private void Awake()
    {
      if ((UnityEngine.Object) this.buttonClosed != (UnityEngine.Object) null)
      {
        this.buttonClosed.SelectEvent += new Action(((ContainerView) this).FireSelectEvent);
        this.buttonClosed.DeselectEvent += new Action(((ContainerView) this).FireDeselectEvent);
        this.buttonClosed.OpenBeginEvent += new Action(((ContainerView) this).FireOpenBeginEvent);
        this.buttonClosed.OpenEndEvent += new Action<bool>(((ContainerView) this).FireOpenEndEvent);
      }
      if (!((UnityEngine.Object) this.storableView != (UnityEngine.Object) null))
        return;
      this.storableView.SelectEvent += new Action<IStorableComponent>(((ContainerView) this).FireItemSelectEventEvent);
      this.storableView.DeselectEvent += new Action<IStorableComponent>(((ContainerView) this).FireItemDeselectEventEvent);
      this.storableView.InteractEvent += new Action<IStorableComponent>(((ContainerView) this).FireItemInteractEventEvent);
    }

    private void UpdateStorable()
    {
      IStorableComponent storableComponent1 = (IStorableComponent) null;
      IStorageComponent storage = this.Container?.Storage;
      if (storage != null)
      {
        foreach (IStorableComponent storableComponent2 in storage.Items)
        {
          if (storableComponent2.Container == this.Container)
          {
            storableComponent1 = storableComponent2;
            break;
          }
        }
      }
      this.storableView.Storable = (StorableComponent) storableComponent1;
    }

    private void UpdateState()
    {
      bool flag = this.Container != null;
      if ((UnityEngine.Object) this.hideableView != (UnityEngine.Object) null)
        this.hideableView.Visible = flag;
      IParameter<ContainerOpenStateEnum> openState = this.Container?.OpenState;
      if ((UnityEngine.Object) this.hideableViewOpen != (UnityEngine.Object) null)
        this.hideableViewOpen.Visible = flag && (openState == null || openState.Value == ContainerOpenStateEnum.Open);
      if (!((UnityEngine.Object) this.hideableViewClosed != (UnityEngine.Object) null))
        return;
      this.hideableViewClosed.Visible = openState != null && openState.Value != ContainerOpenStateEnum.Open;
    }

    protected override void OnContainerSet(InventoryComponent previousContainer)
    {
      if (previousContainer != null)
      {
        if (previousContainer.Storage is StorageComponent storage)
        {
          storage.OnAddItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.OnItemsChange);
          storage.OnChangeItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.OnItemsChange);
          storage.OnRemoveItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.OnItemsChange);
        }
        if (previousContainer.OpenState != null)
          previousContainer.OpenState.RemoveListener((IChangeParameterListener) this);
      }
      if (this.Container != null)
      {
        if (this.Container.Storage is StorageComponent storage)
        {
          storage.OnAddItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.OnItemsChange);
          storage.OnChangeItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.OnItemsChange);
          storage.OnRemoveItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.OnItemsChange);
        }
        if (this.Container.OpenState != null)
          this.Container.OpenState.AddListener((IChangeParameterListener) this);
      }
      this.UpdateState();
      this.UpdateStorable();
    }

    private void OnItemsChange(IStorableComponent storable, IInventoryComponent container)
    {
      if (container != this.Container)
        return;
      this.UpdateStorable();
    }

    public override void SetCanOpen(bool canOpen)
    {
      if (!((UnityEngine.Object) this.buttonClosed != (UnityEngine.Object) null))
        return;
      this.buttonClosed.interactable = canOpen;
    }

    public void OnParameterChanged(IParameter parameter) => this.UpdateState();
  }
}
