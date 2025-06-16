using System;
using Cinemachine.Utility;

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
    public Heading m_Heading = new Heading(Heading.HeadingDefinition.TargetForward, 4, 0.0f);
    [Tooltip("Automatic heading recentering.  The settings here defines how the camera will reposition itself in the absence of player input.")]
    public Recentering m_RecenterToTargetHeading = new Recentering(true, 1f, 2f);
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
    internal UpdateHeadingDelegate HeadingUpdater = (orbital, deltaTime, up) => orbital.UpdateHeading(deltaTime, up, ref orbital.m_XAxis);
    private float mLastHeadingAxisInputTime;
    private float mHeadingRecenteringVelocity;
    private Vector3 mLastTargetPosition = Vector3.zero;
    private HeadingTracker mHeadingTracker;
    private Rigidbody mTargetRigidBody = (Rigidbody) null;
    private Quaternion mHeadingPrevFrame = Quaternion.identity;
    private Vector3 mOffsetPrevFrame = Vector3.zero;

    protected override void OnValidate()
    {
      if (m_LegacyRadius != 3.4028234663852886E+38 && m_LegacyHeightOffset != 3.4028234663852886E+38 && m_LegacyHeadingBias != 3.4028234663852886E+38)
      {
        m_FollowOffset = new Vector3(0.0f, m_LegacyHeightOffset, -m_LegacyRadius);
        m_LegacyHeightOffset = m_LegacyRadius = float.MaxValue;
        m_Heading.m_HeadingBias = m_LegacyHeadingBias;
        m_XAxis.m_MaxSpeed /= 10f;
        m_XAxis.m_AccelTime /= 10f;
        m_XAxis.m_DecelTime /= 10f;
        m_LegacyHeadingBias = float.MaxValue;
        m_RecenterToTargetHeading.LegacyUpgrade(ref m_Heading.m_HeadingDefinition, ref m_Heading.m_VelocityFilterStrength);
      }
      m_XAxis.Validate();
      m_RecenterToTargetHeading.Validate();
      base.OnValidate();
    }

    public float UpdateHeading(float deltaTime, Vector3 up, ref AxisState axis)
    {
      if ((deltaTime >= 0.0 || CinemachineCore.Instance.IsLive(VirtualCamera)) && false | axis.Update(deltaTime))
      {
        mLastHeadingAxisInputTime = Time.time;
        mHeadingRecenteringVelocity = 0.0f;
      }
      float targetHeading = GetTargetHeading(axis.Value, GetReferenceOrientation(up), deltaTime);
      if (deltaTime < 0.0)
      {
        mHeadingRecenteringVelocity = 0.0f;
        if (m_RecenterToTargetHeading.m_enabled)
          axis.Value = targetHeading;
      }
      else if (m_BindingMode != BindingMode.SimpleFollowWithWorldUp && m_RecenterToTargetHeading.m_enabled && (double) Time.time > mLastHeadingAxisInputTime + (double) m_RecenterToTargetHeading.m_RecenterWaitTime)
      {
        float num1 = m_RecenterToTargetHeading.m_RecenteringTime / 3f;
        if (num1 <= (double) deltaTime)
        {
          axis.Value = targetHeading;
        }
        else
        {
          float f = Mathf.DeltaAngle(axis.Value, targetHeading);
          float a = Mathf.Abs(f);
          if (a < 9.9999997473787516E-05)
          {
            axis.Value = targetHeading;
            mHeadingRecenteringVelocity = 0.0f;
          }
          else
          {
            float num2 = deltaTime / num1;
            float num3 = Mathf.Sign(f) * Mathf.Min(a, a * num2);
            float num4 = num3 - mHeadingRecenteringVelocity;
            if (num3 < 0.0 && num4 < 0.0 || num3 > 0.0 && num4 > 0.0)
              num3 = mHeadingRecenteringVelocity + num3 * num2;
            axis.Value += num3;
            mHeadingRecenteringVelocity = num3;
          }
        }
      }
      float num = axis.Value;
      if (m_BindingMode == BindingMode.SimpleFollowWithWorldUp)
        axis.Value = 0.0f;
      return num;
    }

    private void OnEnable()
    {
      m_XAxis.SetThresholds(0.0f, 360f, true);
      PreviousTarget = (Transform) null;
      mLastTargetPosition = Vector3.zero;
    }

    private Transform PreviousTarget { get; set; }

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      InitPrevFrameStateInfo(ref curState, deltaTime);
      if ((UnityEngine.Object) FollowTarget != (UnityEngine.Object) PreviousTarget)
      {
        PreviousTarget = FollowTarget;
        mTargetRigidBody = (UnityEngine.Object) PreviousTarget == (UnityEngine.Object) null ? (Rigidbody) null : PreviousTarget.GetComponent<Rigidbody>();
        mLastTargetPosition = (UnityEngine.Object) PreviousTarget == (UnityEngine.Object) null ? Vector3.zero : PreviousTarget.position;
        mHeadingTracker = null;
      }
      float angle = HeadingUpdater(this, deltaTime, curState.ReferenceUp);
      if (!IsValid)
        return;
      mLastTargetPosition = FollowTarget.position;
      if (m_BindingMode != BindingMode.SimpleFollowWithWorldUp)
        angle += m_Heading.m_HeadingBias;
      Quaternion quaternion1 = Quaternion.AngleAxis(angle, curState.ReferenceUp);
      Vector3 effectiveOffset = EffectiveOffset;
      Vector3 outTargetPosition;
      Quaternion outTargetOrient;
      TrackTarget(deltaTime, curState.ReferenceUp, quaternion1 * effectiveOffset, out outTargetPosition, out outTargetOrient);
      curState.ReferenceUp = outTargetOrient * Vector3.up;
      if (deltaTime >= 0.0)
      {
        Vector3 vector3_1 = quaternion1 * effectiveOffset - mHeadingPrevFrame * mOffsetPrevFrame;
        Vector3 vector3_2 = outTargetOrient * vector3_1;
        curState.PositionDampingBypass = vector3_2;
      }
      Quaternion quaternion2 = outTargetOrient * quaternion1;
      curState.RawPosition = outTargetPosition + quaternion2 * effectiveOffset;
      mHeadingPrevFrame = m_BindingMode == BindingMode.SimpleFollowWithWorldUp ? Quaternion.identity : quaternion1;
      mOffsetPrevFrame = effectiveOffset;
    }

    public override void OnPositionDragged(Vector3 delta)
    {
      m_FollowOffset = m_FollowOffset + (Quaternion.Inverse(GetReferenceOrientation(VcamState.ReferenceUp)) * delta) with
      {
        x = 0.0f
      };
      m_FollowOffset = EffectiveOffset;
    }

    private static string GetFullName(GameObject current)
    {
      if ((UnityEngine.Object) current == (UnityEngine.Object) null)
        return "";
      return (UnityEngine.Object) current.transform.parent == (UnityEngine.Object) null ? "/" + current.name : GetFullName(current.transform.parent.gameObject) + "/" + current.name;
    }

    private float GetTargetHeading(
      float currentHeading,
      Quaternion targetOrientation,
      float deltaTime)
    {
      if (m_BindingMode == BindingMode.SimpleFollowWithWorldUp)
        return 0.0f;
      if ((UnityEngine.Object) FollowTarget == (UnityEngine.Object) null)
        return currentHeading;
      if (m_Heading.m_HeadingDefinition == Heading.HeadingDefinition.Velocity && (UnityEngine.Object) mTargetRigidBody == (UnityEngine.Object) null)
      {
        Debug.Log((object) string.Format("Attempted to use HeadingDerivationMode.Velocity to calculate heading for {0}. No RigidBody was present on '{1}'. Defaulting to position delta", GetFullName(VirtualCamera.VirtualCameraGameObject), (object) FollowTarget));
        m_Heading.m_HeadingDefinition = Heading.HeadingDefinition.PositionDelta;
      }
      Vector3 zero = Vector3.zero;
      Vector3 vector;
      switch (m_Heading.m_HeadingDefinition)
      {
        case Heading.HeadingDefinition.PositionDelta:
          vector = FollowTarget.position - mLastTargetPosition;
          break;
        case Heading.HeadingDefinition.Velocity:
          vector = mTargetRigidBody.velocity;
          break;
        case Heading.HeadingDefinition.TargetForward:
          vector = FollowTarget.forward;
          break;
        default:
          return 0.0f;
      }
      int filterSize = m_Heading.m_VelocityFilterStrength * 5;
      if (mHeadingTracker == null || mHeadingTracker.FilterSize != filterSize)
        mHeadingTracker = new HeadingTracker(filterSize);
      mHeadingTracker.DecayHistory();
      Vector3 vector3_1 = targetOrientation * Vector3.up;
      Vector3 vector3_2 = vector.ProjectOntoPlane(vector3_1);
      if (!vector3_2.AlmostZero())
        mHeadingTracker.Add(vector3_2);
      Vector3 reliableHeading = mHeadingTracker.GetReliableHeading();
      return !reliableHeading.AlmostZero() ? UnityVectorExtensions.SignedAngle(targetOrientation * Vector3.forward, reliableHeading, vector3_1) : currentHeading;
    }

    [DocumentationSorting(6.2f, DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public struct Heading
    {
      [Tooltip("How 'forward' is defined.  The camera will be placed by default behind the target.  PositionDelta will consider 'forward' to be the direction in which the target is moving.")]
      public HeadingDefinition m_HeadingDefinition;
      [Range(0.0f, 10f)]
      [Tooltip("Size of the velocity sampling window for target heading filter.  This filters out irregularities in the target's movement.  Used only if deriving heading from target's movement (PositionDelta or Velocity)")]
      public int m_VelocityFilterStrength;
      [Range(-180f, 180f)]
      [Tooltip("Where the camera is placed when the X-axis value is zero.  This is a rotation in degrees around the Y axis.  When this value is 0, the camera will be placed behind the target.  Nonzero offsets will rotate the zero position around the target.")]
      public float m_HeadingBias;

      public Heading(
        HeadingDefinition def,
        int filterStrength,
        float bias)
      {
        m_HeadingDefinition = def;
        m_VelocityFilterStrength = filterStrength;
        m_HeadingBias = bias;
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
        m_enabled = enabled;
        m_RecenterWaitTime = recenterWaitTime;
        m_RecenteringTime = recenteringSpeed;
        m_LegacyHeadingDefinition = m_LegacyVelocityFilterStrength = -1;
      }

      public void Validate()
      {
        m_RecenterWaitTime = Mathf.Max(0.0f, m_RecenterWaitTime);
        m_RecenteringTime = Mathf.Max(0.0f, m_RecenteringTime);
      }

      internal bool LegacyUpgrade(
        ref Heading.HeadingDefinition heading,
        ref int velocityFilter)
      {
        if (m_LegacyHeadingDefinition == -1 || m_LegacyVelocityFilterStrength == -1)
          return false;
        heading = (Heading.HeadingDefinition) m_LegacyHeadingDefinition;
        velocityFilter = m_LegacyVelocityFilterStrength;
        m_LegacyHeadingDefinition = m_LegacyVelocityFilterStrength = -1;
        return true;
      }
    }

    internal delegate float UpdateHeadingDelegate(
      CinemachineOrbitalTransposer orbital,
      float deltaTime,
      Vector3 up);

    private class HeadingTracker
    {
      private Item[] mHistory;
      private int mTop;
      private int mBottom;
      private int mCount;
      private Vector3 mHeadingSum;
      private float mWeightSum;
      private float mWeightTime;
      private Vector3 mLastGoodHeading = Vector3.zero;
      private static float mDecayExponent;

      public HeadingTracker(int filterSize)
      {
        mHistory = new Item[filterSize];
        float num = filterSize / 5f;
        mDecayExponent = -Mathf.Log(2f) / num;
        ClearHistory();
      }

      public int FilterSize => mHistory.Length;

      private void ClearHistory()
      {
        mTop = mBottom = mCount = 0;
        mWeightSum = 0.0f;
        mHeadingSum = Vector3.zero;
      }

      private static float Decay(float time)
      {
        return Mathf.Exp(time * mDecayExponent);
      }

      public void Add(Vector3 velocity)
      {
        if (FilterSize == 0)
        {
          mLastGoodHeading = velocity;
        }
        else
        {
          float magnitude = velocity.magnitude;
          if (magnitude <= 9.9999997473787516E-05)
            return;
          Item obj = new Item();
          obj.velocity = velocity;
          obj.weight = magnitude;
          obj.time = Time.time;
          if (mCount == FilterSize)
            PopBottom();
          ++mCount;
          mHistory[mTop] = obj;
          if (++mTop == FilterSize)
            mTop = 0;
          mWeightSum *= Decay(obj.time - mWeightTime);
          mWeightTime = obj.time;
          mWeightSum += magnitude;
          mHeadingSum += obj.velocity;
        }
      }

      private void PopBottom()
      {
        if (mCount <= 0)
          return;
        float time = Time.time;
        Item obj = mHistory[mBottom];
        if (++mBottom == FilterSize)
          mBottom = 0;
        --mCount;
        float num = Decay(time - obj.time);
        mWeightSum -= obj.weight * num;
        mHeadingSum -= obj.velocity * num;
        if (mWeightSum <= 9.9999997473787516E-05 || mCount == 0)
          ClearHistory();
      }

      public void DecayHistory()
      {
        float time = Time.time;
        float num = Decay(time - mWeightTime);
        mWeightSum *= num;
        mWeightTime = time;
        if (mWeightSum < 9.9999997473787516E-05)
          ClearHistory();
        else
          mHeadingSum *= num;
      }

      public Vector3 GetReliableHeading()
      {
        if (mWeightSum > 9.9999997473787516E-05 && (mCount == mHistory.Length || mLastGoodHeading.AlmostZero()))
        {
          Vector3 v = mHeadingSum / mWeightSum;
          if (!v.AlmostZero())
            mLastGoodHeading = v.normalized;
        }
        return mLastGoodHeading;
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
