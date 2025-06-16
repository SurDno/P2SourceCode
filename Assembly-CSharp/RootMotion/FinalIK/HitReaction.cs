using System;

namespace RootMotion.FinalIK
{
  public class HitReaction : OffsetModifier
  {
    [Tooltip("Hit points for the FBBIK effectors")]
    public HitPointEffector[] effectorHitPoints;
    [Tooltip(" Hit points for bones without an effector, such as the head")]
    public HitPointBone[] boneHitPoints;

    public bool inProgress
    {
      get
      {
        foreach (HitPoint effectorHitPoint in effectorHitPoints)
        {
          if (effectorHitPoint.inProgress)
            return true;
        }
        foreach (HitPoint boneHitPoint in boneHitPoints)
        {
          if (boneHitPoint.inProgress)
            return true;
        }
        return false;
      }
    }

    protected override void OnModifyOffset()
    {
      foreach (HitPoint effectorHitPoint in effectorHitPoints)
        effectorHitPoint.Apply(ik.solver, weight);
      foreach (HitPoint boneHitPoint in boneHitPoints)
        boneHitPoint.Apply(ik.solver, weight);
    }

    public void Hit(Collider collider, Vector3 force, Vector3 point)
    {
      if ((UnityEngine.Object) ik == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "No IK assigned in HitReaction");
      }
      else
      {
        foreach (HitPointEffector effectorHitPoint in effectorHitPoints)
        {
          if ((UnityEngine.Object) effectorHitPoint.collider == (UnityEngine.Object) collider)
            effectorHitPoint.Hit(force, point);
        }
        foreach (HitPointBone boneHitPoint in boneHitPoints)
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

      public bool inProgress => timer < (double) length;

      protected float crossFader { get; private set; }

      protected float timer { get; private set; }

      protected Vector3 force { get; private set; }

      protected Vector3 point { get; private set; }

      public void Hit(Vector3 force, Vector3 point)
      {
        if (length == 0.0)
          length = GetLength();
        if (length <= 0.0)
        {
          Debug.LogError((object) "Hit Point WeightCurve length is zero.");
        }
        else
        {
          if (timer < 1.0)
            crossFader = 0.0f;
          crossFadeSpeed = crossFadeTime > 0.0 ? 1f / crossFadeTime : 0.0f;
          CrossFadeStart();
          timer = 0.0f;
          this.force = force;
          this.point = point;
        }
      }

      public void Apply(IKSolverFullBodyBiped solver, float weight)
      {
        float num = Time.time - lastTime;
        lastTime = Time.time;
        if (timer >= (double) length)
          return;
        timer = Mathf.Clamp(timer + num, 0.0f, length);
        crossFader = crossFadeSpeed <= 0.0 ? 1f : Mathf.Clamp(crossFader + num * crossFadeSpeed, 0.0f, 1f);
        OnApply(solver, weight);
      }

      protected abstract float GetLength();

      protected abstract void CrossFadeStart();

      protected abstract void OnApply(IKSolverFullBodyBiped solver, float weight);
    }

    [Serializable]
    public class HitPointEffector : HitPoint
    {
      [Tooltip("Offset magnitude in the direction of the hit force")]
      public AnimationCurve offsetInForceDirection;
      [Tooltip("Offset magnitude in the direction of character.up")]
      public AnimationCurve offsetInUpDirection;
      [Tooltip("Linking this offset to the FBBIK effectors")]
      public EffectorLink[] effectorLinks;

      protected override float GetLength()
      {
        float max = offsetInForceDirection.keys.Length != 0 ? offsetInForceDirection.keys[offsetInForceDirection.length - 1].time : 0.0f;
        float min = offsetInUpDirection.keys.Length != 0 ? offsetInUpDirection.keys[offsetInUpDirection.length - 1].time : 0.0f;
        return Mathf.Clamp(max, min, max);
      }

      protected override void CrossFadeStart()
      {
        foreach (EffectorLink effectorLink in effectorLinks)
          effectorLink.CrossFadeStart();
      }

      protected override void OnApply(IKSolverFullBodyBiped solver, float weight)
      {
        Vector3 vector3 = solver.GetRoot().up * force.magnitude;
        Vector3 offset = (offsetInForceDirection.Evaluate(timer) * force + offsetInUpDirection.Evaluate(timer) * vector3) * weight;
        foreach (EffectorLink effectorLink in effectorLinks)
          effectorLink.Apply(solver, offset, crossFader);
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
          current = Vector3.Lerp(lastValue, offset * weight, crossFader);
          solver.GetEffector(effector).positionOffset += current;
        }

        public void CrossFadeStart() => lastValue = current;
      }
    }

    [Serializable]
    public class HitPointBone : HitPoint
    {
      [Tooltip("The angle to rotate the bone around it's rigidbody's world center of mass")]
      public AnimationCurve aroundCenterOfMass;
      [Tooltip("Linking this hit point to bone(s)")]
      public BoneLink[] boneLinks;
      private Rigidbody rigidbody;

      protected override float GetLength()
      {
        return aroundCenterOfMass.keys.Length != 0 ? aroundCenterOfMass.keys[aroundCenterOfMass.length - 1].time : 0.0f;
      }

      protected override void CrossFadeStart()
      {
        foreach (BoneLink boneLink in boneLinks)
          boneLink.CrossFadeStart();
      }

      protected override void OnApply(IKSolverFullBodyBiped solver, float weight)
      {
        if ((UnityEngine.Object) rigidbody == (UnityEngine.Object) null)
          rigidbody = collider.GetComponent<Rigidbody>();
        if (!((UnityEngine.Object) rigidbody != (UnityEngine.Object) null))
          return;
        Vector3 axis = Vector3.Cross(force, point - rigidbody.worldCenterOfMass);
        Quaternion offset = Quaternion.AngleAxis(aroundCenterOfMass.Evaluate(timer) * weight, axis);
        foreach (BoneLink boneLink in boneLinks)
          boneLink.Apply(solver, offset, crossFader);
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
          current = Quaternion.Lerp(lastValue, Quaternion.Lerp(Quaternion.identity, offset, weight), crossFader);
          bone.rotation = current * bone.rotation;
        }

        public void CrossFadeStart() => lastValue = current;
      }
    }
  }
}
