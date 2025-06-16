using Engine.Impl.UI.Controls;
using Engine.Source.Components;

public class EntityItemView : ItemView
{
  [SerializeField]
  private EntityView view;

  public override StorableComponent Storable
  {
    get => view?.Value?.GetComponent<StorableComponent>();
    set
    {
      if (!((Object) view != (Object) null))
        return;
      view.Value = value?.Owner;
    }
  }
}
