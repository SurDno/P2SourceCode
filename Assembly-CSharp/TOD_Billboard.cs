using UnityEngine;

public class TOD_Billboard : MonoBehaviour
{
  public float Altitude = 0.0f;
  public float Azimuth = 0.0f;
  public float Distance = 1f;
  public float Size = 1f;

  private T GetComponentInParents<T>() where T : Component
  {
    Transform transform = this.transform;
    T component;
    for (component = transform.GetComponent<T>(); (Object) component == (Object) null && (Object) transform.parent != (Object) null; component = transform.GetComponent<T>())
      transform = transform.parent;
    return component;
  }
}
