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
      Vector2 vector2_1 = mData[mCurrentPos];
      for (int index = 0; index < KernelSize; ++index)
      {
        Vector2 vector2_2 = mData[windowPos] - vector2_1;
        if ((double) vector2_2.y > 180.0)
          vector2_2.y -= 360f;
        if ((double) vector2_2.y < -180.0)
          vector2_2.y += 360f;
        zero += vector2_2 * mKernel[index];
        if (++windowPos == KernelSize)
          windowPos = 0;
      }
      return vector2_1 + zero / mKernelSum;
    }
  }
}
