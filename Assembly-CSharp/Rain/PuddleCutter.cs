using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rain;

public class PuddleCutter : MonoBehaviour {
	private const string DebugPrefix = "Puddle Cutter : ";
	[Header("Source")] public MeshFilter SourceMeshFilter;
	public UVSet SourceUVSet = UVSet.UV0;
	[Header("Settings")] public Vector3 Size = new(5f, 1f, 5f);
	public Color ColorMask = Color.white;
	public float Elevation = 0.01f;
	[Range(0.0f, 1f)] public float MinYNormal = 0.5f;

	private void AddTriangle(
		List<Vector3> outputVertices,
		List<Vector2> outputPrimaryUV,
		List<Vector2> outputSecondaryUV,
		List<Vector3> outputNormals,
		List<int> outputIndices,
		Vector3[] triangleVertices,
		Vector2[] triangleUV,
		Vector3[] triangleNormals) {
		var count = outputVertices.Count;
		outputIndices.Add(count);
		var num1 = count + 1;
		outputIndices.Add(num1);
		var num2 = num1 + 1;
		outputIndices.Add(num2);
		for (var index = 0; index < 3; ++index) {
			outputVertices.Add(triangleVertices[index]);
			outputPrimaryUV.Add(new Vector2((float)(triangleVertices[index].x / (double)Size.x + 0.5),
				(float)(triangleVertices[index].z / (double)Size.z + 0.5)));
		}

		if (outputSecondaryUV != null)
			for (var index = 0; index < 3; ++index)
				outputSecondaryUV.Add(triangleUV[index]);
		if (outputNormals == null)
			return;
		for (var index = 0; index < 3; ++index)
			outputNormals.Add(triangleNormals[index]);
	}

	public void Build() {
		if (Size.x * 0.5 <= 0.0 || Size.y * 0.5 <= 0.0 || Size.z * 0.5 <= 0.0 || SourceMeshFilter == null)
			return;
		var sharedMesh = SourceMeshFilter.sharedMesh;
		if (sharedMesh == null)
			return;
		if (!sharedMesh.isReadable)
			Debug.LogWarning("Puddle Cutter : Mesh \"" + sharedMesh.name + "\" is not readable.");
		else {
			var vertices = sharedMesh.vertices;
			var triangles = sharedMesh.triangles;
			var normals = sharedMesh.normals;
			if (vertices == null || vertices.Length == 0 || triangles == null || triangles.Length == 0)
				return;
			var matrix4x4 = transform.worldToLocalMatrix * SourceMeshFilter.transform.localToWorldMatrix;
			for (var index = 0; index < vertices.Length; ++index) {
				vertices[index] = matrix4x4.MultiplyPoint(vertices[index]);
				vertices[index].y += Elevation;
				if (normals != null)
					normals[index] = matrix4x4.MultiplyVector(normals[index]);
			}

			Vector2[] vector2Array = null;
			switch (SourceUVSet) {
				case UVSet.UV0:
					vector2Array = sharedMesh.uv;
					break;
				case UVSet.UV1:
					vector2Array = sharedMesh.uv2;
					break;
				case UVSet.UV2:
					vector2Array = sharedMesh.uv3;
					break;
				case UVSet.UV3:
					vector2Array = sharedMesh.uv4;
					break;
			}

			if (SourceUVSet != UVSet.None && vector2Array == null)
				Debug.LogWarning("Puddle Cutter : UV set of mesh \"" + sharedMesh.name +
				                 "\" is empty. None will be used.");
			var outputVertices = new List<Vector3>();
			var outputPrimaryUV = new List<Vector2>();
			var outputSecondaryUV = vector2Array == null ? null : new List<Vector2>();
			var outputNormals = normals == null ? null : new List<Vector3>();
			var outputIndices = new List<int>();
			var polygonVertices = new List<Vector3>(3);
			var polygonUV = vector2Array == null ? null : new List<Vector2>(3);
			var polygonNormals = normals == null ? null : new List<Vector3>(3);
			for (var index = 0; index < triangles.Length; index += 3) {
				polygonVertices.Clear();
				polygonVertices.Add(vertices[triangles[index]]);
				polygonVertices.Add(vertices[triangles[index + 1]]);
				polygonVertices.Add(vertices[triangles[index + 2]]);
				if (vector2Array != null) {
					polygonUV.Clear();
					polygonUV.Add(vector2Array[triangles[index]]);
					polygonUV.Add(vector2Array[triangles[index + 1]]);
					polygonUV.Add(vector2Array[triangles[index + 2]]);
				}

				if (normals != null) {
					polygonNormals.Clear();
					polygonNormals.Add(normals[triangles[index]]);
					polygonNormals.Add(normals[triangles[index + 1]]);
					polygonNormals.Add(normals[triangles[index + 2]]);
				}

				ProcessTriangle(outputVertices, outputPrimaryUV, outputSecondaryUV, outputNormals, outputIndices,
					polygonVertices, polygonUV, polygonNormals);
			}

			var mesh = new Mesh();
			mesh.SetVertices(outputVertices);
			mesh.SetTriangles(outputIndices, 0, true);
			mesh.SetUVs(0, outputPrimaryUV);
			if (outputSecondaryUV != null)
				mesh.SetUVs(1, outputSecondaryUV);
			if (outputNormals != null) {
				mesh.SetNormals(outputNormals);
				mesh.RecalculateTangents();
			}

			if (ColorMask != Color.white) {
				var colorArray = new Color[outputVertices.Count];
				for (var index = 0; index < colorArray.Length; ++index)
					colorArray[index] = ColorMask;
				mesh.colors = colorArray;
			}

			var meshFilter = GetComponent<MeshFilter>();
			if (meshFilter == null)
				meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = mesh;
			var component = GetComponent<MeshCollider>();
			if (!(component != null))
				return;
			component.sharedMesh = mesh;
		}
	}

	private void CutPolygonPositiveSides(
		List<Vector3> polygonVertices,
		List<Vector2> polygonUV,
		List<Vector3> polygonNormals,
		Vector3 extents) {
		CutPolygonSide(polygonVertices, polygonUV, polygonNormals, 0, extents.x);
		if (polygonVertices.Count == 0)
			return;
		CutPolygonSide(polygonVertices, polygonUV, polygonNormals, 2, extents.z);
		if (polygonVertices.Count != 0)
			;
	}

	private void CutPolygonSide(
		List<Vector3> polygonVertices,
		List<Vector2> polygonUV,
		List<Vector3> polygonNormals,
		int axisIndex,
		float extent) {
		var num1 = polygonVertices[0][axisIndex];
		var num2 = polygonVertices[0][axisIndex];
		Vector3 polygonVertex;
		for (var index = 1; index < polygonVertices.Count; ++index) {
			polygonVertex = polygonVertices[index];
			if (polygonVertex[axisIndex] < (double)num1) {
				polygonVertex = polygonVertices[index];
				num1 = polygonVertex[axisIndex];
			}

			polygonVertex = polygonVertices[index];
			if (polygonVertex[axisIndex] > (double)num2) {
				polygonVertex = polygonVertices[index];
				num2 = polygonVertex[axisIndex];
			}
		}

		if (num2 <= (double)extent)
			return;
		if (num1 >= (double)extent) {
			polygonVertices.Clear();
			polygonUV?.Clear();
			polygonNormals?.Clear();
		} else {
			for (var index1 = 0; index1 < polygonVertices.Count; ++index1) {
				polygonVertex = polygonVertices[index1];
				var f1 = polygonVertex[axisIndex] - extent;
				var index2 = index1 + 1;
				if (index2 == polygonVertices.Count)
					index2 = 0;
				polygonVertex = polygonVertices[index2];
				var f2 = polygonVertex[axisIndex] - extent;
				if (Mathf.Sign(f1 * f2) < 0.0) {
					var num3 = Mathf.Abs(f1);
					var num4 = Mathf.Abs(f2);
					var num5 = num3 + num4;
					var vector3_1 = (polygonVertices[index1] * num4 + polygonVertices[index2] * num3) / num5;
					vector3_1[axisIndex] = extent;
					polygonVertices.Insert(index1 + 1, vector3_1);
					if (polygonUV != null) {
						var vector2 = (polygonUV[index1] * num4 + polygonUV[index2] * num3) / num5;
						polygonUV.Insert(index1 + 1, vector2);
					}

					if (polygonNormals != null) {
						var vector3_2 = (polygonNormals[index1] * num4 + polygonNormals[index2] * num3) / num5;
						polygonNormals.Insert(index1 + 1, vector3_2);
					}

					++index1;
				}
			}

			for (var index = 0; index < polygonVertices.Count; ++index) {
				polygonVertex = polygonVertices[index];
				if (polygonVertex[axisIndex] > (double)extent) {
					polygonVertices.RemoveAt(index);
					polygonUV?.RemoveAt(index);
					polygonNormals?.RemoveAt(index);
					--index;
				}
			}
		}
	}

	private void FlipPolygon(List<Vector3> polygonVertices) {
		for (var index = 0; index < polygonVertices.Count; ++index)
			polygonVertices[index] = -polygonVertices[index];
	}

	private void OnDrawGizmosSelected() {
		var vector3_1 = Size * 0.5f;
		var point1 = new Vector3(-vector3_1.x, -vector3_1.y, -vector3_1.z);
		var point2 = new Vector3(-vector3_1.x, -vector3_1.y, vector3_1.z);
		var point3 = new Vector3(-vector3_1.x, vector3_1.y, vector3_1.z);
		var point4 = new Vector3(-vector3_1.x, vector3_1.y, -vector3_1.z);
		var point5 = new Vector3(vector3_1.x, -vector3_1.y, -vector3_1.z);
		var point6 = new Vector3(vector3_1.x, -vector3_1.y, vector3_1.z);
		var point7 = new Vector3(vector3_1.x, vector3_1.y, vector3_1.z);
		var point8 = new Vector3(vector3_1.x, vector3_1.y, -vector3_1.z);
		var localToWorldMatrix = transform.localToWorldMatrix;
		var vector3_2 = localToWorldMatrix.MultiplyPoint3x4(point1);
		var vector3_3 = localToWorldMatrix.MultiplyPoint3x4(point2);
		var vector3_4 = localToWorldMatrix.MultiplyPoint3x4(point3);
		var vector3_5 = localToWorldMatrix.MultiplyPoint3x4(point4);
		var vector3_6 = localToWorldMatrix.MultiplyPoint3x4(point5);
		var vector3_7 = localToWorldMatrix.MultiplyPoint3x4(point6);
		var vector3_8 = localToWorldMatrix.MultiplyPoint3x4(point7);
		var vector3_9 = localToWorldMatrix.MultiplyPoint3x4(point8);
		Gizmos.color = new Color(0.0f, 0.5f, 1f, 0.75f);
		Gizmos.DrawLine(vector3_2, vector3_3);
		Gizmos.DrawLine(vector3_3, vector3_4);
		Gizmos.DrawLine(vector3_4, vector3_5);
		Gizmos.DrawLine(vector3_5, vector3_2);
		Gizmos.DrawLine(vector3_6, vector3_7);
		Gizmos.DrawLine(vector3_7, vector3_8);
		Gizmos.DrawLine(vector3_8, vector3_9);
		Gizmos.DrawLine(vector3_9, vector3_6);
		Gizmos.DrawLine(vector3_2, vector3_6);
		Gizmos.DrawLine(vector3_3, vector3_7);
		Gizmos.DrawLine(vector3_4, vector3_8);
		Gizmos.DrawLine(vector3_5, vector3_9);
	}

	private void OnValidate() {
		if (hideFlags == HideFlags.DontSaveInBuild)
			hideFlags = HideFlags.None;
		if (Size.x < 0.0)
			Size.x = 0.0f;
		if (Size.y < 0.0)
			Size.y = 0.0f;
		if (Size.z >= 0.0)
			return;
		Size.z = 0.0f;
	}

	private void ProcessTriangle(
		List<Vector3> outputVertices,
		List<Vector2> outputPrimaryUV,
		List<Vector2> outputSecondaryUV,
		List<Vector3> outputNormals,
		List<int> outputIndices,
		List<Vector3> polygonVertices,
		List<Vector2> polygonUV,
		List<Vector3> polygonNormals) {
		var vector3_1 = Vector3.Cross(polygonVertices[1] - polygonVertices[0], polygonVertices[2] - polygonVertices[0]);
		vector3_1.Normalize();
		if (vector3_1.y < (double)MinYNormal)
			return;
		var vector3_2 = new Vector3(polygonVertices[0].x, polygonVertices[0].y, polygonVertices[0].z);
		var vector3_3 = vector3_2;
		for (var index1 = 1; index1 < 3; ++index1) {
			for (var index2 = 0; index2 < 3; ++index2) {
				if (polygonVertices[index1][index2] < (double)vector3_2[index2])
					vector3_2[index2] = polygonVertices[index1][index2];
				if (polygonVertices[index1][index2] > (double)vector3_3[index2])
					vector3_3[index2] = polygonVertices[index1][index2];
			}
		}

		if (!new Bounds((vector3_2 + vector3_3) * 0.5f, vector3_3 - vector3_2).Intersects(
			    new Bounds(Vector3.zero, Size)))
			return;
		var extents = Size * 0.5f;
		CutPolygonPositiveSides(polygonVertices, polygonUV, polygonNormals, extents);
		if (polygonVertices.Count == 0)
			return;
		FlipPolygon(polygonVertices);
		CutPolygonPositiveSides(polygonVertices, polygonUV, polygonNormals, extents);
		if (polygonVertices.Count == 0)
			return;
		FlipPolygon(polygonVertices);
		var triangleVertices = new Vector3[3];
		var triangleUV = outputSecondaryUV == null ? null : new Vector2[3];
		var triangleNormals = new Vector3[3];
		while (polygonVertices.Count > 3) {
			var index3 = polygonVertices.Count - 1;
			var index4 = 0;
			var index5 = 1;
			var num1 = Vector3.Distance(polygonVertices[1], polygonVertices[polygonVertices.Count - 1]);
			for (var index6 = 1; index6 < polygonVertices.Count; ++index6) {
				var index7 = index6 - 1;
				var index8 = index6 + 1;
				if (index8 == polygonVertices.Count)
					index8 = 0;
				var num2 = Vector3.Distance(polygonVertices[index7], polygonVertices[index8]);
				if (num2 < (double)num1) {
					num1 = num2;
					index3 = index7;
					index4 = index6;
					index5 = index8;
				}
			}

			triangleVertices[0] = polygonVertices[index3];
			triangleVertices[1] = polygonVertices[index4];
			triangleVertices[2] = polygonVertices[index5];
			polygonVertices.RemoveAt(index4);
			if (outputSecondaryUV != null) {
				triangleUV[0] = polygonUV[index3];
				triangleUV[1] = polygonUV[index4];
				triangleUV[2] = polygonUV[index5];
				polygonUV.RemoveAt(index4);
			}

			if (outputNormals != null) {
				triangleNormals[0] = polygonNormals[index3];
				triangleNormals[1] = polygonNormals[index4];
				triangleNormals[2] = polygonNormals[index5];
				polygonNormals.RemoveAt(index4);
			}

			AddTriangle(outputVertices, outputPrimaryUV, outputSecondaryUV, outputNormals, outputIndices,
				triangleVertices, triangleUV, triangleNormals);
		}

		if (polygonVertices.Count != 3)
			return;
		triangleVertices[0] = polygonVertices[0];
		triangleVertices[1] = polygonVertices[1];
		triangleVertices[2] = polygonVertices[2];
		if (outputSecondaryUV != null) {
			triangleUV[0] = polygonUV[0];
			triangleUV[1] = polygonUV[1];
			triangleUV[2] = polygonUV[2];
		}

		if (outputNormals != null) {
			triangleNormals[0] = polygonNormals[0];
			triangleNormals[1] = polygonNormals[1];
			triangleNormals[2] = polygonNormals[2];
		}

		AddTriangle(outputVertices, outputPrimaryUV, outputSecondaryUV, outputNormals, outputIndices, triangleVertices,
			triangleUV, triangleNormals);
	}

	[Serializable]
	public enum UVSet : byte {
		UV0 = 0,
		UV1 = 1,
		UV2 = 2,
		UV3 = 3,
		None = 255
	}
}