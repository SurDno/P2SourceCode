public class UnluckDistanceDisabler : MonoBehaviour
{
  public int _distanceDisable = 1000;
  public Transform _distanceFrom;
  public bool _distanceFromMainCam;
  public float _disableCheckInterval = 10f;
  public float _enableCheckInterval = 1f;
  public bool _disableOnStart;

  public void Start()
  {
    if (_distanceFromMainCam)
      _distanceFrom = Camera.main.transform;
    this.InvokeRepeating("CheckDisable", _disableCheckInterval + Random.value * _disableCheckInterval, _disableCheckInterval);
    this.InvokeRepeating("CheckEnable", _enableCheckInterval + Random.value * _enableCheckInterval, _enableCheckInterval);
    this.Invoke("DisableOnStart", 0.01f);
  }

  public void DisableOnStart()
  {
    if (!_disableOnStart)
      return;
    this.gameObject.SetActive(false);
  }

  public void CheckDisable()
  {
    if (!this.gameObject.activeInHierarchy || (double) (this.transform.position - _distanceFrom.position).sqrMagnitude <= _distanceDisable * _distanceDisable)
      return;
    this.gameObject.SetActive(false);
  }

  public void CheckEnable()
  {
    if (this.gameObject.activeInHierarchy || (double) (this.transform.position - _distanceFrom.position).sqrMagnitude >= _distanceDisable * _distanceDisable)
      return;
    this.gameObject.SetActive(true);
  }
}
