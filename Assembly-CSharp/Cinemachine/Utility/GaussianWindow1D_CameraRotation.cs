using UnityEngine;

namespace Cinemachine.Utility;

internal class GaussianWindow1D_CameraRotation : GaussianWindow1d<Vector2> {
	public GaussianWindow1D_CameraRotation(float sigma, int maxKernelRadius = 10)
		: base(sigma, maxKernelRadius) { }

	protected override Vector2 Compute(int windowPos) {
		var zero = Vector2.zero;
		var vector2_1 = mData[mCurrentPos];
		for (var index = 0; index < KernelSize; ++index) {
			var vector2_2 = mData[windowPos] - vector2_1;
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