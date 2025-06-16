using System;

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
  public SplineBendAxis axis = SplineBendAxis.z;
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
    for (float t = 0.0f; t < 1.0099999904632568; t += 0.1f)
    {
      Vector3 beizerPoint = GetBeizerPoint(p0, p1, p2, p3, t);
      beizerLength += (vector3 - beizerPoint).magnitude;
      vector3 = beizerPoint;
    }
    return beizerLength;
  }

  public static float GetBeizerLength(SplineBendMarker marker1, SplineBendMarker marker2)
  {
    float num = (marker2.position - marker1.position).magnitude * 0.5f;
    return GetBeizerLength(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position);
  }

  public static Vector3 AlignPoint(
    SplineBendMarker marker1,
    SplineBendMarker marker2,
    float percent,
    Vector3 coords)
  {
    float num1 = (marker2.position - marker1.position).magnitude * 0.5f;
    Vector3 beizerPoint1 = GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position, Mathf.Max(0.0f, percent - 0.01f));
    Vector3 beizerPoint2 = GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position, Mathf.Min(1f, percent + 0.01f));
    Vector3 beizerPoint3 = GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position, percent);
    Vector3 vector3_1 = beizerPoint1 - beizerPoint2;
    Vector3 rhs = Vector3.Slerp(marker1.up, marker2.up, percent);
    Vector3 normalized1 = Vector3.Cross(vector3_1, rhs).normalized;
    Vector3 normalized2 = Vector3.Cross(normalized1, vector3_1).normalized;
    Vector3 vector3_2 = new Vector3(1f, 1f, 1f);
    if (marker1.expandWithScale || marker2.expandWithScale)
    {
      float num2 = percent * percent;
      float num3 = (float) ((1.0 - (1.0 - percent) * (1.0 - percent)) * percent + num2 * (1.0 - percent));
      vector3_2.x = (float) ((double) marker1.transform.localScale.x * (1.0 - num3) + (double) marker2.transform.localScale.x * num3);
      vector3_2.y = (float) ((double) marker1.transform.localScale.y * (1.0 - num3) + (double) marker2.transform.localScale.y * num3);
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
    if ((bool) (UnityEngine.Object) renderMesh)
    {
      MeshFilter component = this.GetComponent<MeshFilter>();
      if (!(bool) (UnityEngine.Object) component)
        return;
      renderMesh.Clear(true);
      BuildMesh(renderMesh, initialRenderMesh, tiles, tileOffset);
      component.sharedMesh = renderMesh;
      renderMesh.RecalculateBounds();
      renderMesh.RecalculateNormals();
    }
    if (!(bool) (UnityEngine.Object) collisionMesh)
      return;
    MeshCollider component1 = this.GetComponent<MeshCollider>();
    if (!(bool) (UnityEngine.Object) component1)
      return;
    collisionMesh.Clear(true);
    BuildMesh(collisionMesh, initialCollisionMesh, tiles, tileOffset);
    component1.sharedMesh = (Mesh) null;
    component1.sharedMesh = collisionMesh;
    collisionMesh.RecalculateBounds();
    collisionMesh.RecalculateNormals();
  }

  private void Align(Mesh mesh, Mesh initialMesh)
  {
    Vector3[] vector3Array = new Vector3[mesh.vertexCount];
    Vector3[] vertices = initialMesh.vertices;
    for (int index1 = 0; index1 < tiles; ++index1)
    {
      for (int index2 = 0; index2 < vertices.Length; ++index2)
      {
        int index3 = index1 * vertices.Length + index2;
        vector3Array[index3] = vertices[index2] + axisVector * tileOffset * (float) index1;
        if (axis == SplineBendAxis.x)
          vector3Array[index3] = new Vector3(-vector3Array[index3].z, vector3Array[index3].y, vector3Array[index3].x);
        else if (axis == SplineBendAxis.y)
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
      for (int index6 = 1; index6 < markers.Length; ++index6)
      {
        if (markers[index6].percent >= (double) num1)
        {
          index5 = index6 - 1;
          break;
        }
      }
      if (closed && num1 < (double) markers[1].percent)
        index5 = 0;
      float percent = (float) ((num1 - (double) markers[index5].percent) / (markers[index5 + 1].percent - (double) markers[index5].percent));
      if (closed && num1 < (double) markers[1].percent)
        percent = num1 / markers[1].percent;
      if (equalize)
      {
        int index7 = 0;
        for (int index8 = 1; index8 < markers[index5].subPoints.Length; ++index8)
        {
          if (markers[index5].subPointPercents[index8] >= (double) percent)
          {
            index7 = index8 - 1;
            break;
          }
        }
        float num2 = (percent - markers[index5].subPointPercents[index7]) * markers[index5].subPointFactors[index7];
        percent = markers[index5].subPointMustPercents[index7] + num2;
      }
      vector3Array[index4] = AlignPoint(markers[index5], markers[index5 + 1], percent, vector3Array[index4]);
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
    switch (axis)
    {
      case SplineBendAxis.x:
      case SplineBendAxis.z:
        for (int index1 = 0; index1 < tiles; ++index1)
        {
          for (int index2 = 0; index2 < vertices2.Length; ++index2)
            numArray[index1 * vertices2.Length + index2] = vertices2[index2].y;
        }
        break;
      case SplineBendAxis.y:
        for (int index3 = 0; index3 < tiles; ++index3)
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

  private void ResetMarkers() => ResetMarkers(markers.Length);

  private void ResetMarkers(int count)
  {
    markers = new SplineBendMarker[count];
    Mesh mesh;
    if ((bool) (UnityEngine.Object) initialRenderMesh)
      mesh = initialRenderMesh;
    else if ((bool) (UnityEngine.Object) initialCollisionMesh)
      mesh = initialCollisionMesh;
    Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
    bool flag = false;
    if ((bool) (UnityEngine.Object) initialRenderMesh)
    {
      bounds = initialRenderMesh.bounds;
      flag = true;
    }
    else if ((bool) (UnityEngine.Object) initialCollisionMesh)
    {
      bounds = initialCollisionMesh.bounds;
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
      Transform transform = new GameObject("Marker" + index).transform;
      transform.parent = this.transform;
      transform.localPosition = new Vector3(0.0f, 0.0f, z + num * index);
      markers[index] = transform.gameObject.AddComponent<SplineBendMarker>();
    }
  }

  private void AddMarker(Vector3 coords)
  {
    int prewMarkerNum = 0;
    float num = float.PositiveInfinity;
    for (int index = 0; index < markers.Length; ++index)
    {
      float sqrMagnitude = (markers[index].position - coords).sqrMagnitude;
      if (sqrMagnitude < (double) num)
      {
        prewMarkerNum = index;
        num = sqrMagnitude;
      }
    }
    AddMarker(prewMarkerNum, coords);
  }

  public void AddMarker(Ray camRay)
  {
    float num1 = float.PositiveInfinity;
    int prewMarkerNum = 0;
    int index1 = 0;
    for (int index2 = 0; index2 < markers.Length; ++index2)
    {
      SplineBendMarker marker = markers[index2];
      for (int index3 = 0; index3 < marker.subPoints.Length; ++index3)
      {
        Vector3 vector3 = this.transform.TransformPoint(marker.subPoints[index3]);
        float num2 = Vector3.Dot(camRay.direction, (vector3 - camRay.origin).normalized) * (camRay.origin - vector3).magnitude;
        float magnitude = (camRay.origin + camRay.direction * num2 - vector3).magnitude;
        if (magnitude < (double) num1)
        {
          prewMarkerNum = index2;
          index1 = index3;
          num1 = magnitude;
        }
      }
    }
    Vector3 vector3_1 = this.transform.TransformPoint(markers[prewMarkerNum].subPoints[index1]);
    float magnitude1 = (camRay.origin - vector3_1).magnitude;
    this.AddMarker(prewMarkerNum, camRay.origin + camRay.direction * magnitude1);
    UpdateNow();
    UpdateNow();
  }

  private void AddMarker(int prewMarkerNum, Vector3 coords)
  {
    SplineBendMarker[] splineBendMarkerArray = new SplineBendMarker[markers.Length + 1];
    for (int index = 0; index < markers.Length; ++index)
    {
      if (index <= prewMarkerNum)
        splineBendMarkerArray[index] = markers[index];
      else
        splineBendMarkerArray[index + 1] = markers[index];
    }
    Transform transform = new GameObject("Marker" + (prewMarkerNum + 1)).transform;
    transform.parent = this.transform;
    transform.position = coords;
    splineBendMarkerArray[prewMarkerNum + 1] = transform.gameObject.AddComponent<SplineBendMarker>();
    markers = splineBendMarkerArray;
  }

  private void RefreshMarkers()
  {
    int length = 0;
    for (int index = 0; index < markers.Length; ++index)
    {
      if ((bool) (UnityEngine.Object) markers[index])
        ++length;
    }
    SplineBendMarker[] splineBendMarkerArray = new SplineBendMarker[length];
    int index1 = 0;
    for (int index2 = 0; index2 < markers.Length; ++index2)
    {
      if ((bool) (UnityEngine.Object) markers[index2])
      {
        splineBendMarkerArray[index1] = markers[index2];
        ++index1;
      }
    }
    markers = splineBendMarkerArray;
  }

  private void RemoveMarker(int num)
  {
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) markers[num].gameObject);
    SplineBendMarker[] splineBendMarkerArray = new SplineBendMarker[markers.Length - 1];
    for (int index = 0; index < markers.Length - 1; ++index)
      splineBendMarkerArray[index] = index >= num ? markers[index + 1] : markers[index];
    markers = splineBendMarkerArray;
  }

  private void CloseMarkers()
  {
    if (closed || (UnityEngine.Object) markers[0] == (UnityEngine.Object) markers[markers.Length - 1])
      return;
    SplineBendMarker[] splineBendMarkerArray = new SplineBendMarker[markers.Length + 1];
    for (int index = 0; index < markers.Length; ++index)
      splineBendMarkerArray[index] = markers[index];
    markers = splineBendMarkerArray;
    markers[markers.Length - 1] = markers[0];
    UpdateNow();
    closed = true;
  }

  private void UnCloseMarkers()
  {
    if (!closed || (UnityEngine.Object) markers[0] != (UnityEngine.Object) markers[markers.Length - 1])
      return;
    SplineBendMarker[] splineBendMarkerArray = new SplineBendMarker[markers.Length - 1];
    for (int index = 0; index < markers.Length - 1; ++index)
      splineBendMarkerArray[index] = markers[index];
    markers = splineBendMarkerArray;
    UpdateNow();
    closed = false;
  }

  private void OnEnable()
  {
    if (Environment.CommandLine.Contains("-batchmode"))
      return;
    renderMesh = (Mesh) null;
    collisionMesh = (Mesh) null;
    ForceUpdate();
    MeshFilter component1 = this.GetComponent<MeshFilter>();
    MeshCollider component2 = this.GetComponent<MeshCollider>();
    if ((bool) (UnityEngine.Object) renderMesh && (bool) (UnityEngine.Object) component1)
      component1.sharedMesh = renderMesh;
    if (!(bool) (UnityEngine.Object) collisionMesh || !(bool) (UnityEngine.Object) component2)
      return;
    component2.sharedMesh = (Mesh) null;
    component2.sharedMesh = collisionMesh;
  }

  private void OnDisable()
  {
    if (Environment.CommandLine.Contains("-batchmode"))
      return;
    MeshFilter component1 = this.GetComponent<MeshFilter>();
    MeshCollider component2 = this.GetComponent<MeshCollider>();
    if ((bool) (UnityEngine.Object) initialRenderMesh && (bool) (UnityEngine.Object) component1)
      component1.sharedMesh = initialRenderMesh;
    if (!(bool) (UnityEngine.Object) initialCollisionMesh || !(bool) (UnityEngine.Object) component2)
      return;
    component2.sharedMesh = (Mesh) null;
    component2.sharedMesh = initialCollisionMesh;
  }

  public void UpdateNow() => ForceUpdate(true);

  public void ForceUpdate() => ForceUpdate(true);

  public void ForceUpdate(bool refreshCollisionMesh)
  {
    MeshCollider component1 = this.GetComponent<MeshCollider>();
    MeshFilter component2 = this.GetComponent<MeshFilter>();
    switch (axis)
    {
      case SplineBendAxis.x:
        axisVector = new Vector3(1f, 0.0f, 0.0f);
        break;
      case SplineBendAxis.y:
        axisVector = new Vector3(0.0f, 1f, 0.0f);
        break;
      case SplineBendAxis.z:
        axisVector = new Vector3(0.0f, 0.0f, 1f);
        break;
    }
    if ((bool) (UnityEngine.Object) initialRenderMesh)
      tiles = Mathf.Min(tiles, Mathf.FloorToInt(65000f / (float) initialRenderMesh.vertices.Length));
    else if ((bool) (UnityEngine.Object) initialCollisionMesh)
      tiles = Mathf.Min(tiles, Mathf.FloorToInt(65000f / (float) initialCollisionMesh.vertices.Length));
    tiles = Mathf.Max(tiles, 1);
    if (markers == null)
      ResetMarkers(2);
    for (int index = 0; index < markers.Length; ++index)
    {
      if (!(bool) (UnityEngine.Object) markers[index])
        RefreshMarkers();
    }
    if (markers.Length < 2)
      ResetMarkers(2);
    for (int mnum = 0; mnum < markers.Length; ++mnum)
      markers[mnum].Init(this, mnum);
    if (closed)
      markers[0].dist = markers[markers.Length - 2].dist + GetBeizerLength(markers[markers.Length - 2], markers[0]);
    float dist = markers[markers.Length - 1].dist;
    if (closed)
      dist = markers[0].dist;
    for (int index = 0; index < markers.Length; ++index)
      markers[index].percent = markers[index].dist / dist;
    if (closed && !wasClosed)
      CloseMarkers();
    if (!closed && wasClosed)
      UnCloseMarkers();
    wasClosed = closed;
    if ((bool) (UnityEngine.Object) component2 && !(bool) (UnityEngine.Object) renderMesh)
    {
      if (!(bool) (UnityEngine.Object) initialRenderMesh)
        initialRenderMesh = component2.sharedMesh;
      if ((bool) (UnityEngine.Object) initialRenderMesh)
      {
        if (tileOffset < 0.0)
          tileOffset = initialRenderMesh.bounds.size.z;
        renderMesh = UnityEngine.Object.Instantiate<Mesh>(initialRenderMesh);
        renderMesh.hideFlags = HideFlags.HideAndDontSave;
        component2.sharedMesh = renderMesh;
      }
    }
    if ((bool) (UnityEngine.Object) component1 && !(bool) (UnityEngine.Object) collisionMesh)
    {
      if (!(bool) (UnityEngine.Object) initialCollisionMesh)
        initialCollisionMesh = component1.sharedMesh;
      if ((bool) (UnityEngine.Object) initialCollisionMesh)
      {
        if (tileOffset < 0.0)
          tileOffset = initialCollisionMesh.bounds.size.z;
        collisionMesh = UnityEngine.Object.Instantiate<Mesh>(initialCollisionMesh);
        collisionMesh.hideFlags = HideFlags.HideAndDontSave;
        component1.sharedMesh = collisionMesh;
      }
    }
    if ((bool) (UnityEngine.Object) renderMesh && (bool) (UnityEngine.Object) initialRenderMesh && (bool) (UnityEngine.Object) component2)
    {
      if (renderMesh.vertexCount != initialRenderMesh.vertexCount * tiles)
        BuildMesh(renderMesh, initialRenderMesh, tiles, 0.0f);
      Align(renderMesh, initialRenderMesh);
      if (dropToTerrain)
        FallToTerrain(renderMesh, initialRenderMesh, terrainSeekDist, terrainLayer, terrainOffset);
      renderMesh.RecalculateBounds();
      renderMesh.RecalculateNormals();
    }
    if (!(bool) (UnityEngine.Object) collisionMesh || !(bool) (UnityEngine.Object) initialCollisionMesh || !(bool) (UnityEngine.Object) component1)
      return;
    if (collisionMesh.vertexCount != initialCollisionMesh.vertexCount * tiles)
      BuildMesh(collisionMesh, initialCollisionMesh, tiles, 0.0f);
    Align(collisionMesh, initialCollisionMesh);
    if (dropToTerrain)
      FallToTerrain(collisionMesh, initialCollisionMesh, terrainSeekDist, terrainLayer, terrainOffset);
    if (refreshCollisionMesh && (UnityEngine.Object) component1.sharedMesh == (UnityEngine.Object) collisionMesh)
    {
      collisionMesh.RecalculateBounds();
      collisionMesh.RecalculateNormals();
      component1.sharedMesh = (Mesh) null;
      component1.sharedMesh = collisionMesh;
    }
  }

  public enum SplineBendAxis
  {
    x,
    y,
    z,
  }
}
