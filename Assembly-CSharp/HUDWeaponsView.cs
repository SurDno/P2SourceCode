using Engine.Common;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
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
    get => entity;
    set
    {
      if (entity == value)
        return;
      entity = value;
      if (attacker != null)
      {
        attacker.AvailableWeaponItemsChangeEvent -= AssignAvailableItems;
        attacker.CurrentWeaponUnholstered -= AssignCurrentItem;
      }
      attacker = entity?.GetComponent<AttackerPlayerComponent>();
      if (attacker != null)
      {
        AssignAvailableItems();
        attacker.AvailableWeaponItemsChangeEvent += AssignAvailableItems;
        attacker.CurrentWeaponUnholstered += AssignCurrentItem;
      }
      else
        ClearAvailableItems();
    }
  }

  public AttackerPlayerComponent Attacker => attacker;

  public void Hide() => changeEventView.Hide();

  public void Show()
  {
    changeEventView.Show();
    AssignCurrentItem();
  }

  private void AssignAvailableItems()
  {
    int index = 0;
    foreach (IEntity availableWeaponItem in attacker.AvailableWeaponItems)
    {
      availableItemViews[index].Value = availableWeaponItem;
      ++index;
      if (index >= availableItemViews.Length)
        break;
    }
    ClearCurrentItem();
  }

  public virtual void AssignCurrentItem()
  {
    if (attacker.CurrentWeaponItem == null)
    {
      ClearCurrentItem();
    }
    else
    {
      currentItemView.Value = attacker.CurrentWeaponItem;
      for (int index = 0; index < availableItemViews.Length; ++index)
        availableItemSelectionViews[index].Visible = availableItemViews[index].Value == currentItemView.Value;
    }
  }

  private void ClearAvailableItems()
  {
    for (int index = 0; index < availableItemViews.Length; ++index)
      availableItemViews[index].Value = null;
    ClearCurrentItem();
  }

  private void ClearCurrentItem()
  {
    currentItemView.Value = null;
    for (int index = 0; index < availableItemViews.Length; ++index)
      availableItemSelectionViews[index].Visible = false;
  }
}
