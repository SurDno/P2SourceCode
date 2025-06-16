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
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      if (!((Object) this.hideableView != (Object) null))
        return;
      this.hideableView.Visible = this.storable != null;
    }
  }

  public override void SkipAnimation() => this.hideableView?.SkipAnimation();
}
