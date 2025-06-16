using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Components;
using Engine.Source.Components.Repairing;
using Engine.Source.Components.Utilities;

public class RepairingCostView : MonoBehaviour
{
  [SerializeField]
  private List<ItemView> itemViews = new List<ItemView>();
  private IStorageComponent actor;
  private List<RepairableCostItem> cost;

  public IStorageComponent Actor
  {
    get => actor;
    set
    {
      if (actor == value)
        return;
      if (actor != null)
      {
        actor.OnAddItemEvent -= OnActorContentChange;
        actor.OnChangeItemEvent -= OnActorContentChange;
        actor.OnRemoveItemEvent -= OnActorContentChange;
      }
      actor = value;
      if (actor != null)
      {
        actor.OnAddItemEvent += OnActorContentChange;
        actor.OnChangeItemEvent += OnActorContentChange;
        actor.OnRemoveItemEvent += OnActorContentChange;
      }
      Rebuild();
    }
  }

  public List<RepairableCostItem> Cost
  {
    get => cost;
    set
    {
      if (cost == value)
        return;
      cost = value;
      Rebuild();
    }
  }

  private void OnActorContentChange(IStorableComponent item, IInventoryComponent container)
  {
    Rebuild();
  }

  private void Rebuild()
  {
    if (Actor == null || Cost == null)
    {
      PrepareItemViews(0);
    }
    else
    {
      PrepareItemViews(Cost.Count);
      for (int index = 0; index < Cost.Count && index < itemViews.Count; ++index)
      {
        IEntity resource = Cost[index].Template.Value;
        if (resource == null)
        {
          itemViews[index].gameObject.SetActive(false);
        }
        else
        {
          itemViews[index].Storable = resource.GetComponent<StorableComponent>();
          Text componentInChildren = itemViews[index].GetComponentInChildren<Text>();
          if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
          {
            int count = Cost[index].Count;
            int itemAmount = StorageUtility.GetItemAmount(Actor.Items, resource);
            componentInChildren.text = Math.Min(count, itemAmount) + " / " + count;
          }
        }
      }
    }
  }

  private void PrepareItemViews(int count)
  {
    if (itemViews.Count == 0 || (UnityEngine.Object) itemViews[0] == (UnityEngine.Object) null)
      return;
    while (itemViews.Count < count)
    {
      ItemView itemView1 = itemViews[0];
      ItemView itemView2 = UnityEngine.Object.Instantiate<ItemView>(itemView1);
      itemView2.transform.SetParent(itemView1.transform.parent, false);
      itemViews.Add(itemView2);
    }
    for (int index = 0; index < itemViews.Count; ++index)
    {
      itemViews[index].gameObject.SetActive(index < count);
      itemViews[index].Storable = null;
    }
  }
}
