[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (SphereCollider))]
[AddComponentMenu("")]
public class AmplifyColorTriggerProxy : AmplifyColorTriggerProxyBase
{
  private SphereCollider sphereCollider;
  private Rigidbody rigidBody;

  private void Start()
  {
    sphereCollider = this.GetComponent<SphereCollider>();
    sphereCollider.radius = 0.01f;
    sphereCollider.isTrigger = true;
    rigidBody = this.GetComponent<Rigidbody>();
    rigidBody.useGravity = false;
    rigidBody.isKinematic = true;
  }

  private void LateUpdate()
  {
    this.transform.position = Reference.position;
    this.transform.rotation = Reference.rotation;
  }
}
