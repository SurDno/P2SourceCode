using System;

namespace RootMotion.Dynamics
{
  [Serializable]
  public class Muscle
  {
    [HideInInspector]
    public string name;
    public ConfigurableJoint joint;
    public Transform target;
    public Props props = new Props();
    public State state = State.Default;
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

    public Collider[] colliders => _colliders;

    public Vector3 targetVelocity { get; private set; }

    public Vector3 targetAngularVelocity { get; private set; }

    public Vector3 mappedVelocity { get; private set; }

    public Vector3 mappedAngularVelocity { get; private set; }

    public Quaternion targetRotationRelative { get; private set; }

    public bool IsValid(bool log)
    {
      if ((UnityEngine.Object) joint == (UnityEngine.Object) null)
      {
        if (log)
          Debug.LogError((object) "Muscle joint is null");
        return false;
      }
      if ((UnityEngine.Object) target == (UnityEngine.Object) null)
      {
        if (log)
          Debug.LogError((object) ("Muscle " + joint.name + "target is null, please remove the muscle from PuppetMaster or disable PuppetMaster before destroying a muscle's target."));
        return false;
      }
      if (props == null && log)
        Debug.LogError((object) ("Muscle " + joint.name + "props is null"));
      return true;
    }

    public virtual void Initiate(Muscle[] colleagues)
    {
      initiated = false;
      if (!IsValid(true))
        return;
      name = joint.name;
      state = State.Default;
      if ((UnityEngine.Object) joint.connectedBody != (UnityEngine.Object) null)
      {
        for (int index = 0; index < colleagues.Length; ++index)
        {
          if ((UnityEngine.Object) colleagues[index].joint.GetComponent<Rigidbody>() == (UnityEngine.Object) joint.connectedBody)
            connectedBodyTarget = colleagues[index].target;
        }
      }
      transform = joint.transform;
      rigidbody = transform.GetComponent<Rigidbody>();
      rigidbody.isKinematic = false;
      UpdateColliders();
      if (_colliders.Length == 0)
      {
        Vector3 size = Vector3.one * 0.1f;
        Renderer component = transform.GetComponent<Renderer>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          size = component.bounds.size;
        rigidbody.inertiaTensor = CalculateInertiaTensorCuboid(size, rigidbody.mass);
      }
      targetParent = (UnityEngine.Object) connectedBodyTarget != (UnityEngine.Object) null ? connectedBodyTarget : target.parent;
      defaultLocalRotation = localRotation;
      Vector3 normalized1 = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
      Vector3 normalized2 = Vector3.Cross(normalized1, joint.axis).normalized;
      if (normalized1 == normalized2)
      {
        Debug.LogError((object) ("Joint " + joint.name + " secondaryAxis is in the exact same direction as it's axis. Please make sure they are not aligned."));
      }
      else
      {
        rotationRelativeToTarget = Quaternion.Inverse(target.rotation) * transform.rotation;
        Quaternion rotation = Quaternion.LookRotation(normalized1, normalized2);
        toJointSpaceInverse = Quaternion.Inverse(rotation);
        toJointSpaceDefault = defaultLocalRotation * rotation;
        toParentSpace = Quaternion.Inverse(targetParentRotation) * parentRotation;
        localRotationConvert = Quaternion.Inverse(targetLocalRotation) * localRotation;
        if ((UnityEngine.Object) joint.connectedBody != (UnityEngine.Object) null)
        {
          joint.autoConfigureConnectedAnchor = false;
          connectedBodyTransform = joint.connectedBody.transform;
          directTargetParent = (UnityEngine.Object) target.parent == (UnityEngine.Object) connectedBodyTarget;
        }
        angularXMotionDefault = joint.angularXMotion;
        angularYMotionDefault = joint.angularYMotion;
        angularZMotionDefault = joint.angularZMotion;
        if ((UnityEngine.Object) joint.connectedBody == (UnityEngine.Object) null)
          props.mapPosition = true;
        targetRotationRelative = Quaternion.Inverse(rigidbody.rotation) * target.rotation;
        if ((UnityEngine.Object) joint.connectedBody == (UnityEngine.Object) null)
        {
          defaultPosition = transform.localPosition;
          defaultRotation = transform.localRotation;
        }
        else
        {
          defaultPosition = joint.connectedBody.transform.InverseTransformPoint(transform.position);
          defaultRotation = Quaternion.Inverse(joint.connectedBody.transform.rotation) * transform.rotation;
        }
        defaultTargetLocalPosition = target.localPosition;
        defaultTargetLocalRotation = target.localRotation;
        joint.rotationDriveMode = RotationDriveMode.Slerp;
        if (!joint.gameObject.activeInHierarchy)
        {
          Debug.LogError((object) "Can not initiate a puppet that has deactivated muscles.", (UnityEngine.Object) joint.transform);
        }
        else
        {
          joint.configuredInWorldSpace = false;
          joint.projectionMode = JointProjectionMode.None;
          if (joint.anchor != Vector3.zero)
          {
            Debug.LogError((object) ("PuppetMaster joint anchors need to be Vector3.zero. Joint axis on " + transform.name + " is " + (object) joint.anchor), (UnityEngine.Object) transform);
          }
          else
          {
            targetAnimatedPosition = target.position;
            targetAnimatedWorldRotation = target.rotation;
            targetAnimatedRotation = targetLocalRotation * localRotationConvert;
            Read();
            lastReadTime = Time.time;
            lastWriteTime = Time.time;
            lastMappedPosition = target.position;
            lastMappedRotation = target.rotation;
            initiated = true;
          }
        }
      }
    }

    public void UpdateColliders()
    {
      _colliders = new Collider[0];
      AddColliders(joint.transform, ref _colliders, true);
      int childCount = joint.transform.childCount;
      for (int index = 0; index < childCount; ++index)
        AddCompoundColliders(joint.transform.GetChild(index), ref _colliders);
      disabledColliders = new bool[_colliders.Length];
    }

    public void DisableColliders()
    {
      for (int index = 0; index < _colliders.Length; ++index)
      {
        disabledColliders[index] = _colliders[index].enabled;
        _colliders[index].enabled = false;
      }
    }

    public void EnableColliders()
    {
      for (int index = 0; index < _colliders.Length; ++index)
      {
        if (disabledColliders[index])
          _colliders[index].enabled = true;
        disabledColliders[index] = false;
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
      Array.Resize(ref C, length + num1);
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
      AddColliders(t, ref colliders, false);
      int childCount = t.childCount;
      for (int index = 0; index < childCount; ++index)
        AddCompoundColliders(t.GetChild(index), ref colliders);
    }

    public void IgnoreCollisions(Muscle m, bool ignore)
    {
      foreach (Collider collider1 in colliders)
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
      if (!initiated)
        return;
      joint.angularXMotion = ignore ? ConfigurableJointMotion.Free : angularXMotionDefault;
      joint.angularYMotion = ignore ? ConfigurableJointMotion.Free : angularYMotionDefault;
      joint.angularZMotion = ignore ? ConfigurableJointMotion.Free : angularZMotionDefault;
    }

    public void FixTargetTransforms()
    {
      if (!initiated)
        return;
      target.localPosition = defaultTargetLocalPosition;
      target.localRotation = defaultTargetLocalRotation;
    }

    public void Reset()
    {
      if (!initiated || (UnityEngine.Object) joint == (UnityEngine.Object) null)
        return;
      if ((UnityEngine.Object) joint.connectedBody == (UnityEngine.Object) null)
      {
        transform.localPosition = defaultPosition;
        transform.localRotation = defaultRotation;
      }
      else
      {
        transform.position = joint.connectedBody.transform.TransformPoint(defaultPosition);
        transform.rotation = joint.connectedBody.transform.rotation * defaultRotation;
      }
    }

    public void MoveToTarget()
    {
      if (!initiated)
        return;
      transform.position = target.position;
      transform.rotation = target.rotation * rotationRelativeToTarget;
    }

    public void Read()
    {
      float num = Time.time - lastReadTime;
      lastReadTime = Time.time;
      if (num > 0.0)
      {
        targetVelocity = (target.position - targetAnimatedPosition) / num;
        targetAngularVelocity = QuaTools.FromToRotation(targetAnimatedWorldRotation, target.rotation).eulerAngles / num;
      }
      targetAnimatedPosition = target.position;
      targetAnimatedWorldRotation = target.rotation;
      if (!((UnityEngine.Object) joint.connectedBody != (UnityEngine.Object) null))
        return;
      targetAnimatedRotation = targetLocalRotation * localRotationConvert;
    }

    public void ClearVelocities()
    {
      targetVelocity = Vector3.zero;
      targetAngularVelocity = Vector3.zero;
      mappedVelocity = Vector3.zero;
      mappedAngularVelocity = Vector3.zero;
      targetAnimatedPosition = target.position;
      targetAnimatedWorldRotation = target.rotation;
      lastMappedPosition = target.position;
      lastMappedRotation = target.rotation;
    }

    public void UpdateAnchor(bool supportTranslationAnimation)
    {
      if ((UnityEngine.Object) joint.connectedBody == (UnityEngine.Object) null || (UnityEngine.Object) connectedBodyTarget == (UnityEngine.Object) null || directTargetParent && !supportTranslationAnimation)
        return;
      joint.connectedAnchor = (joint.connectedAnchor = InverseTransformPointUnscaled(connectedBodyTarget.position, connectedBodyTarget.rotation * toParentSpace, target.position)) * (1f / connectedBodyTransform.lossyScale.x);
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
      state.velocity = rigidbody.velocity;
      state.angularVelocity = rigidbody.angularVelocity;
      props.Clamp();
      state.Clamp();
      Pin(pinWeightMaster, pinPow, pinDistanceFalloff);
      if (!rotationTargetChanged)
        return;
      MuscleRotation(muscleWeightMaster, muscleSpring, muscleDamper);
    }

    public void Map(float mappingWeightMaster)
    {
      float t = props.mappingWeight * mappingWeightMaster * state.mappingWeightMlp;
      if (t <= 0.0)
        return;
      Vector3 position = transform.position;
      Quaternion rotation = transform.rotation;
      if (t >= 1.0)
      {
        target.rotation = rotation * targetRotationRelative;
        if (!props.mapPosition)
          return;
        target.position = !((UnityEngine.Object) connectedBodyTransform != (UnityEngine.Object) null) ? position : connectedBodyTarget.TransformPoint(connectedBodyTransform.InverseTransformPoint(position));
      }
      else
      {
        target.rotation = Quaternion.Lerp(target.rotation, rotation * targetRotationRelative, t);
        if (!props.mapPosition)
          return;
        target.position = !((UnityEngine.Object) connectedBodyTransform != (UnityEngine.Object) null) ? Vector3.Lerp(target.position, position, t) : Vector3.Lerp(target.position, connectedBodyTarget.TransformPoint(connectedBodyTransform.InverseTransformPoint(position)), t);
      }
    }

    public void CalculateMappedVelocity()
    {
      float num = Time.time - lastWriteTime;
      if (num > 0.0)
      {
        mappedVelocity = (target.position - lastMappedPosition) / num;
        mappedAngularVelocity = QuaTools.FromToRotation(lastMappedRotation, target.rotation).eulerAngles / num;
        lastWriteTime = Time.time;
      }
      lastMappedPosition = target.position;
      lastMappedRotation = target.rotation;
    }

    private void Pin(float pinWeightMaster, float pinPow, float pinDistanceFalloff)
    {
      positionOffset = targetAnimatedPosition - rigidbody.position;
      if (float.IsNaN(positionOffset.x))
        positionOffset = Vector3.zero;
      float f = pinWeightMaster * props.pinWeight * state.pinWeightMlp;
      if (f <= 0.0)
        return;
      Vector3 vector3 = (-rigidbody.velocity + targetVelocity + positionOffset / Time.fixedDeltaTime) * Mathf.Pow(f, pinPow);
      if (pinDistanceFalloff > 0.0)
        vector3 /= (float) (1.0 + (double) positionOffset.sqrMagnitude * pinDistanceFalloff);
      rigidbody.velocity += vector3;
    }

    private void MuscleRotation(float muscleWeightMaster, float muscleSpring, float muscleDamper)
    {
      float a = (float) (muscleWeightMaster * (double) props.muscleWeight * muscleSpring * state.muscleWeightMlp * 10.0);
      if ((UnityEngine.Object) joint.connectedBody == (UnityEngine.Object) null)
        a = 0.0f;
      else if (a > 0.0)
        joint.targetRotation = LocalToJointSpace(targetAnimatedRotation);
      float b = props.muscleDamper * muscleDamper * state.muscleDamperMlp + state.muscleDamperAdd;
      if (a == (double) lastJointDriveRotationWeight && b == (double) lastRotationDamper)
        return;
      lastJointDriveRotationWeight = a;
      lastRotationDamper = b;
      slerpDrive.positionSpring = a;
      slerpDrive.maximumForce = Mathf.Max(a, b) * state.maxForceMlp;
      slerpDrive.positionDamper = b;
      joint.slerpDrive = slerpDrive;
    }

    private Quaternion localRotation
    {
      get => Quaternion.Inverse(parentRotation) * transform.rotation;
    }

    private Quaternion parentRotation
    {
      get
      {
        if ((UnityEngine.Object) joint.connectedBody != (UnityEngine.Object) null)
          return joint.connectedBody.rotation;
        return (UnityEngine.Object) transform.parent == (UnityEngine.Object) null ? Quaternion.identity : transform.parent.rotation;
      }
    }

    private Quaternion targetParentRotation
    {
      get
      {
        return (UnityEngine.Object) targetParent == (UnityEngine.Object) null ? Quaternion.identity : targetParent.rotation;
      }
    }

    private Quaternion targetLocalRotation
    {
      get
      {
        return Quaternion.Inverse(targetParentRotation * toParentSpace) * target.rotation;
      }
    }

    private Quaternion LocalToJointSpace(Quaternion localRotation)
    {
      return toJointSpaceInverse * Quaternion.Inverse(localRotation) * toJointSpaceDefault;
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
      public Group group;
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
        mappingWeight = 1f;
        pinWeight = 1f;
        muscleWeight = 1f;
        muscleDamper = 1f;
      }

      public Props(
        float pinWeight,
        float muscleWeight,
        float mappingWeight,
        float muscleDamper,
        bool mapPosition,
        Group group = Group.Hips)
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
        mappingWeight = Mathf.Clamp(mappingWeight, 0.0f, 1f);
        pinWeight = Mathf.Clamp(pinWeight, 0.0f, 1f);
        muscleWeight = Mathf.Clamp(muscleWeight, 0.0f, 1f);
        muscleDamper = Mathf.Clamp(muscleDamper, 0.0f, 1f);
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

      public static State Default
      {
        get
        {
          return new State {
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
        mappingWeightMlp = Mathf.Clamp(mappingWeightMlp, 0.0f, 1f);
        pinWeightMlp = Mathf.Clamp(pinWeightMlp, 0.0f, 1f);
        muscleWeightMlp = Mathf.Clamp(muscleWeightMlp, 0.0f, muscleWeightMlp);
        immunity = Mathf.Clamp(immunity, 0.0f, 1f);
        impulseMlp = Mathf.Max(impulseMlp, 0.0f);
      }
    }
  }
}
