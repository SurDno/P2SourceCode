using UnityEngine;

namespace RootMotion.Dynamics
{
  [HelpURL("http://root-motion.com/puppetmasterdox/html/page6.html")]
  [AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Prop Root")]
  public class PropRoot : MonoBehaviour
  {
    [Tooltip("Reference to the PuppetMaster component.")]
    public PuppetMaster puppetMaster;
    [Tooltip("If a prop is connected, what will it's joint be connected to?")]
    public Rigidbody connectTo;
    [Tooltip("Is there a Prop connected to this PropRoot? Simply assign this value to connect, replace or drop props.")]
    public Prop currentProp;
    private Prop lastProp;
    private bool fixedUpdateCalled;

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page6.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/class_root_motion_1_1_dynamics_1_1_prop_root.html");
    }

    public void DropImmediate()
    {
      if ((Object) this.lastProp == (Object) null)
        return;
      this.puppetMaster.RemoveMuscleRecursive(this.lastProp.muscle, true);
      this.lastProp.Drop();
      this.currentProp = (Prop) null;
      this.lastProp = (Prop) null;
    }

    private void Awake()
    {
      if (!((Object) this.currentProp != (Object) null))
        return;
      this.currentProp.StartPickedUp(this);
    }

    private void Update()
    {
      if (!this.fixedUpdateCalled || !((Object) this.currentProp != (Object) null) || !((Object) this.lastProp == (Object) this.currentProp) || !((Object) this.currentProp.muscle.connectedBody == (Object) null))
        return;
      this.currentProp.Drop();
      this.currentProp = (Prop) null;
      this.lastProp = (Prop) null;
    }

    private void FixedUpdate()
    {
      this.fixedUpdateCalled = true;
      if ((Object) this.currentProp == (Object) this.lastProp)
        return;
      if ((Object) this.currentProp == (Object) null)
      {
        this.puppetMaster.RemoveMuscleRecursive(this.lastProp.muscle, true);
        this.lastProp.Drop();
      }
      if ((Object) this.lastProp == (Object) null)
        this.AttachProp(this.currentProp);
      if ((Object) this.lastProp != (Object) null && (Object) this.currentProp != (Object) null)
      {
        this.puppetMaster.RemoveMuscleRecursive(this.lastProp.muscle, true);
        this.AttachProp(this.currentProp);
      }
      this.lastProp = this.currentProp;
    }

    private void AttachProp(Prop prop)
    {
      prop.transform.position = this.transform.position;
      prop.transform.rotation = this.transform.rotation;
      prop.PickUp(this);
      this.puppetMaster.AddMuscle(prop.muscle, prop.transform, this.connectTo, this.transform, prop.muscleProps, forceLayers: prop.forceLayers);
      if (!((Object) prop.additionalPin != (Object) null) || !((Object) prop.additionalPinTarget != (Object) null))
        return;
      this.puppetMaster.AddMuscle(prop.additionalPin, prop.additionalPinTarget, prop.muscle.GetComponent<Rigidbody>(), prop.transform, new Muscle.Props(prop.additionalPinWeight, 0.0f, 0.0f, 0.0f, false, Muscle.Group.Prop), true, prop.forceLayers);
    }
  }
}
