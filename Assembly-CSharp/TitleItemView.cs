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
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      if (!((Object) this.view != (Object) null))
        return;
      this.view.StringValue = ServiceLocator.GetService<LocalizationService>().GetText(this.storable != null ? this.storable.Title : LocalizedText.Empty);
    }
  }
}
