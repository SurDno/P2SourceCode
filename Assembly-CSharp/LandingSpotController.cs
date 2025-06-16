using System.Collections;
using UnityEngine;

public class LandingSpotController : MonoBehaviour
{
  public bool _randomRotate = true;
  public Vector2 _autoCatchDelay = new Vector2(10f, 20f);
  public Vector2 _autoDismountDelay = new Vector2(10f, 20f);
  public float _maxBirdDistance = 20f;
  public float _minBirdDistance = 5f;
  public bool _takeClosest;
  public FlockController _flock;
  public bool _landOnStart;
  public bool _soarLand = true;
  public bool _onlyBirdsAbove;
  public float _landingSpeedModifier = 0.5f;
  public float _landingTurnSpeedModifier = 5f;
  public Transform _featherPS;
  private Transform _thisT;
  public int _activeLandingSpots;
  public float _snapLandDistance = 0.1f;
  public float _landedRotateSpeed = 0.01f;
  public float _gizmoSize = 0.2f;

  public void Start()
  {
    if ((Object) this._thisT == (Object) null)
      this._thisT = this.transform;
    if ((Object) this._flock == (Object) null)
    {
      this._flock = (FlockController) Object.FindObjectOfType(typeof (FlockController));
      Debug.Log((object) (this.ToString() + " has no assigned FlockController, a random FlockController has been assigned"));
    }
    if (!this._landOnStart)
      return;
    this.StartCoroutine(this.InstantLandOnStart(0.1f));
  }

  public void ScareAll() => this.ScareAll(0.0f, 1f);

  public void ScareAll(float minDelay, float maxDelay)
  {
    for (int index = 0; index < this._thisT.childCount; ++index)
    {
      if ((Object) this._thisT.GetChild(index).GetComponent<LandingSpot>() != (Object) null)
        this._thisT.GetChild(index).GetComponent<LandingSpot>().Invoke("ReleaseFlockChild", Random.Range(minDelay, maxDelay));
    }
  }

  public void LandAll()
  {
    if ((Object) this._thisT == (Object) null)
      return;
    for (int index = 0; index < this._thisT.childCount; ++index)
    {
      if ((Object) this._thisT.GetChild(index).GetComponent<LandingSpot>() != (Object) null)
        this.StartCoroutine(this._thisT.GetChild(index).GetComponent<LandingSpot>().GetFlockChild(0.0f, 2f));
    }
  }

  public IEnumerator InstantLandOnStart(float delay)
  {
    yield return (object) new WaitForSeconds(delay);
    for (int i = 0; i < this._thisT.childCount; ++i)
    {
      if ((Object) this._thisT.GetChild(i).GetComponent<LandingSpot>() != (Object) null)
      {
        LandingSpot spot = this._thisT.GetChild(i).GetComponent<LandingSpot>();
        spot.InstantLand();
        spot = (LandingSpot) null;
      }
    }
  }

  public IEnumerator InstantLand(float delay)
  {
    yield return (object) new WaitForSeconds(delay);
    for (int i = 0; i < this._thisT.childCount; ++i)
    {
      if ((Object) this._thisT.GetChild(i).GetComponent<LandingSpot>() != (Object) null)
      {
        LandingSpot spot = this._thisT.GetChild(i).GetComponent<LandingSpot>();
        spot.InstantLand();
        spot = (LandingSpot) null;
      }
    }
  }
}
