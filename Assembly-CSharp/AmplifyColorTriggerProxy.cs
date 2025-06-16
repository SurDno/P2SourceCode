using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (SphereCollider))]
[AddComponentMenu("")]
public class AmplifyColorTriggerProxy : AmplifyColorTriggerProxyBase
{
  private SphereCollider sphereCollider;
  private Rigidbody rigidBody;

  private void Start()
  {
    this.sphereCollider = this.GetComponent<SphereCollider>();
    this.sphereCollider.radius = 0.01f;
    this.sphereCollider.isTrigger = true;
    this.rigidBody = this.GetComponent<Rigidbody>();
    this.rigidBody.useGravity = false;
    this.rigidBody.isKinematic = true;
  }

  private void LateUpdate()
  {
    this.transform.position = this.Reference.position;
    this.transform.rotation = this.Reference.rotation;
  }
}
