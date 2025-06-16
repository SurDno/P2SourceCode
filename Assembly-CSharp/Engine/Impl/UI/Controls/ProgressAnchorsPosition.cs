namespace Engine.Impl.UI.Controls
{
  public class ProgressAnchorsPosition : ProgressView
  {
    [SerializeField]
    private Transform minAnchor = (Transform) null;
    [SerializeField]
    private Transform maxAnchor = (Transform) null;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if (!((Object) minAnchor != (Object) null) || !((Object) maxAnchor != (Object) null))
        return;
      this.transform.position = Vector3.Lerp(minAnchor.position, maxAnchor.position, Progress);
    }

    private void LateUpdate() => ApplyProgress();
  }
}
