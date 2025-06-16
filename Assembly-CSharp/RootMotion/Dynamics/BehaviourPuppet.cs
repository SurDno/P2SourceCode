using System;

namespace RootMotion.Dynamics
{
  [HelpURL("http://root-motion.com/puppetmasterdox/html/page10.html")]
  [AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Behaviours/BehaviourPuppet")]
  public class BehaviourPuppet : BehaviourBase
  {
    [LargeHeader("Collision And Recovery")]
    public MasterProps masterProps = new MasterProps();
    [Tooltip("Will ground the target to those layers when getting up.")]
    public LayerMask groundLayers;
    [Tooltip("Will unpin the muscles that collide with those layers.")]
    public LayerMask collisionLayers;
    [Tooltip("The collision impulse sqrMagnitude threshold under which collisions will be ignored.")]
    public float collisionThreshold;
    public Weight collisionResistance = new Weight(3f, "Smaller value means more unpinning from collisions so the characters get knocked out more easily. If using a curve, the value will be evaluated by each muscle's target velocity magnitude. This can be used to make collision resistance higher while the character moves or animates faster.");
    [Tooltip("Multiplies collision resistance for the specified layers.")]
    public CollisionResistanceMultiplier[] collisionResistanceMultipliers;
    [Tooltip("An optimisation. Will only process up to this number of collisions per physics step.")]
    [Range(1f, 30f)]
    public int maxCollisions = 30;
    [Tooltip("How fast will the muscles of this group regain their pin weight?")]
    [Range(0.001f, 10f)]
    public float regainPinSpeed = 1f;
    [Tooltip("'Boosting' is a term used for making muscles temporarily immune to collisions and/or deal more damage to the muscles of other characters. That is done by increasing Muscle.State.immunity and Muscle.State.impulseMlp. For example when you set muscle.state.immunity to 1, boostFalloff will determine how fast this value will fall back to normal (0). Use BehaviourPuppet.BoostImmunity() and BehaviourPuppet.BoostImpulseMlp() for boosting from your own scripts. It is helpful for making the puppet stronger and deliever more punch while playing a melee hitting/kicking animation.")]
    public float boostFalloff = 1f;
    [LargeHeader("Muscle Group Properties")]
    [Tooltip("The default muscle properties. If there are no 'Group Overrides', this will be used for all muscles.")]
    public MuscleProps defaults;
    [Tooltip("Overriding default muscle properties for some muscle groups (for example making the feet stiffer or the hands looser).")]
    public MusclePropsGroup[] groupOverrides;
    [LargeHeader("Losing Balance")]
    [Tooltip("If the distance from the muscle to it's target is larger than this value, the character will be knocked out.")]
    [Range(0.001f, 10f)]
    public float knockOutDistance = 1f;
    [Tooltip("Smaller value makes the muscles weaker when the puppet is knocked out.")]
    [Range(0.0f, 1f)]
    public float unpinnedMuscleWeightMlp = 0.3f;
    [Tooltip("Most character controllers apply supernatural accelerations to characters when changing running direction or jumping. It will require major pinning forces to be applied on the ragdoll to keep up with that acceleration. When a puppet collides with something at that point and is unpinned, those forces might shoot the puppet off to space. This variable limits the velocity of the ragdoll's Rigidbodies when the puppet is unpinned.")]
    public float maxRigidbodyVelocity = 10f;
    [Tooltip("If a muscle has drifted farther than 'Knock Out Distance', will only unpin the puppet if it's pin weight is less than this value. Lowering this value will make puppets less likely to lose balance on minor collisions.")]
    [Range(0.0f, 1f)]
    public float pinWeightThreshold = 1f;
    [Tooltip("If false, will not unbalance the puppet by muscles that have their pin weight set to 0 in PuppetMaster muscle settings.")]
    public bool unpinnedMuscleKnockout = true;
    [Tooltip("If true, all muscles of the 'Prop' group will be detached from the puppet when it loses balance.")]
    public bool dropProps;
    [LargeHeader("Getting Up")]
    [Tooltip("If true, GetUp state will be triggerred automatically after 'Get Up Delay' and when the velocity of the hip muscle is less than 'Max Get Up Velocity'.")]
    public bool canGetUp = true;
    [Tooltip("Minimum delay for getting up after loosing balance. After that time has passed, will wait for the velocity of the hip muscle to come down below 'Max Get Up Velocity' and then switch to the GetUp state.")]
    public float getUpDelay = 5f;
    [Tooltip("The duration of blending the animation target from the ragdoll pose to the getting up animation once the GetUp state has been triggered.")]
    public float blendToAnimationTime = 0.2f;
    [Tooltip("Will not get up before the velocity of the hip muscle has come down to this value.")]
    public float maxGetUpVelocity = 0.3f;
    [Tooltip("The duration of the 'GetUp' state after which it switches to the 'Puppetä state.")]
    public float minGetUpDuration = 1f;
    [Tooltip("Collision resistance multiplier while in the GetUp state. Increasing this will prevent the character from loosing balance again immediatelly after going from Unpinned to GetUp state.")]
    public float getUpCollisionResistanceMlp = 2f;
    [Tooltip("Regain pin weight speed multiplier while in the GetUp state. Increasing this will prevent the character from loosing balance again immediatelly after going from Unpinned to GetUp state.")]
    public float getUpRegainPinSpeedMlp = 2f;
    [Tooltip("Knock out distance multiplier while in the GetUp state. Increasing this will prevent the character from loosing balance again immediatelly after going from Unpinned to GetUp state.")]
    public float getUpKnockOutDistanceMlp = 10f;
    [Tooltip("Offset of the target character (in character rotation space) from the hip bone when initiating getting up animation from a prone pose. Tweak this value if your character slides a bit when starting to get up.")]
    public Vector3 getUpOffsetProne;
    [Tooltip("Offset of the target character (in character rotation space) from the hip bone when initiating getting up animation from a supine pose. Tweak this value if your character slides a bit when starting to get up.")]
    public Vector3 getUpOffsetSupine;
    [LargeHeader("Events")]
    [Tooltip("Called when the character starts getting up from a prone pose (facing down).")]
    public PuppetEvent onGetUpProne;
    [Tooltip("Called when the character starts getting up from a supine pose (facing up).")]
    public PuppetEvent onGetUpSupine;
    [Tooltip("Called when the character is knocked out (loses balance). Doesn't matter from which state.")]
    public PuppetEvent onLoseBalance;
    [Tooltip("Called when the character is knocked out (loses balance) only from the normal Puppet state.")]
    public PuppetEvent onLoseBalanceFromPuppet;
    [Tooltip("Called when the character is knocked out (loses balance) only from the GetUp state.")]
    public PuppetEvent onLoseBalanceFromGetUp;
    [Tooltip("Called when the character has fully recovered and switched to the Puppet state.")]
    public PuppetEvent onRegainBalance;
    public CollisionImpulseDelegate OnCollisionImpulse;
    [HideInInspector]
    public bool canMoveTarget = true;
    private float unpinnedTimer;
    private float getUpTimer;
    private Vector3 hipsForward;
    private Vector3 hipsUp;
    private float getupAnimationBlendWeight;
    private float getupAnimationBlendWeightV;
    private bool getUpTargetFixed;
    private NormalMode lastNormalMode;
    private int collisions;
    private bool eventsEnabled;
    private float lastKnockOutDistance;
    private float knockOutDistanceSqr;
    private bool getupDisabled;
    private bool hasCollidedSinceGetUp;
    private bool hasBoosted;
    private MuscleCollisionBroadcaster broadcaster;
    private Vector3 getUpPosition;
    private bool dropPropFlag;

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page10.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/class_root_motion_1_1_dynamics_1_1_behaviour_puppet.html");
    }

    public State state { get; private set; }

    public override void OnReactivate()
    {
      state = puppetMaster.state == PuppetMaster.State.Alive ? State.Puppet : State.Unpinned;
      getUpTimer = 0.0f;
      unpinnedTimer = 0.0f;
      getupAnimationBlendWeight = 0.0f;
      getupAnimationBlendWeightV = 0.0f;
      getUpTargetFixed = false;
      getupDisabled = puppetMaster.state != 0;
      state = puppetMaster.state == PuppetMaster.State.Alive ? State.Puppet : State.Unpinned;
      foreach (Muscle muscle in puppetMaster.muscles)
        SetColliders(muscle, state == State.Unpinned);
      this.enabled = true;
    }

    public void Reset(Vector3 position, Quaternion rotation)
    {
      Debug.LogWarning((object) "BehaviourPuppet.Reset has been deprecated, please use PuppetMaster.Teleport instead.");
    }

    public override void OnTeleport(
      Quaternion deltaRotation,
      Vector3 deltaPosition,
      Vector3 pivot,
      bool moveToTarget)
    {
      getUpPosition = pivot + deltaRotation * (getUpPosition - pivot) + deltaPosition;
      if (!moveToTarget)
        return;
      getupAnimationBlendWeight = 0.0f;
      getupAnimationBlendWeightV = 0.0f;
    }

    protected override void OnInitiate()
    {
      foreach (CollisionResistanceMultiplier resistanceMultiplier in collisionResistanceMultipliers)
      {
        if ((int) resistanceMultiplier.layers == 0)
          Debug.LogWarning((object) "BehaviourPuppet has a Collision Resistance Multiplier that's layers is set to Nothing. Please add some layers.", (UnityEngine.Object) this.transform);
      }
      foreach (Muscle muscle in puppetMaster.muscles)
      {
        if (muscle.joint.gameObject.layer == puppetMaster.targetRoot.gameObject.layer)
          Debug.LogWarning((object) "One of the ragdoll bones is on the same layer as the animated character. This might make the ragdoll collide with the character controller.");
        if (!Physics.GetIgnoreLayerCollision(muscle.joint.gameObject.layer, puppetMaster.targetRoot.gameObject.layer))
          Debug.LogWarning((object) ("The ragdoll layer (" + (object) muscle.joint.gameObject.layer + ") and the character controller layer (" + (object) puppetMaster.targetRoot.gameObject.layer + ") are not set to ignore each other in Edit/Project Settings/Physics/Layer Collision Matrix. This might cause the ragdoll bones to collide with the character controller."));
      }
      hipsForward = Quaternion.Inverse(puppetMaster.muscles[0].transform.rotation) * puppetMaster.targetRoot.forward;
      hipsUp = Quaternion.Inverse(puppetMaster.muscles[0].transform.rotation) * puppetMaster.targetRoot.up;
      state = State.Unpinned;
      eventsEnabled = true;
    }

    protected override void OnActivate()
    {
      bool flag = true;
      foreach (Muscle muscle in puppetMaster.muscles)
      {
        if (muscle.state.pinWeightMlp > 0.5)
        {
          flag = false;
          break;
        }
      }
      bool eventsEnabled = this.eventsEnabled;
      this.eventsEnabled = false;
      if (flag)
        SetState(State.Unpinned);
      else
        SetState(State.Puppet);
      this.eventsEnabled = eventsEnabled;
    }

    public override void KillStart()
    {
      getupDisabled = true;
      foreach (Muscle muscle in puppetMaster.muscles)
      {
        muscle.state.pinWeightMlp = 0.0f;
        if (hasBoosted)
          muscle.state.immunity = 0.0f;
        SetColliders(muscle, true);
      }
    }

    public override void KillEnd() => SetState(State.Unpinned);

    public override void Resurrect()
    {
      getupDisabled = false;
      if (state != State.Unpinned)
        return;
      getUpTimer = float.PositiveInfinity;
      unpinnedTimer = float.PositiveInfinity;
      getupAnimationBlendWeight = 0.0f;
      getupAnimationBlendWeightV = 0.0f;
      foreach (Muscle muscle in puppetMaster.muscles)
        muscle.state.pinWeightMlp = 0.0f;
    }

    protected override void OnDeactivate() => state = State.Unpinned;

    protected override void OnFixedUpdate()
    {
      collisions = 0;
      if (dropPropFlag)
      {
        RemoveMusclesOfGroup(Muscle.Group.Prop);
        dropPropFlag = false;
      }
      if (!puppetMaster.isActive)
        SetState(State.Puppet);
      else if (!puppetMaster.isAlive)
      {
        foreach (Muscle muscle in puppetMaster.muscles)
        {
          muscle.state.pinWeightMlp = 0.0f;
          muscle.state.mappingWeightMlp = Mathf.MoveTowards(muscle.state.mappingWeightMlp, 1f, Time.deltaTime * 5f);
        }
      }
      else
      {
        if (hasBoosted)
        {
          foreach (Muscle muscle in puppetMaster.muscles)
          {
            muscle.state.immunity = Mathf.MoveTowards(muscle.state.immunity, 0.0f, Time.deltaTime * boostFalloff);
            muscle.state.impulseMlp = Mathf.Lerp(muscle.state.impulseMlp, 1f, Time.deltaTime * boostFalloff);
          }
        }
        if (state == State.Unpinned)
        {
          unpinnedTimer += Time.deltaTime;
          if (unpinnedTimer >= (double) getUpDelay && canGetUp && !getupDisabled && (double) puppetMaster.muscles[0].rigidbody.velocity.magnitude < maxGetUpVelocity)
          {
            SetState(State.GetUp);
            return;
          }
          foreach (Muscle muscle in puppetMaster.muscles)
          {
            muscle.state.pinWeightMlp = 0.0f;
            muscle.state.mappingWeightMlp = Mathf.MoveTowards(muscle.state.mappingWeightMlp, 1f, Time.deltaTime * masterProps.mappingBlendSpeed);
          }
        }
        if (state != State.Unpinned)
        {
          if (knockOutDistance != (double) lastKnockOutDistance)
          {
            knockOutDistanceSqr = Mathf.Sqrt(knockOutDistance);
            lastKnockOutDistance = knockOutDistance;
          }
          foreach (Muscle muscle in puppetMaster.muscles)
          {
            MuscleProps props = GetProps(muscle.props.group);
            float b = 1f;
            if (state == State.GetUp)
              b = Mathf.Lerp(getUpKnockOutDistanceMlp, b, muscle.state.pinWeightMlp);
            float num1 = unpinnedMuscleKnockout ? muscle.positionOffset.sqrMagnitude : muscle.positionOffset.sqrMagnitude * muscle.props.pinWeight;
            if (hasCollidedSinceGetUp && !puppetMaster.isBlending && num1 > 0.0 && muscle.state.pinWeightMlp < (double) pinWeightThreshold && num1 > props.knockOutDistance * (double) knockOutDistanceSqr * b)
            {
              if (state == State.GetUp && !getUpTargetFixed)
                return;
              SetState(State.Unpinned);
              return;
            }
            muscle.state.muscleWeightMlp = Mathf.Lerp(unpinnedMuscleWeightMlp, 1f, muscle.state.pinWeightMlp);
            if (state == State.GetUp)
              muscle.state.muscleDamperAdd = 0.0f;
            if (!puppetMaster.isKilling)
            {
              float num2 = 1f;
              if (state == State.GetUp)
                num2 = Mathf.Lerp(getUpRegainPinSpeedMlp, 1f, muscle.state.pinWeightMlp);
              muscle.state.pinWeightMlp += Time.deltaTime * props.regainPinSpeed * regainPinSpeed * num2;
            }
          }
          float num = 1f;
          foreach (Muscle muscle in puppetMaster.muscles)
          {
            if ((muscle.props.group == Muscle.Group.Leg || muscle.props.group == Muscle.Group.Foot) && muscle.state.pinWeightMlp < (double) num)
              num = muscle.state.pinWeightMlp;
          }
          foreach (Muscle muscle in puppetMaster.muscles)
            muscle.state.pinWeightMlp = Mathf.Clamp(muscle.state.pinWeightMlp, 0.0f, num * 5f);
        }
        if (state != State.GetUp)
          return;
        getUpTimer += Time.deltaTime;
        if (getUpTimer > (double) minGetUpDuration)
        {
          getUpTimer = 0.0f;
          SetState(State.Puppet);
        }
      }
    }

    protected override void OnLateUpdate()
    {
      forceActive = state != 0;
      if (!puppetMaster.isAlive)
        return;
      if (masterProps.normalMode != lastNormalMode)
      {
        if (lastNormalMode == NormalMode.Unmapped)
        {
          foreach (Muscle muscle in puppetMaster.muscles)
            muscle.state.mappingWeightMlp = 1f;
        }
        if (lastNormalMode == NormalMode.Kinematic && puppetMaster.mode == PuppetMaster.Mode.Kinematic)
          puppetMaster.mode = PuppetMaster.Mode.Active;
        lastNormalMode = masterProps.normalMode;
      }
      switch (masterProps.normalMode)
      {
        case NormalMode.Unmapped:
          if (!puppetMaster.isActive)
            break;
          bool to = false;
          for (int muscleIndex = 0; muscleIndex < puppetMaster.muscles.Length; ++muscleIndex)
            BlendMuscleMapping(muscleIndex, ref to);
          break;
        case NormalMode.Kinematic:
          if (!SetKinematic())
            break;
          puppetMaster.mode = PuppetMaster.Mode.Kinematic;
          break;
      }
    }

    private bool SetKinematic()
    {
      if (!puppetMaster.isActive || state != 0 || puppetMaster.isBlending || getupAnimationBlendWeight > 0.0 || !puppetMaster.isAlive)
        return false;
      foreach (Muscle muscle in puppetMaster.muscles)
      {
        if (muscle.state.pinWeightMlp < 1.0)
          return false;
      }
      return true;
    }

    protected override void OnReadBehaviour()
    {
      if (!this.enabled)
        return;
      if (!puppetMaster.isFrozen && state == State.Unpinned && puppetMaster.isActive)
      {
        MoveTarget(puppetMaster.muscles[0].rigidbody.position);
        GroundTarget(groundLayers);
        getUpPosition = puppetMaster.targetRoot.position;
      }
      if (getupAnimationBlendWeight > 0.0)
      {
        getUpPosition += Vector3.Project(puppetMaster.targetRoot.position - getUpPosition, puppetMaster.targetRoot.up);
        MoveTarget(getUpPosition);
        getupAnimationBlendWeight = Mathf.SmoothDamp(getupAnimationBlendWeight, 0.0f, ref getupAnimationBlendWeightV, blendToAnimationTime);
        if (getupAnimationBlendWeight < 0.0099999997764825821)
          getupAnimationBlendWeight = 0.0f;
        puppetMaster.FixTargetToSampledState(getupAnimationBlendWeight);
      }
      getUpTargetFixed = true;
    }

    private void BlendMuscleMapping(int muscleIndex, ref bool to)
    {
      if (puppetMaster.muscles[muscleIndex].state.pinWeightMlp < 1.0)
        to = true;
      MuscleProps props = GetProps(puppetMaster.muscles[muscleIndex].props.group);
      float target = to ? (state == State.Puppet ? props.maxMappingWeight : 1f) : props.minMappingWeight;
      puppetMaster.muscles[muscleIndex].state.mappingWeightMlp = Mathf.MoveTowards(puppetMaster.muscles[muscleIndex].state.mappingWeightMlp, target, Time.deltaTime * masterProps.mappingBlendSpeed);
    }

    public override void OnMuscleAdded(Muscle m)
    {
      base.OnMuscleAdded(m);
      SetColliders(m, state == State.Unpinned);
    }

    public override void OnMuscleRemoved(Muscle m)
    {
      base.OnMuscleRemoved(m);
      SetColliders(m, true);
    }

    protected void MoveTarget(Vector3 position)
    {
      if (!canMoveTarget)
        return;
      puppetMaster.targetRoot.position = position;
    }

    protected void RotateTarget(Quaternion rotation)
    {
      if (!canMoveTarget)
        return;
      puppetMaster.targetRoot.rotation = rotation;
    }

    protected override void GroundTarget(LayerMask layers)
    {
      if (!canMoveTarget)
        return;
      base.GroundTarget(layers);
    }

    private void OnDrawGizmosSelected()
    {
      for (int index1 = 0; index1 < groupOverrides.Length; ++index1)
      {
        groupOverrides[index1].name = string.Empty;
        if (groupOverrides[index1].groups.Length != 0)
        {
          for (int index2 = 0; index2 < groupOverrides[index1].groups.Length; ++index2)
          {
            if (index2 > 0)
            {
              // ISSUE: explicit reference operation
              ^ref groupOverrides[index1].name += ", ";
            }
            // ISSUE: explicit reference operation
            ^ref groupOverrides[index1].name += groupOverrides[index1].groups[index2].ToString();
          }
        }
      }
    }

    public void Boost(float immunity, float impulseMlp)
    {
      hasBoosted = true;
      for (int muscleIndex = 0; muscleIndex < puppetMaster.muscles.Length; ++muscleIndex)
        Boost(muscleIndex, immunity, impulseMlp);
    }

    public void Boost(int muscleIndex, float immunity, float impulseMlp)
    {
      hasBoosted = true;
      BoostImmunity(muscleIndex, immunity);
      BoostImpulseMlp(muscleIndex, impulseMlp);
    }

    public void Boost(
      int muscleIndex,
      float immunity,
      float impulseMlp,
      float boostParents,
      float boostChildren)
    {
      hasBoosted = true;
      if (boostParents <= 0.0 && boostChildren <= 0.0)
      {
        Boost(muscleIndex, immunity, impulseMlp);
      }
      else
      {
        for (int index = 0; index < puppetMaster.muscles.Length; ++index)
        {
          float falloff = GetFalloff(index, muscleIndex, boostParents, boostChildren);
          Boost(index, immunity * falloff, impulseMlp * falloff);
        }
      }
    }

    public void BoostImmunity(float immunity)
    {
      hasBoosted = true;
      if (immunity < 0.0)
        return;
      for (int muscleIndex = 0; muscleIndex < puppetMaster.muscles.Length; ++muscleIndex)
        BoostImmunity(muscleIndex, immunity);
    }

    public void BoostImmunity(int muscleIndex, float immunity)
    {
      hasBoosted = true;
      puppetMaster.muscles[muscleIndex].state.immunity = Mathf.Clamp(immunity, puppetMaster.muscles[muscleIndex].state.immunity, 1f);
    }

    public void BoostImmunity(
      int muscleIndex,
      float immunity,
      float boostParents,
      float boostChildren)
    {
      hasBoosted = true;
      for (int index = 0; index < puppetMaster.muscles.Length; ++index)
      {
        float falloff = GetFalloff(index, muscleIndex, boostParents, boostChildren);
        BoostImmunity(index, immunity * falloff);
      }
    }

    public void BoostImpulseMlp(float impulseMlp)
    {
      hasBoosted = true;
      for (int muscleIndex = 0; muscleIndex < puppetMaster.muscles.Length; ++muscleIndex)
        BoostImpulseMlp(muscleIndex, impulseMlp);
    }

    public void BoostImpulseMlp(int muscleIndex, float impulseMlp)
    {
      hasBoosted = true;
      puppetMaster.muscles[muscleIndex].state.impulseMlp = Mathf.Max(impulseMlp, puppetMaster.muscles[muscleIndex].state.impulseMlp);
    }

    public void BoostImpulseMlp(
      int muscleIndex,
      float impulseMlp,
      float boostParents,
      float boostChildren)
    {
      hasBoosted = true;
      for (int index = 0; index < puppetMaster.muscles.Length; ++index)
      {
        float falloff = GetFalloff(index, muscleIndex, boostParents, boostChildren);
        BoostImpulseMlp(index, impulseMlp * falloff);
      }
    }

    public void Unpin()
    {
      Debug.Log((object) "BehaviourPuppet.Unpin() has been deprecated. Use SetState(BehaviourPuppet.State) instead.");
      SetState(State.Unpinned);
    }

    protected override void OnMuscleHitBehaviour(MuscleHit hit)
    {
      if (masterProps.normalMode == NormalMode.Kinematic)
        puppetMaster.mode = PuppetMaster.Mode.Active;
      UnPin(hit.muscleIndex, hit.unPin);
      puppetMaster.muscles[hit.muscleIndex].rigidbody.isKinematic = false;
      puppetMaster.muscles[hit.muscleIndex].rigidbody.AddForceAtPosition(hit.force, hit.position);
    }

    protected override void OnMuscleCollisionBehaviour(MuscleCollision m)
    {
      if (!this.enabled || state == State.Unpinned || collisions > maxCollisions || !LayerMaskExtensions.Contains(collisionLayers, m.collision.gameObject.layer) || masterProps.normalMode == NormalMode.Kinematic && !puppetMaster.isActive && !masterProps.activateOnStaticCollisions && m.collision.gameObject.isStatic)
        return;
      float collisionThreshold = this.collisionThreshold;
      float impulse = GetImpulse(m, ref collisionThreshold);
      if (OnCollisionImpulse != null)
        OnCollisionImpulse(m, impulse);
      float num1 = (UnityEngine.Object) Singleton<PuppetMasterSettings>.instance != (UnityEngine.Object) null ? (float) (1.0 + Singleton<PuppetMasterSettings>.instance.currentlyActivePuppets * (double) Singleton<PuppetMasterSettings>.instance.activePuppetCollisionThresholdMlp) : 1f;
      float num2 = collisionThreshold * num1;
      if (impulse <= (double) num2)
        return;
      ++collisions;
      if ((UnityEngine.Object) m.collision.collider.attachedRigidbody != (UnityEngine.Object) null)
      {
        broadcaster = m.collision.collider.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();
        if ((UnityEngine.Object) broadcaster != (UnityEngine.Object) null && broadcaster.muscleIndex < broadcaster.puppetMaster.muscles.Length)
          impulse *= broadcaster.puppetMaster.muscles[broadcaster.muscleIndex].state.impulseMlp;
      }
      if (Activate(m.collision, impulse))
        puppetMaster.mode = PuppetMaster.Mode.Active;
      UnPin(m.muscleIndex, impulse);
    }

    private float GetImpulse(MuscleCollision m, ref float layerThreshold)
    {
      float impulse = m.collision.impulse.sqrMagnitude / puppetMaster.muscles[m.muscleIndex].rigidbody.mass * 0.3f;
      foreach (CollisionResistanceMultiplier resistanceMultiplier in collisionResistanceMultipliers)
      {
        if (LayerMaskExtensions.Contains(resistanceMultiplier.layers, m.collision.collider.gameObject.layer))
        {
          if (resistanceMultiplier.multiplier <= 0.0)
            impulse = float.PositiveInfinity;
          else
            impulse /= resistanceMultiplier.multiplier;
          layerThreshold = resistanceMultiplier.collisionThreshold;
          break;
        }
      }
      return impulse;
    }

    private void UnPin(int muscleIndex, float unpin)
    {
      if (muscleIndex >= puppetMaster.muscles.Length)
        return;
      MuscleProps props = GetProps(puppetMaster.muscles[muscleIndex].props.group);
      for (int index = 0; index < puppetMaster.muscles.Length; ++index)
        UnPinMuscle(index, unpin * GetFalloff(index, muscleIndex, props.unpinParents, props.unpinChildren, props.unpinGroup));
      hasCollidedSinceGetUp = true;
    }

    private void UnPinMuscle(int muscleIndex, float unpin)
    {
      if (unpin <= 0.0 || puppetMaster.muscles[muscleIndex].state.immunity >= 1.0)
        return;
      MuscleProps props = GetProps(puppetMaster.muscles[muscleIndex].props.group);
      float num1 = 1f;
      if (state == State.GetUp)
        num1 = Mathf.Lerp(getUpCollisionResistanceMlp, 1f, puppetMaster.muscles[muscleIndex].state.pinWeightMlp);
      float num2 = collisionResistance.mode == Weight.Mode.Float ? collisionResistance.floatValue : collisionResistance.GetValue(puppetMaster.muscles[muscleIndex].targetVelocity.magnitude);
      float num3 = unpin / (props.collisionResistance * num2 * num1) * (1f - puppetMaster.muscles[muscleIndex].state.immunity);
      puppetMaster.muscles[muscleIndex].state.pinWeightMlp -= num3;
    }

    private bool Activate(Collision collision, float impulse)
    {
      if (masterProps.normalMode != NormalMode.Kinematic || puppetMaster.mode != PuppetMaster.Mode.Kinematic || impulse < (double) masterProps.activateOnImpulse)
        return false;
      return !collision.gameObject.isStatic || masterProps.activateOnStaticCollisions;
    }

    public bool IsProne()
    {
      return (double) Vector3.Dot(puppetMaster.muscles[0].transform.rotation * hipsForward, puppetMaster.targetRoot.up) < 0.0;
    }

    private float GetFalloff(int i, int muscleIndex, float falloffParents, float falloffChildren)
    {
      if (i == muscleIndex)
        return 1f;
      bool childFlag = puppetMaster.muscles[muscleIndex].childFlags[i];
      int kinshipDegree = puppetMaster.muscles[muscleIndex].kinshipDegrees[i];
      return Mathf.Pow(childFlag ? falloffChildren : falloffParents, (float) kinshipDegree);
    }

    private float GetFalloff(
      int i,
      int muscleIndex,
      float falloffParents,
      float falloffChildren,
      float falloffGroup)
    {
      float a = GetFalloff(i, muscleIndex, falloffParents, falloffChildren);
      if (falloffGroup > 0.0 && i != muscleIndex && InGroup(puppetMaster.muscles[i].props.group, puppetMaster.muscles[muscleIndex].props.group))
        a = Mathf.Max(a, falloffGroup);
      return a;
    }

    private bool InGroup(Muscle.Group group1, Muscle.Group group2)
    {
      if (group1 == group2)
        return true;
      foreach (MusclePropsGroup groupOverride in groupOverrides)
      {
        foreach (Muscle.Group group3 in groupOverride.groups)
        {
          if (group3 == group1)
          {
            foreach (Muscle.Group group4 in groupOverride.groups)
            {
              if (group4 == group2)
                return true;
            }
          }
        }
      }
      return false;
    }

    private MuscleProps GetProps(Muscle.Group group)
    {
      foreach (MusclePropsGroup groupOverride in groupOverrides)
      {
        foreach (Muscle.Group group1 in groupOverride.groups)
        {
          if (group1 == group)
            return groupOverride.props;
        }
      }
      return defaults;
    }

    public void SetState(State newState)
    {
      if (state == newState)
        return;
      switch (newState)
      {
        case State.Puppet:
          puppetMaster.SampleTargetMappedState();
          unpinnedTimer = 0.0f;
          getUpTimer = 0.0f;
          if (state == State.Unpinned)
          {
            foreach (Muscle muscle in puppetMaster.muscles)
            {
              muscle.state.pinWeightMlp = 1f;
              muscle.state.muscleWeightMlp = 1f;
              muscle.state.muscleDamperAdd = 0.0f;
              SetColliders(muscle, false);
            }
          }
          state = State.Puppet;
          if (eventsEnabled)
          {
            onRegainBalance.Trigger(puppetMaster);
            if (onRegainBalance.switchBehaviour)
              return;
          }
          break;
        case State.Unpinned:
          unpinnedTimer = 0.0f;
          getUpTimer = 0.0f;
          getupAnimationBlendWeight = 0.0f;
          getupAnimationBlendWeightV = 0.0f;
          foreach (Muscle muscle in puppetMaster.muscles)
          {
            if (hasBoosted)
              muscle.state.immunity = 0.0f;
            if (maxRigidbodyVelocity != double.PositiveInfinity)
              muscle.rigidbody.velocity = Vector3.ClampMagnitude(muscle.rigidbody.velocity, maxRigidbodyVelocity);
            SetColliders(muscle, true);
          }
          if (dropProps)
            dropPropFlag = true;
          foreach (Muscle muscle in puppetMaster.muscles)
            muscle.state.muscleWeightMlp = puppetMaster.isAlive ? unpinnedMuscleWeightMlp : puppetMaster.stateSettings.deadMuscleWeight;
          onLoseBalance.Trigger(puppetMaster, puppetMaster.isAlive);
          if (onLoseBalance.switchBehaviour)
          {
            state = State.Unpinned;
            return;
          }
          if (state == State.Puppet)
          {
            onLoseBalanceFromPuppet.Trigger(puppetMaster, puppetMaster.isAlive);
            if (onLoseBalanceFromPuppet.switchBehaviour)
            {
              state = State.Unpinned;
              return;
            }
          }
          else
          {
            onLoseBalanceFromGetUp.Trigger(puppetMaster, puppetMaster.isAlive);
            if (onLoseBalanceFromGetUp.switchBehaviour)
            {
              state = State.Unpinned;
              return;
            }
          }
          foreach (Muscle muscle in puppetMaster.muscles)
            muscle.state.pinWeightMlp = 0.0f;
          break;
        case State.GetUp:
          unpinnedTimer = 0.0f;
          getUpTimer = 0.0f;
          hasCollidedSinceGetUp = false;
          bool flag = IsProne();
          state = State.GetUp;
          if (flag)
          {
            onGetUpProne.Trigger(puppetMaster);
            if (onGetUpProne.switchBehaviour)
              return;
          }
          else
          {
            onGetUpSupine.Trigger(puppetMaster);
            if (onGetUpSupine.switchBehaviour)
              return;
          }
          foreach (Muscle muscle in puppetMaster.muscles)
          {
            muscle.state.muscleWeightMlp = 0.0f;
            muscle.state.pinWeightMlp = 0.0f;
            muscle.state.muscleDamperAdd = 0.0f;
            SetColliders(muscle, false);
          }
          Vector3 tangent = puppetMaster.muscles[0].rigidbody.rotation * hipsUp;
          Vector3 up = puppetMaster.targetRoot.up;
          Vector3.OrthoNormalize(ref up, ref tangent);
          RotateTarget(Quaternion.LookRotation(flag ? tangent : -tangent, puppetMaster.targetRoot.up));
          puppetMaster.SampleTargetMappedState();
          MoveTarget(puppetMaster.muscles[0].rigidbody.position + puppetMaster.targetRoot.rotation * (flag ? getUpOffsetProne : getUpOffsetSupine));
          GroundTarget(groundLayers);
          getUpPosition = puppetMaster.targetRoot.position;
          getupAnimationBlendWeight = 1f;
          getUpTargetFixed = false;
          break;
      }
      state = newState;
    }

    public void SetColliders(bool unpinned)
    {
      foreach (Muscle muscle in puppetMaster.muscles)
        SetColliders(muscle, unpinned);
    }

    private void SetColliders(Muscle m, bool unpinned)
    {
      MuscleProps props = GetProps(m.props.group);
      if (unpinned)
      {
        foreach (Collider collider in m.colliders)
        {
          collider.material = (UnityEngine.Object) props.unpinnedMaterial != (UnityEngine.Object) null ? props.unpinnedMaterial : defaults.unpinnedMaterial;
          if (props.disableColliders)
            collider.enabled = true;
        }
      }
      else
      {
        foreach (Collider collider in m.colliders)
        {
          collider.material = (UnityEngine.Object) props.puppetMaterial != (UnityEngine.Object) null ? props.puppetMaterial : defaults.puppetMaterial;
          if (props.disableColliders)
            collider.enabled = false;
        }
      }
    }

    [Serializable]
    public enum State
    {
      Puppet,
      Unpinned,
      GetUp,
    }

    [Serializable]
    public enum NormalMode
    {
      Active,
      Unmapped,
      Kinematic,
    }

    [Serializable]
    public class MasterProps
    {
      public NormalMode normalMode;
      public float mappingBlendSpeed = 10f;
      public bool activateOnStaticCollisions;
      public float activateOnImpulse;
    }

    [Serializable]
    public struct MuscleProps
    {
      [Tooltip("How much will collisions with muscles of this group unpin parent muscles?")]
      [Range(0.0f, 1f)]
      public float unpinParents;
      [Tooltip("How much will collisions with muscles of this group unpin child muscles?")]
      [Range(0.0f, 1f)]
      public float unpinChildren;
      [Tooltip("How much will collisions with muscles of this group unpin muscles of the same group?")]
      [Range(0.0f, 1f)]
      public float unpinGroup;
      [Tooltip("If 1, muscles of this group will always be mapped to the ragdoll.")]
      [Range(0.0f, 1f)]
      public float minMappingWeight;
      [Tooltip("If 0, muscles of this group will not be mapped to the ragdoll pose even if they are unpinned.")]
      [Range(0.0f, 1f)]
      public float maxMappingWeight;
      [Tooltip("If true, muscles of this group will have their colliders disabled while in puppet state (not unbalanced nor getting up).")]
      public bool disableColliders;
      [Tooltip("How fast will muscles of this group regain their pin weight (multiplier)?")]
      public float regainPinSpeed;
      [Tooltip("Smaller value means more unpinning from collisions (multiplier).")]
      public float collisionResistance;
      [Tooltip("If the distance from the muscle to it's target is larger than this value, the character will be knocked out.")]
      public float knockOutDistance;
      [Tooltip("The PhysicsMaterial applied to the muscles while the character is in Puppet or GetUp state. Using a lower friction material reduces the risk of muscles getting stuck and pulled out of their joints.")]
      public PhysicMaterial puppetMaterial;
      [Tooltip("The PhysicsMaterial applied to the muscles while the character is in Unpinned state.")]
      public PhysicMaterial unpinnedMaterial;
    }

    [Serializable]
    public struct MusclePropsGroup
    {
      [HideInInspector]
      public string name;
      [Tooltip("Muscle groups to which those properties apply.")]
      public Muscle.Group[] groups;
      [Tooltip("The muscle properties for those muscle groups.")]
      public MuscleProps props;
    }

    [Serializable]
    public struct CollisionResistanceMultiplier
    {
      public LayerMask layers;
      [Tooltip("Multiplier for the 'Collision Resistance' for these layers.")]
      public float multiplier;
      [Tooltip("Overrides 'Collision Threshold' for these layers.")]
      public float collisionThreshold;
    }

    public delegate void CollisionImpulseDelegate(MuscleCollision m, float impulse);
  }
}
