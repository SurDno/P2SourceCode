using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  public class GraphicColorView : SingleColorView
  {
    [SerializeField]
    private Graphic graphic;

    protected override void ApplyValue(bool instant)
    {
      if (!((Object) this.graphic != (Object) null))
        return;
      this.graphic.color = this.GetValue();
    }
  }
}
