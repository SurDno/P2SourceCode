// Decompiled with JetBrains decompiler
// Type: Pathologic.Prototype.PlagueFacePoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Pathologic.Prototype
{
  [ExecuteInEditMode]
  public class PlagueFacePoint : MonoBehaviour
  {
    [Space]
    public PlagueFacePoint[] neighbors;
    public bool roamable = true;

    private void OnDrawGizmos()
    {
      Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
      Gizmos.color = this.roamable ? Color.green : Color.yellow;
      Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.1f)), 0.1f);
      if (this.neighbors == null || this.neighbors.Length == 0)
        return;
      Vector3 from = localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.1f));
      Vector3 vector3_1 = from * 0.5f;
      for (int index = 0; index < this.neighbors.Length; ++index)
      {
        if (!((Object) this.neighbors[index] == (Object) null))
        {
          Vector3 vector3_2 = this.neighbors[index].transform.localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.1f));
          Gizmos.DrawLine(from, vector3_1 + vector3_2 * 0.5f);
        }
      }
    }

    private void OnDrawGizmosSelected()
    {
      Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
      Gizmos.color = this.roamable ? Color.green : Color.yellow;
      float num = 1f;
      Vector3 vector3_1 = localToWorldMatrix.MultiplyPoint(new Vector3(num, 0.0f, 0.0f));
      Vector3 vector3_2 = localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, num, 0.0f));
      Vector3 vector3_3 = localToWorldMatrix.MultiplyPoint(new Vector3(-num, 0.0f, 0.0f));
      Vector3 vector3_4 = localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, -num, 0.0f));
      Vector3 to = localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.25f));
      Gizmos.DrawLine(vector3_1, vector3_2);
      Gizmos.DrawLine(vector3_1, to);
      Gizmos.DrawLine(vector3_2, vector3_3);
      Gizmos.DrawLine(vector3_2, to);
      Gizmos.DrawLine(vector3_3, vector3_4);
      Gizmos.DrawLine(vector3_3, to);
      Gizmos.DrawLine(vector3_4, vector3_1);
      Gizmos.DrawLine(vector3_4, to);
    }
  }
}
