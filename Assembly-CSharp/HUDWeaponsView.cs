using Engine.Common;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using System;
using UnityEngine;

public class HUDWeaponsView : EntityView
{
  [SerializeField]
  private EntityView currentItemView;
  [SerializeField]
  private EntityView[] availableItemViews;
  [SerializeField]
  private HideableView[] availableItemSelectionViews;
  [SerializeField]
  protected HideableEventView changeEventView;
  private IEntity entity;
  private AttackerPlayerComponent attacker;

  public override IEntity Value
  {
    get => this.entity;
    set
    {
      if (this.entity == value)
        return;
      this.entity = value;
      if (this.attacker != null)
      {
        this.attacker.AvailableWeaponItemsChangeEvent -= new Action(this.AssignAvailableItems);
        this.attacker.CurrentWeaponUnholstered -= new Action(this.AssignCurrentItem);
      }
      this.attacker = this.entity?.GetComponent<AttackerPlayerComponent>();
      if (this.attacker != null)
      {
        this.AssignAvailableItems();
        this.attacker.AvailableWeaponItemsChangeEvent += new Action(this.AssignAvailableItems);
        this.attacker.CurrentWeaponUnholstered += new Action(this.AssignCurrentItem);
      }
      else
        this.ClearAvailableItems();
    }
  }

  public AttackerPlayerComponent Attacker => this.attacker;

  public void Hide() => this.changeEventView.Hide();

  public void Show()
  {
    this.changeEventView.Show();
    this.AssignCurrentItem();
  }

  private void AssignAvailableItems()
  {
    int index = 0;
    foreach (IEntity availableWeaponItem in this.attacker.AvailableWeaponItems)
    {
      this.availableItemViews[index].Value = availableWeaponItem;
      ++index;
      if (index >= this.availableItemViews.Length)
        break;
    }
    this.ClearCurrentItem();
  }

  public virtual void AssignCurrentItem()
  {
    if (this.attacker.CurrentWeaponItem == null)
    {
      this.ClearCurrentItem();
    }
    else
    {
      this.currentItemView.Value = this.attacker.CurrentWeaponItem;
      for (int index = 0; index < this.availableItemViews.Length; ++index)
        this.availableItemSelectionViews[index].Visible = this.availableItemViews[index].Value == this.currentItemView.Value;
    }
  }

  private void ClearAvailableItems()
  {
    for (int index = 0; index < this.availableItemViews.Length; ++index)
      this.availableItemViews[index].Value = (IEntity) null;
    this.ClearCurrentItem();
  }

  private void ClearCurrentItem()
  {
    this.currentItemView.Value = (IEntity) null;
    for (int index = 0; index < this.availableItemViews.Length; ++index)
      this.availableItemSelectionViews[index].Visible = false;
  }
}
