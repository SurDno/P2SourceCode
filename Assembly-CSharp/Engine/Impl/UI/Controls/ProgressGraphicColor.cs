using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  public class ProgressGraphicColor : ProgressView
  {
    [SerializeField]
    private Graphic graphic;
    [SerializeField]
    private Color minColor = Color.white;
    [SerializeField]
    private Color maxColor = Color.white;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if (!((Object) this.graphic != (Object) null))
        return;
      this.graphic.color = Color.Lerp(this.minColor, this.maxColor, this.Progress);
    }
  }
}
