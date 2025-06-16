using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SplitColorView : SingleColorView
  {
    [SerializeField]
    private ColorViewHandle[] views;

    protected override void ApplyValue(bool instant)
    {
      if (views == null)
        return;
      for (int index = 0; index < views.Length; ++index)
        views[index].SetValue(GetValue(), instant);
    }
  }
}
