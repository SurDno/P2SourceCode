// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressScale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressScale : ProgressView
  {
    [SerializeField]
    private Vector3 minScale = Vector3.one;
    [SerializeField]
    private Vector3 maxScale = Vector3.one;

    protected override void ApplyProgress()
    {
      this.transform.localScale = Vector3.Lerp(this.minScale, this.maxScale, this.Progress);
    }

    public override void SkipAnimation()
    {
    }
  }
}
