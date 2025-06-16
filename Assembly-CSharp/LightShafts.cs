using Engine.Common;
using Engine.Source.Commons;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(ParticleSystem))]
public class LightShafts : MonoBehaviour, IUpdatable {
	public static bool isLightActive = false;
	private static Vector3 _lightDirection = Vector3.forward;
	public static LayerMask shadowCastingColliders;
	public static bool isPlayerSet = false;
	public static Vector3 playerPosition = Vector3.zero;
	public static float nearDistance = 5f;
	public static float farDistance = 100f;
	public static float nearUpdateTime = 0.5f;
	private static MaterialPropertyBlock _materialProperties;
	private static int _opacityShaderProperty;
	private static int thisFrameIndex = -1;
	private static int thisFrameRebuilds;
	public float brightness = 0.25f;
	public float fadeIn = 0.5f;
	[Header("Raycasts")] public float indoorBias = 0.5f;
	public float length = 5f;
	public float outdoorBias = 0.5f;
	public float outdoorDistance = 50f;
	[Space] public float particlesPerUnit = 1f;
	[Space] public Vector3[] points = new Vector3[0];
	[Header("Rays")] public float radius = 0.1f;
	private GameObject _child;
	private MeshFilter _childMeshFilter;
	private MeshRenderer _childMeshRenderer;
	private MeshFilter _meshFilter;
	private MeshRenderer _meshRenderer;
	private ParticleSystem _particleSystem;
	private float _lastUpdateTime;
	private float _phase;

	public static Vector3 LightDirection {
		get => _lightDirection;
		set => _lightDirection = value.normalized;
	}

	private MeshFilter meshFilter {
		get {
			if (_meshFilter == null)
				_meshFilter = GetComponent<MeshFilter>();
			return _meshFilter;
		}
	}

	private MeshRenderer meshRenderer {
		get {
			if (_meshRenderer == null)
				_meshRenderer = GetComponent<MeshRenderer>();
			return _meshRenderer;
		}
	}

	private ParticleSystem particleSystem {
		get {
			if (_particleSystem == null)
				_particleSystem = GetComponent<ParticleSystem>();
			return _particleSystem;
		}
	}

	private void CheckChild() {
		if (!(_child == null))
			return;
		_child = new GameObject("Light Shaft Fade", typeof(MeshFilter), typeof(MeshRenderer));
		_child.layer = gameObject.layer;
		_child.tag = gameObject.tag;
		_child.transform.SetParent(transform, false);
		_child.transform.localPosition = Vector3.zero;
		_child.transform.localRotation = Quaternion.identity;
		_child.transform.localScale = Vector3.one;
		_child.hideFlags = HideFlags.DontSave;
		_childMeshRenderer = _child.GetComponent<MeshRenderer>();
		_childMeshRenderer.sharedMaterial = meshRenderer.sharedMaterial;
		_childMeshFilter = _child.GetComponent<MeshFilter>();
	}

	private void NullChild() {
		if (!(_child != null))
			return;
		Destroy(_child);
		_child = null;
		_childMeshFilter = null;
		_childMeshRenderer = null;
	}

	private void CheckMesh() {
		if (meshFilter.sharedMesh != null)
			return;
		var mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.hideFlags = HideFlags.DontSave;
		meshFilter.sharedMesh = mesh;
		var shape = particleSystem.shape with {
			shapeType = ParticleSystemShapeType.Mesh,
			mesh = mesh
		};
		particleSystem.Play();
		meshRenderer.enabled = true;
	}

	public void NullMesh() {
		if (meshFilter.sharedMesh != null) {
			Destroy(meshFilter.sharedMesh);
			meshFilter.sharedMesh = null;
		}

		particleSystem.Stop();
		var shape = particleSystem.shape with {
			shapeType = ParticleSystemShapeType.Sphere,
			mesh = null
		};
		meshRenderer.enabled = false;
	}

	private void Reset() {
		NullChild();
		NullMesh();
		_phase = 1f;
	}

	private void OnEnable() {
		LightShaftsBuilder.Occupy();
		InstanceByRequest<UpdateService>.Instance.LightShaftsUpdater.AddUpdatable(this);
		Reset();
	}

	private void OnDisable() {
		InstanceByRequest<UpdateService>.Instance.LightShaftsUpdater.RemoveUpdatable(this);
		LightShaftsBuilder.Vacate();
		Reset();
	}

	public void ComputeUpdate() {
		if (!isLightActive)
			Reset();
		else {
			if (nearUpdateTime <= 0.0)
				++_phase;
			else {
				var num1 = 1f / nearUpdateTime;
				if (isPlayerSet) {
					var num2 = Vector3.Distance(transform.position, playerPosition);
					if (num2 >= (double)farDistance)
						num1 = 0.0f;
					else if (num2 > (double)nearDistance)
						num1 *= (float)((farDistance - (double)num2) / (farDistance - (double)nearDistance));
				}

				_phase += num1 * (Time.unscaledTime - _lastUpdateTime);
			}

			if (_phase >= 1.0) {
				if (thisFrameIndex != Time.frameCount) {
					thisFrameIndex = Time.frameCount;
					thisFrameRebuilds = 0;
				}

				if (thisFrameRebuilds < 4 || _phase >= 2.0) {
					if (meshFilter.sharedMesh == null)
						NullChild();
					else {
						CheckChild();
						var sharedMesh = _childMeshFilter.sharedMesh;
						_childMeshFilter.sharedMesh = meshFilter.sharedMesh;
						meshFilter.sharedMesh = sharedMesh;
					}

					UpdateMesh();
					_phase = 0.0f;
					++thisFrameRebuilds;
				}
			}

			if (meshFilter.sharedMesh != null)
				UpdateRenderer(meshRenderer, _phase);
			if (_childMeshRenderer != null)
				UpdateRenderer(_childMeshRenderer, 1f - _phase);
		}

		_lastUpdateTime = Time.unscaledTime;
	}

	private void UpdateRenderer(MeshRenderer renderer, float phase) {
		if (_materialProperties == null) {
			_materialProperties = new MaterialPropertyBlock();
			_opacityShaderProperty = Shader.PropertyToID("_Opacity");
		}

		phase = Mathf.Clamp01(phase);
		_materialProperties.SetFloat(_opacityShaderProperty, phase * brightness);
		renderer.SetPropertyBlock(_materialProperties);
	}

	private void UpdateMesh() {
		if (!isLightActive || this.length <= 0.0)
			NullMesh();
		else {
			var opacity = Mathf.Clamp01(Vector3.Dot(LightDirection, transform.forward));
			if (opacity <= 0.0)
				NullMesh();
			else {
				var instance = LightShaftsBuilder.Instance;
				instance.Prepare(this, opacity, points.Length);
				var localToWorldMatrix = transform.localToWorldMatrix;
				var flag = false;
				var num = 0.0f;
				for (var index = 0; index < points.Length; ++index) {
					var origin = localToWorldMatrix.MultiplyPoint(points[index]);
					if (outdoorDistance > (double)outdoorBias && Physics.Raycast(origin - _lightDirection * outdoorBias,
						    -_lightDirection, outdoorDistance - outdoorBias, shadowCastingColliders,
						    QueryTriggerInteraction.Ignore)) {
						var length = 0.0f;
						instance.AddRay(origin, length);
					} else {
						RaycastHit hitInfo;
						float length1;
						if (length > (double)indoorBias && Physics.Raycast(origin + _lightDirection * indoorBias,
							    _lightDirection, out hitInfo, length - indoorBias, shadowCastingColliders,
							    QueryTriggerInteraction.Ignore)) {
							length1 = hitInfo.distance + indoorBias + radius;
							if (length1 > (double)length)
								length1 = length;
						} else
							length1 = length;

						if (length1 <= (double)radius) {
							var length2 = 0.0f;
							instance.AddRay(origin, length2);
						} else {
							instance.AddRay(origin, length1);
							num += length1;
							flag = true;
						}
					}
				}

				if (!flag)
					NullMesh();
				else {
					CheckMesh();
					instance.BuildTo(meshFilter.sharedMesh);
					var emission = particleSystem.emission;
					emission.rateOverTime = new ParticleSystem.MinMaxCurve(num * particlesPerUnit);
					var shape = particleSystem.shape with {
						shapeType = ParticleSystemShapeType.Mesh,
						mesh = meshFilter.sharedMesh
					};
				}
			}
		}
	}
}