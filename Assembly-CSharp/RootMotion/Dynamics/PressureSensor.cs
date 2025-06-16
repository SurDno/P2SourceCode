namespace RootMotion.Dynamics
{
  public class PressureSensor : MonoBehaviour
  {
    public bool visualize;
    public LayerMask layers;
    private bool fixedFrame;
    private Vector3 P;
    private int count;

    public Vector3 center { get; private set; }

    public bool inContact { get; private set; }

    public Vector3 bottom { get; private set; }

    public Rigidbody r { get; private set; }

    private void Awake()
    {
      r = this.GetComponent<Rigidbody>();
      center = this.transform.position;
    }

    private void OnCollisionEnter(Collision c) => ProcessCollision(c);

    private void OnCollisionStay(Collision c) => ProcessCollision(c);

    private void OnCollisionExit(Collision c) => inContact = false;

    private void FixedUpdate()
    {
      fixedFrame = true;
      if (r.IsSleeping())
        return;
      P = Vector3.zero;
      count = 0;
    }

    private void LateUpdate()
    {
      if (!fixedFrame)
        return;
      if (count > 0)
        center = P / (float) count;
      fixedFrame = false;
    }

    private void ProcessCollision(Collision c)
    {
      if (!LayerMaskExtensions.Contains(layers, c.gameObject.layer))
        return;
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < c.contacts.Length; ++index)
        zero += c.contacts[index].point;
      P += zero / (float) c.contacts.Length;
      ++count;
      inContact = true;
    }

    private void OnDrawGizmos()
    {
      if (!visualize)
        return;
      Gizmos.DrawSphere(center, 0.1f);
    }
  }
}
