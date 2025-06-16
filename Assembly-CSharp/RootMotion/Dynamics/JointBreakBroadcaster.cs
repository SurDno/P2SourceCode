using UnityEngine;

namespace RootMotion.Dynamics
{
  public class JointBreakBroadcaster : MonoBehaviour
  {
    [SerializeField]
    [HideInInspector]
    public PuppetMaster puppetMaster;
    [SerializeField]
    [HideInInspector]
    public int muscleIndex;

    private void OnJointBreak()
    {
      if (!this.enabled)
        return;
      this.puppetMaster.RemoveMuscleRecursive(this.puppetMaster.muscles[this.muscleIndex].joint, true, true, MuscleRemoveMode.Numb);
    }
  }
}
