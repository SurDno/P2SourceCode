// Decompiled with JetBrains decompiler
// Type: EntityItemView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

#nullable disable
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
