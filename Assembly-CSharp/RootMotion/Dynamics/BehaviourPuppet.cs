// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.BehaviourPuppet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  [HelpURL("http://root-motion.com/puppetmasterdox/html/page10.html")]
  [AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Behaviours/BehaviourPuppet")]
  public class BehaviourPuppet : BehaviourBase
  {
    [LargeHeader("Collision And Recovery")]
    public BehaviourPuppet.MasterProps masterProps = new BehaviourPuppet.MasterProps();
    [Tooltip("Will ground the target to those layers when getting up.")]
    public LayerMask groundLayers;
    [Tooltip("Will unpin the muscles that collide with those layers.")]
    public LayerMask collisionLayers;
    [Tooltip("The collision impulse sqrMagnitude threshold under which collisions will be ignored.")]
    public float collisionThreshold;
    public Weight collisionResistance = new Weight(3f, "Smaller value means more unpinning from collisions so the characters get knocked out more easily. If using a curve, the value will be evaluated by each muscle's target velocity magnitude. This can be used to make collision resistance higher while the character moves or animates faster.");
    [Tooltip("Multiplies collision resistance for the specified layers.")]
    public BehaviourPuppet.CollisionResistanceMultiplier[] collisionResistanceMultipliers;
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
    public BehaviourPuppet.MuscleProps defaults;
    [Tooltip("Overriding default muscle properties for some muscle groups (for example making the feet stiffer or the hands looser).")]
    public BehaviourPuppet.MusclePropsGroup[] groupOverrides;
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
    public BehaviourBase.PuppetEvent onGetUpProne;
    [Tooltip("Called when the character starts getting up from a supine pose (facing up).")]
    public BehaviourBase.PuppetEvent onGetUpSupine;
    [Tooltip("Called when the character is knocked out (loses balance). Doesn't matter from which state.")]
    public BehaviourBase.PuppetEvent onLoseBalance;
    [Tooltip("Called when the character is knocked out (loses balance) only from the normal Puppet state.")]
    public BehaviourBase.PuppetEvent onLoseBalanceFromPuppet;
    [Tooltip("Called when the character is knocked out (loses balance) only from the GetUp state.")]
    public BehaviourBase.PuppetEvent onLoseBalanceFromGetUp;
    [Tooltip("Called when the character has fully recovered and switched to the Puppet state.")]
    public BehaviourBase.PuppetEvent onRegainBalance;
    public BehaviourPuppet.CollisionImpulseDelegate OnCollisionImpulse;
    [HideInInspector]
    public bool canMoveTarget = true;
    private float unpinnedTimer;
    private float getUpTimer;
    private Vector3 hipsForward;
    private Vector3 hipsUp;
    private float getupAnimationBlendWeight;
    private float getupAnimationBlendWeightV;
    private bool getUpTargetFixed;
    private BehaviourPuppet.NormalMode lastNormalMode;
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

    public BehaviourPuppet.State state { get; private set; }

    public override void OnReactivate()
    {
      this.state = this.puppetMaster.state == PuppetMaster.State.Alive ? BehaviourPuppet.State.Puppet : BehaviourPuppet.State.Unpinned;
      this.getUpTimer = 0.0f;
      this.unpinnedTimer = 0.0f;
      this.getupAnimationBlendWeight = 0.0f;
      this.getupAnimationBlendWeightV = 0.0f;
      this.getUpTargetFixed = false;
      this.getupDisabled = this.puppetMaster.state != 0;
      this.state = this.puppetMaster.state == PuppetMaster.State.Alive ? BehaviourPuppet.State.Puppet : BehaviourPuppet.State.Unpinned;
      foreach (Muscle muscle in this.puppetMaster.muscles)
        this.SetColliders(muscle, this.state == BehaviourPuppet.State.Unpinned);
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
      this.getUpPosition = pivot + deltaRotation * (this.getUpPosition - pivot) + deltaPosition;
      if (!moveToTarget)
        return;
      this.getupAnimationBlendWeight = 0.0f;
      this.getupAnimationBlendWeightV = 0.0f;
    }

    protected override void OnInitiate()
    {
      foreach (BehaviourPuppet.CollisionResistanceMultiplier resistanceMultiplier in this.collisionResistanceMultipliers)
      {
        if ((int) resistanceMultiplier.layers == 0)
          Debug.LogWarning((object) "BehaviourPuppet has a Collision Resistance Multiplier that's layers is set to Nothing. Please add some layers.", (UnityEngine.Object) this.transform);
      }
      foreach (Muscle muscle in this.puppetMaster.muscles)
      {
        if (muscle.joint.gameObject.layer == this.puppetMaster.targetRoot.gameObject.layer)
          Debug.LogWarning((object) "One of the ragdoll bones is on the same layer as the animated character. This might make the ragdoll collide with the character controller.");
        if (!Physics.GetIgnoreLayerCollision(muscle.joint.gameObject.layer, this.puppetMaster.targetRoot.gameObject.layer))
          Debug.LogWarning((object) ("The ragdoll layer (" + (object) muscle.joint.gameObject.layer + ") and the character controller layer (" + (object) this.puppetMaster.targetRoot.gameObject.layer + ") are not set to ignore each other in Edit/Project Settings/Physics/Layer Collision Matrix. This might cause the ragdoll bones to collide with the character controller."));
      }
      this.hipsForward = Quaternion.Inverse(this.puppetMaster.muscles[0].transform.rotation) * this.puppetMaster.targetRoot.forward;
      this.hipsUp = Quaternion.Inverse(this.puppetMaster.muscles[0].transform.rotation) * this.puppetMaster.targetRoot.up;
      this.state = BehaviourPuppet.State.Unpinned;
      this.eventsEnabled = true;
    }

    protected override void OnActivate()
    {
      bool flag = true;
      foreach (Muscle muscle in this.puppetMaster.muscles)
      {
        if ((double) muscle.state.pinWeightMlp > 0.5)
        {
          flag = false;
          break;
        }
      }
      bool eventsEnabled = this.eventsEnabled;
      this.eventsEnabled = false;
      if (flag)
        this.SetState(BehaviourPuppet.State.Unpinned);
      else
        this.SetState(BehaviourPuppet.State.Puppet);
      this.eventsEnabled = eventsEnabled;
    }

    public override void KillStart()
    {
      this.getupDisabled = true;
      foreach (Muscle muscle in this.puppetMaster.muscles)
      {
        muscle.state.pinWeightMlp = 0.0f;
        if (this.hasBoosted)
          muscle.state.immunity = 0.0f;
        this.SetColliders(muscle, true);
      }
    }

    public override void KillEnd() => this.SetState(BehaviourPuppet.State.Unpinned);

    public override void Resurrect()
    {
      this.getupDisabled = false;
      if (this.state != BehaviourPuppet.State.Unpinned)
        return;
      this.getUpTimer = float.PositiveInfinity;
      this.unpinnedTimer = float.PositiveInfinity;
      this.getupAnimationBlendWeight = 0.0f;
      this.getupAnimationBlendWeightV = 0.0f;
      foreach (Muscle muscle in this.puppetMaster.muscles)
        muscle.state.pinWeightMlp = 0.0f;
    }

    protected override void OnDeactivate() => this.state = BehaviourPuppet.State.Unpinned;

    protected override void OnFixedUpdate()
    {
      this.collisions = 0;
      if (this.dropPropFlag)
      {
        this.RemoveMusclesOfGroup(Muscle.Group.Prop);
        this.dropPropFlag = false;
      }
      if (!this.puppetMaster.isActive)
        this.SetState(BehaviourPuppet.State.Puppet);
      else if (!this.puppetMaster.isAlive)
      {
        foreach (Muscle muscle in this.puppetMaster.muscles)
        {
          muscle.state.pinWeightMlp = 0.0f;
          muscle.state.mappingWeightMlp = Mathf.MoveTowards(muscle.state.mappingWeightMlp, 1f, Time.deltaTime * 5f);
        }
      }
      else
      {
        if (this.hasBoosted)
        {
          foreach (Muscle muscle in this.puppetMaster.muscles)
          {
            muscle.state.immunity = Mathf.MoveTowards(muscle.state.immunity, 0.0f, Time.deltaTime * this.boostFalloff);
            muscle.state.impulseMlp = Mathf.Lerp(muscle.state.impulseMlp, 1f, Time.deltaTime * this.boostFalloff);
          }
        }
        if (this.state == BehaviourPuppet.State.Unpinned)
        {
          this.unpinnedTimer += Time.deltaTime;
          if ((double) this.unpinnedTimer >= (double) this.getUpDelay && this.canGetUp && !this.getupDisabled && (double) this.puppetMaster.muscles[0].rigidbody.velocity.magnitude < (double) this.maxGetUpVelocity)
          {
            this.SetState(BehaviourPuppet.State.GetUp);
            return;
          }
          foreach (Muscle muscle in this.puppetMaster.muscles)
          {
            muscle.state.pinWeightMlp = 0.0f;
            muscle.state.mappingWeightMlp = Mathf.MoveTowards(muscle.state.mappingWeightMlp, 1f, Time.deltaTime * this.masterProps.mappingBlendSpeed);
          }
        }
        if (this.state != BehaviourPuppet.State.Unpinned)
        {
          if ((double) this.knockOutDistance != (double) this.lastKnockOutDistance)
          {
            this.knockOutDistanceSqr = Mathf.Sqrt(this.knockOutDistance);
            this.lastKnockOutDistance = this.knockOutDistance;
          }
          foreach (Muscle muscle in this.puppetMaster.muscles)
          {
            BehaviourPuppet.MuscleProps props = this.GetProps(muscle.props.group);
            float b = 1f;
            if (this.state == BehaviourPuppet.State.GetUp)
              b = Mathf.Lerp(this.getUpKnockOutDistanceMlp, b, muscle.state.pinWeightMlp);
            float num1 = this.unpinnedMuscleKnockout ? muscle.positionOffset.sqrMagnitude : muscle.positionOffset.sqrMagnitude * muscle.props.pinWeight;
            if (this.hasCollidedSinceGetUp && !this.puppetMaster.isBlending && (double) num1 > 0.0 && (double) muscle.state.pinWeightMlp < (double) this.pinWeightThreshold && (double) num1 > (double) props.knockOutDistance * (double) this.knockOutDistanceSqr * (double) b)
            {
              if (this.state == BehaviourPuppet.State.GetUp && !this.getUpTargetFixed)
                return;
              this.SetState(BehaviourPuppet.State.Unpinned);
              return;
            }
            muscle.state.muscleWeightMlp = Mathf.Lerp(this.unpinnedMuscleWeightMlp, 1f, muscle.state.pinWeightMlp);
            if (this.state == BehaviourPuppet.State.GetUp)
              muscle.state.muscleDamperAdd = 0.0f;
            if (!this.puppetMaster.isKilling)
            {
              float num2 = 1f;
              if (this.state == BehaviourPuppet.State.GetUp)
                num2 = Mathf.Lerp(this.getUpRegainPinSpeedMlp, 1f, muscle.state.pinWeightMlp);
              muscle.state.pinWeightMlp += Time.deltaTime * props.regainPinSpeed * this.regainPinSpeed * num2;
            }
          }
          float num = 1f;
          foreach (Muscle muscle in this.puppetMaster.muscles)
          {
            if ((muscle.props.group == Muscle.Group.Leg || muscle.props.group == Muscle.Group.Foot) && (double) muscle.state.pinWeightMlp < (double) num)
              num = muscle.state.pinWeightMlp;
          }
          foreach (Muscle muscle in this.puppetMaster.muscles)
            muscle.state.pinWeightMlp = Mathf.Clamp(muscle.state.pinWeightMlp, 0.0f, num * 5f);
        }
        if (this.state != BehaviourPuppet.State.GetUp)
          return;
        this.getUpTimer += Time.deltaTime;
        if ((double) this.getUpTimer > (double) this.minGetUpDuration)
        {
          this.getUpTimer = 0.0f;
          this.SetState(BehaviourPuppet.State.Puppet);
        }
      }
    }

    protected override void OnLateUpdate()
    {
      this.forceActive = this.state != 0;
      if (!this.puppetMaster.isAlive)
        return;
      if (this.masterProps.normalMode != this.lastNormalMode)
      {
        if (this.lastNormalMode == BehaviourPuppet.NormalMode.Unmapped)
        {
          foreach (Muscle muscle in this.puppetMaster.muscles)
            muscle.state.mappingWeightMlp = 1f;
        }
        if (this.lastNormalMode == BehaviourPuppet.NormalMode.Kinematic && this.puppetMaster.mode == PuppetMaster.Mode.Kinematic)
          this.puppetMaster.mode = PuppetMaster.Mode.Active;
        this.lastNormalMode = this.masterProps.normalMode;
      }
      switch (this.masterProps.normalMode)
      {
        case BehaviourPuppet.NormalMode.Unmapped:
          if (!this.puppetMaster.isActive)
            break;
          bool to = false;
          for (int muscleIndex = 0; muscleIndex < this.puppetMaster.muscles.Length; ++muscleIndex)
            this.BlendMuscleMapping(muscleIndex, ref to);
          break;
        case BehaviourPuppet.NormalMode.Kinematic:
          if (!this.SetKinematic())
            break;
          this.puppetMaster.mode = PuppetMaster.Mode.Kinematic;
          break;
      }
    }

    private bool SetKinematic()
    {
      if (!this.puppetMaster.isActive || this.state != 0 || this.puppetMaster.isBlending || (double) this.getupAnimationBlendWeight > 0.0 || !this.puppetMaster.isAlive)
        return false;
      foreach (Muscle muscle in this.puppetMaster.muscles)
      {
        if ((double) muscle.state.pinWeightMlp < 1.0)
          return false;
      }
      return true;
    }

    protected override void OnReadBehaviour()
    {
      if (!this.enabled)
        return;
      if (!this.puppetMaster.isFrozen && this.state == BehaviourPuppet.State.Unpinned && this.puppetMaster.isActive)
      {
        this.MoveTarget(this.puppetMaster.muscles[0].rigidbody.position);
        this.GroundTarget(this.groundLayers);
        this.getUpPosition = this.puppetMaster.targetRoot.position;
      }
      if ((double) this.getupAnimationBlendWeight > 0.0)
      {
        this.getUpPosition += Vector3.Project(this.puppetMaster.targetRoot.position - this.getUpPosition, this.puppetMaster.targetRoot.up);
        this.MoveTarget(this.getUpPosition);
        this.getupAnimationBlendWeight = Mathf.SmoothDamp(this.getupAnimationBlendWeight, 0.0f, ref this.getupAnimationBlendWeightV, this.blendToAnimationTime);
        if ((double) this.getupAnimationBlendWeight < 0.0099999997764825821)
          this.getupAnimationBlendWeight = 0.0f;
        this.puppetMaster.FixTargetToSampledState(this.getupAnimationBlendWeight);
      }
      this.getUpTargetFixed = true;
    }

    private void BlendMuscleMapping(int muscleIndex, ref bool to)
    {
      if ((double) this.puppetMaster.muscles[muscleIndex].state.pinWeightMlp < 1.0)
        to = true;
      BehaviourPuppet.MuscleProps props = this.GetProps(this.puppetMaster.muscles[muscleIndex].props.group);
      float target = to ? (this.state == BehaviourPuppet.State.Puppet ? props.maxMappingWeight : 1f) : props.minMappingWeight;
      this.puppetMaster.muscles[muscleIndex].state.mappingWeightMlp = Mathf.MoveTowards(this.puppetMaster.muscles[muscleIndex].state.mappingWeightMlp, target, Time.deltaTime * this.masterProps.mappingBlendSpeed);
    }

    public override void OnMuscleAdded(Muscle m)
    {
      base.OnMuscleAdded(m);
      this.SetColliders(m, this.state == BehaviourPuppet.State.Unpinned);
    }

    public override void OnMuscleRemoved(Muscle m)
    {
      base.OnMuscleRemoved(m);
      this.SetColliders(m, true);
    }

    protected void MoveTarget(Vector3 position)
    {
      if (!this.canMoveTarget)
        return;
      this.puppetMaster.targetRoot.position = position;
    }

    protected void RotateTarget(Quaternion rotation)
    {
      if (!this.canMoveTarget)
        return;
      this.puppetMaster.targetRoot.rotation = rotation;
    }

    protected override void GroundTarget(LayerMask layers)
    {
      if (!this.canMoveTarget)
        return;
      base.GroundTarget(layers);
    }

    private void OnDrawGizmosSelected()
    {
      for (int index1 = 0; index1 < this.groupOverrides.Length; ++index1)
      {
        this.groupOverrides[index1].name = string.Empty;
        if (this.groupOverrides[index1].groups.Length != 0)
        {
          for (int index2 = 0; index2 < this.groupOverrides[index1].groups.Length; ++index2)
          {
            if (index2 > 0)
            {
              // ISSUE: explicit reference operation
              ^ref this.groupOverrides[index1].name += ", ";
            }
            // ISSUE: explicit reference operation
            ^ref this.groupOverrides[index1].name += this.groupOverrides[index1].groups[index2].ToString();
          }
        }
      }
    }

    public void Boost(float immunity, float impulseMlp)
    {
      this.hasBoosted = true;
      for (int muscleIndex = 0; muscleIndex < this.puppetMaster.muscles.Length; ++muscleIndex)
        this.Boost(muscleIndex, immunity, impulseMlp);
    }

    public void Boost(int muscleIndex, float immunity, float impulseMlp)
    {
      this.hasBoosted = true;
      this.BoostImmunity(muscleIndex, immunity);
      this.BoostImpulseMlp(muscleIndex, impulseMlp);
    }

    public void Boost(
      int muscleIndex,
      float immunity,
      float impulseMlp,
      float boostParents,
      float boostChildren)
    {
      this.hasBoosted = true;
      if ((double) boostParents <= 0.0 && (double) boostChildren <= 0.0)
      {
        this.Boost(muscleIndex, immunity, impulseMlp);
      }
      else
      {
        for (int index = 0; index < this.puppetMaster.muscles.Length; ++index)
        {
          float falloff = this.GetFalloff(index, muscleIndex, boostParents, boostChildren);
          this.Boost(index, immunity * falloff, impulseMlp * falloff);
        }
      }
    }

    public void BoostImmunity(float immunity)
    {
      this.hasBoosted = true;
      if ((double) immunity < 0.0)
        return;
      for (int muscleIndex = 0; muscleIndex < this.puppetMaster.muscles.Length; ++muscleIndex)
        this.BoostImmunity(muscleIndex, immunity);
    }

    public void BoostImmunity(int muscleIndex, float immunity)
    {
      this.hasBoosted = true;
      this.puppetMaster.muscles[muscleIndex].state.immunity = Mathf.Clamp(immunity, this.puppetMaster.muscles[muscleIndex].state.immunity, 1f);
    }

    public void BoostImmunity(
      int muscleIndex,
      float immunity,
      float boostParents,
      float boostChildren)
    {
      this.hasBoosted = true;
      for (int index = 0; index < this.puppetMaster.muscles.Length; ++index)
      {
        float falloff = this.GetFalloff(index, muscleIndex, boostParents, boostChildren);
        this.BoostImmunity(index, immunity * falloff);
      }
    }

    public void BoostImpulseMlp(float impulseMlp)
    {
      this.hasBoosted = true;
      for (int muscleIndex = 0; muscleIndex < this.puppetMaster.muscles.Length; ++muscleIndex)
        this.BoostImpulseMlp(muscleIndex, impulseMlp);
    }

    public void BoostImpulseMlp(int muscleIndex, float impulseMlp)
    {
      this.hasBoosted = true;
      this.puppetMaster.muscles[muscleIndex].state.impulseMlp = Mathf.Max(impulseMlp, this.puppetMaster.muscles[muscleIndex].state.impulseMlp);
    }

    public void BoostImpulseMlp(
      int muscleIndex,
      float impulseMlp,
      float boostParents,
      float boostChildren)
    {
      this.hasBoosted = true;
      for (int index = 0; index < this.puppetMaster.muscles.Length; ++index)
      {
        float falloff = this.GetFalloff(index, muscleIndex, boostParents, boostChildren);
        this.BoostImpulseMlp(index, impulseMlp * falloff);
      }
    }

    public void Unpin()
    {
      Debug.Log((object) "BehaviourPuppet.Unpin() has been deprecated. Use SetState(BehaviourPuppet.State) instead.");
      this.SetState(BehaviourPuppet.State.Unpinned);
    }

    protected override void OnMuscleHitBehaviour(MuscleHit hit)
    {
      if (this.masterProps.normalMode == BehaviourPuppet.NormalMode.Kinematic)
        this.puppetMaster.mode = PuppetMaster.Mode.Active;
      this.UnPin(hit.muscleIndex, hit.unPin);
      this.puppetMaster.muscles[hit.muscleIndex].rigidbody.isKinematic = false;
      this.puppetMaster.muscles[hit.muscleIndex].rigidbody.AddForceAtPosition(hit.force, hit.position);
    }

    protected override void OnMuscleCollisionBehaviour(MuscleCollision m)
    {
      if (!this.enabled || this.state == BehaviourPuppet.State.Unpinned || this.collisions > this.maxCollisions || !LayerMaskExtensions.Contains(this.collisionLayers, m.collision.gameObject.layer) || this.masterProps.normalMode == BehaviourPuppet.NormalMode.Kinematic && !this.puppetMaster.isActive && !this.masterProps.activateOnStaticCollisions && m.collision.gameObject.isStatic)
        return;
      float collisionThreshold = this.collisionThreshold;
      float impulse = this.GetImpulse(m, ref collisionThreshold);
      if (this.OnCollisionImpulse != null)
        this.OnCollisionImpulse(m, impulse);
      float num1 = (UnityEngine.Object) Singleton<PuppetMasterSettings>.instance != (UnityEngine.Object) null ? (float) (1.0 + (double) Singleton<PuppetMasterSettings>.instance.currentlyActivePuppets * (double) Singleton<PuppetMasterSettings>.instance.activePuppetCollisionThresholdMlp) : 1f;
      float num2 = collisionThreshold * num1;
      if ((double) impulse <= (double) num2)
        return;
      ++this.collisions;
      if ((UnityEngine.Object) m.collision.collider.attachedRigidbody != (UnityEngine.Object) null)
      {
        this.broadcaster = m.collision.collider.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();
        if ((UnityEngine.Object) this.broadcaster != (UnityEngine.Object) null && this.broadcaster.muscleIndex < this.broadcaster.puppetMaster.muscles.Length)
          impulse *= this.broadcaster.puppetMaster.muscles[this.broadcaster.muscleIndex].state.impulseMlp;
      }
      if (this.Activate(m.collision, impulse))
        this.puppetMaster.mode = PuppetMaster.Mode.Active;
      this.UnPin(m.muscleIndex, impulse);
    }

    private float GetImpulse(MuscleCollision m, ref float layerThreshold)
    {
      float impulse = m.collision.impulse.sqrMagnitude / this.puppetMaster.muscles[m.muscleIndex].rigidbody.mass * 0.3f;
      foreach (BehaviourPuppet.CollisionResistanceMultiplier resistanceMultiplier in this.collisionResistanceMultipliers)
      {
        if (LayerMaskExtensions.Contains(resistanceMultiplier.layers, m.collision.collider.gameObject.layer))
        {
          if ((double) resistanceMultiplier.multiplier <= 0.0)
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
      if (muscleIndex >= this.puppetMaster.muscles.Length)
        return;
      BehaviourPuppet.MuscleProps props = this.GetProps(this.puppetMaster.muscles[muscleIndex].props.group);
      for (int index = 0; index < this.puppetMaster.muscles.Length; ++index)
        this.UnPinMuscle(index, unpin * this.GetFalloff(index, muscleIndex, props.unpinParents, props.unpinChildren, props.unpinGroup));
      this.hasCollidedSinceGetUp = true;
    }

    private void UnPinMuscle(int muscleIndex, float unpin)
    {
      if ((double) unpin <= 0.0 || (double) this.puppetMaster.muscles[muscleIndex].state.immunity >= 1.0)
        return;
      BehaviourPuppet.MuscleProps props = this.GetProps(this.puppetMaster.muscles[muscleIndex].props.group);
      float num1 = 1f;
      if (this.state == BehaviourPuppet.State.GetUp)
        num1 = Mathf.Lerp(this.getUpCollisionResistanceMlp, 1f, this.puppetMaster.muscles[muscleIndex].state.pinWeightMlp);
      float num2 = this.collisionResistance.mode == Weight.Mode.Float ? this.collisionResistance.floatValue : this.collisionResistance.GetValue(this.puppetMaster.muscles[muscleIndex].targetVelocity.magnitude);
      float num3 = unpin / (props.collisionResistance * num2 * num1) * (1f - this.puppetMaster.muscles[muscleIndex].state.immunity);
      this.puppetMaster.muscles[muscleIndex].state.pinWeightMlp -= num3;
    }

    private bool Activate(Collision collision, float impulse)
    {
      if (this.masterProps.normalMode != BehaviourPuppet.NormalMode.Kinematic || this.puppetMaster.mode != PuppetMaster.Mode.Kinematic || (double) impulse < (double) this.masterProps.activateOnImpulse)
        return false;
      return !collision.gameObject.isStatic || this.masterProps.activateOnStaticCollisions;
    }

    public bool IsProne()
    {
      return (double) Vector3.Dot(this.puppetMaster.muscles[0].transform.rotation * this.hipsForward, this.puppetMaster.targetRoot.up) < 0.0;
    }

    private float GetFalloff(int i, int muscleIndex, float falloffParents, float falloffChildren)
    {
      if (i == muscleIndex)
        return 1f;
      bool childFlag = this.puppetMaster.muscles[muscleIndex].childFlags[i];
      int kinshipDegree = this.puppetMaster.muscles[muscleIndex].kinshipDegrees[i];
      return Mathf.Pow(childFlag ? falloffChildren : falloffParents, (float) kinshipDegree);
    }

    private float GetFalloff(
      int i,
      int muscleIndex,
      float falloffParents,
      float falloffChildren,
      float falloffGroup)
    {
      float a = this.GetFalloff(i, muscleIndex, falloffParents, falloffChildren);
      if ((double) falloffGroup > 0.0 && i != muscleIndex && this.InGroup(this.puppetMaster.muscles[i].props.group, this.puppetMaster.muscles[muscleIndex].props.group))
        a = Mathf.Max(a, falloffGroup);
      return a;
    }

    private bool InGroup(Muscle.Group group1, Muscle.Group group2)
    {
      if (group1 == group2)
        return true;
      foreach (BehaviourPuppet.MusclePropsGroup groupOverride in this.groupOverrides)
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

    private BehaviourPuppet.MuscleProps GetProps(Muscle.Group group)
    {
      foreach (BehaviourPuppet.MusclePropsGroup groupOverride in this.groupOverrides)
      {
        foreach (Muscle.Group group1 in groupOverride.groups)
        {
          if (group1 == group)
            return groupOverride.props;
        }
      }
      return this.defaults;
    }

    public void SetState(BehaviourPuppet.State newState)
    {
      if (this.state == newState)
        return;
      switch (newState)
      {
        case BehaviourPuppet.State.Puppet:
          this.puppetMaster.SampleTargetMappedState();
          this.unpinnedTimer = 0.0f;
          this.getUpTimer = 0.0f;
          if (this.state == BehaviourPuppet.State.Unpinned)
          {
            foreach (Muscle muscle in this.puppetMaster.muscles)
            {
              muscle.state.pinWeightMlp = 1f;
              muscle.state.muscleWeightMlp = 1f;
              muscle.state.muscleDamperAdd = 0.0f;
              this.SetColliders(muscle, false);
            }
          }
          this.state = BehaviourPuppet.State.Puppet;
          if (this.eventsEnabled)
          {
            this.onRegainBalance.Trigger(this.puppetMaster);
            if (this.onRegainBalance.switchBehaviour)
              return;
            break;
          }
          break;
        case BehaviourPuppet.State.Unpinned:
          this.unpinnedTimer = 0.0f;
          this.getUpTimer = 0.0f;
          this.getupAnimationBlendWeight = 0.0f;
          this.getupAnimationBlendWeightV = 0.0f;
          foreach (Muscle muscle in this.puppetMaster.muscles)
          {
            if (this.hasBoosted)
              muscle.state.immunity = 0.0f;
            if ((double) this.maxRigidbodyVelocity != double.PositiveInfinity)
              muscle.rigidbody.velocity = Vector3.ClampMagnitude(muscle.rigidbody.velocity, this.maxRigidbodyVelocity);
            this.SetColliders(muscle, true);
          }
          if (this.dropProps)
            this.dropPropFlag = true;
          foreach (Muscle muscle in this.puppetMaster.muscles)
            muscle.state.muscleWeightMlp = this.puppetMaster.isAlive ? this.unpinnedMuscleWeightMlp : this.puppetMaster.stateSettings.deadMuscleWeight;
          this.onLoseBalance.Trigger(this.puppetMaster, this.puppetMaster.isAlive);
          if (this.onLoseBalance.switchBehaviour)
          {
            this.state = BehaviourPuppet.State.Unpinned;
            return;
          }
          if (this.state == BehaviourPuppet.State.Puppet)
          {
            this.onLoseBalanceFromPuppet.Trigger(this.puppetMaster, this.puppetMaster.isAlive);
            if (this.onLoseBalanceFromPuppet.switchBehaviour)
            {
              this.state = BehaviourPuppet.State.Unpinned;
              return;
            }
          }
          else
          {
            this.onLoseBalanceFromGetUp.Trigger(this.puppetMaster, this.puppetMaster.isAlive);
            if (this.onLoseBalanceFromGetUp.switchBehaviour)
            {
              this.state = BehaviourPuppet.State.Unpinned;
              return;
            }
          }
          foreach (Muscle muscle in this.puppetMaster.muscles)
            muscle.state.pinWeightMlp = 0.0f;
          break;
        case BehaviourPuppet.State.GetUp:
          this.unpinnedTimer = 0.0f;
          this.getUpTimer = 0.0f;
          this.hasCollidedSinceGetUp = false;
          bool flag = this.IsProne();
          this.state = BehaviourPuppet.State.GetUp;
          if (flag)
          {
            this.onGetUpProne.Trigger(this.puppetMaster);
            if (this.onGetUpProne.switchBehaviour)
              return;
          }
          else
          {
            this.onGetUpSupine.Trigger(this.puppetMaster);
            if (this.onGetUpSupine.switchBehaviour)
              return;
          }
          foreach (Muscle muscle in this.puppetMaster.muscles)
          {
            muscle.state.muscleWeightMlp = 0.0f;
            muscle.state.pinWeightMlp = 0.0f;
            muscle.state.muscleDamperAdd = 0.0f;
            this.SetColliders(muscle, false);
          }
          Vector3 tangent = this.puppetMaster.muscles[0].rigidbody.rotation * this.hipsUp;
          Vector3 up = this.puppetMaster.targetRoot.up;
          Vector3.OrthoNormalize(ref up, ref tangent);
          this.RotateTarget(Quaternion.LookRotation(flag ? tangent : -tangent, this.puppetMaster.targetRoot.up));
          this.puppetMaster.SampleTargetMappedState();
          this.MoveTarget(this.puppetMaster.muscles[0].rigidbody.position + this.puppetMaster.targetRoot.rotation * (flag ? this.getUpOffsetProne : this.getUpOffsetSupine));
          this.GroundTarget(this.groundLayers);
          this.getUpPosition = this.puppetMaster.targetRoot.position;
          this.getupAnimationBlendWeight = 1f;
          this.getUpTargetFixed = false;
          break;
      }
      this.state = newState;
    }

    public void SetColliders(bool unpinned)
    {
      foreach (Muscle muscle in this.puppetMaster.muscles)
        this.SetColliders(muscle, unpinned);
    }

    private void SetColliders(Muscle m, bool unpinned)
    {
      BehaviourPuppet.MuscleProps props = this.GetProps(m.props.group);
      if (unpinned)
      {
        foreach (Collider collider in m.colliders)
        {
          collider.material = (UnityEngine.Object) props.unpinnedMaterial != (UnityEngine.Object) null ? props.unpinnedMaterial : this.defaults.unpinnedMaterial;
          if (props.disableColliders)
            collider.enabled = true;
        }
      }
      else
      {
        foreach (Collider collider in m.colliders)
        {
          collider.material = (UnityEngine.Object) props.puppetMaterial != (UnityEngine.Object) null ? props.puppetMaterial : this.defaults.puppetMaterial;
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
      public BehaviourPuppet.NormalMode normalMode;
      public float mappingBlendSpeed = 10f;
      public bool activateOnStaticCollisions;
      public float activateOnImpulse = 0.0f;
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
      public BehaviourPuppet.MuscleProps props;
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
