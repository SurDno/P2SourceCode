// Decompiled with JetBrains decompiler
// Type: SplittedItemView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Source.Components;
using System;
using UnityEngine;

#nullable disable
public class SplittedItemView : ItemView
{
  [SerializeField]
  private ItemView[] nestedViews = new ItemView[0];
  private StorableComponent storable;

  public override StorableComponent Storable
  {
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      if (this.nestedViews == null)
        return;
      foreach (ItemView nestedView in this.nestedViews)
      {
        if ((UnityEngine.Object) nestedView != (UnityEngine.Object) null)
          nestedView.Storable = this.storable;
      }
    }
  }

  public override void SkipAnimation()
  {
    foreach (ItemView nestedView in this.nestedViews)
      nestedView?.SkipAnimation();
  }

  private void Start()
  {
    foreach (ItemView nestedView in this.nestedViews)
    {
      if (!((UnityEngine.Object) nestedView == (UnityEngine.Object) null))
      {
        nestedView.DeselectEvent += new Action<IStorableComponent>(((ItemView) this).FireDeselectEvent);
        nestedView.SelectEvent += new Action<IStorableComponent>(((ItemView) this).FireSelectEvent);
        nestedView.InteractEvent += new Action<IStorableComponent>(((ItemView) this).FireInteractEvent);
      }
    }
  }
}
