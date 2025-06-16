using UnityEngine;

public class PlagueWebPointSource : MonoBehaviour
{
  public float Strength = 1f;
  public Vector3 Directionality;
  private IPlagueWebPoint point;

  private void OnEnable()
  {
    if ((double) this.Strength <= 0.0)
      return;
    PlagueWeb instance = PlagueWeb.Instance;
    if ((Object) instance != (Object) null)
      this.point = instance.AddPoint(this.transform.position, this.transform.TransformVector(this.Directionality), this.Strength);
  }

  private void OnDisable() => this.RemovePoint();

  private void Update2()
  {
    if ((double) this.Strength > 0.0)
    {
      if (this.point != null)
      {
        this.point.Position = this.transform.position;
        this.point.Directionality = this.transform.TransformVector(this.Directionality);
        this.point.Strength = this.Strength;
      }
      else
        this.AddPoint();
    }
    else
      this.RemovePoint();
  }

  private void AddPoint()
  {
    PlagueWeb instance = PlagueWeb.Instance;
    if (!((Object) instance != (Object) null))
      return;
    this.point = instance.AddPoint(this.transform.position, this.transform.TransformVector(this.Directionality), this.Strength);
  }

  private void RemovePoint()
  {
    if (this.point == null)
      return;
    PlagueWeb instance = PlagueWeb.Instance;
    if (!((Object) instance != (Object) null))
      return;
    instance.RemovePoint(this.point);
    this.point = (IPlagueWebPoint) null;
  }

  private void OnDrawGizmosSelected()
  {
    Vector3 position = this.transform.position;
    Vector3 vector3 = this.transform.TransformVector(this.Directionality);
    Gizmos.DrawLine(position, position + vector3);
  }
}
