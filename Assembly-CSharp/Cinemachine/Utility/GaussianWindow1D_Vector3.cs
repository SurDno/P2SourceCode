using UnityEngine;

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
