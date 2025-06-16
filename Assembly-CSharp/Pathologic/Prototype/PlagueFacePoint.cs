using UnityEngine;

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
      Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
      Gizmos.color = roamable ? Color.green : Color.yellow;
      Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.1f)), 0.1f);
      if (neighbors == null || neighbors.Length == 0)
        return;
      Vector3 from = localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.1f));
      Vector3 vector3_1 = from * 0.5f;
      for (int index = 0; index < neighbors.Length; ++index)
      {
        if (!(neighbors[index] == null))
        {
          Vector3 vector3_2 = neighbors[index].transform.localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.1f));
          Gizmos.DrawLine(from, vector3_1 + vector3_2 * 0.5f);
        }
      }
    }

    private void OnDrawGizmosSelected()
    {
      Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
      Gizmos.color = roamable ? Color.green : Color.yellow;
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
