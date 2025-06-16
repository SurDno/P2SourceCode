using UnityEngine;

public class LodSimple : MonoBehaviour
{
  [SerializeField]
  private float disableDistance = 30f;
  [SerializeField]
  private bool disableRenderers = true;
  [SerializeField]
  private bool disableAnimator = true;

  private void OnEnable()
  {
  }

  private void OnDisable()
  {
  }
}
