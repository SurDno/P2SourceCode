// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.Other.LoadingSpinnerBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRF;
using UnityEngine;

#nullable disable
namespace SRDebugger.UI.Other
{
  public class LoadingSpinnerBehaviour : SRMonoBehaviour
  {
    private float _dt;
    public int FrameCount = 12;
    public float SpinDuration = 0.8f;

    private void Update()
    {
      this._dt += Time.unscaledDeltaTime;
      Vector3 eulerAngles = this.CachedTransform.localRotation.eulerAngles;
      float z = eulerAngles.z;
      float num = this.SpinDuration / (float) this.FrameCount;
      bool flag = false;
      while ((double) this._dt > (double) num)
      {
        z -= 360f / (float) this.FrameCount;
        this._dt -= num;
        flag = true;
      }
      if (!flag)
        return;
      this.CachedTransform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, z);
    }
  }
}
