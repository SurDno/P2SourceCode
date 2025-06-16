// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressMinLayoutHeight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
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
