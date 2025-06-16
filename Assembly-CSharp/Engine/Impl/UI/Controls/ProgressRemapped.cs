// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressRemapped
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressRemapped : ProgressView
  {
    [SerializeField]
    private ProgressViewBase nestedView;
    [SerializeField]
    private Vector2 targetRange = new Vector2(0.0f, 1f);

    protected override void ApplyProgress()
    {
      if (!((Object) this.nestedView != (Object) null))
        return;
      this.nestedView.Progress = Mathf.Lerp(this.targetRange.x, this.targetRange.y, this.Progress);
    }

    public void SetMin(float min)
    {
      this.targetRange.x = min;
      this.ApplyProgress();
    }

    public void SetMax(float max)
    {
      this.targetRange.y = max;
      this.ApplyProgress();
    }

    public override void SkipAnimation()
    {
      if (!((Object) this.nestedView != (Object) null))
        return;
      this.nestedView.SkipAnimation();
    }
  }
}
