// Decompiled with JetBrains decompiler
// Type: TitleItemView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

#nullable disable
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
