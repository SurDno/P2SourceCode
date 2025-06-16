using UnityEngine;

public class ProtagonistLight : MonoBehaviour
{
  private void LateUpdate()
  {
    ProtagonistShadersSettings instance = MonoBehaviourInstance<ProtagonistShadersSettings>.Instance;
    if (instance == null)
      return;
    Vector3 position = transform.parent.position;
    transform.position = instance.ProtagonistToWorld(position);
  }
}
