using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SplitEventView : EventView
  {
    [SerializeField]
    private EventView[] views;

    public override void Invoke()
    {
      if (views == null)
        return;
      for (int index = 0; index < views.Length; ++index)
        views[index]?.Invoke();
    }
  }
}
