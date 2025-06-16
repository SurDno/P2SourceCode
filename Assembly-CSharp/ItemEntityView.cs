using Engine.Common;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

public class ItemEntityView : EntityView
{
  [SerializeField]
  private ItemView view;
  private IEntity entity;

  public override IEntity Value
  {
    get => this.entity;
    set
    {
      if (this.entity == value)
        return;
      this.entity = value;
      if (!((Object) this.view != (Object) null))
        return;
      this.view.Storable = this.entity?.GetComponent<StorableComponent>();
    }
  }
}
