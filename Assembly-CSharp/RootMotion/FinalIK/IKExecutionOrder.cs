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
        return !(animator == null) && animator.updateMode == AnimatorUpdateMode.AnimatePhysics;
      }
    }

    private void Start()
    {
      for (int index = 0; index < IKComponents.Length; ++index)
        IKComponents[index].enabled = false;
    }

    private void Update()
    {
      if (animatePhysics)
        return;
      FixTransforms();
    }

    private void FixedUpdate()
    {
      fixedFrame = true;
      if (!animatePhysics)
        return;
      FixTransforms();
    }

    private void LateUpdate()
    {
      if (animatePhysics && !fixedFrame)
        return;
      for (int index = 0; index < IKComponents.Length; ++index)
        IKComponents[index].GetIKSolver().Update();
      fixedFrame = false;
    }

    private void FixTransforms()
    {
      for (int index = 0; index < IKComponents.Length; ++index)
      {
        if (IKComponents[index].fixTransforms)
          IKComponents[index].GetIKSolver().FixTransforms();
      }
    }
  }
}
