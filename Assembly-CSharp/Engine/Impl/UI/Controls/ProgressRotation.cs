// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressRotation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressRotation : ProgressView
  {
    [SerializeField]
    private Vector3 minRotation = Vector3.zero;
    [SerializeField]
    private Vector3 maxRotation = Vector3.zero;

    protected override void ApplyProgress()
    {
      this.transform.localEulerAngles = Vector3.Lerp(this.minRotation, this.maxRotation, this.Progress);
    }

    public override void SkipAnimation()
    {
    }
  }
}
