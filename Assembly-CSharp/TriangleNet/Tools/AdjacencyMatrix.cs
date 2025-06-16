using System;
using TriangleNet.Data;

namespace TriangleNet.Tools;

public class AdjacencyMatrix {
	private int[] adj;
	private int adj_num;
	private int[] adj_row;
	private int node_num;

	public int[] AdjacencyRow => adj_row;

	public int[] Adjacency => adj;

	public AdjacencyMatrix(Mesh mesh) {
		node_num = mesh.vertices.Count;
		adj_row = AdjacencyCount(mesh);
		adj_num = adj_row[node_num] - 1;
		adj = AdjacencySet(mesh, adj_row);
	}

	public int Bandwidth() {
		var val1_1 = 0;
		var val1_2 = 0;
		for (var index1 = 0; index1 < node_num; ++index1) {
			for (var index2 = adj_row[index1]; index2 <= adj_row[index1 + 1] - 1; ++index2) {
				var num = adj[index2 - 1];
				val1_1 = Math.Max(val1_1, index1 - num);
				val1_2 = Math.Max(val1_2, num - index1);
			}
		}

		return val1_1 + 1 + val1_2;
	}

	private int[] AdjacencyCount(Mesh mesh) {
		var numArray = new int[node_num + 1];
		for (var index = 0; index < node_num; ++index)
			numArray[index] = 1;
		foreach (var triangle in mesh.triangles.Values) {
			var id1 = triangle.id;
			var id2 = triangle.vertices[0].id;
			var id3 = triangle.vertices[1].id;
			var id4 = triangle.vertices[2].id;
			var id5 = triangle.neighbors[2].triangle.id;
			if (id5 < 0 || id1 < id5) {
				++numArray[id2];
				++numArray[id3];
			}

			var id6 = triangle.neighbors[0].triangle.id;
			if (id6 < 0 || id1 < id6) {
				++numArray[id3];
				++numArray[id4];
			}

			var id7 = triangle.neighbors[1].triangle.id;
			if (id7 < 0 || id1 < id7) {
				++numArray[id4];
				++numArray[id2];
			}
		}

		for (var nodeNum = node_num; 1 <= nodeNum; --nodeNum)
			numArray[nodeNum] = numArray[nodeNum - 1];
		numArray[0] = 1;
		for (var index = 1; index <= node_num; ++index)
			numArray[index] = numArray[index - 1] + numArray[index];
		return numArray;
	}

	private int[] AdjacencySet(Mesh mesh, int[] rows) {
		var destinationArray = new int[node_num];
		Array.Copy(rows, destinationArray, node_num);
		var length = rows[node_num] - 1;
		var a = new int[length];
		for (var index = 0; index < length; ++index)
			a[index] = -1;
		for (var index = 0; index < node_num; ++index) {
			a[destinationArray[index] - 1] = index;
			++destinationArray[index];
		}

		foreach (var triangle in mesh.triangles.Values) {
			var id1 = triangle.id;
			var id2 = triangle.vertices[0].id;
			var id3 = triangle.vertices[1].id;
			var id4 = triangle.vertices[2].id;
			var id5 = triangle.neighbors[2].triangle.id;
			if (id5 < 0 || id1 < id5) {
				a[destinationArray[id2] - 1] = id3;
				++destinationArray[id2];
				a[destinationArray[id3] - 1] = id2;
				++destinationArray[id3];
			}

			var id6 = triangle.neighbors[0].triangle.id;
			if (id6 < 0 || id1 < id6) {
				a[destinationArray[id3] - 1] = id4;
				++destinationArray[id3];
				a[destinationArray[id4] - 1] = id3;
				++destinationArray[id4];
			}

			var id7 = triangle.neighbors[1].triangle.id;
			if (id7 < 0 || id1 < id7) {
				a[destinationArray[id2] - 1] = id4;
				++destinationArray[id2];
				a[destinationArray[id4] - 1] = id2;
				++destinationArray[id4];
			}
		}

		for (var index = 0; index < node_num; ++index) {
			var row = rows[index];
			var num = rows[index + 1] - 1;
			HeapSort(a, row - 1, num + 1 - row);
		}

		return a;
	}

	private void CreateHeap(int[] a, int offset, int size) {
		for (var index = size / 2 - 1; 0 <= index; --index) {
			var num1 = a[offset + index];
			var num2 = index;
			while (true) {
				var num3 = 2 * num2 + 1;
				if (size > num3) {
					if (num3 + 1 < size && a[offset + num3] < a[offset + num3 + 1])
						++num3;
					if (num1 < a[offset + num3]) {
						a[offset + num2] = a[offset + num3];
						num2 = num3;
					} else
						break;
				} else
					break;
			}

			a[offset + num2] = num1;
		}
	}

	private void HeapSort(int[] a, int offset, int size) {
		if (size <= 1)
			return;
		CreateHeap(a, offset, size);
		var num1 = a[offset];
		a[offset] = a[offset + size - 1];
		a[offset + size - 1] = num1;
		for (var size1 = size - 1; 2 <= size1; --size1) {
			CreateHeap(a, offset, size1);
			var num2 = a[offset];
			a[offset] = a[offset + size1 - 1];
			a[offset + size1 - 1] = num2;
		}
	}
}