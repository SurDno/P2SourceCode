public class FlockWaypointTrigger : MonoBehaviour
{
  public float _timer = 1f;
  public FlockChild _flockChild;

  public void Start()
  {
    if ((Object) _flockChild == (Object) null)
      _flockChild = this.transform.parent.GetComponent<FlockChild>();
    float num = Random.Range(_timer, _timer * 3f);
    this.InvokeRepeating("Trigger", num, num);
  }

  public void Trigger() => _flockChild.Wander(0.0f);
}
