using UnityEngine;
using UnityEngine.AI;

namespace Pathologic.Prototype
{
  public class TragedianSquare : MonoBehaviour
  {
    private Vector3 _lastPosition;
    public Transform CapsuleTransform;
    public bool TragedianA;

    private void Start()
    {
      this._lastPosition = Vector3.zero;
      if (this.TragedianA)
        this.GetComponent<Animator>().SetBool("TragedianA", true);
      else
        this.GetComponent<Animator>().SetBool("TragedianB", true);
    }

    private void Update()
    {
      this.GetComponent<CapsuleCollider>().center = this.gameObject.transform.InverseTransformPoint(this.CapsuleTransform.position);
      if ((double) (this._lastPosition - this.CapsuleTransform.position).magnitude <= 0.30000001192092896)
        return;
      this.GetComponent<NavMeshObstacle>().center = this.GetComponent<CapsuleCollider>().center;
      this._lastPosition = this.CapsuleTransform.position;
    }
  }
}
