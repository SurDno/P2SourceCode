using UnityEngine;

public class PlagueWebPointSource : MonoBehaviour
{
  public float Strength = 1f;
  public Vector3 Directionality;
  private IPlagueWebPoint point;

  private void OnEnable()
  {
    if (Strength <= 0.0)
      return;
    PlagueWeb instance = PlagueWeb.Instance;
    if (instance != null)
      point = instance.AddPoint(transform.position, transform.TransformVector(Directionality), Strength);
  }

  private void OnDisable() => RemovePoint();

  private void Update2()
  {
    if (Strength > 0.0)
    {
      if (point != null)
      {
        point.Position = transform.position;
        point.Directionality = transform.TransformVector(Directionality);
        point.Strength = Strength;
      }
      else
        AddPoint();
    }
    else
      RemovePoint();
  }

  private void AddPoint()
  {
    PlagueWeb instance = PlagueWeb.Instance;
    if (!(instance != null))
      return;
    point = instance.AddPoint(transform.position, transform.TransformVector(Directionality), Strength);
  }

  private void RemovePoint()
  {
    if (point == null)
      return;
    PlagueWeb instance = PlagueWeb.Instance;
    if (!(instance != null))
      return;
    instance.RemovePoint(point);
    point = null;
  }

  private void OnDrawGizmosSelected()
  {
    Vector3 position = transform.position;
    Vector3 vector3 = transform.TransformVector(Directionality);
    Gizmos.DrawLine(position, position + vector3);
  }
}
