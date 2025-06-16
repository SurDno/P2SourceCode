using Engine.Common.Services;
using Engine.Source.Services;

public class LandingSpotObject : MonoBehaviour
{
  [EnumFlag(typeof (FlockChildTypeEnum))]
  [SerializeField]
  private FlockChildTypeEnum supportedFlocks;
  private LandingSpotController landingController;
  private bool preseted = true;
  private FlockObject flock;

  public FlockChildTypeEnum SupportedFlocks => supportedFlocks;

  public LandingSpotController LandingController
  {
    get
    {
      if ((Object) this != (Object) null && (Object) landingController == (Object) null)
        landingController = this.GetComponent<LandingSpotController>();
      return landingController;
    }
  }

  public bool Preseted => preseted;

  public FlockObject Flock
  {
    get => flock;
    set
    {
      flock = value;
      if ((Object) LandingController == (Object) null || !((Object) flock != (Object) null))
        return;
      LandingController._flock = flock.FlockController;
      LandingController.LandAll();
    }
  }

  private void Start()
  {
    if ((Object) LandingController != (Object) null)
    {
      preseted = (Object) LandingController._flock != (Object) null;
      LandingController._onlyBirdsAbove = true;
      LandingController._maxBirdDistance = 30f;
    }
    ServiceLocator.GetService<FlockService>()?.RegisterLandingZone(this);
  }

  private void OnDestroy()
  {
    ServiceLocator.GetService<FlockService>()?.UnregisterLandingZone(this);
  }

  public void Scare()
  {
    if ((Object) LandingController == (Object) null)
      return;
    LandingController.ScareAll();
  }
}
