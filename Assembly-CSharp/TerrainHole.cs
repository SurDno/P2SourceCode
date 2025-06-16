[ExecuteInEditMode]
public class TerrainHole : MonoBehaviour
{
  private static TerrainHole lastHole = null;
  private static float lastSqrDistance = float.MaxValue;
  private static int terrainCutPropertyId;
  private static int terrainCutMatrixPropertyId;

  private static int TerrainCutPropertyId
  {
    get
    {
      if (terrainCutPropertyId == 0)
        terrainCutPropertyId = Shader.PropertyToID("_TerrainCut");
      return terrainCutPropertyId;
    }
  }

  private static int TerrainCutMatrixPropertyId
  {
    get
    {
      if (terrainCutMatrixPropertyId == 0)
        terrainCutMatrixPropertyId = Shader.PropertyToID("_TerrainCutMatrix");
      return terrainCutMatrixPropertyId;
    }
  }

  private void OnPreCullEvent(Camera camera)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(nameof (TerrainHole));
    OnPreCullEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void OnPreCullEvent2(Camera camera)
  {
    float sqrMagnitude = (this.transform.position - camera.transform.position).sqrMagnitude;
    if ((Object) lastHole == (Object) this)
    {
      lastSqrDistance = sqrMagnitude;
    }
    else
    {
      if (!((Object) lastHole == (Object) null) && sqrMagnitude >= (double) lastSqrDistance)
        return;
      Shader.SetGlobalInt(TerrainCutPropertyId, 1);
      Shader.SetGlobalMatrix(TerrainCutMatrixPropertyId, this.transform.worldToLocalMatrix);
      lastHole = this;
      lastSqrDistance = sqrMagnitude;
    }
  }

  private void OnEnable() => Camera.onPreCull += new Camera.CameraCallback(OnPreCullEvent);

  private void OnDisable()
  {
    Camera.onPreCull -= new Camera.CameraCallback(OnPreCullEvent);
    if (!((Object) lastHole == (Object) this))
      return;
    ResetCurrent();
    Shader.SetGlobalInt(TerrainCutPropertyId, 0);
  }

  private void OnDrawGizmosSelected()
  {
    Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
    Vector3 from = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-1f, -1f, -1f));
    Vector3 vector3_1 = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-1f, -1f, 1f));
    Vector3 vector3_2 = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-1f, 1f, -1f));
    Vector3 vector3_3 = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-1f, 1f, 1f));
    Vector3 vector3_4 = localToWorldMatrix.MultiplyPoint3x4(new Vector3(1f, -1f, -1f));
    Vector3 vector3_5 = localToWorldMatrix.MultiplyPoint3x4(new Vector3(1f, -1f, 1f));
    Vector3 vector3_6 = localToWorldMatrix.MultiplyPoint3x4(new Vector3(1f, 1f, -1f));
    Vector3 to = localToWorldMatrix.MultiplyPoint3x4(new Vector3(1f, 1f, 1f));
    Gizmos.DrawLine(from, vector3_1);
    Gizmos.DrawLine(vector3_2, vector3_3);
    Gizmos.DrawLine(from, vector3_2);
    Gizmos.DrawLine(vector3_1, vector3_3);
    Gizmos.DrawLine(vector3_4, vector3_5);
    Gizmos.DrawLine(vector3_6, to);
    Gizmos.DrawLine(vector3_4, vector3_6);
    Gizmos.DrawLine(vector3_5, to);
    Gizmos.DrawLine(from, vector3_4);
    Gizmos.DrawLine(vector3_1, vector3_5);
    Gizmos.DrawLine(vector3_2, vector3_6);
    Gizmos.DrawLine(vector3_3, to);
  }

  private static void ResetCurrent()
  {
    lastHole = null;
    lastSqrDistance = float.MaxValue;
  }
}
