using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  public class ProgressMinLayoutHeight : ProgressView
  {
    [SerializeField]
    private LayoutElement element;
    [SerializeField]
    private float min = 0.0f;
    [SerializeField]
    private float max = 0.0f;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if (!((Object) this.element != (Object) null))
        return;
      this.element.minHeight = Mathf.Lerp(this.min, this.max, this.Progress);
    }
  }
}
