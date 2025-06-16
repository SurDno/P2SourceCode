// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.GrounderBipedIK
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [HelpURL("http://www.root-motion.com/finalikdox/html/page11.html")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Grounder/Grounder Biped")]
  public class GrounderBipedIK : Grounder
  {
    [Tooltip("The BipedIK componet.")]
    public BipedIK ik;
    [Tooltip("The amount of spine bending towards upward slopes.")]
    public float spineBend = 7f;
    [Tooltip("The interpolation speed of spine bending.")]
    public float spineSpeed = 3f;
    private Transform[] feet = new Transform[2];
    private Quaternion[] footRotations = new Quaternion[2];
    private Vector3 animatedPelvisLocalPosition;
    private Vector3 solvedPelvisLocalPosition;
    private Vector3 spineOffset;
    private float lastWeight;

    [ContextMenu("User Manual")]
    protected override void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page11.html");
    }

    [ContextMenu("Scrpt Reference")]
    protected override void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_grounder_biped_i_k.html");
    }

    public override void ResetPosition()
    {
      this.solver.Reset();
      this.spineOffset = Vector3.zero;
    }

    private bool IsReadyToInitiate()
    {
      return !((Object) this.ik == (Object) null) && this.ik.solvers.leftFoot.initiated && this.ik.solvers.rightFoot.initiated;
    }

    private void Update()
    {
      this.weight = Mathf.Clamp(this.weight, 0.0f, 1f);
      if ((double) this.weight <= 0.0 || this.initiated || !this.IsReadyToInitiate())
        return;
      this.Initiate();
    }

    private void Initiate()
    {
      this.feet = new Transform[2];
      this.footRotations = new Quaternion[2];
      this.feet[0] = this.ik.references.leftFoot;
      this.feet[1] = this.ik.references.rightFoot;
      this.footRotations[0] = Quaternion.identity;
      this.footRotations[1] = Quaternion.identity;
      IKSolverFABRIK spine = this.ik.solvers.spine;
      spine.OnPreUpdate = spine.OnPreUpdate + new IKSolver.UpdateDelegate(this.OnSolverUpdate);
      IKSolverLimb rightFoot = this.ik.solvers.rightFoot;
      rightFoot.OnPostUpdate = rightFoot.OnPostUpdate + new IKSolver.UpdateDelegate(this.OnPostSolverUpdate);
      this.animatedPelvisLocalPosition = this.ik.references.pelvis.localPosition;
      this.solver.Initiate(this.ik.references.root, this.feet);
      this.initiated = true;
    }

    private void OnDisable()
    {
      if (!this.initiated)
        return;
      this.ik.solvers.leftFoot.IKPositionWeight = 0.0f;
      this.ik.solvers.rightFoot.IKPositionWeight = 0.0f;
    }

    private void OnSolverUpdate()
    {
      if (!this.enabled)
        return;
      if ((double) this.weight <= 0.0)
      {
        if ((double) this.lastWeight <= 0.0)
          return;
        this.OnDisable();
      }
      this.lastWeight = this.weight;
      if (this.OnPreGrounder != null)
        this.OnPreGrounder();
      if (this.ik.references.pelvis.localPosition != this.solvedPelvisLocalPosition)
        this.animatedPelvisLocalPosition = this.ik.references.pelvis.localPosition;
      else
        this.ik.references.pelvis.localPosition = this.animatedPelvisLocalPosition;
      this.solver.Update();
      this.ik.references.pelvis.position += this.solver.pelvis.IKOffset * this.weight;
      this.SetLegIK(this.ik.solvers.leftFoot, 0);
      this.SetLegIK(this.ik.solvers.rightFoot, 1);
      if ((double) this.spineBend != 0.0 && this.ik.references.spine.Length != 0)
      {
        this.spineSpeed = Mathf.Clamp(this.spineSpeed, 0.0f, this.spineSpeed);
        this.spineOffset = Vector3.Lerp(this.spineOffset, this.GetSpineOffsetTarget() * this.weight * this.spineBend, Time.deltaTime * this.spineSpeed);
        Quaternion rotation1 = this.ik.references.leftUpperArm.rotation;
        Quaternion rotation2 = this.ik.references.rightUpperArm.rotation;
        Vector3 up = this.solver.up;
        this.ik.references.spine[0].rotation = Quaternion.FromToRotation(up, up + this.spineOffset) * this.ik.references.spine[0].rotation;
        this.ik.references.leftUpperArm.rotation = rotation1;
        this.ik.references.rightUpperArm.rotation = rotation2;
      }
      if (this.OnPostGrounder == null)
        return;
      this.OnPostGrounder();
    }

    private void SetLegIK(IKSolverLimb limb, int index)
    {
      this.footRotations[index] = this.feet[index].rotation;
      limb.IKPosition = this.solver.legs[index].IKPosition;
      limb.IKPositionWeight = this.weight;
    }

    private void OnPostSolverUpdate()
    {
      if ((double) this.weight <= 0.0 || !this.enabled)
        return;
      for (int index = 0; index < this.feet.Length; ++index)
        this.feet[index].rotation = Quaternion.Slerp(Quaternion.identity, this.solver.legs[index].rotationOffset, this.weight) * this.footRotations[index];
      this.solvedPelvisLocalPosition = this.ik.references.pelvis.localPosition;
    }

    private void OnDestroy()
    {
      if (!this.initiated || !((Object) this.ik != (Object) null))
        return;
      IKSolverFABRIK spine = this.ik.solvers.spine;
      spine.OnPreUpdate = spine.OnPreUpdate - new IKSolver.UpdateDelegate(this.OnSolverUpdate);
      IKSolverLimb rightFoot = this.ik.solvers.rightFoot;
      rightFoot.OnPostUpdate = rightFoot.OnPostUpdate - new IKSolver.UpdateDelegate(this.OnPostSolverUpdate);
    }
  }
}
