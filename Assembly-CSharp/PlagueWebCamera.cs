using UnityEngine;

public class PlagueWebCamera : MonoBehaviour
{
  private void Update() => PlagueWeb.Instance.CameraPosition = transform.position;
}
