using Engine.Impl.UI.Controls;

public class OcclusionCullingSettings : HideableView
{
  [SerializeField]
  private Camera targetCamera;

  protected override void ApplyVisibility()
  {
    if (!((Object) targetCamera != (Object) null))
      return;
    targetCamera.useOcclusionCulling = Visible;
  }
}
