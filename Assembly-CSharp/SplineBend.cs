using System;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteInEditMode]
public class SplineBend : MonoBehaviour {
	public SplineBendMarker[] markers;
	[HideInInspector] public bool showMeshes;
	[HideInInspector] public bool showTiles;
	[HideInInspector] public bool showTerrain;
	[HideInInspector] public bool showUpdate;
	[HideInInspector] public bool showExport;
	[HideInInspector] public Mesh initialRenderMesh;
	[HideInInspector] public Mesh renderMesh;
	[HideInInspector] public Mesh initialCollisionMesh;
	[HideInInspector] public Mesh collisionMesh;
	[HideInInspector] public int tiles = 1;
	[HideInInspector] public float tileOffset = -1f;
	[HideInInspector] public bool dropToTerrain;
	[HideInInspector] public float terrainSeekDist = 1000f;
	[HideInInspector] public int terrainLayer;
	[HideInInspector] public float terrainOffset;
	[HideInInspector] public bool equalize = true;
	[HideInInspector] public bool closed;
	[HideInInspector] private bool wasClosed;
	[HideInInspector] public float markerSize = 1f;
	[HideInInspector] public bool displayRolloutOpen;
	[HideInInspector] public bool settingsRolloutOpen;
	[HideInInspector] public bool terrainRolloutOpen;
	public SplineBendAxis axis = SplineBendAxis.z;
	private Vector3 axisVector;
	[HideInInspector] public Transform objFile;

	private static Vector3 GetBeizerPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		var num = 1f - t;
		return num * num * num * p0 + 3f * t * num * num * p1 + 3f * t * t * num * p2 + t * t * t * p3;
	}

	private static float GetBeizerLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
		var beizerLength = 0.0f;
		var vector3 = p0;
		for (var t = 0.0f; t < 1.0099999904632568; t += 0.1f) {
			var beizerPoint = GetBeizerPoint(p0, p1, p2, p3, t);
			beizerLength += (vector3 - beizerPoint).magnitude;
			vector3 = beizerPoint;
		}

		return beizerLength;
	}

	public static float GetBeizerLength(SplineBendMarker marker1, SplineBendMarker marker2) {
		var num = (marker2.position - marker1.position).magnitude * 0.5f;
		return GetBeizerLength(marker1.position, marker1.nextHandle + marker1.position,
			marker2.prewHandle + marker2.position, marker2.position);
	}

	public static Vector3 AlignPoint(
		SplineBendMarker marker1,
		SplineBendMarker marker2,
		float percent,
		Vector3 coords) {
		var num1 = (marker2.position - marker1.position).magnitude * 0.5f;
		var beizerPoint1 = GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position,
			marker2.prewHandle + marker2.position, marker2.position, Mathf.Max(0.0f, percent - 0.01f));
		var beizerPoint2 = GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position,
			marker2.prewHandle + marker2.position, marker2.position, Mathf.Min(1f, percent + 0.01f));
		var beizerPoint3 = GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position,
			marker2.prewHandle + marker2.position, marker2.position, percent);
		var vector3_1 = beizerPoint1 - beizerPoint2;
		var rhs = Vector3.Slerp(marker1.up, marker2.up, percent);
		var normalized1 = Vector3.Cross(vector3_1, rhs).normalized;
		var normalized2 = Vector3.Cross(normalized1, vector3_1).normalized;
		var vector3_2 = new Vector3(1f, 1f, 1f);
		if (marker1.expandWithScale || marker2.expandWithScale) {
			var num2 = percent * percent;
			var num3 = (float)((1.0 - (1.0 - percent) * (1.0 - percent)) * percent + num2 * (1.0 - percent));
			vector3_2.x = (float)(marker1.transform.localScale.x * (1.0 - num3) +
			                      marker2.transform.localScale.x * (double)num3);
			vector3_2.y = (float)(marker1.transform.localScale.y * (1.0 - num3) +
			                      marker2.transform.localScale.y * (double)num3);
		}

		return beizerPoint3 + normalized1 * coords.x * vector3_2.x + normalized2 * coords.y * vector3_2.y;
	}

	private void BuildMesh(Mesh mesh, Mesh initialMesh, int num, float offset) {
		var vertices = initialMesh.vertices;
		var uv = initialMesh.uv;
		var uv2 = initialMesh.uv2;
		var triangles = initialMesh.triangles;
		var tangents = initialMesh.tangents;
		var vector3Array = new Vector3[vertices.Length * num];
		var vector2Array1 = new Vector2[vertices.Length * num];
		var vector2Array2 = new Vector2[vertices.Length * num];
		var vector4Array = new Vector4[vertices.Length * num];
		var flag = uv2.Length != 0;
		for (var index1 = 0; index1 < num; ++index1) {
			for (var index2 = 0; index2 < vertices.Length; ++index2) {
				vector3Array[index1 * vertices.Length + index2] = vertices[index2];
				vector2Array1[index1 * vertices.Length + index2] = uv[index2];
				vector4Array[index1 * vertices.Length + index2] = tangents[index2];
			}
		}

		var numArray = new int[triangles.Length * num];
		for (var index3 = 0; index3 < num; ++index3) {
			for (var index4 = 0; index4 < triangles.Length; ++index4)
				numArray[index3 * triangles.Length + index4] = triangles[index4] + vertices.Length * index3;
		}

		mesh.Clear(true);
		mesh.vertices = vector3Array;
		mesh.uv = vector2Array1;
		mesh.uv2 = vector2Array2;
		mesh.triangles = numArray;
		mesh.tangents = vector4Array;
		mesh.RecalculateNormals();
	}

	private void RebuildMeshes() {
		if ((bool)(Object)renderMesh) {
			var component = GetComponent<MeshFilter>();
			if (!(bool)(Object)component)
				return;
			renderMesh.Clear(true);
			BuildMesh(renderMesh, initialRenderMesh, tiles, tileOffset);
			component.sharedMesh = renderMesh;
			renderMesh.RecalculateBounds();
			renderMesh.RecalculateNormals();
		}

		if (!(bool)(Object)collisionMesh)
			return;
		var component1 = GetComponent<MeshCollider>();
		if (!(bool)(Object)component1)
			return;
		collisionMesh.Clear(true);
		BuildMesh(collisionMesh, initialCollisionMesh, tiles, tileOffset);
		component1.sharedMesh = null;
		component1.sharedMesh = collisionMesh;
		collisionMesh.RecalculateBounds();
		collisionMesh.RecalculateNormals();
	}

	private void Align(Mesh mesh, Mesh initialMesh) {
		var vector3Array = new Vector3[mesh.vertexCount];
		var vertices = initialMesh.vertices;
		for (var index1 = 0; index1 < tiles; ++index1) {
			for (var index2 = 0; index2 < vertices.Length; ++index2) {
				var index3 = index1 * vertices.Length + index2;
				vector3Array[index3] = vertices[index2] + axisVector * tileOffset * index1;
				if (axis == SplineBendAxis.x)
					vector3Array[index3] = new Vector3(-vector3Array[index3].z, vector3Array[index3].y,
						vector3Array[index3].x);
				else if (axis == SplineBendAxis.y)
					vector3Array[index3] = new Vector3(-vector3Array[index3].x, vector3Array[index3].z,
						vector3Array[index3].y);
			}
		}

		var a1 = float.PositiveInfinity;
		var a2 = float.NegativeInfinity;
		for (var index = 0; index < vector3Array.Length; ++index) {
			a1 = Mathf.Min(a1, vector3Array[index].z);
			a2 = Mathf.Max(a2, vector3Array[index].z);
		}

		var a3 = a2 - a1;
		for (var index4 = 0; index4 < vector3Array.Length; ++index4) {
			var num1 = Mathf.Clamp01((vector3Array[index4].z - a1) / a3);
			if (Mathf.Approximately(a3, 0.0f))
				num1 = 0.0f;
			var index5 = 0;
			for (var index6 = 1; index6 < markers.Length; ++index6)
				if (markers[index6].percent >= (double)num1) {
					index5 = index6 - 1;
					break;
				}

			if (closed && num1 < (double)markers[1].percent)
				index5 = 0;
			var percent = (float)((num1 - (double)markers[index5].percent) /
			                      (markers[index5 + 1].percent - (double)markers[index5].percent));
			if (closed && num1 < (double)markers[1].percent)
				percent = num1 / markers[1].percent;
			if (equalize) {
				var index7 = 0;
				for (var index8 = 1; index8 < markers[index5].subPoints.Length; ++index8)
					if (markers[index5].subPointPercents[index8] >= (double)percent) {
						index7 = index8 - 1;
						break;
					}

				var num2 = (percent - markers[index5].subPointPercents[index7]) *
				           markers[index5].subPointFactors[index7];
				percent = markers[index5].subPointMustPercents[index7] + num2;
			}

			vector3Array[index4] = AlignPoint(markers[index5], markers[index5 + 1], percent, vector3Array[index4]);
		}

		mesh.vertices = vector3Array;
	}

	private void FallToTerrain(
		Mesh mesh,
		Mesh initialMesh,
		float seekDist,
		int layer,
		float offset) {
		var vertices1 = mesh.vertices;
		var numArray = new float[mesh.vertexCount];
		var vertices2 = initialMesh.vertices;
		switch (axis) {
			case SplineBendAxis.x:
			case SplineBendAxis.z:
				for (var index1 = 0; index1 < tiles; ++index1) {
					for (var index2 = 0; index2 < vertices2.Length; ++index2)
						numArray[index1 * vertices2.Length + index2] = vertices2[index2].y;
				}

				break;
			case SplineBendAxis.y:
				for (var index3 = 0; index3 < tiles; ++index3) {
					for (var index4 = 0; index4 < vertices2.Length; ++index4)
						numArray[index3 * vertices2.Length + index4] = vertices2[index4].z;
				}

				break;
		}

		var layer1 = gameObject.layer;
		gameObject.layer = 4;
		for (var index = 0; index < vertices1.Length; ++index) {
			RaycastHit hitInfo;
			if (Physics.Raycast(transform.TransformPoint(vertices1[index]) with {
					    y = transform.position.y
				    } + new Vector3(0.0f, seekDist * 0.5f, 0.0f), -Vector3.up, out hitInfo, seekDist, 1 << layer,
				    QueryTriggerInteraction.Ignore))
				vertices1[index].y = numArray[index] + transform.InverseTransformPoint(hitInfo.point).y + offset;
		}

		gameObject.layer = layer1;
		mesh.vertices = vertices1;
	}

	private void ResetMarkers() {
		ResetMarkers(markers.Length);
	}

	private void ResetMarkers(int count) {
		markers = new SplineBendMarker[count];
		Mesh mesh;
		if ((bool)(Object)initialRenderMesh)
			mesh = initialRenderMesh;
		else if ((bool)(Object)initialCollisionMesh)
			mesh = initialCollisionMesh;
		var bounds = new Bounds(Vector3.zero, Vector3.zero);
		var flag = false;
		if ((bool)(Object)initialRenderMesh) {
			bounds = initialRenderMesh.bounds;
			flag = true;
		} else if ((bool)(Object)initialCollisionMesh) {
			bounds = initialCollisionMesh.bounds;
			flag = true;
		}

		if (!flag && (bool)(Object)GetComponent<MeshFilter>()) {
			bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
			flag = true;
		}

		if (!flag && (bool)(Object)GetComponent<MeshCollider>()) {
			bounds = GetComponent<MeshCollider>().sharedMesh.bounds;
			flag = true;
		}

		if (!flag)
			bounds = new Bounds(Vector3.zero, new Vector3(1f, 1f, 1f));
		var z = bounds.min.z;
		var num = bounds.size.z / (count - 1);
		for (var index = 0; index < count; ++index) {
			var transform = new GameObject("Marker" + index).transform;
			transform.parent = this.transform;
			transform.localPosition = new Vector3(0.0f, 0.0f, z + num * index);
			markers[index] = transform.gameObject.AddComponent<SplineBendMarker>();
		}
	}

	private void AddMarker(Vector3 coords) {
		var prewMarkerNum = 0;
		var num = float.PositiveInfinity;
		for (var index = 0; index < markers.Length; ++index) {
			var sqrMagnitude = (markers[index].position - coords).sqrMagnitude;
			if (sqrMagnitude < (double)num) {
				prewMarkerNum = index;
				num = sqrMagnitude;
			}
		}

		AddMarker(prewMarkerNum, coords);
	}

	public void AddMarker(Ray camRay) {
		var num1 = float.PositiveInfinity;
		var prewMarkerNum = 0;
		var index1 = 0;
		for (var index2 = 0; index2 < markers.Length; ++index2) {
			var marker = markers[index2];
			for (var index3 = 0; index3 < marker.subPoints.Length; ++index3) {
				var vector3 = transform.TransformPoint(marker.subPoints[index3]);
				var num2 = Vector3.Dot(camRay.direction, (vector3 - camRay.origin).normalized) *
				           (camRay.origin - vector3).magnitude;
				var magnitude = (camRay.origin + camRay.direction * num2 - vector3).magnitude;
				if (magnitude < (double)num1) {
					prewMarkerNum = index2;
					index1 = index3;
					num1 = magnitude;
				}
			}
		}

		var vector3_1 = transform.TransformPoint(markers[prewMarkerNum].subPoints[index1]);
		var magnitude1 = (camRay.origin - vector3_1).magnitude;
		AddMarker(prewMarkerNum, camRay.origin + camRay.direction * magnitude1);
		UpdateNow();
		UpdateNow();
	}

	private void AddMarker(int prewMarkerNum, Vector3 coords) {
		var splineBendMarkerArray = new SplineBendMarker[markers.Length + 1];
		for (var index = 0; index < markers.Length; ++index)
			if (index <= prewMarkerNum)
				splineBendMarkerArray[index] = markers[index];
			else
				splineBendMarkerArray[index + 1] = markers[index];
		var transform = new GameObject("Marker" + (prewMarkerNum + 1)).transform;
		transform.parent = this.transform;
		transform.position = coords;
		splineBendMarkerArray[prewMarkerNum + 1] = transform.gameObject.AddComponent<SplineBendMarker>();
		markers = splineBendMarkerArray;
	}

	private void RefreshMarkers() {
		var length = 0;
		for (var index = 0; index < markers.Length; ++index)
			if ((bool)(Object)markers[index])
				++length;
		var splineBendMarkerArray = new SplineBendMarker[length];
		var index1 = 0;
		for (var index2 = 0; index2 < markers.Length; ++index2)
			if ((bool)(Object)markers[index2]) {
				splineBendMarkerArray[index1] = markers[index2];
				++index1;
			}

		markers = splineBendMarkerArray;
	}

	private void RemoveMarker(int num) {
		DestroyImmediate(markers[num].gameObject);
		var splineBendMarkerArray = new SplineBendMarker[markers.Length - 1];
		for (var index = 0; index < markers.Length - 1; ++index)
			splineBendMarkerArray[index] = index >= num ? markers[index + 1] : markers[index];
		markers = splineBendMarkerArray;
	}

	private void CloseMarkers() {
		if (closed || markers[0] == markers[markers.Length - 1])
			return;
		var splineBendMarkerArray = new SplineBendMarker[markers.Length + 1];
		for (var index = 0; index < markers.Length; ++index)
			splineBendMarkerArray[index] = markers[index];
		markers = splineBendMarkerArray;
		markers[markers.Length - 1] = markers[0];
		UpdateNow();
		closed = true;
	}

	private void UnCloseMarkers() {
		if (!closed || markers[0] != markers[markers.Length - 1])
			return;
		var splineBendMarkerArray = new SplineBendMarker[markers.Length - 1];
		for (var index = 0; index < markers.Length - 1; ++index)
			splineBendMarkerArray[index] = markers[index];
		markers = splineBendMarkerArray;
		UpdateNow();
		closed = false;
	}

	private void OnEnable() {
		if (Environment.CommandLine.Contains("-batchmode"))
			return;
		renderMesh = null;
		collisionMesh = null;
		ForceUpdate();
		var component1 = GetComponent<MeshFilter>();
		var component2 = GetComponent<MeshCollider>();
		if ((bool)(Object)renderMesh && (bool)(Object)component1)
			component1.sharedMesh = renderMesh;
		if (!(bool)(Object)collisionMesh || !(bool)(Object)component2)
			return;
		component2.sharedMesh = null;
		component2.sharedMesh = collisionMesh;
	}

	private void OnDisable() {
		if (Environment.CommandLine.Contains("-batchmode"))
			return;
		var component1 = GetComponent<MeshFilter>();
		var component2 = GetComponent<MeshCollider>();
		if ((bool)(Object)initialRenderMesh && (bool)(Object)component1)
			component1.sharedMesh = initialRenderMesh;
		if (!(bool)(Object)initialCollisionMesh || !(bool)(Object)component2)
			return;
		component2.sharedMesh = null;
		component2.sharedMesh = initialCollisionMesh;
	}

	public void UpdateNow() {
		ForceUpdate(true);
	}

	public void ForceUpdate() {
		ForceUpdate(true);
	}

	public void ForceUpdate(bool refreshCollisionMesh) {
		var component1 = GetComponent<MeshCollider>();
		var component2 = GetComponent<MeshFilter>();
		switch (axis) {
			case SplineBendAxis.x:
				axisVector = new Vector3(1f, 0.0f, 0.0f);
				break;
			case SplineBendAxis.y:
				axisVector = new Vector3(0.0f, 1f, 0.0f);
				break;
			case SplineBendAxis.z:
				axisVector = new Vector3(0.0f, 0.0f, 1f);
				break;
		}

		if ((bool)(Object)initialRenderMesh)
			tiles = Mathf.Min(tiles, Mathf.FloorToInt(65000f / initialRenderMesh.vertices.Length));
		else if ((bool)(Object)initialCollisionMesh)
			tiles = Mathf.Min(tiles, Mathf.FloorToInt(65000f / initialCollisionMesh.vertices.Length));
		tiles = Mathf.Max(tiles, 1);
		if (markers == null)
			ResetMarkers(2);
		for (var index = 0; index < markers.Length; ++index)
			if (!(bool)(Object)markers[index])
				RefreshMarkers();
		if (markers.Length < 2)
			ResetMarkers(2);
		for (var mnum = 0; mnum < markers.Length; ++mnum)
			markers[mnum].Init(this, mnum);
		if (closed)
			markers[0].dist = markers[markers.Length - 2].dist +
			                  GetBeizerLength(markers[markers.Length - 2], markers[0]);
		var dist = markers[markers.Length - 1].dist;
		if (closed)
			dist = markers[0].dist;
		for (var index = 0; index < markers.Length; ++index)
			markers[index].percent = markers[index].dist / dist;
		if (closed && !wasClosed)
			CloseMarkers();
		if (!closed && wasClosed)
			UnCloseMarkers();
		wasClosed = closed;
		if ((bool)(Object)component2 && !(bool)(Object)renderMesh) {
			if (!(bool)(Object)initialRenderMesh)
				initialRenderMesh = component2.sharedMesh;
			if ((bool)(Object)initialRenderMesh) {
				if (tileOffset < 0.0)
					tileOffset = initialRenderMesh.bounds.size.z;
				renderMesh = Instantiate(initialRenderMesh);
				renderMesh.hideFlags = HideFlags.HideAndDontSave;
				component2.sharedMesh = renderMesh;
			}
		}

		if ((bool)(Object)component1 && !(bool)(Object)collisionMesh) {
			if (!(bool)(Object)initialCollisionMesh)
				initialCollisionMesh = component1.sharedMesh;
			if ((bool)(Object)initialCollisionMesh) {
				if (tileOffset < 0.0)
					tileOffset = initialCollisionMesh.bounds.size.z;
				collisionMesh = Instantiate(initialCollisionMesh);
				collisionMesh.hideFlags = HideFlags.HideAndDontSave;
				component1.sharedMesh = collisionMesh;
			}
		}

		if ((bool)(Object)renderMesh && (bool)(Object)initialRenderMesh && (bool)(Object)component2) {
			if (renderMesh.vertexCount != initialRenderMesh.vertexCount * tiles)
				BuildMesh(renderMesh, initialRenderMesh, tiles, 0.0f);
			Align(renderMesh, initialRenderMesh);
			if (dropToTerrain)
				FallToTerrain(renderMesh, initialRenderMesh, terrainSeekDist, terrainLayer, terrainOffset);
			renderMesh.RecalculateBounds();
			renderMesh.RecalculateNormals();
		}

		if (!(bool)(Object)collisionMesh || !(bool)(Object)initialCollisionMesh || !(bool)(Object)component1)
			return;
		if (collisionMesh.vertexCount != initialCollisionMesh.vertexCount * tiles)
			BuildMesh(collisionMesh, initialCollisionMesh, tiles, 0.0f);
		Align(collisionMesh, initialCollisionMesh);
		if (dropToTerrain)
			FallToTerrain(collisionMesh, initialCollisionMesh, terrainSeekDist, terrainLayer, terrainOffset);
		if (refreshCollisionMesh && component1.sharedMesh == collisionMesh) {
			collisionMesh.RecalculateBounds();
			collisionMesh.RecalculateNormals();
			component1.sharedMesh = null;
			component1.sharedMesh = collisionMesh;
		}
	}

	public enum SplineBendAxis {
		x,
		y,
		z
	}
}