// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.SwitchIntView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class SwitchIntView : IntView
  {
    [SerializeField]
    private SwitchIntView.ValueViewPair[] views = new SwitchIntView.ValueViewPair[0];

    protected override void ApplyIntValue()
    {
      foreach (SwitchIntView.ValueViewPair view in this.views)
      {
        if ((UnityEngine.Object) view.hideableView != (UnityEngine.Object) null)
          view.hideableView.Visible = this.IntValue == view.value;
      }
    }

    public override void SkipAnimation()
    {
      foreach (SwitchIntView.ValueViewPair view in this.views)
      {
        if ((UnityEngine.Object) view.hideableView != (UnityEngine.Object) null)
          view.hideableView.SkipAnimation();
      }
    }

    [Serializable]
    public struct ValueViewPair
    {
      public int value;
      public HideableView hideableView;
    }
  }
}
