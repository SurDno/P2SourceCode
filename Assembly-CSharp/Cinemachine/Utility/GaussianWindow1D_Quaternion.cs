// Decompiled with JetBrains decompiler
// Type: Cinemachine.Utility.GaussianWindow1D_Quaternion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Cinemachine.Utility
{
  internal class GaussianWindow1D_Quaternion : GaussianWindow1d<Quaternion>
  {
    public GaussianWindow1D_Quaternion(float sigma, int maxKernelRadius = 10)
      : base(sigma, maxKernelRadius)
    {
    }

    protected override Quaternion Compute(int windowPos)
    {
      Quaternion quaternion1 = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
      Quaternion rotation = this.mData[this.mCurrentPos];
      Quaternion quaternion2 = Quaternion.Inverse(rotation);
      for (int index = 0; index < this.KernelSize; ++index)
      {
        float num = this.mKernel[index];
        Quaternion b = quaternion2 * this.mData[windowPos];
        if ((double) Quaternion.Dot(Quaternion.identity, b) < 0.0)
          num = -num;
        quaternion1.x += b.x * num;
        quaternion1.y += b.y * num;
        quaternion1.z += b.z * num;
        quaternion1.w += b.w * num;
        if (++windowPos == this.KernelSize)
          windowPos = 0;
      }
      return rotation * quaternion1;
    }
  }
}
