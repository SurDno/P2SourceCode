// Decompiled with JetBrains decompiler
// Type: FlockScare
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
    this.IterateLandingSpots();
    if (this.currentController._activeLandingSpots > 0 && this.CheckDistanceToLandingSpot(this.landingSpotControllers[this.lsc]))
      this.landingSpotControllers[this.lsc].ScareAll();
    this.Invoke(nameof (CheckProximityToLandingSpots), this.scareInterval);
  }

  private void IterateLandingSpots()
  {
    this.ls += this.checkEveryNthLandingSpot;
    this.currentController = this.landingSpotControllers[this.lsc];
    int childCount = this.currentController.transform.childCount;
    if (this.ls <= childCount - 1)
      return;
    this.ls -= childCount;
    if (this.lsc < this.landingSpotControllers.Length - 1)
      ++this.lsc;
    else
      this.lsc = 0;
  }

  private bool CheckDistanceToLandingSpot(LandingSpotController lc)
  {
    Transform child = lc.transform.GetChild(this.ls);
    return (Object) child.GetComponent<LandingSpot>().landingChild != (Object) null && (double) (child.position - this.transform.position).sqrMagnitude < (double) this.distanceToScare * (double) this.distanceToScare;
  }

  private void Invoker()
  {
    for (int index = 0; index < this.InvokeAmounts; ++index)
      this.Invoke("CheckProximityToLandingSpots", this.scareInterval + this.scareInterval / (float) this.InvokeAmounts * (float) index);
  }

  private void OnEnable()
  {
    this.CancelInvoke("CheckProximityToLandingSpots");
    if (this.landingSpotControllers.Length == 0)
      return;
    this.Invoker();
  }

  private void OnDisable() => this.CancelInvoke("CheckProximityToLandingSpots");
}
