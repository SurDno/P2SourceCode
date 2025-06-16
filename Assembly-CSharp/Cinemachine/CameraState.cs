using System;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cinemachine;

public struct CameraState {
	public static Vector3 kNoPoint = new(float.NaN, float.NaN, float.NaN);
	private CustomBlendable mCustom0;
	private CustomBlendable mCustom1;
	private CustomBlendable mCustom2;
	private CustomBlendable mCustom3;
	private List<CustomBlendable> m_CustomOverflow;

	public LensSettings Lens { get; set; }

	public Vector3 ReferenceUp { get; set; }

	public Vector3 ReferenceLookAt { get; set; }

	public bool HasLookAt => ReferenceLookAt == ReferenceLookAt;

	public Vector3 RawPosition { get; set; }

	public Quaternion RawOrientation { get; set; }

	internal Vector3 PositionDampingBypass { get; set; }

	public float ShotQuality { get; set; }

	public Vector3 PositionCorrection { get; set; }

	public Quaternion OrientationCorrection { get; set; }

	public Vector3 CorrectedPosition => RawPosition + PositionCorrection;

	public Quaternion CorrectedOrientation => RawOrientation * OrientationCorrection;

	public Vector3 FinalPosition => RawPosition + PositionCorrection;

	public Quaternion FinalOrientation => Mathf.Abs(Lens.Dutch) > 9.9999997473787516E-05
		? CorrectedOrientation * Quaternion.AngleAxis(Lens.Dutch, Vector3.forward)
		: CorrectedOrientation;

	public static CameraState Default =>
		new() {
			Lens = LensSettings.Default,
			ReferenceUp = Vector3.up,
			ReferenceLookAt = kNoPoint,
			RawPosition = Vector3.zero,
			RawOrientation = Quaternion.identity,
			ShotQuality = 1f,
			PositionCorrection = Vector3.zero,
			OrientationCorrection = Quaternion.identity,
			PositionDampingBypass = Vector3.zero
		};

	public int NumCustomBlendables { get; private set; }

	public CustomBlendable GetCustomBlendable(int index) {
		switch (index) {
			case 0:
				return mCustom0;
			case 1:
				return mCustom1;
			case 2:
				return mCustom2;
			case 3:
				return mCustom3;
			default:
				index -= 4;
				return m_CustomOverflow != null && index < m_CustomOverflow.Count
					? m_CustomOverflow[index]
					: new CustomBlendable(null, 0.0f);
		}
	}

	private int FindCustomBlendable(Object custom) {
		if (mCustom0.m_Custom == custom)
			return 0;
		if (mCustom1.m_Custom == custom)
			return 1;
		if (mCustom2.m_Custom == custom)
			return 2;
		if (mCustom3.m_Custom == custom)
			return 3;
		if (m_CustomOverflow != null)
			for (var index = 0; index < m_CustomOverflow.Count; ++index)
				if (m_CustomOverflow[index].m_Custom == custom)
					return index + 4;
		return -1;
	}

	public void AddCustomBlendable(CustomBlendable b) {
		var index = FindCustomBlendable(b.m_Custom);
		if (index >= 0)
			b.m_Weight += GetCustomBlendable(index).m_Weight;
		else
			index = NumCustomBlendables++;
		switch (index) {
			case 0:
				mCustom0 = b;
				break;
			case 1:
				mCustom1 = b;
				break;
			case 2:
				mCustom2 = b;
				break;
			case 3:
				mCustom3 = b;
				break;
			default:
				if (m_CustomOverflow == null)
					m_CustomOverflow = new List<CustomBlendable>();
				m_CustomOverflow.Add(b);
				break;
		}
	}

	public static CameraState Lerp(CameraState stateA, CameraState stateB, float t) {
		t = Mathf.Clamp01(t);
		var t1 = t;
		var cameraState = new CameraState();
		cameraState.Lens = LensSettings.Lerp(stateA.Lens, stateB.Lens, t);
		cameraState.ReferenceUp = Vector3.Slerp(stateA.ReferenceUp, stateB.ReferenceUp, t);
		cameraState.RawPosition = Vector3.Lerp(stateA.RawPosition, stateB.RawPosition, t);
		cameraState.ShotQuality = Mathf.Lerp(stateA.ShotQuality, stateB.ShotQuality, t);
		cameraState.PositionCorrection = Vector3.Lerp(stateA.PositionCorrection, stateB.PositionCorrection, t);
		cameraState.OrientationCorrection =
			Quaternion.Slerp(stateA.OrientationCorrection, stateB.OrientationCorrection, t);
		var vector3 = Vector3.zero;
		if (!stateA.HasLookAt || !stateB.HasLookAt)
			cameraState.ReferenceLookAt = kNoPoint;
		else {
			var fieldOfView1 = stateA.Lens.FieldOfView;
			var fieldOfView2 = stateB.Lens.FieldOfView;
			if (!cameraState.Lens.Orthographic && !Mathf.Approximately(fieldOfView1, fieldOfView2)) {
				var lens = cameraState.Lens with {
					FieldOfView = cameraState.InterpolateFOV(fieldOfView1, fieldOfView2,
						Mathf.Max((stateA.ReferenceLookAt - stateA.CorrectedPosition).magnitude,
							stateA.Lens.NearClipPlane),
						Mathf.Max((stateB.ReferenceLookAt - stateB.CorrectedPosition).magnitude,
							stateB.Lens.NearClipPlane), t)
				};
				cameraState.Lens = lens;
				t1 = Mathf.Abs(
					(float)((lens.FieldOfView - (double)fieldOfView1) / (fieldOfView2 - (double)fieldOfView1)));
			}

			cameraState.ReferenceLookAt = Vector3.Lerp(stateA.ReferenceLookAt, stateB.ReferenceLookAt, t1);
			if (Quaternion.Angle(stateA.RawOrientation, stateB.RawOrientation) > 9.9999997473787516E-05)
				vector3 = cameraState.ReferenceLookAt - cameraState.CorrectedPosition;
		}

		if (vector3.AlmostZero())
			cameraState.RawOrientation = UnityQuaternionExtensions.SlerpWithReferenceUp(stateA.RawOrientation,
				stateB.RawOrientation, t, cameraState.ReferenceUp);
		else {
			vector3 = vector3.normalized;
			if ((vector3 - cameraState.ReferenceUp).AlmostZero() || (vector3 + cameraState.ReferenceUp).AlmostZero())
				cameraState.RawOrientation = UnityQuaternionExtensions.SlerpWithReferenceUp(stateA.RawOrientation,
					stateB.RawOrientation, t, cameraState.ReferenceUp);
			else {
				cameraState.RawOrientation = Quaternion.LookRotation(vector3, cameraState.ReferenceUp);
				var a = -stateA.RawOrientation.GetCameraRotationToTarget(
					stateA.ReferenceLookAt - stateA.CorrectedPosition, stateA.ReferenceUp);
				var b = -stateB.RawOrientation.GetCameraRotationToTarget(
					stateB.ReferenceLookAt - stateB.CorrectedPosition, stateB.ReferenceUp);
				cameraState.RawOrientation =
					cameraState.RawOrientation.ApplyCameraRotation(Vector2.Lerp(a, b, t1), cameraState.ReferenceUp);
			}
		}

		for (var index = 0; index < stateA.NumCustomBlendables; ++index) {
			var customBlendable = stateA.GetCustomBlendable(index);
			customBlendable.m_Weight *= 1f - t;
			if (customBlendable.m_Weight > 9.9999997473787516E-05)
				cameraState.AddCustomBlendable(customBlendable);
		}

		for (var index = 0; index < stateB.NumCustomBlendables; ++index) {
			var customBlendable = stateB.GetCustomBlendable(index);
			customBlendable.m_Weight *= t;
			if (customBlendable.m_Weight > 9.9999997473787516E-05)
				cameraState.AddCustomBlendable(customBlendable);
		}

		return cameraState;
	}

	private float InterpolateFOV(float fovA, float fovB, float dA, float dB, float t) {
		var num1 = Mathf.Lerp(dA * 2f * Mathf.Tan((float)(fovA * (Math.PI / 180.0) / 2.0)),
			dB * 2f * Mathf.Tan((float)(fovB * (Math.PI / 180.0) / 2.0)), t);
		var num2 = 179f;
		var num3 = Mathf.Lerp(dA, dB, t);
		if (num3 > 9.9999997473787516E-05)
			num2 = (float)(2.0 * Mathf.Atan(num1 / (2f * num3)) * 57.295780181884766);
		return Mathf.Clamp(num2, Mathf.Min(fovA, fovB), Mathf.Max(fovA, fovB));
	}

	public struct CustomBlendable {
		public Object m_Custom;
		public float m_Weight;

		public CustomBlendable(Object custom, float weight) {
			m_Custom = custom;
			m_Weight = weight;
		}
	}
}