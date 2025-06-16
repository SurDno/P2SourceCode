using System;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class DeferredProjector : MonoBehaviour {
	private const CameraEvent RenderTime = CameraEvent.BeforeReflections;
	private const int MaxActiveInctances = 4096;

	[SerializeField] [FormerlySerializedAs("Material")]
	private Material material;

	[SerializeField] private Property[] properties;
	private int index = -1;
	private Material actualMaterial;
	private Vector3 lossyScale;
	private static DeferredProjector[] instances = new DeferredProjector[4096];
	private static BoundingSphere[] boundingSpheres = new BoundingSphere[4096];
	private static int[] cullingResults = new int[4096];
	private static int count;
	private static CullingGroup cullingGroup;
	private static CommandBuffer commandBuffer;
	private static Camera bufferCamera;
	private static RenderTargetIdentifier[] mrt;
	private static Mesh boxMesh;
	private static int normalsID;
	private static int specularID;
	private static int cullId;
	private static int zTestId;

	public Property[] Properties => properties;

	public void PopulateBuffer(CommandBuffer buffer, bool inside) {
		if (actualMaterial == null)
			return;
		var flag = (lossyScale.x < 0.0) ^ (lossyScale.y < 0.0) ^ (lossyScale.z < 0.0);
		if (properties != null && properties.Length != 0)
			for (var index = 0; index < properties.Length; ++index)
				actualMaterial.SetFloat(properties[index].Name, properties[index].Value);
		actualMaterial.SetInt(cullId, flag ^ inside ? 1 : 2);
		actualMaterial.SetInt(zTestId, inside ? 5 : 2);
		buffer.DrawMesh(BoxMesh, transform.localToWorldMatrix, actualMaterial, 0, -1);
	}

	private void DrawGizmo(bool selected) {
		Gizmos.matrix = transform.localToWorldMatrix;
		var color = new Color(0.0f, 0.7f, 1f, 1f);
		if (selected) {
			color.a = 0.25f;
			Gizmos.color = color;
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
		}

		color.a = selected ? 0.5f : 0.1f;
		Gizmos.color = color;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}

	private void OnEnable() {
		if (count == 4096)
			return;
		index = count;
		instances[index] = this;
		UpdateBoundingSphere();
		if (material != null)
			actualMaterial = new Material(material);
		++count;
		if (count == 1) {
			Camera.onPreCull += OnPreCullEvent;
			Camera.onPreRender += OnPreRenderEvent;
			Camera.onPostRender += OnPostRenderEvent;
			cullingGroup = new CullingGroup();
			cullingGroup.SetBoundingSpheres(boundingSpheres);
			commandBuffer = new CommandBuffer();
			commandBuffer.name = "Decals";
		}

		cullingGroup.SetBoundingSphereCount(count);
	}

	private void OnDisable() {
		if (this.index == -1)
			return;
		var index = count - 1;
		if (actualMaterial != null) {
			if (Application.isPlaying)
				Destroy(actualMaterial);
			else
				DestroyImmediate(actualMaterial);
			actualMaterial = null;
		}

		if (this.index != index) {
			var instance = instances[index];
			instance.index = this.index;
			instances[this.index] = instance;
			boundingSpheres[this.index] = boundingSpheres[index];
		}

		instances[index] = null;
		--count;
		if (count == 0) {
			Camera.onPreCull -= OnPreCullEvent;
			Camera.onPreRender -= OnPreRenderEvent;
			Camera.onPostRender -= OnPostRenderEvent;
			cullingGroup.Dispose();
			cullingGroup = null;
			commandBuffer = null;
		} else
			cullingGroup.SetBoundingSphereCount(count);

		this.index = -1;
	}

	private void OnDrawGizmos() {
		DrawGizmo(false);
	}

	private void OnDrawGizmosSelected() {
		DrawGizmo(true);
		UpdateBoundingSphere();
	}

	private void UpdateBoundingSphere() {
		if (index == -1)
			return;
		lossyScale = transform.lossyScale;
		boundingSpheres[index] = new BoundingSphere(transform.position, lossyScale.magnitude * 0.5f);
	}

	private void OnValidate() {
		UpdateBoundingSphere();
	}

	private static Mesh BoxMesh {
		get {
			if (boxMesh == null) {
				boxMesh = new Mesh();
				boxMesh.hideFlags = HideFlags.HideAndDontSave;
				boxMesh.vertices = new Vector3[8] {
					new(-0.5f, -0.5f, -0.5f),
					new(-0.5f, 0.5f, -0.5f),
					new(0.5f, 0.5f, -0.5f),
					new(0.5f, -0.5f, -0.5f),
					new(0.5f, -0.5f, 0.5f),
					new(0.5f, 0.5f, 0.5f),
					new(-0.5f, 0.5f, 0.5f),
					new(-0.5f, -0.5f, 0.5f)
				};
				boxMesh.triangles = new int[36] {
					0,
					1,
					2,
					2,
					3,
					0,
					3,
					2,
					5,
					5,
					4,
					3,
					4,
					5,
					6,
					6,
					7,
					4,
					7,
					6,
					1,
					1,
					0,
					7,
					1,
					6,
					5,
					5,
					2,
					1,
					7,
					0,
					3,
					3,
					4,
					7
				};
			}

			return boxMesh;
		}
	}

	private static void OnPreCullEvent(Camera camera) {
		if (Profiler.enabled)
			Profiler.BeginSample(nameof(DeferredProjector));
		OnPreCullEvent2(camera);
		if (!Profiler.enabled)
			return;
		Profiler.EndSample();
	}

	private static void OnPreCullEvent2(Camera camera) {
		if (cullingGroup.targetCamera != null || camera.actualRenderingPath != RenderingPath.DeferredShading)
			return;
		cullingGroup.targetCamera = camera;
	}

	private static void OnPreRenderEvent(Camera camera) {
		if (Profiler.enabled)
			Profiler.BeginSample(nameof(DeferredProjector));
		OnPreRenderEvent2(camera);
		if (!Profiler.enabled)
			return;
		Profiler.EndSample();
	}

	private static void OnPreRenderEvent2(Camera camera) {
		if (cullingGroup.targetCamera != camera)
			return;
		var num1 = cullingGroup.QueryIndices(true, cullingResults, 0);
		var position = camera.transform.position;
		var magnitude = camera.projectionMatrix.inverse.MultiplyPoint(new Vector3(-1f, -1f, -1f)).magnitude;
		if (num1 <= 0)
			return;
		var cullingMask = camera.cullingMask;
		var num2 = 0;
		for (var index = 0; index < num1; ++index)
			if ((cullingMask & (1 << instances[cullingResults[index]].gameObject.layer)) != 0) {
				if (num2 > 0)
					cullingResults[index - num2] = cullingResults[index];
			} else
				++num2;

		var num3 = num1 - num2;
		if (num3 > 0) {
			if (normalsID == 0)
				normalsID = Shader.PropertyToID("_CameraGBufferTexture2Copy");
			commandBuffer.GetTemporaryRT(normalsID, -1, -1);
			commandBuffer.Blit(BuiltinRenderTextureType.GBuffer2, normalsID);
			if (specularID == 0)
				specularID = Shader.PropertyToID("_CameraGBufferTexture1Copy");
			commandBuffer.GetTemporaryRT(specularID, -1, -1);
			commandBuffer.Blit(BuiltinRenderTextureType.GBuffer1, specularID);
			if (mrt == null)
				mrt = new RenderTargetIdentifier[4] {
					BuiltinRenderTextureType.GBuffer0,
					BuiltinRenderTextureType.GBuffer1,
					BuiltinRenderTextureType.GBuffer2,
					BuiltinRenderTextureType.CameraTarget
				};
			commandBuffer.SetRenderTarget(mrt, BuiltinRenderTextureType.CameraTarget);
			if (cullId == 0)
				cullId = Shader.PropertyToID("_Cull");
			if (zTestId == 0)
				zTestId = Shader.PropertyToID("_ZTest");
			for (var index = 0; index < num3; ++index) {
				var cullingResult = cullingResults[index];
				var boundingSphere = boundingSpheres[cullingResult];
				var num4 = magnitude + boundingSphere.radius;
				var inside = (boundingSphere.position - position).sqrMagnitude < num4 * (double)num4;
				instances[cullingResult].PopulateBuffer(commandBuffer, inside);
			}

			commandBuffer.ReleaseTemporaryRT(normalsID);
			commandBuffer.ReleaseTemporaryRT(specularID);
			camera.AddCommandBuffer(CameraEvent.BeforeReflections, commandBuffer);
			bufferCamera = camera;
		}
	}

	private static void OnPostRenderEvent(Camera camera) {
		if (Profiler.enabled)
			Profiler.BeginSample(nameof(DeferredProjector));
		OnPostRenderEvent2(camera);
		if (!Profiler.enabled)
			return;
		Profiler.EndSample();
	}

	private static void OnPostRenderEvent2(Camera camera) {
		if (cullingGroup.targetCamera != camera)
			return;
		cullingGroup.targetCamera = null;
		if (!(bufferCamera == camera))
			return;
		camera.RemoveCommandBuffer(CameraEvent.BeforeReflections, commandBuffer);
		commandBuffer.Clear();
		bufferCamera = null;
	}

	[Serializable]
	public struct Property {
		public string Name;
		public float Value;
	}
}