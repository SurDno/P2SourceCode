// Decompiled with JetBrains decompiler
// Type: RepairingCostView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Components;
using Engine.Source.Components.Repairing;
using Engine.Source.Components.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class RepairingCostView : MonoBehaviour
{
  [SerializeField]
  private List<ItemView> itemViews = new List<ItemView>();
  private IStorageComponent actor = (IStorageComponent) null;
  private List<RepairableCostItem> cost = (List<RepairableCostItem>) null;

  public IStorageComponent Actor
  {
    get => this.actor;
    set
    {
      if (this.actor == value)
        return;
      if (this.actor != null)
      {
        this.actor.OnAddItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.OnActorContentChange);
        this.actor.OnChangeItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.OnActorContentChange);
        this.actor.OnRemoveItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.OnActorContentChange);
      }
      this.actor = value;
      if (this.actor != null)
      {
        this.actor.OnAddItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.OnActorContentChange);
        this.actor.OnChangeItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.OnActorContentChange);
        this.actor.OnRemoveItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.OnActorContentChange);
      }
      this.Rebuild();
    }
  }

  public List<RepairableCostItem> Cost
  {
    get => this.cost;
    set
    {
      if (this.cost == value)
        return;
      this.cost = value;
      this.Rebuild();
    }
  }

  private void OnActorContentChange(IStorableComponent item, IInventoryComponent container)
  {
    this.Rebuild();
  }

  private void Rebuild()
  {
    if (this.Actor == null || this.Cost == null)
    {
      this.PrepareItemViews(0);
    }
    else
    {
      this.PrepareItemViews(this.Cost.Count);
      for (int index = 0; index < this.Cost.Count && index < this.itemViews.Count; ++index)
      {
        IEntity resource = this.Cost[index].Template.Value;
        if (resource == null)
        {
          this.itemViews[index].gameObject.SetActive(false);
        }
        else
        {
          this.itemViews[index].Storable = resource.GetComponent<StorableComponent>();
          Text componentInChildren = this.itemViews[index].GetComponentInChildren<Text>();
          if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
          {
            int count = this.Cost[index].Count;
            int itemAmount = StorageUtility.GetItemAmount(this.Actor.Items, resource);
            componentInChildren.text = Math.Min(count, itemAmount).ToString() + " / " + count.ToString();
          }
        }
      }
    }
  }

  private void PrepareItemViews(int count)
  {
    if (this.itemViews.Count == 0 || (UnityEngine.Object) this.itemViews[0] == (UnityEngine.Object) null)
      return;
    while (this.itemViews.Count < count)
    {
      ItemView itemView1 = this.itemViews[0];
      ItemView itemView2 = UnityEngine.Object.Instantiate<ItemView>(itemView1);
      itemView2.transform.SetParent(itemView1.transform.parent, false);
      this.itemViews.Add(itemView2);
    }
    for (int index = 0; index < this.itemViews.Count; ++index)
    {
      this.itemViews[index].gameObject.SetActive(index < count);
      this.itemViews[index].Storable = (StorableComponent) null;
    }
  }
}
