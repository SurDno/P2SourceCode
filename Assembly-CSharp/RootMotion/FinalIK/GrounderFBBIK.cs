using System;

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
    public SpineEffector[] spine = new SpineEffector[0];
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
      solver.Reset();
      spineOffset = Vector3.zero;
    }

    private bool IsReadyToInitiate()
    {
      return !((UnityEngine.Object) ik == (UnityEngine.Object) null) && ik.solver.initiated;
    }

    private void Update()
    {
      firstSolve = true;
      weight = Mathf.Clamp(weight, 0.0f, 1f);
      if (weight <= 0.0 || initiated || !IsReadyToInitiate())
        return;
      Initiate();
    }

    private void FixedUpdate() => firstSolve = true;

    private void LateUpdate() => firstSolve = true;

    private void Initiate()
    {
      ik.solver.leftLegMapping.maintainRotationWeight = 1f;
      ik.solver.rightLegMapping.maintainRotationWeight = 1f;
      feet = new Transform[2];
      feet[0] = ik.solver.leftFootEffector.bone;
      feet[1] = ik.solver.rightFootEffector.bone;
      IKSolverFullBodyBiped solver = ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate + OnSolverUpdate;
      this.solver.Initiate(ik.references.root, feet);
      initiated = true;
    }

    private void OnSolverUpdate()
    {
      if (!firstSolve)
        return;
      firstSolve = false;
      if (!this.enabled || weight <= 0.0)
        return;
      if (OnPreGrounder != null)
        OnPreGrounder();
      solver.Update();
      ik.references.pelvis.position += solver.pelvis.IKOffset * weight;
      SetLegIK(ik.solver.leftFootEffector, solver.legs[0]);
      SetLegIK(ik.solver.rightFootEffector, solver.legs[1]);
      if (spineBend != 0.0)
      {
        spineSpeed = Mathf.Clamp(spineSpeed, 0.0f, spineSpeed);
        spineOffset = Vector3.Lerp(spineOffset, GetSpineOffsetTarget() * weight * spineBend, Time.deltaTime * spineSpeed);
        Vector3 vector3 = ik.references.root.up * spineOffset.magnitude;
        for (int index = 0; index < spine.Length; ++index)
          ik.solver.GetEffector(spine[index].effectorType).positionOffset += spineOffset * spine[index].horizontalWeight + vector3 * spine[index].verticalWeight;
      }
      if (OnPostGrounder == null)
        return;
      OnPostGrounder();
    }

    private void SetLegIK(IKEffector effector, Grounding.Leg leg)
    {
      effector.positionOffset += (leg.IKPosition - effector.bone.position) * weight;
      effector.bone.rotation = Quaternion.Slerp(Quaternion.identity, leg.rotationOffset, weight) * effector.bone.rotation;
    }

    private void OnDrawGizmosSelected()
    {
      if ((UnityEngine.Object) ik == (UnityEngine.Object) null)
        ik = this.GetComponent<FullBodyBipedIK>();
      if ((UnityEngine.Object) ik == (UnityEngine.Object) null)
        ik = this.GetComponentInParent<FullBodyBipedIK>();
      if (!((UnityEngine.Object) ik == (UnityEngine.Object) null))
        return;
      ik = this.GetComponentInChildren<FullBodyBipedIK>();
    }

    private void OnDestroy()
    {
      if (!initiated || !((UnityEngine.Object) ik != (UnityEngine.Object) null))
        return;
      IKSolverFullBodyBiped solver = ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate - OnSolverUpdate;
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
