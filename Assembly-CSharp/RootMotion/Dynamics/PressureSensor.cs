using UnityEngine;

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
      this.r = this.GetComponent<Rigidbody>();
      this.center = this.transform.position;
    }

    private void OnCollisionEnter(Collision c) => this.ProcessCollision(c);

    private void OnCollisionStay(Collision c) => this.ProcessCollision(c);

    private void OnCollisionExit(Collision c) => this.inContact = false;

    private void FixedUpdate()
    {
      this.fixedFrame = true;
      if (this.r.IsSleeping())
        return;
      this.P = Vector3.zero;
      this.count = 0;
    }

    private void LateUpdate()
    {
      if (!this.fixedFrame)
        return;
      if (this.count > 0)
        this.center = this.P / (float) this.count;
      this.fixedFrame = false;
    }

    private void ProcessCollision(Collision c)
    {
      if (!LayerMaskExtensions.Contains(this.layers, c.gameObject.layer))
        return;
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < c.contacts.Length; ++index)
        zero += c.contacts[index].point;
      this.P += zero / (float) c.contacts.Length;
      ++this.count;
      this.inContact = true;
    }

    private void OnDrawGizmos()
    {
      if (!this.visualize)
        return;
      Gizmos.DrawSphere(this.center, 0.1f);
    }
  }
}
