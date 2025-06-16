using UnityEngine;

namespace RootMotion.FinalIK
{
  public class FBBIKArmBending : MonoBehaviour
  {
    public FullBodyBipedIK ik;
    public Vector3 bendDirectionOffsetLeft;
    public Vector3 bendDirectionOffsetRight;
    public Vector3 characterSpaceBendOffsetLeft;
    public Vector3 characterSpaceBendOffsetRight;
    private Quaternion leftHandTargetRotation;
    private Quaternion rightHandTargetRotation;
    private bool initiated;

    private void LateUpdate()
    {
      if (ik == null)
        return;
      if (!initiated)
      {
        IKSolverFullBodyBiped solver = ik.solver;
        solver.OnPostUpdate = solver.OnPostUpdate + OnPostFBBIK;
        initiated = true;
      }
      if (ik.solver.leftHandEffector.target != null)
      {
        ik.solver.leftArmChain.bendConstraint.direction = ik.solver.leftHandEffector.target.rotation * Vector3.left + ik.solver.leftHandEffector.target.rotation * bendDirectionOffsetLeft + ik.transform.rotation * characterSpaceBendOffsetLeft;
        ik.solver.leftArmChain.bendConstraint.weight = 1f;
      }
      if (!(ik.solver.rightHandEffector.target != null))
        return;
      ik.solver.rightArmChain.bendConstraint.direction = ik.solver.rightHandEffector.target.rotation * Vector3.right + ik.solver.rightHandEffector.target.rotation * bendDirectionOffsetRight + ik.transform.rotation * characterSpaceBendOffsetRight;
      ik.solver.rightArmChain.bendConstraint.weight = 1f;
    }

    private void OnPostFBBIK()
    {
      if (ik == null)
        return;
      if (ik.solver.leftHandEffector.target != null)
        ik.references.leftHand.rotation = ik.solver.leftHandEffector.target.rotation;
      if (!(ik.solver.rightHandEffector.target != null))
        return;
      ik.references.rightHand.rotation = ik.solver.rightHandEffector.target.rotation;
    }

    private void OnDestroy()
    {
      if (!(ik != null))
        return;
      IKSolverFullBodyBiped solver = ik.solver;
      solver.OnPostUpdate = solver.OnPostUpdate - OnPostFBBIK;
    }
  }
}
