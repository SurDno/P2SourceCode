using System;
using UnityEngine;

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
