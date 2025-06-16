using UnityEngine;

public class ProtagonistLight : MonoBehaviour
{
  private void LateUpdate()
  {
    ProtagonistShadersSettings instance = MonoBehaviourInstance<ProtagonistShadersSettings>.Instance;
    if ((Object) instance == (Object) null)
      return;
    Vector3 position = this.transform.parent.position;
    this.transform.position = instance.ProtagonistToWorld(position);
  }
}
