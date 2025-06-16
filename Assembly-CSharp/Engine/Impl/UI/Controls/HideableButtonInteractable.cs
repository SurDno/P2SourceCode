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
      if (!(button != null))
        return;
      button.interactable = Visible;
    }
  }
}
