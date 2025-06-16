public class ThrowingExample : MonoBehaviour
{
  [SerializeField]
  private KeyCode key;
  [SerializeField]
  private GameObject prefab;
  [SerializeField]
  private LayerMask layerMask;

  private void Update()
  {
    if (!Input.GetKeyDown(key))
      return;
    Throw();
  }

  private void Throw()
  {
    Vector3 position = this.transform.position;
    Vector3 forward = this.transform.forward;
    RaycastHit hitInfo;
    if (!Physics.Raycast(position + forward, forward, out hitInfo, 50f, (int) layerMask, QueryTriggerInteraction.Ignore))
      return;
    GameObject gameObject = Object.Instantiate<GameObject>(prefab);
    gameObject.transform.position = hitInfo.point;
    gameObject.transform.rotation = Quaternion.LookRotation(forward);
  }
}
