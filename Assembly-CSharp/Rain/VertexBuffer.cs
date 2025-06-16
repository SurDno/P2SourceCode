using System.Collections.Generic;
using UnityEngine;

namespace Rain;

public class VertexBuffer {
	public List<Vector3> vertices = new();
	public List<Color32> colors = new();
	public List<Vector2> uvs = new();
	public List<Vector3> normals = new();
	public List<int> triangles = new();

	public void SetCapacity(int vertexCount, int indexCount) {
		if (vertices.Capacity < vertexCount) {
			vertices.Capacity = vertexCount;
			colors.Capacity = vertexCount;
			uvs.Capacity = vertexCount;
			normals.Capacity = vertexCount;
		}

		if (triangles.Capacity >= indexCount)
			return;
		triangles.Capacity = indexCount;
	}

	public void Clear() {
		vertices.Clear();
		colors.Clear();
		uvs.Clear();
		normals.Clear();
		triangles.Clear();
	}
}