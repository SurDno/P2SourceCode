using Engine.Common.Components;
using Engine.Source.Components;
using System;
using UnityEngine;

public class SplittedItemView : ItemView
{
  [SerializeField]
  private ItemView[] nestedViews = new ItemView[0];
  private StorableComponent storable;

  public override StorableComponent Storable
  {
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      if (this.nestedViews == null)
        return;
      foreach (ItemView nestedView in this.nestedViews)
      {
        if ((UnityEngine.Object) nestedView != (UnityEngine.Object) null)
          nestedView.Storable = this.storable;
      }
    }
  }

  public override void SkipAnimation()
  {
    foreach (ItemView nestedView in this.nestedViews)
      nestedView?.SkipAnimation();
  }

  private void Start()
  {
    foreach (ItemView nestedView in this.nestedViews)
    {
      if (!((UnityEngine.Object) nestedView == (UnityEngine.Object) null))
      {
        nestedView.DeselectEvent += new Action<IStorableComponent>(((ItemView) this).FireDeselectEvent);
        nestedView.SelectEvent += new Action<IStorableComponent>(((ItemView) this).FireSelectEvent);
        nestedView.InteractEvent += new Action<IStorableComponent>(((ItemView) this).FireInteractEvent);
      }
    }
  }
}
