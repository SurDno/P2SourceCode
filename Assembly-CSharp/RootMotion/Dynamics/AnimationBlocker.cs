using UnityEngine;

namespace RootMotion.Dynamics
{
  public class AnimationBlocker : MonoBehaviour
  {
    private void LateUpdate()
    {
      transform.localPosition = Vector3.zero;
      transform.localRotation = Quaternion.identity;
    }
  }
}
