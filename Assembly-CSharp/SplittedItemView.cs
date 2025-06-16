using System;
using Engine.Common.Components;
using Engine.Source.Components;

public class SplittedItemView : ItemView
{
  [SerializeField]
  private ItemView[] nestedViews = new ItemView[0];
  private StorableComponent storable;

  public override StorableComponent Storable
  {
    get => storable;
    set
    {
      if (storable == value)
        return;
      storable = value;
      if (nestedViews == null)
        return;
      foreach (ItemView nestedView in nestedViews)
      {
        if ((UnityEngine.Object) nestedView != (UnityEngine.Object) null)
          nestedView.Storable = storable;
      }
    }
  }

  public override void SkipAnimation()
  {
    foreach (ItemView nestedView in nestedViews)
      nestedView?.SkipAnimation();
  }

  private void Start()
  {
    foreach (ItemView nestedView in nestedViews)
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
