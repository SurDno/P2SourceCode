// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.PuppetMaster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
    public PuppetMaster.State state;
    [ContextMenuItem("Reset To Default", "ResetStateSettings")]
    [Tooltip("Settings for killing and freezing the puppet.")]
    public PuppetMaster.StateSettings stateSettings = PuppetMaster.StateSettings.Default;
    [Tooltip("Active mode means all muscles are active and the character is physically simulated. Kinematic mode sets rigidbody.isKinematic to true for all the muscles and simply updates their position/rotation to match the target's. Disabled mode disables the ragdoll. Switching modes is done by simply changing this value, blending in/out will be handled automatically by the PuppetMaster.")]
    public PuppetMaster.Mode mode;
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
    public float muscleDamper = 0.0f;
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
    public Muscle[] muscles = new Muscle[0];
    public PuppetMaster.UpdateDelegate OnPostInitiate;
    public PuppetMaster.UpdateDelegate OnRead;
    public PuppetMaster.UpdateDelegate OnWrite;
    public PuppetMaster.UpdateDelegate OnPostLateUpdate;
    public PuppetMaster.UpdateDelegate OnFixTransforms;
    public PuppetMaster.UpdateDelegate OnHierarchyChanged;
    public PuppetMaster.MuscleDelegate OnMuscleRemoved;
    private Animator _targetAnimator;
    [HideInInspector]
    public List<SolverManager> solvers = new List<SolverManager>();
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
    private PuppetMaster.Mode activeMode;
    private PuppetMaster.Mode lastMode;
    private float mappingBlend = 1f;
    public PuppetMaster.UpdateDelegate OnFreeze;
    public PuppetMaster.UpdateDelegate OnUnfreeze;
    public PuppetMaster.UpdateDelegate OnDeath;
    public PuppetMaster.UpdateDelegate OnResurrection;
    private PuppetMaster.State activeState;
    private PuppetMaster.State lastState;
    private bool angularLimitsEnabledOnKill;
    private bool internalCollisionsEnabledOnKill;
    private bool animationDisabledbyStates;
    [HideInInspector]
    public bool storeTargetMappedState = true;
    private Transform[] targetChildren = new Transform[0];
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

    private void ResetStateSettings() => this.stateSettings = PuppetMaster.StateSettings.Default;

    public Animator targetAnimator
    {
      get
      {
        if ((UnityEngine.Object) this._targetAnimator == (UnityEngine.Object) null)
          this._targetAnimator = this.targetRoot.GetComponentInChildren<Animator>();
        if ((UnityEngine.Object) this._targetAnimator == (UnityEngine.Object) null && (UnityEngine.Object) this.targetRoot.parent != (UnityEngine.Object) null)
          this._targetAnimator = this.targetRoot.parent.GetComponentInChildren<Animator>();
        return this._targetAnimator;
      }
      set => this._targetAnimator = value;
    }

    public Animation targetAnimation { get; private set; }

    public BehaviourBase[] behaviours { get; private set; }

    public bool isActive
    {
      get
      {
        return this.isActiveAndEnabled && this.initiated && (this.activeMode == PuppetMaster.Mode.Active || this.isBlending);
      }
    }

    public bool initiated { get; private set; }

    public PuppetMaster.UpdateMode updateMode
    {
      get
      {
        return this.targetUpdateMode == AnimatorUpdateMode.AnimatePhysics ? (this.isLegacy ? PuppetMaster.UpdateMode.AnimatePhysics : PuppetMaster.UpdateMode.FixedUpdate) : PuppetMaster.UpdateMode.Normal;
      }
    }

    public bool controlsAnimator
    {
      get
      {
        return this.isActiveAndEnabled && this.isActive && this.initiated && this.updateMode == PuppetMaster.UpdateMode.FixedUpdate;
      }
    }

    public bool isBlending => this.isSwitchingMode || this.isSwitchingState;

    public void Teleport(Vector3 position, Quaternion rotation, bool moveToTarget)
    {
      this.teleport = true;
      this.teleportPosition = position;
      this.teleportRotation = rotation;
      this.teleportMoveToTarget = moveToTarget;
    }

    private void OnDisable()
    {
      if (!this.gameObject.activeInHierarchy && this.initiated && Application.isPlaying)
      {
        foreach (Muscle muscle in this.muscles)
          muscle.Reset();
      }
      this.hasBeenDisabled = true;
    }

    private void OnEnable()
    {
      if (!this.gameObject.activeInHierarchy || !this.initiated || !this.hasBeenDisabled || !Application.isPlaying)
        return;
      this.isSwitchingMode = false;
      this.activeMode = this.mode;
      this.lastMode = this.mode;
      this.mappingBlend = this.mode == PuppetMaster.Mode.Active ? 1f : 0.0f;
      this.activeState = this.state;
      this.lastState = this.state;
      this.isKilling = false;
      this.freezeFlag = false;
      this.SetAnimationEnabled(this.state == PuppetMaster.State.Alive);
      if (this.state == PuppetMaster.State.Alive && (UnityEngine.Object) this.targetAnimator != (UnityEngine.Object) null && this.mode != PuppetMaster.Mode.Disabled)
        this.targetAnimator.Update(1f / 1000f);
      foreach (Muscle muscle in this.muscles)
      {
        muscle.state.pinWeightMlp = this.state == PuppetMaster.State.Alive ? 1f : 0.0f;
        muscle.state.muscleWeightMlp = this.state == PuppetMaster.State.Alive ? 1f : this.stateSettings.deadMuscleWeight;
        muscle.state.muscleDamperAdd = 0.0f;
      }
      if (this.state != PuppetMaster.State.Frozen && this.mode != PuppetMaster.Mode.Disabled)
      {
        this.ActivateRagdoll(this.mode == PuppetMaster.Mode.Kinematic);
        foreach (Component behaviour in this.behaviours)
          behaviour.gameObject.SetActive(true);
      }
      else
      {
        foreach (Muscle muscle in this.muscles)
          muscle.joint.gameObject.SetActive(false);
        if (this.state == PuppetMaster.State.Frozen)
        {
          foreach (BehaviourBase behaviour in this.behaviours)
          {
            if (behaviour.gameObject.activeSelf)
            {
              behaviour.deactivated = true;
              behaviour.gameObject.SetActive(false);
            }
          }
          if (this.stateSettings.freezePermanently)
          {
            if (this.behaviours.Length != 0 && (UnityEngine.Object) this.behaviours[0] != (UnityEngine.Object) null)
              UnityEngine.Object.Destroy((UnityEngine.Object) this.behaviours[0].transform.parent.gameObject);
            UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
            return;
          }
        }
      }
      foreach (BehaviourBase behaviour in this.behaviours)
        behaviour.OnReactivate();
    }

    private void Awake()
    {
      if (this.muscles.Length == 0)
        return;
      this.Initiate();
      if (this.initiated)
        return;
      this.awakeFailed = true;
    }

    private void Start()
    {
      if (!this.initiated && !this.awakeFailed)
        this.Initiate();
      if (!this.initiated)
        return;
      this.solvers.AddRange((IEnumerable<SolverManager>) this.targetRoot.GetComponentsInChildren<SolverManager>());
    }

    public Transform FindTargetRootRecursive(Transform t)
    {
      if ((UnityEngine.Object) t.parent == (UnityEngine.Object) null)
        return (Transform) null;
      foreach (UnityEngine.Object @object in t.parent)
      {
        if (@object == (UnityEngine.Object) this.transform)
          return t;
      }
      return this.FindTargetRootRecursive(t.parent);
    }

    private void Initiate()
    {
      this.initiated = false;
      if (this.muscles.Length != 0 && (UnityEngine.Object) this.muscles[0].target != (UnityEngine.Object) null && (UnityEngine.Object) this.targetRoot == (UnityEngine.Object) null)
        this.targetRoot = this.FindTargetRootRecursive(this.muscles[0].target);
      if ((UnityEngine.Object) this.targetRoot != (UnityEngine.Object) null && (UnityEngine.Object) this.targetAnimator == (UnityEngine.Object) null)
      {
        this.targetAnimator = this.targetRoot.GetComponentInChildren<Animator>();
        if ((UnityEngine.Object) this.targetAnimator == (UnityEngine.Object) null)
          this.targetAnimation = this.targetRoot.GetComponentInChildren<Animation>();
      }
      if (!this.IsValid(true))
        return;
      if ((UnityEngine.Object) this.humanoidConfig != (UnityEngine.Object) null && (UnityEngine.Object) this.targetAnimator != (UnityEngine.Object) null && this.targetAnimator.isHuman)
        this.humanoidConfig.ApplyTo(this);
      this.isLegacy = (UnityEngine.Object) this.targetAnimator == (UnityEngine.Object) null && (UnityEngine.Object) this.targetAnimation != (UnityEngine.Object) null;
      this.behaviours = this.transform.GetComponentsInChildren<BehaviourBase>();
      if (this.behaviours.Length == 0 && (UnityEngine.Object) this.transform.parent != (UnityEngine.Object) null)
        this.behaviours = this.transform.parent.GetComponentsInChildren<BehaviourBase>();
      for (int index = 0; index < this.muscles.Length; ++index)
      {
        this.muscles[index].Initiate(this.muscles);
        if (this.behaviours.Length != 0)
        {
          this.muscles[index].broadcaster = this.muscles[index].joint.gameObject.GetComponent<MuscleCollisionBroadcaster>();
          if ((UnityEngine.Object) this.muscles[index].broadcaster == (UnityEngine.Object) null)
            this.muscles[index].broadcaster = this.muscles[index].joint.gameObject.AddComponent<MuscleCollisionBroadcaster>();
          this.muscles[index].broadcaster.puppetMaster = this;
          this.muscles[index].broadcaster.muscleIndex = index;
        }
        if (double.PositiveInfinity != (double) this.muscles[index].joint.breakForce)
        {
          this.muscles[index].jointBreakBroadcaster = this.muscles[index].joint.gameObject.GetComponent<JointBreakBroadcaster>();
          if ((UnityEngine.Object) this.muscles[index].jointBreakBroadcaster == (UnityEngine.Object) null)
            this.muscles[index].jointBreakBroadcaster = this.muscles[index].joint.gameObject.AddComponent<JointBreakBroadcaster>();
          this.muscles[index].jointBreakBroadcaster.puppetMaster = this;
          this.muscles[index].jointBreakBroadcaster.muscleIndex = index;
        }
      }
      this.UpdateHierarchies();
      this.hierarchyIsFlat = this.HierarchyIsFlat();
      this.initiated = true;
      foreach (BehaviourBase behaviour in this.behaviours)
        behaviour.puppetMaster = this;
      foreach (BehaviourBase behaviour in this.behaviours)
        behaviour.Initiate();
      this.SwitchStates();
      this.SwitchModes();
      foreach (Muscle muscle in this.muscles)
        muscle.Read();
      this.StoreTargetMappedState();
      if ((UnityEngine.Object) Singleton<PuppetMasterSettings>.instance != (UnityEngine.Object) null)
        Singleton<PuppetMasterSettings>.instance.Register(this);
      bool flag = false;
      foreach (BehaviourBase behaviour in this.behaviours)
      {
        if (behaviour is BehaviourPuppet && behaviour.enabled)
        {
          this.ActivateBehaviour(behaviour);
          flag = true;
          break;
        }
      }
      if (!flag && this.behaviours.Length != 0)
      {
        foreach (BehaviourBase behaviour in this.behaviours)
        {
          if (behaviour.enabled)
          {
            this.ActivateBehaviour(behaviour);
            break;
          }
        }
      }
      if (this.OnPostInitiate == null)
        return;
      this.OnPostInitiate();
    }

    private void ActivateBehaviour(BehaviourBase behaviour)
    {
      foreach (BehaviourBase behaviour1 in this.behaviours)
      {
        behaviour1.enabled = (UnityEngine.Object) behaviour1 == (UnityEngine.Object) behaviour;
        if (behaviour1.enabled)
          behaviour1.Activate();
      }
    }

    private void OnDestroy()
    {
      if (!((UnityEngine.Object) Singleton<PuppetMasterSettings>.instance != (UnityEngine.Object) null))
        return;
      Singleton<PuppetMasterSettings>.instance.Unregister(this);
    }

    private bool IsInterpolated()
    {
      if (!this.initiated)
        return false;
      foreach (Muscle muscle in this.muscles)
      {
        if (muscle.rigidbody.interpolation != 0)
          return true;
      }
      return false;
    }

    protected virtual void FixedUpdate()
    {
      if (!this.initiated || this.muscles.Length == 0)
        return;
      this.interpolated = this.IsInterpolated();
      this.fixedFrame = true;
      if (!this.isActive)
        return;
      this.pinWeight = Mathf.Clamp(this.pinWeight, 0.0f, 1f);
      this.muscleWeight = Mathf.Clamp(this.muscleWeight, 0.0f, 1f);
      this.muscleSpring = Mathf.Clamp(this.muscleSpring, 0.0f, this.muscleSpring);
      this.muscleDamper = Mathf.Clamp(this.muscleDamper, 0.0f, this.muscleDamper);
      this.pinPow = Mathf.Clamp(this.pinPow, 1f, 8f);
      this.pinDistanceFalloff = Mathf.Max(this.pinDistanceFalloff, 0.0f);
      if (this.updateMode == PuppetMaster.UpdateMode.FixedUpdate)
      {
        this.FixTargetTransforms();
        if (this.targetAnimator.enabled || !this.targetAnimator.enabled && this.animatorDisabled)
        {
          this.targetAnimator.enabled = false;
          this.animatorDisabled = true;
          this.targetAnimator.Update(Time.fixedDeltaTime);
        }
        else
        {
          this.animatorDisabled = false;
          this.targetAnimator.enabled = false;
        }
        foreach (SolverManager solver in this.solvers)
        {
          if ((UnityEngine.Object) solver != (UnityEngine.Object) null)
            solver.UpdateSolverExternal();
        }
        this.Read();
      }
      if (!this.isFrozen)
      {
        this.SetInternalCollisions(this.internalCollisions);
        this.SetAngularLimits(this.angularLimits);
        if (this.solverIterationCount != this.lastSolverIterationCount)
        {
          for (int index = 0; index < this.muscles.Length; ++index)
            this.muscles[index].rigidbody.solverIterations = this.solverIterationCount;
          this.lastSolverIterationCount = this.solverIterationCount;
        }
        for (int index = 0; index < this.muscles.Length; ++index)
          this.muscles[index].Update(this.pinWeight, this.muscleWeight, this.muscleSpring, this.muscleDamper, this.pinPow, this.pinDistanceFalloff, true);
      }
      if (this.updateMode != PuppetMaster.UpdateMode.AnimatePhysics)
        return;
      this.FixTargetTransforms();
    }

    protected virtual void Update()
    {
      if (!this.initiated || this.muscles.Length == 0)
        return;
      if (this.animatorDisabled)
      {
        this.targetAnimator.enabled = true;
        this.animatorDisabled = false;
      }
      if (this.updateMode != 0)
        return;
      this.FixTargetTransforms();
    }

    protected virtual void LateUpdate()
    {
      if (this.muscles.Length == 0)
        return;
      this.OnLateUpdate();
      if (this.OnPostLateUpdate == null)
        return;
      this.OnPostLateUpdate();
    }

    protected virtual void OnLateUpdate()
    {
      if (!this.initiated || this.mode == PuppetMaster.Mode.Disabled)
        return;
      if (this.animatorDisabled)
      {
        this.targetAnimator.enabled = true;
        this.animatorDisabled = false;
      }
      this.SwitchStates();
      this.SwitchModes();
      switch (this.updateMode)
      {
        case PuppetMaster.UpdateMode.Normal:
          if (this.isActive)
          {
            this.Read();
            break;
          }
          break;
        case PuppetMaster.UpdateMode.AnimatePhysics:
          if (!this.fixedFrame && !this.interpolated)
            return;
          if (this.isActive && !this.fixedFrame)
          {
            this.Read();
            break;
          }
          break;
        case PuppetMaster.UpdateMode.FixedUpdate:
          if (!this.fixedFrame && !this.interpolated)
            return;
          break;
      }
      this.fixedFrame = false;
      if (!this.isFrozen)
      {
        this.mappingWeight = Mathf.Clamp(this.mappingWeight, 0.0f, 1f);
        float mappingWeightMaster = this.mappingWeight * this.mappingBlend;
        if ((double) mappingWeightMaster > 0.0)
        {
          if (this.isActive)
          {
            for (int index = 0; index < this.muscles.Length; ++index)
              this.muscles[index].Map(mappingWeightMaster);
          }
        }
        else if (this.activeMode == PuppetMaster.Mode.Kinematic)
          this.MoveToTarget();
        foreach (BehaviourBase behaviour in this.behaviours)
          behaviour.OnWrite();
        if (this.OnWrite != null)
          this.OnWrite();
        this.StoreTargetMappedState();
        foreach (Muscle muscle in this.muscles)
          muscle.CalculateMappedVelocity();
      }
      if (!this.freezeFlag)
        return;
      this.OnFreezeFlag();
    }

    private void MoveToTarget()
    {
      if (!((UnityEngine.Object) Singleton<PuppetMasterSettings>.instance == (UnityEngine.Object) null) && (!((UnityEngine.Object) Singleton<PuppetMasterSettings>.instance != (UnityEngine.Object) null) || !Singleton<PuppetMasterSettings>.instance.UpdateMoveToTarget(this)))
        return;
      foreach (Muscle muscle in this.muscles)
        muscle.MoveToTarget();
    }

    private void Read()
    {
      if (this.teleport)
      {
        GameObject gameObject = new GameObject();
        gameObject.transform.position = (UnityEngine.Object) this.transform.parent != (UnityEngine.Object) null ? this.transform.parent.position : Vector3.zero;
        gameObject.transform.rotation = (UnityEngine.Object) this.transform.parent != (UnityEngine.Object) null ? this.transform.parent.rotation : Quaternion.identity;
        Transform parent1 = this.transform.parent;
        Transform parent2 = this.targetRoot.parent;
        this.transform.parent = gameObject.transform;
        this.targetRoot.parent = gameObject.transform;
        Vector3 position = this.transform.parent.position;
        Quaternion rotation = QuaTools.FromToRotation(this.targetRoot.rotation, this.teleportRotation);
        this.transform.parent.rotation = rotation * this.transform.parent.rotation;
        Vector3 deltaPosition = this.teleportPosition - this.targetRoot.position;
        this.transform.parent.position += deltaPosition;
        this.transform.parent = parent1;
        this.targetRoot.parent = parent2;
        UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
        this.targetMappedPositions[0] = position + rotation * (this.targetMappedPositions[0] - position) + deltaPosition;
        this.targetSampledPositions[0] = position + rotation * (this.targetSampledPositions[0] - position) + deltaPosition;
        this.targetMappedRotations[0] = rotation * this.targetMappedRotations[0];
        this.targetSampledRotations[0] = rotation * this.targetSampledRotations[0];
        if (this.teleportMoveToTarget)
        {
          foreach (Muscle muscle in this.muscles)
            muscle.MoveToTarget();
        }
        foreach (Muscle muscle in this.muscles)
          muscle.ClearVelocities();
        foreach (BehaviourBase behaviour in this.behaviours)
          behaviour.OnTeleport(rotation, deltaPosition, position, this.teleportMoveToTarget);
        this.teleport = false;
      }
      if (this.OnRead != null)
        this.OnRead();
      foreach (BehaviourBase behaviour in this.behaviours)
        behaviour.OnRead();
      if (!this.isAlive)
        return;
      foreach (Muscle muscle in this.muscles)
        muscle.Read();
      if (!this.isAlive || !this.updateJointAnchors)
        return;
      for (int index = 0; index < this.muscles.Length; ++index)
        this.muscles[index].UpdateAnchor(this.supportTranslationAnimation);
    }

    private void FixTargetTransforms()
    {
      if (!this.isAlive)
        return;
      if (this.OnFixTransforms != null)
        this.OnFixTransforms();
      foreach (BehaviourBase behaviour in this.behaviours)
        behaviour.OnFixTransforms();
      if (!this.fixTargetTransforms && !this.hasProp || !this.isActive)
        return;
      this.mappingWeight = Mathf.Clamp(this.mappingWeight, 0.0f, 1f);
      if ((double) (this.mappingWeight * this.mappingBlend) <= 0.0)
        return;
      for (int index = 0; index < this.muscles.Length; ++index)
      {
        if (this.fixTargetTransforms || this.muscles[index].props.group == Muscle.Group.Prop)
          this.muscles[index].FixTargetTransforms();
      }
    }

    private AnimatorUpdateMode targetUpdateMode
    {
      get
      {
        if ((UnityEngine.Object) this.targetAnimator != (UnityEngine.Object) null)
          return this.targetAnimator.updateMode;
        return (UnityEngine.Object) this.targetAnimation != (UnityEngine.Object) null ? (this.targetAnimation.animatePhysics ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal) : AnimatorUpdateMode.Normal;
      }
    }

    private void VisualizeTargetPose()
    {
      if (!this.visualizeTargetPose || !Application.isEditor || !this.isActive)
        return;
      foreach (Muscle muscle1 in this.muscles)
      {
        if ((UnityEngine.Object) muscle1.joint.connectedBody != (UnityEngine.Object) null && (UnityEngine.Object) muscle1.connectedBodyTarget != (UnityEngine.Object) null)
        {
          Debug.DrawLine(muscle1.target.position, muscle1.connectedBodyTarget.position, Color.cyan);
          bool flag = true;
          foreach (Muscle muscle2 in this.muscles)
          {
            if (muscle1 != muscle2 && (UnityEngine.Object) muscle2.joint.connectedBody == (UnityEngine.Object) muscle1.rigidbody)
            {
              flag = false;
              break;
            }
          }
          if (flag)
            this.VisualizeHierarchy(muscle1.target, Color.cyan);
        }
      }
    }

    private void VisualizeHierarchy(Transform t, Color color)
    {
      for (int index = 0; index < t.childCount; ++index)
      {
        Debug.DrawLine(t.position, t.GetChild(index).position, color);
        this.VisualizeHierarchy(t.GetChild(index), color);
      }
    }

    private void SetInternalCollisions(bool collide)
    {
      if (this.internalCollisionsEnabled == collide)
        return;
      for (int index1 = 0; index1 < this.muscles.Length; ++index1)
      {
        for (int index2 = index1; index2 < this.muscles.Length; ++index2)
        {
          if (index1 != index2)
            this.muscles[index1].IgnoreCollisions(this.muscles[index2], !collide);
        }
      }
      this.internalCollisionsEnabled = collide;
    }

    private void SetAngularLimits(bool limited)
    {
      if (this.angularLimitsEnabled == limited)
        return;
      for (int index = 0; index < this.muscles.Length; ++index)
        this.muscles[index].IgnoreAngularLimits(!limited);
      this.angularLimitsEnabled = limited;
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
      if (!this.CheckIfInitiated())
        return;
      if (!this.initiated)
        Debug.LogWarning((object) "PuppetMaster has not been initiated.", (UnityEngine.Object) this.transform);
      else if (this.ContainsJoint(joint))
        Debug.LogWarning((object) ("Joint " + joint.name + " is already used by a Muscle"), (UnityEngine.Object) this.transform);
      else if ((UnityEngine.Object) target == (UnityEngine.Object) null)
        Debug.LogWarning((object) "AddMuscle was called with a null 'target' reference.", (UnityEngine.Object) this.transform);
      else if ((UnityEngine.Object) connectTo == (UnityEngine.Object) joint.GetComponent<Rigidbody>())
        Debug.LogWarning((object) "ConnectTo is the joint's own Rigidbody, can not add muscle.", (UnityEngine.Object) this.transform);
      else if (!this.isActive)
      {
        Debug.LogWarning((object) "Adding muscles to inactive PuppetMasters is not currently supported.", (UnityEngine.Object) this.transform);
      }
      else
      {
        if (muscleProps == null)
          muscleProps = new Muscle.Props();
        Muscle m = new Muscle();
        m.props = muscleProps;
        m.joint = joint;
        m.target = target;
        m.joint.transform.parent = !this.hierarchyIsFlat && !((UnityEngine.Object) connectTo == (UnityEngine.Object) null) || forceTreeHierarchy ? connectTo.transform : this.transform;
        if (forceLayers)
        {
          joint.gameObject.layer = this.gameObject.layer;
          target.gameObject.layer = this.targetRoot.gameObject.layer;
        }
        if ((UnityEngine.Object) connectTo != (UnityEngine.Object) null)
        {
          m.target.parent = targetParent;
          Vector3 position = this.GetMuscle(connectTo).transform.InverseTransformPoint(m.target.position);
          Quaternion quaternion = Quaternion.Inverse(this.GetMuscle(connectTo).transform.rotation) * m.target.rotation;
          joint.transform.position = connectTo.transform.TransformPoint(position);
          joint.transform.rotation = connectTo.transform.rotation * quaternion;
          joint.connectedBody = connectTo;
        }
        m.Initiate(this.muscles);
        if ((UnityEngine.Object) connectTo != (UnityEngine.Object) null)
        {
          m.rigidbody.velocity = connectTo.velocity;
          m.rigidbody.angularVelocity = connectTo.angularVelocity;
        }
        if (!this.internalCollisions)
        {
          for (int index = 0; index < this.muscles.Length; ++index)
            m.IgnoreCollisions(this.muscles[index], true);
        }
        Array.Resize<Muscle>(ref this.muscles, this.muscles.Length + 1);
        this.muscles[this.muscles.Length - 1] = m;
        m.IgnoreAngularLimits(!this.angularLimits);
        if (this.behaviours.Length != 0)
        {
          m.broadcaster = m.joint.gameObject.AddComponent<MuscleCollisionBroadcaster>();
          m.broadcaster.puppetMaster = this;
          m.broadcaster.muscleIndex = this.muscles.Length - 1;
        }
        m.jointBreakBroadcaster = m.joint.gameObject.AddComponent<JointBreakBroadcaster>();
        m.jointBreakBroadcaster.puppetMaster = this;
        m.jointBreakBroadcaster.muscleIndex = this.muscles.Length - 1;
        this.UpdateHierarchies();
        this.CheckMassVariation(100f, true);
        foreach (BehaviourBase behaviour in this.behaviours)
          behaviour.OnMuscleAdded(m);
      }
    }

    public void RemoveMuscleRecursive(
      ConfigurableJoint joint,
      bool attachTarget,
      bool blockTargetAnimation = false,
      MuscleRemoveMode removeMode = MuscleRemoveMode.Sever)
    {
      if (!this.CheckIfInitiated())
        return;
      if ((UnityEngine.Object) joint == (UnityEngine.Object) null)
        Debug.LogWarning((object) "RemoveMuscleRecursive was called with a null 'joint' reference.", (UnityEngine.Object) this.transform);
      else if (!this.ContainsJoint(joint))
      {
        Debug.LogWarning((object) "No Muscle with the specified joint was found, can not remove muscle.", (UnityEngine.Object) this.transform);
      }
      else
      {
        int muscleIndex = this.GetMuscleIndex(joint);
        Muscle[] muscleArray = new Muscle[this.muscles.Length - (this.muscles[muscleIndex].childIndexes.Length + 1)];
        int index1 = 0;
        for (int index2 = 0; index2 < this.muscles.Length; ++index2)
        {
          if (index2 != muscleIndex && !this.muscles[muscleIndex].childFlags[index2])
          {
            muscleArray[index1] = this.muscles[index2];
            ++index1;
          }
          else
          {
            if ((UnityEngine.Object) this.muscles[index2].broadcaster != (UnityEngine.Object) null)
            {
              this.muscles[index2].broadcaster.enabled = false;
              UnityEngine.Object.Destroy((UnityEngine.Object) this.muscles[index2].broadcaster);
            }
            if ((UnityEngine.Object) this.muscles[index2].jointBreakBroadcaster != (UnityEngine.Object) null)
            {
              this.muscles[index2].jointBreakBroadcaster.enabled = false;
              UnityEngine.Object.Destroy((UnityEngine.Object) this.muscles[index2].jointBreakBroadcaster);
            }
          }
        }
        switch (removeMode)
        {
          case MuscleRemoveMode.Sever:
            this.DisconnectJoint(this.muscles[muscleIndex].joint);
            for (int index3 = 0; index3 < this.muscles[muscleIndex].childIndexes.Length; ++index3)
              this.KillJoint(this.muscles[this.muscles[muscleIndex].childIndexes[index3]].joint);
            break;
          case MuscleRemoveMode.Explode:
            this.DisconnectJoint(this.muscles[muscleIndex].joint);
            for (int index4 = 0; index4 < this.muscles[muscleIndex].childIndexes.Length; ++index4)
              this.DisconnectJoint(this.muscles[this.muscles[muscleIndex].childIndexes[index4]].joint);
            break;
          case MuscleRemoveMode.Numb:
            this.KillJoint(this.muscles[muscleIndex].joint);
            for (int index5 = 0; index5 < this.muscles[muscleIndex].childIndexes.Length; ++index5)
              this.KillJoint(this.muscles[this.muscles[muscleIndex].childIndexes[index5]].joint);
            break;
        }
        this.muscles[muscleIndex].transform.parent = (Transform) null;
        for (int index6 = 0; index6 < this.muscles[muscleIndex].childIndexes.Length; ++index6)
        {
          if (removeMode == MuscleRemoveMode.Explode || (UnityEngine.Object) this.muscles[this.muscles[muscleIndex].childIndexes[index6]].transform.parent == (UnityEngine.Object) this.transform)
            this.muscles[this.muscles[muscleIndex].childIndexes[index6]].transform.parent = (Transform) null;
        }
        foreach (BehaviourBase behaviour in this.behaviours)
        {
          behaviour.OnMuscleRemoved(this.muscles[muscleIndex]);
          for (int index7 = 0; index7 < this.muscles[muscleIndex].childIndexes.Length; ++index7)
          {
            Muscle muscle = this.muscles[this.muscles[muscleIndex].childIndexes[index7]];
            behaviour.OnMuscleRemoved(muscle);
          }
        }
        if (attachTarget)
        {
          this.muscles[muscleIndex].target.parent = this.muscles[muscleIndex].transform;
          this.muscles[muscleIndex].target.position = this.muscles[muscleIndex].transform.position;
          this.muscles[muscleIndex].target.rotation = this.muscles[muscleIndex].transform.rotation * this.muscles[muscleIndex].targetRotationRelative;
          for (int index8 = 0; index8 < this.muscles[muscleIndex].childIndexes.Length; ++index8)
          {
            Muscle muscle = this.muscles[this.muscles[muscleIndex].childIndexes[index8]];
            muscle.target.parent = muscle.transform;
            muscle.target.position = muscle.transform.position;
            muscle.target.rotation = muscle.transform.rotation;
          }
        }
        if (blockTargetAnimation)
        {
          this.muscles[muscleIndex].target.gameObject.AddComponent<AnimationBlocker>();
          for (int index9 = 0; index9 < this.muscles[muscleIndex].childIndexes.Length; ++index9)
            this.muscles[this.muscles[muscleIndex].childIndexes[index9]].target.gameObject.AddComponent<AnimationBlocker>();
        }
        if (this.OnMuscleRemoved != null)
          this.OnMuscleRemoved(this.muscles[muscleIndex]);
        for (int index10 = 0; index10 < this.muscles[muscleIndex].childIndexes.Length; ++index10)
        {
          Muscle muscle = this.muscles[this.muscles[muscleIndex].childIndexes[index10]];
          if (this.OnMuscleRemoved != null)
            this.OnMuscleRemoved(muscle);
        }
        if (!this.internalCollisionsEnabled)
        {
          foreach (Muscle muscle in muscleArray)
          {
            foreach (Collider collider1 in muscle.colliders)
            {
              foreach (Collider collider2 in this.muscles[muscleIndex].colliders)
                Physics.IgnoreCollision(collider1, collider2, false);
              for (int index11 = 0; index11 < this.muscles[muscleIndex].childIndexes.Length; ++index11)
              {
                foreach (Collider collider3 in this.muscles[index11].colliders)
                  Physics.IgnoreCollision(collider1, collider3, false);
              }
            }
          }
        }
        this.muscles = muscleArray;
        this.UpdateHierarchies();
      }
    }

    public void ReplaceMuscle(ConfigurableJoint oldJoint, ConfigurableJoint newJoint)
    {
      if (!this.CheckIfInitiated())
        return;
      Debug.LogWarning((object) "@todo", (UnityEngine.Object) this.transform);
    }

    public void SetMuscles(Muscle[] newMuscles)
    {
      if (!this.CheckIfInitiated())
        return;
      Debug.LogWarning((object) "@todo", (UnityEngine.Object) this.transform);
    }

    public void DisableMuscleRecursive(ConfigurableJoint joint)
    {
      if (!this.CheckIfInitiated())
        return;
      Debug.LogWarning((object) "@todo", (UnityEngine.Object) this.transform);
    }

    public void EnableMuscleRecursive(ConfigurableJoint joint)
    {
      if (!this.CheckIfInitiated())
        return;
      Debug.LogWarning((object) "@todo", (UnityEngine.Object) this.transform);
    }

    [ContextMenu("Flatten Muscle Hierarchy")]
    public void FlattenHierarchy()
    {
      foreach (Muscle muscle in this.muscles)
      {
        if ((UnityEngine.Object) muscle.joint != (UnityEngine.Object) null)
          muscle.joint.transform.parent = this.transform;
      }
      this.hierarchyIsFlat = true;
    }

    [ContextMenu("Tree Muscle Hierarchy")]
    public void TreeHierarchy()
    {
      foreach (Muscle muscle in this.muscles)
      {
        if ((UnityEngine.Object) muscle.joint != (UnityEngine.Object) null)
          muscle.joint.transform.parent = (UnityEngine.Object) muscle.joint.connectedBody != (UnityEngine.Object) null ? muscle.joint.connectedBody.transform : this.transform;
      }
      this.hierarchyIsFlat = false;
    }

    [ContextMenu("Fix Muscle Positions")]
    public void FixMusclePositions()
    {
      foreach (Muscle muscle in this.muscles)
      {
        if ((UnityEngine.Object) muscle.joint != (UnityEngine.Object) null && (UnityEngine.Object) muscle.target != (UnityEngine.Object) null)
          muscle.joint.transform.position = muscle.target.position;
      }
    }

    private void AddIndexesRecursive(int index, ref int[] indexes)
    {
      int length = indexes.Length;
      Array.Resize<int>(ref indexes, indexes.Length + 1 + this.muscles[index].childIndexes.Length);
      indexes[length] = index;
      if (this.muscles[index].childIndexes.Length == 0)
        return;
      for (int index1 = 0; index1 < this.muscles[index].childIndexes.Length; ++index1)
        this.AddIndexesRecursive(this.muscles[index].childIndexes[index1], ref indexes);
    }

    private bool HierarchyIsFlat()
    {
      foreach (Muscle muscle in this.muscles)
      {
        if ((UnityEngine.Object) muscle.joint.transform.parent != (UnityEngine.Object) this.transform)
          return false;
      }
      return true;
    }

    private void DisconnectJoint(ConfigurableJoint joint)
    {
      joint.connectedBody = (Rigidbody) null;
      this.KillJoint(joint);
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
      joint.slerpDrive = new JointDrive()
      {
        positionSpring = 0.0f,
        positionDamper = 0.0f
      };
    }

    public bool isSwitchingMode { get; private set; }

    public void DisableImmediately()
    {
      this.mappingBlend = 0.0f;
      this.isSwitchingMode = false;
      this.mode = PuppetMaster.Mode.Disabled;
      this.activeMode = this.mode;
      this.lastMode = this.mode;
      foreach (Muscle muscle in this.muscles)
        muscle.rigidbody.gameObject.SetActive(false);
    }

    protected virtual void SwitchModes()
    {
      if (!this.initiated)
        return;
      if (this.isKilling)
        this.mode = PuppetMaster.Mode.Active;
      if (!this.isAlive)
        this.mode = PuppetMaster.Mode.Active;
      foreach (BehaviourBase behaviour in this.behaviours)
      {
        if (behaviour.forceActive)
        {
          this.mode = PuppetMaster.Mode.Active;
          break;
        }
      }
      if (this.mode == this.lastMode || this.isSwitchingMode || this.isKilling && this.mode != 0 || this.state != PuppetMaster.State.Alive && this.mode != 0)
        return;
      this.isSwitchingMode = true;
      if (this.lastMode == PuppetMaster.Mode.Disabled)
      {
        if (this.mode == PuppetMaster.Mode.Kinematic)
          this.DisabledToKinematic();
        else if (this.mode == PuppetMaster.Mode.Active)
          this.StartCoroutine(this.DisabledToActive());
      }
      else if (this.lastMode == PuppetMaster.Mode.Kinematic)
      {
        if (this.mode == PuppetMaster.Mode.Disabled)
          this.KinematicToDisabled();
        else if (this.mode == PuppetMaster.Mode.Active)
          this.StartCoroutine(this.KinematicToActive());
      }
      else if (this.lastMode == PuppetMaster.Mode.Active)
      {
        if (this.mode == PuppetMaster.Mode.Disabled)
          this.StartCoroutine(this.ActiveToDisabled());
        else if (this.mode == PuppetMaster.Mode.Kinematic)
          this.StartCoroutine(this.ActiveToKinematic());
      }
      this.lastMode = this.mode;
    }

    private void DisabledToKinematic()
    {
      foreach (Muscle muscle in this.muscles)
        muscle.Reset();
      foreach (Muscle muscle in this.muscles)
      {
        muscle.rigidbody.gameObject.SetActive(true);
        muscle.rigidbody.isKinematic = true;
      }
      this.internalCollisionsEnabled = true;
      this.SetInternalCollisions(this.internalCollisions);
      foreach (Muscle muscle in this.muscles)
        muscle.MoveToTarget();
      this.activeMode = PuppetMaster.Mode.Kinematic;
      this.isSwitchingMode = false;
    }

    private IEnumerator DisabledToActive()
    {
      Muscle[] muscleArray1 = this.muscles;
      for (int index = 0; index < muscleArray1.Length; ++index)
      {
        Muscle m = muscleArray1[index];
        if (!m.rigidbody.gameObject.activeInHierarchy)
          m.Reset();
        m = (Muscle) null;
      }
      muscleArray1 = (Muscle[]) null;
      Muscle[] muscleArray2 = this.muscles;
      for (int index = 0; index < muscleArray2.Length; ++index)
      {
        Muscle m = muscleArray2[index];
        m.rigidbody.gameObject.SetActive(true);
        m.rigidbody.isKinematic = false;
        m.rigidbody.WakeUp();
        m.rigidbody.velocity = m.mappedVelocity;
        m.rigidbody.angularVelocity = m.mappedAngularVelocity;
        m = (Muscle) null;
      }
      muscleArray2 = (Muscle[]) null;
      this.internalCollisionsEnabled = true;
      this.SetInternalCollisions(this.internalCollisions);
      this.Read();
      Muscle[] muscleArray3 = this.muscles;
      for (int index = 0; index < muscleArray3.Length; ++index)
      {
        Muscle m = muscleArray3[index];
        m.MoveToTarget();
        m = (Muscle) null;
      }
      muscleArray3 = (Muscle[]) null;
      this.UpdateInternalCollisions();
      while ((double) this.mappingBlend < 1.0)
      {
        this.mappingBlend = Mathf.Clamp(this.mappingBlend + Time.deltaTime / this.blendTime, 0.0f, 1f);
        yield return (object) null;
      }
      this.activeMode = PuppetMaster.Mode.Active;
      this.isSwitchingMode = false;
    }

    private void KinematicToDisabled()
    {
      foreach (Muscle muscle in this.muscles)
        muscle.rigidbody.gameObject.SetActive(false);
      this.activeMode = PuppetMaster.Mode.Disabled;
      this.isSwitchingMode = false;
    }

    private IEnumerator KinematicToActive()
    {
      Muscle[] muscleArray1 = this.muscles;
      for (int index = 0; index < muscleArray1.Length; ++index)
      {
        Muscle m = muscleArray1[index];
        m.rigidbody.isKinematic = false;
        m.rigidbody.WakeUp();
        m.rigidbody.velocity = m.mappedVelocity;
        m.rigidbody.angularVelocity = m.mappedAngularVelocity;
        m = (Muscle) null;
      }
      muscleArray1 = (Muscle[]) null;
      this.Read();
      Muscle[] muscleArray2 = this.muscles;
      for (int index = 0; index < muscleArray2.Length; ++index)
      {
        Muscle m = muscleArray2[index];
        m.MoveToTarget();
        m = (Muscle) null;
      }
      muscleArray2 = (Muscle[]) null;
      this.UpdateInternalCollisions();
      while ((double) this.mappingBlend < 1.0)
      {
        this.mappingBlend = Mathf.Min(this.mappingBlend + Time.deltaTime / this.blendTime, 1f);
        yield return (object) null;
      }
      this.activeMode = PuppetMaster.Mode.Active;
      this.isSwitchingMode = false;
    }

    private IEnumerator ActiveToDisabled()
    {
      while ((double) this.mappingBlend > 0.0)
      {
        this.mappingBlend = Mathf.Max(this.mappingBlend - Time.deltaTime / this.blendTime, 0.0f);
        yield return (object) null;
      }
      Muscle[] muscleArray = this.muscles;
      for (int index = 0; index < muscleArray.Length; ++index)
      {
        Muscle m = muscleArray[index];
        m.rigidbody.gameObject.SetActive(false);
        m = (Muscle) null;
      }
      muscleArray = (Muscle[]) null;
      this.activeMode = PuppetMaster.Mode.Disabled;
      this.isSwitchingMode = false;
    }

    private IEnumerator ActiveToKinematic()
    {
      while ((double) this.mappingBlend > 0.0)
      {
        this.mappingBlend = Mathf.Max(this.mappingBlend - Time.deltaTime / this.blendTime, 0.0f);
        yield return (object) null;
      }
      Muscle[] muscleArray1 = this.muscles;
      for (int index = 0; index < muscleArray1.Length; ++index)
      {
        Muscle m = muscleArray1[index];
        m.rigidbody.isKinematic = true;
        m = (Muscle) null;
      }
      muscleArray1 = (Muscle[]) null;
      Muscle[] muscleArray2 = this.muscles;
      for (int index = 0; index < muscleArray2.Length; ++index)
      {
        Muscle m = muscleArray2[index];
        m.MoveToTarget();
        m = (Muscle) null;
      }
      muscleArray2 = (Muscle[]) null;
      this.activeMode = PuppetMaster.Mode.Kinematic;
      this.isSwitchingMode = false;
    }

    private void UpdateInternalCollisions()
    {
      if (this.internalCollisions)
        return;
      for (int index1 = 0; index1 < this.muscles.Length; ++index1)
      {
        for (int index2 = index1; index2 < this.muscles.Length; ++index2)
        {
          if (index1 != index2)
            this.muscles[index1].IgnoreCollisions(this.muscles[index2], true);
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
      if (!this.CheckIfInitiated())
        return;
      foreach (Muscle muscle in this.muscles)
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
      if (!this.CheckIfInitiated())
        return;
      int muscleIndex = this.GetMuscleIndex(target);
      if (muscleIndex == -1)
        return;
      this.SetMuscleWeights(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
    }

    public void SetMuscleWeights(
      HumanBodyBones humanBodyBone,
      float muscleWeight,
      float pinWeight = 1f,
      float mappingWeight = 1f,
      float muscleDamper = 1f)
    {
      if (!this.CheckIfInitiated())
        return;
      int muscleIndex = this.GetMuscleIndex(humanBodyBone);
      if (muscleIndex == -1)
        return;
      this.SetMuscleWeights(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
    }

    public void SetMuscleWeightsRecursive(
      Transform target,
      float muscleWeight,
      float pinWeight = 1f,
      float mappingWeight = 1f,
      float muscleDamper = 1f)
    {
      if (!this.CheckIfInitiated())
        return;
      for (int muscleIndex = 0; muscleIndex < this.muscles.Length; ++muscleIndex)
      {
        if ((UnityEngine.Object) this.muscles[muscleIndex].target == (UnityEngine.Object) target)
        {
          this.SetMuscleWeightsRecursive(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
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
      if (!this.CheckIfInitiated())
        return;
      this.SetMuscleWeights(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
      for (int index = 0; index < this.muscles[muscleIndex].childIndexes.Length; ++index)
        this.SetMuscleWeights(this.muscles[muscleIndex].childIndexes[index], muscleWeight, pinWeight, mappingWeight, muscleDamper);
    }

    public void SetMuscleWeightsRecursive(
      HumanBodyBones humanBodyBone,
      float muscleWeight,
      float pinWeight = 1f,
      float mappingWeight = 1f,
      float muscleDamper = 1f)
    {
      if (!this.CheckIfInitiated())
        return;
      int muscleIndex = this.GetMuscleIndex(humanBodyBone);
      if (muscleIndex == -1)
        return;
      this.SetMuscleWeightsRecursive(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
    }

    public void SetMuscleWeights(
      int muscleIndex,
      float muscleWeight,
      float pinWeight,
      float mappingWeight,
      float muscleDamper)
    {
      if (!this.CheckIfInitiated())
        return;
      if ((double) muscleIndex < 0.0 || muscleIndex >= this.muscles.Length)
      {
        Debug.LogWarning((object) ("Muscle index out of range (" + (object) muscleIndex + ")."), (UnityEngine.Object) this.transform);
      }
      else
      {
        this.muscles[muscleIndex].props.muscleWeight = muscleWeight;
        this.muscles[muscleIndex].props.pinWeight = pinWeight;
        this.muscles[muscleIndex].props.mappingWeight = mappingWeight;
        this.muscles[muscleIndex].props.muscleDamper = muscleDamper;
      }
    }

    public Muscle GetMuscle(Transform target)
    {
      int muscleIndex = this.GetMuscleIndex(target);
      return muscleIndex == -1 ? (Muscle) null : this.muscles[muscleIndex];
    }

    public Muscle GetMuscle(Rigidbody rigidbody)
    {
      int muscleIndex = this.GetMuscleIndex(rigidbody);
      return muscleIndex == -1 ? (Muscle) null : this.muscles[muscleIndex];
    }

    public Muscle GetMuscle(ConfigurableJoint joint)
    {
      int muscleIndex = this.GetMuscleIndex(joint);
      return muscleIndex == -1 ? (Muscle) null : this.muscles[muscleIndex];
    }

    public bool ContainsJoint(ConfigurableJoint joint)
    {
      if (!this.CheckIfInitiated())
        return false;
      foreach (Muscle muscle in this.muscles)
      {
        if ((UnityEngine.Object) muscle.joint == (UnityEngine.Object) joint)
          return true;
      }
      return false;
    }

    public int GetMuscleIndex(HumanBodyBones humanBodyBone)
    {
      if (!this.CheckIfInitiated())
        return -1;
      if ((UnityEngine.Object) this.targetAnimator == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "PuppetMaster 'Target Root' has no Animator component on it nor on it's children.", (UnityEngine.Object) this.transform);
        return -1;
      }
      if (!this.targetAnimator.isHuman)
      {
        Debug.LogWarning((object) "PuppetMaster target's Animator does not belong to a Humanoid, can hot get human muscle index.", (UnityEngine.Object) this.transform);
        return -1;
      }
      Transform boneTransform = this.targetAnimator.GetBoneTransform(humanBodyBone);
      if (!((UnityEngine.Object) boneTransform == (UnityEngine.Object) null))
        return this.GetMuscleIndex(boneTransform);
      Debug.LogWarning((object) ("PuppetMaster target's Avatar does not contain a bone Transform for " + (object) humanBodyBone), (UnityEngine.Object) this.transform);
      return -1;
    }

    public int GetMuscleIndex(Transform target)
    {
      if (!this.CheckIfInitiated())
        return -1;
      if ((UnityEngine.Object) target == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "Target is null, can not get muscle index.", (UnityEngine.Object) this.transform);
        return -1;
      }
      for (int muscleIndex = 0; muscleIndex < this.muscles.Length; ++muscleIndex)
      {
        if ((UnityEngine.Object) this.muscles[muscleIndex].target == (UnityEngine.Object) target)
          return muscleIndex;
      }
      Debug.LogWarning((object) ("No muscle with target " + target.name + "found on the PuppetMaster."), (UnityEngine.Object) this.transform);
      return -1;
    }

    public int GetMuscleIndex(Rigidbody rigidbody)
    {
      if (!this.CheckIfInitiated())
        return -1;
      if ((UnityEngine.Object) rigidbody == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "Rigidbody is null, can not get muscle index.", (UnityEngine.Object) this.transform);
        return -1;
      }
      for (int muscleIndex = 0; muscleIndex < this.muscles.Length; ++muscleIndex)
      {
        if ((UnityEngine.Object) this.muscles[muscleIndex].rigidbody == (UnityEngine.Object) rigidbody)
          return muscleIndex;
      }
      Debug.LogWarning((object) ("No muscle with Rigidbody " + rigidbody.name + "found on the PuppetMaster."), (UnityEngine.Object) this.transform);
      return -1;
    }

    public int GetMuscleIndex(ConfigurableJoint joint)
    {
      if (!this.CheckIfInitiated())
        return -1;
      if ((UnityEngine.Object) joint == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "Joint is null, can not get muscle index.", (UnityEngine.Object) this.transform);
        return -1;
      }
      for (int muscleIndex = 0; muscleIndex < this.muscles.Length; ++muscleIndex)
      {
        if ((UnityEngine.Object) this.muscles[muscleIndex].joint == (UnityEngine.Object) joint)
          return muscleIndex;
      }
      Debug.LogWarning((object) ("No muscle with Joint " + joint.name + "found on the PuppetMaster."), (UnityEngine.Object) this.transform);
      return -1;
    }

    public static PuppetMaster SetUp(
      Transform target,
      Transform ragdoll,
      int characterControllerLayer,
      int ragdollLayer)
    {
      if (!((UnityEngine.Object) ragdoll != (UnityEngine.Object) target))
        return PuppetMaster.SetUp(ragdoll, characterControllerLayer, ragdollLayer);
      PuppetMaster puppetMaster = ragdoll.gameObject.AddComponent<PuppetMaster>();
      puppetMaster.SetUpTo(target, characterControllerLayer, ragdollLayer);
      return puppetMaster;
    }

    public static PuppetMaster SetUp(
      Transform target,
      int characterControllerLayer,
      int ragdollLayer)
    {
      PuppetMaster puppetMaster = UnityEngine.Object.Instantiate<GameObject>(target.gameObject, target.position, target.rotation).transform.gameObject.AddComponent<PuppetMaster>();
      puppetMaster.SetUpTo(target, characterControllerLayer, ragdollLayer);
      PuppetMaster.RemoveRagdollComponents(target, characterControllerLayer);
      return puppetMaster;
    }

    public void SetUpTo(Transform setUpTo, int characterControllerLayer, int ragdollLayer)
    {
      if ((UnityEngine.Object) setUpTo == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "SetUpTo is null. Can not set the PuppetMaster up to a null Transform.", (UnityEngine.Object) this.transform);
      }
      else
      {
        if ((UnityEngine.Object) setUpTo == (UnityEngine.Object) this.transform)
        {
          setUpTo = UnityEngine.Object.Instantiate<GameObject>(setUpTo.gameObject, setUpTo.position, setUpTo.rotation).transform;
          setUpTo.name = this.name;
          PuppetMaster.RemoveRagdollComponents(setUpTo, characterControllerLayer);
        }
        this.RemoveUnnecessaryBones();
        Component[] componentsInChildren = this.GetComponentsInChildren<Component>();
        for (int index = 0; index < componentsInChildren.Length; ++index)
        {
          if (!(componentsInChildren[index] is PuppetMaster) && !(componentsInChildren[index] is Transform) && !(componentsInChildren[index] is Rigidbody) && !(componentsInChildren[index] is BoxCollider) && !(componentsInChildren[index] is CapsuleCollider) && !(componentsInChildren[index] is SphereCollider) && !(componentsInChildren[index] is MeshCollider) && !(componentsInChildren[index] is Joint) && !(componentsInChildren[index] is Animator))
            UnityEngine.Object.DestroyImmediate((UnityEngine.Object) componentsInChildren[index]);
        }
        foreach (UnityEngine.Object componentsInChild in this.GetComponentsInChildren<Animator>())
          UnityEngine.Object.DestroyImmediate(componentsInChild);
        Component[] components = this.transform.GetComponents<Component>();
        for (int index = 0; index < components.Length; ++index)
        {
          if (!(components[index] is PuppetMaster) && !(components[index] is Transform))
            UnityEngine.Object.DestroyImmediate((UnityEngine.Object) components[index]);
        }
        foreach (Rigidbody componentsInChild in this.transform.GetComponentsInChildren<Rigidbody>())
        {
          if ((UnityEngine.Object) componentsInChild.transform != (UnityEngine.Object) this.transform && (UnityEngine.Object) componentsInChild.GetComponent<ConfigurableJoint>() == (UnityEngine.Object) null)
            componentsInChild.gameObject.AddComponent<ConfigurableJoint>();
        }
        this.targetRoot = setUpTo;
        this.SetUpMuscles(setUpTo);
        this.name = nameof (PuppetMaster);
        Transform transform1 = (UnityEngine.Object) setUpTo.parent == (UnityEngine.Object) null || (UnityEngine.Object) setUpTo.parent != (UnityEngine.Object) this.transform.parent || setUpTo.parent.name != setUpTo.name + " Root" ? new GameObject(setUpTo.name + " Root").transform : setUpTo.parent;
        transform1.parent = this.transform.parent;
        Transform transform2 = new GameObject("Behaviours").transform;
        Comments comments = transform2.gameObject.GetComponent<Comments>();
        if ((UnityEngine.Object) comments == (UnityEngine.Object) null)
          comments = transform2.gameObject.AddComponent<Comments>();
        comments.text = "All Puppet Behaviours should be parented to this GameObject, the PuppetMaster will automatically find them from here. All Puppet Behaviours have been designed so that they could be simply copied from one character to another without changing any references. It is important because they contain a lot of parameters and would be otherwise tedious to set up and tweak.";
        transform1.position = setUpTo.position;
        transform1.rotation = setUpTo.rotation;
        transform2.position = setUpTo.position;
        transform2.rotation = setUpTo.rotation;
        this.transform.position = setUpTo.position;
        this.transform.rotation = setUpTo.rotation;
        transform2.parent = transform1;
        this.transform.parent = transform1;
        setUpTo.parent = transform1;
        this.targetRoot.gameObject.layer = characterControllerLayer;
        this.gameObject.layer = ragdollLayer;
        foreach (Muscle muscle in this.muscles)
          muscle.joint.gameObject.layer = ragdollLayer;
        Physics.IgnoreLayerCollision(characterControllerLayer, ragdollLayer);
      }
    }

    public static void RemoveRagdollComponents(Transform target, int characterControllerLayer)
    {
      if ((UnityEngine.Object) target == (UnityEngine.Object) null)
        return;
      Rigidbody[] componentsInChildren1 = target.GetComponentsInChildren<Rigidbody>();
      Cloth[] componentsInChildren2 = target.GetComponentsInChildren<Cloth>();
      for (int index = 0; index < componentsInChildren1.Length; ++index)
      {
        if ((UnityEngine.Object) componentsInChildren1[index].gameObject != (UnityEngine.Object) target.gameObject)
        {
          Joint component1 = componentsInChildren1[index].GetComponent<Joint>();
          Collider component2 = componentsInChildren1[index].GetComponent<Collider>();
          if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
            UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component1);
          if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
          {
            if (!PuppetMaster.IsClothCollider(component2, componentsInChildren2))
              UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component2);
            else
              component2.gameObject.layer = characterControllerLayer;
          }
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) componentsInChildren1[index]);
        }
      }
      Collider[] componentsInChildren3 = target.GetComponentsInChildren<Collider>();
      for (int index = 0; index < componentsInChildren3.Length; ++index)
      {
        if ((UnityEngine.Object) componentsInChildren3[index].transform != (UnityEngine.Object) target && !PuppetMaster.IsClothCollider(componentsInChildren3[index], componentsInChildren2))
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) componentsInChildren3[index]);
      }
      PuppetMaster component = target.GetComponent<PuppetMaster>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component);
    }

    private void SetUpMuscles(Transform setUpTo)
    {
      ConfigurableJoint[] componentsInChildren1 = this.transform.GetComponentsInChildren<ConfigurableJoint>();
      if (componentsInChildren1.Length == 0)
      {
        Debug.LogWarning((object) "No ConfigurableJoints found, can not build PuppetMaster. Please create ConfigurableJoints to connect the ragdoll bones together.", (UnityEngine.Object) this.transform);
      }
      else
      {
        Animator componentInChildren = this.targetRoot.GetComponentInChildren<Animator>();
        Transform[] componentsInChildren2 = setUpTo.GetComponentsInChildren<Transform>();
        this.muscles = new Muscle[componentsInChildren1.Length];
        int index1 = -1;
        for (int index2 = 0; index2 < componentsInChildren1.Length; ++index2)
        {
          this.muscles[index2] = new Muscle();
          this.muscles[index2].joint = componentsInChildren1[index2];
          this.muscles[index2].name = componentsInChildren1[index2].name;
          this.muscles[index2].props = new Muscle.Props(1f, 1f, 1f, 1f, (UnityEngine.Object) this.muscles[index2].joint.connectedBody == (UnityEngine.Object) null);
          if ((UnityEngine.Object) this.muscles[index2].joint.connectedBody == (UnityEngine.Object) null && index1 == -1)
            index1 = index2;
          foreach (Transform transform in componentsInChildren2)
          {
            if (transform.name == componentsInChildren1[index2].name)
            {
              this.muscles[index2].target = transform;
              if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
              {
                this.muscles[index2].props.group = PuppetMaster.FindGroup(componentInChildren, this.muscles[index2].target);
                if (this.muscles[index2].props.group == Muscle.Group.Hips || this.muscles[index2].props.group == Muscle.Group.Leg || this.muscles[index2].props.group == Muscle.Group.Foot)
                  this.muscles[index2].props.mapPosition = true;
                break;
              }
              break;
            }
          }
        }
        if (index1 != 0)
        {
          Muscle muscle1 = this.muscles[0];
          Muscle muscle2 = this.muscles[index1];
          this.muscles[index1] = muscle1;
          this.muscles[0] = muscle2;
        }
        bool flag = true;
        foreach (Muscle muscle in this.muscles)
        {
          if ((UnityEngine.Object) muscle.target == (UnityEngine.Object) null)
            Debug.LogWarning((object) ("No target Transform found for PuppetMaster muscle " + muscle.joint.name + ". Please assign manually."), (UnityEngine.Object) this.transform);
          if (muscle.props.group != this.muscles[0].props.group)
            flag = false;
        }
        if (!flag)
          return;
        Debug.LogWarning((object) "Muscle groups need to be assigned in the PuppetMaster!", (UnityEngine.Object) this.transform);
      }
    }

    private static Muscle.Group FindGroup(Animator animator, Transform t)
    {
      if (!animator.isHuman)
        return Muscle.Group.Hips;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.Chest))
        return Muscle.Group.Spine;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.Head))
        return Muscle.Group.Head;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.Hips))
        return Muscle.Group.Hips;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.LeftFoot))
        return Muscle.Group.Foot;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.LeftHand))
        return Muscle.Group.Hand;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.LeftLowerArm))
        return Muscle.Group.Arm;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg))
        return Muscle.Group.Leg;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.LeftUpperArm))
        return Muscle.Group.Arm;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg))
        return Muscle.Group.Leg;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.RightFoot))
        return Muscle.Group.Foot;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.RightHand))
        return Muscle.Group.Hand;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.RightLowerArm))
        return Muscle.Group.Arm;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.RightLowerLeg))
        return Muscle.Group.Leg;
      if ((UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.RightUpperArm))
        return Muscle.Group.Arm;
      return (UnityEngine.Object) t == (UnityEngine.Object) animator.GetBoneTransform(HumanBodyBones.RightUpperLeg) ? Muscle.Group.Leg : Muscle.Group.Spine;
    }

    private void RemoveUnnecessaryBones()
    {
      Transform[] componentsInChildren = this.GetComponentsInChildren<Transform>();
      for (int index1 = 1; index1 < componentsInChildren.Length; ++index1)
      {
        bool flag = false;
        if ((UnityEngine.Object) componentsInChildren[index1].GetComponent<Rigidbody>() != (UnityEngine.Object) null || (UnityEngine.Object) componentsInChildren[index1].GetComponent<ConfigurableJoint>() != (UnityEngine.Object) null)
          flag = true;
        if ((UnityEngine.Object) componentsInChildren[index1].GetComponent<Collider>() != (UnityEngine.Object) null && (UnityEngine.Object) componentsInChildren[index1].GetComponent<Rigidbody>() == (UnityEngine.Object) null)
          flag = true;
        if ((UnityEngine.Object) componentsInChildren[index1].GetComponent<CharacterController>() != (UnityEngine.Object) null)
          flag = false;
        if (!flag)
        {
          Transform[] transformArray = new Transform[componentsInChildren[index1].childCount];
          for (int index2 = 0; index2 < transformArray.Length; ++index2)
            transformArray[index2] = componentsInChildren[index1].GetChild(index2);
          for (int index3 = 0; index3 < transformArray.Length; ++index3)
            transformArray[index3].parent = componentsInChildren[index1].parent;
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) componentsInChildren[index1].gameObject);
        }
      }
    }

    private static bool IsClothCollider(Collider collider, Cloth[] cloths)
    {
      if (cloths == null)
        return false;
      foreach (Cloth cloth in cloths)
      {
        if ((UnityEngine.Object) cloth == (UnityEngine.Object) null)
          return false;
        foreach (CapsuleCollider capsuleCollider in cloth.capsuleColliders)
        {
          if ((UnityEngine.Object) capsuleCollider != (UnityEngine.Object) null && (UnityEngine.Object) capsuleCollider.gameObject == (UnityEngine.Object) collider.gameObject)
            return true;
        }
        foreach (ClothSphereColliderPair sphereCollider in cloth.sphereColliders)
        {
          if ((UnityEngine.Object) sphereCollider.first != (UnityEngine.Object) null && (UnityEngine.Object) sphereCollider.first.gameObject == (UnityEngine.Object) collider.gameObject || (UnityEngine.Object) sphereCollider.second != (UnityEngine.Object) null && (UnityEngine.Object) sphereCollider.second.gameObject == (UnityEngine.Object) collider.gameObject)
            return true;
        }
      }
      return false;
    }

    public bool isSwitchingState => this.activeState != this.state;

    public bool isKilling { get; private set; }

    public bool isAlive => this.activeState == PuppetMaster.State.Alive;

    public bool isFrozen => this.activeState == PuppetMaster.State.Frozen;

    public void Kill() => this.state = PuppetMaster.State.Dead;

    public void Kill(PuppetMaster.StateSettings stateSettings)
    {
      this.stateSettings = stateSettings;
      this.state = PuppetMaster.State.Dead;
    }

    public void Freeze() => this.state = PuppetMaster.State.Frozen;

    public void Freeze(PuppetMaster.StateSettings stateSettings)
    {
      this.stateSettings = stateSettings;
      this.state = PuppetMaster.State.Frozen;
    }

    public void Resurrect() => this.state = PuppetMaster.State.Alive;

    protected virtual void SwitchStates()
    {
      if (this.state == this.lastState || this.isKilling)
        return;
      if (this.freezeFlag)
      {
        if (this.state == PuppetMaster.State.Alive)
        {
          this.activeState = PuppetMaster.State.Dead;
          this.lastState = PuppetMaster.State.Dead;
          this.freezeFlag = false;
        }
        else if (this.state == PuppetMaster.State.Dead)
        {
          this.lastState = PuppetMaster.State.Dead;
          this.freezeFlag = false;
          return;
        }
        if (this.freezeFlag)
          return;
      }
      if (this.lastState == PuppetMaster.State.Alive)
      {
        if (this.state == PuppetMaster.State.Dead)
          this.StartCoroutine(this.AliveToDead(false));
        else if (this.state == PuppetMaster.State.Frozen)
          this.StartCoroutine(this.AliveToDead(true));
      }
      else if (this.lastState == PuppetMaster.State.Dead)
      {
        if (this.state == PuppetMaster.State.Alive)
          this.DeadToAlive();
        else if (this.state == PuppetMaster.State.Frozen)
          this.DeadToFrozen();
      }
      else if (this.lastState == PuppetMaster.State.Frozen)
      {
        if (this.state == PuppetMaster.State.Alive)
          this.FrozenToAlive();
        else if (this.state == PuppetMaster.State.Dead)
          this.FrozenToDead();
      }
      this.lastState = this.state;
    }

    private IEnumerator AliveToDead(bool freeze)
    {
      this.isKilling = true;
      this.mode = PuppetMaster.Mode.Active;
      if (this.stateSettings.enableAngularLimitsOnKill && !this.angularLimits)
      {
        this.angularLimits = true;
        this.angularLimitsEnabledOnKill = true;
      }
      if (this.stateSettings.enableInternalCollisionsOnKill && !this.internalCollisions)
      {
        this.internalCollisions = true;
        this.internalCollisionsEnabledOnKill = true;
      }
      Muscle[] muscleArray1 = this.muscles;
      for (int index = 0; index < muscleArray1.Length; ++index)
      {
        Muscle m = muscleArray1[index];
        m.state.pinWeightMlp = 0.0f;
        m.state.muscleDamperAdd = this.stateSettings.deadMuscleDamper;
        m.rigidbody.velocity = m.mappedVelocity;
        m.rigidbody.angularVelocity = m.mappedAngularVelocity;
        m = (Muscle) null;
      }
      muscleArray1 = (Muscle[]) null;
      float range = this.muscles[0].state.muscleWeightMlp - this.stateSettings.deadMuscleWeight;
      BehaviourBase[] behaviourBaseArray1 = this.behaviours;
      for (int index = 0; index < behaviourBaseArray1.Length; ++index)
      {
        BehaviourBase behaviour = behaviourBaseArray1[index];
        behaviour.KillStart();
        behaviour = (BehaviourBase) null;
      }
      behaviourBaseArray1 = (BehaviourBase[]) null;
      if ((double) this.stateSettings.killDuration > 0.0 && (double) range > 0.0)
      {
        float mW = this.muscles[0].state.muscleWeightMlp;
        while ((double) mW > (double) this.stateSettings.deadMuscleWeight)
        {
          mW = Mathf.Max(mW - Time.deltaTime * (range / this.stateSettings.killDuration), this.stateSettings.deadMuscleWeight);
          Muscle[] muscleArray2 = this.muscles;
          for (int index = 0; index < muscleArray2.Length; ++index)
          {
            Muscle m = muscleArray2[index];
            m.state.muscleWeightMlp = mW;
            m = (Muscle) null;
          }
          muscleArray2 = (Muscle[]) null;
          yield return (object) null;
        }
      }
      Muscle[] muscleArray3 = this.muscles;
      for (int index = 0; index < muscleArray3.Length; ++index)
      {
        Muscle m = muscleArray3[index];
        m.state.muscleWeightMlp = this.stateSettings.deadMuscleWeight;
        m = (Muscle) null;
      }
      muscleArray3 = (Muscle[]) null;
      this.SetAnimationEnabled(false);
      this.isKilling = false;
      this.activeState = PuppetMaster.State.Dead;
      if (freeze)
        this.freezeFlag = true;
      BehaviourBase[] behaviourBaseArray2 = this.behaviours;
      for (int index = 0; index < behaviourBaseArray2.Length; ++index)
      {
        BehaviourBase behaviour = behaviourBaseArray2[index];
        behaviour.KillEnd();
        behaviour = (BehaviourBase) null;
      }
      behaviourBaseArray2 = (BehaviourBase[]) null;
      if (this.OnDeath != null)
        this.OnDeath();
    }

    private void OnFreezeFlag()
    {
      if (!this.CanFreeze())
        return;
      this.SetAnimationEnabled(false);
      foreach (Muscle muscle in this.muscles)
        muscle.joint.gameObject.SetActive(false);
      foreach (BehaviourBase behaviour in this.behaviours)
      {
        behaviour.Freeze();
        if (behaviour.gameObject.activeSelf)
        {
          behaviour.deactivated = true;
          behaviour.gameObject.SetActive(false);
        }
      }
      this.freezeFlag = false;
      this.activeState = PuppetMaster.State.Frozen;
      if (this.OnFreeze != null)
        this.OnFreeze();
      if (!this.stateSettings.freezePermanently)
        return;
      if (this.behaviours.Length != 0 && (UnityEngine.Object) this.behaviours[0] != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.behaviours[0].transform.parent.gameObject);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    private void DeadToAlive()
    {
      foreach (Muscle muscle in this.muscles)
      {
        muscle.state.pinWeightMlp = 1f;
        muscle.state.muscleWeightMlp = 1f;
        muscle.state.muscleDamperAdd = 0.0f;
      }
      if (this.angularLimitsEnabledOnKill)
      {
        this.angularLimits = false;
        this.angularLimitsEnabledOnKill = false;
      }
      if (this.internalCollisionsEnabledOnKill)
      {
        this.internalCollisions = false;
        this.internalCollisionsEnabledOnKill = false;
      }
      foreach (BehaviourBase behaviour in this.behaviours)
        behaviour.Resurrect();
      this.SetAnimationEnabled(true);
      this.activeState = PuppetMaster.State.Alive;
      if (this.OnResurrection == null)
        return;
      this.OnResurrection();
    }

    private void SetAnimationEnabled(bool to)
    {
      this.animatorDisabled = false;
      if ((UnityEngine.Object) this.targetAnimator != (UnityEngine.Object) null)
        this.targetAnimator.enabled = to;
      if (!((UnityEngine.Object) this.targetAnimation != (UnityEngine.Object) null))
        return;
      this.targetAnimation.enabled = to;
    }

    private void DeadToFrozen() => this.freezeFlag = true;

    private void FrozenToAlive()
    {
      this.freezeFlag = false;
      foreach (Muscle muscle in this.muscles)
      {
        muscle.state.pinWeightMlp = 1f;
        muscle.state.muscleWeightMlp = 1f;
        muscle.state.muscleDamperAdd = 0.0f;
      }
      if (this.angularLimitsEnabledOnKill)
      {
        this.angularLimits = false;
        this.angularLimitsEnabledOnKill = false;
      }
      if (this.internalCollisionsEnabledOnKill)
      {
        this.internalCollisions = false;
        this.internalCollisionsEnabledOnKill = false;
      }
      this.ActivateRagdoll();
      foreach (BehaviourBase behaviour in this.behaviours)
      {
        behaviour.Unfreeze();
        behaviour.Resurrect();
        if (behaviour.deactivated)
          behaviour.gameObject.SetActive(true);
      }
      if ((UnityEngine.Object) this.targetAnimator != (UnityEngine.Object) null)
        this.targetAnimator.enabled = true;
      if ((UnityEngine.Object) this.targetAnimation != (UnityEngine.Object) null)
        this.targetAnimation.enabled = true;
      this.activeState = PuppetMaster.State.Alive;
      if (this.OnUnfreeze != null)
        this.OnUnfreeze();
      if (this.OnResurrection == null)
        return;
      this.OnResurrection();
    }

    private void FrozenToDead()
    {
      this.freezeFlag = false;
      this.ActivateRagdoll();
      foreach (BehaviourBase behaviour in this.behaviours)
      {
        behaviour.Unfreeze();
        if (behaviour.deactivated)
          behaviour.gameObject.SetActive(true);
      }
      this.activeState = PuppetMaster.State.Dead;
      if (this.OnUnfreeze == null)
        return;
      this.OnUnfreeze();
    }

    private void ActivateRagdoll(bool kinematic = false)
    {
      foreach (Muscle muscle in this.muscles)
        muscle.Reset();
      foreach (Muscle muscle in this.muscles)
      {
        muscle.joint.gameObject.SetActive(true);
        muscle.rigidbody.isKinematic = kinematic;
        muscle.rigidbody.velocity = Vector3.zero;
        muscle.rigidbody.angularVelocity = Vector3.zero;
      }
      this.internalCollisionsEnabled = true;
      this.SetInternalCollisions(this.internalCollisions);
      this.Read();
      foreach (Muscle muscle in this.muscles)
        muscle.MoveToTarget();
    }

    private bool CanFreeze()
    {
      foreach (Muscle muscle in this.muscles)
      {
        if ((double) muscle.rigidbody.velocity.sqrMagnitude > (double) this.stateSettings.maxFreezeSqrVelocity)
          return false;
      }
      return true;
    }

    public void SampleTargetMappedState()
    {
      if (!this.CheckIfInitiated())
        return;
      this.sampleTargetMappedState = true;
      if (!this.targetMappedStateStored)
      {
        this.sampleTargetMappedState = true;
      }
      else
      {
        for (int index = 0; index < this.targetChildren.Length; ++index)
        {
          this.targetSampledPositions[index] = this.targetMappedPositions[index];
          this.targetSampledRotations[index] = this.targetMappedRotations[index];
        }
        this.targetMappedStateSampled = true;
      }
    }

    public void FixTargetToSampledState(float weight)
    {
      if (!this.CheckIfInitiated() || (double) weight <= 0.0 || !this.targetMappedStateSampled)
        return;
      for (int index = 0; index < this.targetChildren.Length; ++index)
      {
        if ((UnityEngine.Object) this.targetChildren[index] == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) "PuppetMaster.UpdateTargetHierarchy() needs to be called when any of the child Transforms of the targetRoot are unparented or removed.", (UnityEngine.Object) this.transform);
          return;
        }
        if (index == 0)
        {
          this.targetChildren[index].position = Vector3.Lerp(this.targetChildren[index].position, this.targetSampledPositions[index], weight);
          this.targetChildren[index].rotation = Quaternion.Lerp(this.targetChildren[index].rotation, this.targetSampledRotations[index], weight);
        }
        else
        {
          this.targetChildren[index].position = Vector3.Lerp(this.targetChildren[index].position, this.targetSampledPositions[0] + this.targetSampledRotations[0] * this.targetSampledPositions[index], weight);
          this.targetChildren[index].rotation = Quaternion.Lerp(this.targetChildren[index].rotation, this.targetSampledRotations[0] * this.targetSampledRotations[index], weight);
        }
      }
      foreach (Muscle muscle in this.muscles)
        muscle.positionOffset = muscle.target.position - muscle.rigidbody.position;
    }

    public void StoreTargetMappedState()
    {
      if (!this.CheckIfInitiated() || !this.storeTargetMappedState)
        return;
      for (int index = 0; index < this.targetChildren.Length; ++index)
      {
        if (index == 0)
        {
          this.targetMappedPositions[index] = this.targetChildren[index].position;
          this.targetMappedRotations[index] = this.targetChildren[index].rotation;
        }
        else
        {
          this.targetMappedPositions[index] = Quaternion.Inverse(this.targetChildren[0].rotation) * (this.targetChildren[index].position - this.targetChildren[0].position);
          this.targetMappedRotations[index] = Quaternion.Inverse(this.targetChildren[0].rotation) * this.targetChildren[index].rotation;
        }
      }
      this.targetMappedStateStored = true;
      if (this.sampleTargetMappedState)
        this.SampleTargetMappedState();
      this.sampleTargetMappedState = false;
    }

    private void UpdateHierarchies()
    {
      this.targetChildren = new Transform[this.muscles.Length];
      for (int index = 0; index < this.muscles.Length; ++index)
        this.targetChildren[index] = this.muscles[index].target;
      this.targetMappedPositions = new Vector3[this.targetChildren.Length];
      this.targetMappedRotations = new Quaternion[this.targetChildren.Length];
      this.targetSampledPositions = new Vector3[this.targetChildren.Length];
      this.targetSampledRotations = new Quaternion[this.targetChildren.Length];
      this.targetMappedStateStored = false;
      this.targetMappedStateSampled = false;
      this.AssignParentAndChildIndexes();
      this.AssignKinshipDegrees();
      this.UpdateBroadcasterMuscleIndexes();
      this.internalCollisionsEnabled = !this.internalCollisions;
      this.SetInternalCollisions(this.internalCollisions);
      this.angularLimitsEnabled = !this.angularLimits;
      this.SetAngularLimits(this.angularLimits);
      this.hasProp = this.HasProp();
      if (this.OnHierarchyChanged == null)
        return;
      this.OnHierarchyChanged();
    }

    private bool HasProp()
    {
      foreach (Muscle muscle in this.muscles)
      {
        if (muscle.props.group == Muscle.Group.Prop)
          return true;
      }
      return false;
    }

    private void UpdateBroadcasterMuscleIndexes()
    {
      for (int index = 0; index < this.muscles.Length; ++index)
      {
        if ((UnityEngine.Object) this.muscles[index].broadcaster != (UnityEngine.Object) null)
          this.muscles[index].broadcaster.muscleIndex = index;
        if ((UnityEngine.Object) this.muscles[index].jointBreakBroadcaster != (UnityEngine.Object) null)
          this.muscles[index].jointBreakBroadcaster.muscleIndex = index;
      }
    }

    private void AssignParentAndChildIndexes()
    {
      for (int index1 = 0; index1 < this.muscles.Length; ++index1)
      {
        this.muscles[index1].parentIndexes = new int[0];
        if ((UnityEngine.Object) this.muscles[index1].joint.connectedBody != (UnityEngine.Object) null)
          this.AddToParentsRecursive(this.muscles[index1].joint.connectedBody.GetComponent<ConfigurableJoint>(), ref this.muscles[index1].parentIndexes);
        this.muscles[index1].childIndexes = new int[0];
        this.muscles[index1].childFlags = new bool[this.muscles.Length];
        for (int index2 = 0; index2 < this.muscles.Length; ++index2)
        {
          if (index1 != index2 && (UnityEngine.Object) this.muscles[index2].joint.connectedBody == (UnityEngine.Object) this.muscles[index1].rigidbody)
            this.AddToChildrenRecursive(this.muscles[index2].joint, ref this.muscles[index1].childIndexes, ref this.muscles[index1].childFlags);
        }
      }
    }

    private void AddToParentsRecursive(ConfigurableJoint joint, ref int[] indexes)
    {
      if ((UnityEngine.Object) joint == (UnityEngine.Object) null)
        return;
      int muscleIndexLowLevel = this.GetMuscleIndexLowLevel(joint);
      if (muscleIndexLowLevel == -1)
        return;
      Array.Resize<int>(ref indexes, indexes.Length + 1);
      indexes[indexes.Length - 1] = muscleIndexLowLevel;
      if ((UnityEngine.Object) joint.connectedBody == (UnityEngine.Object) null)
        return;
      this.AddToParentsRecursive(joint.connectedBody.GetComponent<ConfigurableJoint>(), ref indexes);
    }

    private void AddToChildrenRecursive(
      ConfigurableJoint joint,
      ref int[] indexes,
      ref bool[] childFlags)
    {
      if ((UnityEngine.Object) joint == (UnityEngine.Object) null)
        return;
      int muscleIndexLowLevel = this.GetMuscleIndexLowLevel(joint);
      if (muscleIndexLowLevel == -1)
        return;
      Array.Resize<int>(ref indexes, indexes.Length + 1);
      indexes[indexes.Length - 1] = muscleIndexLowLevel;
      childFlags[muscleIndexLowLevel] = true;
      for (int index = 0; index < this.muscles.Length; ++index)
      {
        if (index != muscleIndexLowLevel && (UnityEngine.Object) this.muscles[index].joint.connectedBody == (UnityEngine.Object) joint.GetComponent<Rigidbody>())
          this.AddToChildrenRecursive(this.muscles[index].joint, ref indexes, ref childFlags);
      }
    }

    private void AssignKinshipDegrees()
    {
      for (int index = 0; index < this.muscles.Length; ++index)
      {
        this.muscles[index].kinshipDegrees = new int[this.muscles.Length];
        this.AssignKinshipsDownRecursive(ref this.muscles[index].kinshipDegrees, 1, index);
        this.AssignKinshipsUpRecursive(ref this.muscles[index].kinshipDegrees, 1, index);
      }
    }

    private void AssignKinshipsDownRecursive(ref int[] kinshipDegrees, int degree, int index)
    {
      for (int index1 = 0; index1 < this.muscles.Length; ++index1)
      {
        if (index1 != index && (UnityEngine.Object) this.muscles[index1].joint.connectedBody == (UnityEngine.Object) this.muscles[index].rigidbody)
        {
          kinshipDegrees[index1] = degree;
          this.AssignKinshipsDownRecursive(ref kinshipDegrees, degree + 1, index1);
        }
      }
    }

    private void AssignKinshipsUpRecursive(ref int[] kinshipDegrees, int degree, int index)
    {
      for (int index1 = 0; index1 < this.muscles.Length; ++index1)
      {
        if (index1 != index && (UnityEngine.Object) this.muscles[index1].rigidbody == (UnityEngine.Object) this.muscles[index].joint.connectedBody)
        {
          kinshipDegrees[index1] = degree;
          this.AssignKinshipsUpRecursive(ref kinshipDegrees, degree + 1, index1);
          for (int index2 = 0; index2 < this.muscles.Length; ++index2)
          {
            if (index2 != index1 && index2 != index && (UnityEngine.Object) this.muscles[index2].joint.connectedBody == (UnityEngine.Object) this.muscles[index1].rigidbody)
            {
              kinshipDegrees[index2] = degree + 1;
              this.AssignKinshipsDownRecursive(ref kinshipDegrees, degree + 2, index2);
            }
          }
        }
      }
    }

    private int GetMuscleIndexLowLevel(ConfigurableJoint joint)
    {
      for (int muscleIndexLowLevel = 0; muscleIndexLowLevel < this.muscles.Length; ++muscleIndexLowLevel)
      {
        if ((UnityEngine.Object) this.muscles[muscleIndexLowLevel].joint == (UnityEngine.Object) joint)
          return muscleIndexLowLevel;
      }
      return -1;
    }

    public bool IsValid(bool log)
    {
      if (this.muscles == null)
      {
        if (log)
          Debug.LogWarning((object) "PuppetMaster Muscles is null.", (UnityEngine.Object) this.transform);
        return false;
      }
      if (this.muscles.Length == 0)
      {
        if (log)
          Debug.LogWarning((object) "PuppetMaster has no muscles.", (UnityEngine.Object) this.transform);
        return false;
      }
      for (int index = 0; index < this.muscles.Length; ++index)
      {
        if (this.muscles[index] == null)
        {
          if (log)
            Debug.LogWarning((object) "Muscle is null, PuppetMaster muscle setup is invalid.", (UnityEngine.Object) this.transform);
          return false;
        }
        if (!this.muscles[index].IsValid(log))
          return false;
      }
      if ((UnityEngine.Object) this.targetRoot == (UnityEngine.Object) null)
      {
        if (log)
          Debug.LogWarning((object) "'Target Root' of PuppetMaster is null.");
        return false;
      }
      if (this.targetRoot.position != this.transform.position)
      {
        if (log)
          Debug.LogWarning((object) "The position of the animated character (Target) must match with the position of the PuppetMaster when initiating PuppetMaster. If you are creating the Puppet in runtime, make sure you don't move the Target to another position immediatelly after instantiation. Move the Root Transform instead.");
        return false;
      }
      if ((UnityEngine.Object) this.targetRoot == (UnityEngine.Object) null)
      {
        if (log)
          Debug.LogWarning((object) "Invalid PuppetMaster setup. (targetRoot not found)", (UnityEngine.Object) this.transform);
        return false;
      }
      for (int index1 = 0; index1 < this.muscles.Length; ++index1)
      {
        for (int index2 = 0; index2 < this.muscles.Length; ++index2)
        {
          if (index1 != index2 && (this.muscles[index1] == this.muscles[index2] || (UnityEngine.Object) this.muscles[index1].joint == (UnityEngine.Object) this.muscles[index2].joint))
          {
            if (log)
              Debug.LogWarning((object) ("Joint " + this.muscles[index1].joint.name + " is used by multiple muscles (indexes " + (object) index1 + " and " + (object) index2 + "), PuppetMaster muscle setup is invalid."), (UnityEngine.Object) this.transform);
            return false;
          }
        }
      }
      if ((UnityEngine.Object) this.muscles[0].joint.connectedBody != (UnityEngine.Object) null && this.muscles.Length > 1)
      {
        for (int index = 1; index < this.muscles.Length; ++index)
        {
          if ((UnityEngine.Object) this.muscles[index].joint.GetComponent<Rigidbody>() == (UnityEngine.Object) this.muscles[0].joint.connectedBody)
          {
            if (log)
              Debug.LogWarning((object) "The first muscle needs to be the one that all the others are connected to (the hips).", (UnityEngine.Object) this.transform);
            return false;
          }
        }
      }
      for (int index = 0; index < this.muscles.Length; ++index)
      {
        if ((double) Vector3.SqrMagnitude(this.muscles[index].joint.transform.position - this.muscles[index].target.position) > 1.0 / 1000.0)
        {
          if (log)
            Debug.LogWarning((object) ("The position of each muscle needs to match with the position of it's target. Muscle '" + this.muscles[index].joint.name + "' position does not match with it's target. Right-click on the PuppetMaster component's header and select 'Fix Muscle Positions' from the context menu."), (UnityEngine.Object) this.muscles[index].joint.transform);
          return false;
        }
      }
      this.CheckMassVariation(100f, true);
      return true;
    }

    private bool CheckMassVariation(float threshold, bool log)
    {
      float num1 = float.PositiveInfinity;
      float num2 = 0.0f;
      for (int index = 0; index < this.muscles.Length; ++index)
      {
        float mass = this.muscles[index].joint.GetComponent<Rigidbody>().mass;
        if ((double) mass < (double) num1)
          num1 = mass;
        if ((double) mass > (double) num2)
          num2 = mass;
      }
      if ((double) num2 / (double) num1 <= (double) threshold)
        return true;
      if (log)
        Debug.LogWarning((object) ("Mass variation between the Rigidbodies in the ragdoll is more than " + threshold.ToString() + " times. This might cause instability and unwanted results with Rigidbodies connected by Joints. Min mass: " + (object) num1 + ", max mass: " + (object) num2), (UnityEngine.Object) this.transform);
      return false;
    }

    private bool CheckIfInitiated()
    {
      if (!this.initiated)
        Debug.LogError((object) "PuppetMaster has not been initiated yet.");
      return this.initiated;
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
    public struct StateSettings
    {
      [Tooltip("How much does it take to weigh out muscle weight to deadMuscleWeight?")]
      public float killDuration;
      [Tooltip("The muscle weight mlp while the puppet is Dead.")]
      public float deadMuscleWeight;
      [Tooltip("The muscle damper add while the puppet is Dead.")]
      public float deadMuscleDamper;
      [Tooltip("The max square velocity of the ragdoll bones for freezing the puppet.")]
      public float maxFreezeSqrVelocity;
      [Tooltip("If true, PuppetMaster, all it's behaviours and the ragdoll will be destroyed when the puppet is frozen.")]
      public bool freezePermanently;
      [Tooltip("If true, will enable angular limits when killing the puppet.")]
      public bool enableAngularLimitsOnKill;
      [Tooltip("If true, will enable internal collisions when killing the puppet.")]
      public bool enableInternalCollisionsOnKill;

      public StateSettings(
        float killDuration,
        float deadMuscleWeight = 0.01f,
        float deadMuscleDamper = 2f,
        float maxFreezeSqrVelocity = 0.02f,
        bool freezePermanently = false,
        bool enableAngularLimitsOnKill = true,
        bool enableInternalCollisionsOnKill = true)
      {
        this.killDuration = killDuration;
        this.deadMuscleWeight = deadMuscleWeight;
        this.deadMuscleDamper = deadMuscleDamper;
        this.maxFreezeSqrVelocity = maxFreezeSqrVelocity;
        this.freezePermanently = freezePermanently;
        this.enableAngularLimitsOnKill = enableAngularLimitsOnKill;
        this.enableInternalCollisionsOnKill = enableInternalCollisionsOnKill;
      }

      public static PuppetMaster.StateSettings Default => new PuppetMaster.StateSettings(1f);
    }
  }
}
