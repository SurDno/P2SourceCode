using Engine.Impl.UI.Controls;

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
    if (!((Object) targetCamera != (Object) null))
      return;
    targetCamera.farClipPlane = Visible ? onDistance : offDistance;
  }
}
