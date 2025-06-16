using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressRotation : ProgressView
  {
    [SerializeField]
    private Vector3 minRotation = Vector3.zero;
    [SerializeField]
    private Vector3 maxRotation = Vector3.zero;

    protected override void ApplyProgress()
    {
      this.transform.localEulerAngles = Vector3.Lerp(this.minRotation, this.maxRotation, this.Progress);
    }

    public override void SkipAnimation()
    {
    }
  }
}
