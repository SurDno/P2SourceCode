using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

public class EntityItemView : ItemView
{
  [SerializeField]
  private EntityView view;

  public override StorableComponent Storable
  {
    get => this.view?.Value?.GetComponent<StorableComponent>();
    set
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.Value = value?.Owner;
    }
  }
}
