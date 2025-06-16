using Engine.Impl.UI.Controls;
using UnityEngine;

public class OcclusionCullingSettings : HideableView
{
  [SerializeField]
  private Camera targetCamera;

  protected override void ApplyVisibility()
  {
    if (!((Object) this.targetCamera != (Object) null))
      return;
    this.targetCamera.useOcclusionCulling = this.Visible;
  }
}
