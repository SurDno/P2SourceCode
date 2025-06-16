using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SplitEventView : EventView
  {
    [SerializeField]
    private EventView[] views = (EventView[]) null;

    public override void Invoke()
    {
      if (this.views == null)
        return;
      for (int index = 0; index < this.views.Length; ++index)
        this.views[index]?.Invoke();
    }
  }
}
