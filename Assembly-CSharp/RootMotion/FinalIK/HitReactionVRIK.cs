using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  public class HitReactionVRIK : OffsetModifierVRIK
  {
    public AnimationCurve[] offsetCurves;
    [Tooltip("Hit points for the FBBIK effectors")]
    public HitReactionVRIK.PositionOffset[] positionOffsets;
    [Tooltip(" Hit points for bones without an effector, such as the head")]
    public HitReactionVRIK.RotationOffset[] rotationOffsets;

    protected override void OnModifyOffset()
    {
      foreach (HitReactionVRIK.Offset positionOffset in this.positionOffsets)
        positionOffset.Apply(this.ik, this.offsetCurves, this.weight);
      foreach (HitReactionVRIK.Offset rotationOffset in this.rotationOffsets)
        rotationOffset.Apply(this.ik, this.offsetCurves, this.weight);
    }

    public void Hit(Collider collider, Vector3 force, Vector3 point)
    {
      if ((UnityEngine.Object) this.ik == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "No IK assigned in HitReaction");
      }
      else
      {
        foreach (HitReactionVRIK.PositionOffset positionOffset in this.positionOffsets)
        {
          if ((UnityEngine.Object) positionOffset.collider == (UnityEngine.Object) collider)
            positionOffset.Hit(force, this.offsetCurves, point);
        }
        foreach (HitReactionVRIK.RotationOffset rotationOffset in this.rotationOffsets)
        {
          if ((UnityEngine.Object) rotationOffset.collider == (UnityEngine.Object) collider)
            rotationOffset.Hit(force, this.offsetCurves, point);
        }
      }
    }

    [Serializable]
    public abstract class Offset
    {
      [Tooltip("Just for visual clarity, not used at all")]
      public string name;
      [Tooltip("Linking this hit point to a collider")]
      public Collider collider;
      [Tooltip("Only used if this hit point gets hit when already processing another hit")]
      [SerializeField]
      private float crossFadeTime = 0.1f;
      private float length;
      private float crossFadeSpeed;
      private float lastTime;

      protected float crossFader { get; private set; }

      protected float timer { get; private set; }

      protected Vector3 force { get; private set; }

      protected Vector3 point { get; private set; }

      public void Hit(Vector3 force, AnimationCurve[] curves, Vector3 point)
      {
        if ((double) this.length == 0.0)
          this.length = this.GetLength(curves);
        if ((double) this.length <= 0.0)
        {
          Debug.LogError((object) "Hit Point WeightCurve length is zero.");
        }
        else
        {
          if ((double) this.timer < 1.0)
            this.crossFader = 0.0f;
          this.crossFadeSpeed = (double) this.crossFadeTime > 0.0 ? 1f / this.crossFadeTime : 0.0f;
          this.CrossFadeStart();
          this.timer = 0.0f;
          this.force = force;
          this.point = point;
        }
      }

      public void Apply(VRIK ik, AnimationCurve[] curves, float weight)
      {
        float num = Time.time - this.lastTime;
        this.lastTime = Time.time;
        if ((double) this.timer >= (double) this.length)
          return;
        this.timer = Mathf.Clamp(this.timer + num, 0.0f, this.length);
        this.crossFader = (double) this.crossFadeSpeed <= 0.0 ? 1f : Mathf.Clamp(this.crossFader + num * this.crossFadeSpeed, 0.0f, 1f);
        this.OnApply(ik, curves, weight);
      }

      protected abstract float GetLength(AnimationCurve[] curves);

      protected abstract void CrossFadeStart();

      protected abstract void OnApply(VRIK ik, AnimationCurve[] curves, float weight);
    }

    [Serializable]
    public class PositionOffset : HitReactionVRIK.Offset
    {
      [Tooltip("Offset magnitude in the direction of the hit force")]
      public int forceDirCurveIndex;
      [Tooltip("Offset magnitude in the direction of character.up")]
      public int upDirCurveIndex = 1;
      [Tooltip("Linking this offset to the VRIK position offsets")]
      public HitReactionVRIK.PositionOffset.PositionOffsetLink[] offsetLinks;

      protected override float GetLength(AnimationCurve[] curves)
      {
        float max = curves[this.forceDirCurveIndex].keys.Length != 0 ? curves[this.forceDirCurveIndex].keys[curves[this.forceDirCurveIndex].length - 1].time : 0.0f;
        float min = curves[this.upDirCurveIndex].keys.Length != 0 ? curves[this.upDirCurveIndex].keys[curves[this.upDirCurveIndex].length - 1].time : 0.0f;
        return Mathf.Clamp(max, min, max);
      }

      protected override void CrossFadeStart()
      {
        foreach (HitReactionVRIK.PositionOffset.PositionOffsetLink offsetLink in this.offsetLinks)
          offsetLink.CrossFadeStart();
      }

      protected override void OnApply(VRIK ik, AnimationCurve[] curves, float weight)
      {
        Vector3 vector3 = ik.transform.up * this.force.magnitude;
        Vector3 offset = (curves[this.forceDirCurveIndex].Evaluate(this.timer) * this.force + curves[this.upDirCurveIndex].Evaluate(this.timer) * vector3) * weight;
        foreach (HitReactionVRIK.PositionOffset.PositionOffsetLink offsetLink in this.offsetLinks)
          offsetLink.Apply(ik, offset, this.crossFader);
      }

      [Serializable]
      public class PositionOffsetLink
      {
        [Tooltip("The FBBIK effector type")]
        public IKSolverVR.PositionOffset positionOffset;
        [Tooltip("The weight of this effector (could also be negative)")]
        public float weight;
        private Vector3 lastValue;
        private Vector3 current;

        public void Apply(VRIK ik, Vector3 offset, float crossFader)
        {
          this.current = Vector3.Lerp(this.lastValue, offset * this.weight, crossFader);
          ik.solver.AddPositionOffset(this.positionOffset, this.current);
        }

        public void CrossFadeStart() => this.lastValue = this.current;
      }
    }

    [Serializable]
    public class RotationOffset : HitReactionVRIK.Offset
    {
      [Tooltip("The angle to rotate the bone around it's rigidbody's world center of mass")]
      public int curveIndex;
      [Tooltip("Linking this hit point to bone(s)")]
      public HitReactionVRIK.RotationOffset.RotationOffsetLink[] offsetLinks;
      private Rigidbody rigidbody;

      protected override float GetLength(AnimationCurve[] curves)
      {
        return curves[this.curveIndex].keys.Length != 0 ? curves[this.curveIndex].keys[curves[this.curveIndex].length - 1].time : 0.0f;
      }

      protected override void CrossFadeStart()
      {
        foreach (HitReactionVRIK.RotationOffset.RotationOffsetLink offsetLink in this.offsetLinks)
          offsetLink.CrossFadeStart();
      }

      protected override void OnApply(VRIK ik, AnimationCurve[] curves, float weight)
      {
        if ((UnityEngine.Object) this.collider == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "No collider assigned for a HitPointBone in the HitReaction component.");
        }
        else
        {
          if ((UnityEngine.Object) this.rigidbody == (UnityEngine.Object) null)
            this.rigidbody = this.collider.GetComponent<Rigidbody>();
          if (!((UnityEngine.Object) this.rigidbody != (UnityEngine.Object) null))
            return;
          Vector3 axis = Vector3.Cross(this.force, this.point - this.rigidbody.worldCenterOfMass);
          Quaternion offset = Quaternion.AngleAxis(curves[this.curveIndex].Evaluate(this.timer) * weight, axis);
          foreach (HitReactionVRIK.RotationOffset.RotationOffsetLink offsetLink in this.offsetLinks)
            offsetLink.Apply(ik, offset, this.crossFader);
        }
      }

      [Serializable]
      public class RotationOffsetLink
      {
        [Tooltip("Reference to the bone that this hit point rotates")]
        public IKSolverVR.RotationOffset rotationOffset;
        [Tooltip("Weight of rotating the bone")]
        [Range(0.0f, 1f)]
        public float weight;
        private Quaternion lastValue = Quaternion.identity;
        private Quaternion current = Quaternion.identity;

        public void Apply(VRIK ik, Quaternion offset, float crossFader)
        {
          this.current = Quaternion.Lerp(this.lastValue, Quaternion.Lerp(Quaternion.identity, offset, this.weight), crossFader);
          ik.solver.AddRotationOffset(this.rotationOffset, this.current);
        }

        public void CrossFadeStart() => this.lastValue = this.current;
      }
    }
  }
}
