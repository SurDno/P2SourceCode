using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Services;
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
  private float timeFromLastPositionRecount;
  private Vector3 audioPosition = Vector3.zero;
  private bool audioPositionSet;
  private float audioMoveSpeed = 20f;

  public FlockChildTypeEnum FlockType => flockType;

  public FlockController FlockController => flockController;

  private void Awake()
  {
    if (audioSource != null)
      audioSource.SetActive(false);
    audioPositionSet = false;
  }

  private void Start()
  {
    ServiceLocator.GetService<FlockService>()?.RegisterFlock(this);
    if (flockController != null)
      audioMoveSpeed = flockController._minSpeed;
    SetAudioSourcePosition();
    if (!(audioSource != null))
      return;
    audioSource.transform.position = audioPosition;
  }

  private void Update()
  {
    timeFromLastPositionRecount -= Time.deltaTime;
    if (timeFromLastPositionRecount <= 0.0)
      SetAudioSourcePosition();
    if (!audioPositionSet || !(audioSource != null))
      return;
    audioSource.transform.position = Vector3.MoveTowards(audioSource.transform.position, audioPosition, audioMoveSpeed * Time.deltaTime);
  }

  private void OnDestroy()
  {
    foreach (LandingSpotObject landingZone in landingZones)
      landingZone.Flock = null;
    ServiceLocator.GetService<FlockService>()?.UnregisterFlock(this);
  }

  public void AddLandingZone(LandingSpotObject zone)
  {
    landingZones.Add(zone);
    zone.Flock = this;
  }

  public void RemoveLandingZone(LandingSpotObject zone)
  {
    if (!landingZones.Remove(zone))
      return;
    zone.Flock = null;
  }

  public int GetZonesCount() => landingZones.Count;

  private void SetAudioSourcePosition()
  {
    timeFromLastPositionRecount = positionRecountTimeout;
    if (audioSource == null)
      return;
    if (FlockController == null)
    {
      audioSource.SetActive(false);
    }
    else
    {
      if (!audioSource.activeSelf)
        audioSource.SetActive(true);
      Vector3 zero = Vector3.zero;
      int num = 0;
      foreach (FlockChild child in FlockController.childs)
      {
        if (child.gameObject != null && !child.landing)
        {
          zero += child.transform.position;
          ++num;
        }
      }
      if (num == 0)
      {
        audioPositionSet = false;
        audioSource.SetActive(false);
      }
      else
      {
        audioPositionSet = true;
        audioPosition = zero / num;
      }
    }
  }

  public void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawLine(FlockController.transform.position + FlockController.targetPosition, audioSource.transform.position);
  }
}
