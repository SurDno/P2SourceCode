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
    private Transform[] feet = [];
    private Quaternion[] footRotations = [];
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

    public override void ResetPosition() => solver.Reset();

    private bool IsReadyToInitiate()
    {
      if (pelvis == null || legs.Length == 0)
        return false;
      foreach (IK leg in legs)
      {
        if (leg == null)
          return false;
        switch (leg)
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
      for (int index = 0; index < legs.Length; ++index)
      {
        if (legs[index] != null)
          legs[index].GetIKSolver().IKPositionWeight = 0.0f;
      }
    }

    private void Update()
    {
      weight = Mathf.Clamp(weight, 0.0f, 1f);
      if (weight <= 0.0)
        return;
      solved = false;
      if (initiated)
      {
        rootRotationWeight = Mathf.Clamp(rootRotationWeight, 0.0f, 1f);
        rootRotationSpeed = Mathf.Clamp(rootRotationSpeed, 0.0f, rootRotationSpeed);
        if (!(characterRoot != null) || rootRotationSpeed <= 0.0 || rootRotationWeight <= 0.0)
          return;
        Vector3 vector3 = solver.GetLegsPlaneNormal();
        if (rootRotationWeight < 1.0)
          vector3 = Vector3.Slerp(Vector3.up, vector3, rootRotationWeight);
        characterRoot.rotation = Quaternion.Lerp(characterRoot.rotation, Quaternion.RotateTowards(Quaternion.FromToRotation(transform.up, Vector3.up) * characterRoot.rotation, Quaternion.FromToRotation(transform.up, vector3) * characterRoot.rotation, maxRootRotationAngle), Time.deltaTime * rootRotationSpeed);
      }
      else
      {
        if (!IsReadyToInitiate())
          return;
        Initiate();
      }
    }

    private void Initiate()
    {
      feet = new Transform[legs.Length];
      footRotations = new Quaternion[legs.Length];
      for (int index = 0; index < feet.Length; ++index)
        footRotations[index] = Quaternion.identity;
      for (int index = 0; index < legs.Length; ++index)
      {
        IKSolver.Point[] points = legs[index].GetIKSolver().GetPoints();
        feet[index] = points[points.Length - 1].transform;
        legs[index].GetIKSolver().OnPreUpdate += OnSolverUpdate;
        legs[index].GetIKSolver().OnPostUpdate += OnPostSolverUpdate;
      }
      animatedPelvisLocalPosition = pelvis.localPosition;
      solver.Initiate(transform, feet);
      for (int index = 0; index < legs.Length; ++index)
      {
        if (legs[index] is LegIK)
          solver.legs[index].invertFootCenter = true;
      }
      initiated = true;
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
      solver.Update();
      for (int index = 0; index < legs.Length; ++index)
        SetLegIK(index);
      pelvis.position += solver.pelvis.IKOffset * weight;
      solved = true;
      solvedFeet = 0;
      if (OnPostGrounder == null)
        return;
      OnPostGrounder();
    }

    private void SetLegIK(int index)
    {
      footRotations[index] = feet[index].rotation;
      if (legs[index] is LegIK)
      {
        (legs[index].GetIKSolver() as IKSolverLeg).IKRotation = Quaternion.Slerp(Quaternion.identity, solver.legs[index].rotationOffset, weight) * footRotations[index];
        (legs[index].GetIKSolver() as IKSolverLeg).IKRotationWeight = 1f;
      }
      legs[index].GetIKSolver().IKPosition = solver.legs[index].IKPosition;
      legs[index].GetIKSolver().IKPositionWeight = weight;
    }

    private void OnPostSolverUpdate()
    {
      if (weight <= 0.0 || !enabled)
        return;
      ++solvedFeet;
      if (solvedFeet < feet.Length)
        return;
      for (int index = 0; index < feet.Length; ++index)
        feet[index].rotation = Quaternion.Slerp(Quaternion.identity, solver.legs[index].rotationOffset, weight) * footRotations[index];
      solvedPelvisLocalPosition = pelvis.localPosition;
    }

    private void OnDestroy()
    {
      if (!initiated)
        return;
      foreach (IK leg in legs)
      {
        if (leg != null)
        {
          leg.GetIKSolver().OnPreUpdate -= OnSolverUpdate;
          leg.GetIKSolver().OnPostUpdate -= OnPostSolverUpdate;
        }
      }
    }
  }
}
