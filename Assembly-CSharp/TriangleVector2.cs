using System;
using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Data;
using TriangleNet.Geometry;
using UnityEngine;
using Mesh = TriangleNet.Mesh;

public static class TriangleVector2 {
	public static void Triangulate(
		Vector2[] polygon,
		float maxArea,
		out Vector2[] vertices,
		out int[] triangles) {
		var inputGeometry = new InputGeometry();
		AddContour(inputGeometry, polygon, 0);
		var mesh = new Mesh(new Behavior {
			Quality = true,
			MaxArea = maxArea,
			VarArea = true,
			Jettison = true
		});
		mesh.Triangulate(inputGeometry);
		var vertices1 = mesh.vertices;
		var dictionary = new Dictionary<int, int>();
		var length = 0;
		foreach (var key in vertices1.Keys) {
			dictionary.Add(key, length);
			++length;
		}

		vertices = new Vector2[length];
		foreach (var keyValuePair in vertices1) {
			var vertex = keyValuePair.Value;
			vertices[dictionary[keyValuePair.Key]] =
				new Vector2(Convert.ToSingle(vertex.X), Convert.ToSingle(vertex.Y));
		}

		var triangles1 = mesh.triangles;
		var index1 = 0;
		triangles = new int[triangles1.Count * 3];
		foreach (var triangle in triangles1.Values) {
			triangles[index1] = dictionary[triangle.P2];
			var index2 = index1 + 1;
			triangles[index2] = dictionary[triangle.P1];
			var index3 = index2 + 1;
			triangles[index3] = dictionary[triangle.P0];
			index1 = index3 + 1;
		}
	}

	public static void Triangulate(
		Vector2[][] innerContours,
		Vector2[][] outerContours,
		out Vector2[] vertices,
		out bool[] isInner,
		out int[] triangles) {
		var inputGeometry = new InputGeometry();
		for (var index = 0; index < innerContours.Length; ++index)
			AddContour(inputGeometry, innerContours[index], 1);
		var count = inputGeometry.Count;
		if (outerContours != null)
			for (var index = 0; index < outerContours.Length; ++index)
				AddContour(inputGeometry, outerContours[index], 0);
		var mesh = new Mesh();
		mesh.Triangulate(inputGeometry);
		var vertices1 = mesh.vertices;
		vertices = new Vector2[vertices1.Count];
		isInner = new bool[vertices.Length];
		for (var key = 0; key < vertices.Length; ++key) {
			vertices[key] = new Vector2(Convert.ToSingle(vertices1[key].X), Convert.ToSingle(vertices1[key].Y));
			isInner[key] = key < count;
		}

		var triangles1 = mesh.triangles;
		var index1 = 0;
		triangles = new int[triangles1.Count * 3];
		foreach (var triangle in triangles1.Values) {
			triangles[index1] = triangle.P2;
			var index2 = index1 + 1;
			triangles[index2] = triangle.P1;
			var index3 = index2 + 1;
			triangles[index3] = triangle.P0;
			index1 = index3 + 1;
		}
	}

	private static void AddContour(InputGeometry inputGeometry, Vector2[] contour, int marker) {
		var count = inputGeometry.Count;
		for (var index = 0; index < contour.Length; ++index) {
			inputGeometry.AddPoint(contour[index].x, contour[index].y, marker);
			inputGeometry.AddSegment(count + index, index == 0 ? count + contour.Length - 1 : count + index - 1,
				marker);
		}
	}
}