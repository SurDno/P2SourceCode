using UnityEngine;

namespace RootMotion.FinalIK
{
  public class IKExecutionOrder : MonoBehaviour
  {
    [Tooltip("The IK components, assign in the order in which you wish to update them.")]
    public IK[] IKComponents;
    [Tooltip("Optional. Assign it if you are using 'Animate Physics' as the Update Mode.")]
    public Animator animator;
    private bool fixedFrame;

    private bool animatePhysics
    {
      get
      {
        return !((Object) this.animator == (Object) null) && this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics;
      }
    }

    private void Start()
    {
      for (int index = 0; index < this.IKComponents.Length; ++index)
        this.IKComponents[index].enabled = false;
    }

    private void Update()
    {
      if (this.animatePhysics)
        return;
      this.FixTransforms();
    }

    private void FixedUpdate()
    {
      this.fixedFrame = true;
      if (!this.animatePhysics)
        return;
      this.FixTransforms();
    }

    private void LateUpdate()
    {
      if (this.animatePhysics && !this.fixedFrame)
        return;
      for (int index = 0; index < this.IKComponents.Length; ++index)
        this.IKComponents[index].GetIKSolver().Update();
      this.fixedFrame = false;
    }

    private void FixTransforms()
    {
      for (int index = 0; index < this.IKComponents.Length; ++index)
      {
        if (this.IKComponents[index].fixTransforms)
          this.IKComponents[index].GetIKSolver().FixTransforms();
      }
    }
  }
}
