using System.Collections.Generic;
using Engine.Common.Components.Locations;
using Engine.Source.Services;

public class POIBase : MonoBehaviour
{
  public static HashSet<POIBase> ActivePOIs = new HashSet<POIBase>();
  [SerializeField]
  [FormerlySerializedAs("SupportedAnimations")]
  private POIAnimationEnum _supportedAnimations;
  [SerializeField]
  private bool supportsDialogs;
  private LocationType locationType = LocationType.None;
  protected GameObject lockedBy;
  protected POIAnimationEnum lockedByAnimation;
  private bool isChildPOI;

  public POIAnimationEnum SupportedAnimations
  {
    get => _supportedAnimations;
    set => _supportedAnimations = value;
  }

  public bool SupportsDialog
  {
    get => supportsDialogs;
    set => supportsDialogs = value;
  }

  public LocationType LocationType
  {
    get
    {
      if (locationType == LocationType.None)
        locationType = LocationItemUtility.GetLocationType(this.gameObject);
      return locationType;
    }
  }

  public bool IsChildPOI
  {
    get => isChildPOI;
    set => isChildPOI = value;
  }

  private void Start()
  {
  }

  private void OnEnable()
  {
    if (isChildPOI)
      return;
    ActivePOIs.Add(this);
  }

  private void OnDisable() => ActivePOIs.Remove(this);

  public void Lock(GameObject who, POIAnimationEnum animation)
  {
    lockedBy = who;
    lockedByAnimation = animation;
  }

  public void Unlock(GameObject who)
  {
    lockedBy = (GameObject) null;
    lockedByAnimation = 0;
  }

  public GameObject LockedBy => lockedBy;

  public float GetAngle(GameObject to)
  {
    Vector3 lhs = to.transform.position - this.transform.position;
    lhs = lhs.normalized;
    Vector3 vector3 = this.transform.forward * -1f;
    float num = Mathf.Acos(Vector3.Dot(lhs, vector3)) * 57.29578f;
    Vector3 rhs = to.transform.position - this.transform.position;
    float y = Vector3.Cross(vector3, rhs).y;
    return num * Mathf.Sign(y);
  }

  public virtual void GetClosestTargetPoint(
    POIAnimationEnum animation,
    int animationIndex,
    POISetup character,
    Vector3 currentPosition,
    out Vector3 closestTargetPosition,
    out Quaternion closestTargetRotation)
  {
    closestTargetPosition = this.transform.position;
    closestTargetRotation = this.transform.rotation;
  }

  public virtual void GetRandomTargetPoint(
    POIAnimationEnum animation,
    int animationIndex,
    POISetup character,
    out Vector3 targetPosition,
    out Quaternion targetRotation)
  {
    targetPosition = this.transform.position;
    targetRotation = this.transform.rotation;
  }
}
