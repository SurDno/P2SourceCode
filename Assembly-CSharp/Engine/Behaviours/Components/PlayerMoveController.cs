using Engine.Behaviours.Engines.Services;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Utility;
using Inspectors;
using System;
using UnityEngine;

namespace Engine.Behaviours.Components
{
  public class PlayerMoveController : MonoBehaviour, IEntityAttachable
  {
    [SerializeField]
    private float stickToGroundForce;
    private CharacterController characterController;
    private EngineBehavior behavior;
    [Inspected]
    private Vector3 moveDir = Vector3.zero;
    private bool previouslyGrounded;
    private bool jump;
    private bool jumping;
    private bool falling;
    private float beginFallingY;
    private float prevPositionY;
    private ControllerComponent controllerComponent;
    private PlayerWeaponServiceNew weaponService;

    public event Action<float> FallDamageEvent;

    public event Action<bool> JumpEvent;

    public bool DisableMovement { get; set; }

    public float StickToGroundForce => this.stickToGroundForce;

    public Vector3 MoveDirection => this.moveDir;

    public void Attach(IEntity owner)
    {
      this.controllerComponent = owner.GetComponent<ControllerComponent>();
    }

    private void OnEnable()
    {
      this.previouslyGrounded = false;
      this.falling = false;
    }

    public void Detach() => this.controllerComponent = (ControllerComponent) null;

    private void Awake()
    {
      this.characterController = this.GetComponent<CharacterController>();
      this.behavior = this.GetComponent<EngineBehavior>();
      this.weaponService = this.GetComponent<PlayerWeaponServiceNew>();
    }

    protected void FixedUpdate()
    {
      if ((UnityEngine.Object) this.characterController == (UnityEngine.Object) null || this.controllerComponent == null || !PlayerUtility.IsPlayerCanControlling)
        return;
      this.UpdateCharacterPhysics(Time.fixedDeltaTime);
      this.UpdateStandart(Time.fixedDeltaTime);
    }

    private void UpdateStandart(float deltaTime)
    {
      if (!this.jump && !this.jumping)
      {
        this.jump = this.controllerComponent.IsJump;
        if (this.jump)
        {
          Action<bool> jumpEvent = this.JumpEvent;
          if (jumpEvent != null)
            jumpEvent(true);
        }
      }
      this.controllerComponent.IsJump = false;
      if (this.previouslyGrounded && !this.characterController.isGrounded)
      {
        this.beginFallingY = this.prevPositionY;
        this.falling = true;
      }
      if (!this.previouslyGrounded && this.characterController.isGrounded)
      {
        if (this.falling)
        {
          float num = this.beginFallingY - this.transform.position.y;
          Action<float> fallDamageEvent = this.FallDamageEvent;
          if (fallDamageEvent != null)
            fallDamageEvent(num);
          this.falling = false;
        }
        if (this.jumping)
        {
          Action<bool> jumpEvent = this.JumpEvent;
          if (jumpEvent != null)
            jumpEvent(false);
        }
        this.moveDir.y = 0.0f;
        this.jumping = false;
      }
      if (!this.characterController.isGrounded && !this.jumping && this.previouslyGrounded)
        this.moveDir.y = 0.0f;
      this.previouslyGrounded = this.characterController.isGrounded;
      this.prevPositionY = this.transform.position.y;
    }

    private void UpdateCharacterPhysics(float deltaTime)
    {
      if (!this.controllerComponent.WalkBlock.Value)
      {
        if (this.weaponService.IsWeaponOn())
        {
          Vector3 fightVelocity = this.GetFightVelocity();
          Vector3 vector = this.transform.forward * fightVelocity.z + this.transform.right * fightVelocity.x;
          LayerMask puddlesLayer = ScriptableObjectInstance<GameSettingsData>.Instance.PuddlesLayer;
          RaycastHit hitInfo;
          bool flag = Physics.Raycast(this.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Vector3.down, out hitInfo, 1f, -1 ^ (int) puddlesLayer, QueryTriggerInteraction.Ignore);
          Vector3 vector3 = Vector3.ProjectOnPlane(vector, flag ? hitInfo.normal : Vector3.up).normalized * fightVelocity.magnitude;
          this.moveDir.x = vector3.x;
          this.moveDir.z = vector3.z;
        }
        else
        {
          Vector3 velocity = this.GetVelocity();
          float num1 = (double) velocity.z > 0.0 ? ScriptableObjectInstance<InputSettingsData>.Instance.RunSpeed : ScriptableObjectInstance<InputSettingsData>.Instance.RunBackSpeed;
          float num2 = (double) velocity.z > 0.0 ? ScriptableObjectInstance<InputSettingsData>.Instance.WalkSpeed : ScriptableObjectInstance<InputSettingsData>.Instance.WalkBackSpeed;
          float num3 = this.controllerComponent.IsRun.Value ? num1 * this.controllerComponent.RunModifier.Value : (this.controllerComponent.IsStelth.Value ? ScriptableObjectInstance<InputSettingsData>.Instance.SneakSpeed : num2);
          Vector3 vector = this.transform.forward * velocity.z + this.transform.right * velocity.x;
          LayerMask puddlesLayer = ScriptableObjectInstance<GameSettingsData>.Instance.PuddlesLayer;
          RaycastHit hitInfo;
          bool flag = Physics.Raycast(this.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Vector3.down, out hitInfo, 1f, -1 ^ (int) puddlesLayer, QueryTriggerInteraction.Ignore);
          Vector3 vector3 = Vector3.ProjectOnPlane(vector, flag ? hitInfo.normal : Vector3.up).normalized * velocity.magnitude;
          this.moveDir.x = vector3.x * num3;
          this.moveDir.z = vector3.z * num3;
          this.moveDir *= Mathf.Min(1f, this.controllerComponent.WalkModifier.Value);
        }
      }
      else
        this.moveDir = Vector3.zero;
      this.moveDir += this.controllerComponent.PushVelocity;
      if (this.characterController.isGrounded)
      {
        this.moveDir.y = -this.stickToGroundForce;
        if (this.jump)
        {
          this.moveDir.y = ScriptableObjectInstance<InputSettingsData>.Instance.JumpSpeed;
          this.jump = false;
          this.jumping = true;
        }
      }
      else
        this.moveDir += Physics.gravity * ScriptableObjectInstance<InputSettingsData>.Instance.GravityMultiplier * deltaTime;
      if (this.DisableMovement)
        return;
      int num = (int) this.characterController.Move(this.moveDir * deltaTime);
    }

    private void OnAnimatorMove() => this.behavior.OnExternalAnimatorMove();

    private Vector3 GetFightVelocity()
    {
      Vector3 zero = Vector3.zero;
      if (!PlayerUtility.IsPlayerCanControlling)
        return zero;
      if (!this.controllerComponent.IsForward || !this.controllerComponent.IsBackward)
      {
        if (this.controllerComponent.IsForward)
        {
          if (this.controllerComponent.IsStelth.Value)
            zero.z += ScriptableObjectInstance<InputSettingsData>.Instance.FightForwardSneakSpeed;
          else if (this.controllerComponent.IsRun.Value)
            zero.z += ScriptableObjectInstance<InputSettingsData>.Instance.FightForwardRunSpeed;
          else
            zero.z += ScriptableObjectInstance<InputSettingsData>.Instance.FightForwardSpeed;
        }
        if (this.controllerComponent.IsBackward)
        {
          if (this.controllerComponent.IsStelth.Value)
            zero.z -= ScriptableObjectInstance<InputSettingsData>.Instance.FightBackwardSneakSpeed;
          else if (this.controllerComponent.IsRun.Value)
            zero.z -= ScriptableObjectInstance<InputSettingsData>.Instance.FightBackwardRunSpeed;
          else
            zero.z -= ScriptableObjectInstance<InputSettingsData>.Instance.FightBackwardSpeed;
        }
      }
      if (this.controllerComponent.IsRight)
      {
        if (this.controllerComponent.IsStelth.Value)
          zero.x += ScriptableObjectInstance<InputSettingsData>.Instance.FightStrafeSneakSpeed;
        else if (this.controllerComponent.IsRun.Value)
          zero.x += ScriptableObjectInstance<InputSettingsData>.Instance.FightStrafeRunSpeed;
        else
          zero.x += ScriptableObjectInstance<InputSettingsData>.Instance.FightStrafeSpeed;
      }
      if (this.controllerComponent.IsLeft)
      {
        if (this.controllerComponent.IsStelth.Value)
          zero.x -= ScriptableObjectInstance<InputSettingsData>.Instance.FightStrafeSneakSpeed;
        else if (this.controllerComponent.IsRun.Value)
          zero.x -= ScriptableObjectInstance<InputSettingsData>.Instance.FightStrafeRunSpeed;
        else
          zero.x -= ScriptableObjectInstance<InputSettingsData>.Instance.FightStrafeSpeed;
      }
      return zero;
    }

    private Vector3 GetVelocity()
    {
      Vector3 vector = Vector3.zero;
      if (!PlayerUtility.IsPlayerCanControlling)
        return vector;
      vector.z += this.controllerComponent.ForwardValue;
      vector.z -= this.controllerComponent.BackwardValue;
      vector.x += this.controllerComponent.RightValue;
      vector.x -= this.controllerComponent.LeftValue;
      vector = Vector3.ClampMagnitude(vector, 1f);
      return vector;
    }
  }
}
