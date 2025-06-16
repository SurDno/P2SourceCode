using System;
using UnityEngine;

namespace Cinemachine;

public class CinemachineBlend {
	public ICinemachineCamera CamA { get; set; }

	public ICinemachineCamera CamB { get; set; }

	public AnimationCurve BlendCurve { get; set; }

	public float TimeInBlend { get; set; }

	public float BlendWeight => BlendCurve != null ? BlendCurve.Evaluate(TimeInBlend) : 0.0f;

	public bool IsValid => CamA != null || CamB != null;

	public float Duration { get; set; }

	public bool IsComplete => TimeInBlend >= (double)Duration;

	public string Description {
		get {
			var str = CamA != null ? "[" + CamA.Name + "]" : "(none)";
			return string.Format("{0} {1}% from {2}", CamB != null ? "[" + CamB.Name + "]" : (object)"(none)",
				(int)(BlendWeight * 100.0), str);
		}
	}

	public bool Uses(ICinemachineCamera cam) {
		return cam == CamA || cam == CamB || (CamA is BlendSourceVirtualCamera camA && camA.Blend.Uses(cam)) ||
		       (CamB is BlendSourceVirtualCamera camB && camB.Blend.Uses(cam));
	}

	public CinemachineBlend(
		ICinemachineCamera a,
		ICinemachineCamera b,
		AnimationCurve curve,
		float duration,
		float t) {
		CamA = a != null && b != null ? a : throw new ArgumentException("Blend cameras cannot be null");
		CamB = b;
		BlendCurve = curve;
		TimeInBlend = t;
		Duration = duration;
	}

	public void UpdateCameraState(Vector3 worldUp, float deltaTime) {
		CinemachineCore.Instance.UpdateVirtualCamera(CamA, worldUp, deltaTime);
		CinemachineCore.Instance.UpdateVirtualCamera(CamB, worldUp, deltaTime);
	}

	public CameraState State => CameraState.Lerp(CamA.State, CamB.State, BlendWeight);
}