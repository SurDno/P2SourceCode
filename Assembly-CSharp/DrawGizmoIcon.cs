using UnityEngine;

public class DrawGizmoIcon : MonoBehaviour
{
  [SerializeField]
  private string textureName;
  [SerializeField]
  private Vector3 offset;

  private void OnDrawGizmos()
  {
    Gizmos.DrawIcon(transform.position + offset, textureName);
  }
}
