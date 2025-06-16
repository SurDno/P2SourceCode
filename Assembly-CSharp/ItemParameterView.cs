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
    get => storable;
    set
    {
      if (storable == value)
        return;
      storable = value;
      nestedView.Storable = value;
      parameterView.Value = value?.Owner;
    }
  }
}
