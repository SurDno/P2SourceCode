using UnityEngine;

public class Soldier_BreakDoor_SetIsKinematic : MonoBehaviour
{
  private bool isKinematic;
  private Rigidbody rigidbody;

  private void Start() => this.rigidbody = this.GetComponent<Rigidbody>();

  private void Update()
  {
    if (this.rigidbody.isKinematic)
      return;
    this.rigidbody.isKinematic = true;
  }
}
