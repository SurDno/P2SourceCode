using System;
using TriangleNet.Data;

namespace TriangleNet.Tools
{
  public class AdjacencyMatrix
  {
    private int[] adj;
    private int adj_num;
    private int[] adj_row;
    private int node_num;

    public int[] AdjacencyRow => adj_row;

    public int[] Adjacency => adj;

    public AdjacencyMatrix(Mesh mesh)
    {
      node_num = mesh.vertices.Count;
      adj_row = AdjacencyCount(mesh);
      adj_num = adj_row[node_num] - 1;
      adj = AdjacencySet(mesh, adj_row);
    }

    public int Bandwidth()
    {
      int val1_1 = 0;
      int val1_2 = 0;
      for (int index1 = 0; index1 < node_num; ++index1)
      {
        for (int index2 = adj_row[index1]; index2 <= adj_row[index1 + 1] - 1; ++index2)
        {
          int num = adj[index2 - 1];
          val1_1 = Math.Max(val1_1, index1 - num);
          val1_2 = Math.Max(val1_2, num - index1);
        }
      }
      return val1_1 + 1 + val1_2;
    }

    private int[] AdjacencyCount(Mesh mesh)
    {
      int[] numArray = new int[node_num + 1];
      for (int index = 0; index < node_num; ++index)
        numArray[index] = 1;
      foreach (Triangle triangle in mesh.triangles.Values)
      {
        int id1 = triangle.id;
        int id2 = triangle.vertices[0].id;
        int id3 = triangle.vertices[1].id;
        int id4 = triangle.vertices[2].id;
        int id5 = triangle.neighbors[2].triangle.id;
        if (id5 < 0 || id1 < id5)
        {
          ++numArray[id2];
          ++numArray[id3];
        }
        int id6 = triangle.neighbors[0].triangle.id;
        if (id6 < 0 || id1 < id6)
        {
          ++numArray[id3];
          ++numArray[id4];
        }
        int id7 = triangle.neighbors[1].triangle.id;
        if (id7 < 0 || id1 < id7)
        {
          ++numArray[id4];
          ++numArray[id2];
        }
      }
      for (int nodeNum = node_num; 1 <= nodeNum; --nodeNum)
        numArray[nodeNum] = numArray[nodeNum - 1];
      numArray[0] = 1;
      for (int index = 1; index <= node_num; ++index)
        numArray[index] = numArray[index - 1] + numArray[index];
      return numArray;
    }

    private int[] AdjacencySet(Mesh mesh, int[] rows)
    {
      int[] destinationArray = new int[node_num];
      Array.Copy(rows, destinationArray, node_num);
      int length = rows[node_num] - 1;
      int[] a = new int[length];
      for (int index = 0; index < length; ++index)
        a[index] = -1;
      for (int index = 0; index < node_num; ++index)
      {
        a[destinationArray[index] - 1] = index;
        ++destinationArray[index];
      }
      foreach (Triangle triangle in mesh.triangles.Values)
      {
        int id1 = triangle.id;
        int id2 = triangle.vertices[0].id;
        int id3 = triangle.vertices[1].id;
        int id4 = triangle.vertices[2].id;
        int id5 = triangle.neighbors[2].triangle.id;
        if (id5 < 0 || id1 < id5)
        {
          a[destinationArray[id2] - 1] = id3;
          ++destinationArray[id2];
          a[destinationArray[id3] - 1] = id2;
          ++destinationArray[id3];
        }
        int id6 = triangle.neighbors[0].triangle.id;
        if (id6 < 0 || id1 < id6)
        {
          a[destinationArray[id3] - 1] = id4;
          ++destinationArray[id3];
          a[destinationArray[id4] - 1] = id3;
          ++destinationArray[id4];
        }
        int id7 = triangle.neighbors[1].triangle.id;
        if (id7 < 0 || id1 < id7)
        {
          a[destinationArray[id2] - 1] = id4;
          ++destinationArray[id2];
          a[destinationArray[id4] - 1] = id2;
          ++destinationArray[id4];
        }
      }
      for (int index = 0; index < node_num; ++index)
      {
        int row = rows[index];
        int num = rows[index + 1] - 1;
        HeapSort(a, row - 1, num + 1 - row);
      }
      return a;
    }

    private void CreateHeap(int[] a, int offset, int size)
    {
      for (int index = size / 2 - 1; 0 <= index; --index)
      {
        int num1 = a[offset + index];
        int num2 = index;
        while (true)
        {
          int num3 = 2 * num2 + 1;
          if (size > num3)
          {
            if (num3 + 1 < size && a[offset + num3] < a[offset + num3 + 1])
              ++num3;
            if (num1 < a[offset + num3])
            {
              a[offset + num2] = a[offset + num3];
              num2 = num3;
            }
            else
              break;
          }
          else
            break;
        }
        a[offset + num2] = num1;
      }
    }

    private void HeapSort(int[] a, int offset, int size)
    {
      if (size <= 1)
        return;
      CreateHeap(a, offset, size);
      int num1 = a[offset];
      a[offset] = a[offset + size - 1];
      a[offset + size - 1] = num1;
      for (int size1 = size - 1; 2 <= size1; --size1)
      {
        CreateHeap(a, offset, size1);
        int num2 = a[offset];
        a[offset] = a[offset + size1 - 1];
        a[offset + size1 - 1] = num2;
      }
    }
  }
}
