using Engine.Common.Services;
using Engine.Source.Services;
using System.Collections.Generic;
using UnityEngine;

public class FlockObject : MonoBehaviour
{
  [SerializeField]
  private GameObject audioSource;
  [SerializeField]
  private FlockChildTypeEnum flockType;
  [SerializeField]
  private FlockController flockController;
  private HashSet<LandingSpotObject> landingZones = new HashSet<LandingSpotObject>();
  private float positionRecountTimeout = 1f;
  private float timeFromLastPositionRecount = 0.0f;
  private Vector3 audioPosition = Vector3.zero;
  private bool audioPositionSet = false;
  private float audioMoveSpeed = 20f;

  public FlockChildTypeEnum FlockType => this.flockType;

  public FlockController FlockController => this.flockController;

  private void Awake()
  {
    if ((Object) this.audioSource != (Object) null)
      this.audioSource.SetActive(false);
    this.audioPositionSet = false;
  }

  private void Start()
  {
    ServiceLocator.GetService<FlockService>()?.RegisterFlock(this);
    if ((Object) this.flockController != (Object) null)
      this.audioMoveSpeed = this.flockController._minSpeed;
    this.SetAudioSourcePosition();
    if (!((Object) this.audioSource != (Object) null))
      return;
    this.audioSource.transform.position = this.audioPosition;
  }

  private void Update()
  {
    this.timeFromLastPositionRecount -= Time.deltaTime;
    if ((double) this.timeFromLastPositionRecount <= 0.0)
      this.SetAudioSourcePosition();
    if (!this.audioPositionSet || !((Object) this.audioSource != (Object) null))
      return;
    this.audioSource.transform.position = Vector3.MoveTowards(this.audioSource.transform.position, this.audioPosition, this.audioMoveSpeed * Time.deltaTime);
  }

  private void OnDestroy()
  {
    foreach (LandingSpotObject landingZone in this.landingZones)
      landingZone.Flock = (FlockObject) null;
    ServiceLocator.GetService<FlockService>()?.UnregisterFlock(this);
  }

  public void AddLandingZone(LandingSpotObject zone)
  {
    this.landingZones.Add(zone);
    zone.Flock = this;
  }

  public void RemoveLandingZone(LandingSpotObject zone)
  {
    if (!this.landingZones.Remove(zone))
      return;
    zone.Flock = (FlockObject) null;
  }

  public int GetZonesCount() => this.landingZones.Count;

  private void SetAudioSourcePosition()
  {
    this.timeFromLastPositionRecount = this.positionRecountTimeout;
    if ((Object) this.audioSource == (Object) null)
      return;
    if ((Object) this.FlockController == (Object) null)
    {
      this.audioSource.SetActive(false);
    }
    else
    {
      if (!this.audioSource.activeSelf)
        this.audioSource.SetActive(true);
      Vector3 zero = Vector3.zero;
      int num = 0;
      foreach (FlockChild child in this.FlockController.childs)
      {
        if ((Object) child.gameObject != (Object) null && !child.landing)
        {
          zero += child.transform.position;
          ++num;
        }
      }
      if (num == 0)
      {
        this.audioPositionSet = false;
        this.audioSource.SetActive(false);
      }
      else
      {
        this.audioPositionSet = true;
        this.audioPosition = zero / (float) num;
      }
    }
  }

  public void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawLine(this.FlockController.transform.position + this.FlockController.targetPosition, this.audioSource.transform.position);
  }
}
