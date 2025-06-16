using UnityEngine;

namespace RootMotion.Dynamics
{
  public class AnimationBlocker : MonoBehaviour
  {
    private void LateUpdate()
    {
      this.transform.localPosition = Vector3.zero;
      this.transform.localRotation = Quaternion.identity;
    }
  }
}
