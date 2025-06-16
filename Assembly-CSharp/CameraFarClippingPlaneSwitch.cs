using Engine.Impl.UI.Controls;
using UnityEngine;

public class CameraFarClippingPlaneSwitch : HideableView
{
  [SerializeField]
  private Camera targetCamera;
  [SerializeField]
  private float offDistance;
  [SerializeField]
  private float onDistance;

  protected override void ApplyVisibility()
  {
    if (!((Object) this.targetCamera != (Object) null))
      return;
    this.targetCamera.farClipPlane = this.Visible ? this.onDistance : this.offDistance;
  }
}
