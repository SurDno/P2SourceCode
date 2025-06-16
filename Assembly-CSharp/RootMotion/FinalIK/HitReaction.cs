using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  public class HitReaction : OffsetModifier
  {
    [Tooltip("Hit points for the FBBIK effectors")]
    public HitReaction.HitPointEffector[] effectorHitPoints;
    [Tooltip(" Hit points for bones without an effector, such as the head")]
    public HitReaction.HitPointBone[] boneHitPoints;

    public bool inProgress
    {
      get
      {
        foreach (HitReaction.HitPoint effectorHitPoint in this.effectorHitPoints)
        {
          if (effectorHitPoint.inProgress)
            return true;
        }
        foreach (HitReaction.HitPoint boneHitPoint in this.boneHitPoints)
        {
          if (boneHitPoint.inProgress)
            return true;
        }
        return false;
      }
    }

    protected override void OnModifyOffset()
    {
      foreach (HitReaction.HitPoint effectorHitPoint in this.effectorHitPoints)
        effectorHitPoint.Apply(this.ik.solver, this.weight);
      foreach (HitReaction.HitPoint boneHitPoint in this.boneHitPoints)
        boneHitPoint.Apply(this.ik.solver, this.weight);
    }

    public void Hit(Collider collider, Vector3 force, Vector3 point)
    {
      if ((UnityEngine.Object) this.ik == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "No IK assigned in HitReaction");
      }
      else
      {
        foreach (HitReaction.HitPointEffector effectorHitPoint in this.effectorHitPoints)
        {
          if ((UnityEngine.Object) effectorHitPoint.collider == (UnityEngine.Object) collider)
            effectorHitPoint.Hit(force, point);
        }
        foreach (HitReaction.HitPointBone boneHitPoint in this.boneHitPoints)
        {
          if ((UnityEngine.Object) boneHitPoint.collider == (UnityEngine.Object) collider)
            boneHitPoint.Hit(force, point);
        }
      }
    }

    [Serializable]
    public abstract class HitPoint
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

      public bool inProgress => (double) this.timer < (double) this.length;

      protected float crossFader { get; private set; }

      protected float timer { get; private set; }

      protected Vector3 force { get; private set; }

      protected Vector3 point { get; private set; }

      public void Hit(Vector3 force, Vector3 point)
      {
        if ((double) this.length == 0.0)
          this.length = this.GetLength();
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

      public void Apply(IKSolverFullBodyBiped solver, float weight)
      {
        float num = Time.time - this.lastTime;
        this.lastTime = Time.time;
        if ((double) this.timer >= (double) this.length)
          return;
        this.timer = Mathf.Clamp(this.timer + num, 0.0f, this.length);
        this.crossFader = (double) this.crossFadeSpeed <= 0.0 ? 1f : Mathf.Clamp(this.crossFader + num * this.crossFadeSpeed, 0.0f, 1f);
        this.OnApply(solver, weight);
      }

      protected abstract float GetLength();

      protected abstract void CrossFadeStart();

      protected abstract void OnApply(IKSolverFullBodyBiped solver, float weight);
    }

    [Serializable]
    public class HitPointEffector : HitReaction.HitPoint
    {
      [Tooltip("Offset magnitude in the direction of the hit force")]
      public AnimationCurve offsetInForceDirection;
      [Tooltip("Offset magnitude in the direction of character.up")]
      public AnimationCurve offsetInUpDirection;
      [Tooltip("Linking this offset to the FBBIK effectors")]
      public HitReaction.HitPointEffector.EffectorLink[] effectorLinks;

      protected override float GetLength()
      {
        float max = this.offsetInForceDirection.keys.Length != 0 ? this.offsetInForceDirection.keys[this.offsetInForceDirection.length - 1].time : 0.0f;
        float min = this.offsetInUpDirection.keys.Length != 0 ? this.offsetInUpDirection.keys[this.offsetInUpDirection.length - 1].time : 0.0f;
        return Mathf.Clamp(max, min, max);
      }

      protected override void CrossFadeStart()
      {
        foreach (HitReaction.HitPointEffector.EffectorLink effectorLink in this.effectorLinks)
          effectorLink.CrossFadeStart();
      }

      protected override void OnApply(IKSolverFullBodyBiped solver, float weight)
      {
        Vector3 vector3 = solver.GetRoot().up * this.force.magnitude;
        Vector3 offset = (this.offsetInForceDirection.Evaluate(this.timer) * this.force + this.offsetInUpDirection.Evaluate(this.timer) * vector3) * weight;
        foreach (HitReaction.HitPointEffector.EffectorLink effectorLink in this.effectorLinks)
          effectorLink.Apply(solver, offset, this.crossFader);
      }

      [Serializable]
      public class EffectorLink
      {
        [Tooltip("The FBBIK effector type")]
        public FullBodyBipedEffector effector;
        [Tooltip("The weight of this effector (could also be negative)")]
        public float weight;
        private Vector3 lastValue;
        private Vector3 current;

        public void Apply(IKSolverFullBodyBiped solver, Vector3 offset, float crossFader)
        {
          this.current = Vector3.Lerp(this.lastValue, offset * this.weight, crossFader);
          solver.GetEffector(this.effector).positionOffset += this.current;
        }

        public void CrossFadeStart() => this.lastValue = this.current;
      }
    }

    [Serializable]
    public class HitPointBone : HitReaction.HitPoint
    {
      [Tooltip("The angle to rotate the bone around it's rigidbody's world center of mass")]
      public AnimationCurve aroundCenterOfMass;
      [Tooltip("Linking this hit point to bone(s)")]
      public HitReaction.HitPointBone.BoneLink[] boneLinks;
      private Rigidbody rigidbody;

      protected override float GetLength()
      {
        return this.aroundCenterOfMass.keys.Length != 0 ? this.aroundCenterOfMass.keys[this.aroundCenterOfMass.length - 1].time : 0.0f;
      }

      protected override void CrossFadeStart()
      {
        foreach (HitReaction.HitPointBone.BoneLink boneLink in this.boneLinks)
          boneLink.CrossFadeStart();
      }

      protected override void OnApply(IKSolverFullBodyBiped solver, float weight)
      {
        if ((UnityEngine.Object) this.rigidbody == (UnityEngine.Object) null)
          this.rigidbody = this.collider.GetComponent<Rigidbody>();
        if (!((UnityEngine.Object) this.rigidbody != (UnityEngine.Object) null))
          return;
        Vector3 axis = Vector3.Cross(this.force, this.point - this.rigidbody.worldCenterOfMass);
        Quaternion offset = Quaternion.AngleAxis(this.aroundCenterOfMass.Evaluate(this.timer) * weight, axis);
        foreach (HitReaction.HitPointBone.BoneLink boneLink in this.boneLinks)
          boneLink.Apply(solver, offset, this.crossFader);
      }

      [Serializable]
      public class BoneLink
      {
        [Tooltip("Reference to the bone that this hit point rotates")]
        public Transform bone;
        [Tooltip("Weight of rotating the bone")]
        [Range(0.0f, 1f)]
        public float weight;
        private Quaternion lastValue = Quaternion.identity;
        private Quaternion current = Quaternion.identity;

        public void Apply(IKSolverFullBodyBiped solver, Quaternion offset, float crossFader)
        {
          this.current = Quaternion.Lerp(this.lastValue, Quaternion.Lerp(Quaternion.identity, offset, this.weight), crossFader);
          this.bone.rotation = this.current * this.bone.rotation;
        }

        public void CrossFadeStart() => this.lastValue = this.current;
      }
    }
  }
}
