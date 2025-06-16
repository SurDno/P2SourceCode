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
      this.transform.localScale = Vector3.Lerp(this.minScale, this.maxScale, this.Progress);
    }

    public override void SkipAnimation()
    {
    }
  }
}
