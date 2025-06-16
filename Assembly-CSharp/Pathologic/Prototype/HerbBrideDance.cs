namespace Pathologic.Prototype
{
  [RequireComponent(typeof (CapsuleCollider))]
  [RequireComponent(typeof (NavMeshObstacle))]
  public class HerbBrideDance : MonoBehaviour
  {
    private Vector3 _lastPosition;
    public Transform CapsuleTransform;

    private void Start() => _lastPosition = Vector3.zero;

    private void Update()
    {
      this.GetComponent<CapsuleCollider>().center = this.gameObject.transform.InverseTransformPoint(CapsuleTransform.position);
      if ((double) (_lastPosition - CapsuleTransform.position).magnitude <= 0.30000001192092896)
        return;
      this.GetComponent<NavMeshObstacle>().center = this.GetComponent<CapsuleCollider>().center;
      _lastPosition = CapsuleTransform.position;
    }
  }
}
