using System;

namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/SRText")]
  public class SRText : Text
  {
    public event Action<SRText> LayoutDirty;

    public override void SetLayoutDirty()
    {
      base.SetLayoutDirty();
      Action<SRText> layoutDirty = LayoutDirty;
      if (layoutDirty == null)
        return;
      layoutDirty(this);
    }
  }
}
