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
      if (!(image != null))
        return;
      image.fillAmount = Progress;
    }
  }
}
