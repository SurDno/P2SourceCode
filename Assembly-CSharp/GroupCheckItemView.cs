using System.Linq;
using Engine.Common.Components.Storable;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;

public class GroupCheckItemView : ItemView
{
  [SerializeField]
  private HideableView hideableView;
  [SerializeField]
  private StorableGroup[] groups = new StorableGroup[0];
  private StorableComponent storable;

  public override StorableComponent Storable
  {
    get => storable;
    set
    {
      if (storable == value)
        return;
      storable = value;
      if ((Object) hideableView == (Object) null)
        return;
      if (storable != null)
      {
        foreach (StorableGroup group in groups)
        {
          if (storable.Groups.Contains(group))
          {
            hideableView.Visible = true;
            return;
          }
        }
      }
      hideableView.Visible = false;
    }
  }

  public override void SkipAnimation() => hideableView?.SkipAnimation();
}
