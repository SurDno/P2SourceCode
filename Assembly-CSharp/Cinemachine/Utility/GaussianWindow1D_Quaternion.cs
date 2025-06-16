using UnityEngine;

namespace Cinemachine.Utility;

internal class GaussianWindow1D_Quaternion : GaussianWindow1d<Quaternion> {
	public GaussianWindow1D_Quaternion(float sigma, int maxKernelRadius = 10)
		: base(sigma, maxKernelRadius) { }

	protected override Quaternion Compute(int windowPos) {
		var quaternion1 = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
		var rotation = mData[mCurrentPos];
		var quaternion2 = Quaternion.Inverse(rotation);
		for (var index = 0; index < KernelSize; ++index) {
			var num = mKernel[index];
			var b = quaternion2 * mData[windowPos];
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