// Decompiled with JetBrains decompiler
// Type: Cinemachine.Utility.GaussianWindow1D_Vector3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Cinemachine.Utility
{
  internal class GaussianWindow1D_Vector3 : GaussianWindow1d<Vector3>
  {
    public GaussianWindow1D_Vector3(float sigma, int maxKernelRadius = 10)
      : base(sigma, maxKernelRadius)
    {
    }

    protected override Vector3 Compute(int windowPos)
    {
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < this.KernelSize; ++index)
      {
        zero += this.mData[windowPos] * this.mKernel[index];
        if (++windowPos == this.KernelSize)
          windowPos = 0;
      }
      return zero / this.mKernelSum;
    }
  }
}
