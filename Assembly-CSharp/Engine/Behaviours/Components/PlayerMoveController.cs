using System;
using Engine.Behaviours.Engines.Services;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Utility;
using Inspectors;
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

    public float StickToGroundForce => stickToGroundForce;

    public Vector3 MoveDirection => moveDir;

    public void Attach(IEntity owner)
    {
      controllerComponent = owner.GetComponent<ControllerComponent>();
    }

    private void OnEnable()
    {
      previouslyGrounded = false;
      falling = false;
    }

    public void Detach() => controllerComponent = null;

    private void Awake()
    {
      characterController = GetComponent<CharacterController>();
      behavior = GetComponent<EngineBehavior>();
      weaponService = GetComponent<PlayerWeaponServiceNew>();
    }

    protected void FixedUpdate()
    {
      if (characterController == null || controllerComponent == null || !PlayerUtility.IsPlayerCanControlling)
        return;
      UpdateCharacterPhysics(Time.fixedDeltaTime);
      UpdateStandart(Time.fixedDeltaTime);
    }

    private void UpdateStandart(float deltaTime)
    {
      if (!jump && !jumping)
      {
        jump = controllerComponent.IsJump;
        if (jump)
        {
          Action<bool> jumpEvent = JumpEvent;
          if (jumpEvent != null)
            jumpEvent(true);
        }
      }
      controllerComponent.IsJump = false;
      if (previouslyGrounded && !characterController.isGrounded)
      {
        beginFallingY = prevPositionY;
        falling = true;
      }
      if (!previouslyGrounded && characterController.isGrounded)
      {
        if (falling)
        {
          float num = beginFallingY - transform.position.y;
          Action<float> fallDamageEvent = FallDamageEvent;
          if (fallDamageEvent != null)
            fallDamageEvent(num);
          falling = false;
        }
        if (jumping)
        {
          Action<bool> jumpEvent = JumpEvent;
          if (jumpEvent != null)
            jumpEvent(false);
        }
        moveDir.y = 0.0f;
        jumping = false;
      }
      if (!characterController.isGrounded && !jumping && previouslyGrounded)
        moveDir.y = 0.0f;
      previouslyGrounded = characterController.isGrounded;
      prevPositionY = transform.position.y;
    }

    private void UpdateCharacterPhysics(float deltaTime)
    {
      if (!controllerComponent.WalkBlock.Value)
      {
        if (weaponService.IsWeaponOn())
        {
          Vector3 fightVelocity = GetFightVelocity();
          Vector3 vector = transform.forward * fightVelocity.z + transform.right * fightVelocity.x;
          LayerMask puddlesLayer = ScriptableObjectInstance<GameSettingsData>.Instance.PuddlesLayer;
          RaycastHit hitInfo;
          bool flag = Physics.Raycast(transform.position + new Vector3(0.0f, 0.5f, 0.0f), Vector3.down, out hitInfo, 1f, -1 ^ puddlesLayer, QueryTriggerInteraction.Ignore);
          Vector3 vector3 = Vector3.ProjectOnPlane(vector, flag ? hitInfo.normal : Vector3.up).normalized * fightVelocity.magnitude;
          moveDir.x = vector3.x;
          moveDir.z = vector3.z;
        }
        else
        {
          Vector3 velocity = GetVelocity();
          float num1 = velocity.z > 0.0 ? ScriptableObjectInstance<InputSettingsData>.Instance.RunSpeed : ScriptableObjectInstance<InputSettingsData>.Instance.RunBackSpeed;
          float num2 = velocity.z > 0.0 ? ScriptableObjectInstance<InputSettingsData>.Instance.WalkSpeed : ScriptableObjectInstance<InputSettingsData>.Instance.WalkBackSpeed;
          float num3 = controllerComponent.IsRun.Value ? num1 * controllerComponent.RunModifier.Value : (controllerComponent.IsStelth.Value ? ScriptableObjectInstance<InputSettingsData>.Instance.SneakSpeed : num2);
          Vector3 vector = transform.forward * velocity.z + transform.right * velocity.x;
          LayerMask puddlesLayer = ScriptableObjectInstance<GameSettingsData>.Instance.PuddlesLayer;
          RaycastHit hitInfo;
          bool flag = Physics.Raycast(transform.position + new Vector3(0.0f, 0.5f, 0.0f), Vector3.down, out hitInfo, 1f, -1 ^ puddlesLayer, QueryTriggerInteraction.Ignore);
          Vector3 vector3 = Vector3.ProjectOnPlane(vector, flag ? hitInfo.normal : Vector3.up).normalized * velocity.magnitude;
          moveDir.x = vector3.x * num3;
          moveDir.z = vector3.z * num3;
          moveDir *= Mathf.Min(1f, controllerComponent.WalkModifier.Value);
        }
      }
      else
        moveDir = Vector3.zero;
      moveDir += controllerComponent.PushVelocity;
      if (characterController.isGrounded)
      {
        moveDir.y = -stickToGroundForce;
        if (jump)
        {
          moveDir.y = ScriptableObjectInstance<InputSettingsData>.Instance.JumpSpeed;
          jump = false;
          jumping = true;
        }
      }
      else
        moveDir += Physics.gravity * ScriptableObjectInstance<InputSettingsData>.Instance.GravityMultiplier * deltaTime;
      if (DisableMovement)
        return;
      int num = (int) characterController.Move(moveDir * deltaTime);
    }

    private void OnAnimatorMove() => behavior.OnExternalAnimatorMove();

    private Vector3 GetFightVelocity()
    {
      Vector3 zero = Vector3.zero;
      if (!PlayerUtility.IsPlayerCanControlling)
        return zero;
      if (!controllerComponent.IsForward || !controllerComponent.IsBackward)
      {
        if (controllerComponent.IsForward)
        {
          if (controllerComponent.IsStelth.Value)
            zero.z += ScriptableObjectInstance<InputSettingsData>.Instance.FightForwardSneakSpeed;
          else if (controllerComponent.IsRun.Value)
            zero.z += ScriptableObjectInstance<InputSettingsData>.Instance.FightForwardRunSpeed;
          else
            zero.z += ScriptableObjectInstance<InputSettingsData>.Instance.FightForwardSpeed;
        }
        if (controllerComponent.IsBackward)
        {
          if (controllerComponent.IsStelth.Value)
            zero.z -= ScriptableObjectInstance<InputSettingsData>.Instance.FightBackwardSneakSpeed;
          else if (controllerComponent.IsRun.Value)
            zero.z -= ScriptableObjectInstance<InputSettingsData>.Instance.FightBackwardRunSpeed;
          else
            zero.z -= ScriptableObjectInstance<InputSettingsData>.Instance.FightBackwardSpeed;
        }
      }
      if (controllerComponent.IsRight)
      {
        if (controllerComponent.IsStelth.Value)
          zero.x += ScriptableObjectInstance<InputSettingsData>.Instance.FightStrafeSneakSpeed;
        else if (controllerComponent.IsRun.Value)
          zero.x += ScriptableObjectInstance<InputSettingsData>.Instance.FightStrafeRunSpeed;
        else
          zero.x += ScriptableObjectInstance<InputSettingsData>.Instance.FightStrafeSpeed;
      }
      if (controllerComponent.IsLeft)
      {
        if (controllerComponent.IsStelth.Value)
          zero.x -= ScriptableObjectInstance<InputSettingsData>.Instance.FightStrafeSneakSpeed;
        else if (controllerComponent.IsRun.Value)
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
      vector.z += controllerComponent.ForwardValue;
      vector.z -= controllerComponent.BackwardValue;
      vector.x += controllerComponent.RightValue;
      vector.x -= controllerComponent.LeftValue;
      vector = Vector3.ClampMagnitude(vector, 1f);
      return vector;
    }
  }
}
