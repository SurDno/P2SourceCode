using System;
using UnityEngine;

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
    private int lastCount = 0;
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
      if ((UnityEngine.Object) this.mesh == (UnityEngine.Object) null)
      {
        this.mesh = new Mesh();
        this.mesh.MarkDynamic();
        this.mesh.name = "Rain";
        component.sharedMesh = this.mesh;
      }
      else
        this.mesh.Clear();
      buffer.Clear();
      buffer.SetCapacity(this.count * 9, this.count * 18);
      this.UpdateSettings();
      for (int index = 0; index < this.count; ++index)
        this.AddRaindrop(index, buffer);
      this.mesh.SetVertices(buffer.vertices);
      this.mesh.SetColors(buffer.colors);
      this.mesh.SetUVs(0, buffer.uvs);
      this.mesh.SetNormals(buffer.normals);
      this.mesh.SetTriangles(buffer.triangles, 0);
      this.UpdateBounds();
      this.lastCount = this.count;
    }

    private void DestroyMesh()
    {
      if (!((UnityEngine.Object) this.mesh != (UnityEngine.Object) null))
        return;
      this.GetComponent<MeshFilter>().sharedMesh = (Mesh) null;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.mesh);
      this.mesh = (Mesh) null;
    }

    private void AddRaindrop(int index, VertexBuffer buffer)
    {
      int num = index * 9;
      Vector3 impactPosition;
      Vector3 originDirection;
      Vector3 splashNormal;
      this.CalculateRaindrop(out impactPosition, out originDirection, out splashNormal);
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
      this.mesh.RecalculateBounds();
      Bounds bounds = this.mesh.bounds;
      Vector3 max = bounds.max;
      max.y += this.raycastLength;
      bounds.max = max;
      this.mesh.bounds = bounds;
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
      Vector2 vector2_2 = UnityEngine.Random.insideUnitCircle * this.diviation - vector2_1;
      originDirection = new Vector3(vector2_2.x, 1f, vector2_2.y).normalized;
      Vector2 vector2_3 = UnityEngine.Random.insideUnitCircle * this.radius;
      Vector3 point = this.playerPosition + new Vector3(vector2_3.x, 0.0f, vector2_3.y) + originDirection * this.raycastLength * 0.5f;
      RaycastHit hitInfo;
      if (Physics.Raycast(this.toWorldMatrix.MultiplyPoint(point), -originDirection, out hitInfo, this.raycastLength, (int) this.collisionMask, QueryTriggerInteraction.Ignore))
      {
        impactPosition = this.toLocalMatrix.MultiplyPoint(hitInfo.point);
        splashNormal = hitInfo.normal;
      }
      else
      {
        impactPosition = point - originDirection * this.raycastLength;
        splashNormal = Vector3.down;
      }
    }

    private void OnDisable() => this.DestroyMesh();

    private void UpdateSettings()
    {
      this.toLocalMatrix = this.transform.worldToLocalMatrix;
      this.toWorldMatrix = this.transform.localToWorldMatrix;
      RainManager instance = RainManager.Instance;
      if (!((UnityEngine.Object) instance != (UnityEngine.Object) null))
        return;
      this.collisionMask = instance.rainColliders;
      this.playerPosition = instance.PlayerPosition;
    }

    public void UpdateMesh(VertexBuffer buffer)
    {
      if ((UnityEngine.Object) this.mesh == (UnityEngine.Object) null || this.count != this.lastCount)
      {
        this.CreateMesh(buffer);
      }
      else
      {
        buffer.Clear();
        buffer.SetCapacity(this.count * 9, 0);
        this.UpdateSettings();
        for (int index = 0; index < this.count; ++index)
        {
          Vector3 impactPosition;
          Vector3 originDirection;
          Vector3 splashNormal;
          this.CalculateRaindrop(out impactPosition, out originDirection, out splashNormal);
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
        this.mesh.SetVertices(buffer.vertices);
        this.mesh.SetNormals(buffer.normals);
        this.UpdateBounds();
      }
    }
  }
}
