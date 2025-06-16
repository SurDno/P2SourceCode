using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableEnabling : HideableView
  {
    [SerializeField]
    private Behaviour component;

    protected override void ApplyVisibility()
    {
      if (!((Object) this.component != (Object) null))
        return;
      this.component.enabled = this.Visible;
    }
  }
}
