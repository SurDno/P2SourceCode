using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

public class ItemParameterView : ItemView
{
  [SerializeField]
  private ItemView nestedView;
  [SerializeField]
  private FloatParameterView parameterView;
  private StorableComponent storable;

  public override StorableComponent Storable
  {
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      this.nestedView.Storable = value;
      this.parameterView.Value = value?.Owner;
    }
  }
}
