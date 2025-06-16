using System.Collections.Generic;
using Engine.Source.Settings.External;
using UnityEngine;

public class TerrainDetailChunk : MonoBehaviour {
	private static List<Byte2> pointList;
	private static List<int> indexList;
	private static List<Vector3> boundList;
	private static int frame = -1;
	private static int renderersCreatedThisFrame;
	[SerializeField] private TerrainDetailRenderer rendererPrefab;
	private TerrainDetailRenderer[] renderers;
	private byte[] invisibilityFrameCount;
	private int[] indices;
	private Vector3[] bounds;
	private bool needsUpdate;

	public TerrainDetailLayer Layer { get; set; }

	public int ChunkX { get; set; }

	public int ChunkY { get; set; }

	public Byte2[] Points { get; set; }

	public void Preload() {
		if (rendererPrefab == null)
			return;
		for (var index = 0; index < renderers.Length; ++index)
			if (renderers[index] == null) {
				var terrainDetailRenderer = TerrainDetailRenderer.Create(rendererPrefab, this, indices[index * 2],
					indices[index * 2 + 1], out bounds[index * 2], out bounds[index * 2 + 1]);
				renderers[index] = terrainDetailRenderer;
				terrainDetailRenderer.Hide();
			}
	}

	public bool UpdateVisibility(Vector3 terrainSpaceCameraPosition, float drawDistance) {
		if (rendererPrefab == null)
			return false;
		needsUpdate = false;
		for (var index1 = 0; index1 < renderers.Length; ++index1) {
			var index2 = index1;
			var b = terrainSpaceCameraPosition;
			var bound1 = bounds[index1 * 2];
			var bound2 = bounds[index1 * 2 + 1];
			if (b.x < (double)bound1.x)
				b.x = bound1.x;
			if (b.y < (double)bound1.y)
				b.y = bound1.y;
			if (b.z < (double)bound1.z)
				b.z = bound1.z;
			if (b.x > (double)bound2.x)
				b.x = bound2.x;
			if (b.y > (double)bound2.y)
				b.y = bound2.y;
			if (b.z > (double)bound2.z)
				b.z = bound2.z;
			var num = Vector3.Distance(terrainSpaceCameraPosition, b);
			if (num < (double)drawDistance) {
				if (renderers[index2] == null) {
					if (frame != Time.frameCount) {
						frame = Time.frameCount;
						renderersCreatedThisFrame = 0;
					}

					if (num < 3.0 || renderersCreatedThisFrame < 1) {
						++renderersCreatedThisFrame;
						renderers[index2] = TerrainDetailRenderer.Create(rendererPrefab, this, indices[index2 * 2],
							indices[index2 * 2 + 1], out bounds[index2 * 2], out bounds[index2 * 2 + 1]);
						needsUpdate = true;
					}
				} else {
					renderers[index2].Show();
					needsUpdate = true;
				}

				invisibilityFrameCount[index2] = 0;
			} else
				needsUpdate = HideRenderer(index2) || needsUpdate;
		}

		return needsUpdate;
	}

	public bool HideRenderer(int index) {
		if (!(renderers[index] != null))
			return false;
		if (Application.isPlaying && ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PreloadGrass) {
			renderers[index].Hide();
			return false;
		}

		if (invisibilityFrameCount[index] == 0) {
			renderers[index].Hide();
			invisibilityFrameCount[index] = 1;
			return true;
		}

		if (invisibilityFrameCount[index] < 128) {
			++invisibilityFrameCount[index];
			return true;
		}

		Destroy(renderers[index].gameObject);
		renderers[index] = null;
		return false;
	}

	public bool HideAll(bool forceDestroy = false) {
		if (!needsUpdate)
			return false;
		needsUpdate = false;
		for (var index = 0; index < renderers.Length; ++index)
			if (forceDestroy) {
				if (renderers[index] != null) {
					Destroy(renderers[index].gameObject);
					renderers[index] = null;
				}
			} else
				needsUpdate = HideRenderer(index) || needsUpdate;

		return needsUpdate;
	}

	public static TerrainDetailChunk Create(
		TerrainDetailChunk prefab,
		TerrainDetailLayer layer,
		int chunkX,
		int chunkY) {
		TerrainDetailLayer.PrepareClearList(ref pointList);
		TerrainDetailLayer.PrepareClearList(ref indexList);
		TerrainDetailLayer.PrepareClearList(ref boundList);
		var xBase = chunkX * 104;
		var yBase = chunkY * 104;
		var num1 = (chunkX + 1) * 104;
		if (num1 > layer.TerrainData.detailWidth)
			num1 = layer.TerrainData.detailWidth;
		var width = num1 - xBase;
		var num2 = (chunkY + 1) * 104;
		if (num2 > layer.TerrainData.detailHeight)
			num2 = layer.TerrainData.detailHeight;
		var height = num2 - yBase;
		var detailLayer = layer.TerrainData.GetDetailLayer(xBase, yBase, width, height, layer.LayerIndex);
		for (var x = 0; x < width; ++x) {
			for (var y = 0; y < height; ++y) {
				var num3 = detailLayer[y, x];
				if (num3 != 0)
					for (var index = 0; index < num3; ++index)
						pointList.Add(new Byte2((byte)x, (byte)y));
			}
		}

		var f = pointList.Count / layer.MaxChunkCapacity;
		if (pointList.Count % layer.MaxChunkCapacity > 0)
			++f;
		var num4 = (int)Mathf.Sqrt(f);
		for (var index1 = 0; index1 < num4; ++index1) {
			var num5 = index1 * f / num4;
			var num6 = (index1 + 1) * f / num4;
			var index2 = num5 * pointList.Count / f;
			var count = num6 * pointList.Count / f - index2;
			pointList.Sort(index2, count, new Byte2.YComparer());
			for (var index3 = num5; index3 < num6; ++index3) {
				var num7 = index3 * pointList.Count / f;
				var num8 = (index3 + 1) * pointList.Count / f - num7;
				if (num8 > layer.MaxChunkCapacity) {
					Debug.LogWarning("Grass : Renderer exceeds mesh capacity (points.Count = " + pointList.Count +
					                 ", maxChunkCapacity = " + layer.MaxChunkCapacity + ", rendererCount = " + f +
					                 ", cols = " + num4 + ", colFirstRenderer = " + num5 + ", colAfterLastRenderer = " +
					                 num6 + ", colFirstPoint = " + index2 + ", colPointCount = " + count +
					                 ", rendererFirstPoint = " + num7 + ", rendererPointCount = " + num8);
					num8 = layer.MaxChunkCapacity;
				}

				var num9 = byte.MaxValue;
				byte num10 = 0;
				var num11 = byte.MaxValue;
				byte num12 = 0;
				for (var index4 = 0; index4 < num8; ++index4) {
					var point = pointList[index4 + num7];
					if (num9 > point.X)
						num9 = point.X;
					if (num11 > point.Y)
						num11 = point.Y;
					if (num10 < point.X)
						num10 = point.X;
					if (num12 < point.Y)
						num12 = point.Y;
				}

				indexList.Add(num7);
				indexList.Add(num8);
				boundList.Add(new Vector3((xBase + num9) * layer.DetailTexelSize.x - layer.PrototypeExtents.x,
					-layer.PrototypeExtents.z, (yBase + num11) * layer.DetailTexelSize.y - layer.PrototypeExtents.x));
				boundList.Add(new Vector3((xBase + num10 + 1) * layer.DetailTexelSize.x + layer.PrototypeExtents.x,
					layer.TerrainData.size.y + layer.PrototypeExtents.y,
					(yBase + num12 + 1) * layer.DetailTexelSize.y + layer.PrototypeExtents.x));
			}
		}

		var terrainDetailChunk = Instantiate(prefab, layer.transform, false);
		var gameObject = terrainDetailChunk.gameObject;
		gameObject.transform.localPosition =
			new Vector3(xBase * layer.DetailTexelSize.x, 0.0f, yBase * layer.DetailTexelSize.y);
		gameObject.name = "Chunk (" + chunkX + ", " + chunkY + ")";
		gameObject.layer = layer.gameObject.layer;
		if (Application.isPlaying)
			gameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
		else
			gameObject.hideFlags = HideFlags.HideAndDontSave;
		terrainDetailChunk.renderers = new TerrainDetailRenderer[f];
		terrainDetailChunk.invisibilityFrameCount = new byte[f];
		terrainDetailChunk.Points = pointList.ToArray();
		terrainDetailChunk.indices = indexList.ToArray();
		terrainDetailChunk.bounds = boundList.ToArray();
		terrainDetailChunk.Layer = layer;
		terrainDetailChunk.ChunkX = chunkX;
		terrainDetailChunk.ChunkY = chunkY;
		return terrainDetailChunk;
	}
}