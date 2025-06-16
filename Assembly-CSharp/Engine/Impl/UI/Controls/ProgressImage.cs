using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  public class ProgressImage : ProgressView
  {
    [SerializeField]
    private Image image;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if (!((Object) this.image != (Object) null))
        return;
      this.image.fillAmount = this.Progress;
    }
  }
}
