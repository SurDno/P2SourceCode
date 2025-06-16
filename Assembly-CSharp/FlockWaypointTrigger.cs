using UnityEngine;

public class FlockWaypointTrigger : MonoBehaviour
{
  public float _timer = 1f;
  public FlockChild _flockChild;

  public void Start()
  {
    if ((Object) this._flockChild == (Object) null)
      this._flockChild = this.transform.parent.GetComponent<FlockChild>();
    float num = Random.Range(this._timer, this._timer * 3f);
    this.InvokeRepeating("Trigger", num, num);
  }

  public void Trigger() => this._flockChild.Wander(0.0f);
}
