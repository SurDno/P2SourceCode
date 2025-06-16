using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SumColorView : BinaryColorView
  {
    [SerializeField]
    private ColorViewHandle view;

    protected override void ApplyValues(bool instant)
    {
      this.view.SetValue(this.GetValue(0) + this.GetValue(1), instant);
    }
  }
}
