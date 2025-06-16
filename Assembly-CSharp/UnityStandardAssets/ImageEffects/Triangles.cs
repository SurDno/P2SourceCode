namespace UnityStandardAssets.ImageEffects
{
  internal class Triangles
  {
    private static Mesh[] meshes;
    private static int currentTris;

    private static bool HasMeshes()
    {
      if (meshes == null)
        return false;
      for (int index = 0; index < meshes.Length; ++index)
      {
        if ((Object) null == (Object) meshes[index])
          return false;
      }
      return true;
    }

    private static void Cleanup()
    {
      if (meshes == null)
        return;
      for (int index = 0; index < meshes.Length; ++index)
      {
        if ((Object) null != (Object) meshes[index])
        {
          Object.DestroyImmediate((Object) meshes[index]);
          meshes[index] = (Mesh) null;
        }
      }
      meshes = (Mesh[]) null;
    }

    private static Mesh[] GetMeshes(int totalWidth, int totalHeight)
    {
      if (HasMeshes() && currentTris == totalWidth * totalHeight)
        return meshes;
      int max = 21666;
      int num = totalWidth * totalHeight;
      currentTris = num;
      meshes = new Mesh[Mathf.CeilToInt((float) (1.0 * num / (1.0 * max)))];
      int index = 0;
      for (int triOffset = 0; triOffset < num; triOffset += max)
      {
        int triCount = Mathf.FloorToInt((float) Mathf.Clamp(num - triOffset, 0, max));
        meshes[index] = GetMesh(triCount, triOffset, totalWidth, totalHeight);
        ++index;
      }
      return meshes;
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
        Vector3 vector3 = new Vector3((float) (x * 2.0 - 1.0), (float) (y * 2.0 - 1.0), 1f);
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
