using SRF;
using UnityEngine;

namespace SRDebugger.UI.Other;

public class LoadingSpinnerBehaviour : SRMonoBehaviour {
	private float _dt;
	public int FrameCount = 12;
	public float SpinDuration = 0.8f;

	private void Update() {
		_dt += Time.unscaledDeltaTime;
		var eulerAngles = CachedTransform.localRotation.eulerAngles;
		var z = eulerAngles.z;
		var num = SpinDuration / FrameCount;
		var flag = false;
		while (_dt > (double)num) {
			z -= 360f / FrameCount;
			_dt -= num;
			flag = true;
		}

		if (!flag)
			return;
		CachedTransform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, z);
	}
}