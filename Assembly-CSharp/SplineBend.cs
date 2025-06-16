// Decompiled with JetBrains decompiler
// Type: SplineBend
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class SplineBend : MonoBehaviour
{
  public SplineBendMarker[] markers;
  [HideInInspector]
  public bool showMeshes = false;
  [HideInInspector]
  public bool showTiles = false;
  [HideInInspector]
  public bool showTerrain = false;
  [HideInInspector]
  public bool showUpdate = false;
  [HideInInspector]
  public bool showExport = false;
  [HideInInspector]
  public Mesh initialRenderMesh;
  [HideInInspector]
  public Mesh renderMesh;
  [HideInInspector]
  public Mesh initialCollisionMesh;
  [HideInInspector]
  public Mesh collisionMesh;
  [HideInInspector]
  public int tiles = 1;
  [HideInInspector]
  public float tileOffset = -1f;
  [HideInInspector]
  public bool dropToTerrain = false;
  [HideInInspector]
  public float terrainSeekDist = 1000f;
  [HideInInspector]
  public int terrainLayer = 0;
  [HideInInspector]
  public float terrainOffset = 0.0f;
  [HideInInspector]
  public bool equalize = true;
  [HideInInspector]
  public bool closed;
  [HideInInspector]
  private bool wasClosed;
  [HideInInspector]
  public float markerSize = 1f;
  [HideInInspector]
  public bool displayRolloutOpen = false;
  [HideInInspector]
  public bool settingsRolloutOpen = false;
  [HideInInspector]
  public bool terrainRolloutOpen = false;
  public SplineBend.SplineBendAxis axis = SplineBend.SplineBendAxis.z;
  private Vector3 axisVector;
  [HideInInspector]
  public Transform objFile;

  private static Vector3 GetBeizerPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
  {
    float num = 1f - t;
    return num * num * num * p0 + 3f * t * num * num * p1 + 3f * t * t * num * p2 + t * t * t * p3;
  }

  private static float GetBeizerLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
  {
    float beizerLength = 0.0f;
    Vector3 vector3 = p0;
    for (float t = 0.0f; (double) t < 1.0099999904632568; t += 0.1f)
    {
      Vector3 beizerPoint = SplineBend.GetBeizerPoint(p0, p1, p2, p3, t);
      beizerLength += (vector3 - beizerPoint).magnitude;
      vector3 = beizerPoint;
    }
    return beizerLength;
  }

  public static float GetBeizerLength(SplineBendMarker marker1, SplineBendMarker marker2)
  {
    float num = (marker2.position - marker1.position).magnitude * 0.5f;
    return SplineBend.GetBeizerLength(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position);
  }

  public static Vector3 AlignPoint(
    SplineBendMarker marker1,
    SplineBendMarker marker2,
    float percent,
    Vector3 coords)
  {
    float num1 = (marker2.position - marker1.position).magnitude * 0.5f;
    Vector3 beizerPoint1 = SplineBend.GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position, Mathf.Max(0.0f, percent - 0.01f));
    Vector3 beizerPoint2 = SplineBend.GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position, Mathf.Min(1f, percent + 0.01f));
    Vector3 beizerPoint3 = SplineBend.GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position, percent);
    Vector3 vector3_1 = beizerPoint1 - beizerPoint2;
    Vector3 rhs = Vector3.Slerp(marker1.up, marker2.up, percent);
    Vector3 normalized1 = Vector3.Cross(vector3_1, rhs).normalized;
    Vector3 normalized2 = Vector3.Cross(normalized1, vector3_1).normalized;
    Vector3 vector3_2 = new Vector3(1f, 1f, 1f);
    if (marker1.expandWithScale || marker2.expandWithScale)
    {
      float num2 = percent * percent;
      float num3 = (float) ((1.0 - (1.0 - (double) percent) * (1.0 - (double) percent)) * (double) percent + (double) num2 * (1.0 - (double) percent));
      vector3_2.x = (float) ((double) marker1.transform.localScale.x * (1.0 - (double) num3) + (double) marker2.transform.localScale.x * (double) num3);
      vector3_2.y = (float) ((double) marker1.transform.localScale.y * (1.0 - (double) num3) + (double) marker2.transform.localScale.y * (double) num3);
    }
    return beizerPoint3 + normalized1 * coords.x * vector3_2.x + normalized2 * coords.y * vector3_2.y;
  }

  private void BuildMesh(Mesh mesh, Mesh initialMesh, int num, float offset)
  {
    Vector3[] vertices = initialMesh.vertices;
    Vector2[] uv = initialMesh.uv;
    Vector2[] uv2 = initialMesh.uv2;
    int[] triangles = initialMesh.triangles;
    Vector4[] tangents = initialMesh.tangents;
    Vector3[] vector3Array = new Vector3[vertices.Length * num];
    Vector2[] vector2Array1 = new Vector2[vertices.Length * num];
    Vector2[] vector2Array2 = new Vector2[vertices.Length * num];
    Vector4[] vector4Array = new Vector4[vertices.Length * num];
    bool flag = uv2.Length != 0;
    for (int index1 = 0; index1 < num; ++index1)
    {
      for (int index2 = 0; index2 < vertices.Length; ++index2)
      {
        vector3Array[index1 * vertices.Length + index2] = vertices[index2];
        vector2Array1[index1 * vertices.Length + index2] = uv[index2];
        vector4Array[index1 * vertices.Length + index2] = tangents[index2];
      }
    }
    int[] numArray = new int[triangles.Length * num];
    for (int index3 = 0; index3 < num; ++index3)
    {
      for (int index4 = 0; index4 < triangles.Length; ++index4)
        numArray[index3 * triangles.Length + index4] = triangles[index4] + vertices.Length * index3;
    }
    mesh.Clear(true);
    mesh.vertices = vector3Array;
    mesh.uv = vector2Array1;
    mesh.uv2 = vector2Array2;
    mesh.triangles = numArray;
    mesh.tangents = vector4Array;
    mesh.RecalculateNormals();
  }

  private void RebuildMeshes()
  {
    if ((bool) (UnityEngine.Object) this.renderMesh)
    {
      MeshFilter component = this.GetComponent<MeshFilter>();
      if (!(bool) (UnityEngine.Object) component)
        return;
      this.renderMesh.Clear(true);
      this.BuildMesh(this.renderMesh, this.initialRenderMesh, this.tiles, this.tileOffset);
      component.sharedMesh = this.renderMesh;
      this.renderMesh.RecalculateBounds();
      this.renderMesh.RecalculateNormals();
    }
    if (!(bool) (UnityEngine.Object) this.collisionMesh)
      return;
    MeshCollider component1 = this.GetComponent<MeshCollider>();
    if (!(bool) (UnityEngine.Object) component1)
      return;
    this.collisionMesh.Clear(true);
    this.BuildMesh(this.collisionMesh, this.initialCollisionMesh, this.tiles, this.tileOffset);
    component1.sharedMesh = (Mesh) null;
    component1.sharedMesh = this.collisionMesh;
    this.collisionMesh.RecalculateBounds();
    this.collisionMesh.RecalculateNormals();
  }

  private void Align(Mesh mesh, Mesh initialMesh)
  {
    Vector3[] vector3Array = new Vector3[mesh.vertexCount];
    Vector3[] vertices = initialMesh.vertices;
    for (int index1 = 0; index1 < this.tiles; ++index1)
    {
      for (int index2 = 0; index2 < vertices.Length; ++index2)
      {
        int index3 = index1 * vertices.Length + index2;
        vector3Array[index3] = vertices[index2] + this.axisVector * this.tileOffset * (float) index1;
        if (this.axis == SplineBend.SplineBendAxis.x)
          vector3Array[index3] = new Vector3(-vector3Array[index3].z, vector3Array[index3].y, vector3Array[index3].x);
        else if (this.axis == SplineBend.SplineBendAxis.y)
          vector3Array[index3] = new Vector3(-vector3Array[index3].x, vector3Array[index3].z, vector3Array[index3].y);
      }
    }
    float a1 = float.PositiveInfinity;
    float a2 = float.NegativeInfinity;
    for (int index = 0; index < vector3Array.Length; ++index)
    {
      a1 = Mathf.Min(a1, vector3Array[index].z);
      a2 = Mathf.Max(a2, vector3Array[index].z);
    }
    float a3 = a2 - a1;
    for (int index4 = 0; index4 < vector3Array.Length; ++index4)
    {
      float num1 = Mathf.Clamp01((vector3Array[index4].z - a1) / a3);
      if (Mathf.Approximately(a3, 0.0f))
        num1 = 0.0f;
      int index5 = 0;
      for (int index6 = 1; index6 < this.markers.Length; ++index6)
      {
        if ((double) this.markers[index6].percent >= (double) num1)
        {
          index5 = index6 - 1;
          break;
        }
      }
      if (this.closed && (double) num1 < (double) this.markers[1].percent)
        index5 = 0;
      float percent = (float) (((double) num1 - (double) this.markers[index5].percent) / ((double) this.markers[index5 + 1].percent - (double) this.markers[index5].percent));
      if (this.closed && (double) num1 < (double) this.markers[1].percent)
        percent = num1 / this.markers[1].percent;
      if (this.equalize)
      {
        int index7 = 0;
        for (int index8 = 1; index8 < this.markers[index5].subPoints.Length; ++index8)
        {
          if ((double) this.markers[index5].subPointPercents[index8] >= (double) percent)
          {
            index7 = index8 - 1;
            break;
          }
        }
        float num2 = (percent - this.markers[index5].subPointPercents[index7]) * this.markers[index5].subPointFactors[index7];
        percent = this.markers[index5].subPointMustPercents[index7] + num2;
      }
      vector3Array[index4] = SplineBend.AlignPoint(this.markers[index5], this.markers[index5 + 1], percent, vector3Array[index4]);
    }
    mesh.vertices = vector3Array;
  }

  private void FallToTerrain(
    Mesh mesh,
    Mesh initialMesh,
    float seekDist,
    int layer,
    float offset)
  {
    Vector3[] vertices1 = mesh.vertices;
    float[] numArray = new float[mesh.vertexCount];
    Vector3[] vertices2 = initialMesh.vertices;
    switch (this.axis)
    {
      case SplineBend.SplineBendAxis.x:
      case SplineBend.SplineBendAxis.z:
        for (int index1 = 0; index1 < this.tiles; ++index1)
        {
          for (int index2 = 0; index2 < vertices2.Length; ++index2)
            numArray[index1 * vertices2.Length + index2] = vertices2[index2].y;
        }
        break;
      case SplineBend.SplineBendAxis.y:
        for (int index3 = 0; index3 < this.tiles; ++index3)
        {
          for (int index4 = 0; index4 < vertices2.Length; ++index4)
            numArray[index3 * vertices2.Length + index4] = vertices2[index4].z;
        }
        break;
    }
    int layer1 = this.gameObject.layer;
    this.gameObject.layer = 4;
    for (int index = 0; index < vertices1.Length; ++index)
    {
      RaycastHit hitInfo;
      if (Physics.Raycast(this.transform.TransformPoint(vertices1[index]) with
      {
        y = this.transform.position.y
      } + new Vector3(0.0f, seekDist * 0.5f, 0.0f), -Vector3.up, out hitInfo, seekDist, 1 << layer, QueryTriggerInteraction.Ignore))
        vertices1[index].y = numArray[index] + this.transform.InverseTransformPoint(hitInfo.point).y + offset;
    }
    this.gameObject.layer = layer1;
    mesh.vertices = vertices1;
  }

  private void ResetMarkers() => this.ResetMarkers(this.markers.Length);

  private void ResetMarkers(int count)
  {
    this.markers = new SplineBendMarker[count];
    Mesh mesh;
    if ((bool) (UnityEngine.Object) this.initialRenderMesh)
      mesh = this.initialRenderMesh;
    else if ((bool) (UnityEngine.Object) this.initialCollisionMesh)
      mesh = this.initialCollisionMesh;
    Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
    bool flag = false;
    if ((bool) (UnityEngine.Object) this.initialRenderMesh)
    {
      bounds = this.initialRenderMesh.bounds;
      flag = true;
    }
    else if ((bool) (UnityEngine.Object) this.initialCollisionMesh)
    {
      bounds = this.initialCollisionMesh.bounds;
      flag = true;
    }
    if (!flag && (bool) (UnityEngine.Object) this.GetComponent<MeshFilter>())
    {
      bounds = this.GetComponent<MeshFilter>().sharedMesh.bounds;
      flag = true;
    }
    if (!flag && (bool) (UnityEngine.Object) this.GetComponent<MeshCollider>())
    {
      bounds = this.GetComponent<MeshCollider>().sharedMesh.bounds;
      flag = true;
    }
    if (!flag)
      bounds = new Bounds(Vector3.zero, new Vector3(1f, 1f, 1f));
    float z = bounds.min.z;
    float num = bounds.size.z / (float) (count - 1);
    for (int index = 0; index < count; ++index)
    {
      Transform transform = new GameObject("Marker" + (object) index).transform;
      transform.parent = this.transform;
      transform.localPosition = new Vector3(0.0f, 0.0f, z + num * (float) index);
      this.markers[index] = transform.gameObject.AddComponent<SplineBendMarker>();
    }
  }

  private void AddMarker(Vector3 coords)
  {
    int prewMarkerNum = 0;
    float num = float.PositiveInfinity;
    for (int index = 0; index < this.markers.Length; ++index)
    {
      float sqrMagnitude = (this.markers[index].position - coords).sqrMagnitude;
      if ((double) sqrMagnitude < (double) num)
      {
        prewMarkerNum = index;
        num = sqrMagnitude;
      }
    }
    this.AddMarker(prewMarkerNum, coords);
  }

  public void AddMarker(Ray camRay)
  {
    float num1 = float.PositiveInfinity;
    int prewMarkerNum = 0;
    int index1 = 0;
    for (int index2 = 0; index2 < this.markers.Length; ++index2)
    {
      SplineBendMarker marker = this.markers[index2];
      for (int index3 = 0; index3 < marker.subPoints.Length; ++index3)
      {
        Vector3 vector3 = this.transform.TransformPoint(marker.subPoints[index3]);
        float num2 = Vector3.Dot(camRay.direction, (vector3 - camRay.origin).normalized) * (camRay.origin - vector3).magnitude;
        float magnitude = (camRay.origin + camRay.direction * num2 - vector3).magnitude;
        if ((double) magnitude < (double) num1)
        {
          prewMarkerNum = index2;
          index1 = index3;
          num1 = magnitude;
        }
      }
    }
    Vector3 vector3_1 = this.transform.TransformPoint(this.markers[prewMarkerNum].subPoints[index1]);
    float magnitude1 = (camRay.origin - vector3_1).magnitude;
    this.AddMarker(prewMarkerNum, camRay.origin + camRay.direction * magnitude1);
    this.UpdateNow();
    this.UpdateNow();
  }

  private void AddMarker(int prewMarkerNum, Vector3 coords)
  {
    SplineBendMarker[] splineBendMarkerArray = new SplineBendMarker[this.markers.Length + 1];
    for (int index = 0; index < this.markers.Length; ++index)
    {
      if (index <= prewMarkerNum)
        splineBendMarkerArray[index] = this.markers[index];
      else
        splineBendMarkerArray[index + 1] = this.markers[index];
    }
    Transform transform = new GameObject("Marker" + (object) (prewMarkerNum + 1)).transform;
    transform.parent = this.transform;
    transform.position = coords;
    splineBendMarkerArray[prewMarkerNum + 1] = transform.gameObject.AddComponent<SplineBendMarker>();
    this.markers = splineBendMarkerArray;
  }

  private void RefreshMarkers()
  {
    int length = 0;
    for (int index = 0; index < this.markers.Length; ++index)
    {
      if ((bool) (UnityEngine.Object) this.markers[index])
        ++length;
    }
    SplineBendMarker[] splineBendMarkerArray = new SplineBendMarker[length];
    int index1 = 0;
    for (int index2 = 0; index2 < this.markers.Length; ++index2)
    {
      if ((bool) (UnityEngine.Object) this.markers[index2])
      {
        splineBendMarkerArray[index1] = this.markers[index2];
        ++index1;
      }
    }
    this.markers = splineBendMarkerArray;
  }

  private void RemoveMarker(int num)
  {
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.markers[num].gameObject);
    SplineBendMarker[] splineBendMarkerArray = new SplineBendMarker[this.markers.Length - 1];
    for (int index = 0; index < this.markers.Length - 1; ++index)
      splineBendMarkerArray[index] = index >= num ? this.markers[index + 1] : this.markers[index];
    this.markers = splineBendMarkerArray;
  }

  private void CloseMarkers()
  {
    if (this.closed || (UnityEngine.Object) this.markers[0] == (UnityEngine.Object) this.markers[this.markers.Length - 1])
      return;
    SplineBendMarker[] splineBendMarkerArray = new SplineBendMarker[this.markers.Length + 1];
    for (int index = 0; index < this.markers.Length; ++index)
      splineBendMarkerArray[index] = this.markers[index];
    this.markers = splineBendMarkerArray;
    this.markers[this.markers.Length - 1] = this.markers[0];
    this.UpdateNow();
    this.closed = true;
  }

  private void UnCloseMarkers()
  {
    if (!this.closed || (UnityEngine.Object) this.markers[0] != (UnityEngine.Object) this.markers[this.markers.Length - 1])
      return;
    SplineBendMarker[] splineBendMarkerArray = new SplineBendMarker[this.markers.Length - 1];
    for (int index = 0; index < this.markers.Length - 1; ++index)
      splineBendMarkerArray[index] = this.markers[index];
    this.markers = splineBendMarkerArray;
    this.UpdateNow();
    this.closed = false;
  }

  private void OnEnable()
  {
    if (Environment.CommandLine.Contains("-batchmode"))
      return;
    this.renderMesh = (Mesh) null;
    this.collisionMesh = (Mesh) null;
    this.ForceUpdate();
    MeshFilter component1 = this.GetComponent<MeshFilter>();
    MeshCollider component2 = this.GetComponent<MeshCollider>();
    if ((bool) (UnityEngine.Object) this.renderMesh && (bool) (UnityEngine.Object) component1)
      component1.sharedMesh = this.renderMesh;
    if (!(bool) (UnityEngine.Object) this.collisionMesh || !(bool) (UnityEngine.Object) component2)
      return;
    component2.sharedMesh = (Mesh) null;
    component2.sharedMesh = this.collisionMesh;
  }

  private void OnDisable()
  {
    if (Environment.CommandLine.Contains("-batchmode"))
      return;
    MeshFilter component1 = this.GetComponent<MeshFilter>();
    MeshCollider component2 = this.GetComponent<MeshCollider>();
    if ((bool) (UnityEngine.Object) this.initialRenderMesh && (bool) (UnityEngine.Object) component1)
      component1.sharedMesh = this.initialRenderMesh;
    if (!(bool) (UnityEngine.Object) this.initialCollisionMesh || !(bool) (UnityEngine.Object) component2)
      return;
    component2.sharedMesh = (Mesh) null;
    component2.sharedMesh = this.initialCollisionMesh;
  }

  public void UpdateNow() => this.ForceUpdate(true);

  public void ForceUpdate() => this.ForceUpdate(true);

  public void ForceUpdate(bool refreshCollisionMesh)
  {
    MeshCollider component1 = this.GetComponent<MeshCollider>();
    MeshFilter component2 = this.GetComponent<MeshFilter>();
    switch (this.axis)
    {
      case SplineBend.SplineBendAxis.x:
        this.axisVector = new Vector3(1f, 0.0f, 0.0f);
        break;
      case SplineBend.SplineBendAxis.y:
        this.axisVector = new Vector3(0.0f, 1f, 0.0f);
        break;
      case SplineBend.SplineBendAxis.z:
        this.axisVector = new Vector3(0.0f, 0.0f, 1f);
        break;
    }
    if ((bool) (UnityEngine.Object) this.initialRenderMesh)
      this.tiles = Mathf.Min(this.tiles, Mathf.FloorToInt(65000f / (float) this.initialRenderMesh.vertices.Length));
    else if ((bool) (UnityEngine.Object) this.initialCollisionMesh)
      this.tiles = Mathf.Min(this.tiles, Mathf.FloorToInt(65000f / (float) this.initialCollisionMesh.vertices.Length));
    this.tiles = Mathf.Max(this.tiles, 1);
    if (this.markers == null)
      this.ResetMarkers(2);
    for (int index = 0; index < this.markers.Length; ++index)
    {
      if (!(bool) (UnityEngine.Object) this.markers[index])
        this.RefreshMarkers();
    }
    if (this.markers.Length < 2)
      this.ResetMarkers(2);
    for (int mnum = 0; mnum < this.markers.Length; ++mnum)
      this.markers[mnum].Init(this, mnum);
    if (this.closed)
      this.markers[0].dist = this.markers[this.markers.Length - 2].dist + SplineBend.GetBeizerLength(this.markers[this.markers.Length - 2], this.markers[0]);
    float dist = this.markers[this.markers.Length - 1].dist;
    if (this.closed)
      dist = this.markers[0].dist;
    for (int index = 0; index < this.markers.Length; ++index)
      this.markers[index].percent = this.markers[index].dist / dist;
    if (this.closed && !this.wasClosed)
      this.CloseMarkers();
    if (!this.closed && this.wasClosed)
      this.UnCloseMarkers();
    this.wasClosed = this.closed;
    if ((bool) (UnityEngine.Object) component2 && !(bool) (UnityEngine.Object) this.renderMesh)
    {
      if (!(bool) (UnityEngine.Object) this.initialRenderMesh)
        this.initialRenderMesh = component2.sharedMesh;
      if ((bool) (UnityEngine.Object) this.initialRenderMesh)
      {
        if ((double) this.tileOffset < 0.0)
          this.tileOffset = this.initialRenderMesh.bounds.size.z;
        this.renderMesh = UnityEngine.Object.Instantiate<Mesh>(this.initialRenderMesh);
        this.renderMesh.hideFlags = HideFlags.HideAndDontSave;
        component2.sharedMesh = this.renderMesh;
      }
    }
    if ((bool) (UnityEngine.Object) component1 && !(bool) (UnityEngine.Object) this.collisionMesh)
    {
      if (!(bool) (UnityEngine.Object) this.initialCollisionMesh)
        this.initialCollisionMesh = component1.sharedMesh;
      if ((bool) (UnityEngine.Object) this.initialCollisionMesh)
      {
        if ((double) this.tileOffset < 0.0)
          this.tileOffset = this.initialCollisionMesh.bounds.size.z;
        this.collisionMesh = UnityEngine.Object.Instantiate<Mesh>(this.initialCollisionMesh);
        this.collisionMesh.hideFlags = HideFlags.HideAndDontSave;
        component1.sharedMesh = this.collisionMesh;
      }
    }
    if ((bool) (UnityEngine.Object) this.renderMesh && (bool) (UnityEngine.Object) this.initialRenderMesh && (bool) (UnityEngine.Object) component2)
    {
      if (this.renderMesh.vertexCount != this.initialRenderMesh.vertexCount * this.tiles)
        this.BuildMesh(this.renderMesh, this.initialRenderMesh, this.tiles, 0.0f);
      this.Align(this.renderMesh, this.initialRenderMesh);
      if (this.dropToTerrain)
        this.FallToTerrain(this.renderMesh, this.initialRenderMesh, this.terrainSeekDist, this.terrainLayer, this.terrainOffset);
      this.renderMesh.RecalculateBounds();
      this.renderMesh.RecalculateNormals();
    }
    if (!(bool) (UnityEngine.Object) this.collisionMesh || !(bool) (UnityEngine.Object) this.initialCollisionMesh || !(bool) (UnityEngine.Object) component1)
      return;
    if (this.collisionMesh.vertexCount != this.initialCollisionMesh.vertexCount * this.tiles)
      this.BuildMesh(this.collisionMesh, this.initialCollisionMesh, this.tiles, 0.0f);
    this.Align(this.collisionMesh, this.initialCollisionMesh);
    if (this.dropToTerrain)
      this.FallToTerrain(this.collisionMesh, this.initialCollisionMesh, this.terrainSeekDist, this.terrainLayer, this.terrainOffset);
    if (refreshCollisionMesh && (UnityEngine.Object) component1.sharedMesh == (UnityEngine.Object) this.collisionMesh)
    {
      this.collisionMesh.RecalculateBounds();
      this.collisionMesh.RecalculateNormals();
      component1.sharedMesh = (Mesh) null;
      component1.sharedMesh = this.collisionMesh;
    }
  }

  public enum SplineBendAxis
  {
    x,
    y,
    z,
  }
}
