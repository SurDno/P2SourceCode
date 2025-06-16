using UnityEngine;

namespace RootMotion.Dynamics
{
  public abstract class Prop : MonoBehaviour
  {
    [Tooltip("This has no other purpose but helping you distinguish props by PropRoot.currentProp.propType.")]
    public int propType;
    [LargeHeader("Muscle")]
    [Tooltip("The Muscle of this prop.")]
    public ConfigurableJoint muscle;
    [Tooltip("The muscle properties that will be applied to the Muscle on pickup.")]
    public Muscle.Props muscleProps = new Muscle.Props();
    [Tooltip("If true, this prop's layer will be forced to PuppetMaster layer and target's layer forced to PuppetMaster's Target Root's layer when the prop is picked up.")]
    public bool forceLayers = true;
    [LargeHeader("Additional Pin")]
    [Tooltip("Optinal additional pin, useful for long melee weapons that would otherwise require a lot of muscle force and solver iterations to be swinged quickly. Should normally be without any colliders attached. The position of the pin, it's mass and the pin weight will effect how the prop will handle.")]
    public ConfigurableJoint additionalPin;
    [Tooltip("Target Transform for the additional pin.")]
    public Transform additionalPinTarget;
    [Tooltip("The pin weight of the additional pin. Increasing this weight will make the prop follow animation better, but will increase jitter when colliding with objects.")]
    [Range(0.0f, 1f)]
    public float additionalPinWeight = 1f;
    private ConfigurableJointMotion xMotion;
    private ConfigurableJointMotion yMotion;
    private ConfigurableJointMotion zMotion;
    private ConfigurableJointMotion angularXMotion;
    private ConfigurableJointMotion angularYMotion;
    private ConfigurableJointMotion angularZMotion;
    private Collider[] colliders = new Collider[0];

    public bool isPickedUp => (Object) this.propRoot != (Object) null;

    public PropRoot propRoot { get; private set; }

    public void PickUp(PropRoot propRoot)
    {
      this.muscle.xMotion = this.xMotion;
      this.muscle.yMotion = this.yMotion;
      this.muscle.zMotion = this.zMotion;
      this.muscle.angularXMotion = this.angularXMotion;
      this.muscle.angularYMotion = this.angularYMotion;
      this.muscle.angularZMotion = this.angularZMotion;
      this.propRoot = propRoot;
      this.muscle.gameObject.layer = propRoot.puppetMaster.gameObject.layer;
      foreach (Collider collider in this.colliders)
      {
        if (!collider.isTrigger)
          collider.gameObject.layer = this.muscle.gameObject.layer;
      }
      this.OnPickUp(propRoot);
    }

    public void Drop()
    {
      this.propRoot = (PropRoot) null;
      this.OnDrop();
    }

    public void StartPickedUp(PropRoot propRoot) => this.propRoot = propRoot;

    protected virtual void OnPickUp(PropRoot propRoot)
    {
    }

    protected virtual void OnDrop()
    {
    }

    protected virtual void OnStart()
    {
    }

    protected virtual void Awake()
    {
      if (this.transform.position != this.muscle.transform.position)
        Debug.LogError((object) "Prop target position must match exactly with it's muscle's position!", (Object) this.transform);
      this.xMotion = this.muscle.xMotion;
      this.yMotion = this.muscle.yMotion;
      this.zMotion = this.muscle.zMotion;
      this.angularXMotion = this.muscle.angularXMotion;
      this.angularYMotion = this.muscle.angularYMotion;
      this.angularZMotion = this.muscle.angularZMotion;
      this.colliders = this.muscle.GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
      if (!this.isPickedUp)
        this.ReleaseJoint();
      this.OnStart();
    }

    private void ReleaseJoint()
    {
      this.muscle.connectedBody = (Rigidbody) null;
      this.muscle.targetRotation = Quaternion.identity;
      this.muscle.slerpDrive = new JointDrive()
      {
        positionSpring = 0.0f
      };
      this.muscle.xMotion = ConfigurableJointMotion.Free;
      this.muscle.yMotion = ConfigurableJointMotion.Free;
      this.muscle.zMotion = ConfigurableJointMotion.Free;
      this.muscle.angularXMotion = ConfigurableJointMotion.Free;
      this.muscle.angularYMotion = ConfigurableJointMotion.Free;
      this.muscle.angularZMotion = ConfigurableJointMotion.Free;
    }

    private void OnDrawGizmos()
    {
      if ((Object) this.muscle == (Object) null || Application.isPlaying)
        return;
      this.transform.position = this.muscle.transform.position;
      this.transform.rotation = this.muscle.transform.rotation;
      if ((Object) this.additionalPinTarget != (Object) null && (Object) this.additionalPin != (Object) null)
        this.additionalPinTarget.position = this.additionalPin.transform.position;
      this.muscleProps.group = Muscle.Group.Prop;
    }
  }
}
