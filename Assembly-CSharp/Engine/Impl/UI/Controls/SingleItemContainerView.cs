using System;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Source.Components;
using Engine.Source.UI.Controls;
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
      if (buttonClosed != null)
      {
        buttonClosed.SelectEvent += FireSelectEvent;
        buttonClosed.DeselectEvent += FireDeselectEvent;
        buttonClosed.OpenBeginEvent += FireOpenBeginEvent;
        buttonClosed.OpenEndEvent += FireOpenEndEvent;
      }
      if (!(storableView != null))
        return;
      storableView.SelectEvent += FireItemSelectEventEvent;
      storableView.DeselectEvent += FireItemDeselectEventEvent;
      storableView.InteractEvent += FireItemInteractEventEvent;
    }

    private void UpdateStorable()
    {
      IStorableComponent storableComponent1 = null;
      IStorageComponent storage = Container?.Storage;
      if (storage != null)
      {
        foreach (IStorableComponent storableComponent2 in storage.Items)
        {
          if (storableComponent2.Container == Container)
          {
            storableComponent1 = storableComponent2;
            break;
          }
        }
      }
      storableView.Storable = (StorableComponent) storableComponent1;
    }

    private void UpdateState()
    {
      bool flag = Container != null;
      if (hideableView != null)
        hideableView.Visible = flag;
      IParameter<ContainerOpenStateEnum> openState = Container?.OpenState;
      if (hideableViewOpen != null)
        hideableViewOpen.Visible = flag && (openState == null || openState.Value == ContainerOpenStateEnum.Open);
      if (!(hideableViewClosed != null))
        return;
      hideableViewClosed.Visible = openState != null && openState.Value != ContainerOpenStateEnum.Open;
    }

    protected override void OnContainerSet(InventoryComponent previousContainer)
    {
      if (previousContainer != null)
      {
        if (previousContainer.Storage is StorageComponent storage)
        {
          storage.OnAddItemEvent -= OnItemsChange;
          storage.OnChangeItemEvent -= OnItemsChange;
          storage.OnRemoveItemEvent -= OnItemsChange;
        }
        if (previousContainer.OpenState != null)
          previousContainer.OpenState.RemoveListener(this);
      }
      if (Container != null)
      {
        if (Container.Storage is StorageComponent storage)
        {
          storage.OnAddItemEvent += OnItemsChange;
          storage.OnChangeItemEvent += OnItemsChange;
          storage.OnRemoveItemEvent += OnItemsChange;
        }
        if (Container.OpenState != null)
          Container.OpenState.AddListener(this);
      }
      UpdateState();
      UpdateStorable();
    }

    private void OnItemsChange(IStorableComponent storable, IInventoryComponent container)
    {
      if (container != Container)
        return;
      UpdateStorable();
    }

    public override void SetCanOpen(bool canOpen)
    {
      if (!(buttonClosed != null))
        return;
      buttonClosed.interactable = canOpen;
    }

    public void OnParameterChanged(IParameter parameter) => UpdateState();
  }
}
