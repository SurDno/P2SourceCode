// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.Muscle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  [Serializable]
  public class Muscle
  {
    [HideInInspector]
    public string name;
    public ConfigurableJoint joint;
    public Transform target;
    public Muscle.Props props = new Muscle.Props();
    public Muscle.State state = Muscle.State.Default;
    [HideInInspector]
    public int[] parentIndexes = new int[0];
    [HideInInspector]
    public int[] childIndexes = new int[0];
    [HideInInspector]
    public bool[] childFlags = new bool[0];
    [HideInInspector]
    public int[] kinshipDegrees = new int[0];
    [HideInInspector]
    public MuscleCollisionBroadcaster broadcaster;
    [HideInInspector]
    public JointBreakBroadcaster jointBreakBroadcaster;
    [HideInInspector]
    public Vector3 positionOffset;
    private JointDrive slerpDrive = new JointDrive();
    private float lastJointDriveRotationWeight = -1f;
    private float lastRotationDamper = -1f;
    private Vector3 defaultPosition;
    private Vector3 defaultTargetLocalPosition;
    private Vector3 lastMappedPosition;
    private Quaternion defaultLocalRotation;
    private Quaternion localRotationConvert;
    private Quaternion toParentSpace;
    private Quaternion toJointSpaceInverse;
    private Quaternion toJointSpaceDefault;
    private Quaternion targetAnimatedRotation;
    private Quaternion targetAnimatedWorldRotation;
    private Quaternion defaultRotation;
    private Quaternion rotationRelativeToTarget;
    private Quaternion defaultTargetLocalRotation;
    private Quaternion lastMappedRotation;
    private Transform targetParent;
    private Transform connectedBodyTransform;
    private ConfigurableJointMotion angularXMotionDefault;
    private ConfigurableJointMotion angularYMotionDefault;
    private ConfigurableJointMotion angularZMotionDefault;
    private bool directTargetParent;
    private bool initiated;
    private Collider[] _colliders = new Collider[0];
    private float lastReadTime;
    private float lastWriteTime;
    private bool[] disabledColliders = new bool[0];

    public Transform transform { get; private set; }

    public Rigidbody rigidbody { get; private set; }

    public Transform connectedBodyTarget { get; private set; }

    public Vector3 targetAnimatedPosition { get; private set; }

    public Collider[] colliders => this._colliders;

    public Vector3 targetVelocity { get; private set; }

    public Vector3 targetAngularVelocity { get; private set; }

    public Vector3 mappedVelocity { get; private set; }

    public Vector3 mappedAngularVelocity { get; private set; }

    public Quaternion targetRotationRelative { get; private set; }

    public bool IsValid(bool log)
    {
      if ((UnityEngine.Object) this.joint == (UnityEngine.Object) null)
      {
        if (log)
          Debug.LogError((object) "Muscle joint is null");
        return false;
      }
      if ((UnityEngine.Object) this.target == (UnityEngine.Object) null)
      {
        if (log)
          Debug.LogError((object) ("Muscle " + this.joint.name + "target is null, please remove the muscle from PuppetMaster or disable PuppetMaster before destroying a muscle's target."));
        return false;
      }
      if (this.props == null && log)
        Debug.LogError((object) ("Muscle " + this.joint.name + "props is null"));
      return true;
    }

    public virtual void Initiate(Muscle[] colleagues)
    {
      this.initiated = false;
      if (!this.IsValid(true))
        return;
      this.name = this.joint.name;
      this.state = Muscle.State.Default;
      if ((UnityEngine.Object) this.joint.connectedBody != (UnityEngine.Object) null)
      {
        for (int index = 0; index < colleagues.Length; ++index)
        {
          if ((UnityEngine.Object) colleagues[index].joint.GetComponent<Rigidbody>() == (UnityEngine.Object) this.joint.connectedBody)
            this.connectedBodyTarget = colleagues[index].target;
        }
      }
      this.transform = this.joint.transform;
      this.rigidbody = this.transform.GetComponent<Rigidbody>();
      this.rigidbody.isKinematic = false;
      this.UpdateColliders();
      if (this._colliders.Length == 0)
      {
        Vector3 size = Vector3.one * 0.1f;
        Renderer component = this.transform.GetComponent<Renderer>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          size = component.bounds.size;
        this.rigidbody.inertiaTensor = this.CalculateInertiaTensorCuboid(size, this.rigidbody.mass);
      }
      this.targetParent = (UnityEngine.Object) this.connectedBodyTarget != (UnityEngine.Object) null ? this.connectedBodyTarget : this.target.parent;
      this.defaultLocalRotation = this.localRotation;
      Vector3 normalized1 = Vector3.Cross(this.joint.axis, this.joint.secondaryAxis).normalized;
      Vector3 normalized2 = Vector3.Cross(normalized1, this.joint.axis).normalized;
      if (normalized1 == normalized2)
      {
        Debug.LogError((object) ("Joint " + this.joint.name + " secondaryAxis is in the exact same direction as it's axis. Please make sure they are not aligned."));
      }
      else
      {
        this.rotationRelativeToTarget = Quaternion.Inverse(this.target.rotation) * this.transform.rotation;
        Quaternion rotation = Quaternion.LookRotation(normalized1, normalized2);
        this.toJointSpaceInverse = Quaternion.Inverse(rotation);
        this.toJointSpaceDefault = this.defaultLocalRotation * rotation;
        this.toParentSpace = Quaternion.Inverse(this.targetParentRotation) * this.parentRotation;
        this.localRotationConvert = Quaternion.Inverse(this.targetLocalRotation) * this.localRotation;
        if ((UnityEngine.Object) this.joint.connectedBody != (UnityEngine.Object) null)
        {
          this.joint.autoConfigureConnectedAnchor = false;
          this.connectedBodyTransform = this.joint.connectedBody.transform;
          this.directTargetParent = (UnityEngine.Object) this.target.parent == (UnityEngine.Object) this.connectedBodyTarget;
        }
        this.angularXMotionDefault = this.joint.angularXMotion;
        this.angularYMotionDefault = this.joint.angularYMotion;
        this.angularZMotionDefault = this.joint.angularZMotion;
        if ((UnityEngine.Object) this.joint.connectedBody == (UnityEngine.Object) null)
          this.props.mapPosition = true;
        this.targetRotationRelative = Quaternion.Inverse(this.rigidbody.rotation) * this.target.rotation;
        if ((UnityEngine.Object) this.joint.connectedBody == (UnityEngine.Object) null)
        {
          this.defaultPosition = this.transform.localPosition;
          this.defaultRotation = this.transform.localRotation;
        }
        else
        {
          this.defaultPosition = this.joint.connectedBody.transform.InverseTransformPoint(this.transform.position);
          this.defaultRotation = Quaternion.Inverse(this.joint.connectedBody.transform.rotation) * this.transform.rotation;
        }
        this.defaultTargetLocalPosition = this.target.localPosition;
        this.defaultTargetLocalRotation = this.target.localRotation;
        this.joint.rotationDriveMode = RotationDriveMode.Slerp;
        if (!this.joint.gameObject.activeInHierarchy)
        {
          Debug.LogError((object) "Can not initiate a puppet that has deactivated muscles.", (UnityEngine.Object) this.joint.transform);
        }
        else
        {
          this.joint.configuredInWorldSpace = false;
          this.joint.projectionMode = JointProjectionMode.None;
          if (this.joint.anchor != Vector3.zero)
          {
            Debug.LogError((object) ("PuppetMaster joint anchors need to be Vector3.zero. Joint axis on " + this.transform.name + " is " + (object) this.joint.anchor), (UnityEngine.Object) this.transform);
          }
          else
          {
            this.targetAnimatedPosition = this.target.position;
            this.targetAnimatedWorldRotation = this.target.rotation;
            this.targetAnimatedRotation = this.targetLocalRotation * this.localRotationConvert;
            this.Read();
            this.lastReadTime = Time.time;
            this.lastWriteTime = Time.time;
            this.lastMappedPosition = this.target.position;
            this.lastMappedRotation = this.target.rotation;
            this.initiated = true;
          }
        }
      }
    }

    public void UpdateColliders()
    {
      this._colliders = new Collider[0];
      this.AddColliders(this.joint.transform, ref this._colliders, true);
      int childCount = this.joint.transform.childCount;
      for (int index = 0; index < childCount; ++index)
        this.AddCompoundColliders(this.joint.transform.GetChild(index), ref this._colliders);
      this.disabledColliders = new bool[this._colliders.Length];
    }

    public void DisableColliders()
    {
      for (int index = 0; index < this._colliders.Length; ++index)
      {
        this.disabledColliders[index] = this._colliders[index].enabled;
        this._colliders[index].enabled = false;
      }
    }

    public void EnableColliders()
    {
      for (int index = 0; index < this._colliders.Length; ++index)
      {
        if (this.disabledColliders[index])
          this._colliders[index].enabled = true;
        this.disabledColliders[index] = false;
      }
    }

    private void AddColliders(Transform t, ref Collider[] C, bool includeMeshColliders)
    {
      Collider[] components = t.GetComponents<Collider>();
      int num1 = 0;
      foreach (Collider collider in components)
      {
        bool flag = collider is MeshCollider;
        if (!collider.isTrigger && (!includeMeshColliders || !flag))
          ++num1;
      }
      if (num1 == 0)
        return;
      int length = C.Length;
      Array.Resize<Collider>(ref C, length + num1);
      int num2 = 0;
      for (int index = 0; index < components.Length; ++index)
      {
        bool flag = components[index] is MeshCollider;
        if (!components[index].isTrigger && (!includeMeshColliders || !flag))
        {
          C[length + num2] = components[index];
          ++num2;
        }
      }
    }

    private void AddCompoundColliders(Transform t, ref Collider[] colliders)
    {
      if ((UnityEngine.Object) t.GetComponent<Rigidbody>() != (UnityEngine.Object) null)
        return;
      this.AddColliders(t, ref colliders, false);
      int childCount = t.childCount;
      for (int index = 0; index < childCount; ++index)
        this.AddCompoundColliders(t.GetChild(index), ref colliders);
    }

    public void IgnoreCollisions(Muscle m, bool ignore)
    {
      foreach (Collider collider1 in this.colliders)
      {
        foreach (Collider collider2 in m.colliders)
        {
          if ((UnityEngine.Object) collider1 != (UnityEngine.Object) null && (UnityEngine.Object) collider2 != (UnityEngine.Object) null && collider1.enabled && collider2.enabled && collider1.gameObject.activeInHierarchy && collider2.gameObject.activeInHierarchy)
            Physics.IgnoreCollision(collider1, collider2, ignore);
        }
      }
    }

    public void IgnoreAngularLimits(bool ignore)
    {
      if (!this.initiated)
        return;
      this.joint.angularXMotion = ignore ? ConfigurableJointMotion.Free : this.angularXMotionDefault;
      this.joint.angularYMotion = ignore ? ConfigurableJointMotion.Free : this.angularYMotionDefault;
      this.joint.angularZMotion = ignore ? ConfigurableJointMotion.Free : this.angularZMotionDefault;
    }

    public void FixTargetTransforms()
    {
      if (!this.initiated)
        return;
      this.target.localPosition = this.defaultTargetLocalPosition;
      this.target.localRotation = this.defaultTargetLocalRotation;
    }

    public void Reset()
    {
      if (!this.initiated || (UnityEngine.Object) this.joint == (UnityEngine.Object) null)
        return;
      if ((UnityEngine.Object) this.joint.connectedBody == (UnityEngine.Object) null)
      {
        this.transform.localPosition = this.defaultPosition;
        this.transform.localRotation = this.defaultRotation;
      }
      else
      {
        this.transform.position = this.joint.connectedBody.transform.TransformPoint(this.defaultPosition);
        this.transform.rotation = this.joint.connectedBody.transform.rotation * this.defaultRotation;
      }
    }

    public void MoveToTarget()
    {
      if (!this.initiated)
        return;
      this.transform.position = this.target.position;
      this.transform.rotation = this.target.rotation * this.rotationRelativeToTarget;
    }

    public void Read()
    {
      float num = Time.time - this.lastReadTime;
      this.lastReadTime = Time.time;
      if ((double) num > 0.0)
      {
        this.targetVelocity = (this.target.position - this.targetAnimatedPosition) / num;
        this.targetAngularVelocity = QuaTools.FromToRotation(this.targetAnimatedWorldRotation, this.target.rotation).eulerAngles / num;
      }
      this.targetAnimatedPosition = this.target.position;
      this.targetAnimatedWorldRotation = this.target.rotation;
      if (!((UnityEngine.Object) this.joint.connectedBody != (UnityEngine.Object) null))
        return;
      this.targetAnimatedRotation = this.targetLocalRotation * this.localRotationConvert;
    }

    public void ClearVelocities()
    {
      this.targetVelocity = Vector3.zero;
      this.targetAngularVelocity = Vector3.zero;
      this.mappedVelocity = Vector3.zero;
      this.mappedAngularVelocity = Vector3.zero;
      this.targetAnimatedPosition = this.target.position;
      this.targetAnimatedWorldRotation = this.target.rotation;
      this.lastMappedPosition = this.target.position;
      this.lastMappedRotation = this.target.rotation;
    }

    public void UpdateAnchor(bool supportTranslationAnimation)
    {
      if ((UnityEngine.Object) this.joint.connectedBody == (UnityEngine.Object) null || (UnityEngine.Object) this.connectedBodyTarget == (UnityEngine.Object) null || this.directTargetParent && !supportTranslationAnimation)
        return;
      this.joint.connectedAnchor = (this.joint.connectedAnchor = Muscle.InverseTransformPointUnscaled(this.connectedBodyTarget.position, this.connectedBodyTarget.rotation * this.toParentSpace, this.target.position)) * (1f / this.connectedBodyTransform.lossyScale.x);
    }

    public virtual void Update(
      float pinWeightMaster,
      float muscleWeightMaster,
      float muscleSpring,
      float muscleDamper,
      float pinPow,
      float pinDistanceFalloff,
      bool rotationTargetChanged)
    {
      this.state.velocity = this.rigidbody.velocity;
      this.state.angularVelocity = this.rigidbody.angularVelocity;
      this.props.Clamp();
      this.state.Clamp();
      this.Pin(pinWeightMaster, pinPow, pinDistanceFalloff);
      if (!rotationTargetChanged)
        return;
      this.MuscleRotation(muscleWeightMaster, muscleSpring, muscleDamper);
    }

    public void Map(float mappingWeightMaster)
    {
      float t = this.props.mappingWeight * mappingWeightMaster * this.state.mappingWeightMlp;
      if ((double) t <= 0.0)
        return;
      Vector3 position = this.transform.position;
      Quaternion rotation = this.transform.rotation;
      if ((double) t >= 1.0)
      {
        this.target.rotation = rotation * this.targetRotationRelative;
        if (!this.props.mapPosition)
          return;
        this.target.position = !((UnityEngine.Object) this.connectedBodyTransform != (UnityEngine.Object) null) ? position : this.connectedBodyTarget.TransformPoint(this.connectedBodyTransform.InverseTransformPoint(position));
      }
      else
      {
        this.target.rotation = Quaternion.Lerp(this.target.rotation, rotation * this.targetRotationRelative, t);
        if (!this.props.mapPosition)
          return;
        this.target.position = !((UnityEngine.Object) this.connectedBodyTransform != (UnityEngine.Object) null) ? Vector3.Lerp(this.target.position, position, t) : Vector3.Lerp(this.target.position, this.connectedBodyTarget.TransformPoint(this.connectedBodyTransform.InverseTransformPoint(position)), t);
      }
    }

    public void CalculateMappedVelocity()
    {
      float num = Time.time - this.lastWriteTime;
      if ((double) num > 0.0)
      {
        this.mappedVelocity = (this.target.position - this.lastMappedPosition) / num;
        this.mappedAngularVelocity = QuaTools.FromToRotation(this.lastMappedRotation, this.target.rotation).eulerAngles / num;
        this.lastWriteTime = Time.time;
      }
      this.lastMappedPosition = this.target.position;
      this.lastMappedRotation = this.target.rotation;
    }

    private void Pin(float pinWeightMaster, float pinPow, float pinDistanceFalloff)
    {
      this.positionOffset = this.targetAnimatedPosition - this.rigidbody.position;
      if (float.IsNaN(this.positionOffset.x))
        this.positionOffset = Vector3.zero;
      float f = pinWeightMaster * this.props.pinWeight * this.state.pinWeightMlp;
      if ((double) f <= 0.0)
        return;
      Vector3 vector3 = (-this.rigidbody.velocity + this.targetVelocity + this.positionOffset / Time.fixedDeltaTime) * Mathf.Pow(f, pinPow);
      if ((double) pinDistanceFalloff > 0.0)
        vector3 /= (float) (1.0 + (double) this.positionOffset.sqrMagnitude * (double) pinDistanceFalloff);
      this.rigidbody.velocity += vector3;
    }

    private void MuscleRotation(float muscleWeightMaster, float muscleSpring, float muscleDamper)
    {
      float a = (float) ((double) muscleWeightMaster * (double) this.props.muscleWeight * (double) muscleSpring * (double) this.state.muscleWeightMlp * 10.0);
      if ((UnityEngine.Object) this.joint.connectedBody == (UnityEngine.Object) null)
        a = 0.0f;
      else if ((double) a > 0.0)
        this.joint.targetRotation = this.LocalToJointSpace(this.targetAnimatedRotation);
      float b = this.props.muscleDamper * muscleDamper * this.state.muscleDamperMlp + this.state.muscleDamperAdd;
      if ((double) a == (double) this.lastJointDriveRotationWeight && (double) b == (double) this.lastRotationDamper)
        return;
      this.lastJointDriveRotationWeight = a;
      this.lastRotationDamper = b;
      this.slerpDrive.positionSpring = a;
      this.slerpDrive.maximumForce = Mathf.Max(a, b) * this.state.maxForceMlp;
      this.slerpDrive.positionDamper = b;
      this.joint.slerpDrive = this.slerpDrive;
    }

    private Quaternion localRotation
    {
      get => Quaternion.Inverse(this.parentRotation) * this.transform.rotation;
    }

    private Quaternion parentRotation
    {
      get
      {
        if ((UnityEngine.Object) this.joint.connectedBody != (UnityEngine.Object) null)
          return this.joint.connectedBody.rotation;
        return (UnityEngine.Object) this.transform.parent == (UnityEngine.Object) null ? Quaternion.identity : this.transform.parent.rotation;
      }
    }

    private Quaternion targetParentRotation
    {
      get
      {
        return (UnityEngine.Object) this.targetParent == (UnityEngine.Object) null ? Quaternion.identity : this.targetParent.rotation;
      }
    }

    private Quaternion targetLocalRotation
    {
      get
      {
        return Quaternion.Inverse(this.targetParentRotation * this.toParentSpace) * this.target.rotation;
      }
    }

    private Quaternion LocalToJointSpace(Quaternion localRotation)
    {
      return this.toJointSpaceInverse * Quaternion.Inverse(localRotation) * this.toJointSpaceDefault;
    }

    private static Vector3 InverseTransformPointUnscaled(
      Vector3 position,
      Quaternion rotation,
      Vector3 point)
    {
      return Quaternion.Inverse(rotation) * (point - position);
    }

    private Vector3 CalculateInertiaTensorCuboid(Vector3 size, float mass)
    {
      float num1 = Mathf.Pow(size.x, 2f);
      float num2 = Mathf.Pow(size.y, 2f);
      float num3 = Mathf.Pow(size.z, 2f);
      float num4 = 0.0833333358f * mass;
      return new Vector3(num4 * (num2 + num3), num4 * (num1 + num3), num4 * (num1 + num2));
    }

    [Serializable]
    public enum Group
    {
      Hips,
      Spine,
      Head,
      Arm,
      Hand,
      Leg,
      Foot,
      Tail,
      Prop,
    }

    [Serializable]
    public class Props
    {
      [Tooltip("Which body part does this muscle belong to?")]
      public Muscle.Group group;
      [Tooltip("The weight (multiplier) of mapping this muscle's target to the muscle.")]
      [Range(0.0f, 1f)]
      public float mappingWeight = 1f;
      [Tooltip("The weight (multiplier) of pinning this muscle to it's target's position using a simple AddForce command.")]
      [Range(0.0f, 1f)]
      public float pinWeight = 1f;
      [Tooltip("The muscle strength (multiplier).")]
      [Range(0.0f, 1f)]
      public float muscleWeight = 1f;
      [Tooltip("Multiplier of the positionDamper of the ConfigurableJoints' Slerp Drive.")]
      [Range(0.0f, 1f)]
      public float muscleDamper = 1f;
      [Tooltip("If true, will map the target to the world space position of the muscle. Normally this should be true for only the root muscle (the hips).")]
      public bool mapPosition;

      public Props()
      {
        this.mappingWeight = 1f;
        this.pinWeight = 1f;
        this.muscleWeight = 1f;
        this.muscleDamper = 1f;
      }

      public Props(
        float pinWeight,
        float muscleWeight,
        float mappingWeight,
        float muscleDamper,
        bool mapPosition,
        Muscle.Group group = Muscle.Group.Hips)
      {
        this.pinWeight = pinWeight;
        this.muscleWeight = muscleWeight;
        this.mappingWeight = mappingWeight;
        this.muscleDamper = muscleDamper;
        this.group = group;
        this.mapPosition = mapPosition;
      }

      public void Clamp()
      {
        this.mappingWeight = Mathf.Clamp(this.mappingWeight, 0.0f, 1f);
        this.pinWeight = Mathf.Clamp(this.pinWeight, 0.0f, 1f);
        this.muscleWeight = Mathf.Clamp(this.muscleWeight, 0.0f, 1f);
        this.muscleDamper = Mathf.Clamp(this.muscleDamper, 0.0f, 1f);
      }
    }

    public struct State
    {
      public float mappingWeightMlp;
      public float pinWeightMlp;
      public float muscleWeightMlp;
      public float maxForceMlp;
      public float muscleDamperMlp;
      public float muscleDamperAdd;
      public float immunity;
      public float impulseMlp;
      public Vector3 velocity;
      public Vector3 angularVelocity;

      public static Muscle.State Default
      {
        get
        {
          return new Muscle.State()
          {
            mappingWeightMlp = 1f,
            pinWeightMlp = 1f,
            muscleWeightMlp = 1f,
            muscleDamperMlp = 1f,
            muscleDamperAdd = 0.0f,
            maxForceMlp = 1f,
            immunity = 0.0f,
            impulseMlp = 1f
          };
        }
      }

      public void Clamp()
      {
        this.mappingWeightMlp = Mathf.Clamp(this.mappingWeightMlp, 0.0f, 1f);
        this.pinWeightMlp = Mathf.Clamp(this.pinWeightMlp, 0.0f, 1f);
        this.muscleWeightMlp = Mathf.Clamp(this.muscleWeightMlp, 0.0f, this.muscleWeightMlp);
        this.immunity = Mathf.Clamp(this.immunity, 0.0f, 1f);
        this.impulseMlp = Mathf.Max(this.impulseMlp, 0.0f);
      }
    }
  }
}
