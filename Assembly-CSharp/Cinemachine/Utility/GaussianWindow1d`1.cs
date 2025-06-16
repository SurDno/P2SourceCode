// Decompiled with JetBrains decompiler
// Type: Cinemachine.Utility.GaussianWindow1d`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Cinemachine.Utility
{
  internal abstract class GaussianWindow1d<T>
  {
    protected T[] mData;
    protected float[] mKernel;
    protected float mKernelSum;
    protected int mCurrentPos;

    public float Sigma { get; private set; }

    public int KernelSize => this.mKernel.Length;

    private void GenerateKernel(float sigma, int maxKernelRadius)
    {
      int num = Math.Min(maxKernelRadius, Mathf.FloorToInt(Mathf.Abs(sigma) * 2.5f));
      this.mKernel = new float[2 * num + 1];
      this.mKernelSum = 0.0f;
      if (num == 0)
      {
        this.mKernelSum = this.mKernel[0] = 1f;
      }
      else
      {
        for (int index = -num; index <= num; ++index)
        {
          this.mKernel[index + num] = (float) (Math.Exp((double) -(index * index) / (2.0 * (double) sigma * (double) sigma)) / Math.Sqrt(2.0 * Math.PI * (double) sigma));
          this.mKernelSum += this.mKernel[index + num];
        }
      }
      this.Sigma = sigma;
    }

    protected abstract T Compute(int windowPos);

    public GaussianWindow1d(float sigma, int maxKernelRadius = 10)
    {
      this.GenerateKernel(sigma, maxKernelRadius);
      this.mCurrentPos = 0;
    }

    public void Reset() => this.mData = (T[]) null;

    public bool IsEmpty() => this.mData == null;

    public void AddValue(T v)
    {
      if (this.mData == null)
      {
        this.mData = new T[this.KernelSize];
        for (int index = 0; index < this.KernelSize; ++index)
          this.mData[index] = v;
        this.mCurrentPos = Mathf.Min(1, this.KernelSize - 1);
      }
      this.mData[this.mCurrentPos] = v;
      if (++this.mCurrentPos != this.KernelSize)
        return;
      this.mCurrentPos = 0;
    }

    public T Filter(T v)
    {
      if (this.KernelSize < 3)
        return v;
      this.AddValue(v);
      return this.Value();
    }

    public T Value() => this.Compute(this.mCurrentPos);
  }
}
