using UnityEngine;

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
    if (this._distanceFromMainCam)
      this._distanceFrom = Camera.main.transform;
    this.InvokeRepeating("CheckDisable", this._disableCheckInterval + Random.value * this._disableCheckInterval, this._disableCheckInterval);
    this.InvokeRepeating("CheckEnable", this._enableCheckInterval + Random.value * this._enableCheckInterval, this._enableCheckInterval);
    this.Invoke("DisableOnStart", 0.01f);
  }

  public void DisableOnStart()
  {
    if (!this._disableOnStart)
      return;
    this.gameObject.SetActive(false);
  }

  public void CheckDisable()
  {
    if (!this.gameObject.activeInHierarchy || (double) (this.transform.position - this._distanceFrom.position).sqrMagnitude <= (double) (this._distanceDisable * this._distanceDisable))
      return;
    this.gameObject.SetActive(false);
  }

  public void CheckEnable()
  {
    if (this.gameObject.activeInHierarchy || (double) (this.transform.position - this._distanceFrom.position).sqrMagnitude >= (double) (this._distanceDisable * this._distanceDisable))
      return;
    this.gameObject.SetActive(true);
  }
}
