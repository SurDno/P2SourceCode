using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [HelpURL("https://www.youtube.com/watch?v=9MiZiaJorws&index=6&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Grounder/Grounder Full Body Biped")]
  public class GrounderFBBIK : Grounder
  {
    [Tooltip("Reference to the FBBIK componet.")]
    public FullBodyBipedIK ik;
    [Tooltip("The amount of spine bending towards upward slopes.")]
    public float spineBend = 2f;
    [Tooltip("The interpolation speed of spine bending.")]
    public float spineSpeed = 3f;
    public GrounderFBBIK.SpineEffector[] spine = new GrounderFBBIK.SpineEffector[0];
    private Transform[] feet = new Transform[2];
    private Vector3 spineOffset;
    private bool firstSolve;

    [ContextMenu("TUTORIAL VIDEO")]
    private void OpenTutorial()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=9MiZiaJorws&index=6&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6");
    }

    [ContextMenu("User Manual")]
    protected override void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page11.html");
    }

    [ContextMenu("Scrpt Reference")]
    protected override void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_grounder_f_b_b_i_k.html");
    }

    public override void ResetPosition()
    {
      this.solver.Reset();
      this.spineOffset = Vector3.zero;
    }

    private bool IsReadyToInitiate()
    {
      return !((UnityEngine.Object) this.ik == (UnityEngine.Object) null) && this.ik.solver.initiated;
    }

    private void Update()
    {
      this.firstSolve = true;
      this.weight = Mathf.Clamp(this.weight, 0.0f, 1f);
      if ((double) this.weight <= 0.0 || this.initiated || !this.IsReadyToInitiate())
        return;
      this.Initiate();
    }

    private void FixedUpdate() => this.firstSolve = true;

    private void LateUpdate() => this.firstSolve = true;

    private void Initiate()
    {
      this.ik.solver.leftLegMapping.maintainRotationWeight = 1f;
      this.ik.solver.rightLegMapping.maintainRotationWeight = 1f;
      this.feet = new Transform[2];
      this.feet[0] = this.ik.solver.leftFootEffector.bone;
      this.feet[1] = this.ik.solver.rightFootEffector.bone;
      IKSolverFullBodyBiped solver = this.ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate + new IKSolver.UpdateDelegate(this.OnSolverUpdate);
      this.solver.Initiate(this.ik.references.root, this.feet);
      this.initiated = true;
    }

    private void OnSolverUpdate()
    {
      if (!this.firstSolve)
        return;
      this.firstSolve = false;
      if (!this.enabled || (double) this.weight <= 0.0)
        return;
      if (this.OnPreGrounder != null)
        this.OnPreGrounder();
      this.solver.Update();
      this.ik.references.pelvis.position += this.solver.pelvis.IKOffset * this.weight;
      this.SetLegIK(this.ik.solver.leftFootEffector, this.solver.legs[0]);
      this.SetLegIK(this.ik.solver.rightFootEffector, this.solver.legs[1]);
      if ((double) this.spineBend != 0.0)
      {
        this.spineSpeed = Mathf.Clamp(this.spineSpeed, 0.0f, this.spineSpeed);
        this.spineOffset = Vector3.Lerp(this.spineOffset, this.GetSpineOffsetTarget() * this.weight * this.spineBend, Time.deltaTime * this.spineSpeed);
        Vector3 vector3 = this.ik.references.root.up * this.spineOffset.magnitude;
        for (int index = 0; index < this.spine.Length; ++index)
          this.ik.solver.GetEffector(this.spine[index].effectorType).positionOffset += this.spineOffset * this.spine[index].horizontalWeight + vector3 * this.spine[index].verticalWeight;
      }
      if (this.OnPostGrounder == null)
        return;
      this.OnPostGrounder();
    }

    private void SetLegIK(IKEffector effector, Grounding.Leg leg)
    {
      effector.positionOffset += (leg.IKPosition - effector.bone.position) * this.weight;
      effector.bone.rotation = Quaternion.Slerp(Quaternion.identity, leg.rotationOffset, this.weight) * effector.bone.rotation;
    }

    private void OnDrawGizmosSelected()
    {
      if ((UnityEngine.Object) this.ik == (UnityEngine.Object) null)
        this.ik = this.GetComponent<FullBodyBipedIK>();
      if ((UnityEngine.Object) this.ik == (UnityEngine.Object) null)
        this.ik = this.GetComponentInParent<FullBodyBipedIK>();
      if (!((UnityEngine.Object) this.ik == (UnityEngine.Object) null))
        return;
      this.ik = this.GetComponentInChildren<FullBodyBipedIK>();
    }

    private void OnDestroy()
    {
      if (!this.initiated || !((UnityEngine.Object) this.ik != (UnityEngine.Object) null))
        return;
      IKSolverFullBodyBiped solver = this.ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate - new IKSolver.UpdateDelegate(this.OnSolverUpdate);
    }

    [Serializable]
    public class SpineEffector
    {
      [Tooltip("The type of the effector.")]
      public FullBodyBipedEffector effectorType;
      [Tooltip("The weight of horizontal bend offset towards the slope.")]
      public float horizontalWeight = 1f;
      [Tooltip("The vertical bend offset weight.")]
      public float verticalWeight;

      public SpineEffector()
      {
      }

      public SpineEffector(
        FullBodyBipedEffector effectorType,
        float horizontalWeight,
        float verticalWeight)
      {
        this.effectorType = effectorType;
        this.horizontalWeight = horizontalWeight;
        this.verticalWeight = verticalWeight;
      }
    }
  }
}
