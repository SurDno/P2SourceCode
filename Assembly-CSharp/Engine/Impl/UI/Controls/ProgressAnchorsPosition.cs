using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressAnchorsPosition : ProgressView
  {
    [SerializeField]
    private Transform minAnchor;
    [SerializeField]
    private Transform maxAnchor;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if (!(minAnchor != null) || !(maxAnchor != null))
        return;
      transform.position = Vector3.Lerp(minAnchor.position, maxAnchor.position, Progress);
    }

    private void LateUpdate() => ApplyProgress();
  }
}
