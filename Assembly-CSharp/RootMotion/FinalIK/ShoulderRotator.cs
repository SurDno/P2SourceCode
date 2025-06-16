// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.ShoulderRotator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  public class ShoulderRotator : MonoBehaviour
  {
    [Tooltip("Weight of shoulder rotation")]
    public float weight = 1.5f;
    [Tooltip("The greater the offset, the sooner the shoulder will start rotating")]
    public float offset = 0.2f;
    private FullBodyBipedIK ik;
    private bool skip;

    private void Start()
    {
      this.ik = this.GetComponent<FullBodyBipedIK>();
      IKSolverFullBodyBiped solver = this.ik.solver;
      solver.OnPostUpdate = solver.OnPostUpdate + new IKSolver.UpdateDelegate(this.RotateShoulders);
    }

    private void RotateShoulders()
    {
      if ((Object) this.ik == (Object) null || (double) this.ik.solver.IKPositionWeight <= 0.0)
        return;
      if (this.skip)
      {
        this.skip = false;
      }
      else
      {
        this.RotateShoulder(FullBodyBipedChain.LeftArm, this.weight, this.offset);
        this.RotateShoulder(FullBodyBipedChain.RightArm, this.weight, this.offset);
        this.skip = true;
        this.ik.solver.Update();
      }
    }

    private void RotateShoulder(FullBodyBipedChain chain, float weight, float offset)
    {
      Quaternion quaternion = Quaternion.Lerp(Quaternion.identity, Quaternion.FromToRotation(this.GetParentBoneMap(chain).swingDirection, this.ik.solver.GetEndEffector(chain).position - this.GetParentBoneMap(chain).transform.position), Mathf.Clamp(((float) ((double) (this.ik.solver.GetEndEffector(chain).position - this.ik.solver.GetLimbMapping(chain).bone1.position).magnitude / (double) (this.ik.solver.GetChain(chain).nodes[0].length + this.ik.solver.GetChain(chain).nodes[1].length) - 1.0) + offset) * weight, 0.0f, 1f) * this.ik.solver.GetEndEffector(chain).positionWeight * this.ik.solver.IKPositionWeight);
      this.ik.solver.GetLimbMapping(chain).parentBone.rotation = quaternion * this.ik.solver.GetLimbMapping(chain).parentBone.rotation;
    }

    private IKMapping.BoneMap GetParentBoneMap(FullBodyBipedChain chain)
    {
      return this.ik.solver.GetLimbMapping(chain).GetBoneMap(IKMappingLimb.BoneMapType.Parent);
    }

    private void OnDestroy()
    {
      if (!((Object) this.ik != (Object) null))
        return;
      IKSolverFullBodyBiped solver = this.ik.solver;
      solver.OnPostUpdate = solver.OnPostUpdate - new IKSolver.UpdateDelegate(this.RotateShoulders);
    }
  }
}
