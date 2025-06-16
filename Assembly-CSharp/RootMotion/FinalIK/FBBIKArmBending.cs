// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.FBBIKArmBending
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
      if ((Object) this.ik == (Object) null)
        return;
      if (!this.initiated)
      {
        IKSolverFullBodyBiped solver = this.ik.solver;
        solver.OnPostUpdate = solver.OnPostUpdate + new IKSolver.UpdateDelegate(this.OnPostFBBIK);
        this.initiated = true;
      }
      if ((Object) this.ik.solver.leftHandEffector.target != (Object) null)
      {
        this.ik.solver.leftArmChain.bendConstraint.direction = this.ik.solver.leftHandEffector.target.rotation * Vector3.left + this.ik.solver.leftHandEffector.target.rotation * this.bendDirectionOffsetLeft + this.ik.transform.rotation * this.characterSpaceBendOffsetLeft;
        this.ik.solver.leftArmChain.bendConstraint.weight = 1f;
      }
      if (!((Object) this.ik.solver.rightHandEffector.target != (Object) null))
        return;
      this.ik.solver.rightArmChain.bendConstraint.direction = this.ik.solver.rightHandEffector.target.rotation * Vector3.right + this.ik.solver.rightHandEffector.target.rotation * this.bendDirectionOffsetRight + this.ik.transform.rotation * this.characterSpaceBendOffsetRight;
      this.ik.solver.rightArmChain.bendConstraint.weight = 1f;
    }

    private void OnPostFBBIK()
    {
      if ((Object) this.ik == (Object) null)
        return;
      if ((Object) this.ik.solver.leftHandEffector.target != (Object) null)
        this.ik.references.leftHand.rotation = this.ik.solver.leftHandEffector.target.rotation;
      if (!((Object) this.ik.solver.rightHandEffector.target != (Object) null))
        return;
      this.ik.references.rightHand.rotation = this.ik.solver.rightHandEffector.target.rotation;
    }

    private void OnDestroy()
    {
      if (!((Object) this.ik != (Object) null))
        return;
      IKSolverFullBodyBiped solver = this.ik.solver;
      solver.OnPostUpdate = solver.OnPostUpdate - new IKSolver.UpdateDelegate(this.OnPostFBBIK);
    }
  }
}
