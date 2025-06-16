using System;
using UnityEngine;

namespace Cinemachine.Utility;

internal abstract class GaussianWindow1d<T> {
	protected T[] mData;
	protected float[] mKernel;
	protected float mKernelSum;
	protected int mCurrentPos;

	public float Sigma { get; private set; }

	public int KernelSize => mKernel.Length;

	private void GenerateKernel(float sigma, int maxKernelRadius) {
		var num = Math.Min(maxKernelRadius, Mathf.FloorToInt(Mathf.Abs(sigma) * 2.5f));
		mKernel = new float[2 * num + 1];
		mKernelSum = 0.0f;
		if (num == 0)
			mKernelSum = mKernel[0] = 1f;
		else
			for (var index = -num; index <= num; ++index) {
				mKernel[index + num] = (float)(Math.Exp(-(index * index) / (2.0 * sigma * sigma)) /
				                               Math.Sqrt(2.0 * Math.PI * sigma));
				mKernelSum += mKernel[index + num];
			}

		Sigma = sigma;
	}

	protected abstract T Compute(int windowPos);

	public GaussianWindow1d(float sigma, int maxKernelRadius = 10) {
		GenerateKernel(sigma, maxKernelRadius);
		mCurrentPos = 0;
	}

	public void Reset() {
		mData = null;
	}

	public bool IsEmpty() {
		return mData == null;
	}

	public void AddValue(T v) {
		if (mData == null) {
			mData = new T[KernelSize];
			for (var index = 0; index < KernelSize; ++index)
				mData[index] = v;
			mCurrentPos = Mathf.Min(1, KernelSize - 1);
		}

		mData[mCurrentPos] = v;
		if (++mCurrentPos != KernelSize)
			return;
		mCurrentPos = 0;
	}

	public T Filter(T v) {
		if (KernelSize < 3)
			return v;
		AddValue(v);
		return Value();
	}

	public T Value() {
		return Compute(mCurrentPos);
	}
}