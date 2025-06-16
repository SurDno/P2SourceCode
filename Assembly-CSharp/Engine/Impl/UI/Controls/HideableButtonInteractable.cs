using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  public class HideableButtonInteractable : HideableView
  {
    [SerializeField]
    private Button button;

    protected override void ApplyVisibility()
    {
      if (!((Object) this.button != (Object) null))
        return;
      this.button.interactable = this.Visible;
    }
  }
}
