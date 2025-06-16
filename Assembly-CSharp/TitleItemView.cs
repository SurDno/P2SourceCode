using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

public class TitleItemView : ItemView
{
  [SerializeField]
  private StringView view;
  private StorableComponent storable;

  public override StorableComponent Storable
  {
    get => storable;
    set
    {
      if (storable == value)
        return;
      storable = value;
      if (!(view != null))
        return;
      view.StringValue = ServiceLocator.GetService<LocalizationService>().GetText(storable != null ? storable.Title : LocalizedText.Empty);
    }
  }
}
