using Engine.Common.Services;
using Engine.Source.Services;
using UnityEngine;

public class LandingSpotObject : MonoBehaviour {
	[EnumFlag(typeof(FlockChildTypeEnum))] [SerializeField]
	private FlockChildTypeEnum supportedFlocks;

	private LandingSpotController landingController;
	private bool preseted = true;
	private FlockObject flock;

	public FlockChildTypeEnum SupportedFlocks => supportedFlocks;

	public LandingSpotController LandingController {
		get {
			if (this != null && landingController == null)
				landingController = GetComponent<LandingSpotController>();
			return landingController;
		}
	}

	public bool Preseted => preseted;

	public FlockObject Flock {
		get => flock;
		set {
			flock = value;
			if (LandingController == null || !(flock != null))
				return;
			LandingController._flock = flock.FlockController;
			LandingController.LandAll();
		}
	}

	private void Start() {
		if (LandingController != null) {
			preseted = LandingController._flock != null;
			LandingController._onlyBirdsAbove = true;
			LandingController._maxBirdDistance = 30f;
		}

		ServiceLocator.GetService<FlockService>()?.RegisterLandingZone(this);
	}

	private void OnDestroy() {
		ServiceLocator.GetService<FlockService>()?.UnregisterLandingZone(this);
	}

	public void Scare() {
		if (LandingController == null)
			return;
		LandingController.ScareAll();
	}
}