using UnityEngine;

internal class SphereParticleSystemCuller : MonoBehaviour {
	[SerializeField] private float raduis = 3f;
	[SerializeField] private Vector3 offset;
	private CullingGroup cullingGroup;
	private BoundingSphere[] boundingSpheres = new BoundingSphere[1];
	private ParticleSystemRenderer[] renderers;
	private bool[] renderersIsEnabled;
	private ParticleSystem[] particleSystems;
	private bool[] particleSystemsIsPlaying;

	private void OnEnable() {
		cullingGroup = new CullingGroup();
		boundingSpheres[0] = new BoundingSphere(transform.position + offset, raduis);
		cullingGroup.SetBoundingSpheres(boundingSpheres);
		cullingGroup.onStateChanged += StateChanged;
		if (renderers == null) {
			renderers = gameObject.GetComponentsInChildren<ParticleSystemRenderer>();
			renderersIsEnabled = new bool[renderers.Length];
			for (var index = 0; index < renderers.Length; ++index)
				renderersIsEnabled[index] = renderers[index].enabled;
			particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
			particleSystemsIsPlaying = new bool[particleSystems.Length];
			for (var index = 0; index < particleSystems.Length; ++index)
				particleSystemsIsPlaying[index] = particleSystems[index].isPlaying;
		}

		Camera.onPreCull += OnPreCullEvent;
	}

	private void OnDisable() {
		cullingGroup.onStateChanged -= StateChanged;
		Camera.onPreCull -= OnPreCullEvent;
		cullingGroup.Dispose();
		cullingGroup = null;
	}

	public void StateChanged(CullingGroupEvent sphere) {
		for (var index = 0; index < renderers.Length; ++index)
			if (renderersIsEnabled[index])
				renderers[index].enabled = sphere.isVisible;
		for (var index = 0; index < particleSystems.Length; ++index)
			if (particleSystemsIsPlaying[index]) {
				if (sphere.isVisible)
					particleSystems[index].Play();
				else
					particleSystems[index].Pause();
			}
	}

	private void OnPreCullEvent(Camera camera) {
		if (GameCamera.Instance == null || camera != GameCamera.Instance.Camera)
			return;
		cullingGroup.targetCamera = camera;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + offset, raduis);
	}
}