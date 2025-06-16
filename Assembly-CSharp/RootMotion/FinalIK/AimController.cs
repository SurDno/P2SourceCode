// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.AimController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  public class AimController : MonoBehaviour
  {
    [Tooltip("Reference to the AimIK component.")]
    public AimIK ik;
    [Tooltip("Master weight of the IK solver.")]
    [Range(0.0f, 1f)]
    public float weight = 1f;
    [Header("Target Smoothing")]
    [Tooltip("The target to aim at. Do not use the Target transform that is assigned to AimIK. Set to null if you wish to stop aiming.")]
    public Transform target;
    [Tooltip("The time it takes to switch targets.")]
    public float targetSwitchSmoothTime = 0.3f;
    [Tooltip("The time it takes to blend in/out of AimIK weight.")]
    public float weightSmoothTime = 0.3f;
    [Header("Turning Towards The Target")]
    [Tooltip("Enables smooth turning towards the target according to the parameters under this header.")]
    public bool smoothTurnTowardsTarget = true;
    [Tooltip("Speed of turning towards the target using Vector3.RotateTowards.")]
    public float maxRadiansDelta = 3f;
    [Tooltip("Speed of moving towards the target using Vector3.RotateTowards.")]
    public float maxMagnitudeDelta = 3f;
    [Tooltip("Speed of slerping towards the target.")]
    public float slerpSpeed = 3f;
    [Tooltip("The position of the pivot that the aim target is rotated around relative to the root of the character.")]
    public Vector3 pivotOffsetFromRoot = Vector3.up;
    [Tooltip("Minimum distance of aiming from the first bone. Keeps the solver from failing if the target is too close.")]
    public float minDistance = 1f;
    [Tooltip("Offset applied to the target in world space. Convenient for scripting aiming inaccuracy.")]
    public Vector3 offset;
    [Header("RootRotation")]
    [Tooltip("Character root will be rotate around the Y axis to keep root forward within this angle from the aiming direction.")]
    [Range(0.0f, 180f)]
    public float maxRootAngle = 45f;
    [Header("Mode")]
    [Tooltip("If true, AimIK will consider whatever the current direction of the weapon to be the forward aiming direction and work additively on top of that. This enables you to use recoil and reloading animations seamlessly with AimIK. Adjust the Vector3 value below if the weapon is not aiming perfectly forward in the aiming animation clip.")]
    public bool useAnimatedAimDirection;
    [Tooltip("The direction of the animated weapon aiming in character space. Tweak this value to adjust the aiming. 'Use Animated Aim Direction' must be enabled for this property to work.")]
    public Vector3 animatedAimDirection = Vector3.forward;
    private Transform lastTarget;
    private float switchWeight;
    private float switchWeightV;
    private float weightV;
    private Vector3 lastPosition;
    private Vector3 dir;
    private bool lastSmoothTowardsTarget;

    private void Start()
    {
      this.lastPosition = this.ik.solver.IKPosition;
      this.dir = this.ik.solver.IKPosition - this.pivot;
      this.ik.solver.target = (Transform) null;
    }

    private void LateUpdate()
    {
      if ((Object) this.target != (Object) this.lastTarget)
      {
        if ((Object) this.lastTarget == (Object) null && (Object) this.target != (Object) null)
        {
          this.lastPosition = this.target.position;
          this.dir = this.target.position - this.pivot;
          this.ik.solver.IKPosition = this.target.position + this.offset;
        }
        else
        {
          this.lastPosition = this.ik.solver.IKPosition;
          this.dir = this.ik.solver.IKPosition - this.pivot;
        }
        this.switchWeight = 0.0f;
        this.lastTarget = this.target;
      }
      this.ik.solver.IKPositionWeight = Mathf.SmoothDamp(this.ik.solver.IKPositionWeight, (Object) this.target != (Object) null ? this.weight : 0.0f, ref this.weightV, this.weightSmoothTime);
      if ((double) this.ik.solver.IKPositionWeight >= 0.99900001287460327)
        this.ik.solver.IKPositionWeight = 1f;
      if ((double) this.ik.solver.IKPositionWeight <= 1.0 / 1000.0)
        this.ik.solver.IKPositionWeight = 0.0f;
      if ((double) this.ik.solver.IKPositionWeight <= 0.0)
        return;
      this.switchWeight = Mathf.SmoothDamp(this.switchWeight, 1f, ref this.switchWeightV, this.targetSwitchSmoothTime);
      if ((double) this.switchWeight >= 0.99900001287460327)
        this.switchWeight = 1f;
      if ((Object) this.target != (Object) null)
        this.ik.solver.IKPosition = Vector3.Lerp(this.lastPosition, this.target.position + this.offset, this.switchWeight);
      if (this.smoothTurnTowardsTarget != this.lastSmoothTowardsTarget)
      {
        this.dir = this.ik.solver.IKPosition - this.pivot;
        this.lastSmoothTowardsTarget = this.smoothTurnTowardsTarget;
      }
      if (this.smoothTurnTowardsTarget)
      {
        Vector3 vector3 = this.ik.solver.IKPosition - this.pivot;
        this.dir = Vector3.Slerp(this.dir, vector3, Time.deltaTime * this.slerpSpeed);
        this.dir = Vector3.RotateTowards(this.dir, vector3, Time.deltaTime * this.maxRadiansDelta, this.maxMagnitudeDelta);
        this.ik.solver.IKPosition = this.pivot + this.dir;
      }
      this.ApplyMinDistance();
      this.RootRotation();
      if (!this.useAnimatedAimDirection)
        return;
      this.ik.solver.axis = this.ik.solver.transform.InverseTransformVector(this.ik.transform.rotation * this.animatedAimDirection);
    }

    private Vector3 pivot
    {
      get => this.ik.transform.position + this.ik.transform.rotation * this.pivotOffsetFromRoot;
    }

    private void ApplyMinDistance()
    {
      Vector3 pivot = this.pivot;
      Vector3 vector3 = this.ik.solver.IKPosition - pivot;
      vector3 = vector3.normalized * Mathf.Max(vector3.magnitude, this.minDistance);
      this.ik.solver.IKPosition = pivot + vector3;
    }

    private void RootRotation()
    {
      float num1 = Mathf.Lerp(180f, this.maxRootAngle, this.ik.solver.IKPositionWeight);
      if ((double) num1 >= 180.0)
        return;
      Vector3 vector3 = Quaternion.Inverse(this.ik.transform.rotation) * (this.ik.solver.IKPosition - this.pivot);
      float num2 = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
      float angle = 0.0f;
      if ((double) num2 > (double) num1)
        angle = num2 - num1;
      if ((double) num2 < -(double) num1)
        angle = num2 + num1;
      this.ik.transform.rotation = Quaternion.AngleAxis(angle, this.ik.transform.up) * this.ik.transform.rotation;
    }
  }
}
