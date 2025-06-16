using UnityEngine;

namespace RootMotion.FinalIK
{
  [HelpURL("http://www.root-motion.com/finalikdox/html/page11.html")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Grounder/Grounder Quadruped")]
  public class GrounderQuadruped : Grounder
  {
    [Tooltip("The Grounding solver for the forelegs.")]
    public Grounding forelegSolver = new Grounding();
    [Tooltip("The weight of rotating the character root to the ground angle (range: 0 - 1).")]
    [Range(0.0f, 1f)]
    public float rootRotationWeight = 0.5f;
    [Tooltip("The maximum angle of rotating the quadruped downwards (going downhill, range: -90 - 0).")]
    [Range(-90f, 0.0f)]
    public float minRootRotation = -25f;
    [Tooltip("The maximum angle of rotating the quadruped upwards (going uphill, range: 0 - 90).")]
    [Range(0.0f, 90f)]
    public float maxRootRotation = 45f;
    [Tooltip("The speed of interpolating the character root rotation (range: 0 - inf).")]
    public float rootRotationSpeed = 5f;
    [Tooltip("The maximum IK offset for the legs (range: 0 - inf).")]
    public float maxLegOffset = 0.5f;
    [Tooltip("The maximum IK offset for the forelegs (range: 0 - inf).")]
    public float maxForeLegOffset = 0.5f;
    [Tooltip("The weight of maintaining the head's rotation as it was before solving the Grounding (range: 0 - 1).")]
    [Range(0.0f, 1f)]
    public float maintainHeadRotationWeight = 0.5f;
    [Tooltip("The root Transform of the character, with the rigidbody and the collider.")]
    public Transform characterRoot;
    [Tooltip("The pelvis transform. Common ancestor of both legs and the spine.")]
    public Transform pelvis;
    [Tooltip("The last bone in the spine that is the common parent for both forelegs.")]
    public Transform lastSpineBone;
    [Tooltip("The head (optional, if you intend to maintain it's rotation).")]
    public Transform head;
    public IK[] legs;
    public IK[] forelegs;
    [HideInInspector]
    public Vector3 gravity = Vector3.down;
    private GrounderQuadruped.Foot[] feet = new GrounderQuadruped.Foot[0];
    private Vector3 animatedPelvisLocalPosition;
    private Quaternion animatedPelvisLocalRotation;
    private Quaternion animatedHeadLocalRotation;
    private Vector3 solvedPelvisLocalPosition;
    private Quaternion solvedPelvisLocalRotation;
    private Quaternion solvedHeadLocalRotation;
    private int solvedFeet;
    private bool solved;
    private float angle;
    private Transform forefeetRoot;
    private Quaternion headRotation;
    private float lastWeight;

    [ContextMenu("User Manual")]
    protected override void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page11.html");
    }

    [ContextMenu("Scrpt Reference")]
    protected override void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_grounder_quadruped.html");
    }

    public override void ResetPosition()
    {
      this.solver.Reset();
      this.forelegSolver.Reset();
    }

    private bool IsReadyToInitiate()
    {
      return !((Object) this.pelvis == (Object) null) && !((Object) this.lastSpineBone == (Object) null) && this.legs.Length != 0 && this.forelegs.Length != 0 && !((Object) this.characterRoot == (Object) null) && this.IsReadyToInitiateLegs(this.legs) && this.IsReadyToInitiateLegs(this.forelegs);
    }

    private bool IsReadyToInitiateLegs(IK[] ikComponents)
    {
      foreach (IK ikComponent in ikComponents)
      {
        if ((Object) ikComponent == (Object) null)
          return false;
        switch (ikComponent)
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
      for (int index = 0; index < this.feet.Length; ++index)
      {
        if (this.feet[index].solver != null)
          this.feet[index].solver.IKPositionWeight = 0.0f;
      }
    }

    private void Update()
    {
      this.weight = Mathf.Clamp(this.weight, 0.0f, 1f);
      if ((double) this.weight <= 0.0)
        return;
      this.solved = false;
      if (this.initiated || !this.IsReadyToInitiate())
        return;
      this.Initiate();
    }

    private void Initiate()
    {
      this.feet = new GrounderQuadruped.Foot[this.legs.Length + this.forelegs.Length];
      Transform[] feet1 = this.InitiateFeet(this.legs, ref this.feet, 0);
      Transform[] feet2 = this.InitiateFeet(this.forelegs, ref this.feet, this.legs.Length);
      this.animatedPelvisLocalPosition = this.pelvis.localPosition;
      this.animatedPelvisLocalRotation = this.pelvis.localRotation;
      if ((Object) this.head != (Object) null)
        this.animatedHeadLocalRotation = this.head.localRotation;
      this.forefeetRoot = new GameObject().transform;
      this.forefeetRoot.parent = this.transform;
      this.forefeetRoot.name = "Forefeet Root";
      this.solver.Initiate(this.transform, feet1);
      this.forelegSolver.Initiate(this.forefeetRoot, feet2);
      for (int index = 0; index < feet1.Length; ++index)
        this.feet[index].leg = this.solver.legs[index];
      for (int index = 0; index < feet2.Length; ++index)
        this.feet[index + this.legs.Length].leg = this.forelegSolver.legs[index];
      this.initiated = true;
    }

    private Transform[] InitiateFeet(
      IK[] ikComponents,
      ref GrounderQuadruped.Foot[] f,
      int indexOffset)
    {
      Transform[] transformArray = new Transform[ikComponents.Length];
      for (int index = 0; index < ikComponents.Length; ++index)
      {
        IKSolver.Point[] points = ikComponents[index].GetIKSolver().GetPoints();
        f[index + indexOffset] = new GrounderQuadruped.Foot(ikComponents[index].GetIKSolver(), points[points.Length - 1].transform);
        transformArray[index] = f[index + indexOffset].transform;
        f[index + indexOffset].solver.OnPreUpdate += new IKSolver.UpdateDelegate(this.OnSolverUpdate);
        f[index + indexOffset].solver.OnPostUpdate += new IKSolver.UpdateDelegate(this.OnPostSolverUpdate);
      }
      return transformArray;
    }

    private void LateUpdate()
    {
      if ((double) this.weight <= 0.0)
        return;
      this.rootRotationWeight = Mathf.Clamp(this.rootRotationWeight, 0.0f, 1f);
      this.minRootRotation = Mathf.Clamp(this.minRootRotation, -90f, this.maxRootRotation);
      this.maxRootRotation = Mathf.Clamp(this.maxRootRotation, this.minRootRotation, 90f);
      this.rootRotationSpeed = Mathf.Clamp(this.rootRotationSpeed, 0.0f, this.rootRotationSpeed);
      this.maxLegOffset = Mathf.Clamp(this.maxLegOffset, 0.0f, this.maxLegOffset);
      this.maxForeLegOffset = Mathf.Clamp(this.maxForeLegOffset, 0.0f, this.maxForeLegOffset);
      this.maintainHeadRotationWeight = Mathf.Clamp(this.maintainHeadRotationWeight, 0.0f, 1f);
      this.RootRotation();
    }

    private void RootRotation()
    {
      if ((double) this.rootRotationWeight <= 0.0 || (double) this.rootRotationSpeed <= 0.0)
        return;
      this.solver.rotateSolver = true;
      this.forelegSolver.rotateSolver = true;
      Vector3 forward = this.characterRoot.forward;
      Vector3 normal = -this.gravity;
      Vector3.OrthoNormalize(ref normal, ref forward);
      Quaternion rotation = Quaternion.LookRotation(forward, -this.gravity);
      Vector3 vector3_1 = this.forelegSolver.rootHit.point - this.solver.rootHit.point;
      Vector3 vector3_2 = Quaternion.Inverse(rotation) * vector3_1;
      this.angle = Mathf.Lerp(this.angle, Mathf.Clamp(Mathf.Atan2(vector3_2.y, vector3_2.z) * 57.29578f * this.rootRotationWeight, this.minRootRotation, this.maxRootRotation), Time.deltaTime * this.rootRotationSpeed);
      this.characterRoot.rotation = Quaternion.Slerp(this.characterRoot.rotation, Quaternion.AngleAxis(-this.angle, this.characterRoot.right) * rotation, this.weight);
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
      if (this.pelvis.localRotation != this.solvedPelvisLocalRotation)
        this.animatedPelvisLocalRotation = this.pelvis.localRotation;
      else
        this.pelvis.localRotation = this.animatedPelvisLocalRotation;
      if ((Object) this.head != (Object) null)
      {
        if (this.head.localRotation != this.solvedHeadLocalRotation)
          this.animatedHeadLocalRotation = this.head.localRotation;
        else
          this.head.localRotation = this.animatedHeadLocalRotation;
      }
      for (int index = 0; index < this.feet.Length; ++index)
        this.feet[index].rotation = this.feet[index].transform.rotation;
      if ((Object) this.head != (Object) null)
        this.headRotation = this.head.rotation;
      this.UpdateForefeetRoot();
      this.solver.Update();
      this.forelegSolver.Update();
      this.pelvis.position += this.solver.pelvis.IKOffset * this.weight;
      this.pelvis.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation(this.lastSpineBone.position - this.pelvis.position, this.lastSpineBone.position + this.forelegSolver.root.up * Mathf.Clamp(this.forelegSolver.pelvis.heightOffset, float.NegativeInfinity, 0.0f) - this.solver.root.up * this.solver.pelvis.heightOffset - this.pelvis.position), this.weight) * this.pelvis.rotation;
      for (int index = 0; index < this.feet.Length; ++index)
        this.SetFootIK(this.feet[index], index < 2 ? this.maxLegOffset : this.maxForeLegOffset);
      this.solved = true;
      this.solvedFeet = 0;
      if (this.OnPostGrounder == null)
        return;
      this.OnPostGrounder();
    }

    private void UpdateForefeetRoot()
    {
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < this.forelegSolver.legs.Length; ++index)
        zero += this.forelegSolver.legs[index].transform.position;
      Vector3 vector3 = zero / (float) this.forelegs.Length - this.transform.position;
      Vector3 up = this.transform.up;
      Vector3 tangent = vector3;
      Vector3.OrthoNormalize(ref up, ref tangent);
      this.forefeetRoot.position = this.transform.position + tangent.normalized * vector3.magnitude;
    }

    private void SetFootIK(GrounderQuadruped.Foot foot, float maxOffset)
    {
      Vector3 vector = foot.leg.IKPosition - foot.transform.position;
      foot.solver.IKPosition = foot.transform.position + Vector3.ClampMagnitude(vector, maxOffset);
      foot.solver.IKPositionWeight = this.weight;
    }

    private void OnPostSolverUpdate()
    {
      if ((double) this.weight <= 0.0 || !this.enabled)
        return;
      ++this.solvedFeet;
      if (this.solvedFeet < this.feet.Length)
        return;
      for (int index = 0; index < this.feet.Length; ++index)
        this.feet[index].transform.rotation = Quaternion.Slerp(Quaternion.identity, this.feet[index].leg.rotationOffset, this.weight) * this.feet[index].rotation;
      if ((Object) this.head != (Object) null)
        this.head.rotation = Quaternion.Lerp(this.head.rotation, this.headRotation, this.maintainHeadRotationWeight * this.weight);
      this.solvedPelvisLocalPosition = this.pelvis.localPosition;
      this.solvedPelvisLocalRotation = this.pelvis.localRotation;
      if (!((Object) this.head != (Object) null))
        return;
      this.solvedHeadLocalRotation = this.head.localRotation;
    }

    private void OnDestroy()
    {
      if (!this.initiated)
        return;
      this.DestroyLegs(this.legs);
      this.DestroyLegs(this.forelegs);
    }

    private void DestroyLegs(IK[] ikComponents)
    {
      foreach (IK ikComponent in ikComponents)
      {
        if ((Object) ikComponent != (Object) null)
        {
          ikComponent.GetIKSolver().OnPreUpdate -= new IKSolver.UpdateDelegate(this.OnSolverUpdate);
          ikComponent.GetIKSolver().OnPostUpdate -= new IKSolver.UpdateDelegate(this.OnPostSolverUpdate);
        }
      }
    }

    public struct Foot
    {
      public IKSolver solver;
      public Transform transform;
      public Quaternion rotation;
      public Grounding.Leg leg;

      public Foot(IKSolver solver, Transform transform)
      {
        this.solver = solver;
        this.transform = transform;
        this.leg = (Grounding.Leg) null;
        this.rotation = transform.rotation;
      }
    }
  }
}
