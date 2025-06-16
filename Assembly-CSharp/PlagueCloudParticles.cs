using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlagueCloudParticles : MonoBehaviour {
	private static Dictionary<int, Mesh> meshes;
	private static MaterialPropertyBlock mpb;
	public Shader KernelShader;
	[FormerlySerializedAs("Radius")] public float EmissionRadius = 2f;
	[Range(1f, 65000f)] public int MaxPointCount = 1000;
	[Range(0.0f, 1f)] public float Emission = 1f;
	public float LifeTime = 10f;
	public Vector3 Acceleration = Vector3.zero;
	public float Drag = 1f;
	[Space] public float NoiseFrequency = 1f;
	public float NoiseAmplitude = 1f;
	public float NoiseMotion = 1f;
	[Space] public float Gravity;
	public float GravitySphereRadius = 0.5f;
	public float GravityFadeRadius = 10f;
	public float GravityMovementPrediction = 2f;
	[Space] public Texture2D Pattern;
	public Vector2 PatternSize;
	public float PatternPlaneForce;
	public float PatternOrthogonalForce;
	public float PatternRandomForce;
	private RenderTexture positionBuffer;
	private RenderTexture velocityBuffer;
	private RenderTexture targetBuffer;
	private Material kernelMaterial;
	private Vector3 noiseOffset;
	private Vector3 delayedPosition;
	private Vector3 velocity;

	private void Start() {
		var height = Mathf.CeilToInt(MaxPointCount / 256f);
		Mesh mesh = null;
		if (meshes == null)
			meshes = new Dictionary<int, Mesh>();
		else
			meshes.TryGetValue(MaxPointCount, out mesh);
		if (mesh == null) {
			mesh = new Mesh();
			var vector3Array = new Vector3[MaxPointCount];
			var indices = new int[MaxPointCount];
			var index = 0;
			var num1 = 0;
			var num2 = 0;
			while (index < MaxPointCount) {
				if (num1 == 256) {
					num1 = 0;
					++num2;
				}

				vector3Array[index] = new Vector3((float)((num1 + 0.5) / 256.0), (num2 + 0.5f) / height, Random.value);
				indices[index] = index;
				++index;
				++num1;
			}

			mesh.vertices = vector3Array;
			mesh.SetIndices(indices, MeshTopology.Points, 0);
			var num3 = 100f;
			mesh.bounds = new Bounds(Vector3.zero, new Vector3(num3, num3, num3));
			mesh.name = "Plague Cloud Particles (" + MaxPointCount + ")";
			meshes.Add(MaxPointCount, mesh);
		}

		GetComponent<MeshFilter>().sharedMesh = mesh;
		positionBuffer = new RenderTexture(256, height, 0, RenderTextureFormat.ARGBFloat);
		velocityBuffer = new RenderTexture(256, height, 0, RenderTextureFormat.ARGBFloat);
		targetBuffer = new RenderTexture(256, height, 0, RenderTextureFormat.ARGBFloat);
		if (!UpdateKernelMaterial())
			return;
		Graphics.Blit(null, positionBuffer, kernelMaterial, 0);
		ApplyPositionBuffer();
	}

	private void ApplyPositionBuffer() {
		if (mpb == null)
			mpb = new MaterialPropertyBlock();
		mpb.SetTexture("_PositionBuffer", positionBuffer);
		GetComponent<MeshRenderer>().SetPropertyBlock(mpb);
	}

	private void SwapBuffers(ref RenderTexture rt0, ref RenderTexture rt1) {
		var renderTexture = rt0;
		rt0 = rt1;
		rt1 = renderTexture;
	}

	private bool UpdateKernelMaterial() {
		if (kernelMaterial == null) {
			if (!(KernelShader != null))
				return false;
			kernelMaterial = new Material(KernelShader);
		}

		var z = Mathf.Min(Time.deltaTime, 0.1f);
		var position = transform.position;
		delayedPosition = Vector3.SmoothDamp(delayedPosition, position, ref velocity, 0.1f);
		var vector3 = position + velocity * GravityMovementPrediction;
		kernelMaterial.SetVector("_Acceleration",
			new Vector4(Acceleration.x, Acceleration.y, Acceleration.z, Mathf.Exp(-Drag * z)));
		kernelMaterial.SetVector("_NoiseParams", new Vector2(NoiseFrequency, NoiseAmplitude));
		if (Acceleration == Vector3.zero)
			noiseOffset += Vector3.up * NoiseMotion * z;
		else
			noiseOffset += Acceleration.normalized * NoiseMotion * z;
		kernelMaterial.SetVector("_NoiseOffset", noiseOffset);
		kernelMaterial.SetFloat("_LifeTime", 1f / LifeTime);
		kernelMaterial.SetVector("_Emitter", new Vector4(position.x, position.y, position.z, EmissionRadius));
		kernelMaterial.SetVector("_GravityPosition", new Vector4(vector3.x, vector3.y, vector3.z, 0.0f));
		kernelMaterial.SetVector("_GravityConfig", new Vector4(Gravity, GravitySphereRadius, GravityFadeRadius, 0.0f));
		kernelMaterial.SetVector("_Config", new Vector4(Emission, Random.value, z, 0.0f));
		kernelMaterial.SetTexture("_PatternTex", Pattern);
		kernelMaterial.SetVector("_PatternConfig",
			new Vector4(PatternPlaneForce, PatternOrthogonalForce, PatternRandomForce, 0.0f));
		var rotation = transform.rotation;
		kernelMaterial.SetMatrix("_ToPattern",
			Matrix4x4.TRS(position, rotation, new Vector3(PatternSize.x, PatternSize.y, 1f)).inverse);
		kernelMaterial.SetMatrix("_FromPatternRotation", Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one));
		return true;
	}

	private void Update() {
		if (!UpdateKernelMaterial())
			return;
		kernelMaterial.SetTexture("_PositionBuffer", positionBuffer);
		kernelMaterial.SetTexture("_VelocityBuffer", velocityBuffer);
		Graphics.Blit(null, targetBuffer, kernelMaterial, 1);
		SwapBuffers(ref velocityBuffer, ref targetBuffer);
		kernelMaterial.SetTexture("_PositionBuffer", positionBuffer);
		kernelMaterial.SetTexture("_VelocityBuffer", velocityBuffer);
		Graphics.Blit(null, targetBuffer, kernelMaterial, 2);
		SwapBuffers(ref positionBuffer, ref targetBuffer);
		ApplyPositionBuffer();
	}
}