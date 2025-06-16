// Decompiled with JetBrains decompiler
// Type: ItemParameterView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

#nullable disable
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
