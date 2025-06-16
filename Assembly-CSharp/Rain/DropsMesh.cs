using System;

namespace Rain
{
  [RequireComponent(typeof (MeshFilter))]
  public class DropsMesh : MonoBehaviour
  {
    public float raycastOrigin = 50f;
    public float raycastLength = 75f;
    public float diviation = 0.05f;
    [Space]
    public float radius = 25f;
    public int count = 128;
    private int lastCount;
    private Mesh mesh;
    private Matrix4x4 toWorldMatrix;
    private Matrix4x4 toLocalMatrix;
    private LayerMask collisionMask;
    private Vector3 playerPosition;

    public void CreateMesh(VertexBuffer buffer)
    {
      if (buffer == null)
        throw new Exception("buffer == null");
      MeshFilter component = this.GetComponent<MeshFilter>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        throw new Exception("filter == null");
      if ((UnityEngine.Object) mesh == (UnityEngine.Object) null)
      {
        mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.name = "Rain";
        component.sharedMesh = mesh;
      }
      else
        mesh.Clear();
      buffer.Clear();
      buffer.SetCapacity(count * 9, count * 18);
      UpdateSettings();
      for (int index = 0; index < count; ++index)
        AddRaindrop(index, buffer);
      mesh.SetVertices(buffer.vertices);
      mesh.SetColors(buffer.colors);
      mesh.SetUVs(0, buffer.uvs);
      mesh.SetNormals(buffer.normals);
      mesh.SetTriangles(buffer.triangles, 0);
      UpdateBounds();
      lastCount = count;
    }

    private void DestroyMesh()
    {
      if (!((UnityEngine.Object) mesh != (UnityEngine.Object) null))
        return;
      this.GetComponent<MeshFilter>().sharedMesh = (Mesh) null;
      UnityEngine.Object.Destroy((UnityEngine.Object) mesh);
      mesh = (Mesh) null;
    }

    private void AddRaindrop(int index, VertexBuffer buffer)
    {
      int num = index * 9;
      Vector3 impactPosition;
      Vector3 originDirection;
      Vector3 splashNormal;
      CalculateRaindrop(out impactPosition, out originDirection, out splashNormal);
      buffer.vertices.Add(impactPosition);
      buffer.vertices.Add(impactPosition);
      buffer.vertices.Add(impactPosition);
      buffer.vertices.Add(impactPosition);
      buffer.vertices.Add(impactPosition);
      buffer.vertices.Add(impactPosition);
      buffer.vertices.Add(impactPosition);
      buffer.vertices.Add(impactPosition);
      buffer.vertices.Add(impactPosition);
      byte a = (byte) UnityEngine.Random.Range(0, 256);
      buffer.colors.Add(new Color32((byte) 0, (byte) 0, (byte) 0, a));
      buffer.colors.Add(new Color32((byte) 0, (byte) 0, byte.MaxValue, a));
      buffer.colors.Add(new Color32((byte) 0, byte.MaxValue, byte.MaxValue, a));
      buffer.colors.Add(new Color32((byte) 0, byte.MaxValue, (byte) 0, a));
      buffer.colors.Add(new Color32(byte.MaxValue, (byte) 128, (byte) 128, a));
      buffer.colors.Add(new Color32(byte.MaxValue, (byte) 0, (byte) 0, a));
      buffer.colors.Add(new Color32(byte.MaxValue, (byte) 0, byte.MaxValue, a));
      buffer.colors.Add(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, a));
      buffer.colors.Add(new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, a));
      Vector2 vector2 = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
      buffer.uvs.Add(vector2);
      buffer.uvs.Add(vector2);
      buffer.uvs.Add(vector2);
      buffer.uvs.Add(vector2);
      buffer.uvs.Add(vector2);
      buffer.uvs.Add(vector2);
      buffer.uvs.Add(vector2);
      buffer.uvs.Add(vector2);
      buffer.uvs.Add(vector2);
      buffer.triangles.Add(num);
      buffer.triangles.Add(num + 1);
      buffer.triangles.Add(num + 2);
      buffer.triangles.Add(num);
      buffer.triangles.Add(num + 2);
      buffer.triangles.Add(num + 3);
      buffer.triangles.Add(num + 4);
      buffer.triangles.Add(num + 5);
      buffer.triangles.Add(num + 6);
      buffer.triangles.Add(num + 4);
      buffer.triangles.Add(num + 6);
      buffer.triangles.Add(num + 7);
      buffer.triangles.Add(num + 4);
      buffer.triangles.Add(num + 7);
      buffer.triangles.Add(num + 8);
      buffer.triangles.Add(num + 4);
      buffer.triangles.Add(num + 8);
      buffer.triangles.Add(num + 5);
      buffer.normals.Add(originDirection);
      buffer.normals.Add(originDirection);
      buffer.normals.Add(originDirection);
      buffer.normals.Add(originDirection);
      buffer.normals.Add(splashNormal);
      buffer.normals.Add(splashNormal);
      buffer.normals.Add(splashNormal);
      buffer.normals.Add(splashNormal);
      buffer.normals.Add(splashNormal);
    }

    private void UpdateBounds()
    {
      mesh.RecalculateBounds();
      Bounds bounds = mesh.bounds;
      Vector3 max = bounds.max;
      max.y += raycastLength;
      bounds.max = max;
      mesh.bounds = bounds;
    }

    private void CalculateRaindrop(
      out Vector3 impactPosition,
      out Vector3 originDirection,
      out Vector3 splashNormal)
    {
      Vector2 vector2_1 = Vector2.zero;
      RainManager instance = RainManager.Instance;
      if ((UnityEngine.Object) instance != (UnityEngine.Object) null)
        vector2_1 = instance.actualWindVector;
      Vector2 vector2_2 = UnityEngine.Random.insideUnitCircle * diviation - vector2_1;
      originDirection = new Vector3(vector2_2.x, 1f, vector2_2.y).normalized;
      Vector2 vector2_3 = UnityEngine.Random.insideUnitCircle * radius;
      Vector3 point = playerPosition + new Vector3(vector2_3.x, 0.0f, vector2_3.y) + originDirection * raycastLength * 0.5f;
      RaycastHit hitInfo;
      if (Physics.Raycast(toWorldMatrix.MultiplyPoint(point), -originDirection, out hitInfo, raycastLength, (int) collisionMask, QueryTriggerInteraction.Ignore))
      {
        impactPosition = toLocalMatrix.MultiplyPoint(hitInfo.point);
        splashNormal = hitInfo.normal;
      }
      else
      {
        impactPosition = point - originDirection * raycastLength;
        splashNormal = Vector3.down;
      }
    }

    private void OnDisable() => DestroyMesh();

    private void UpdateSettings()
    {
      toLocalMatrix = this.transform.worldToLocalMatrix;
      toWorldMatrix = this.transform.localToWorldMatrix;
      RainManager instance = RainManager.Instance;
      if (!((UnityEngine.Object) instance != (UnityEngine.Object) null))
        return;
      collisionMask = instance.rainColliders;
      playerPosition = instance.PlayerPosition;
    }

    public void UpdateMesh(VertexBuffer buffer)
    {
      if ((UnityEngine.Object) mesh == (UnityEngine.Object) null || count != lastCount)
      {
        CreateMesh(buffer);
      }
      else
      {
        buffer.Clear();
        buffer.SetCapacity(count * 9, 0);
        UpdateSettings();
        for (int index = 0; index < count; ++index)
        {
          Vector3 impactPosition;
          Vector3 originDirection;
          Vector3 splashNormal;
          CalculateRaindrop(out impactPosition, out originDirection, out splashNormal);
          buffer.vertices.Add(impactPosition);
          buffer.vertices.Add(impactPosition);
          buffer.vertices.Add(impactPosition);
          buffer.vertices.Add(impactPosition);
          buffer.vertices.Add(impactPosition);
          buffer.vertices.Add(impactPosition);
          buffer.vertices.Add(impactPosition);
          buffer.vertices.Add(impactPosition);
          buffer.vertices.Add(impactPosition);
          buffer.normals.Add(originDirection);
          buffer.normals.Add(originDirection);
          buffer.normals.Add(originDirection);
          buffer.normals.Add(originDirection);
          buffer.normals.Add(splashNormal);
          buffer.normals.Add(splashNormal);
          buffer.normals.Add(splashNormal);
          buffer.normals.Add(splashNormal);
          buffer.normals.Add(splashNormal);
        }
        mesh.SetVertices(buffer.vertices);
        mesh.SetNormals(buffer.normals);
        UpdateBounds();
      }
    }
  }
}
