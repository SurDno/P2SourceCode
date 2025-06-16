using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine;

[DocumentationSorting(8f, DocumentationSortingAttribute.Level.UserRef)]
[AddComponentMenu("")]
[RequireComponent(typeof(CinemachinePipeline))]
[SaveDuringPlay]
public class CinemachineBasicMultiChannelPerlin : CinemachineComponentBase {
	[HideInInspector]
	[Tooltip(
		"The asset containing the Noise Profile.  Define the frequencies and amplitudes there to make a characteristic noise profile.  Make your own or just use one of the many presets.")]
	[FormerlySerializedAs("m_Definition")]
	public NoiseSettings m_NoiseProfile;

	[Tooltip(
		"Gain to apply to the amplitudes defined in the NoiseSettings asset.  1 is normal.  Setting this to 0 completely mutes the noise.")]
	public float m_AmplitudeGain = 1f;

	[Tooltip(
		"Scale factor to apply to the frequencies defined in the NoiseSettings asset.  1 is normal.  Larger magnitudes will make the noise shake more rapidly.")]
	public float m_FrequencyGain = 1f;

	private bool mInitialized;
	private float mNoiseTime;
	private Vector3 mNoiseOffsets = Vector3.zero;

	public override bool IsValid => enabled && m_NoiseProfile != null;

	public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Noise;

	public override void MutateCameraState(ref CameraState curState, float deltaTime) {
		if (!IsValid || deltaTime < 0.0)
			return;
		if (!mInitialized)
			Initialize();
		mNoiseTime += deltaTime * m_FrequencyGain;
		curState.PositionCorrection += curState.CorrectedOrientation *
		                               GetCombinedFilterResults(m_NoiseProfile.PositionNoise, mNoiseTime,
			                               mNoiseOffsets) * m_AmplitudeGain;
		var quaternion =
			Quaternion.Euler(GetCombinedFilterResults(m_NoiseProfile.OrientationNoise, mNoiseTime, mNoiseOffsets) *
			                 m_AmplitudeGain);
		curState.OrientationCorrection *= quaternion;
	}

	private void Initialize() {
		mInitialized = true;
		mNoiseTime = 0.0f;
		mNoiseOffsets = new Vector3(Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f),
			Random.Range(-10000f, 10000f));
	}

	private static Vector3 GetCombinedFilterResults(
		NoiseSettings.TransformNoiseParams[] noiseParams,
		float time,
		Vector3 noiseOffsets) {
		var x = 0.0f;
		var y = 0.0f;
		var z = 0.0f;
		if (noiseParams != null)
			for (var index = 0; index < noiseParams.Length; ++index) {
				var noiseParam = noiseParams[index];
				var vector3_1 =
					new Vector3(noiseParam.X.Frequency, noiseParam.Y.Frequency, noiseParam.Z.Frequency) * time +
					noiseOffsets;
				var vector3_2 = new Vector3(Mathf.PerlinNoise(vector3_1.x, 0.0f) - 0.5f,
					Mathf.PerlinNoise(vector3_1.y, 0.0f) - 0.5f, Mathf.PerlinNoise(vector3_1.z, 0.0f) - 0.5f);
				x += vector3_2.x * noiseParam.X.Amplitude;
				y += vector3_2.y * noiseParam.Y.Amplitude;
				z += vector3_2.z * noiseParam.Z.Amplitude;
			}

		return new Vector3(x, y, z);
	}
}