using Cinemachine.Utility;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine
{
  [DocumentationSorting(6f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachineOrbitalTransposer : CinemachineTransposer
  {
    [Space]
    [Tooltip("The definition of Forward.  Camera will follow behind.")]
    public CinemachineOrbitalTransposer.Heading m_Heading = new CinemachineOrbitalTransposer.Heading(CinemachineOrbitalTransposer.Heading.HeadingDefinition.TargetForward, 4, 0.0f);
    [Tooltip("Automatic heading recentering.  The settings here defines how the camera will reposition itself in the absence of player input.")]
    public CinemachineOrbitalTransposer.Recentering m_RecenterToTargetHeading = new CinemachineOrbitalTransposer.Recentering(true, 1f, 2f);
    [Tooltip("Heading Control.  The settings here control the behaviour of the camera in response to the player's input.")]
    public AxisState m_XAxis = new AxisState(300f, 2f, 1f, 0.0f, "Mouse X", true);
    [SerializeField]
    [HideInInspector]
    [FormerlySerializedAs("m_Radius")]
    private float m_LegacyRadius = float.MaxValue;
    [SerializeField]
    [HideInInspector]
    [FormerlySerializedAs("m_HeightOffset")]
    private float m_LegacyHeightOffset = float.MaxValue;
    [SerializeField]
    [HideInInspector]
    [FormerlySerializedAs("m_HeadingBias")]
    private float m_LegacyHeadingBias = float.MaxValue;
    [HideInInspector]
    [NoSaveDuringPlay]
    public bool m_HeadingIsSlave = false;
    internal CinemachineOrbitalTransposer.UpdateHeadingDelegate HeadingUpdater = (CinemachineOrbitalTransposer.UpdateHeadingDelegate) ((orbital, deltaTime, up) => orbital.UpdateHeading(deltaTime, up, ref orbital.m_XAxis));
    private float mLastHeadingAxisInputTime = 0.0f;
    private float mHeadingRecenteringVelocity = 0.0f;
    private Vector3 mLastTargetPosition = Vector3.zero;
    private CinemachineOrbitalTransposer.HeadingTracker mHeadingTracker;
    private Rigidbody mTargetRigidBody = (Rigidbody) null;
    private Quaternion mHeadingPrevFrame = Quaternion.identity;
    private Vector3 mOffsetPrevFrame = Vector3.zero;

    protected override void OnValidate()
    {
      if ((double) this.m_LegacyRadius != 3.4028234663852886E+38 && (double) this.m_LegacyHeightOffset != 3.4028234663852886E+38 && (double) this.m_LegacyHeadingBias != 3.4028234663852886E+38)
      {
        this.m_FollowOffset = new Vector3(0.0f, this.m_LegacyHeightOffset, -this.m_LegacyRadius);
        this.m_LegacyHeightOffset = this.m_LegacyRadius = float.MaxValue;
        this.m_Heading.m_HeadingBias = this.m_LegacyHeadingBias;
        this.m_XAxis.m_MaxSpeed /= 10f;
        this.m_XAxis.m_AccelTime /= 10f;
        this.m_XAxis.m_DecelTime /= 10f;
        this.m_LegacyHeadingBias = float.MaxValue;
        this.m_RecenterToTargetHeading.LegacyUpgrade(ref this.m_Heading.m_HeadingDefinition, ref this.m_Heading.m_VelocityFilterStrength);
      }
      this.m_XAxis.Validate();
      this.m_RecenterToTargetHeading.Validate();
      base.OnValidate();
    }

    public float UpdateHeading(float deltaTime, Vector3 up, ref AxisState axis)
    {
      if (((double) deltaTime >= 0.0 || CinemachineCore.Instance.IsLive((ICinemachineCamera) this.VirtualCamera)) && false | axis.Update(deltaTime))
      {
        this.mLastHeadingAxisInputTime = Time.time;
        this.mHeadingRecenteringVelocity = 0.0f;
      }
      float targetHeading = this.GetTargetHeading(axis.Value, this.GetReferenceOrientation(up), deltaTime);
      if ((double) deltaTime < 0.0)
      {
        this.mHeadingRecenteringVelocity = 0.0f;
        if (this.m_RecenterToTargetHeading.m_enabled)
          axis.Value = targetHeading;
      }
      else if (this.m_BindingMode != CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp && this.m_RecenterToTargetHeading.m_enabled && (double) Time.time > (double) this.mLastHeadingAxisInputTime + (double) this.m_RecenterToTargetHeading.m_RecenterWaitTime)
      {
        float num1 = this.m_RecenterToTargetHeading.m_RecenteringTime / 3f;
        if ((double) num1 <= (double) deltaTime)
        {
          axis.Value = targetHeading;
        }
        else
        {
          float f = Mathf.DeltaAngle(axis.Value, targetHeading);
          float a = Mathf.Abs(f);
          if ((double) a < 9.9999997473787516E-05)
          {
            axis.Value = targetHeading;
            this.mHeadingRecenteringVelocity = 0.0f;
          }
          else
          {
            float num2 = deltaTime / num1;
            float num3 = Mathf.Sign(f) * Mathf.Min(a, a * num2);
            float num4 = num3 - this.mHeadingRecenteringVelocity;
            if ((double) num3 < 0.0 && (double) num4 < 0.0 || (double) num3 > 0.0 && (double) num4 > 0.0)
              num3 = this.mHeadingRecenteringVelocity + num3 * num2;
            axis.Value += num3;
            this.mHeadingRecenteringVelocity = num3;
          }
        }
      }
      float num = axis.Value;
      if (this.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
        axis.Value = 0.0f;
      return num;
    }

    private void OnEnable()
    {
      this.m_XAxis.SetThresholds(0.0f, 360f, true);
      this.PreviousTarget = (Transform) null;
      this.mLastTargetPosition = Vector3.zero;
    }

    private Transform PreviousTarget { get; set; }

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      this.InitPrevFrameStateInfo(ref curState, deltaTime);
      if ((UnityEngine.Object) this.FollowTarget != (UnityEngine.Object) this.PreviousTarget)
      {
        this.PreviousTarget = this.FollowTarget;
        this.mTargetRigidBody = (UnityEngine.Object) this.PreviousTarget == (UnityEngine.Object) null ? (Rigidbody) null : this.PreviousTarget.GetComponent<Rigidbody>();
        this.mLastTargetPosition = (UnityEngine.Object) this.PreviousTarget == (UnityEngine.Object) null ? Vector3.zero : this.PreviousTarget.position;
        this.mHeadingTracker = (CinemachineOrbitalTransposer.HeadingTracker) null;
      }
      float angle = this.HeadingUpdater(this, deltaTime, curState.ReferenceUp);
      if (!this.IsValid)
        return;
      this.mLastTargetPosition = this.FollowTarget.position;
      if (this.m_BindingMode != CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
        angle += this.m_Heading.m_HeadingBias;
      Quaternion quaternion1 = Quaternion.AngleAxis(angle, curState.ReferenceUp);
      Vector3 effectiveOffset = this.EffectiveOffset;
      Vector3 outTargetPosition;
      Quaternion outTargetOrient;
      this.TrackTarget(deltaTime, curState.ReferenceUp, quaternion1 * effectiveOffset, out outTargetPosition, out outTargetOrient);
      curState.ReferenceUp = outTargetOrient * Vector3.up;
      if ((double) deltaTime >= 0.0)
      {
        Vector3 vector3_1 = quaternion1 * effectiveOffset - this.mHeadingPrevFrame * this.mOffsetPrevFrame;
        Vector3 vector3_2 = outTargetOrient * vector3_1;
        curState.PositionDampingBypass = vector3_2;
      }
      Quaternion quaternion2 = outTargetOrient * quaternion1;
      curState.RawPosition = outTargetPosition + quaternion2 * effectiveOffset;
      this.mHeadingPrevFrame = this.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp ? Quaternion.identity : quaternion1;
      this.mOffsetPrevFrame = effectiveOffset;
    }

    public override void OnPositionDragged(Vector3 delta)
    {
      this.m_FollowOffset = this.m_FollowOffset + (Quaternion.Inverse(this.GetReferenceOrientation(this.VcamState.ReferenceUp)) * delta) with
      {
        x = 0.0f
      };
      this.m_FollowOffset = this.EffectiveOffset;
    }

    private static string GetFullName(GameObject current)
    {
      if ((UnityEngine.Object) current == (UnityEngine.Object) null)
        return "";
      return (UnityEngine.Object) current.transform.parent == (UnityEngine.Object) null ? "/" + current.name : CinemachineOrbitalTransposer.GetFullName(current.transform.parent.gameObject) + "/" + current.name;
    }

    private float GetTargetHeading(
      float currentHeading,
      Quaternion targetOrientation,
      float deltaTime)
    {
      if (this.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
        return 0.0f;
      if ((UnityEngine.Object) this.FollowTarget == (UnityEngine.Object) null)
        return currentHeading;
      if (this.m_Heading.m_HeadingDefinition == CinemachineOrbitalTransposer.Heading.HeadingDefinition.Velocity && (UnityEngine.Object) this.mTargetRigidBody == (UnityEngine.Object) null)
      {
        Debug.Log((object) string.Format("Attempted to use HeadingDerivationMode.Velocity to calculate heading for {0}. No RigidBody was present on '{1}'. Defaulting to position delta", (object) CinemachineOrbitalTransposer.GetFullName(this.VirtualCamera.VirtualCameraGameObject), (object) this.FollowTarget));
        this.m_Heading.m_HeadingDefinition = CinemachineOrbitalTransposer.Heading.HeadingDefinition.PositionDelta;
      }
      Vector3 zero = Vector3.zero;
      Vector3 vector;
      switch (this.m_Heading.m_HeadingDefinition)
      {
        case CinemachineOrbitalTransposer.Heading.HeadingDefinition.PositionDelta:
          vector = this.FollowTarget.position - this.mLastTargetPosition;
          break;
        case CinemachineOrbitalTransposer.Heading.HeadingDefinition.Velocity:
          vector = this.mTargetRigidBody.velocity;
          break;
        case CinemachineOrbitalTransposer.Heading.HeadingDefinition.TargetForward:
          vector = this.FollowTarget.forward;
          break;
        default:
          return 0.0f;
      }
      int filterSize = this.m_Heading.m_VelocityFilterStrength * 5;
      if (this.mHeadingTracker == null || this.mHeadingTracker.FilterSize != filterSize)
        this.mHeadingTracker = new CinemachineOrbitalTransposer.HeadingTracker(filterSize);
      this.mHeadingTracker.DecayHistory();
      Vector3 vector3_1 = targetOrientation * Vector3.up;
      Vector3 vector3_2 = vector.ProjectOntoPlane(vector3_1);
      if (!vector3_2.AlmostZero())
        this.mHeadingTracker.Add(vector3_2);
      Vector3 reliableHeading = this.mHeadingTracker.GetReliableHeading();
      return !reliableHeading.AlmostZero() ? UnityVectorExtensions.SignedAngle(targetOrientation * Vector3.forward, reliableHeading, vector3_1) : currentHeading;
    }

    [DocumentationSorting(6.2f, DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public struct Heading
    {
      [Tooltip("How 'forward' is defined.  The camera will be placed by default behind the target.  PositionDelta will consider 'forward' to be the direction in which the target is moving.")]
      public CinemachineOrbitalTransposer.Heading.HeadingDefinition m_HeadingDefinition;
      [Range(0.0f, 10f)]
      [Tooltip("Size of the velocity sampling window for target heading filter.  This filters out irregularities in the target's movement.  Used only if deriving heading from target's movement (PositionDelta or Velocity)")]
      public int m_VelocityFilterStrength;
      [Range(-180f, 180f)]
      [Tooltip("Where the camera is placed when the X-axis value is zero.  This is a rotation in degrees around the Y axis.  When this value is 0, the camera will be placed behind the target.  Nonzero offsets will rotate the zero position around the target.")]
      public float m_HeadingBias;

      public Heading(
        CinemachineOrbitalTransposer.Heading.HeadingDefinition def,
        int filterStrength,
        float bias)
      {
        this.m_HeadingDefinition = def;
        this.m_VelocityFilterStrength = filterStrength;
        this.m_HeadingBias = bias;
      }

      [DocumentationSorting(6.21f, DocumentationSortingAttribute.Level.UserRef)]
      public enum HeadingDefinition
      {
        PositionDelta,
        Velocity,
        TargetForward,
        WorldForward,
      }
    }

    [DocumentationSorting(6.5f, DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public struct Recentering
    {
      [Tooltip("If checked, will enable automatic recentering of the camera based on the heading definition. If unchecked, recenting is disabled.")]
      public bool m_enabled;
      [Tooltip("If no input has been detected, the camera will wait this long in seconds before moving its heading to the zero position.")]
      public float m_RecenterWaitTime;
      [Tooltip("Maximum angular speed of recentering.  Will accelerate into and decelerate out of this.")]
      public float m_RecenteringTime;
      [SerializeField]
      [HideInInspector]
      [FormerlySerializedAs("m_HeadingDefinition")]
      private int m_LegacyHeadingDefinition;
      [SerializeField]
      [HideInInspector]
      [FormerlySerializedAs("m_VelocityFilterStrength")]
      private int m_LegacyVelocityFilterStrength;

      public Recentering(bool enabled, float recenterWaitTime, float recenteringSpeed)
      {
        this.m_enabled = enabled;
        this.m_RecenterWaitTime = recenterWaitTime;
        this.m_RecenteringTime = recenteringSpeed;
        this.m_LegacyHeadingDefinition = this.m_LegacyVelocityFilterStrength = -1;
      }

      public void Validate()
      {
        this.m_RecenterWaitTime = Mathf.Max(0.0f, this.m_RecenterWaitTime);
        this.m_RecenteringTime = Mathf.Max(0.0f, this.m_RecenteringTime);
      }

      internal bool LegacyUpgrade(
        ref CinemachineOrbitalTransposer.Heading.HeadingDefinition heading,
        ref int velocityFilter)
      {
        if (this.m_LegacyHeadingDefinition == -1 || this.m_LegacyVelocityFilterStrength == -1)
          return false;
        heading = (CinemachineOrbitalTransposer.Heading.HeadingDefinition) this.m_LegacyHeadingDefinition;
        velocityFilter = this.m_LegacyVelocityFilterStrength;
        this.m_LegacyHeadingDefinition = this.m_LegacyVelocityFilterStrength = -1;
        return true;
      }
    }

    internal delegate float UpdateHeadingDelegate(
      CinemachineOrbitalTransposer orbital,
      float deltaTime,
      Vector3 up);

    private class HeadingTracker
    {
      private CinemachineOrbitalTransposer.HeadingTracker.Item[] mHistory;
      private int mTop;
      private int mBottom;
      private int mCount;
      private Vector3 mHeadingSum;
      private float mWeightSum = 0.0f;
      private float mWeightTime = 0.0f;
      private Vector3 mLastGoodHeading = Vector3.zero;
      private static float mDecayExponent;

      public HeadingTracker(int filterSize)
      {
        this.mHistory = new CinemachineOrbitalTransposer.HeadingTracker.Item[filterSize];
        float num = (float) filterSize / 5f;
        CinemachineOrbitalTransposer.HeadingTracker.mDecayExponent = -Mathf.Log(2f) / num;
        this.ClearHistory();
      }

      public int FilterSize => this.mHistory.Length;

      private void ClearHistory()
      {
        this.mTop = this.mBottom = this.mCount = 0;
        this.mWeightSum = 0.0f;
        this.mHeadingSum = Vector3.zero;
      }

      private static float Decay(float time)
      {
        return Mathf.Exp(time * CinemachineOrbitalTransposer.HeadingTracker.mDecayExponent);
      }

      public void Add(Vector3 velocity)
      {
        if (this.FilterSize == 0)
        {
          this.mLastGoodHeading = velocity;
        }
        else
        {
          float magnitude = velocity.magnitude;
          if ((double) magnitude <= 9.9999997473787516E-05)
            return;
          CinemachineOrbitalTransposer.HeadingTracker.Item obj = new CinemachineOrbitalTransposer.HeadingTracker.Item();
          obj.velocity = velocity;
          obj.weight = magnitude;
          obj.time = Time.time;
          if (this.mCount == this.FilterSize)
            this.PopBottom();
          ++this.mCount;
          this.mHistory[this.mTop] = obj;
          if (++this.mTop == this.FilterSize)
            this.mTop = 0;
          this.mWeightSum *= CinemachineOrbitalTransposer.HeadingTracker.Decay(obj.time - this.mWeightTime);
          this.mWeightTime = obj.time;
          this.mWeightSum += magnitude;
          this.mHeadingSum += obj.velocity;
        }
      }

      private void PopBottom()
      {
        if (this.mCount <= 0)
          return;
        float time = Time.time;
        CinemachineOrbitalTransposer.HeadingTracker.Item obj = this.mHistory[this.mBottom];
        if (++this.mBottom == this.FilterSize)
          this.mBottom = 0;
        --this.mCount;
        float num = CinemachineOrbitalTransposer.HeadingTracker.Decay(time - obj.time);
        this.mWeightSum -= obj.weight * num;
        this.mHeadingSum -= obj.velocity * num;
        if ((double) this.mWeightSum <= 9.9999997473787516E-05 || this.mCount == 0)
          this.ClearHistory();
      }

      public void DecayHistory()
      {
        float time = Time.time;
        float num = CinemachineOrbitalTransposer.HeadingTracker.Decay(time - this.mWeightTime);
        this.mWeightSum *= num;
        this.mWeightTime = time;
        if ((double) this.mWeightSum < 9.9999997473787516E-05)
          this.ClearHistory();
        else
          this.mHeadingSum *= num;
      }

      public Vector3 GetReliableHeading()
      {
        if ((double) this.mWeightSum > 9.9999997473787516E-05 && (this.mCount == this.mHistory.Length || this.mLastGoodHeading.AlmostZero()))
        {
          Vector3 v = this.mHeadingSum / this.mWeightSum;
          if (!v.AlmostZero())
            this.mLastGoodHeading = v.normalized;
        }
        return this.mLastGoodHeading;
      }

      private struct Item
      {
        public Vector3 velocity;
        public float weight;
        public float time;
      }
    }
  }
}
