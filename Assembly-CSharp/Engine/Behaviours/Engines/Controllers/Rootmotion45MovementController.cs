using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Engine.Behaviours.Engines.Controllers
{
  public class Rootmotion45MovementController : IMovementController
  {
    private Animator animator;
    private Rigidbody rigidbody;
    private Pivot pivot;
    private Rootmotion45 rootmotion45;
    private CapsuleCollider collider;
    private NavMeshAgent agent;
    private GameObject gameObject;
    private Quaternion deltaRotation = Quaternion.identity;
    private float movementSpeed;
    private float rotationError = 0.0f;
    private bool isPaused;
    private bool movementDone = false;
    private float desiredMovemetSpeed = 0.0f;
    private Vector3 direction;
    private float remainingDistance;
    private EngineBehavior.GaitType gait;
    private float rotationSpeed = 270f;

    public bool IsPaused
    {
      get => this.isPaused;
      set
      {
        this.isPaused = value;
        if (!((UnityEngine.Object) this.animator != (UnityEngine.Object) null) || !this.animator.gameObject.activeSelf)
          return;
        if (this.isPaused)
          this.animator.SetFloat("Mecanim.Speed", 0.0f);
        else
          this.animator.SetFloat("Mecanim.Speed", 1f);
      }
    }

    public bool IsRotating => AnimatorState45.GetAnimatorState(this.animator).IsRotate;

    public bool IsStopping => AnimatorState45.GetAnimatorState(this.animator).IsMovementStop;

    public bool GeometryVisible
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public void Initialize(GameObject gameObject)
    {
      this.gameObject = gameObject;
      this.pivot = gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (gameObject.name + " doesn't contain " + typeof (Pivot).Name + " unity component."), (UnityEngine.Object) gameObject);
      }
      else
      {
        this.animator = this.pivot.GetAnimator();
        if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
          return;
        this.rootmotion45 = this.animator.gameObject.GetComponent<Rootmotion45>();
        this.agent = this.pivot.GetAgent();
        if ((UnityEngine.Object) this.agent == (UnityEngine.Object) null)
        {
          Debug.LogError((object) (gameObject.name + " doesn't contain " + typeof (NavMeshAgent).Name + " unity component."), (UnityEngine.Object) gameObject);
        }
        else
        {
          this.agent.updatePosition = false;
          this.agent.updateRotation = false;
          this.rigidbody = this.pivot.GetRigidbody();
          AnimatorState45.GetAnimatorState(this.animator).StopDoneEvent += new Action(this.OnMoveDone);
          this.animator.SetLayerWeight(this.animator.GetLayerIndex("Dialog Layer"), 0.0f);
        }
      }
    }

    private float CalculateAngleInLocalSpace(Vector3 directionInWorldSpace)
    {
      Vector3 vector3 = Vector3.ProjectOnPlane(this.gameObject.transform.InverseTransformDirection(directionInWorldSpace), Vector3.up);
      return Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
    }

    public void StartMovement(Vector3 direction, EngineBehavior.GaitType gait)
    {
      if ((UnityEngine.Object) this.rootmotion45 == (UnityEngine.Object) null)
      {
        Debug.LogError((object) ("StartMovement " + this.gameObject.name + " doesn't contain " + typeof (Rootmotion45).Name + " unity component."), (UnityEngine.Object) this.gameObject);
      }
      else
      {
        AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(this.animator);
        animatorState.MovableStop = false;
        if (animatorState.IsMove && !animatorState.IsMovementStop)
          return;
        animatorState.MovableAngleStart = this.CalculateAngleInLocalSpace(direction);
        if ((double) animatorState.MovableAngleStart > 0.0)
        {
          if (gait == EngineBehavior.GaitType.Walk)
          {
            int num = (int) ((double) animatorState.MovableAngleStart / 45.0 + 0.5);
            this.rotationError = animatorState.MovableAngleStart - (float) (num * 45);
            switch (num)
            {
              case 0:
                animatorState.NextMoveIsLeft = this.rootmotion45.AngleR000ToLeft;
                break;
              case 1:
                animatorState.NextMoveIsLeft = this.rootmotion45.AngleR045ToLeft;
                break;
              case 2:
                animatorState.NextMoveIsLeft = this.rootmotion45.AngleR090ToLeft;
                break;
              case 3:
                animatorState.NextMoveIsLeft = this.rootmotion45.AngleR135ToLeft;
                break;
              case 4:
                animatorState.NextMoveIsLeft = this.rootmotion45.AngleR180ToLeft;
                break;
            }
          }
          else
          {
            int num = (int) ((double) animatorState.MovableAngleStart / 90.0 + 0.5);
            this.rotationError = animatorState.MovableAngleStart - (float) (num * 90);
            switch (num)
            {
              case 0:
                animatorState.NextMoveIsLeft = this.rootmotion45.AngleRunR000ToLeft;
                break;
              case 1:
                animatorState.NextMoveIsLeft = this.rootmotion45.AngleRunR090ToLeft;
                break;
              case 2:
                animatorState.NextMoveIsLeft = this.rootmotion45.AngleRunR180ToLeft;
                break;
            }
          }
        }
        else if (gait == EngineBehavior.GaitType.Walk)
        {
          int num = (int) (-(double) animatorState.MovableAngleStart / 45.0 + 0.5);
          this.rotationError = (float) -(-(double) animatorState.MovableAngleStart - (double) (num * 45));
          switch (num)
          {
            case 0:
              animatorState.NextMoveIsLeft = this.rootmotion45.AngleL000ToLeft;
              break;
            case 1:
              animatorState.NextMoveIsLeft = this.rootmotion45.AngleL045ToLeft;
              break;
            case 2:
              animatorState.NextMoveIsLeft = this.rootmotion45.AngleL090ToLeft;
              break;
            case 3:
              animatorState.NextMoveIsLeft = this.rootmotion45.AngleL135ToLeft;
              break;
            case 4:
              animatorState.NextMoveIsLeft = this.rootmotion45.AngleL180ToLeft;
              break;
          }
        }
        else
        {
          int num = (int) (-(double) animatorState.MovableAngleStart / 90.0 + 0.5);
          this.rotationError = (float) -(-(double) animatorState.MovableAngleStart - (double) (num * 90));
          switch (num)
          {
            case 0:
              animatorState.NextMoveIsLeft = this.rootmotion45.AngleRunL000ToLeft;
              break;
            case 1:
              animatorState.NextMoveIsLeft = this.rootmotion45.AngleRunL090ToLeft;
              break;
            case 2:
              animatorState.NextMoveIsLeft = this.rootmotion45.AngleRunL180ToLeft;
              break;
          }
        }
        this.movementSpeed = 0.0f;
        switch (gait)
        {
          case EngineBehavior.GaitType.Walk:
            this.movementSpeed = 1f;
            break;
          case EngineBehavior.GaitType.Run:
            this.movementSpeed = 2f;
            break;
        }
        animatorState.MovableSpeed = this.movementSpeed;
        this.movementDone = false;
        animatorState.ControlMovableState = AnimatorState45.MovableState45.Move;
        animatorState.VelocityScale = 1f;
      }
    }

    public bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait)
    {
      this.direction = direction;
      this.remainingDistance = remainingDistance;
      this.gait = gait;
      AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(this.animator);
      animatorState.RemainingDistance = remainingDistance;
      if (!animatorState.IsMove)
      {
        if (this.movementDone)
          return this.movementDone;
        this.StartMovement(direction, gait);
        return false;
      }
      if (animatorState.IsMovementStart)
        return this.movementDone;
      this.SetAgentSpeed(this.agent, gait);
      this.desiredMovemetSpeed = 1f;
      switch (gait)
      {
        case EngineBehavior.GaitType.Walk:
          this.desiredMovemetSpeed = 1f;
          break;
        case EngineBehavior.GaitType.Run:
          this.desiredMovemetSpeed = 2f;
          break;
      }
      if ((double) animatorState.RemainingDistance > 0.25)
        this.deltaRotation *= Quaternion.Euler(0.0f, this.CalculateRotationDeltaAngle(direction, Time.deltaTime, gait), 0.0f);
      return this.movementDone;
    }

    private void UpdateMovement()
    {
      AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(this.animator);
      if (!animatorState.IsMove)
        return;
      if (animatorState.IsMovementStart)
      {
        this.deltaRotation *= Quaternion.Euler(0.0f, this.rotationError * Time.deltaTime / animatorState.StateLength, 0.0f);
      }
      else
      {
        this.movementSpeed = Mathf.MoveTowards(this.movementSpeed, this.desiredMovemetSpeed, Time.deltaTime / 0.5f);
        animatorState.MovableSpeed = this.movementSpeed;
        if ((double) animatorState.RemainingDistance <= 0.25)
          return;
        this.deltaRotation *= Quaternion.Euler(0.0f, this.CalculateRotationDeltaAngle(this.direction, Time.deltaTime, this.gait), 0.0f);
      }
    }

    private void OnMoveDone()
    {
      this.movementDone = true;
      AnimatorState45.GetAnimatorState(this.animator).ControlMovableState = AnimatorState45.MovableState45.Idle;
    }

    private void SetAgentSpeed(NavMeshAgent agent, EngineBehavior.GaitType gait)
    {
      if ((UnityEngine.Object) agent == (UnityEngine.Object) null)
        Debug.LogWarning((object) ("gameobject " + (object) this.gameObject + " doesn`t have nav mesh agent!"));
      else if ((UnityEngine.Object) this.rootmotion45 == (UnityEngine.Object) null)
        Debug.LogWarning((object) ("gameobject " + (object) this.gameObject + " doesn`t have rootmotion45!"));
      else if (gait == EngineBehavior.GaitType.Walk)
      {
        if ((double) agent.speed == (double) this.rootmotion45.WalkSpeed)
          return;
        agent.speed = this.rootmotion45.WalkSpeed;
      }
      else
      {
        if (gait != EngineBehavior.GaitType.Run)
          throw new NotSupportedException();
        if ((double) agent.speed == (double) this.rootmotion45.RunSpeed)
          return;
        agent.speed = this.rootmotion45.RunSpeed;
      }
    }

    public bool Rotate(Vector3 direction)
    {
      AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(this.animator);
      if (!animatorState.IsRotate)
        animatorState.ControlMovableState = AnimatorState45.MovableState45.Rotate;
      Vector3 vector3 = Vector3.ProjectOnPlane(this.gameObject.transform.InverseTransformDirection(direction), Vector3.up);
      float num = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
      animatorState.MovableAngleStart = num;
      return (double) num < 25.0;
    }

    public bool Rotate(float angle)
    {
      AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(this.animator);
      animatorState.ControlMovableState = AnimatorState45.MovableState45.Rotate;
      animatorState.MovableAngleStart = (double) Mathf.Abs(angle) > 5.0 ? angle : 0.0f;
      animatorState.MovableSpeed = 0.0f;
      return (double) Mathf.Abs(angle) < 10.0;
    }

    private float CalculateRotationDeltaAngle(
      Vector3 direction,
      float dt,
      EngineBehavior.GaitType gait)
    {
      float angleInLocalSpace = this.CalculateAngleInLocalSpace(direction);
      if (gait == EngineBehavior.GaitType.Run)
      {
        float num1 = Mathf.InverseLerp(0.0f, 40f, Mathf.Abs(angleInLocalSpace));
        float num2 = Mathf.Lerp(0.0f, 1f, num1 * num1);
        return Mathf.Clamp(angleInLocalSpace / dt, -num2 * this.rotationSpeed, this.rotationSpeed * num2) * dt;
      }
      float num3 = Mathf.InverseLerp(0.0f, 90f, Mathf.Abs(angleInLocalSpace));
      float num4 = Mathf.Lerp(0.0f, 1f, num3 * num3);
      return Mathf.Clamp(angleInLocalSpace / dt, -num4 * this.rotationSpeed, num4 * this.rotationSpeed) * dt;
    }

    public void OnAnimatorMove()
    {
      float num = this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
      if (this.agent.isActiveAndEnabled && this.agent.isOnNavMesh)
      {
        this.agent.nextPosition = this.gameObject.transform.position + this.animator.deltaPosition;
        this.gameObject.transform.position = this.agent.nextPosition;
      }
      this.gameObject.transform.rotation *= Quaternion.AngleAxis(57.29578f * this.animator.angularVelocity.y * num, Vector3.up);
      this.gameObject.transform.rotation *= this.deltaRotation;
      this.deltaRotation = Quaternion.identity;
    }

    public void LateUpdate()
    {
    }

    public void Update() => this.UpdateMovement();

    public void FixedUpdate()
    {
    }
  }
}
