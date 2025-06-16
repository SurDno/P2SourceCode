using UnityEngine;

public class TOD_Billboard : MonoBehaviour
{
  public float Altitude;
  public float Azimuth;
  public float Distance = 1f;
  public float Size = 1f;

  private T GetComponentInParents<T>() where T : Component
  {
    Transform transform = this.transform;
    T component;
    for (component = transform.GetComponent<T>(); component == null && transform.parent != null; component = transform.GetComponent<T>())
      transform = transform.parent;
    return component;
  }
}
