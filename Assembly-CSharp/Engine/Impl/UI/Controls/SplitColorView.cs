using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SplitColorView : SingleColorView
  {
    [SerializeField]
    private ColorViewHandle[] views;

    protected override void ApplyValue(bool instant)
    {
      if (this.views == null)
        return;
      for (int index = 0; index < this.views.Length; ++index)
        this.views[index].SetValue(this.GetValue(), instant);
    }
  }
}
