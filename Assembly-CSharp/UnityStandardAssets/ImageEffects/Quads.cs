﻿using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  internal class Quads
  {
    private static Mesh[] meshes;
    private static int currentQuads;

    private static bool HasMeshes()
    {
      if (meshes == null)
        return false;
      foreach (Object mesh in meshes)
      {
        if (null == mesh)
          return false;
      }
      return true;
    }

    public static void Cleanup()
    {
      if (meshes == null)
        return;
      for (int index = 0; index < meshes.Length; ++index)
      {
        if (null != meshes[index])
        {
          Object.DestroyImmediate(meshes[index]);
          meshes[index] = null;
        }
      }
      meshes = null;
    }

    public static Mesh[] GetMeshes(int totalWidth, int totalHeight)
    {
      if (HasMeshes() && currentQuads == totalWidth * totalHeight)
        return meshes;
      int max = 10833;
      int num = totalWidth * totalHeight;
      currentQuads = num;
      meshes = new Mesh[Mathf.CeilToInt((float) (1.0 * num / (1.0 * max)))];
      int index = 0;
      for (int triOffset = 0; triOffset < num; triOffset += max)
      {
        int triCount = Mathf.FloorToInt(Mathf.Clamp(num - triOffset, 0, max));
        meshes[index] = GetMesh(triCount, triOffset, totalWidth, totalHeight);
        ++index;
      }
      return meshes;
    }

    private static Mesh GetMesh(int triCount, int triOffset, int totalWidth, int totalHeight)
    {
      Mesh mesh = new Mesh();
      mesh.hideFlags = HideFlags.DontSave;
      Vector3[] vector3Array = new Vector3[triCount * 4];
      Vector2[] vector2Array1 = new Vector2[triCount * 4];
      Vector2[] vector2Array2 = new Vector2[triCount * 4];
      int[] numArray = new int[triCount * 6];
      for (int index1 = 0; index1 < triCount; ++index1)
      {
        int index2 = index1 * 4;
        int index3 = index1 * 6;
        int num = triOffset + index1;
        float x = Mathf.Floor(num % totalWidth) / totalWidth;
        float y = Mathf.Floor(num / totalWidth) / totalHeight;
        Vector3 vector3 = new Vector3((float) (x * 2.0 - 1.0), (float) (y * 2.0 - 1.0), 1f);
        vector3Array[index2] = vector3;
        vector3Array[index2 + 1] = vector3;
        vector3Array[index2 + 2] = vector3;
        vector3Array[index2 + 3] = vector3;
        vector2Array1[index2] = new Vector2(0.0f, 0.0f);
        vector2Array1[index2 + 1] = new Vector2(1f, 0.0f);
        vector2Array1[index2 + 2] = new Vector2(0.0f, 1f);
        vector2Array1[index2 + 3] = new Vector2(1f, 1f);
        vector2Array2[index2] = new Vector2(x, y);
        vector2Array2[index2 + 1] = new Vector2(x, y);
        vector2Array2[index2 + 2] = new Vector2(x, y);
        vector2Array2[index2 + 3] = new Vector2(x, y);
        numArray[index3] = index2;
        numArray[index3 + 1] = index2 + 1;
        numArray[index3 + 2] = index2 + 2;
        numArray[index3 + 3] = index2 + 1;
        numArray[index3 + 4] = index2 + 2;
        numArray[index3 + 5] = index2 + 3;
      }
      mesh.vertices = vector3Array;
      mesh.triangles = numArray;
      mesh.uv = vector2Array1;
      mesh.uv2 = vector2Array2;
      return mesh;
    }
  }
}
