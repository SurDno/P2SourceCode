// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressAnchorsPosition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressAnchorsPosition : ProgressView
  {
    [SerializeField]
    private Transform minAnchor = (Transform) null;
    [SerializeField]
    private Transform maxAnchor = (Transform) null;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if (!((Object) this.minAnchor != (Object) null) || !((Object) this.maxAnchor != (Object) null))
        return;
      this.transform.position = Vector3.Lerp(this.minAnchor.position, this.maxAnchor.position, this.Progress);
    }

    private void LateUpdate() => this.ApplyProgress();
  }
}
