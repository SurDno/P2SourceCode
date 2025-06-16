using System;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine;

[DocumentationSorting(11f, DocumentationSortingAttribute.Level.UserRef)]
[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("Cinemachine/CinemachineFreeLook")]
public class CinemachineFreeLook : CinemachineVirtualCameraBase {
	[Tooltip("Object for the camera children to look at (the aim target).")] [NoSaveDuringPlay]
	public Transform m_LookAt;

	[Tooltip("Object for the camera children wants to move with (the body target).")] [NoSaveDuringPlay]
	public Transform m_Follow;

	[Tooltip(
		"If enabled, this lens setting will apply to all three child rigs, otherwise the child rig lens settings will be used")]
	[FormerlySerializedAs("m_UseCommonLensSetting")]
	public bool m_CommonLens = true;

	[FormerlySerializedAs("m_LensAttributes")]
	[Tooltip(
		"Specifies the lens properties of this Virtual Camera.  This generally mirrors the Unity Camera's lens settings, and will be used to drive the Unity camera when the vcam is active")]
	[LensSettingsProperty]
	public LensSettings m_Lens = LensSettings.Default;

	[Header("Axis Control")] [Tooltip("The Vertical axis.  Value is 0..1.  Chooses how to blend the child rigs")]
	public AxisState m_YAxis = new(2f, 0.2f, 0.1f, 0.5f, "Mouse Y", false);

	[Tooltip("The Horizontal axis.  Value is 0..359.  This is passed on to the rigs' OrbitalTransposer component")]
	public AxisState m_XAxis = new(300f, 0.1f, 0.1f, 0.0f, "Mouse X", true);

	[Tooltip("The definition of Forward.  Camera will follow behind.")]
	public CinemachineOrbitalTransposer.Heading m_Heading =
		new(CinemachineOrbitalTransposer.Heading.HeadingDefinition.TargetForward, 4, 0.0f);

	[Tooltip("Controls how automatic recentering of the X axis is accomplished")]
	public CinemachineOrbitalTransposer.Recentering m_RecenterToTargetHeading = new(false, 1f, 2f);

	[Header("Orbits")]
	[Tooltip(
		"The coordinate space to use when interpreting the offset from the target.  This is also used to set the camera's Up vector, which will be maintained when aiming the camera.")]
	public CinemachineTransposer.BindingMode m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;

	[Tooltip(
		"Controls how taut is the line that connects the rigs' orbits, which determines final placement on the Y axis")]
	[Range(0.0f, 1f)]
	[FormerlySerializedAs("m_SplineTension")]
	public float m_SplineCurvature = 0.2f;

	[Tooltip("The radius and height of the three orbiting rigs.")]
	public Orbit[] m_Orbits = new Orbit[3] {
		new(4.5f, 1.75f),
		new(2.5f, 3f),
		new(0.4f, 1.3f)
	};

	[SerializeField] [HideInInspector] [FormerlySerializedAs("m_HeadingBias")]
	private float m_LegacyHeadingBias = float.MaxValue;

	private bool mUseLegacyRigDefinitions;
	private bool mIsDestroyed;
	private CameraState m_State = CameraState.Default;

	[SerializeField] [HideInInspector] [NoSaveDuringPlay]
	private CinemachineVirtualCamera[] m_Rigs = new CinemachineVirtualCamera[3];

	private CinemachineOrbitalTransposer[] mOrbitals;
	private CinemachineBlend mBlendA;
	private CinemachineBlend mBlendB;
	public static CreateRigDelegate CreateRigOverride;
	public static DestroyRigDelegate DestroyRigOverride;
	private Orbit[] m_CachedOrbits;
	private float m_CachedTension;
	private Vector4[] m_CachedKnots;
	private Vector4[] m_CachedCtrl1;
	private Vector4[] m_CachedCtrl2;

	protected override void OnValidate() {
		base.OnValidate();
		if (m_LegacyHeadingBias != 3.4028234663852886E+38) {
			m_Heading.m_HeadingBias = m_LegacyHeadingBias;
			m_LegacyHeadingBias = float.MaxValue;
			m_RecenterToTargetHeading.LegacyUpgrade(ref m_Heading.m_HeadingDefinition,
				ref m_Heading.m_VelocityFilterStrength);
			mUseLegacyRigDefinitions = true;
		}

		m_YAxis.Validate();
		m_XAxis.Validate();
		m_RecenterToTargetHeading.Validate();
		m_Lens.Validate();
		InvalidateRigCache();
	}

	public CinemachineVirtualCamera GetRig(int i) {
		UpdateRigCache();
		return i < 0 || i > 2 ? null : m_Rigs[i];
	}

	public static string[] RigNames {
		get {
			return new string[3] {
				"TopRig",
				"MiddleRig",
				"BottomRig"
			};
		}
	}

	protected override void OnEnable() {
		mIsDestroyed = false;
		base.OnEnable();
		InvalidateRigCache();
	}

	protected override void OnDestroy() {
		if (m_Rigs != null)
			foreach (var rig in m_Rigs)
				if (rig != null && rig.gameObject != null)
					rig.gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
		mIsDestroyed = true;
		base.OnDestroy();
	}

	private void OnTransformChildrenChanged() {
		InvalidateRigCache();
	}

	private void Reset() {
		DestroyRigs();
	}

	public override CameraState State => m_State;

	public override Transform LookAt {
		get => ResolveLookAt(m_LookAt);
		set => m_LookAt = value;
	}

	public override Transform Follow {
		get => ResolveFollow(m_Follow);
		set => m_Follow = value;
	}

	public override ICinemachineCamera LiveChildOrSelf {
		get {
			if (m_Rigs == null || m_Rigs.Length != 3)
				return this;
			if (m_YAxis.Value < 0.33000001311302185)
				return m_Rigs[2];
			return m_YAxis.Value > 0.6600000262260437 ? m_Rigs[0] : (ICinemachineCamera)m_Rigs[1];
		}
	}

	public override bool IsLiveChild(ICinemachineCamera vcam) {
		if (m_Rigs == null || m_Rigs.Length != 3)
			return false;
		if (m_YAxis.Value < 0.33000001311302185)
			return vcam == m_Rigs[2];
		return m_YAxis.Value > 0.6600000262260437 ? vcam == m_Rigs[0] : vcam == m_Rigs[1];
	}

	public override void RemovePostPipelineStageHook(
		OnPostPipelineStageDelegate d) {
		base.RemovePostPipelineStageHook(d);
		UpdateRigCache();
		if (m_Rigs == null)
			return;
		foreach (var rig in m_Rigs)
			if (rig != null)
				rig.RemovePostPipelineStageHook(d);
	}

	public override void UpdateCameraState(Vector3 worldUp, float deltaTime) {
		if (!PreviousStateIsValid)
			deltaTime = -1f;
		UpdateRigCache();
		if (deltaTime < 0.0)
			m_State = PullStateFromVirtualCamera(worldUp);
		m_State = CalculateNewState(worldUp, deltaTime);
		if (Follow != null) {
			var vector3 = State.RawPosition - transform.position;
			transform.position = State.RawPosition;
			m_Rigs[0].transform.position -= vector3;
			m_Rigs[1].transform.position -= vector3;
			m_Rigs[2].transform.position -= vector3;
		}

		PreviousStateIsValid = true;
		if (deltaTime >= 0.0 || CinemachineCore.Instance.IsLive(this))
			m_YAxis.Update(deltaTime);
		PushSettingsToRigs();
	}

	public override void OnTransitionFromCamera(
		ICinemachineCamera fromCam,
		Vector3 worldUp,
		float deltaTime) {
		base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
		if (fromCam == null || !(fromCam is CinemachineFreeLook))
			return;
		var cinemachineFreeLook = fromCam as CinemachineFreeLook;
		if (cinemachineFreeLook.Follow == Follow) {
			m_XAxis.Value = cinemachineFreeLook.m_XAxis.Value;
			m_YAxis.Value = cinemachineFreeLook.m_YAxis.Value;
			UpdateCameraState(worldUp, deltaTime);
		}
	}

	private void InvalidateRigCache() {
		mOrbitals = null;
	}

	private void DestroyRigs() {
		var cinemachineVirtualCameraArray = new CinemachineVirtualCamera[RigNames.Length];
		for (var index = 0; index < RigNames.Length; ++index)
			foreach (Transform transform in this.transform)
				if (transform.gameObject.name == RigNames[index])
					cinemachineVirtualCameraArray[index] = transform.GetComponent<CinemachineVirtualCamera>();
		for (var index = 0; index < cinemachineVirtualCameraArray.Length; ++index)
			if (cinemachineVirtualCameraArray[index] != null) {
				if (DestroyRigOverride != null)
					DestroyRigOverride(cinemachineVirtualCameraArray[index].gameObject);
				else
					Destroy(cinemachineVirtualCameraArray[index].gameObject);
			}

		m_Rigs = null;
		mOrbitals = null;
	}

	private CinemachineVirtualCamera[] CreateRigs(CinemachineVirtualCamera[] copyFrom) {
		mOrbitals = null;
		var numArray = new float[3] { 0.5f, 0.55f, 0.6f };
		var rigs = new CinemachineVirtualCamera[3];
		for (var index = 0; index < RigNames.Length; ++index) {
			CinemachineVirtualCamera cinemachineVirtualCamera = null;
			if (copyFrom != null && copyFrom.Length > index)
				cinemachineVirtualCamera = copyFrom[index];
			if (CreateRigOverride != null)
				rigs[index] = CreateRigOverride(this, RigNames[index], cinemachineVirtualCamera);
			else {
				rigs[index] = new GameObject(RigNames[index]) {
					transform = {
						parent = transform
					}
				}.AddComponent<CinemachineVirtualCamera>();
				if (cinemachineVirtualCamera != null)
					ReflectionHelpers.CopyFields(cinemachineVirtualCamera, rigs[index]);
				else {
					var gameObject = rigs[index].GetComponentOwner().gameObject;
					gameObject.AddComponent<CinemachineOrbitalTransposer>();
					gameObject.AddComponent<CinemachineComposer>();
				}
			}

			rigs[index].InvalidateComponentPipeline();
			var orbitalTransposer = rigs[index].GetCinemachineComponent<CinemachineOrbitalTransposer>();
			if (orbitalTransposer == null)
				orbitalTransposer = rigs[index].AddCinemachineComponent<CinemachineOrbitalTransposer>();
			if (cinemachineVirtualCamera == null) {
				orbitalTransposer.m_YawDamping = 0.0f;
				var cinemachineComponent = rigs[index].GetCinemachineComponent<CinemachineComposer>();
				if (cinemachineComponent != null) {
					cinemachineComponent.m_HorizontalDamping = cinemachineComponent.m_VerticalDamping = 0.0f;
					cinemachineComponent.m_ScreenX = 0.5f;
					cinemachineComponent.m_ScreenY = numArray[index];
					cinemachineComponent.m_DeadZoneWidth = cinemachineComponent.m_DeadZoneHeight = 0.1f;
					cinemachineComponent.m_SoftZoneWidth = cinemachineComponent.m_SoftZoneHeight = 0.8f;
					cinemachineComponent.m_BiasX = cinemachineComponent.m_BiasY = 0.0f;
				}
			}
		}

		return rigs;
	}

	private void UpdateRigCache() {
		if (mIsDestroyed)
			return;
		if (m_Rigs != null && m_Rigs.Length == 3 && m_Rigs[0] != null && m_Rigs[0].transform.parent != transform) {
			DestroyRigs();
			m_Rigs = CreateRigs(m_Rigs);
		}

		if (mOrbitals != null && mOrbitals.Length == 3)
			return;
		if (LocateExistingRigs(RigNames, false) != 3) {
			DestroyRigs();
			m_Rigs = CreateRigs(null);
			LocateExistingRigs(RigNames, true);
		}

		foreach (var rig in m_Rigs) {
			var cinemachineVirtualCamera = rig;
			string[] strArray;
			if (!m_CommonLens)
				strArray = new string[5] {
					"m_Script",
					"Header",
					"Extensions",
					"m_Priority",
					"m_Follow"
				};
			else
				strArray = new string[6] {
					"m_Script",
					"Header",
					"Extensions",
					"m_Priority",
					"m_Follow",
					"m_Lens"
				};
			cinemachineVirtualCamera.m_ExcludedPropertiesInInspector = strArray;
			rig.m_LockStageInInspector = new CinemachineCore.Stage[1];
		}

		mBlendA = new CinemachineBlend(m_Rigs[1], m_Rigs[0], AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f), 1f, 0.0f);
		mBlendB = new CinemachineBlend(m_Rigs[2], m_Rigs[1], AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f), 1f, 0.0f);
		m_XAxis.SetThresholds(0.0f, 360f, true);
		m_YAxis.SetThresholds(0.0f, 1f, false);
	}

	private int LocateExistingRigs(string[] rigNames, bool forceOrbital) {
		mOrbitals = new CinemachineOrbitalTransposer[rigNames.Length];
		m_Rigs = new CinemachineVirtualCamera[rigNames.Length];
		var num = 0;
		foreach (Transform transform in this.transform) {
			var component = transform.GetComponent<CinemachineVirtualCamera>();
			if (component != null) {
				var gameObject = transform.gameObject;
				for (var index = 0; index < rigNames.Length; ++index)
					if (mOrbitals[index] == null && gameObject.name == rigNames[index]) {
						mOrbitals[index] = component.GetCinemachineComponent<CinemachineOrbitalTransposer>();
						if ((mOrbitals[index] == null) & forceOrbital)
							mOrbitals[index] = component.AddCinemachineComponent<CinemachineOrbitalTransposer>();
						if (mOrbitals[index] != null) {
							mOrbitals[index].m_HeadingIsSlave = true;
							if (index == 0)
								mOrbitals[index].HeadingUpdater = (orbital, deltaTime, up) =>
									orbital.UpdateHeading(deltaTime, up, ref m_XAxis);
							m_Rigs[index] = component;
							++num;
						}
					}
			}
		}

		return num;
	}

	private void PushSettingsToRigs() {
		UpdateRigCache();
		for (var index = 0; index < m_Rigs.Length; ++index)
			if (!(m_Rigs[index] == null)) {
				if (m_CommonLens)
					m_Rigs[index].m_Lens = m_Lens;
				if (mUseLegacyRigDefinitions) {
					mUseLegacyRigDefinitions = false;
					m_Orbits[index].m_Height = mOrbitals[index].m_FollowOffset.y;
					m_Orbits[index].m_Radius = -mOrbitals[index].m_FollowOffset.z;
					if (m_Rigs[index].Follow != null)
						Follow = m_Rigs[index].Follow;
				}

				m_Rigs[index].Follow = null;
				if (CinemachineCore.sShowHiddenObjects)
					m_Rigs[index].gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
				else
					m_Rigs[index].gameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.HideInInspector;
				mOrbitals[index].m_FollowOffset = GetLocalPositionForCameraFromInput(m_YAxis.Value);
				mOrbitals[index].m_BindingMode = m_BindingMode;
				mOrbitals[index].m_Heading = m_Heading;
				mOrbitals[index].m_XAxis = m_XAxis;
				mOrbitals[index].m_RecenterToTargetHeading = m_RecenterToTargetHeading;
				if (index > 0)
					mOrbitals[index].m_RecenterToTargetHeading.m_enabled = false;
				if (m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
					m_Rigs[index].SetStateRawPosition(State.RawPosition);
			}
	}

	private CameraState CalculateNewState(Vector3 worldUp, float deltaTime) {
		var newState = PullStateFromVirtualCamera(worldUp);
		var num = m_YAxis.Value;
		if (num > 0.5) {
			if (mBlendA != null) {
				mBlendA.TimeInBlend = (float)((num - 0.5) * 2.0);
				mBlendA.UpdateCameraState(worldUp, deltaTime);
				newState = mBlendA.State;
			}
		} else if (mBlendB != null) {
			mBlendB.TimeInBlend = num * 2f;
			mBlendB.UpdateCameraState(worldUp, deltaTime);
			newState = mBlendB.State;
		}

		return newState;
	}

	private CameraState PullStateFromVirtualCamera(Vector3 worldUp) {
		var cameraState = CameraState.Default with {
			RawPosition = transform.position,
			RawOrientation = transform.rotation,
			ReferenceUp = worldUp
		};
		var potentialTargetBrain = CinemachineCore.Instance.FindPotentialTargetBrain(this);
		m_Lens.Aspect = potentialTargetBrain != null ? potentialTargetBrain.OutputCamera.aspect : 1f;
		m_Lens.Orthographic = potentialTargetBrain != null && potentialTargetBrain.OutputCamera.orthographic;
		cameraState.Lens = m_Lens;
		return cameraState;
	}

	public Vector3 GetLocalPositionForCameraFromInput(float t) {
		if (mOrbitals == null)
			return Vector3.zero;
		UpdateCachedSpline();
		var index = 1;
		if (t > 0.5) {
			t -= 0.5f;
			index = 2;
		}

		return SplineHelpers.Bezier3(t * 2f, m_CachedKnots[index], m_CachedCtrl1[index], m_CachedCtrl2[index],
			m_CachedKnots[index + 1]);
	}

	private void UpdateCachedSpline() {
		var flag = m_CachedOrbits != null && m_CachedTension == (double)m_SplineCurvature;
		for (var index = 0; (index < 3) & flag; ++index)
			flag = m_CachedOrbits[index].m_Height == (double)m_Orbits[index].m_Height &&
			       m_CachedOrbits[index].m_Radius == (double)m_Orbits[index].m_Radius;
		if (flag)
			return;
		var splineCurvature = m_SplineCurvature;
		m_CachedKnots = new Vector4[5];
		m_CachedCtrl1 = new Vector4[5];
		m_CachedCtrl2 = new Vector4[5];
		m_CachedKnots[1] = new Vector4(0.0f, m_Orbits[2].m_Height, -m_Orbits[2].m_Radius, 0.0f);
		m_CachedKnots[2] = new Vector4(0.0f, m_Orbits[1].m_Height, -m_Orbits[1].m_Radius, 0.0f);
		m_CachedKnots[3] = new Vector4(0.0f, m_Orbits[0].m_Height, -m_Orbits[0].m_Radius, 0.0f);
		m_CachedKnots[0] = Vector4.Lerp(m_CachedKnots[1], Vector4.zero, splineCurvature);
		m_CachedKnots[4] = Vector4.Lerp(m_CachedKnots[3], Vector4.zero, splineCurvature);
		SplineHelpers.ComputeSmoothControlPoints(ref m_CachedKnots, ref m_CachedCtrl1, ref m_CachedCtrl2);
		m_CachedOrbits = new Orbit[3];
		for (var index = 0; index < 3; ++index)
			m_CachedOrbits[index] = m_Orbits[index];
		m_CachedTension = m_SplineCurvature;
	}

	[Serializable]
	public struct Orbit {
		public float m_Height;
		public float m_Radius;

		public Orbit(float h, float r) {
			m_Height = h;
			m_Radius = r;
		}
	}

	public delegate CinemachineVirtualCamera CreateRigDelegate(
		CinemachineFreeLook vcam,
		string name,
		CinemachineVirtualCamera copyFrom);

	public delegate void DestroyRigDelegate(GameObject rig);
}