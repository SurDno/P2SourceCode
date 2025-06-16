using UnityEngine;

namespace RootMotion.FinalIK
{
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Grounder/Grounder VRIK")]
  public class GrounderVRIK : Grounder
  {
    [Tooltip("Reference to the VRIK componet.")]
    public VRIK ik;
    private Transform[] feet = new Transform[2];

    [ContextMenu("TUTORIAL VIDEO")]
    private void OpenTutorial()
    {
    }

    [ContextMenu("User Manual")]
    protected override void OpenUserManual()
    {
    }

    [ContextMenu("Scrpt Reference")]
    protected override void OpenScriptReference()
    {
    }

    public override void ResetPosition() => this.solver.Reset();

    private bool IsReadyToInitiate()
    {
      return !((Object) this.ik == (Object) null) && this.ik.solver.initiated;
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
      this.feet[0] = this.ik.references.leftFoot;
      this.feet[1] = this.ik.references.rightFoot;
      IKSolverVR solver1 = this.ik.solver;
      solver1.OnPreUpdate = solver1.OnPreUpdate + new IKSolver.UpdateDelegate(this.OnSolverUpdate);
      IKSolverVR solver2 = this.ik.solver;
      solver2.OnPostUpdate = solver2.OnPostUpdate + new IKSolver.UpdateDelegate(this.OnPostSolverUpdate);
      this.solver.Initiate(this.ik.references.root, this.feet);
      this.initiated = true;
    }

    private void OnSolverUpdate()
    {
      if (!this.enabled || (double) this.weight <= 0.0)
        return;
      if (this.OnPreGrounder != null)
        this.OnPreGrounder();
      this.solver.Update();
      this.ik.references.pelvis.position += this.solver.pelvis.IKOffset * this.weight;
      this.ik.solver.AddPositionOffset(IKSolverVR.PositionOffset.LeftFoot, (this.solver.legs[0].IKPosition - this.ik.references.leftFoot.position) * this.weight);
      this.ik.solver.AddPositionOffset(IKSolverVR.PositionOffset.RightFoot, (this.solver.legs[1].IKPosition - this.ik.references.rightFoot.position) * this.weight);
      if (this.OnPostGrounder == null)
        return;
      this.OnPostGrounder();
    }

    private void SetLegIK(
      IKSolverVR.PositionOffset positionOffset,
      Transform bone,
      Grounding.Leg leg)
    {
      this.ik.solver.AddPositionOffset(positionOffset, (leg.IKPosition - bone.position) * this.weight);
    }

    private void OnPostSolverUpdate()
    {
      this.ik.references.leftFoot.rotation = Quaternion.Slerp(Quaternion.identity, this.solver.legs[0].rotationOffset, this.weight) * this.ik.references.leftFoot.rotation;
      this.ik.references.rightFoot.rotation = Quaternion.Slerp(Quaternion.identity, this.solver.legs[1].rotationOffset, this.weight) * this.ik.references.rightFoot.rotation;
    }

    private void OnDrawGizmosSelected()
    {
      if ((Object) this.ik == (Object) null)
        this.ik = this.GetComponent<VRIK>();
      if ((Object) this.ik == (Object) null)
        this.ik = this.GetComponentInParent<VRIK>();
      if (!((Object) this.ik == (Object) null))
        return;
      this.ik = this.GetComponentInChildren<VRIK>();
    }

    private void OnDestroy()
    {
      if (!this.initiated || !((Object) this.ik != (Object) null))
        return;
      IKSolverVR solver1 = this.ik.solver;
      solver1.OnPreUpdate = solver1.OnPreUpdate - new IKSolver.UpdateDelegate(this.OnSolverUpdate);
      IKSolverVR solver2 = this.ik.solver;
      solver2.OnPostUpdate = solver2.OnPostUpdate - new IKSolver.UpdateDelegate(this.OnPostSolverUpdate);
    }
  }
}
