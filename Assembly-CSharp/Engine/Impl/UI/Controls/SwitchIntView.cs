using System;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SwitchIntView : IntView
  {
    [SerializeField]
    private ValueViewPair[] views = [];

    protected override void ApplyIntValue()
    {
      foreach (ValueViewPair view in views)
      {
        if (view.hideableView != null)
          view.hideableView.Visible = IntValue == view.value;
      }
    }

    public override void SkipAnimation()
    {
      foreach (ValueViewPair view in views)
      {
        if (view.hideableView != null)
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
