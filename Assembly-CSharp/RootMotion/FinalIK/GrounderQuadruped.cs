using UnityEngine;

namespace RootMotion.FinalIK
{
  [HelpURL("http://www.root-motion.com/finalikdox/html/page11.html")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Grounder/Grounder Quadruped")]
  public class GrounderQuadruped : Grounder
  {
    [Tooltip("The Grounding solver for the forelegs.")]
    public Grounding forelegSolver = new();
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
    private Foot[] feet = [];
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
      solver.Reset();
      forelegSolver.Reset();
    }

    private bool IsReadyToInitiate()
    {
      return !(pelvis == null) && !(lastSpineBone == null) && legs.Length != 0 && forelegs.Length != 0 && !(characterRoot == null) && IsReadyToInitiateLegs(legs) && IsReadyToInitiateLegs(forelegs);
    }

    private bool IsReadyToInitiateLegs(IK[] ikComponents)
    {
      foreach (IK ikComponent in ikComponents)
      {
        if (ikComponent == null)
          return false;
        switch (ikComponent)
        {
          case FullBodyBipedIK _:
            LogWarning("GrounderIK does not support FullBodyBipedIK, use CCDIK, FABRIK, LimbIK or TrigonometricIK instead. If you want to use FullBodyBipedIK, use the GrounderFBBIK component.");
            return false;
          case FABRIKRoot _:
            LogWarning("GrounderIK does not support FABRIKRoot, use CCDIK, FABRIK, LimbIK or TrigonometricIK instead.");
            return false;
          case AimIK _:
            LogWarning("GrounderIK does not support AimIK, use CCDIK, FABRIK, LimbIK or TrigonometricIK instead.");
            return false;
          default:
            continue;
        }
      }
      return true;
    }

    private void OnDisable()
    {
      if (!initiated)
        return;
      for (int index = 0; index < feet.Length; ++index)
      {
        if (feet[index].solver != null)
          feet[index].solver.IKPositionWeight = 0.0f;
      }
    }

    private void Update()
    {
      weight = Mathf.Clamp(weight, 0.0f, 1f);
      if (weight <= 0.0)
        return;
      solved = false;
      if (initiated || !IsReadyToInitiate())
        return;
      Initiate();
    }

    private void Initiate()
    {
      feet = new Foot[legs.Length + forelegs.Length];
      Transform[] feet1 = InitiateFeet(legs, ref feet, 0);
      Transform[] feet2 = InitiateFeet(forelegs, ref feet, legs.Length);
      animatedPelvisLocalPosition = pelvis.localPosition;
      animatedPelvisLocalRotation = pelvis.localRotation;
      if (head != null)
        animatedHeadLocalRotation = head.localRotation;
      forefeetRoot = new GameObject().transform;
      forefeetRoot.parent = transform;
      forefeetRoot.name = "Forefeet Root";
      solver.Initiate(transform, feet1);
      forelegSolver.Initiate(forefeetRoot, feet2);
      for (int index = 0; index < feet1.Length; ++index)
        feet[index].leg = solver.legs[index];
      for (int index = 0; index < feet2.Length; ++index)
        feet[index + legs.Length].leg = forelegSolver.legs[index];
      initiated = true;
    }

    private Transform[] InitiateFeet(
      IK[] ikComponents,
      ref Foot[] f,
      int indexOffset)
    {
      Transform[] transformArray = new Transform[ikComponents.Length];
      for (int index = 0; index < ikComponents.Length; ++index)
      {
        IKSolver.Point[] points = ikComponents[index].GetIKSolver().GetPoints();
        f[index + indexOffset] = new Foot(ikComponents[index].GetIKSolver(), points[points.Length - 1].transform);
        transformArray[index] = f[index + indexOffset].transform;
        f[index + indexOffset].solver.OnPreUpdate += OnSolverUpdate;
        f[index + indexOffset].solver.OnPostUpdate += OnPostSolverUpdate;
      }
      return transformArray;
    }

    private void LateUpdate()
    {
      if (weight <= 0.0)
        return;
      rootRotationWeight = Mathf.Clamp(rootRotationWeight, 0.0f, 1f);
      minRootRotation = Mathf.Clamp(minRootRotation, -90f, maxRootRotation);
      maxRootRotation = Mathf.Clamp(maxRootRotation, minRootRotation, 90f);
      rootRotationSpeed = Mathf.Clamp(rootRotationSpeed, 0.0f, rootRotationSpeed);
      maxLegOffset = Mathf.Clamp(maxLegOffset, 0.0f, maxLegOffset);
      maxForeLegOffset = Mathf.Clamp(maxForeLegOffset, 0.0f, maxForeLegOffset);
      maintainHeadRotationWeight = Mathf.Clamp(maintainHeadRotationWeight, 0.0f, 1f);
      RootRotation();
    }

    private void RootRotation()
    {
      if (rootRotationWeight <= 0.0 || rootRotationSpeed <= 0.0)
        return;
      solver.rotateSolver = true;
      forelegSolver.rotateSolver = true;
      Vector3 forward = characterRoot.forward;
      Vector3 normal = -gravity;
      Vector3.OrthoNormalize(ref normal, ref forward);
      Quaternion rotation = Quaternion.LookRotation(forward, -gravity);
      Vector3 vector3_1 = forelegSolver.rootHit.point - solver.rootHit.point;
      Vector3 vector3_2 = Quaternion.Inverse(rotation) * vector3_1;
      angle = Mathf.Lerp(angle, Mathf.Clamp(Mathf.Atan2(vector3_2.y, vector3_2.z) * 57.29578f * rootRotationWeight, minRootRotation, maxRootRotation), Time.deltaTime * rootRotationSpeed);
      characterRoot.rotation = Quaternion.Slerp(characterRoot.rotation, Quaternion.AngleAxis(-angle, characterRoot.right) * rotation, weight);
    }

    private void OnSolverUpdate()
    {
      if (!enabled)
        return;
      if (weight <= 0.0)
      {
        if (lastWeight <= 0.0)
          return;
        OnDisable();
      }
      lastWeight = weight;
      if (solved)
        return;
      if (OnPreGrounder != null)
        OnPreGrounder();
      if (pelvis.localPosition != solvedPelvisLocalPosition)
        animatedPelvisLocalPosition = pelvis.localPosition;
      else
        pelvis.localPosition = animatedPelvisLocalPosition;
      if (pelvis.localRotation != solvedPelvisLocalRotation)
        animatedPelvisLocalRotation = pelvis.localRotation;
      else
        pelvis.localRotation = animatedPelvisLocalRotation;
      if (head != null)
      {
        if (head.localRotation != solvedHeadLocalRotation)
          animatedHeadLocalRotation = head.localRotation;
        else
          head.localRotation = animatedHeadLocalRotation;
      }
      for (int index = 0; index < feet.Length; ++index)
        feet[index].rotation = feet[index].transform.rotation;
      if (head != null)
        headRotation = head.rotation;
      UpdateForefeetRoot();
      solver.Update();
      forelegSolver.Update();
      pelvis.position += solver.pelvis.IKOffset * weight;
      pelvis.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation(lastSpineBone.position - pelvis.position, lastSpineBone.position + forelegSolver.root.up * Mathf.Clamp(forelegSolver.pelvis.heightOffset, float.NegativeInfinity, 0.0f) - solver.root.up * solver.pelvis.heightOffset - pelvis.position), weight) * pelvis.rotation;
      for (int index = 0; index < feet.Length; ++index)
        SetFootIK(feet[index], index < 2 ? maxLegOffset : maxForeLegOffset);
      solved = true;
      solvedFeet = 0;
      if (OnPostGrounder == null)
        return;
      OnPostGrounder();
    }

    private void UpdateForefeetRoot()
    {
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < forelegSolver.legs.Length; ++index)
        zero += forelegSolver.legs[index].transform.position;
      Vector3 vector3 = zero / forelegs.Length - transform.position;
      Vector3 up = transform.up;
      Vector3 tangent = vector3;
      Vector3.OrthoNormalize(ref up, ref tangent);
      forefeetRoot.position = transform.position + tangent.normalized * vector3.magnitude;
    }

    private void SetFootIK(Foot foot, float maxOffset)
    {
      Vector3 vector = foot.leg.IKPosition - foot.transform.position;
      foot.solver.IKPosition = foot.transform.position + Vector3.ClampMagnitude(vector, maxOffset);
      foot.solver.IKPositionWeight = weight;
    }

    private void OnPostSolverUpdate()
    {
      if (weight <= 0.0 || !enabled)
        return;
      ++solvedFeet;
      if (solvedFeet < feet.Length)
        return;
      for (int index = 0; index < feet.Length; ++index)
        feet[index].transform.rotation = Quaternion.Slerp(Quaternion.identity, feet[index].leg.rotationOffset, weight) * feet[index].rotation;
      if (head != null)
        head.rotation = Quaternion.Lerp(head.rotation, headRotation, maintainHeadRotationWeight * weight);
      solvedPelvisLocalPosition = pelvis.localPosition;
      solvedPelvisLocalRotation = pelvis.localRotation;
      if (!(head != null))
        return;
      solvedHeadLocalRotation = head.localRotation;
    }

    private void OnDestroy()
    {
      if (!initiated)
        return;
      DestroyLegs(legs);
      DestroyLegs(forelegs);
    }

    private void DestroyLegs(IK[] ikComponents)
    {
      foreach (IK ikComponent in ikComponents)
      {
        if (ikComponent != null)
        {
          ikComponent.GetIKSolver().OnPreUpdate -= OnSolverUpdate;
          ikComponent.GetIKSolver().OnPostUpdate -= OnPostSolverUpdate;
        }
      }
    }

    public struct Foot(IKSolver solver, Transform transform) {
      public IKSolver solver = solver;
      public Transform transform = transform;
      public Quaternion rotation = transform.rotation;
      public Grounding.Leg leg = null;
    }
  }
}
