// Decompiled with JetBrains decompiler
// Type: TerrainHole
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
[ExecuteInEditMode]
public class TerrainHole : MonoBehaviour
{
  private static TerrainHole lastHole = (TerrainHole) null;
  private static float lastSqrDistance = float.MaxValue;
  private static int terrainCutPropertyId = 0;
  private static int terrainCutMatrixPropertyId = 0;

  private static int TerrainCutPropertyId
  {
    get
    {
      if (TerrainHole.terrainCutPropertyId == 0)
        TerrainHole.terrainCutPropertyId = Shader.PropertyToID("_TerrainCut");
      return TerrainHole.terrainCutPropertyId;
    }
  }

  private static int TerrainCutMatrixPropertyId
  {
    get
    {
      if (TerrainHole.terrainCutMatrixPropertyId == 0)
        TerrainHole.terrainCutMatrixPropertyId = Shader.PropertyToID("_TerrainCutMatrix");
      return TerrainHole.terrainCutMatrixPropertyId;
    }
  }

  private void OnPreCullEvent(Camera camera)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(nameof (TerrainHole));
    this.OnPreCullEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void OnPreCullEvent2(Camera camera)
  {
    float sqrMagnitude = (this.transform.position - camera.transform.position).sqrMagnitude;
    if ((Object) TerrainHole.lastHole == (Object) this)
    {
      TerrainHole.lastSqrDistance = sqrMagnitude;
    }
    else
    {
      if (!((Object) TerrainHole.lastHole == (Object) null) && (double) sqrMagnitude >= (double) TerrainHole.lastSqrDistance)
        return;
      Shader.SetGlobalInt(TerrainHole.TerrainCutPropertyId, 1);
      Shader.SetGlobalMatrix(TerrainHole.TerrainCutMatrixPropertyId, this.transform.worldToLocalMatrix);
      TerrainHole.lastHole = this;
      TerrainHole.lastSqrDistance = sqrMagnitude;
    }
  }

  private void OnEnable() => Camera.onPreCull += new Camera.CameraCallback(this.OnPreCullEvent);

  private void OnDisable()
  {
    Camera.onPreCull -= new Camera.CameraCallback(this.OnPreCullEvent);
    if (!((Object) TerrainHole.lastHole == (Object) this))
      return;
    TerrainHole.ResetCurrent();
    Shader.SetGlobalInt(TerrainHole.TerrainCutPropertyId, 0);
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
    TerrainHole.lastHole = (TerrainHole) null;
    TerrainHole.lastSqrDistance = float.MaxValue;
  }
}
