using UnityEngine;

namespace Cinemachine.Utility
{
  internal class GaussianWindow1D_CameraRotation(float sigma, int maxKernelRadius = 10)
    : GaussianWindow1d<Vector2>(sigma, maxKernelRadius) {
    protected override Vector2 Compute(int windowPos)
    {
      Vector2 zero = Vector2.zero;
      Vector2 vector2_1 = mData[mCurrentPos];
      for (int index = 0; index < KernelSize; ++index)
      {
        Vector2 vector2_2 = mData[windowPos] - vector2_1;
        if (vector2_2.y > 180.0)
          vector2_2.y -= 360f;
        if (vector2_2.y < -180.0)
          vector2_2.y += 360f;
        zero += vector2_2 * mKernel[index];
        if (++windowPos == KernelSize)
          windowPos = 0;
      }
      return vector2_1 + zero / mKernelSum;
    }
  }
}
