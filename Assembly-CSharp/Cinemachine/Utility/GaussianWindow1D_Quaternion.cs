﻿using UnityEngine;

namespace Cinemachine.Utility
{
  internal class GaussianWindow1D_Quaternion(float sigma, int maxKernelRadius = 10)
    : GaussianWindow1d<Quaternion>(sigma, maxKernelRadius) {
    protected override Quaternion Compute(int windowPos)
    {
      Quaternion quaternion1 = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
      Quaternion rotation = mData[mCurrentPos];
      Quaternion quaternion2 = Quaternion.Inverse(rotation);
      for (int index = 0; index < KernelSize; ++index)
      {
        float num = mKernel[index];
        Quaternion b = quaternion2 * mData[windowPos];
        if (Quaternion.Dot(Quaternion.identity, b) < 0.0)
          num = -num;
        quaternion1.x += b.x * num;
        quaternion1.y += b.y * num;
        quaternion1.z += b.z * num;
        quaternion1.w += b.w * num;
        if (++windowPos == KernelSize)
          windowPos = 0;
      }
      return rotation * quaternion1;
    }
  }
}
