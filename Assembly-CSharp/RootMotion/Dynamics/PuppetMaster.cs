using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RootMotion.Dynamics
{
  [HelpURL("https://www.youtube.com/watch?v=LYusqeqHAUc")]
  [AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Puppet Master")]
  public class PuppetMaster : MonoBehaviour
  {
    [Tooltip("Humanoid Config allows you to easily share PuppetMaster properties, including individual muscle props between Humanoid puppets.")]
    public PuppetMasterHumanoidConfig humanoidConfig;
    public Transform targetRoot;
    [LargeHeader("Simulation")]
    [Tooltip("Sets/sets the state of the puppet (Alive, Dead or Frozen). Frozen means the ragdoll will be deactivated once it comes to stop in dead state.")]
    public State state;
    [ContextMenuItem("Reset To Default", "ResetStateSettings")]
    [Tooltip("Settings for killing and freezing the puppet.")]
    public StateSettings stateSettings = StateSettings.Default;
    [Tooltip("Active mode means all muscles are active and the character is physically simulated. Kinematic mode sets rigidbody.isKinematic to true for all the muscles and simply updates their position/rotation to match the target's. Disabled mode disables the ragdoll. Switching modes is done by simply changing this value, blending in/out will be handled automatically by the PuppetMaster.")]
    public Mode mode;
    [Tooltip("The time of blending when switching from Active to Kinematic/Disabled or from Kinematic/Disabled to Active. Switching from Kinematic to Disabled or vice versa will be done instantly.")]
    public float blendTime = 0.1f;
    [Tooltip("If true, will fix the target character's Transforms to their default local positions and rotations in each update cycle to avoid drifting from additive reading-writing. Use this only if the target contains unanimated bones.")]
    public bool fixTargetTransforms = true;
    [Tooltip("Rigidbody.solverIterationCount for the muscles of this Puppet.")]
    public int solverIterationCount = 6;
    [Tooltip("If true, will draw the target's pose as green lines in the Scene view. This runs in the Editor only. If you wish to profile PuppetMaster, switch this off.")]
    public bool visualizeTargetPose = true;
    [LargeHeader("Master Weights")]
    [Tooltip("The weight of mapping the animated character to the ragdoll pose.")]
    [Range(0.0f, 1f)]
    public float mappingWeight = 1f;
    [Tooltip("The weight of pinning the muscles to the position of their animated targets using simple AddForce.")]
    [Range(0.0f, 1f)]
    public float pinWeight = 1f;
    [Tooltip("The normalized strength of the muscles.")]
    [Range(0.0f, 1f)]
    public float muscleWeight = 1f;
    [LargeHeader("Joint and Muscle Settings")]
    [Tooltip("The positionSpring of the ConfigurableJoints' Slerp Drive.")]
    public float muscleSpring = 100f;
    [Tooltip("The positionDamper of the ConfigurableJoints' Slerp Drive.")]
    public float muscleDamper;
    [Tooltip("Adjusts the slope of the pinWeight curve. Has effect only while interpolating pinWeight from 0 to 1 and back.")]
    [Range(1f, 8f)]
    public float pinPow = 4f;
    [Tooltip("Reduces pinning force the farther away the target is. Bigger value loosens the pinning, resulting in sloppier behaviour.")]
    [Range(0.0f, 100f)]
    public float pinDistanceFalloff = 5f;
    [Tooltip("When the target has animated bones between the muscle bones, the joint anchors need to be updated in every update cycle because the muscles' targets move relative to each other in position space. This gives much more accurate results, but is computationally expensive so consider leaving it off.")]
    public bool updateJointAnchors = true;
    [Tooltip("Enable this if any of the target's bones has translation animation.")]
    public bool supportTranslationAnimation;
    [Tooltip("Should the joints use angular limits? If the PuppetMaster fails to match the target's pose, it might be because the joint limits are too stiff and do not allow for such motion. Uncheck this to see if the limits are clamping the range of your puppet's animation. Since the joints are actuated, most PuppetMaster simulations will not actually require using joint limits at all.")]
    public bool angularLimits;
    [Tooltip("Should the muscles collide with each other? Consider leaving this off while the puppet is pinned for performance and better accuracy.  Since the joints are actuated, most PuppetMaster simulations will not actually require internal collisions at all.")]
    public bool internalCollisions;
    [LargeHeader("Individual Muscle Settings")]
    [Tooltip("The Muscles managed by this PuppetMaster.")]
    public Muscle[] muscles = [];
    public UpdateDelegate OnPostInitiate;
    public UpdateDelegate OnRead;
    public UpdateDelegate OnWrite;
    public UpdateDelegate OnPostLateUpdate;
    public UpdateDelegate OnFixTransforms;
    public UpdateDelegate OnHierarchyChanged;
    public MuscleDelegate OnMuscleRemoved;
    private Animator _targetAnimator;
    [HideInInspector]
    public List<SolverManager> solvers = [];
    private bool internalCollisionsEnabled = true;
    private bool angularLimitsEnabled = true;
    private bool fixedFrame;
    private int lastSolverIterationCount;
    private bool isLegacy;
    private bool animatorDisabled;
    private bool awakeFailed;
    private bool interpolated;
    private bool freezeFlag;
    private bool hasBeenDisabled;
    private bool hierarchyIsFlat;
    private bool teleport;
    private Vector3 teleportPosition;
    private Quaternion teleportRotation = Quaternion.identity;
    private bool teleportMoveToTarget;
    private Mode activeMode;
    private Mode lastMode;
    private float mappingBlend = 1f;
    public UpdateDelegate OnFreeze;
    public UpdateDelegate OnUnfreeze;
    public UpdateDelegate OnDeath;
    public UpdateDelegate OnResurrection;
    private State activeState;
    private State lastState;
    private bool angularLimitsEnabledOnKill;
    private bool internalCollisionsEnabledOnKill;
    private bool animationDisabledbyStates;
    [HideInInspector]
    public bool storeTargetMappedState = true;
    private Transform[] targetChildren = [];
    private Vector3[] targetMappedPositions;
    private Quaternion[] targetMappedRotations;
    private Vector3[] targetSampledPositions;
    private Quaternion[] targetSampledRotations;
    private bool targetMappedStateStored;
    private bool targetMappedStateSampled;
    private bool sampleTargetMappedState;
    private bool hasProp;

    [ContextMenu("User Manual (Setup)")]
    private void OpenUserManualSetup()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page4.html");
    }

    [ContextMenu("User Manual (Component)")]
    private void OpenUserManualComponent()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page5.html");
    }

    [ContextMenu("User Manual (Performance)")]
    private void OpenUserManualPerformance()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page8.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/class_root_motion_1_1_dynamics_1_1_puppet_master.html");
    }

    [ContextMenu("TUTORIAL VIDEO (SETUP)")]
    private void OpenSetupTutorial()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=mIN9bxJgfOU&index=2&list=PLVxSIA1OaTOuE2SB9NUbckQ9r2hTg4mvL");
    }

    [ContextMenu("TUTORIAL VIDEO (COMPONENT)")]
    private void OpenComponentTutorial()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=LYusqeqHAUc");
    }

    private void ResetStateSettings() => stateSettings = StateSettings.Default;

    public Animator targetAnimator
    {
      get
      {
        if (_targetAnimator == null)
          _targetAnimator = targetRoot.GetComponentInChildren<Animator>();
        if (_targetAnimator == null && targetRoot.parent != null)
          _targetAnimator = targetRoot.parent.GetComponentInChildren<Animator>();
        return _targetAnimator;
      }
      set => _targetAnimator = value;
    }

    public Animation targetAnimation { get; private set; }

    public BehaviourBase[] behaviours { get; private set; }

    public bool isActive => isActiveAndEnabled && initiated && (activeMode == Mode.Active || isBlending);

    public bool initiated { get; private set; }

    public UpdateMode updateMode => targetUpdateMode == AnimatorUpdateMode.AnimatePhysics ? (isLegacy ? UpdateMode.AnimatePhysics : UpdateMode.FixedUpdate) : UpdateMode.Normal;

    public bool controlsAnimator => isActiveAndEnabled && isActive && initiated && updateMode == UpdateMode.FixedUpdate;

    public bool isBlending => isSwitchingMode || isSwitchingState;

    public void Teleport(Vector3 position, Quaternion rotation, bool moveToTarget)
    {
      teleport = true;
      teleportPosition = position;
      teleportRotation = rotation;
      teleportMoveToTarget = moveToTarget;
    }

    private void OnDisable()
    {
      if (!gameObject.activeInHierarchy && initiated && Application.isPlaying)
      {
        foreach (Muscle muscle in muscles)
          muscle.Reset();
      }
      hasBeenDisabled = true;
    }

    private void OnEnable()
    {
      if (!gameObject.activeInHierarchy || !initiated || !hasBeenDisabled || !Application.isPlaying)
        return;
      isSwitchingMode = false;
      activeMode = mode;
      lastMode = mode;
      mappingBlend = mode == Mode.Active ? 1f : 0.0f;
      activeState = state;
      lastState = state;
      isKilling = false;
      freezeFlag = false;
      SetAnimationEnabled(state == State.Alive);
      if (state == State.Alive && targetAnimator != null && mode != Mode.Disabled)
        targetAnimator.Update(1f / 1000f);
      foreach (Muscle muscle in muscles)
      {
        muscle.state.pinWeightMlp = state == State.Alive ? 1f : 0.0f;
        muscle.state.muscleWeightMlp = state == State.Alive ? 1f : stateSettings.deadMuscleWeight;
        muscle.state.muscleDamperAdd = 0.0f;
      }
      if (state != State.Frozen && mode != Mode.Disabled)
      {
        ActivateRagdoll(mode == Mode.Kinematic);
        foreach (Component behaviour in behaviours)
          behaviour.gameObject.SetActive(true);
      }
      else
      {
        foreach (Muscle muscle in muscles)
          muscle.joint.gameObject.SetActive(false);
        if (state == State.Frozen)
        {
          foreach (BehaviourBase behaviour in behaviours)
          {
            if (behaviour.gameObject.activeSelf)
            {
              behaviour.deactivated = true;
              behaviour.gameObject.SetActive(false);
            }
          }
          if (stateSettings.freezePermanently)
          {
            if (behaviours.Length != 0 && behaviours[0] != null)
              Destroy(behaviours[0].transform.parent.gameObject);
            Destroy(gameObject);
            return;
          }
        }
      }
      foreach (BehaviourBase behaviour in behaviours)
        behaviour.OnReactivate();
    }

    private void Awake()
    {
      if (muscles.Length == 0)
        return;
      Initiate();
      if (initiated)
        return;
      awakeFailed = true;
    }

    private void Start()
    {
      if (!initiated && !awakeFailed)
        Initiate();
      if (!initiated)
        return;
      solvers.AddRange(targetRoot.GetComponentsInChildren<SolverManager>());
    }

    public Transform FindTargetRootRecursive(Transform t)
    {
      if (t.parent == null)
        return null;
      foreach (Object @object in t.parent)
      {
        if (@object == transform)
          return t;
      }
      return FindTargetRootRecursive(t.parent);
    }

    private void Initiate()
    {
      initiated = false;
      if (muscles.Length != 0 && muscles[0].target != null && targetRoot == null)
        targetRoot = FindTargetRootRecursive(muscles[0].target);
      if (targetRoot != null && targetAnimator == null)
      {
        targetAnimator = targetRoot.GetComponentInChildren<Animator>();
        if (targetAnimator == null)
          targetAnimation = targetRoot.GetComponentInChildren<Animation>();
      }
      if (!IsValid(true))
        return;
      if (humanoidConfig != null && targetAnimator != null && targetAnimator.isHuman)
        humanoidConfig.ApplyTo(this);
      isLegacy = targetAnimator == null && targetAnimation != null;
      behaviours = transform.GetComponentsInChildren<BehaviourBase>();
      if (behaviours.Length == 0 && transform.parent != null)
        behaviours = transform.parent.GetComponentsInChildren<BehaviourBase>();
      for (int index = 0; index < muscles.Length; ++index)
      {
        muscles[index].Initiate(muscles);
        if (behaviours.Length != 0)
        {
          muscles[index].broadcaster = muscles[index].joint.gameObject.GetComponent<MuscleCollisionBroadcaster>();
          if (muscles[index].broadcaster == null)
            muscles[index].broadcaster = muscles[index].joint.gameObject.AddComponent<MuscleCollisionBroadcaster>();
          muscles[index].broadcaster.puppetMaster = this;
          muscles[index].broadcaster.muscleIndex = index;
        }
        if (double.PositiveInfinity != muscles[index].joint.breakForce)
        {
          muscles[index].jointBreakBroadcaster = muscles[index].joint.gameObject.GetComponent<JointBreakBroadcaster>();
          if (muscles[index].jointBreakBroadcaster == null)
            muscles[index].jointBreakBroadcaster = muscles[index].joint.gameObject.AddComponent<JointBreakBroadcaster>();
          muscles[index].jointBreakBroadcaster.puppetMaster = this;
          muscles[index].jointBreakBroadcaster.muscleIndex = index;
        }
      }
      UpdateHierarchies();
      hierarchyIsFlat = HierarchyIsFlat();
      initiated = true;
      foreach (BehaviourBase behaviour in behaviours)
        behaviour.puppetMaster = this;
      foreach (BehaviourBase behaviour in behaviours)
        behaviour.Initiate();
      SwitchStates();
      SwitchModes();
      foreach (Muscle muscle in muscles)
        muscle.Read();
      StoreTargetMappedState();
      if (Singleton<PuppetMasterSettings>.instance != null)
        Singleton<PuppetMasterSettings>.instance.Register(this);
      bool flag = false;
      foreach (BehaviourBase behaviour in behaviours)
      {
        if (behaviour is BehaviourPuppet && behaviour.enabled)
        {
          ActivateBehaviour(behaviour);
          flag = true;
          break;
        }
      }
      if (!flag && behaviours.Length != 0)
      {
        foreach (BehaviourBase behaviour in behaviours)
        {
          if (behaviour.enabled)
          {
            ActivateBehaviour(behaviour);
            break;
          }
        }
      }
      if (OnPostInitiate == null)
        return;
      OnPostInitiate();
    }

    private void ActivateBehaviour(BehaviourBase behaviour)
    {
      foreach (BehaviourBase behaviour1 in behaviours)
      {
        behaviour1.enabled = behaviour1 == behaviour;
        if (behaviour1.enabled)
          behaviour1.Activate();
      }
    }

    private void OnDestroy()
    {
      if (!(Singleton<PuppetMasterSettings>.instance != null))
        return;
      Singleton<PuppetMasterSettings>.instance.Unregister(this);
    }

    private bool IsInterpolated()
    {
      if (!initiated)
        return false;
      foreach (Muscle muscle in muscles)
      {
        if (muscle.rigidbody.interpolation != 0)
          return true;
      }
      return false;
    }

    protected virtual void FixedUpdate()
    {
      if (!initiated || muscles.Length == 0)
        return;
      interpolated = IsInterpolated();
      fixedFrame = true;
      if (!isActive)
        return;
      pinWeight = Mathf.Clamp(pinWeight, 0.0f, 1f);
      muscleWeight = Mathf.Clamp(muscleWeight, 0.0f, 1f);
      muscleSpring = Mathf.Clamp(muscleSpring, 0.0f, muscleSpring);
      muscleDamper = Mathf.Clamp(muscleDamper, 0.0f, muscleDamper);
      pinPow = Mathf.Clamp(pinPow, 1f, 8f);
      pinDistanceFalloff = Mathf.Max(pinDistanceFalloff, 0.0f);
      if (updateMode == UpdateMode.FixedUpdate)
      {
        FixTargetTransforms();
        if (targetAnimator.enabled || !targetAnimator.enabled && animatorDisabled)
        {
          targetAnimator.enabled = false;
          animatorDisabled = true;
          targetAnimator.Update(Time.fixedDeltaTime);
        }
        else
        {
          animatorDisabled = false;
          targetAnimator.enabled = false;
        }
        foreach (SolverManager solver in solvers)
        {
          if (solver != null)
            solver.UpdateSolverExternal();
        }
        Read();
      }
      if (!isFrozen)
      {
        SetInternalCollisions(internalCollisions);
        SetAngularLimits(angularLimits);
        if (solverIterationCount != lastSolverIterationCount)
        {
          for (int index = 0; index < muscles.Length; ++index)
            muscles[index].rigidbody.solverIterations = solverIterationCount;
          lastSolverIterationCount = solverIterationCount;
        }
        for (int index = 0; index < muscles.Length; ++index)
          muscles[index].Update(pinWeight, muscleWeight, muscleSpring, muscleDamper, pinPow, pinDistanceFalloff, true);
      }
      if (updateMode != UpdateMode.AnimatePhysics)
        return;
      FixTargetTransforms();
    }

    protected virtual void Update()
    {
      if (!initiated || muscles.Length == 0)
        return;
      if (animatorDisabled)
      {
        targetAnimator.enabled = true;
        animatorDisabled = false;
      }
      if (updateMode != 0)
        return;
      FixTargetTransforms();
    }

    protected virtual void LateUpdate()
    {
      if (muscles.Length == 0)
        return;
      OnLateUpdate();
      if (OnPostLateUpdate == null)
        return;
      OnPostLateUpdate();
    }

    protected virtual void OnLateUpdate()
    {
      if (!initiated || mode == Mode.Disabled)
        return;
      if (animatorDisabled)
      {
        targetAnimator.enabled = true;
        animatorDisabled = false;
      }
      SwitchStates();
      SwitchModes();
      switch (updateMode)
      {
        case UpdateMode.Normal:
          if (isActive)
          {
            Read();
          }
          break;
        case UpdateMode.AnimatePhysics:
          if (!fixedFrame && !interpolated)
            return;
          if (isActive && !fixedFrame)
          {
            Read();
          }
          break;
        case UpdateMode.FixedUpdate:
          if (!fixedFrame && !interpolated)
            return;
          break;
      }
      fixedFrame = false;
      if (!isFrozen)
      {
        mappingWeight = Mathf.Clamp(mappingWeight, 0.0f, 1f);
        float mappingWeightMaster = mappingWeight * mappingBlend;
        if (mappingWeightMaster > 0.0)
        {
          if (isActive)
          {
            for (int index = 0; index < muscles.Length; ++index)
              muscles[index].Map(mappingWeightMaster);
          }
        }
        else if (activeMode == Mode.Kinematic)
          MoveToTarget();
        foreach (BehaviourBase behaviour in behaviours)
          behaviour.OnWrite();
        if (OnWrite != null)
          OnWrite();
        StoreTargetMappedState();
        foreach (Muscle muscle in muscles)
          muscle.CalculateMappedVelocity();
      }
      if (!freezeFlag)
        return;
      OnFreezeFlag();
    }

    private void MoveToTarget()
    {
      if (!(Singleton<PuppetMasterSettings>.instance == null) && (!(Singleton<PuppetMasterSettings>.instance != null) || !Singleton<PuppetMasterSettings>.instance.UpdateMoveToTarget(this)))
        return;
      foreach (Muscle muscle in muscles)
        muscle.MoveToTarget();
    }

    private void Read()
    {
      if (teleport)
      {
        GameObject gameObject = new GameObject();
        gameObject.transform.position = transform.parent != null ? transform.parent.position : Vector3.zero;
        gameObject.transform.rotation = transform.parent != null ? transform.parent.rotation : Quaternion.identity;
        Transform parent1 = transform.parent;
        Transform parent2 = targetRoot.parent;
        transform.parent = gameObject.transform;
        targetRoot.parent = gameObject.transform;
        Vector3 position = transform.parent.position;
        Quaternion rotation = QuaTools.FromToRotation(targetRoot.rotation, teleportRotation);
        transform.parent.rotation = rotation * transform.parent.rotation;
        Vector3 deltaPosition = teleportPosition - targetRoot.position;
        transform.parent.position += deltaPosition;
        transform.parent = parent1;
        targetRoot.parent = parent2;
        Destroy(gameObject);
        targetMappedPositions[0] = position + rotation * (targetMappedPositions[0] - position) + deltaPosition;
        targetSampledPositions[0] = position + rotation * (targetSampledPositions[0] - position) + deltaPosition;
        targetMappedRotations[0] = rotation * targetMappedRotations[0];
        targetSampledRotations[0] = rotation * targetSampledRotations[0];
        if (teleportMoveToTarget)
        {
          foreach (Muscle muscle in muscles)
            muscle.MoveToTarget();
        }
        foreach (Muscle muscle in muscles)
          muscle.ClearVelocities();
        foreach (BehaviourBase behaviour in behaviours)
          behaviour.OnTeleport(rotation, deltaPosition, position, teleportMoveToTarget);
        teleport = false;
      }
      if (OnRead != null)
        OnRead();
      foreach (BehaviourBase behaviour in behaviours)
        behaviour.OnRead();
      if (!isAlive)
        return;
      foreach (Muscle muscle in muscles)
        muscle.Read();
      if (!isAlive || !updateJointAnchors)
        return;
      for (int index = 0; index < muscles.Length; ++index)
        muscles[index].UpdateAnchor(supportTranslationAnimation);
    }

    private void FixTargetTransforms()
    {
      if (!isAlive)
        return;
      if (OnFixTransforms != null)
        OnFixTransforms();
      foreach (BehaviourBase behaviour in behaviours)
        behaviour.OnFixTransforms();
      if (!fixTargetTransforms && !hasProp || !isActive)
        return;
      mappingWeight = Mathf.Clamp(mappingWeight, 0.0f, 1f);
      if (mappingWeight * mappingBlend <= 0.0)
        return;
      for (int index = 0; index < muscles.Length; ++index)
      {
        if (fixTargetTransforms || muscles[index].props.group == Muscle.Group.Prop)
          muscles[index].FixTargetTransforms();
      }
    }

    private AnimatorUpdateMode targetUpdateMode
    {
      get
      {
        if (targetAnimator != null)
          return targetAnimator.updateMode;
        return targetAnimation != null ? (targetAnimation.animatePhysics ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal) : AnimatorUpdateMode.Normal;
      }
    }

    private void VisualizeTargetPose()
    {
      if (!visualizeTargetPose || !Application.isEditor || !isActive)
        return;
      foreach (Muscle muscle1 in muscles)
      {
        if (muscle1.joint.connectedBody != null && muscle1.connectedBodyTarget != null)
        {
          Debug.DrawLine(muscle1.target.position, muscle1.connectedBodyTarget.position, Color.cyan);
          bool flag = true;
          foreach (Muscle muscle2 in muscles)
          {
            if (muscle1 != muscle2 && muscle2.joint.connectedBody == muscle1.rigidbody)
            {
              flag = false;
              break;
            }
          }
          if (flag)
            VisualizeHierarchy(muscle1.target, Color.cyan);
        }
      }
    }

    private void VisualizeHierarchy(Transform t, Color color)
    {
      for (int index = 0; index < t.childCount; ++index)
      {
        Debug.DrawLine(t.position, t.GetChild(index).position, color);
        VisualizeHierarchy(t.GetChild(index), color);
      }
    }

    private void SetInternalCollisions(bool collide)
    {
      if (internalCollisionsEnabled == collide)
        return;
      for (int index1 = 0; index1 < muscles.Length; ++index1)
      {
        for (int index2 = index1; index2 < muscles.Length; ++index2)
        {
          if (index1 != index2)
            muscles[index1].IgnoreCollisions(muscles[index2], !collide);
        }
      }
      internalCollisionsEnabled = collide;
    }

    private void SetAngularLimits(bool limited)
    {
      if (angularLimitsEnabled == limited)
        return;
      for (int index = 0; index < muscles.Length; ++index)
        muscles[index].IgnoreAngularLimits(!limited);
      angularLimitsEnabled = limited;
    }

    public void AddMuscle(
      ConfigurableJoint joint,
      Transform target,
      Rigidbody connectTo,
      Transform targetParent,
      Muscle.Props muscleProps = null,
      bool forceTreeHierarchy = false,
      bool forceLayers = true)
    {
      if (!CheckIfInitiated())
        return;
      if (!initiated)
        Debug.LogWarning("PuppetMaster has not been initiated.", transform);
      else if (ContainsJoint(joint))
        Debug.LogWarning("Joint " + joint.name + " is already used by a Muscle", transform);
      else if (target == null)
        Debug.LogWarning("AddMuscle was called with a null 'target' reference.", transform);
      else if (connectTo == joint.GetComponent<Rigidbody>())
        Debug.LogWarning("ConnectTo is the joint's own Rigidbody, can not add muscle.", transform);
      else if (!isActive)
      {
        Debug.LogWarning("Adding muscles to inactive PuppetMasters is not currently supported.", transform);
      }
      else
      {
        if (muscleProps == null)
          muscleProps = new Muscle.Props();
        Muscle m = new Muscle();
        m.props = muscleProps;
        m.joint = joint;
        m.target = target;
        m.joint.transform.parent = !hierarchyIsFlat && !(connectTo == null) || forceTreeHierarchy ? connectTo.transform : transform;
        if (forceLayers)
        {
          joint.gameObject.layer = gameObject.layer;
          target.gameObject.layer = targetRoot.gameObject.layer;
        }
        if (connectTo != null)
        {
          m.target.parent = targetParent;
          Vector3 position = GetMuscle(connectTo).transform.InverseTransformPoint(m.target.position);
          Quaternion quaternion = Quaternion.Inverse(GetMuscle(connectTo).transform.rotation) * m.target.rotation;
          joint.transform.position = connectTo.transform.TransformPoint(position);
          joint.transform.rotation = connectTo.transform.rotation * quaternion;
          joint.connectedBody = connectTo;
        }
        m.Initiate(muscles);
        if (connectTo != null)
        {
          m.rigidbody.velocity = connectTo.velocity;
          m.rigidbody.angularVelocity = connectTo.angularVelocity;
        }
        if (!internalCollisions)
        {
          for (int index = 0; index < muscles.Length; ++index)
            m.IgnoreCollisions(muscles[index], true);
        }
        Array.Resize(ref muscles, muscles.Length + 1);
        muscles[muscles.Length - 1] = m;
        m.IgnoreAngularLimits(!angularLimits);
        if (behaviours.Length != 0)
        {
          m.broadcaster = m.joint.gameObject.AddComponent<MuscleCollisionBroadcaster>();
          m.broadcaster.puppetMaster = this;
          m.broadcaster.muscleIndex = muscles.Length - 1;
        }
        m.jointBreakBroadcaster = m.joint.gameObject.AddComponent<JointBreakBroadcaster>();
        m.jointBreakBroadcaster.puppetMaster = this;
        m.jointBreakBroadcaster.muscleIndex = muscles.Length - 1;
        UpdateHierarchies();
        CheckMassVariation(100f, true);
        foreach (BehaviourBase behaviour in behaviours)
          behaviour.OnMuscleAdded(m);
      }
    }

    public void RemoveMuscleRecursive(
      ConfigurableJoint joint,
      bool attachTarget,
      bool blockTargetAnimation = false,
      MuscleRemoveMode removeMode = MuscleRemoveMode.Sever)
    {
      if (!CheckIfInitiated())
        return;
      if (joint == null)
        Debug.LogWarning("RemoveMuscleRecursive was called with a null 'joint' reference.", transform);
      else if (!ContainsJoint(joint))
      {
        Debug.LogWarning("No Muscle with the specified joint was found, can not remove muscle.", transform);
      }
      else
      {
        int muscleIndex = GetMuscleIndex(joint);
        Muscle[] muscleArray = new Muscle[muscles.Length - (muscles[muscleIndex].childIndexes.Length + 1)];
        int index1 = 0;
        for (int index2 = 0; index2 < muscles.Length; ++index2)
        {
          if (index2 != muscleIndex && !muscles[muscleIndex].childFlags[index2])
          {
            muscleArray[index1] = muscles[index2];
            ++index1;
          }
          else
          {
            if (muscles[index2].broadcaster != null)
            {
              muscles[index2].broadcaster.enabled = false;
              Destroy(muscles[index2].broadcaster);
            }
            if (muscles[index2].jointBreakBroadcaster != null)
            {
              muscles[index2].jointBreakBroadcaster.enabled = false;
              Destroy(muscles[index2].jointBreakBroadcaster);
            }
          }
        }
        switch (removeMode)
        {
          case MuscleRemoveMode.Sever:
            DisconnectJoint(muscles[muscleIndex].joint);
            for (int index3 = 0; index3 < muscles[muscleIndex].childIndexes.Length; ++index3)
              KillJoint(muscles[muscles[muscleIndex].childIndexes[index3]].joint);
            break;
          case MuscleRemoveMode.Explode:
            DisconnectJoint(muscles[muscleIndex].joint);
            for (int index4 = 0; index4 < muscles[muscleIndex].childIndexes.Length; ++index4)
              DisconnectJoint(muscles[muscles[muscleIndex].childIndexes[index4]].joint);
            break;
          case MuscleRemoveMode.Numb:
            KillJoint(muscles[muscleIndex].joint);
            for (int index5 = 0; index5 < muscles[muscleIndex].childIndexes.Length; ++index5)
              KillJoint(muscles[muscles[muscleIndex].childIndexes[index5]].joint);
            break;
        }
        muscles[muscleIndex].transform.parent = null;
        for (int index6 = 0; index6 < muscles[muscleIndex].childIndexes.Length; ++index6)
        {
          if (removeMode == MuscleRemoveMode.Explode || muscles[muscles[muscleIndex].childIndexes[index6]].transform.parent == transform)
            muscles[muscles[muscleIndex].childIndexes[index6]].transform.parent = null;
        }
        foreach (BehaviourBase behaviour in behaviours)
        {
          behaviour.OnMuscleRemoved(muscles[muscleIndex]);
          for (int index7 = 0; index7 < muscles[muscleIndex].childIndexes.Length; ++index7)
          {
            Muscle muscle = muscles[muscles[muscleIndex].childIndexes[index7]];
            behaviour.OnMuscleRemoved(muscle);
          }
        }
        if (attachTarget)
        {
          muscles[muscleIndex].target.parent = muscles[muscleIndex].transform;
          muscles[muscleIndex].target.position = muscles[muscleIndex].transform.position;
          muscles[muscleIndex].target.rotation = muscles[muscleIndex].transform.rotation * muscles[muscleIndex].targetRotationRelative;
          for (int index8 = 0; index8 < muscles[muscleIndex].childIndexes.Length; ++index8)
          {
            Muscle muscle = muscles[muscles[muscleIndex].childIndexes[index8]];
            muscle.target.parent = muscle.transform;
            muscle.target.position = muscle.transform.position;
            muscle.target.rotation = muscle.transform.rotation;
          }
        }
        if (blockTargetAnimation)
        {
          muscles[muscleIndex].target.gameObject.AddComponent<AnimationBlocker>();
          for (int index9 = 0; index9 < muscles[muscleIndex].childIndexes.Length; ++index9)
            muscles[muscles[muscleIndex].childIndexes[index9]].target.gameObject.AddComponent<AnimationBlocker>();
        }
        if (OnMuscleRemoved != null)
          OnMuscleRemoved(muscles[muscleIndex]);
        for (int index10 = 0; index10 < muscles[muscleIndex].childIndexes.Length; ++index10)
        {
          Muscle muscle = muscles[muscles[muscleIndex].childIndexes[index10]];
          if (OnMuscleRemoved != null)
            OnMuscleRemoved(muscle);
        }
        if (!internalCollisionsEnabled)
        {
          foreach (Muscle muscle in muscleArray)
          {
            foreach (Collider collider1 in muscle.colliders)
            {
              foreach (Collider collider2 in muscles[muscleIndex].colliders)
                Physics.IgnoreCollision(collider1, collider2, false);
              for (int index11 = 0; index11 < muscles[muscleIndex].childIndexes.Length; ++index11)
              {
                foreach (Collider collider3 in muscles[index11].colliders)
                  Physics.IgnoreCollision(collider1, collider3, false);
              }
            }
          }
        }
        muscles = muscleArray;
        UpdateHierarchies();
      }
    }

    public void ReplaceMuscle(ConfigurableJoint oldJoint, ConfigurableJoint newJoint)
    {
      if (!CheckIfInitiated())
        return;
      Debug.LogWarning("@todo", transform);
    }

    public void SetMuscles(Muscle[] newMuscles)
    {
      if (!CheckIfInitiated())
        return;
      Debug.LogWarning("@todo", transform);
    }

    public void DisableMuscleRecursive(ConfigurableJoint joint)
    {
      if (!CheckIfInitiated())
        return;
      Debug.LogWarning("@todo", transform);
    }

    public void EnableMuscleRecursive(ConfigurableJoint joint)
    {
      if (!CheckIfInitiated())
        return;
      Debug.LogWarning("@todo", transform);
    }

    [ContextMenu("Flatten Muscle Hierarchy")]
    public void FlattenHierarchy()
    {
      foreach (Muscle muscle in muscles)
      {
        if (muscle.joint != null)
          muscle.joint.transform.parent = transform;
      }
      hierarchyIsFlat = true;
    }

    [ContextMenu("Tree Muscle Hierarchy")]
    public void TreeHierarchy()
    {
      foreach (Muscle muscle in muscles)
      {
        if (muscle.joint != null)
          muscle.joint.transform.parent = muscle.joint.connectedBody != null ? muscle.joint.connectedBody.transform : transform;
      }
      hierarchyIsFlat = false;
    }

    [ContextMenu("Fix Muscle Positions")]
    public void FixMusclePositions()
    {
      foreach (Muscle muscle in muscles)
      {
        if (muscle.joint != null && muscle.target != null)
          muscle.joint.transform.position = muscle.target.position;
      }
    }

    private void AddIndexesRecursive(int index, ref int[] indexes)
    {
      int length = indexes.Length;
      Array.Resize(ref indexes, indexes.Length + 1 + muscles[index].childIndexes.Length);
      indexes[length] = index;
      if (muscles[index].childIndexes.Length == 0)
        return;
      for (int index1 = 0; index1 < muscles[index].childIndexes.Length; ++index1)
        AddIndexesRecursive(muscles[index].childIndexes[index1], ref indexes);
    }

    private bool HierarchyIsFlat()
    {
      foreach (Muscle muscle in muscles)
      {
        if (muscle.joint.transform.parent != transform)
          return false;
      }
      return true;
    }

    private void DisconnectJoint(ConfigurableJoint joint)
    {
      joint.connectedBody = null;
      KillJoint(joint);
      joint.xMotion = ConfigurableJointMotion.Free;
      joint.yMotion = ConfigurableJointMotion.Free;
      joint.zMotion = ConfigurableJointMotion.Free;
      joint.angularXMotion = ConfigurableJointMotion.Free;
      joint.angularYMotion = ConfigurableJointMotion.Free;
      joint.angularZMotion = ConfigurableJointMotion.Free;
    }

    private void KillJoint(ConfigurableJoint joint)
    {
      joint.targetRotation = Quaternion.identity;
      joint.slerpDrive = new JointDrive {
        positionSpring = 0.0f,
        positionDamper = 0.0f
      };
    }

    public bool isSwitchingMode { get; private set; }

    public void DisableImmediately()
    {
      mappingBlend = 0.0f;
      isSwitchingMode = false;
      mode = Mode.Disabled;
      activeMode = mode;
      lastMode = mode;
      foreach (Muscle muscle in muscles)
        muscle.rigidbody.gameObject.SetActive(false);
    }

    protected virtual void SwitchModes()
    {
      if (!initiated)
        return;
      if (isKilling)
        mode = Mode.Active;
      if (!isAlive)
        mode = Mode.Active;
      foreach (BehaviourBase behaviour in behaviours)
      {
        if (behaviour.forceActive)
        {
          mode = Mode.Active;
          break;
        }
      }
      if (mode == lastMode || isSwitchingMode || isKilling && mode != 0 || state != State.Alive && mode != 0)
        return;
      isSwitchingMode = true;
      if (lastMode == Mode.Disabled)
      {
        if (mode == Mode.Kinematic)
          DisabledToKinematic();
        else if (mode == Mode.Active)
          StartCoroutine(DisabledToActive());
      }
      else if (lastMode == Mode.Kinematic)
      {
        if (mode == Mode.Disabled)
          KinematicToDisabled();
        else if (mode == Mode.Active)
          StartCoroutine(KinematicToActive());
      }
      else if (lastMode == Mode.Active)
      {
        if (mode == Mode.Disabled)
          StartCoroutine(ActiveToDisabled());
        else if (mode == Mode.Kinematic)
          StartCoroutine(ActiveToKinematic());
      }
      lastMode = mode;
    }

    private void DisabledToKinematic()
    {
      foreach (Muscle muscle in muscles)
        muscle.Reset();
      foreach (Muscle muscle in muscles)
      {
        muscle.rigidbody.gameObject.SetActive(true);
        muscle.rigidbody.isKinematic = true;
      }
      internalCollisionsEnabled = true;
      SetInternalCollisions(internalCollisions);
      foreach (Muscle muscle in muscles)
        muscle.MoveToTarget();
      activeMode = Mode.Kinematic;
      isSwitchingMode = false;
    }

    private IEnumerator DisabledToActive()
    {
      Muscle[] muscleArray1 = muscles;
      for (int index = 0; index < muscleArray1.Length; ++index)
      {
        Muscle m = muscleArray1[index];
        if (!m.rigidbody.gameObject.activeInHierarchy)
          m.Reset();
        m = null;
      }
      muscleArray1 = null;
      Muscle[] muscleArray2 = muscles;
      for (int index = 0; index < muscleArray2.Length; ++index)
      {
        Muscle m = muscleArray2[index];
        m.rigidbody.gameObject.SetActive(true);
        m.rigidbody.isKinematic = false;
        m.rigidbody.WakeUp();
        m.rigidbody.velocity = m.mappedVelocity;
        m.rigidbody.angularVelocity = m.mappedAngularVelocity;
        m = null;
      }
      muscleArray2 = null;
      internalCollisionsEnabled = true;
      SetInternalCollisions(internalCollisions);
      Read();
      Muscle[] muscleArray3 = muscles;
      for (int index = 0; index < muscleArray3.Length; ++index)
      {
        Muscle m = muscleArray3[index];
        m.MoveToTarget();
        m = null;
      }
      muscleArray3 = null;
      UpdateInternalCollisions();
      while (mappingBlend < 1.0)
      {
        mappingBlend = Mathf.Clamp(mappingBlend + Time.deltaTime / blendTime, 0.0f, 1f);
        yield return null;
      }
      activeMode = Mode.Active;
      isSwitchingMode = false;
    }

    private void KinematicToDisabled()
    {
      foreach (Muscle muscle in muscles)
        muscle.rigidbody.gameObject.SetActive(false);
      activeMode = Mode.Disabled;
      isSwitchingMode = false;
    }

    private IEnumerator KinematicToActive()
    {
      Muscle[] muscleArray1 = muscles;
      for (int index = 0; index < muscleArray1.Length; ++index)
      {
        Muscle m = muscleArray1[index];
        m.rigidbody.isKinematic = false;
        m.rigidbody.WakeUp();
        m.rigidbody.velocity = m.mappedVelocity;
        m.rigidbody.angularVelocity = m.mappedAngularVelocity;
        m = null;
      }
      muscleArray1 = null;
      Read();
      Muscle[] muscleArray2 = muscles;
      for (int index = 0; index < muscleArray2.Length; ++index)
      {
        Muscle m = muscleArray2[index];
        m.MoveToTarget();
        m = null;
      }
      muscleArray2 = null;
      UpdateInternalCollisions();
      while (mappingBlend < 1.0)
      {
        mappingBlend = Mathf.Min(mappingBlend + Time.deltaTime / blendTime, 1f);
        yield return null;
      }
      activeMode = Mode.Active;
      isSwitchingMode = false;
    }

    private IEnumerator ActiveToDisabled()
    {
      while (mappingBlend > 0.0)
      {
        mappingBlend = Mathf.Max(mappingBlend - Time.deltaTime / blendTime, 0.0f);
        yield return null;
      }
      Muscle[] muscleArray = muscles;
      for (int index = 0; index < muscleArray.Length; ++index)
      {
        Muscle m = muscleArray[index];
        m.rigidbody.gameObject.SetActive(false);
        m = null;
      }
      muscleArray = null;
      activeMode = Mode.Disabled;
      isSwitchingMode = false;
    }

    private IEnumerator ActiveToKinematic()
    {
      while (mappingBlend > 0.0)
      {
        mappingBlend = Mathf.Max(mappingBlend - Time.deltaTime / blendTime, 0.0f);
        yield return null;
      }
      Muscle[] muscleArray1 = muscles;
      for (int index = 0; index < muscleArray1.Length; ++index)
      {
        Muscle m = muscleArray1[index];
        m.rigidbody.isKinematic = true;
        m = null;
      }
      muscleArray1 = null;
      Muscle[] muscleArray2 = muscles;
      for (int index = 0; index < muscleArray2.Length; ++index)
      {
        Muscle m = muscleArray2[index];
        m.MoveToTarget();
        m = null;
      }
      muscleArray2 = null;
      activeMode = Mode.Kinematic;
      isSwitchingMode = false;
    }

    private void UpdateInternalCollisions()
    {
      if (internalCollisions)
        return;
      for (int index1 = 0; index1 < muscles.Length; ++index1)
      {
        for (int index2 = index1; index2 < muscles.Length; ++index2)
        {
          if (index1 != index2)
            muscles[index1].IgnoreCollisions(muscles[index2], true);
        }
      }
    }

    public void SetMuscleWeights(
      Muscle.Group group,
      float muscleWeight,
      float pinWeight = 1f,
      float mappingWeight = 1f,
      float muscleDamper = 1f)
    {
      if (!CheckIfInitiated())
        return;
      foreach (Muscle muscle in muscles)
      {
        if (muscle.props.group == group)
        {
          muscle.props.muscleWeight = muscleWeight;
          muscle.props.pinWeight = pinWeight;
          muscle.props.mappingWeight = mappingWeight;
          muscle.props.muscleDamper = muscleDamper;
        }
      }
    }

    public void SetMuscleWeights(
      Transform target,
      float muscleWeight,
      float pinWeight = 1f,
      float mappingWeight = 1f,
      float muscleDamper = 1f)
    {
      if (!CheckIfInitiated())
        return;
      int muscleIndex = GetMuscleIndex(target);
      if (muscleIndex == -1)
        return;
      SetMuscleWeights(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
    }

    public void SetMuscleWeights(
      HumanBodyBones humanBodyBone,
      float muscleWeight,
      float pinWeight = 1f,
      float mappingWeight = 1f,
      float muscleDamper = 1f)
    {
      if (!CheckIfInitiated())
        return;
      int muscleIndex = GetMuscleIndex(humanBodyBone);
      if (muscleIndex == -1)
        return;
      SetMuscleWeights(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
    }

    public void SetMuscleWeightsRecursive(
      Transform target,
      float muscleWeight,
      float pinWeight = 1f,
      float mappingWeight = 1f,
      float muscleDamper = 1f)
    {
      if (!CheckIfInitiated())
        return;
      for (int muscleIndex = 0; muscleIndex < muscles.Length; ++muscleIndex)
      {
        if (muscles[muscleIndex].target == target)
        {
          SetMuscleWeightsRecursive(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
          break;
        }
      }
    }

    public void SetMuscleWeightsRecursive(
      int muscleIndex,
      float muscleWeight,
      float pinWeight = 1f,
      float mappingWeight = 1f,
      float muscleDamper = 1f)
    {
      if (!CheckIfInitiated())
        return;
      SetMuscleWeights(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
      for (int index = 0; index < muscles[muscleIndex].childIndexes.Length; ++index)
        SetMuscleWeights(muscles[muscleIndex].childIndexes[index], muscleWeight, pinWeight, mappingWeight, muscleDamper);
    }

    public void SetMuscleWeightsRecursive(
      HumanBodyBones humanBodyBone,
      float muscleWeight,
      float pinWeight = 1f,
      float mappingWeight = 1f,
      float muscleDamper = 1f)
    {
      if (!CheckIfInitiated())
        return;
      int muscleIndex = GetMuscleIndex(humanBodyBone);
      if (muscleIndex == -1)
        return;
      SetMuscleWeightsRecursive(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
    }

    public void SetMuscleWeights(
      int muscleIndex,
      float muscleWeight,
      float pinWeight,
      float mappingWeight,
      float muscleDamper)
    {
      if (!CheckIfInitiated())
        return;
      if (muscleIndex < 0.0 || muscleIndex >= muscles.Length)
      {
        Debug.LogWarning("Muscle index out of range (" + muscleIndex + ").", transform);
      }
      else
      {
        muscles[muscleIndex].props.muscleWeight = muscleWeight;
        muscles[muscleIndex].props.pinWeight = pinWeight;
        muscles[muscleIndex].props.mappingWeight = mappingWeight;
        muscles[muscleIndex].props.muscleDamper = muscleDamper;
      }
    }

    public Muscle GetMuscle(Transform target)
    {
      int muscleIndex = GetMuscleIndex(target);
      return muscleIndex == -1 ? null : muscles[muscleIndex];
    }

    public Muscle GetMuscle(Rigidbody rigidbody)
    {
      int muscleIndex = GetMuscleIndex(rigidbody);
      return muscleIndex == -1 ? null : muscles[muscleIndex];
    }

    public Muscle GetMuscle(ConfigurableJoint joint)
    {
      int muscleIndex = GetMuscleIndex(joint);
      return muscleIndex == -1 ? null : muscles[muscleIndex];
    }

    public bool ContainsJoint(ConfigurableJoint joint)
    {
      if (!CheckIfInitiated())
        return false;
      foreach (Muscle muscle in muscles)
      {
        if (muscle.joint == joint)
          return true;
      }
      return false;
    }

    public int GetMuscleIndex(HumanBodyBones humanBodyBone)
    {
      if (!CheckIfInitiated())
        return -1;
      if (targetAnimator == null)
      {
        Debug.LogWarning("PuppetMaster 'Target Root' has no Animator component on it nor on it's children.", transform);
        return -1;
      }
      if (!targetAnimator.isHuman)
      {
        Debug.LogWarning("PuppetMaster target's Animator does not belong to a Humanoid, can hot get human muscle index.", transform);
        return -1;
      }
      Transform boneTransform = targetAnimator.GetBoneTransform(humanBodyBone);
      if (!(boneTransform == null))
        return GetMuscleIndex(boneTransform);
      Debug.LogWarning("PuppetMaster target's Avatar does not contain a bone Transform for " + humanBodyBone, transform);
      return -1;
    }

    public int GetMuscleIndex(Transform target)
    {
      if (!CheckIfInitiated())
        return -1;
      if (target == null)
      {
        Debug.LogWarning("Target is null, can not get muscle index.", transform);
        return -1;
      }
      for (int muscleIndex = 0; muscleIndex < muscles.Length; ++muscleIndex)
      {
        if (muscles[muscleIndex].target == target)
          return muscleIndex;
      }
      Debug.LogWarning("No muscle with target " + target.name + "found on the PuppetMaster.", transform);
      return -1;
    }

    public int GetMuscleIndex(Rigidbody rigidbody)
    {
      if (!CheckIfInitiated())
        return -1;
      if (rigidbody == null)
      {
        Debug.LogWarning("Rigidbody is null, can not get muscle index.", transform);
        return -1;
      }
      for (int muscleIndex = 0; muscleIndex < muscles.Length; ++muscleIndex)
      {
        if (muscles[muscleIndex].rigidbody == rigidbody)
          return muscleIndex;
      }
      Debug.LogWarning("No muscle with Rigidbody " + rigidbody.name + "found on the PuppetMaster.", transform);
      return -1;
    }

    public int GetMuscleIndex(ConfigurableJoint joint)
    {
      if (!CheckIfInitiated())
        return -1;
      if (joint == null)
      {
        Debug.LogWarning("Joint is null, can not get muscle index.", transform);
        return -1;
      }
      for (int muscleIndex = 0; muscleIndex < muscles.Length; ++muscleIndex)
      {
        if (muscles[muscleIndex].joint == joint)
          return muscleIndex;
      }
      Debug.LogWarning("No muscle with Joint " + joint.name + "found on the PuppetMaster.", transform);
      return -1;
    }

    public static PuppetMaster SetUp(
      Transform target,
      Transform ragdoll,
      int characterControllerLayer,
      int ragdollLayer)
    {
      if (!(ragdoll != target))
        return SetUp(ragdoll, characterControllerLayer, ragdollLayer);
      PuppetMaster puppetMaster = ragdoll.gameObject.AddComponent<PuppetMaster>();
      puppetMaster.SetUpTo(target, characterControllerLayer, ragdollLayer);
      return puppetMaster;
    }

    public static PuppetMaster SetUp(
      Transform target,
      int characterControllerLayer,
      int ragdollLayer)
    {
      PuppetMaster puppetMaster = Instantiate(target.gameObject, target.position, target.rotation).transform.gameObject.AddComponent<PuppetMaster>();
      puppetMaster.SetUpTo(target, characterControllerLayer, ragdollLayer);
      RemoveRagdollComponents(target, characterControllerLayer);
      return puppetMaster;
    }

    public void SetUpTo(Transform setUpTo, int characterControllerLayer, int ragdollLayer)
    {
      if (setUpTo == null)
      {
        Debug.LogWarning("SetUpTo is null. Can not set the PuppetMaster up to a null Transform.", transform);
      }
      else
      {
        if (setUpTo == transform)
        {
          setUpTo = Instantiate(setUpTo.gameObject, setUpTo.position, setUpTo.rotation).transform;
          setUpTo.name = name;
          RemoveRagdollComponents(setUpTo, characterControllerLayer);
        }
        RemoveUnnecessaryBones();
        Component[] componentsInChildren = GetComponentsInChildren<Component>();
        for (int index = 0; index < componentsInChildren.Length; ++index)
        {
          if (!(componentsInChildren[index] is PuppetMaster) && !(componentsInChildren[index] is Transform) && !(componentsInChildren[index] is Rigidbody) && !(componentsInChildren[index] is BoxCollider) && !(componentsInChildren[index] is CapsuleCollider) && !(componentsInChildren[index] is SphereCollider) && !(componentsInChildren[index] is MeshCollider) && !(componentsInChildren[index] is Joint) && !(componentsInChildren[index] is Animator))
            DestroyImmediate(componentsInChildren[index]);
        }
        foreach (Object componentsInChild in GetComponentsInChildren<Animator>())
          DestroyImmediate(componentsInChild);
        Component[] components = transform.GetComponents<Component>();
        for (int index = 0; index < components.Length; ++index)
        {
          if (!(components[index] is PuppetMaster) && !(components[index] is Transform))
            DestroyImmediate(components[index]);
        }
        foreach (Rigidbody componentsInChild in transform.GetComponentsInChildren<Rigidbody>())
        {
          if (componentsInChild.transform != transform && componentsInChild.GetComponent<ConfigurableJoint>() == null)
            componentsInChild.gameObject.AddComponent<ConfigurableJoint>();
        }
        targetRoot = setUpTo;
        SetUpMuscles(setUpTo);
        name = nameof (PuppetMaster);
        Transform transform1 = setUpTo.parent == null || setUpTo.parent != transform.parent || setUpTo.parent.name != setUpTo.name + " Root" ? new GameObject(setUpTo.name + " Root").transform : setUpTo.parent;
        transform1.parent = transform.parent;
        Transform transform2 = new GameObject("Behaviours").transform;
        Comments comments = transform2.gameObject.GetComponent<Comments>();
        if (comments == null)
          comments = transform2.gameObject.AddComponent<Comments>();
        comments.text = "All Puppet Behaviours should be parented to this GameObject, the PuppetMaster will automatically find them from here. All Puppet Behaviours have been designed so that they could be simply copied from one character to another without changing any references. It is important because they contain a lot of parameters and would be otherwise tedious to set up and tweak.";
        transform1.position = setUpTo.position;
        transform1.rotation = setUpTo.rotation;
        transform2.position = setUpTo.position;
        transform2.rotation = setUpTo.rotation;
        transform.position = setUpTo.position;
        transform.rotation = setUpTo.rotation;
        transform2.parent = transform1;
        transform.parent = transform1;
        setUpTo.parent = transform1;
        targetRoot.gameObject.layer = characterControllerLayer;
        gameObject.layer = ragdollLayer;
        foreach (Muscle muscle in muscles)
          muscle.joint.gameObject.layer = ragdollLayer;
        Physics.IgnoreLayerCollision(characterControllerLayer, ragdollLayer);
      }
    }

    public static void RemoveRagdollComponents(Transform target, int characterControllerLayer)
    {
      if (target == null)
        return;
      Rigidbody[] componentsInChildren1 = target.GetComponentsInChildren<Rigidbody>();
      Cloth[] componentsInChildren2 = target.GetComponentsInChildren<Cloth>();
      for (int index = 0; index < componentsInChildren1.Length; ++index)
      {
        if (componentsInChildren1[index].gameObject != target.gameObject)
        {
          Joint component1 = componentsInChildren1[index].GetComponent<Joint>();
          Collider component2 = componentsInChildren1[index].GetComponent<Collider>();
          if (component1 != null)
            DestroyImmediate(component1);
          if (component2 != null)
          {
            if (!IsClothCollider(component2, componentsInChildren2))
              DestroyImmediate(component2);
            else
              component2.gameObject.layer = characterControllerLayer;
          }
          DestroyImmediate(componentsInChildren1[index]);
        }
      }
      Collider[] componentsInChildren3 = target.GetComponentsInChildren<Collider>();
      for (int index = 0; index < componentsInChildren3.Length; ++index)
      {
        if (componentsInChildren3[index].transform != target && !IsClothCollider(componentsInChildren3[index], componentsInChildren2))
          DestroyImmediate(componentsInChildren3[index]);
      }
      PuppetMaster component = target.GetComponent<PuppetMaster>();
      if (!(component != null))
        return;
      DestroyImmediate(component);
    }

    private void SetUpMuscles(Transform setUpTo)
    {
      ConfigurableJoint[] componentsInChildren1 = this.transform.GetComponentsInChildren<ConfigurableJoint>();
      if (componentsInChildren1.Length == 0)
      {
        Debug.LogWarning("No ConfigurableJoints found, can not build PuppetMaster. Please create ConfigurableJoints to connect the ragdoll bones together.", transform);
      }
      else
      {
        Animator componentInChildren = targetRoot.GetComponentInChildren<Animator>();
        Transform[] componentsInChildren2 = setUpTo.GetComponentsInChildren<Transform>();
        muscles = new Muscle[componentsInChildren1.Length];
        int index1 = -1;
        for (int index2 = 0; index2 < componentsInChildren1.Length; ++index2)
        {
          muscles[index2] = new Muscle();
          muscles[index2].joint = componentsInChildren1[index2];
          muscles[index2].name = componentsInChildren1[index2].name;
          muscles[index2].props = new Muscle.Props(1f, 1f, 1f, 1f, muscles[index2].joint.connectedBody == null);
          if (muscles[index2].joint.connectedBody == null && index1 == -1)
            index1 = index2;
          foreach (Transform transform in componentsInChildren2)
          {
            if (transform.name == componentsInChildren1[index2].name)
            {
              muscles[index2].target = transform;
              if (componentInChildren != null)
              {
                muscles[index2].props.group = FindGroup(componentInChildren, muscles[index2].target);
                if (muscles[index2].props.group == Muscle.Group.Hips || muscles[index2].props.group == Muscle.Group.Leg || muscles[index2].props.group == Muscle.Group.Foot)
                  muscles[index2].props.mapPosition = true;
              }
              break;
            }
          }
        }
        if (index1 != 0)
        {
          Muscle muscle1 = muscles[0];
          Muscle muscle2 = muscles[index1];
          muscles[index1] = muscle1;
          muscles[0] = muscle2;
        }
        bool flag = true;
        foreach (Muscle muscle in muscles)
        {
          if (muscle.target == null)
            Debug.LogWarning("No target Transform found for PuppetMaster muscle " + muscle.joint.name + ". Please assign manually.", transform);
          if (muscle.props.group != muscles[0].props.group)
            flag = false;
        }
        if (!flag)
          return;
        Debug.LogWarning("Muscle groups need to be assigned in the PuppetMaster!", this.transform);
      }
    }

    private static Muscle.Group FindGroup(Animator animator, Transform t)
    {
      if (!animator.isHuman)
        return Muscle.Group.Hips;
      if (t == animator.GetBoneTransform(HumanBodyBones.Chest))
        return Muscle.Group.Spine;
      if (t == animator.GetBoneTransform(HumanBodyBones.Head))
        return Muscle.Group.Head;
      if (t == animator.GetBoneTransform(HumanBodyBones.Hips))
        return Muscle.Group.Hips;
      if (t == animator.GetBoneTransform(HumanBodyBones.LeftFoot))
        return Muscle.Group.Foot;
      if (t == animator.GetBoneTransform(HumanBodyBones.LeftHand))
        return Muscle.Group.Hand;
      if (t == animator.GetBoneTransform(HumanBodyBones.LeftLowerArm))
        return Muscle.Group.Arm;
      if (t == animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg))
        return Muscle.Group.Leg;
      if (t == animator.GetBoneTransform(HumanBodyBones.LeftUpperArm))
        return Muscle.Group.Arm;
      if (t == animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg))
        return Muscle.Group.Leg;
      if (t == animator.GetBoneTransform(HumanBodyBones.RightFoot))
        return Muscle.Group.Foot;
      if (t == animator.GetBoneTransform(HumanBodyBones.RightHand))
        return Muscle.Group.Hand;
      if (t == animator.GetBoneTransform(HumanBodyBones.RightLowerArm))
        return Muscle.Group.Arm;
      if (t == animator.GetBoneTransform(HumanBodyBones.RightLowerLeg))
        return Muscle.Group.Leg;
      if (t == animator.GetBoneTransform(HumanBodyBones.RightUpperArm))
        return Muscle.Group.Arm;
      return t == animator.GetBoneTransform(HumanBodyBones.RightUpperLeg) ? Muscle.Group.Leg : Muscle.Group.Spine;
    }

    private void RemoveUnnecessaryBones()
    {
      Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
      for (int index1 = 1; index1 < componentsInChildren.Length; ++index1)
      {
        bool flag = false;
        if (componentsInChildren[index1].GetComponent<Rigidbody>() != null || componentsInChildren[index1].GetComponent<ConfigurableJoint>() != null)
          flag = true;
        if (componentsInChildren[index1].GetComponent<Collider>() != null && componentsInChildren[index1].GetComponent<Rigidbody>() == null)
          flag = true;
        if (componentsInChildren[index1].GetComponent<CharacterController>() != null)
          flag = false;
        if (!flag)
        {
          Transform[] transformArray = new Transform[componentsInChildren[index1].childCount];
          for (int index2 = 0; index2 < transformArray.Length; ++index2)
            transformArray[index2] = componentsInChildren[index1].GetChild(index2);
          for (int index3 = 0; index3 < transformArray.Length; ++index3)
            transformArray[index3].parent = componentsInChildren[index1].parent;
          DestroyImmediate(componentsInChildren[index1].gameObject);
        }
      }
    }

    private static bool IsClothCollider(Collider collider, Cloth[] cloths)
    {
      if (cloths == null)
        return false;
      foreach (Cloth cloth in cloths)
      {
        if (cloth == null)
          return false;
        foreach (CapsuleCollider capsuleCollider in cloth.capsuleColliders)
        {
          if (capsuleCollider != null && capsuleCollider.gameObject == collider.gameObject)
            return true;
        }
        foreach (ClothSphereColliderPair sphereCollider in cloth.sphereColliders)
        {
          if (sphereCollider.first != null && sphereCollider.first.gameObject == collider.gameObject || sphereCollider.second != null && sphereCollider.second.gameObject == collider.gameObject)
            return true;
        }
      }
      return false;
    }

    public bool isSwitchingState => activeState != state;

    public bool isKilling { get; private set; }

    public bool isAlive => activeState == State.Alive;

    public bool isFrozen => activeState == State.Frozen;

    public void Kill() => state = State.Dead;

    public void Kill(StateSettings stateSettings)
    {
      this.stateSettings = stateSettings;
      state = State.Dead;
    }

    public void Freeze() => state = State.Frozen;

    public void Freeze(StateSettings stateSettings)
    {
      this.stateSettings = stateSettings;
      state = State.Frozen;
    }

    public void Resurrect() => state = State.Alive;

    protected virtual void SwitchStates()
    {
      if (state == lastState || isKilling)
        return;
      if (freezeFlag)
      {
        if (state == State.Alive)
        {
          activeState = State.Dead;
          lastState = State.Dead;
          freezeFlag = false;
        }
        else if (state == State.Dead)
        {
          lastState = State.Dead;
          freezeFlag = false;
          return;
        }
        if (freezeFlag)
          return;
      }
      if (lastState == State.Alive)
      {
        if (state == State.Dead)
          StartCoroutine(AliveToDead(false));
        else if (state == State.Frozen)
          StartCoroutine(AliveToDead(true));
      }
      else if (lastState == State.Dead)
      {
        if (state == State.Alive)
          DeadToAlive();
        else if (state == State.Frozen)
          DeadToFrozen();
      }
      else if (lastState == State.Frozen)
      {
        if (state == State.Alive)
          FrozenToAlive();
        else if (state == State.Dead)
          FrozenToDead();
      }
      lastState = state;
    }

    private IEnumerator AliveToDead(bool freeze)
    {
      isKilling = true;
      mode = Mode.Active;
      if (stateSettings.enableAngularLimitsOnKill && !angularLimits)
      {
        angularLimits = true;
        angularLimitsEnabledOnKill = true;
      }
      if (stateSettings.enableInternalCollisionsOnKill && !internalCollisions)
      {
        internalCollisions = true;
        internalCollisionsEnabledOnKill = true;
      }
      Muscle[] muscleArray1 = muscles;
      for (int index = 0; index < muscleArray1.Length; ++index)
      {
        Muscle m = muscleArray1[index];
        m.state.pinWeightMlp = 0.0f;
        m.state.muscleDamperAdd = stateSettings.deadMuscleDamper;
        m.rigidbody.velocity = m.mappedVelocity;
        m.rigidbody.angularVelocity = m.mappedAngularVelocity;
        m = null;
      }
      muscleArray1 = null;
      float range = muscles[0].state.muscleWeightMlp - stateSettings.deadMuscleWeight;
      BehaviourBase[] behaviourBaseArray1 = behaviours;
      for (int index = 0; index < behaviourBaseArray1.Length; ++index)
      {
        BehaviourBase behaviour = behaviourBaseArray1[index];
        behaviour.KillStart();
        behaviour = null;
      }
      behaviourBaseArray1 = null;
      if (stateSettings.killDuration > 0.0 && range > 0.0)
      {
        float mW = muscles[0].state.muscleWeightMlp;
        while (mW > (double) stateSettings.deadMuscleWeight)
        {
          mW = Mathf.Max(mW - Time.deltaTime * (range / stateSettings.killDuration), stateSettings.deadMuscleWeight);
          Muscle[] muscleArray2 = muscles;
          for (int index = 0; index < muscleArray2.Length; ++index)
          {
            Muscle m = muscleArray2[index];
            m.state.muscleWeightMlp = mW;
            m = null;
          }
          muscleArray2 = null;
          yield return null;
        }
      }
      Muscle[] muscleArray3 = muscles;
      for (int index = 0; index < muscleArray3.Length; ++index)
      {
        Muscle m = muscleArray3[index];
        m.state.muscleWeightMlp = stateSettings.deadMuscleWeight;
        m = null;
      }
      muscleArray3 = null;
      SetAnimationEnabled(false);
      isKilling = false;
      activeState = State.Dead;
      if (freeze)
        freezeFlag = true;
      BehaviourBase[] behaviourBaseArray2 = behaviours;
      for (int index = 0; index < behaviourBaseArray2.Length; ++index)
      {
        BehaviourBase behaviour = behaviourBaseArray2[index];
        behaviour.KillEnd();
        behaviour = null;
      }
      behaviourBaseArray2 = null;
      if (OnDeath != null)
        OnDeath();
    }

    private void OnFreezeFlag()
    {
      if (!CanFreeze())
        return;
      SetAnimationEnabled(false);
      foreach (Muscle muscle in muscles)
        muscle.joint.gameObject.SetActive(false);
      foreach (BehaviourBase behaviour in behaviours)
      {
        behaviour.Freeze();
        if (behaviour.gameObject.activeSelf)
        {
          behaviour.deactivated = true;
          behaviour.gameObject.SetActive(false);
        }
      }
      freezeFlag = false;
      activeState = State.Frozen;
      if (OnFreeze != null)
        OnFreeze();
      if (!stateSettings.freezePermanently)
        return;
      if (behaviours.Length != 0 && behaviours[0] != null)
        Destroy(behaviours[0].transform.parent.gameObject);
      Destroy(gameObject);
    }

    private void DeadToAlive()
    {
      foreach (Muscle muscle in muscles)
      {
        muscle.state.pinWeightMlp = 1f;
        muscle.state.muscleWeightMlp = 1f;
        muscle.state.muscleDamperAdd = 0.0f;
      }
      if (angularLimitsEnabledOnKill)
      {
        angularLimits = false;
        angularLimitsEnabledOnKill = false;
      }
      if (internalCollisionsEnabledOnKill)
      {
        internalCollisions = false;
        internalCollisionsEnabledOnKill = false;
      }
      foreach (BehaviourBase behaviour in behaviours)
        behaviour.Resurrect();
      SetAnimationEnabled(true);
      activeState = State.Alive;
      if (OnResurrection == null)
        return;
      OnResurrection();
    }

    private void SetAnimationEnabled(bool to)
    {
      animatorDisabled = false;
      if (targetAnimator != null)
        targetAnimator.enabled = to;
      if (!(targetAnimation != null))
        return;
      targetAnimation.enabled = to;
    }

    private void DeadToFrozen() => freezeFlag = true;

    private void FrozenToAlive()
    {
      freezeFlag = false;
      foreach (Muscle muscle in muscles)
      {
        muscle.state.pinWeightMlp = 1f;
        muscle.state.muscleWeightMlp = 1f;
        muscle.state.muscleDamperAdd = 0.0f;
      }
      if (angularLimitsEnabledOnKill)
      {
        angularLimits = false;
        angularLimitsEnabledOnKill = false;
      }
      if (internalCollisionsEnabledOnKill)
      {
        internalCollisions = false;
        internalCollisionsEnabledOnKill = false;
      }
      ActivateRagdoll();
      foreach (BehaviourBase behaviour in behaviours)
      {
        behaviour.Unfreeze();
        behaviour.Resurrect();
        if (behaviour.deactivated)
          behaviour.gameObject.SetActive(true);
      }
      if (targetAnimator != null)
        targetAnimator.enabled = true;
      if (targetAnimation != null)
        targetAnimation.enabled = true;
      activeState = State.Alive;
      if (OnUnfreeze != null)
        OnUnfreeze();
      if (OnResurrection == null)
        return;
      OnResurrection();
    }

    private void FrozenToDead()
    {
      freezeFlag = false;
      ActivateRagdoll();
      foreach (BehaviourBase behaviour in behaviours)
      {
        behaviour.Unfreeze();
        if (behaviour.deactivated)
          behaviour.gameObject.SetActive(true);
      }
      activeState = State.Dead;
      if (OnUnfreeze == null)
        return;
      OnUnfreeze();
    }

    private void ActivateRagdoll(bool kinematic = false)
    {
      foreach (Muscle muscle in muscles)
        muscle.Reset();
      foreach (Muscle muscle in muscles)
      {
        muscle.joint.gameObject.SetActive(true);
        muscle.rigidbody.isKinematic = kinematic;
        muscle.rigidbody.velocity = Vector3.zero;
        muscle.rigidbody.angularVelocity = Vector3.zero;
      }
      internalCollisionsEnabled = true;
      SetInternalCollisions(internalCollisions);
      Read();
      foreach (Muscle muscle in muscles)
        muscle.MoveToTarget();
    }

    private bool CanFreeze()
    {
      foreach (Muscle muscle in muscles)
      {
        if (muscle.rigidbody.velocity.sqrMagnitude > (double) stateSettings.maxFreezeSqrVelocity)
          return false;
      }
      return true;
    }

    public void SampleTargetMappedState()
    {
      if (!CheckIfInitiated())
        return;
      sampleTargetMappedState = true;
      if (!targetMappedStateStored)
      {
        sampleTargetMappedState = true;
      }
      else
      {
        for (int index = 0; index < targetChildren.Length; ++index)
        {
          targetSampledPositions[index] = targetMappedPositions[index];
          targetSampledRotations[index] = targetMappedRotations[index];
        }
        targetMappedStateSampled = true;
      }
    }

    public void FixTargetToSampledState(float weight)
    {
      if (!CheckIfInitiated() || weight <= 0.0 || !targetMappedStateSampled)
        return;
      for (int index = 0; index < targetChildren.Length; ++index)
      {
        if (targetChildren[index] == null)
        {
          Debug.LogWarning("PuppetMaster.UpdateTargetHierarchy() needs to be called when any of the child Transforms of the targetRoot are unparented or removed.", transform);
          return;
        }
        if (index == 0)
        {
          targetChildren[index].position = Vector3.Lerp(targetChildren[index].position, targetSampledPositions[index], weight);
          targetChildren[index].rotation = Quaternion.Lerp(targetChildren[index].rotation, targetSampledRotations[index], weight);
        }
        else
        {
          targetChildren[index].position = Vector3.Lerp(targetChildren[index].position, targetSampledPositions[0] + targetSampledRotations[0] * targetSampledPositions[index], weight);
          targetChildren[index].rotation = Quaternion.Lerp(targetChildren[index].rotation, targetSampledRotations[0] * targetSampledRotations[index], weight);
        }
      }
      foreach (Muscle muscle in muscles)
        muscle.positionOffset = muscle.target.position - muscle.rigidbody.position;
    }

    public void StoreTargetMappedState()
    {
      if (!CheckIfInitiated() || !storeTargetMappedState)
        return;
      for (int index = 0; index < targetChildren.Length; ++index)
      {
        if (index == 0)
        {
          targetMappedPositions[index] = targetChildren[index].position;
          targetMappedRotations[index] = targetChildren[index].rotation;
        }
        else
        {
          targetMappedPositions[index] = Quaternion.Inverse(targetChildren[0].rotation) * (targetChildren[index].position - targetChildren[0].position);
          targetMappedRotations[index] = Quaternion.Inverse(targetChildren[0].rotation) * targetChildren[index].rotation;
        }
      }
      targetMappedStateStored = true;
      if (sampleTargetMappedState)
        SampleTargetMappedState();
      sampleTargetMappedState = false;
    }

    private void UpdateHierarchies()
    {
      targetChildren = new Transform[muscles.Length];
      for (int index = 0; index < muscles.Length; ++index)
        targetChildren[index] = muscles[index].target;
      targetMappedPositions = new Vector3[targetChildren.Length];
      targetMappedRotations = new Quaternion[targetChildren.Length];
      targetSampledPositions = new Vector3[targetChildren.Length];
      targetSampledRotations = new Quaternion[targetChildren.Length];
      targetMappedStateStored = false;
      targetMappedStateSampled = false;
      AssignParentAndChildIndexes();
      AssignKinshipDegrees();
      UpdateBroadcasterMuscleIndexes();
      internalCollisionsEnabled = !internalCollisions;
      SetInternalCollisions(internalCollisions);
      angularLimitsEnabled = !angularLimits;
      SetAngularLimits(angularLimits);
      hasProp = HasProp();
      if (OnHierarchyChanged == null)
        return;
      OnHierarchyChanged();
    }

    private bool HasProp()
    {
      foreach (Muscle muscle in muscles)
      {
        if (muscle.props.group == Muscle.Group.Prop)
          return true;
      }
      return false;
    }

    private void UpdateBroadcasterMuscleIndexes()
    {
      for (int index = 0; index < muscles.Length; ++index)
      {
        if (muscles[index].broadcaster != null)
          muscles[index].broadcaster.muscleIndex = index;
        if (muscles[index].jointBreakBroadcaster != null)
          muscles[index].jointBreakBroadcaster.muscleIndex = index;
      }
    }

    private void AssignParentAndChildIndexes()
    {
      for (int index1 = 0; index1 < muscles.Length; ++index1)
      {
        muscles[index1].parentIndexes = [];
        if (muscles[index1].joint.connectedBody != null)
          AddToParentsRecursive(muscles[index1].joint.connectedBody.GetComponent<ConfigurableJoint>(), ref muscles[index1].parentIndexes);
        muscles[index1].childIndexes = [];
        muscles[index1].childFlags = new bool[muscles.Length];
        for (int index2 = 0; index2 < muscles.Length; ++index2)
        {
          if (index1 != index2 && muscles[index2].joint.connectedBody == muscles[index1].rigidbody)
            AddToChildrenRecursive(muscles[index2].joint, ref muscles[index1].childIndexes, ref muscles[index1].childFlags);
        }
      }
    }

    private void AddToParentsRecursive(ConfigurableJoint joint, ref int[] indexes)
    {
      if (joint == null)
        return;
      int muscleIndexLowLevel = GetMuscleIndexLowLevel(joint);
      if (muscleIndexLowLevel == -1)
        return;
      Array.Resize(ref indexes, indexes.Length + 1);
      indexes[indexes.Length - 1] = muscleIndexLowLevel;
      if (joint.connectedBody == null)
        return;
      AddToParentsRecursive(joint.connectedBody.GetComponent<ConfigurableJoint>(), ref indexes);
    }

    private void AddToChildrenRecursive(
      ConfigurableJoint joint,
      ref int[] indexes,
      ref bool[] childFlags)
    {
      if (joint == null)
        return;
      int muscleIndexLowLevel = GetMuscleIndexLowLevel(joint);
      if (muscleIndexLowLevel == -1)
        return;
      Array.Resize(ref indexes, indexes.Length + 1);
      indexes[indexes.Length - 1] = muscleIndexLowLevel;
      childFlags[muscleIndexLowLevel] = true;
      for (int index = 0; index < muscles.Length; ++index)
      {
        if (index != muscleIndexLowLevel && muscles[index].joint.connectedBody == joint.GetComponent<Rigidbody>())
          AddToChildrenRecursive(muscles[index].joint, ref indexes, ref childFlags);
      }
    }

    private void AssignKinshipDegrees()
    {
      for (int index = 0; index < muscles.Length; ++index)
      {
        muscles[index].kinshipDegrees = new int[muscles.Length];
        AssignKinshipsDownRecursive(ref muscles[index].kinshipDegrees, 1, index);
        AssignKinshipsUpRecursive(ref muscles[index].kinshipDegrees, 1, index);
      }
    }

    private void AssignKinshipsDownRecursive(ref int[] kinshipDegrees, int degree, int index)
    {
      for (int index1 = 0; index1 < muscles.Length; ++index1)
      {
        if (index1 != index && muscles[index1].joint.connectedBody == muscles[index].rigidbody)
        {
          kinshipDegrees[index1] = degree;
          AssignKinshipsDownRecursive(ref kinshipDegrees, degree + 1, index1);
        }
      }
    }

    private void AssignKinshipsUpRecursive(ref int[] kinshipDegrees, int degree, int index)
    {
      for (int index1 = 0; index1 < muscles.Length; ++index1)
      {
        if (index1 != index && muscles[index1].rigidbody == muscles[index].joint.connectedBody)
        {
          kinshipDegrees[index1] = degree;
          AssignKinshipsUpRecursive(ref kinshipDegrees, degree + 1, index1);
          for (int index2 = 0; index2 < muscles.Length; ++index2)
          {
            if (index2 != index1 && index2 != index && muscles[index2].joint.connectedBody == muscles[index1].rigidbody)
            {
              kinshipDegrees[index2] = degree + 1;
              AssignKinshipsDownRecursive(ref kinshipDegrees, degree + 2, index2);
            }
          }
        }
      }
    }

    private int GetMuscleIndexLowLevel(ConfigurableJoint joint)
    {
      for (int muscleIndexLowLevel = 0; muscleIndexLowLevel < muscles.Length; ++muscleIndexLowLevel)
      {
        if (muscles[muscleIndexLowLevel].joint == joint)
          return muscleIndexLowLevel;
      }
      return -1;
    }

    public bool IsValid(bool log)
    {
      if (muscles == null)
      {
        if (log)
          Debug.LogWarning("PuppetMaster Muscles is null.", transform);
        return false;
      }
      if (muscles.Length == 0)
      {
        if (log)
          Debug.LogWarning("PuppetMaster has no muscles.", transform);
        return false;
      }
      for (int index = 0; index < muscles.Length; ++index)
      {
        if (muscles[index] == null)
        {
          if (log)
            Debug.LogWarning("Muscle is null, PuppetMaster muscle setup is invalid.", transform);
          return false;
        }
        if (!muscles[index].IsValid(log))
          return false;
      }
      if (targetRoot == null)
      {
        if (log)
          Debug.LogWarning("'Target Root' of PuppetMaster is null.");
        return false;
      }
      if (targetRoot.position != transform.position)
      {
        if (log)
          Debug.LogWarning("The position of the animated character (Target) must match with the position of the PuppetMaster when initiating PuppetMaster. If you are creating the Puppet in runtime, make sure you don't move the Target to another position immediatelly after instantiation. Move the Root Transform instead.");
        return false;
      }
      if (targetRoot == null)
      {
        if (log)
          Debug.LogWarning("Invalid PuppetMaster setup. (targetRoot not found)", transform);
        return false;
      }
      for (int index1 = 0; index1 < muscles.Length; ++index1)
      {
        for (int index2 = 0; index2 < muscles.Length; ++index2)
        {
          if (index1 != index2 && (muscles[index1] == muscles[index2] || muscles[index1].joint == muscles[index2].joint))
          {
            if (log)
              Debug.LogWarning("Joint " + muscles[index1].joint.name + " is used by multiple muscles (indexes " + index1 + " and " + index2 + "), PuppetMaster muscle setup is invalid.", transform);
            return false;
          }
        }
      }
      if (muscles[0].joint.connectedBody != null && muscles.Length > 1)
      {
        for (int index = 1; index < muscles.Length; ++index)
        {
          if (muscles[index].joint.GetComponent<Rigidbody>() == muscles[0].joint.connectedBody)
          {
            if (log)
              Debug.LogWarning("The first muscle needs to be the one that all the others are connected to (the hips).", transform);
            return false;
          }
        }
      }
      for (int index = 0; index < muscles.Length; ++index)
      {
        if (Vector3.SqrMagnitude(muscles[index].joint.transform.position - muscles[index].target.position) > 1.0 / 1000.0)
        {
          if (log)
            Debug.LogWarning("The position of each muscle needs to match with the position of it's target. Muscle '" + muscles[index].joint.name + "' position does not match with it's target. Right-click on the PuppetMaster component's header and select 'Fix Muscle Positions' from the context menu.", muscles[index].joint.transform);
          return false;
        }
      }
      CheckMassVariation(100f, true);
      return true;
    }

    private bool CheckMassVariation(float threshold, bool log)
    {
      float num1 = float.PositiveInfinity;
      float num2 = 0.0f;
      for (int index = 0; index < muscles.Length; ++index)
      {
        float mass = muscles[index].joint.GetComponent<Rigidbody>().mass;
        if (mass < (double) num1)
          num1 = mass;
        if (mass > (double) num2)
          num2 = mass;
      }
      if (num2 / (double) num1 <= threshold)
        return true;
      if (log)
        Debug.LogWarning("Mass variation between the Rigidbodies in the ragdoll is more than " + threshold + " times. This might cause instability and unwanted results with Rigidbodies connected by Joints. Min mass: " + num1 + ", max mass: " + num2, transform);
      return false;
    }

    private bool CheckIfInitiated()
    {
      if (!initiated)
        Debug.LogError("PuppetMaster has not been initiated yet.");
      return initiated;
    }

    [Serializable]
    public enum Mode
    {
      Active,
      Kinematic,
      Disabled,
    }

    public delegate void UpdateDelegate();

    public delegate void MuscleDelegate(Muscle muscle);

    [Serializable]
    public enum UpdateMode
    {
      Normal,
      AnimatePhysics,
      FixedUpdate,
    }

    [Serializable]
    public enum State
    {
      Alive,
      Dead,
      Frozen,
    }

    [Serializable]
    public struct StateSettings(
      float killDuration,
      float deadMuscleWeight = 0.01f,
      float deadMuscleDamper = 2f,
      float maxFreezeSqrVelocity = 0.02f,
      bool freezePermanently = false,
      bool enableAngularLimitsOnKill = true,
      bool enableInternalCollisionsOnKill = true) {
      [Tooltip("How much does it take to weigh out muscle weight to deadMuscleWeight?")]
      public float killDuration = killDuration;
      [Tooltip("The muscle weight mlp while the puppet is Dead.")]
      public float deadMuscleWeight = deadMuscleWeight;
      [Tooltip("The muscle damper add while the puppet is Dead.")]
      public float deadMuscleDamper = deadMuscleDamper;
      [Tooltip("The max square velocity of the ragdoll bones for freezing the puppet.")]
      public float maxFreezeSqrVelocity = maxFreezeSqrVelocity;
      [Tooltip("If true, PuppetMaster, all it's behaviours and the ragdoll will be destroyed when the puppet is frozen.")]
      public bool freezePermanently = freezePermanently;
      [Tooltip("If true, will enable angular limits when killing the puppet.")]
      public bool enableAngularLimitsOnKill = enableAngularLimitsOnKill;
      [Tooltip("If true, will enable internal collisions when killing the puppet.")]
      public bool enableInternalCollisionsOnKill = enableInternalCollisionsOnKill;

      public static StateSettings Default => new(1f);
    }
  }
}
