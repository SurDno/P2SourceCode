using System.Collections;
using System.Collections.Generic;
using Engine.Source.Settings.External;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class TerrainDetailLayer : MonoBehaviour {
	private static DynamicMeshBuffers meshBuffers;
	private static int frame = -1;
	private static int chunksCreatedThisFrame;

	[SerializeField] [FormerlySerializedAs("PrototypeMesh")]
	private Mesh prototypeMesh;

	[SerializeField] [FormerlySerializedAs("Material")]
	private Material material;

	[SerializeField] private TerrainDetailChunk chunkPrefab;

	[SerializeField] [FormerlySerializedAs("LayerIndex")]
	private int layerIndex;

	[SerializeField] [FormerlySerializedAs("CastShadows")]
	private bool castShadows;

	[SerializeField] private BendingCalculation bendingCalculation = BendingCalculation.Off;
	private TerrainDetailChunk[,] chunks;
	private Vector2 chunkWorldSize;
	private Mesh[] chunkMeshes;
	private StaticMeshBuffers prototypeBuffers;
	private int minActiveChunkX = int.MaxValue;
	private int maxActiveChunkX = int.MinValue;
	private int minActiveChunkY = int.MaxValue;
	private int maxActiveChunkY = int.MinValue;
	private Coroutine preloading;
	private bool preloaded;

	public Vector2 DetailTexelSize { get; set; }

	public int MaxChunkCapacity { get; set; }

	public Vector3 PrototypeExtents { get; set; }

	public Terrain Terrain { get; set; }

	public TerrainData TerrainData { get; set; }

	public bool CastShadows => castShadows;

	public int LayerIndex => layerIndex;

	public Material Material => material;

	private IEnumerator Preload() {
		if (chunkPrefab != null) {
			var iLength = chunks.GetLength(0);
			var jLength = chunks.GetLength(1);
			for (var i = 0; i < iLength; ++i) {
				for (var j = 0; j < jLength; ++j) {
					var chunk = chunks[i, j];
					if (chunk == null) {
						chunk = TerrainDetailChunk.Create(chunkPrefab, this, i, j);
						chunks[i, j] = chunk;
					}

					chunk.Preload();
					chunk = null;
				}

				yield return null;
			}
		}

		preloading = null;
	}

	private Mesh CreateMesh(int capacity) {
		var flag1 = prototypeBuffers.HasColors();
		var flag2 = prototypeBuffers.HasUV();
		var flag3 = prototypeBuffers.HasNormals();
		var flag4 = prototypeBuffers.HasTangents();
		var mesh = new Mesh();
		mesh.hideFlags = HideFlags.HideAndDontSave;
		mesh.name = "Terrain Detail " + layerIndex + " (" + capacity + ")";
		if (meshBuffers == null)
			meshBuffers = new DynamicMeshBuffers();
		PrepareClearList(ref meshBuffers.Vertices);
		if (flag1)
			PrepareClearList(ref meshBuffers.Colors);
		if (flag2)
			PrepareClearList(ref meshBuffers.UV);
		PrepareClearList(ref meshBuffers.UV2);
		if (flag3)
			PrepareClearList(ref meshBuffers.Normals);
		if (flag4)
			PrepareClearList(ref meshBuffers.Tangents);
		PrepareClearList(ref meshBuffers.Triangles);
		for (var index1 = 0; index1 < capacity; ++index1) {
			for (var index2 = 0; index2 < prototypeBuffers.TriangleCount; ++index2)
				meshBuffers.Triangles.Add(prototypeBuffers.Triangles[index2] + meshBuffers.Vertices.Count);
			for (var index3 = 0; index3 < prototypeBuffers.VertexCount; ++index3) {
				meshBuffers.Vertices.Add(prototypeBuffers.Vertices[index3]);
				meshBuffers.UV2.Add(new Vector2((0.5f + index1) / capacity, 0.0f));
				if (flag1)
					meshBuffers.Colors.Add(prototypeBuffers.Colors[index3]);
				if (flag2)
					meshBuffers.UV.Add(prototypeBuffers.UV[index3]);
				if (flag3)
					meshBuffers.Normals.Add(prototypeBuffers.Normals[index3]);
				if (flag4)
					meshBuffers.Tangents.Add(prototypeBuffers.Tangents[index3]);
			}
		}

		mesh.SetVertices(meshBuffers.Vertices);
		mesh.SetUVs(1, meshBuffers.UV2);
		mesh.SetTriangles(meshBuffers.Triangles, 0, false);
		if (flag1)
			mesh.SetColors(meshBuffers.Colors);
		if (flag2)
			mesh.SetUVs(0, meshBuffers.UV);
		if (flag3)
			mesh.SetNormals(meshBuffers.Normals);
		if (flag4)
			mesh.SetTangents(meshBuffers.Tangents);
		mesh.bounds = new Bounds(new Vector3(0.5f, 0.5f, 0.5f), Vector3.one);
		mesh.UploadMeshData(true);
		return mesh;
	}

	public Mesh GetMesh(int pointCount, out int capacity) {
		var num = pointCount * 8;
		var index = num / MaxChunkCapacity - 1;
		if (num % MaxChunkCapacity > 0)
			++index;
		capacity = (index + 1) * MaxChunkCapacity / 8;
		if (chunkMeshes[index] == null)
			chunkMeshes[index] = CreateMesh(capacity);
		return chunkMeshes[index];
	}

	private void OnEnable() {
		Camera.onPreCull -= OnPreCullEvent;
		Camera.onPreCull += OnPreCullEvent;
	}

	private void OnDisable() {
		if (preloading != null) {
			StopCoroutine(preloading);
			preloading = null;
		}

		preloaded = false;
		Camera.onPreCull -= OnPreCullEvent;
		TerrainData = null;
		if (chunks != null) {
			var length1 = chunks.GetLength(0);
			var length2 = chunks.GetLength(1);
			for (var index1 = 0; index1 < length1; ++index1) {
				for (var index2 = 0; index2 < length2; ++index2)
					if (chunks[index1, index2] != null) {
						Destroy(chunks[index1, index2].gameObject);
						chunks[index1, index2] = null;
					}
			}

			chunks = null;
		}

		if (chunkMeshes != null) {
			for (var index = 0; index < chunkMeshes.Length; ++index) {
				Destroy(chunkMeshes[index]);
				chunkMeshes[index] = null;
			}

			chunkMeshes = null;
		}

		prototypeBuffers = null;
	}

	private void OnPreCullEvent(Camera camera) {
		if (Profiler.enabled)
			Profiler.BeginSample(nameof(TerrainDetailLayer));
		OnPreCullEvent2(camera);
		if (!Profiler.enabled)
			return;
		Profiler.EndSample();
	}

	private void OnPreCullEvent2(Camera camera) {
		if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableGrass || prototypeMesh == null ||
		    material == null || Terrain == null || chunkPrefab == null || camera == null ||
		    (camera.cullingMask & (1 << gameObject.layer)) == 0)
			return;
		if (chunks == null) {
			TerrainData = Terrain.terrainData;
			DetailTexelSize = new Vector2(TerrainData.size.x / TerrainData.detailWidth,
				TerrainData.size.z / TerrainData.detailHeight);
			chunkWorldSize = DetailTexelSize * 104f;
			var length1 = TerrainData.detailWidth / 104;
			if (TerrainData.detailWidth % 104 > 0)
				++length1;
			var length2 = TerrainData.detailHeight / 104;
			if (TerrainData.detailHeight % 104 > 0)
				++length2;
			chunks = new TerrainDetailChunk[length1, length2];
			minActiveChunkX = int.MaxValue;
			maxActiveChunkX = int.MinValue;
			minActiveChunkY = int.MaxValue;
			maxActiveChunkY = int.MinValue;
			chunkMeshes = new Mesh[8];
			prototypeBuffers = new StaticMeshBuffers(prototypeMesh);
			MaxChunkCapacity = 8192 / prototypeBuffers.VertexCount;
			var bounds = prototypeMesh.bounds;
			var vector3 = -bounds.min;
			var max = bounds.max;
			PrototypeExtents = new Vector3(Mathf.Max(Mathf.Max(vector3.x, vector3.z), Mathf.Max(max.x, max.z)), max.y,
				vector3.y);
			if (bendingCalculation != 0) {
				var vertices = prototypeBuffers.Vertices;
				var color32Array = prototypeBuffers.Colors;
				if (color32Array == null) {
					color32Array = new Color32[prototypeBuffers.VertexCount];
					prototypeBuffers.Colors = color32Array;
				}

				var flag = bendingCalculation == BendingCalculation.Vertical;
				var num1 = !flag ? Mathf.Max(PrototypeExtents.y, PrototypeExtents.x) : PrototypeExtents.y;
				for (var index = 0; index < color32Array.Length; ++index) {
					var color32 = color32Array[index];
					var num2 = !flag ? vertices[index].magnitude : vertices[index].y;
					color32.a = (byte)(Mathf.Clamp01(num2 / num1) * (double)byte.MaxValue + 0.44999998807907104);
					color32Array[index] = color32;
				}
			}
		}

		if (!preloaded && Application.isPlaying &&
		    ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PreloadGrass) {
			preloaded = true;
			preloading = StartCoroutine(Preload());
		}

		var vector = material.GetVector("_DrawDistance");
		var drawDistance = vector.x + vector.y;
		var vector3_1 = camera.transform.position - Terrain.transform.position;
		var num3 = Mathf.FloorToInt((vector3_1.x - drawDistance) / chunkWorldSize.x);
		var num4 = Mathf.FloorToInt((vector3_1.x + drawDistance) / chunkWorldSize.x);
		var num5 = Mathf.FloorToInt((vector3_1.z - drawDistance) / chunkWorldSize.y);
		var num6 = Mathf.FloorToInt((vector3_1.z + drawDistance) / chunkWorldSize.y);
		var upperBound1 = chunks.GetUpperBound(0);
		var upperBound2 = chunks.GetUpperBound(1);
		if (num3 < 0)
			num3 = 0;
		if (num4 > upperBound1)
			num4 = upperBound1;
		if (num5 < 0)
			num5 = 0;
		if (num6 > upperBound2)
			num6 = upperBound2;
		if (minActiveChunkX < num3)
			num3 = minActiveChunkX;
		if (maxActiveChunkX > num4)
			num4 = maxActiveChunkX;
		if (minActiveChunkY < num5)
			num5 = minActiveChunkY;
		if (maxActiveChunkY > num6)
			num6 = maxActiveChunkY;
		minActiveChunkX = int.MaxValue;
		maxActiveChunkX = int.MinValue;
		minActiveChunkY = int.MaxValue;
		maxActiveChunkY = int.MinValue;
		for (var chunkX = num3; chunkX <= num4; ++chunkX) {
			for (var chunkY = num5; chunkY <= num6; ++chunkY)
				if (chunkX < 0 || chunkX > upperBound1 || chunkY < 0 || chunkY > upperBound2)
					Debug.LogError("TerrainDetailLayer : Chunk [" + chunkX + ", " + chunkY + "] is out of bounds [" +
					               upperBound1 + ", " + upperBound2 + "].");
				else {
					var vector3_2 = new Vector3(chunkX * chunkWorldSize.x - PrototypeExtents.x, -PrototypeExtents.z,
						chunkY * chunkWorldSize.y - PrototypeExtents.x);
					var vector3_3 = new Vector3((chunkX + 1) * chunkWorldSize.x + PrototypeExtents.x,
						TerrainData.size.y + PrototypeExtents.y, (chunkY + 1) * chunkWorldSize.y + PrototypeExtents.x);
					var b = vector3_1;
					if (b.x < (double)vector3_2.x)
						b.x = vector3_2.x;
					if (b.y < (double)vector3_2.y)
						b.y = vector3_2.y;
					if (b.z < (double)vector3_2.z)
						b.z = vector3_2.z;
					if (b.x > (double)vector3_3.x)
						b.x = vector3_3.x;
					if (b.y > (double)vector3_3.y)
						b.y = vector3_3.y;
					if (b.z > (double)vector3_3.z)
						b.z = vector3_3.z;
					var num7 = Vector3.Distance(vector3_1, b);
					var flag = num7 < (double)drawDistance;
					if (flag) {
						if (chunks[chunkX, chunkY] == null) {
							if (frame != Time.frameCount) {
								frame = Time.frameCount;
								chunksCreatedThisFrame = 0;
							}

							if (num7 < 3.0 || chunksCreatedThisFrame < 1) {
								++chunksCreatedThisFrame;
								chunks[chunkX, chunkY] = TerrainDetailChunk.Create(chunkPrefab, this, chunkX, chunkY);
								flag = chunks[chunkX, chunkY].UpdateVisibility(vector3_1, drawDistance);
							} else
								flag = false;
						} else
							flag = chunks[chunkX, chunkY].UpdateVisibility(vector3_1, drawDistance);
					} else if (chunks[chunkX, chunkY] != null)
						flag = chunks[chunkX, chunkY].HideAll();

					if (flag) {
						if (chunkX < minActiveChunkX)
							minActiveChunkX = chunkX;
						if (chunkX > maxActiveChunkX)
							maxActiveChunkX = chunkX;
						if (chunkY < minActiveChunkY)
							minActiveChunkY = chunkY;
						if (chunkY > maxActiveChunkY)
							maxActiveChunkY = chunkY;
					}
				}
		}
	}

	public static void PrepareClearList<T>(ref List<T> list) {
		if (list == null)
			list = new List<T>();
		else
			list.Clear();
	}

	public enum BendingCalculation : byte {
		Off,
		Vertical,
		Spherical
	}
}