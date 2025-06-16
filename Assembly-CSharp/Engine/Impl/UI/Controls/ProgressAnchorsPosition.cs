using UnityEngine;

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
      if (!((Object) this.minAnchor != (Object) null) || !((Object) this.maxAnchor != (Object) null))
        return;
      this.transform.position = Vector3.Lerp(this.minAnchor.position, this.maxAnchor.position, this.Progress);
    }

    private void LateUpdate() => this.ApplyProgress();
  }
}
