using UnityEngine;

public class LookAtCamera : MonoBehaviour {
	public Camera lookAtCamera;
	public bool lookOnlyOnAwake;

	public void Start() {
		if (lookAtCamera == null)
			lookAtCamera = Camera.main;
		if (!lookOnlyOnAwake)
			return;
		LookCam();
	}

	public void Update() {
		if (lookOnlyOnAwake)
			return;
		LookCam();
	}

	public void LookCam() {
		transform.LookAt(lookAtCamera.transform);
	}
}