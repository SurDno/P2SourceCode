using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableDeactivating : HideableView
  {
    [SerializeField]
    private GameObject target;

    protected override void ApplyVisibility()
    {
      if ((Object) this.target != (Object) null)
        this.target.SetActive(this.Visible);
      else
        this.gameObject.SetActive(this.Visible);
    }
  }
}
