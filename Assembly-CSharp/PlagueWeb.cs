using UnityEngine;

public abstract class PlagueWeb : MonoBehaviour
{
  public static PlagueWeb Instance { get; private set; }

  private void Awake() => Instance = this;

  public abstract Vector3 CameraPosition { get; set; }

  public abstract bool IsActive { get; set; }

  public abstract IPlagueWebPoint AddPoint(
    Vector3 position,
    Vector3 directionality,
    float strength);

  public abstract void RemovePoint(IPlagueWebPoint point);
}
