// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.ImageEffects.Triangles
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.ImageEffects
{
  internal class Triangles
  {
    private static Mesh[] meshes;
    private static int currentTris = 0;

    private static bool HasMeshes()
    {
      if (Triangles.meshes == null)
        return false;
      for (int index = 0; index < Triangles.meshes.Length; ++index)
      {
        if ((Object) null == (Object) Triangles.meshes[index])
          return false;
      }
      return true;
    }

    private static void Cleanup()
    {
      if (Triangles.meshes == null)
        return;
      for (int index = 0; index < Triangles.meshes.Length; ++index)
      {
        if ((Object) null != (Object) Triangles.meshes[index])
        {
          Object.DestroyImmediate((Object) Triangles.meshes[index]);
          Triangles.meshes[index] = (Mesh) null;
        }
      }
      Triangles.meshes = (Mesh[]) null;
    }

    private static Mesh[] GetMeshes(int totalWidth, int totalHeight)
    {
      if (Triangles.HasMeshes() && Triangles.currentTris == totalWidth * totalHeight)
        return Triangles.meshes;
      int max = 21666;
      int num = totalWidth * totalHeight;
      Triangles.currentTris = num;
      Triangles.meshes = new Mesh[Mathf.CeilToInt((float) (1.0 * (double) num / (1.0 * (double) max)))];
      int index = 0;
      for (int triOffset = 0; triOffset < num; triOffset += max)
      {
        int triCount = Mathf.FloorToInt((float) Mathf.Clamp(num - triOffset, 0, max));
        Triangles.meshes[index] = Triangles.GetMesh(triCount, triOffset, totalWidth, totalHeight);
        ++index;
      }
      return Triangles.meshes;
    }

    private static Mesh GetMesh(int triCount, int triOffset, int totalWidth, int totalHeight)
    {
      Mesh mesh = new Mesh();
      mesh.hideFlags = HideFlags.DontSave;
      Vector3[] vector3Array = new Vector3[triCount * 3];
      Vector2[] vector2Array1 = new Vector2[triCount * 3];
      Vector2[] vector2Array2 = new Vector2[triCount * 3];
      int[] numArray = new int[triCount * 3];
      for (int index1 = 0; index1 < triCount; ++index1)
      {
        int index2 = index1 * 3;
        int num = triOffset + index1;
        float x = Mathf.Floor((float) (num % totalWidth)) / (float) totalWidth;
        float y = Mathf.Floor((float) (num / totalWidth)) / (float) totalHeight;
        Vector3 vector3 = new Vector3((float) ((double) x * 2.0 - 1.0), (float) ((double) y * 2.0 - 1.0), 1f);
        vector3Array[index2] = vector3;
        vector3Array[index2 + 1] = vector3;
        vector3Array[index2 + 2] = vector3;
        vector2Array1[index2] = new Vector2(0.0f, 0.0f);
        vector2Array1[index2 + 1] = new Vector2(1f, 0.0f);
        vector2Array1[index2 + 2] = new Vector2(0.0f, 1f);
        vector2Array2[index2] = new Vector2(x, y);
        vector2Array2[index2 + 1] = new Vector2(x, y);
        vector2Array2[index2 + 2] = new Vector2(x, y);
        numArray[index2] = index2;
        numArray[index2 + 1] = index2 + 1;
        numArray[index2 + 2] = index2 + 2;
      }
      mesh.vertices = vector3Array;
      mesh.triangles = numArray;
      mesh.uv = vector2Array1;
      mesh.uv2 = vector2Array2;
      return mesh;
    }
  }
}
