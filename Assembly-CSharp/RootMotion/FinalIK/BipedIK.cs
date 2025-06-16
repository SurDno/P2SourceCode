using UnityEngine;

namespace RootMotion.FinalIK
{
  [HelpURL("http://www.root-motion.com/finalikdox/html/page2.html")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/IK/Biped IK")]
  public class BipedIK : SolverManager
  {
    public BipedReferences references = new BipedReferences();
    public BipedIKSolvers solvers = new BipedIKSolvers();

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page2.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_biped_i_k.html");
    }

    [ContextMenu("Support Group")]
    private void SupportGroup()
    {
      Application.OpenURL("https://groups.google.com/forum/#!forum/final-ik");
    }

    [ContextMenu("Asset Store Thread")]
    private void ASThread()
    {
      Application.OpenURL("http://forum.unity3d.com/threads/final-ik-full-body-ik-aim-look-at-fabrik-ccd-ik-1-0-released.222685/");
    }

    public float GetIKPositionWeight(AvatarIKGoal goal)
    {
      return this.GetGoalIK(goal).GetIKPositionWeight();
    }

    public float GetIKRotationWeight(AvatarIKGoal goal)
    {
      return this.GetGoalIK(goal).GetIKRotationWeight();
    }

    public void SetIKPositionWeight(AvatarIKGoal goal, float weight)
    {
      this.GetGoalIK(goal).SetIKPositionWeight(weight);
    }

    public void SetIKRotationWeight(AvatarIKGoal goal, float weight)
    {
      this.GetGoalIK(goal).SetIKRotationWeight(weight);
    }

    public void SetIKPosition(AvatarIKGoal goal, Vector3 IKPosition)
    {
      this.GetGoalIK(goal).SetIKPosition(IKPosition);
    }

    public void SetIKRotation(AvatarIKGoal goal, Quaternion IKRotation)
    {
      this.GetGoalIK(goal).SetIKRotation(IKRotation);
    }

    public Vector3 GetIKPosition(AvatarIKGoal goal) => this.GetGoalIK(goal).GetIKPosition();

    public Quaternion GetIKRotation(AvatarIKGoal goal) => this.GetGoalIK(goal).GetIKRotation();

    public void SetLookAtWeight(
      float weight,
      float bodyWeight,
      float headWeight,
      float eyesWeight,
      float clampWeight,
      float clampWeightHead,
      float clampWeightEyes)
    {
      this.solvers.lookAt.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight, clampWeightHead, clampWeightEyes);
    }

    public void SetLookAtPosition(Vector3 lookAtPosition)
    {
      this.solvers.lookAt.SetIKPosition(lookAtPosition);
    }

    public void SetSpinePosition(Vector3 spinePosition)
    {
      this.solvers.spine.SetIKPosition(spinePosition);
    }

    public void SetSpineWeight(float weight) => this.solvers.spine.SetIKPositionWeight(weight);

    public IKSolverLimb GetGoalIK(AvatarIKGoal goal)
    {
      switch (goal)
      {
        case AvatarIKGoal.LeftFoot:
          return this.solvers.leftFoot;
        case AvatarIKGoal.RightFoot:
          return this.solvers.rightFoot;
        case AvatarIKGoal.LeftHand:
          return this.solvers.leftHand;
        case AvatarIKGoal.RightHand:
          return this.solvers.rightHand;
        default:
          return (IKSolverLimb) null;
      }
    }

    public void InitiateBipedIK() => this.InitiateSolver();

    public void UpdateBipedIK() => this.UpdateSolver();

    public void SetToDefaults()
    {
      foreach (IKSolverLimb limb in this.solvers.limbs)
      {
        limb.SetIKPositionWeight(0.0f);
        limb.SetIKRotationWeight(0.0f);
        limb.bendModifier = IKSolverLimb.BendModifier.Animation;
        limb.bendModifierWeight = 1f;
      }
      this.solvers.leftHand.maintainRotationWeight = 0.0f;
      this.solvers.rightHand.maintainRotationWeight = 0.0f;
      this.solvers.spine.SetIKPositionWeight(0.0f);
      this.solvers.spine.tolerance = 0.0f;
      this.solvers.spine.maxIterations = 2;
      this.solvers.spine.useRotationLimits = false;
      this.solvers.aim.SetIKPositionWeight(0.0f);
      this.solvers.aim.tolerance = 0.0f;
      this.solvers.aim.maxIterations = 2;
      this.SetLookAtWeight(0.0f, 0.5f, 1f, 1f, 0.5f, 0.7f, 0.5f);
    }

    protected override void FixTransforms()
    {
      this.solvers.lookAt.FixTransforms();
      for (int index = 0; index < this.solvers.limbs.Length; ++index)
        this.solvers.limbs[index].FixTransforms();
    }

    protected override void InitiateSolver()
    {
      string errorMessage = "";
      if (BipedReferences.SetupError(this.references, ref errorMessage))
      {
        Warning.Log(errorMessage, this.references.root);
      }
      else
      {
        this.solvers.AssignReferences(this.references);
        if (this.solvers.spine.bones.Length > 1)
          this.solvers.spine.Initiate(this.transform);
        this.solvers.lookAt.Initiate(this.transform);
        this.solvers.aim.Initiate(this.transform);
        foreach (IKSolver limb in this.solvers.limbs)
          limb.Initiate(this.transform);
        this.solvers.pelvis.Initiate(this.references.pelvis);
      }
    }

    protected override void UpdateSolver()
    {
      for (int index = 0; index < this.solvers.limbs.Length; ++index)
      {
        this.solvers.limbs[index].MaintainBend();
        this.solvers.limbs[index].MaintainRotation();
      }
      this.solvers.pelvis.Update();
      if (this.solvers.spine.bones.Length > 1)
        this.solvers.spine.Update();
      this.solvers.aim.Update();
      this.solvers.lookAt.Update();
      for (int index = 0; index < this.solvers.limbs.Length; ++index)
        this.solvers.limbs[index].Update();
    }

    public void LogWarning(string message) => Warning.Log(message, this.transform);
  }
}
