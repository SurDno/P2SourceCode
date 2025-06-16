using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressScale : ProgressView
  {
    [SerializeField]
    private Vector3 minScale = Vector3.one;
    [SerializeField]
    private Vector3 maxScale = Vector3.one;

    protected override void ApplyProgress()
    {
      transform.localScale = Vector3.Lerp(minScale, maxScale, Progress);
    }

    public override void SkipAnimation()
    {
    }
  }
}
