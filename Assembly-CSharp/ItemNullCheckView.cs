using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

public class ItemNullCheckView : ItemView
{
  [SerializeField]
  private HideableView hideableView;
  private StorableComponent storable;

  public override StorableComponent Storable
  {
    get => storable;
    set
    {
      if (storable == value)
        return;
      storable = value;
      if (!(hideableView != null))
        return;
      hideableView.Visible = storable != null;
    }
  }

  public override void SkipAnimation() => hideableView?.SkipAnimation();
}
