using UnityEngine;

public class FlockScare : MonoBehaviour
{
  public LandingSpotController[] landingSpotControllers;
  public float scareInterval = 0.1f;
  public float distanceToScare = 2f;
  public int checkEveryNthLandingSpot = 1;
  public int InvokeAmounts = 1;
  private int lsc;
  private int ls;
  private LandingSpotController currentController;

  private void CheckProximityToLandingSpots()
  {
    IterateLandingSpots();
    if (currentController._activeLandingSpots > 0 && CheckDistanceToLandingSpot(landingSpotControllers[lsc]))
      landingSpotControllers[lsc].ScareAll();
    Invoke(nameof (CheckProximityToLandingSpots), scareInterval);
  }

  private void IterateLandingSpots()
  {
    ls += checkEveryNthLandingSpot;
    currentController = landingSpotControllers[lsc];
    int childCount = currentController.transform.childCount;
    if (ls <= childCount - 1)
      return;
    ls -= childCount;
    if (lsc < landingSpotControllers.Length - 1)
      ++lsc;
    else
      lsc = 0;
  }

  private bool CheckDistanceToLandingSpot(LandingSpotController lc)
  {
    Transform child = lc.transform.GetChild(ls);
    return child.GetComponent<LandingSpot>().landingChild != null && (child.position - transform.position).sqrMagnitude < distanceToScare * (double) distanceToScare;
  }

  private void Invoker()
  {
    for (int index = 0; index < InvokeAmounts; ++index)
      Invoke("CheckProximityToLandingSpots", scareInterval + scareInterval / InvokeAmounts * index);
  }

  private void OnEnable()
  {
    CancelInvoke("CheckProximityToLandingSpots");
    if (landingSpotControllers.Length == 0)
      return;
    Invoker();
  }

  private void OnDisable() => CancelInvoke("CheckProximityToLandingSpots");
}
