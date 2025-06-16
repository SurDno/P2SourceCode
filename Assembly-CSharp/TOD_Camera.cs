using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Time of Day/Camera Main Script")]
public class TOD_Camera : MonoBehaviour {
	public TOD_Sky sky;
	public bool DomePosToCamera = true;
	public Vector3 DomePosOffset = Vector3.zero;
	public bool DomeScaleToFarClip = true;
	public float DomeScaleFactor = 0.95f;
	private Camera cameraComponent;
	private Transform cameraTransform;

	public bool HDR => (bool)(Object)cameraComponent && cameraComponent.allowHDR;

	protected void OnValidate() {
		DomeScaleFactor = Mathf.Clamp(DomeScaleFactor, 0.01f, 1f);
	}

	protected void OnEnable() {
		cameraComponent = GetComponent<Camera>();
		cameraTransform = GetComponent<Transform>();
	}

	protected void Update() {
		if (!(bool)(Object)sky)
			sky = TOD_Sky.Instance;
		if (!(bool)(Object)sky || !sky.Initialized)
			return;
		sky.Components.Camera = this;
		if (cameraComponent.clearFlags != CameraClearFlags.Color)
			cameraComponent.clearFlags = CameraClearFlags.Color;
		if (cameraComponent.backgroundColor != Color.clear)
			cameraComponent.backgroundColor = Color.clear;
		RenderSettings.skybox = sky.Resources.Skybox;
	}

	protected void OnPreCull() {
		if (!(bool)(Object)sky || !sky.Initialized)
			return;
		if (DomeScaleToFarClip)
			DoDomeScaleToFarClip();
		if (!DomePosToCamera)
			return;
		DoDomePosToCamera();
	}

	public void DoDomeScaleToFarClip() {
		var num = DomeScaleFactor * cameraComponent.farClipPlane;
		sky.Components.DomeTransform.localScale = new Vector3(num, num, num);
	}

	public void DoDomePosToCamera() {
		sky.Components.DomeTransform.position = cameraTransform.position + cameraTransform.rotation * DomePosOffset;
	}
}