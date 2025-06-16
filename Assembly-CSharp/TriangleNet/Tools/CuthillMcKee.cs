using System;
using TriangleNet.Log;

namespace TriangleNet.Tools;

public class CuthillMcKee {
	private AdjacencyMatrix matrix;
	private int node_num;

	public int[] Renumber(Mesh mesh) {
		node_num = mesh.vertices.Count;
		mesh.Renumber(NodeNumbering.Linear);
		matrix = new AdjacencyMatrix(mesh);
		var num1 = matrix.Bandwidth();
		var rcm = GenerateRcm();
		var perm_inv = PermInverse(node_num, rcm);
		var num2 = PermBandwidth(rcm, perm_inv);
		if (Behavior.Verbose)
			SimpleLog.Instance.Info(string.Format("Reverse Cuthill-McKee (Bandwidth: {0} > {1})", num1, num2));
		return perm_inv;
	}

	private int PermBandwidth(int[] perm, int[] perm_inv) {
		var adjacencyRow = matrix.AdjacencyRow;
		var adjacency = matrix.Adjacency;
		var val1_1 = 0;
		var val1_2 = 0;
		for (var index1 = 0; index1 < node_num; ++index1) {
			for (var index2 = adjacencyRow[perm[index1]]; index2 <= adjacencyRow[perm[index1] + 1] - 1; ++index2) {
				var num = perm_inv[adjacency[index2 - 1]];
				val1_1 = Math.Max(val1_1, index1 - num);
				val1_2 = Math.Max(val1_2, num - index1);
			}
		}

		return val1_1 + 1 + val1_2;
	}

	private int[] GenerateRcm() {
		var rcm = new int[node_num];
		var iccsze = 0;
		var level_num = 0;
		var level_row = new int[node_num + 1];
		var mask = new int[node_num];
		for (var index = 0; index < node_num; ++index)
			mask[index] = 1;
		var num = 1;
		for (var index = 0; index < node_num; ++index)
			if (mask[index] != 0) {
				var root = index;
				FindRoot(ref root, mask, ref level_num, level_row, rcm, num - 1);
				Rcm(root, mask, rcm, num - 1, ref iccsze);
				num += iccsze;
				if (node_num < num)
					return rcm;
			}

		return rcm;
	}

	private void Rcm(int root, int[] mask, int[] perm, int offset, ref int iccsze) {
		var adjacencyRow = matrix.AdjacencyRow;
		var adjacency = matrix.Adjacency;
		var deg = new int[node_num];
		Degree(root, mask, deg, ref iccsze, perm, offset);
		mask[root] = 0;
		if (iccsze <= 1)
			return;
		var num1 = 0;
		var num2 = 1;
		while (num1 < num2) {
			var num3 = num1 + 1;
			num1 = num2;
			for (var index1 = num3; index1 <= num1; ++index1) {
				var index2 = perm[offset + index1 - 1];
				var num4 = adjacencyRow[index2];
				var num5 = adjacencyRow[index2 + 1] - 1;
				var num6 = num2 + 1;
				for (var index3 = num4; index3 <= num5; ++index3) {
					var index4 = adjacency[index3 - 1];
					if (mask[index4] != 0) {
						++num2;
						mask[index4] = 0;
						perm[offset + num2 - 1] = index4;
					}
				}

				if (num2 > num6) {
					var num7 = num6;
					while (num7 < num2) {
						var num8 = num7;
						++num7;
						var num9 = perm[offset + num7 - 1];
						for (; num6 < num8; --num8) {
							var num10 = perm[offset + num8 - 1];
							if (deg[num10 - 1] > deg[num9 - 1])
								perm[offset + num8] = num10;
							else
								break;
						}

						perm[offset + num8] = num9;
					}
				}
			}
		}

		ReverseVector(perm, offset, iccsze);
	}

	private void FindRoot(
		ref int root,
		int[] mask,
		ref int level_num,
		int[] level_row,
		int[] level,
		int offset) {
		var adjacencyRow = matrix.AdjacencyRow;
		var adjacency = matrix.Adjacency;
		var level_num1 = 0;
		GetLevelSet(ref root, mask, ref level_num, level_row, level, offset);
		var num1 = level_row[level_num] - 1;
		if (level_num == 1 || level_num == num1)
			return;
		do {
			var num2 = num1;
			var num3 = level_row[level_num - 1];
			root = level[offset + num3 - 1];
			if (num3 < num1)
				for (var index1 = num3; index1 <= num1; ++index1) {
					var index2 = level[offset + index1 - 1];
					var num4 = 0;
					var num5 = adjacencyRow[index2 - 1];
					var num6 = adjacencyRow[index2] - 1;
					for (var index3 = num5; index3 <= num6; ++index3) {
						var index4 = adjacency[index3 - 1];
						if (mask[index4] > 0)
							++num4;
					}

					if (num4 < num2) {
						root = index2;
						num2 = num4;
					}
				}

			GetLevelSet(ref root, mask, ref level_num1, level_row, level, offset);
			if (level_num1 > level_num)
				level_num = level_num1;
			else
				goto label_15;
		} while (num1 > level_num);

		goto label_13;
		label_15:
		return;
		label_13: ;
	}

	private void GetLevelSet(
		ref int root,
		int[] mask,
		ref int level_num,
		int[] level_row,
		int[] level,
		int offset) {
		var adjacencyRow = matrix.AdjacencyRow;
		var adjacency = matrix.Adjacency;
		mask[root] = 0;
		level[offset] = root;
		level_num = 0;
		var num1 = 0;
		var num2 = 1;
		do {
			var num3 = num1 + 1;
			num1 = num2;
			++level_num;
			level_row[level_num - 1] = num3;
			for (var index1 = num3; index1 <= num1; ++index1) {
				var index2 = level[offset + index1 - 1];
				var num4 = adjacencyRow[index2];
				var num5 = adjacencyRow[index2 + 1] - 1;
				for (var index3 = num4; index3 <= num5; ++index3) {
					var index4 = adjacency[index3 - 1];
					if (mask[index4] != 0) {
						++num2;
						level[offset + num2 - 1] = index4;
						mask[index4] = 0;
					}
				}
			}
		} while (num2 - num1 > 0);

		level_row[level_num] = num1 + 1;
		for (var index = 0; index < num2; ++index)
			mask[level[offset + index]] = 1;
	}

	private void Degree(int root, int[] mask, int[] deg, ref int iccsze, int[] ls, int offset) {
		var adjacencyRow = matrix.AdjacencyRow;
		var adjacency = matrix.Adjacency;
		var num1 = 1;
		ls[offset] = root;
		adjacencyRow[root] = -adjacencyRow[root];
		var num2 = 0;
		iccsze = 1;
		for (; num1 > 0; num1 = iccsze - num2) {
			var num3 = num2 + 1;
			num2 = iccsze;
			for (var index1 = num3; index1 <= num2; ++index1) {
				var l = ls[offset + index1 - 1];
				var num4 = -adjacencyRow[l];
				var num5 = Math.Abs(adjacencyRow[l + 1]) - 1;
				var num6 = 0;
				for (var index2 = num4; index2 <= num5; ++index2) {
					var index3 = adjacency[index2 - 1];
					if (mask[index3] != 0) {
						++num6;
						if (0 <= adjacencyRow[index3]) {
							adjacencyRow[index3] = -adjacencyRow[index3];
							++iccsze;
							ls[offset + iccsze - 1] = index3;
						}
					}
				}

				deg[l] = num6;
			}
		}

		for (var index = 0; index < iccsze; ++index) {
			var l = ls[offset + index];
			adjacencyRow[l] = -adjacencyRow[l];
		}
	}

	private int[] PermInverse(int n, int[] perm) {
		var numArray = new int[node_num];
		for (var index = 0; index < n; ++index)
			numArray[perm[index]] = index;
		return numArray;
	}

	private void ReverseVector(int[] a, int offset, int size) {
		for (var index = 0; index < size / 2; ++index) {
			var num = a[offset + index];
			a[offset + index] = a[offset + size - 1 - index];
			a[offset + size - 1 - index] = num;
		}
	}
}