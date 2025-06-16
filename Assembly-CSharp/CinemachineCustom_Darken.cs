using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.PostProcessing;

[ExecuteInEditMode]
public class CinemachineCustom_Darken : MonoBehaviour {
	public float DarkenLevel;
	private CinemachinePostFX postFx;

	private void Awake() { }

	private void Update() {
		postFx = GetComponent<CinemachinePostFX>();
		if (postFx == null || postFx.m_Profile == null)
			return;
		var colorGrading = postFx.m_Profile.colorGrading;
		var settings = colorGrading.settings;
		settings.basic.postExposure = -10f * DarkenLevel;
		colorGrading.settings = settings;
	}
}