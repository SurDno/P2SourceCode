using System;

namespace Engine.Impl.UI.Controls
{
  public class SwitchIntView : IntView
  {
    [SerializeField]
    private ValueViewPair[] views = new ValueViewPair[0];

    protected override void ApplyIntValue()
    {
      foreach (ValueViewPair view in views)
      {
        if ((UnityEngine.Object) view.hideableView != (UnityEngine.Object) null)
          view.hideableView.Visible = IntValue == view.value;
      }
    }

    public override void SkipAnimation()
    {
      foreach (ValueViewPair view in views)
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
