// Decompiled with JetBrains decompiler
// Type: Cinemachine.Utility.GaussianWindow1D_CameraRotation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Cinemachine.Utility
{
  internal class GaussianWindow1D_CameraRotation : GaussianWindow1d<Vector2>
  {
    public GaussianWindow1D_CameraRotation(float sigma, int maxKernelRadius = 10)
      : base(sigma, maxKernelRadius)
    {
    }

    protected override Vector2 Compute(int windowPos)
    {
      Vector2 zero = Vector2.zero;
      Vector2 vector2_1 = this.mData[this.mCurrentPos];
      for (int index = 0; index < this.KernelSize; ++index)
      {
        Vector2 vector2_2 = this.mData[windowPos] - vector2_1;
        if ((double) vector2_2.y > 180.0)
          vector2_2.y -= 360f;
        if ((double) vector2_2.y < -180.0)
          vector2_2.y += 360f;
        zero += vector2_2 * this.mKernel[index];
        if (++windowPos == this.KernelSize)
          windowPos = 0;
      }
      return vector2_1 + zero / this.mKernelSum;
    }
  }
}
