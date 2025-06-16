using UnityEngine;

namespace RootMotion.FinalIK
{
  [HelpURL("http://www.root-motion.com/finalikdox/html/page11.html")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Grounder/Grounder IK")]
  public class GrounderIK : Grounder
  {
    public IK[] legs;
    [Tooltip("The pelvis transform. Common ancestor of all the legs.")]
    public Transform pelvis;
    [Tooltip("The root Transform of the character, with the rigidbody and the collider.")]
    public Transform characterRoot;
    [Tooltip("The weight of rotating the character root to the ground normal (range: 0 - 1).")]
    [Range(0.0f, 1f)]
    public float rootRotationWeight;
    [Tooltip("The speed of rotating the character root to the ground normal (range: 0 - inf).")]
    public float rootRotationSpeed = 5f;
    [Tooltip("The maximum angle of root rotation (range: 0 - 90).")]
    public float maxRootRotationAngle = 45f;
    private Transform[] feet = new Transform[0];
    private Quaternion[] footRotations = new Quaternion[0];
    private Vector3 animatedPelvisLocalPosition;
    private Vector3 solvedPelvisLocalPosition;
    private int solvedFeet;
    private bool solved;
    private float lastWeight;

    [ContextMenu("User Manual")]
    protected override void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page11.html");
    }

    [ContextMenu("Scrpt Reference")]
    protected override void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_grounder_i_k.html");
    }

    public override void ResetPosition() => this.solver.Reset();

    private bool IsReadyToInitiate()
    {
      if ((Object) this.pelvis == (Object) null || this.legs.Length == 0)
        return false;
      foreach (IK leg in this.legs)
      {
        if ((Object) leg == (Object) null)
          return false;
        switch (leg)
        {
          case FullBodyBipedIK _:
            this.LogWarning("GrounderIK does not support FullBodyBipedIK, use CCDIK, FABRIK, LimbIK or TrigonometricIK instead. If you want to use FullBodyBipedIK, use the GrounderFBBIK component.");
            return false;
          case FABRIKRoot _:
            this.LogWarning("GrounderIK does not support FABRIKRoot, use CCDIK, FABRIK, LimbIK or TrigonometricIK instead.");
            return false;
          case AimIK _:
            this.LogWarning("GrounderIK does not support AimIK, use CCDIK, FABRIK, LimbIK or TrigonometricIK instead.");
            return false;
          default:
            continue;
        }
      }
      return true;
    }

    private void OnDisable()
    {
      if (!this.initiated)
        return;
      for (int index = 0; index < this.legs.Length; ++index)
      {
        if ((Object) this.legs[index] != (Object) null)
          this.legs[index].GetIKSolver().IKPositionWeight = 0.0f;
      }
    }

    private void Update()
    {
      this.weight = Mathf.Clamp(this.weight, 0.0f, 1f);
      if ((double) this.weight <= 0.0)
        return;
      this.solved = false;
      if (this.initiated)
      {
        this.rootRotationWeight = Mathf.Clamp(this.rootRotationWeight, 0.0f, 1f);
        this.rootRotationSpeed = Mathf.Clamp(this.rootRotationSpeed, 0.0f, this.rootRotationSpeed);
        if (!((Object) this.characterRoot != (Object) null) || (double) this.rootRotationSpeed <= 0.0 || (double) this.rootRotationWeight <= 0.0)
          return;
        Vector3 vector3 = this.solver.GetLegsPlaneNormal();
        if ((double) this.rootRotationWeight < 1.0)
          vector3 = Vector3.Slerp(Vector3.up, vector3, this.rootRotationWeight);
        this.characterRoot.rotation = Quaternion.Lerp(this.characterRoot.rotation, Quaternion.RotateTowards(Quaternion.FromToRotation(this.transform.up, Vector3.up) * this.characterRoot.rotation, Quaternion.FromToRotation(this.transform.up, vector3) * this.characterRoot.rotation, this.maxRootRotationAngle), Time.deltaTime * this.rootRotationSpeed);
      }
      else
      {
        if (!this.IsReadyToInitiate())
          return;
        this.Initiate();
      }
    }

    private void Initiate()
    {
      this.feet = new Transform[this.legs.Length];
      this.footRotations = new Quaternion[this.legs.Length];
      for (int index = 0; index < this.feet.Length; ++index)
        this.footRotations[index] = Quaternion.identity;
      for (int index = 0; index < this.legs.Length; ++index)
      {
        IKSolver.Point[] points = this.legs[index].GetIKSolver().GetPoints();
        this.feet[index] = points[points.Length - 1].transform;
        this.legs[index].GetIKSolver().OnPreUpdate += new IKSolver.UpdateDelegate(this.OnSolverUpdate);
        this.legs[index].GetIKSolver().OnPostUpdate += new IKSolver.UpdateDelegate(this.OnPostSolverUpdate);
      }
      this.animatedPelvisLocalPosition = this.pelvis.localPosition;
      this.solver.Initiate(this.transform, this.feet);
      for (int index = 0; index < this.legs.Length; ++index)
      {
        if (this.legs[index] is LegIK)
          this.solver.legs[index].invertFootCenter = true;
      }
      this.initiated = true;
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
      if (this.solved)
        return;
      if (this.OnPreGrounder != null)
        this.OnPreGrounder();
      if (this.pelvis.localPosition != this.solvedPelvisLocalPosition)
        this.animatedPelvisLocalPosition = this.pelvis.localPosition;
      else
        this.pelvis.localPosition = this.animatedPelvisLocalPosition;
      this.solver.Update();
      for (int index = 0; index < this.legs.Length; ++index)
        this.SetLegIK(index);
      this.pelvis.position += this.solver.pelvis.IKOffset * this.weight;
      this.solved = true;
      this.solvedFeet = 0;
      if (this.OnPostGrounder == null)
        return;
      this.OnPostGrounder();
    }

    private void SetLegIK(int index)
    {
      this.footRotations[index] = this.feet[index].rotation;
      if (this.legs[index] is LegIK)
      {
        (this.legs[index].GetIKSolver() as IKSolverLeg).IKRotation = Quaternion.Slerp(Quaternion.identity, this.solver.legs[index].rotationOffset, this.weight) * this.footRotations[index];
        (this.legs[index].GetIKSolver() as IKSolverLeg).IKRotationWeight = 1f;
      }
      this.legs[index].GetIKSolver().IKPosition = this.solver.legs[index].IKPosition;
      this.legs[index].GetIKSolver().IKPositionWeight = this.weight;
    }

    private void OnPostSolverUpdate()
    {
      if ((double) this.weight <= 0.0 || !this.enabled)
        return;
      ++this.solvedFeet;
      if (this.solvedFeet < this.feet.Length)
        return;
      for (int index = 0; index < this.feet.Length; ++index)
        this.feet[index].rotation = Quaternion.Slerp(Quaternion.identity, this.solver.legs[index].rotationOffset, this.weight) * this.footRotations[index];
      this.solvedPelvisLocalPosition = this.pelvis.localPosition;
    }

    private void OnDestroy()
    {
      if (!this.initiated)
        return;
      foreach (IK leg in this.legs)
      {
        if ((Object) leg != (Object) null)
        {
          leg.GetIKSolver().OnPreUpdate -= new IKSolver.UpdateDelegate(this.OnSolverUpdate);
          leg.GetIKSolver().OnPostUpdate -= new IKSolver.UpdateDelegate(this.OnPostSolverUpdate);
        }
      }
    }
  }
}
