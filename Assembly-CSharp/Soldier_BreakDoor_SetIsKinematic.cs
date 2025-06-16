public class Soldier_BreakDoor_SetIsKinematic : MonoBehaviour
{
  private bool isKinematic;
  private Rigidbody rigidbody;

  private void Start() => rigidbody = this.GetComponent<Rigidbody>();

  private void Update()
  {
    if (rigidbody.isKinematic)
      return;
    rigidbody.isKinematic = true;
  }
}
