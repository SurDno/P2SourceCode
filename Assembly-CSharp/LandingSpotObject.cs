using Engine.Common.Services;
using Engine.Source.Services;
using UnityEngine;

public class LandingSpotObject : MonoBehaviour
{
  [EnumFlag(typeof (FlockChildTypeEnum))]
  [SerializeField]
  private FlockChildTypeEnum supportedFlocks;
  private LandingSpotController landingController;
  private bool preseted = true;
  private FlockObject flock;

  public FlockChildTypeEnum SupportedFlocks => this.supportedFlocks;

  public LandingSpotController LandingController
  {
    get
    {
      if ((Object) this != (Object) null && (Object) this.landingController == (Object) null)
        this.landingController = this.GetComponent<LandingSpotController>();
      return this.landingController;
    }
  }

  public bool Preseted => this.preseted;

  public FlockObject Flock
  {
    get => this.flock;
    set
    {
      this.flock = value;
      if ((Object) this.LandingController == (Object) null || !((Object) this.flock != (Object) null))
        return;
      this.LandingController._flock = this.flock.FlockController;
      this.LandingController.LandAll();
    }
  }

  private void Start()
  {
    if ((Object) this.LandingController != (Object) null)
    {
      this.preseted = (Object) this.LandingController._flock != (Object) null;
      this.LandingController._onlyBirdsAbove = true;
      this.LandingController._maxBirdDistance = 30f;
    }
    ServiceLocator.GetService<FlockService>()?.RegisterLandingZone(this);
  }

  private void OnDestroy()
  {
    ServiceLocator.GetService<FlockService>()?.UnregisterLandingZone(this);
  }

  public void Scare()
  {
    if ((Object) this.LandingController == (Object) null)
      return;
    this.LandingController.ScareAll();
  }
}
